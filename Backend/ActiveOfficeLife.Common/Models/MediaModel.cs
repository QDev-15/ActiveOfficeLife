using ActiveOfficeLife.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Models
{
    public class MediaModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;         // Tên file gốc
        public string FileId { get; set; } = null!;         // Đường dẫn lưu file
        public string FilePath { get; set; } = null!;         // Đường dẫn lưu file
        public string FileType { get; set; } = null!;         // image/png, video/mp4, ...
        public long FileSize { get; set; }                    // Đơn vị: byte
        public MediaType MediaType { get; set; }              // Enum: Image, Video, Document, etc.
        public DateTime UploadedAt { get; set; }

        // Optional: Người upload (nếu cần)
        public Guid? UploadedByUserId { get; set; }
        public UserModel? UploadedBy { get; set; }
    }
}
