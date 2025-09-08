using ActiveOfficeLife.Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;                                                                                                                                                                                
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IStorageService
    {
        Task<MediaModel?> UploadFileAsync(IFormFile file, string settingId, string userId);
        Task<bool> DeleteFileAsync(string mediaId, string settingId);
        Task<MediaModel> GetFileUrlAsync(string mediaId);
        Task<Stream> DownloadFileAsync(string mediaId, string settingId);

    }
}
                                                                                                                                                    