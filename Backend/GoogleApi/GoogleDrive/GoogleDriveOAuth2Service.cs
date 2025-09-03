using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleApi.GoogleDrive
{
    public class GoogleDriveOAuth2Service
    {

        private readonly string _credentialPath;
        private readonly string _tokenPath;

        public GoogleDriveOAuth2Service(string credentialPath, string tokenPath = "google-oauth-token.json")
        {
            _credentialPath = credentialPath;
            _tokenPath = tokenPath;
        }

        /// <summary>
        /// Build authorization URL
        /// </summary>
        public string GetAuthorizationUrl(string redirectUri, string state = null)
        {
            using var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);
            var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.ScopeConstants.DriveFile }
            });

            var url = flow.CreateAuthorizationCodeRequest(redirectUri);
            if (!string.IsNullOrEmpty(state))
                url.State = state;

            return url.Build().AbsoluteUri;
        }

        /// <summary>
        /// Exchange code to token and save
        /// </summary>
        public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string redirectUri, string userId = "user")
        {
            using var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);
            var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.ScopeConstants.DriveFile }
            });

            var token = await flow.ExchangeCodeForTokenAsync(userId, code, redirectUri, CancellationToken.None);

            // Save to file
            var json = JsonConvert.SerializeObject(token, Formatting.Indented);
            File.WriteAllText(_tokenPath, json);

            return token;
        }

        /// <summary>
        /// Load token (refresh nếu hết hạn)
        /// </summary>
        private async Task<TokenResponse> LoadTokenAsync()
        {
            if (!File.Exists(_tokenPath))
                throw new Exception("Token file not found. Please authorize again.");

            var json = File.ReadAllText(_tokenPath);
            var token = JsonConvert.DeserializeObject<TokenResponse>(json);

            using var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);
            var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.ScopeConstants.DriveFile }
            });

            if (token.IsExpired(SystemClock.Default))
            {
                await flow.RefreshTokenAsync("user", token.RefreshToken, CancellationToken.None);
                var newJson = JsonConvert.SerializeObject(token, Formatting.Indented);
                File.WriteAllText(_tokenPath, newJson);
            }

            return token;
        }
        private async Task<UserCredential> GetUserCredentialAsync()
        {
            if (!File.Exists(_tokenPath))
                throw new Exception("Token file not found. Please authorize again.");

            var json = File.ReadAllText(_tokenPath);
            var token = JsonConvert.DeserializeObject<TokenResponse>(json);

            using var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);
            var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { DriveService.Scope.DriveFile }
            });

            var credential = new UserCredential(flow, "user", token);

            // Nếu access token đã hết hạn, refresh lại
            if (credential.Token.IsExpired(SystemClock.Default))
            {
                await credential.RefreshTokenAsync(CancellationToken.None);

                // Lưu lại token mới
                var newJson = JsonConvert.SerializeObject(credential.Token, Formatting.Indented);
                File.WriteAllText(_tokenPath, newJson);
            }

            return credential;
        }
        /// <summary>
        /// Create DriveService instance
        /// </summary>
        public async Task<DriveService> GetDriveServiceAsync()
        {
            var token = await LoadTokenAsync();

            var credential = await GetUserCredentialAsync();
            //var credential = GoogleCredential.FromAccessToken(token.AccessToken);

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "ActiveOfficeLife"
            });
        }

        /// <summary>
        /// Upload file and make it public
        /// </summary>
        public async Task<string> UploadFileAndMakePublicAsync(IFormFile file, string folderId)
        {
            var driveService = await GetDriveServiceAsync();

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

            return $"https://drive.google.com/uc?id={fileId}";
        }

        /// <summary>
        /// Download file
        /// </summary>
        public async Task<Stream> DownloadFileAsync(string fileId)
        {
            var driveService = await GetDriveServiceAsync();
            var request = driveService.Files.Get(fileId);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Position = 0;
            return stream;
        }
        /// <summary>
        /// Delete file by Id
        /// </summary>
        public async Task DeleteFileAsync(string fileId)
        {
            var driveService = await GetDriveServiceAsync();
            await driveService.Files.Delete(fileId).ExecuteAsync();
        }
    }

}
