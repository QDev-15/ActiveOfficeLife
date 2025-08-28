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
        private string GetMimeType(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();

            return ext switch
            {
                // Văn bản
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".tsv" => "text/tab-separated-values",
                ".html" => "text/html",
                ".htm" => "text/html",
                ".xml" => "application/xml",
                ".json" => "application/json",

                // Ảnh
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tif" or ".tiff" => "image/tiff",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",

                // Audio
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".ogg" => "audio/ogg",
                ".m4a" => "audio/mp4",

                // Video
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                ".avi" => "video/x-msvideo",
                ".wmv" => "video/x-ms-wmv",
                ".flv" => "video/x-flv",
                ".mkv" => "video/x-matroska",
                ".webm" => "video/webm",

                // Microsoft Office
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",

                // PDF / Ebook
                ".pdf" => "application/pdf",
                ".epub" => "application/epub+zip",
                ".mobi" => "application/x-mobipocket-ebook",

                // Compressed
                ".zip" => "application/zip",
                ".rar" => "application/vnd.rar",
                ".7z" => "application/x-7z-compressed",
                ".gz" => "application/gzip",
                ".tar" => "application/x-tar",

                // Code
                ".js" => "application/javascript",
                ".css" => "text/css",
                ".java" => "text/x-java-source",
                ".c" => "text/x-c",
                ".cpp" => "text/x-c++",
                ".cs" => "text/plain", // có thể thay bằng "text/x-csharp"
                ".py" => "text/x-python",
                ".php" => "application/x-httpd-php",
                ".sql" => "application/sql",

                // Default
                _ => "application/octet-stream"
            };
        }





        /////// OLD IMPLEMENTATION
        //private readonly DriveService _driveService;
        //public GoogleDriveOAuth2Service(string credentialPath, string tokenPath = "oauthclient-credentials-token", string appName = "MyWebApp")
        //{
        //    using var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read);

        //    var credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tokenPath); // nơi lưu refresh token
        //    var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        GoogleClientSecrets.FromStream(stream).Secrets,
        //        new[] { DriveService.ScopeConstants.DriveFile },
        //        "user",
        //        CancellationToken.None,
        //        new FileDataStore(credPath, true)).Result;

        //    _driveService = new DriveService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = appName
        //    });
        //}


        ///// <summary>
        ///// Upload 1 file lên Google Drive và trả về URL public
        ///// </summary>
        //public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderId = null)
        //{
        //    var fileMetaData = new Google.Apis.Drive.v3.Data.File()
        //    {
        //        Name = fileName,
        //        Parents = folderId != null ? new[] { folderId } : null
        //    };

        //    var request = _driveService.Files.Create(fileMetaData, fileStream, contentType);
        //    request.Fields = "id";
        //    await request.UploadAsync();

        //    var fileId = request.ResponseBody.Id;

        //    // Set quyền public
        //    var permission = new Google.Apis.Drive.v3.Data.Permission()
        //    {
        //        Role = "reader",
        //        Type = "anyone"
        //    };
        //    await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

        //    return $"https://drive.google.com/uc?export=view&id={fileId}";
        //}

        ///// <summary>
        ///// Download file từ Google Drive
        ///// </summary>
        //public async Task<Stream> DownloadFileAsync(string fileId)
        //{
        //    var request = _driveService.Files.Get(fileId);
        //    var stream = new MemoryStream();
        //    await request.DownloadAsync(stream);
        //    stream.Position = 0;
        //    return stream;
        //}
    }

}
