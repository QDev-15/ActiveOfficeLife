using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi.GoogleDrive
{
    public class GoogleDriveAccountService
    {
        private readonly DriveService _driveService;

        public GoogleDriveAccountService(string credentialPath)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(DriveService.ScopeConstants.Drive);
            }

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "AOL Attachment Service",
            });
        }

        /// <summary>
        /// Upload file lên Google Drive
        /// </summary>
        public async Task<string> UploadFileAsync(string filePath, string folderId)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(filePath),
                Parents = new[] { folderId } // upload vào folder được share
            };

            using var stream = new FileStream(filePath, FileMode.Open);
            var request = _driveService.Files.Create(fileMetadata, stream, "application/octet-stream");
            request.Fields = "id, webViewLink, webContentLink";
            await request.UploadAsync();

            var file = request.ResponseBody;
            return file.WebViewLink; // link để view file
        }
        public async Task<string> UploadFileAndMakePublicAsync(string filePath, string folderId)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(filePath),
                Parents = new[] { folderId }
            };

            using var stream = new FileStream(filePath, FileMode.Open);
            var request = _driveService.Files.Create(fileMetadata, stream, "application/octet-stream");
            request.Fields = "id, webViewLink, webContentLink";
            var progress = await request.UploadAsync();
            if (progress.Status != Google.Apis.Upload.UploadStatus.Completed)
            {
                throw new Exception($"Upload failed: {progress.Exception?.Message}");
            }

            var file = request.ResponseBody;

            // Cấp quyền public
            await MakeFilePublicAsync(file.Id);

            // Lấy lại file để có link public
            var uploadedFile = await _driveService.Files.Get(file.Id).ExecuteAsync();
            return uploadedFile.WebContentLink; // link public (direct download)
        }

        /// <summary>
        /// Download file từ Google Drive
        /// </summary>
        public async Task<MemoryStream> DownloadFileAsync(string fileId)
        {
            var request = _driveService.Files.Get(fileId);

            var stream = new MemoryStream();
            await request.DownloadAsync(stream);

            stream.Position = 0; // reset về đầu stream để đọc lại

            return stream;
        }

        /// <summary>
        /// Tạo link public cho file (Anyone with link can view)
        /// </summary>
        public async Task<string> MakeFilePublicAsync(string fileId)
        {
            var permission = new Permission()
            {
                Type = "anyone",
                Role = "reader"
            };

            await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

            var file = await _driveService.Files.Get(fileId).ExecuteAsync();
            return file.WebContentLink; // direct download link
        }
    }
}
