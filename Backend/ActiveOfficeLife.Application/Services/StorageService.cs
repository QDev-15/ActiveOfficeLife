using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Google.Apis.Auth.OAuth2;
using GoogleApi.Interfaces;
using Microsoft.AspNetCore.Http;
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
        private readonly _IUnitOfWork _unitOfWork;
        public StorageService(AppConfigService appConfigService, ISettingRepository settingRepository,
            IGoogleDriveInterface googleDriveInterface, IMediaRepository mediaRepository, _IUnitOfWork unitOfWork)
        {
            _appConfigService = appConfigService ?? throw new ArgumentNullException(nameof(appConfigService));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _googleDriveInterface = googleDriveInterface ?? throw new ArgumentNullException(nameof(googleDriveInterface));
            _mediaRepository = mediaRepository;
            _unitOfWork = unitOfWork;
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

        public async Task<MediaModel> GetFileUrlAsync(string mediaId)
        {
            Guid.TryParse(mediaId, out Guid mediaGuidId);
            var media = await _mediaRepository.GetByIdAsync(mediaGuidId);
            return media.ReturnModel();
        }

        public async Task<MediaModel?> UploadFileAsync(IFormFile file, string settingId, string userId = "")
        {
            Guid.TryParse(settingId, out Guid settingGuid);
            var setting = await _settingRepository.GetByIdAsync(settingGuid);
            var media = new Media();
            if (setting == null)
            {
                throw new Exception("Setting not found");
            }
            if (_appConfigService.AppConfigs.StorageConfig.SaveToGoogle)
            {
                var uploader = await _googleDriveInterface.UploadFileAndMakePublicAsync(file, setting.GoogleFolderId, setting.GoogleToken, new ClientSecrets()
                {
                    ClientId = setting.GoogleClientId,
                    ClientSecret = setting.GoogleClientSecretId
                }, _appConfigService.AppConfigs.ApplicationName);
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
            return null;
        }
    }
}
