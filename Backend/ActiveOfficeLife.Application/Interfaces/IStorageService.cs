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
        #region Local Storage
        Task<MediaModel> GetFileUrlAsync(string mediaId);
        Task<MediaModel?> UploadFileAsync(IFormFile file, string settingId, string userId);
        Task<Stream> DownloadFileAsync(string mediaId, string settingId);
        Task<bool> DeleteFileAsync(string mediaId, string settingId);
        #endregion

        #region Google Drive
        string GetFileUrlGoogleDriveAsync(string fileId);
        string GetFileUrlDownloadGoogleDriveAsync(string fileId);
        Task<bool> DeleteFileGoogleDriveAsync(string mediaId, string settingId);
        Task<MediaModel?> UploadFileToGoogleDriveAsync(IFormFile file, string settingId, string userId);
        Task<Stream> DownloadFileFromGoogleDriveAsync(string fileId, string settingId);
        #endregion

    }
}
                                                                                                                                                    