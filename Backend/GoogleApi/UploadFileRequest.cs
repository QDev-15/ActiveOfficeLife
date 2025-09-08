using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GoogleApi
{
    public class UploadFileRequest
    {
        public string settingId { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
