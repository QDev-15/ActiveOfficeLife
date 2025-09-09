using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using GoogleApi;
using Microsoft.AspNetCore.Mvc;

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
            string settingId = User?.Claims?.FirstOrDefault(c => c.Type == "SettingId")?.Value;
            string userId = User?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(request.settingId))
            {
                request.settingId = settingId;
            }
            var media = await _storageService.UploadFileToGoogleDriveAsync(request.File, request.settingId, userId);

            return Ok(media);
        }

        /// <summary>
        /// download file from Google Drive by mediaId
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        [HttpGet("download/googledrive/{mediaId}")]
        public async Task<IActionResult> DownloadFromGoogleDrive(string mediaId, string orgId)
        {
            // get settingid from claims field "SettingId"
            string settingId = User?.Claims?.FirstOrDefault(c => c.Type == "SettingId")?.Value;
            if (string.IsNullOrEmpty(settingId))
            {
                settingId = orgId;
            }
            var sreamResult = await _storageService.DownloadFileFromGoogleDriveAsync(mediaId, settingId);
            return sreamResult == null ? NotFound() : Ok(sreamResult);
        }
    }
}
