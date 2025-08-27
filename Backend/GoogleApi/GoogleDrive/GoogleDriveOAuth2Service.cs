using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace GoogleApi.GoogleDrive
{
    public class GoogleDriveOAuth2Service
    {
        private readonly DriveService _driveService;

        public GoogleDriveOAuth2Service(string credentialPath, string tokenPath = "oauthclient-credentials-token", string appName = "MyWebApp")
        {
            using var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read);

            var credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tokenPath); // nơi lưu refresh token
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { DriveService.ScopeConstants.DriveFile },
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });
        }

        /// <summary>
        /// Upload 1 file lên Google Drive và trả về URL public
        /// </summary>
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderId = null)
        {
            var fileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = folderId != null ? new[] { folderId } : null
            };

            var request = _driveService.Files.Create(fileMetaData, fileStream, contentType);
            request.Fields = "id";
            await request.UploadAsync();

            var fileId = request.ResponseBody.Id;

            // Set quyền public
            var permission = new Google.Apis.Drive.v3.Data.Permission()
            {
                Role = "reader",
                Type = "anyone"
            };
            await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

            return $"https://drive.google.com/uc?export=view&id={fileId}";
        }

        /// <summary>
        /// Download file từ Google Drive
        /// </summary>
        public async Task<Stream> DownloadFileAsync(string fileId)
        {
            var request = _driveService.Files.Get(fileId);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Position = 0;
            return stream;
        }
    }

}
