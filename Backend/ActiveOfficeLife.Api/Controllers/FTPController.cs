using ActiveOfficeLife.Application.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using GoogleApi;
using GoogleApi.GoogleDrive;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Newtonsoft.Json;
using System.IO;

namespace ActiveOfficeLife.Api.Controllers
{
    public class FTPController : BaseController
    {
        private readonly GoogleDriveOAuth2Service _googleOAuthClientDrive;
        private readonly GoogleDriveAccountService _googleAccountDrive;
        private readonly AppConfigService _appConfigService;

        private const string RedirectUriLocal = "https://localhost:7029/api/ftp/callback";
        private const string RedirectUriServer = "https://api.aol.tkid.io.vn/api/ftp/callback";
        private const string FolderId = "16zz3OdwYgz4HlSBvtm9KRSaSU6qo91C-";
        public FTPController(IWebHostEnvironment env, AppConfigService appConfigService)
        {
            _appConfigService = appConfigService;
            var accountCredentialPath = Path.Combine(env.ContentRootPath, _appConfigService.AppConfigs.GoogleDriveAPI.AccountCredentialFileName);
            var oAuthClientCredentialPath = Path.Combine(env.ContentRootPath, _appConfigService.AppConfigs.GoogleDriveAPI.OAuthClienCredentialFileName);
            _googleOAuthClientDrive = new GoogleDriveOAuth2Service(oAuthClientCredentialPath);
            //_googleAccountDrive = new GoogleDriveAccountService(accountCredentialPath);
        }
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login()
        {
            // Khi chạy local thì dùng RedirectUriLocal, khi chạy server thì dùng RedirectUriServer
            var url = _googleOAuthClientDrive.GetAuthorizationUrl(RedirectUriLocal);
            return Redirect(url);
        }
        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Missing code");

            await _googleOAuthClientDrive.ExchangeCodeForTokenAsync(code, $"{Request.Scheme}://{Request.Host}/ftp/callback");

            return Ok("Google OAuth2 Login Success! Token saved.");
        }
        

       
        // upload file to web api using form-data with key 'file'
        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // rất quan trọng cho Swagger hiểu đây là form-data
        public IActionResult UploadToLocal([FromForm] UploadFileRequest request)
        {
            if (request.File == null || request.File.Length == 0) return BadRequest("File empty");
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filePath = Path.Combine(folderPath, request.File.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                request.File.CopyTo(stream);
            }
            var fileUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{request.File.FileName}";
            return Ok(new { Url = fileUrl });
        }
        // download file from local server by filename
        [HttpGet("download/{fileName}")]
        public IActionResult DownloadFromLocal(string fileName)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
            var filePath = Path.Combine(folderPath, fileName);
            if (!System.IO.File.Exists(filePath)) return NotFound();
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;
            var contentType = "APPLICATION/octet-stream";
            return File(memory, contentType, fileName);
        }
        /// <summary>
        /// upload file to Google Drive
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("upload/googledrive")]
        [Consumes("multipart/form-data")] // rất quan trọng cho Swagger hiểu đây là form-data
        public async Task<IActionResult> UploadToGoogleDrive([FromForm] UploadFileRequest request)
        {
            if (request.File == null || request.File.Length == 0) return BadRequest("File empty");
            string urlResult = string.Empty;
            //if (_appConfigService.AppConfigs.GoogleDriveAPI.AccountService)
            //{
            //    urlResult = await UploadToAccountDrive(request.File);
            //}
            if (_appConfigService.AppConfigs.GoogleDriveAPI.OAuthClientService)
            {
                urlResult = await UploadToOAuthClientDrive(request.File);
            }
            return Ok(new { Url = urlResult });
        }

        /// <summary>
        /// download file from Google Drive by fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("download/googledrive/{fileId}")]
        public async Task<IActionResult> DownloadFromGoogleDrive(string fileId)
        {
            Stream sreamResult = null;
            if (_appConfigService.AppConfigs.GoogleDriveAPI.AccountService)
            {
                sreamResult = await DownloadFromAccountDrive(fileId);
            }
            else if (_appConfigService.AppConfigs.GoogleDriveAPI.OAuthClientService)
            {
                sreamResult = await DownloadFromOAuthClientDrive(fileId);
            }
            
            return sreamResult == null ? NotFound() : Ok(sreamResult);
        }

        #region Upload Methods

        private async Task<string> UploadToAccountDrive(IFormFile file)
        {
            var folderId = _appConfigService.AppConfigs.GoogleDriveAPI.FolderID; // ID của folder đã share với service account
            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var viewLink = await _googleAccountDrive.UploadFileAndMakePublicAsync(filePath, folderId);

            System.IO.File.Delete(filePath);
            return viewLink;
        }
        private async Task<string> UploadToOAuthClientDrive(IFormFile file)
        {
            var url = await _googleOAuthClientDrive.UploadFileAndMakePublicAsync(file, FolderId);

            return url;
        }
        #endregion

        #region Download Methods
        private async Task<Stream> DownloadFromAccountDrive(string fileId)
        {
            var stream = await _googleAccountDrive.DownloadFileAsync(fileId);
            return stream;
        }
        private async Task<Stream> DownloadFromOAuthClientDrive(string fileId)
        {
            var stream = await _googleOAuthClientDrive.DownloadFileAsync(fileId);
            return stream;
        }
        #endregion
    }
}
