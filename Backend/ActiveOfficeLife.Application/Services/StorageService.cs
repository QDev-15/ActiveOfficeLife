using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Google.Apis.Auth.OAuth2;
using GoogleApi.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class StorageService : IStorageService
    {
        private readonly AppConfigService _appConfigService;
        private readonly ISettingRepository _settingRepository;
        private readonly IGoogleDriveInterface _googleDriveInterface;
        private readonly IMediaRepository _mediaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        public StorageService(AppConfigService appConfigService, ISettingRepository settingRepository, IWebHostEnvironment env,
            IGoogleDriveInterface googleDriveInterface, IMediaRepository mediaRepository, IUnitOfWork unitOfWork)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _appConfigService = appConfigService ?? throw new ArgumentNullException(nameof(appConfigService));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _googleDriveInterface = googleDriveInterface ?? throw new ArgumentNullException(nameof(googleDriveInterface));
            _mediaRepository = mediaRepository;
            _unitOfWork = unitOfWork;
        }
        #region Local Storage
        public async Task<MediaModel> GetFileUrlAsync(string mediaId)
        {
            Guid.TryParse(mediaId, out Guid mediaGuidId);
            var media = await _mediaRepository.GetByIdAsync(mediaGuidId);
            return media.ReturnModel();
        }
        public async Task<MediaModel?> UploadFileAsync(IFormFile file, string settingId, string userId = "")
        {
            // Lấy cấu hình lưu local
            var opts = _appConfigService.AppConfigs.FileStorage;
            var contentRoot = _env.ContentRootPath;  // inject: IWebHostEnvironment _env
            var rootPath = Path.IsPathRooted(opts.RootPath)
                ? opts.RootPath
                : Path.Combine(contentRoot, opts.RootPath);

            if (file == null || file.Length == 0)
                throw new Exception("File rỗng.");

            // (Tuỳ bạn) vẫn giữ logic setting nếu cần, nhưng không bắt buộc cho local:
            Guid.TryParse(settingId, out Guid settingGuid);
            var setting = await _settingRepository.GetByIdAsync(settingGuid);
            if (setting == null)
                throw new Exception("Setting not found");

            // Tạo thư mục con theo ngày (tuỳ opts)
            var subFolder = opts.UseDateSubfolders
                ? Path.Combine(DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"), DateTime.UtcNow.ToString("dd"))
                : string.Empty;

            var saveDir = string.IsNullOrEmpty(subFolder) ? rootPath : Path.Combine(rootPath, subFolder);
            Directory.CreateDirectory(saveDir);

            // Làm sạch và đặt tên file duy nhất
            var safeName = Helper.MakeSafeFileName(file.FileName);
            var ext = Path.GetExtension(safeName);
            var uniqueName = $"{Path.GetFileNameWithoutExtension(safeName)}_{Guid.NewGuid():N}{ext}";
            var physicalPath = Path.Combine(saveDir, uniqueName);

            // Lưu file
            using (var stream = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 64 * 1024, useAsync: true))
            {
                await file.CopyToAsync(stream);
            }

            // Suy luận content-type
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(uniqueName, out var contentType))
                contentType = "application/octet-stream";

            // Tạo URL public
            var urlPath = string.IsNullOrEmpty(subFolder)
                ? $"{opts.BaseUrl}/{uniqueName}".Replace("\\", "/")
                : $"{opts.BaseUrl}/{subFolder.Replace("\\", "/")}/{uniqueName}";

            var media = new Media
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FileId = uniqueName,            // hoặc để physicalPath nếu bạn muốn
                FilePath = urlPath,             // URL public để client truy cập
                FileType = contentType,
                MediaType = Helper.GuessMediaType(contentType, ext),
                UploadedAt = DateTime.UtcNow
            };

            Guid.TryParse(userId, out Guid userGuid);
            media.UploadedByUserId = userGuid;

            await _mediaRepository.AddAsync(media);
            await _unitOfWork.SaveChangesAsync();

            return media.ReturnModel();
        }  
        public async Task<Stream> DownloadFileAsync(string mediaId, string settingId)
        {
            Guid.TryParse(settingId, out Guid settingGuid);
            Guid.TryParse(mediaId, out Guid mediaGuidId);
            var setting = await _settingRepository.GetByIdAsync(settingGuid);
            var media = await _mediaRepository.GetByIdAsync(mediaGuidId);
            if (setting == null)
            {
                throw new Exception("Setting not found");
            }
            if (media == null)
            {
                throw new Exception("Media not found");
            }
            if (_appConfigService.AppConfigs.StorageConfig.SaveToGoogle)
            {
                var stream = await _googleDriveInterface.DownloadFileAsync(media.FileId, setting.GoogleToken, new ClientSecrets()
                {
                    ClientId = setting.GoogleClientId,
                    ClientSecret = setting.GoogleClientSecretId
                }, _appConfigService.AppConfigs.ApplicationName);
                return stream;
            }
            return null;
        }
        public async Task<bool> DeleteFileAsync(string mediaId, string settingId)
        {
            Guid.TryParse(settingId, out Guid settingGuid);
            Guid.TryParse(mediaId, out Guid mediaGuidId);
            var setting = await _settingRepository.GetByIdAsync(settingGuid);
            var media = await _mediaRepository.GetByIdAsync(mediaGuidId);
            if (setting == null)
            {
                throw new Exception("Setting not found");
            }
            if (media == null)
            {
                throw new Exception("Media not found");
            }
            if (_appConfigService.AppConfigs.StorageConfig.SaveToGoogle)
            {
                var result = await _googleDriveInterface.DeleteFileAsync(media.FileId, setting.GoogleToken, new ClientSecrets()
                {
                    ClientId = setting.GoogleClientId,
                    ClientSecret = setting.GoogleClientSecretId
                }, _appConfigService.AppConfigs.ApplicationName);
                if (result.IsSuccess)
                {
                    _mediaRepository.Remove(media);
                    await _unitOfWork.SaveChangesAsync();
                }
                return result.IsSuccess;
            }
            return false;
        }

        #endregion

        #region Google Drive

        public string GetFileUrlDownloadGoogleDriveAsync(string fileId)
        {
            return $"https://drive.google.com/uc?id={fileId}&export=download";
        }
        public string GetFileUrlGoogleDriveAsync(string fileId)
        {
            return $"https://drive.google.com/uc?id={fileId}";
        }
        public async Task<MediaModel?> UploadFileToGoogleDriveAsync(IFormFile file, string settingId, string userId)
        {
            try
            {
                // max size 10MB
                var maxSize = _appConfigService.AppConfigs.FileStorage.MaxFileSizeMB * 1024 * 1024;
                if (file == null || file.Length == 0)
                    throw new Exception("File rỗng.");
                if (file.Length > maxSize)
                    throw new Exception("File quá lớn. Vui lòng chọn file nhỏ hơn 10MB.");
                var setting = new Setting();
                Guid.TryParse(settingId, out Guid settingGuid);
                // check settingGuid is empty
                if (settingGuid == Guid.Empty)
                {
                    setting = await _settingRepository.GetSettingDefault();
                } else
                {
                    setting = await _settingRepository.GetByIdAsync(settingGuid);
                }

                var media = new Media();
                if (setting == null)
                {
                    throw new Exception("Setting not found");
                }
                if (setting.GoogleToken == null)
                {
                    throw new Exception("Google drive disconnected");
                }
                // check token expire
                var tokenExpire = await _googleDriveInterface.CheckIsExpiredToken(setting.GoogleToken, new ClientSecrets()
                {
                    ClientId = setting.GoogleClientId,
                    ClientSecret = setting.GoogleClientSecretId
                });
                if (tokenExpire)
                {
                    var refreshToken = await _googleDriveInterface.RefreshToken(setting.GoogleToken, new ClientSecrets()
                    {
                        ClientId = setting.GoogleClientId,
                        ClientSecret = setting.GoogleClientSecretId
                    });
                    setting.GoogleToken = refreshToken.ConvertToJsonToken();
                    _settingRepository.Update(setting);
                    await _unitOfWork.SaveChangesAsync();
                }
                var uploader = await _googleDriveInterface.UploadFileAndMakePublicAsync(file, setting.GoogleFolderId, setting.GoogleToken, new ClientSecrets()
                {
                    ClientId = setting.GoogleClientId,
                    ClientSecret = setting.GoogleClientSecretId
                }, _appConfigService.AppConfigs.ApplicationName);
                if (!string.IsNullOrEmpty(uploader.TokenRefreshed))
                {
                    setting.GoogleToken = uploader.TokenRefreshed;
                }
                media.Id = Guid.NewGuid();
                media.FileName = file.FileName;
                media.FileId = uploader.FileId;
                media.FilePath = uploader.FileLink;
                media.FileType = uploader.FileType;
                media.MediaType = MediaType.Image;
                Guid.TryParse(userId, out Guid userGuid);
                media.UploadedByUserId = userGuid;
                media.UploadedAt = DateTime.UtcNow;
                await _mediaRepository.AddAsync(media);
                await _unitOfWork.SaveChangesAsync();
                return media.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<Stream> DownloadFileFromGoogleDriveAsync(string fileId, string settingId)
        {
            Guid.TryParse(settingId, out Guid settingGuid);
            var setting = await _settingRepository.GetByIdAsync(settingGuid);
            if (setting == null)
            {
                throw new Exception("Setting not found");
            }

            var stream = await _googleDriveInterface.DownloadFileAsync(fileId, setting.GoogleToken, new ClientSecrets()
            {
                ClientId = setting.GoogleClientId,
                ClientSecret = setting.GoogleClientSecretId
            }, _appConfigService.AppConfigs.ApplicationName);
            return stream;
        }
        public async Task<bool> DeleteFileGoogleDriveAsync(string fileId, string settingId)
        {
            Guid.TryParse(settingId, out Guid settingGuid);
            var medias = await _mediaRepository.GetMediaByFileId(fileId);
            var setting = await _settingRepository.GetByIdAsync(settingGuid);
            if (setting == null)
            {
                throw new Exception("Setting not found");
            }
            var result = await _googleDriveInterface.DeleteFileAsync(fileId, setting.GoogleToken, new ClientSecrets()
            {
                ClientId = setting.GoogleClientId,
                ClientSecret = setting.GoogleClientSecretId
            }, _appConfigService.AppConfigs.ApplicationName);
            if (result.IsSuccess && medias.Any())
            {
                _mediaRepository.RemoveRange(medias);
                await _unitOfWork.SaveChangesAsync();
            }
            return result.IsSuccess;
        }
        #endregion
    }
}
