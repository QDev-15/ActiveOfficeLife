using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi.Responses
{
    public class UpLoadResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string FileId { get; set; } = string.Empty;
        public string FileLink { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string TokenRefreshed { get; set; } = string.Empty;
    }
}
