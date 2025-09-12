using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingRepository _settingRepository;
        public SettingService(IUnitOfWork unitOfWork, ISettingRepository settingRepository)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = settingRepository;
        }
        public async Task<SettingModel> Create(SettingModel setting)
        {
            Setting newSetting = new Setting
            {
                Id = Guid.NewGuid(),
                Name = setting.Name,
                Address = setting.Address,
                Email = setting.Email,
                GoogleClientId = setting.GoogleClientId,
                GoogleClientSecretId = setting.GoogleClientSecretId,
                GoogleFolderId = setting.GoogleFolderId,
                GoogleToken = setting.GoogleToken,
                Logo = setting.Logo,
                PhoneNumber = setting.PhoneNumber
            };
            await _settingRepository.AddAsync(newSetting);
            await _unitOfWork.SaveChangesAsync();
            return newSetting.ReturnModel();
        }
        public async Task<SettingModel> Update(SettingModel updateModel)
        {
            if (updateModel == null)
                throw new ArgumentNullException(nameof(updateModel));
            var setting = await _settingRepository.GetByIdAsync(updateModel.Id);
            if (setting == null)
                throw new Exception("Setting not found");
            if (!string.IsNullOrWhiteSpace(updateModel.Name))
            {
                // Check if the new name is unique
                if (await _settingRepository.IsExistsSettingName(setting.Id, updateModel.Name))
                {
                    throw new Exception("Setting name already exists");
                }
                setting.Name = updateModel.Name;
            }
            setting.Address = updateModel.Address;
            setting.Email = updateModel.Email;
            setting.GoogleClientId = updateModel.GoogleClientId;
            setting.GoogleClientSecretId = updateModel.GoogleClientSecretId;
            setting.GoogleFolderId = updateModel.GoogleFolderId;
            setting.GoogleToken = updateModel.GoogleToken;
            setting.Logo = updateModel.Logo;
            setting.PhoneNumber = updateModel.PhoneNumber;
            _settingRepository.Update(setting);
            await _unitOfWork.SaveChangesAsync();
            return setting.ReturnModel();
        }
        public async Task<SettingModel> GetById(Guid id)
        {
            var setting = await _settingRepository.GetByIdAsync(id);
            if (setting == null)
                throw new Exception("Setting not found");
            return setting.ReturnModel();
        }

        public async Task<SettingModel> GetDefault(string? id)
        {
            var setting = new Setting();
            if (string.IsNullOrEmpty(id))
            {
                setting = await _settingRepository.GetSettingDefault();
            }
            else
            {
                var guidId = Guid.Parse(id);
                setting = await _settingRepository.GetSettingById(guidId);
            }
            return setting.ReturnModel();
        }

    }
}
