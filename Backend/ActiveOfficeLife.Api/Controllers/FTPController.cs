using ActiveOfficeLife.Application.Interfaces;
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
        private readonly AppConfigService _appConfigService;

        private readonly IStorageService _storageService;
        public FTPController(IWebHostEnvironment env, AppConfigService appConfigService, IStorageService storageService)
        {
            _storageService = storageService;
            _appConfigService = appConfigService;
            
            //_googleAccountDrive = new GoogleDriveAccountService(accountCredentialPath);
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

            
            return sreamResult == null ? NotFound() : Ok(sreamResult);
        }

        #region Upload Methods

        private async Task<string> UploadToAccountDrive(IFormFile file)
        {
            var folderId = "";
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
