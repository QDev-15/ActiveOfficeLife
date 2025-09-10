using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util;
using GoogleApi.Interfaces;
using GoogleApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi.Adapter
{
    public class GoogleDriveAdapter : IGoogleDriveInterface
    {
        public async Task<bool> CheckIsExpiredToken(string jsonToken, ClientSecrets clientSecrets)
        {
            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonToken);
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.ScopeConstants.DriveFile }
            });

            if (token != null)
            {
                return token.IsExpired(SystemClock.Default);
            }

            return false;
        }
        public TokenResponse GetToken(string jsonToken)
        {
            if (string.IsNullOrWhiteSpace(jsonToken))
            {
                throw new ArgumentException("The provided JSON token is null or empty.", nameof(jsonToken));
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonToken);
            if (token == null)
            {
                throw new InvalidOperationException("Failed to deserialize the JSON token into a TokenResponse object.");
            }

            return token;
        }
        public async Task<TokenResponse> RefreshToken(string jsonToken, ClientSecrets clientSecrets)
        {
            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonToken);
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.ScopeConstants.DriveFile }
            });

            if (token != null && token.IsExpired(SystemClock.Default))
            {
                await flow.RefreshTokenAsync("user", token.RefreshToken, CancellationToken.None);
            }

            return token;
        }
        public async Task<DriveService> GetDriveServiceAsync(string jsonToken, ClientSecrets clientSecrets, string appName = "myGoogleApplication")
        {
            var credential = await GetUserCredentialAsync(jsonToken, clientSecrets);

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });
        }

        public async Task<UserCredential> GetUserCredentialAsync(string jsonToken, ClientSecrets clientSecrets)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.Scope.DriveFile }
            });
            // check token is expired
            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonToken);
            if (token != null && token.IsExpired(SystemClock.Default))
            {
                // return error
                throw new Exception("Token is expired. Please refresh the token.");
            }

            var credential = new UserCredential(flow, "user", token);
            return credential;
        }



        public async Task<UpLoadResponse> UploadFileAndMakePublicAsync(IFormFile file, string folderId, string jsonToken, ClientSecrets clientSecrets, string appName)
        {
            // check clientSecrets is null
            if (clientSecrets == null)
            {
                throw new ArgumentNullException(nameof(clientSecrets), "Client secrets cannot be null.");
            }
            // check jsonToken is null or empty
            if (string.IsNullOrWhiteSpace(jsonToken))
            {
                throw new ArgumentException("The provided JSON token is null or empty.", nameof(jsonToken));
            }
            // check file is null
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("The provided file is null or empty.", nameof(file));
            }
            // check folderId is null or empty  
            if (string.IsNullOrWhiteSpace(folderId))
            {
                throw new ArgumentException("The provided folder ID is null or empty.", nameof(folderId));
            }
            // check appName is null or empty
            if (string.IsNullOrWhiteSpace(appName))
            {
                // set default appName
                appName = "myGoogleApplication";
            }
            // check token is expired
            var istokenExpired = await CheckIsExpiredToken(jsonToken, clientSecrets);
            string tokenRefreshed = string.Empty;
            if (istokenExpired)
            {
                var tokenRefresh = await RefreshToken(jsonToken, clientSecrets);
                tokenRefreshed = JsonConvert.SerializeObject(tokenRefresh);
                jsonToken = tokenRefreshed;
            }

            // get drive service
            var driveService = await GetDriveServiceAsync(jsonToken, clientSecrets, appName);

            var fileName = Path.GetFileName(file.FileName);
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = Guid.NewGuid().ToString();

            await using var readStream = file.OpenReadStream();

            var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? Helper.GetMimeType(fileName) : file.ContentType;

            var fileMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name = Path.GetFileName(fileName),
                Parents = new[] { folderId }
            };

            var request = driveService.Files.Create(fileMeta, readStream, contentType);
            request.Fields = "id, webViewLink, webContentLink";
            var fileUploadResponse = await request.UploadAsync();

            if (fileUploadResponse.Status != Google.Apis.Upload.UploadStatus.Completed)
                throw new Exception("Upload failed");

            var fileId = request.ResponseBody.Id;

            // Make public
            var permission = new Google.Apis.Drive.v3.Data.Permission
            {
                Role = "reader",
                Type = "anyone"
            };
            await driveService.Permissions.Create(permission, fileId).ExecuteAsync();

            var response = new UpLoadResponse
            {
                FileName = fileName,
                FileId = fileId,
                FileType = contentType,
                FileLink = $"https://drive.google.com/uc?id={fileId}",
                ErrorMessage = string.Empty,
                TokenRefreshed = tokenRefreshed
            };
            return response;
        }

        public async Task<Stream> DownloadFileAsync(string fileId, string jsonToken, ClientSecrets clientSecrets, string appName = "myGoogleApplication")
        {
            var driveService = await GetDriveServiceAsync(jsonToken, clientSecrets, appName);
            var request = driveService.Files.Get(fileId);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<ResultResponse> DeleteFileAsync(string fileId, string jsonToken, ClientSecrets clientSecrets, string appName)
        {
            try
            {
                var driveService = await GetDriveServiceAsync(jsonToken, clientSecrets, appName);
                await driveService.Files.Delete(fileId).ExecuteAsync();
                return new ResultResponse(true);
            }
            catch (Exception ex)
            {
                // log error
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return new ResultResponse(false, ex);
            }     
        }

        public string GetAuthorizationUrl(ClientSecrets clientSecrets, string redirectUri, string? state = null, string? orgId = null)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.ScopeConstants.DriveFile }
            });

            var req = flow.CreateAuthorizationCodeRequest(redirectUri);

            // Build custom state
            var stateDict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(state))
                stateDict["state"] = state;
            if (!string.IsNullOrEmpty(orgId))
                stateDict["orgId"] = orgId;

            if (stateDict.Any())
            {
                // Encode thành chuỗi query
                req.State = string.Join("&", stateDict.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            }
            // URL gốc do SDK build (đã có client_id, redirect_uri, scope, response_type, state,...)
            var baseUrl = req.Build().AbsoluteUri;

            // --- Thay vì AddQueryString (có thể tạo trùng key), ta parse & thay thế ---
            var ub = new UriBuilder(baseUrl);

            // ParseQuery trả Dictionary<string, StringValues>
            var parsed = QueryHelpers.ParseQuery(ub.Query);

            // Chuyển về Dictionary<string,string> (lấy giá trị đầu)
            var dict = parsed.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Count > 0 ? kv.Value[0] : string.Empty,
                StringComparer.OrdinalIgnoreCase
            );

            // Ghi đè/đặt các tham số cần thiết
            dict["access_type"] = "offline";
            // CHỈ dùng một tham số "prompt" (không gửi kèm approval_prompt để tránh mâu thuẫn)
            dict["prompt"] = "consent";
            // Nếu trước đó có approval_prompt thì bỏ đi để không gửi hai kiểu prompt
            dict.Remove("approval_prompt");

            // Rebuild query string an toàn
            var newQuery = string.Join("&",
                dict.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value ?? string.Empty)}"));

            ub.Query = newQuery;
            var finalUrl = ub.Uri.ToString();
            return finalUrl;
        }

        public async Task<TokenResponse> ExchangeCodeForTokenAsync(ClientSecrets clientSecrets, string code, string redirectUri, string userId = "user")
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.ScopeConstants.DriveFile }
            });

            var token = await flow.ExchangeCodeForTokenAsync(userId, code, redirectUri, CancellationToken.None);

            return token;
        }
    }
}
