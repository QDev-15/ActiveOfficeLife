﻿using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Domain;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly _IUnitOfWork _unitOfWord;
        private readonly IMemoryCache _memoryCache;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, _IUnitOfWork unitOfWord, IMemoryCache memoryCache)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWord = unitOfWord;
            _memoryCache = memoryCache;
        }
        public async Task<UserModel> GetUser(Guid id)
        {
            var cacheKey = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}:{id}";

            if (_memoryCache.TryGetValue(cacheKey, out UserModel cachedUser))
            {
                return cachedUser;
            }
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user != null)
                {
                    _memoryCache.Set(cacheKey, user.ReturnModel(), TimeSpan.FromMinutes(30)); // TTL 30 phút
                    return user.ReturnModel();
                } else
                {
                    AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - Id = " + id.ToString() + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            } catch(Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        
        public async Task<UserModel> Create(RegisterRequest registerRequest)
        {
            try
            {
                if (registerRequest.Username.Length < 6 || registerRequest.Password.Length < 6)
                {
                    throw new Exception("The username and password must each be at least 6 characters long.");
                }

                var roles = await _roleRepository.GetAllAsync();
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    Username = registerRequest.Username,
                    PasswordHash = DomainHelper.HashPassword(registerRequest.Password),
                    CreatedAt = DateTime.UtcNow,
                    Email = registerRequest.Email,
                    UpdatedAt = DateTime.UtcNow,
                    Status = UserStatus.Active,
                    Roles = new List<Role>()
                };
                if (roles.Any())
                {
                    foreach (var item in roles)
                    {
                        if (item.Name.ToLower() == "comment")
                        {
                            user.Roles.Add(item);
                        }
                    }
                }
                await _userRepository.AddAsync(user);
                await _unitOfWord.SaveChangesAsync();
                return user.ReturnModel();
            } catch(Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserModel> Update(UserModel model)
        {
            try
            {
                if (model == null || model.Id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user model.");
                }
                var user = await _userRepository.GetByIdAsync(model.Id);
                if (user == null)
                {
                    AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - Id = " + model.Id.ToString() + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
                user.Username = model.Username;
                user.Email = model.Email;
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    user.PasswordHash = DomainHelper.HashPassword(model.PasswordHash);
                }
                if (!string.IsNullOrEmpty(model.AvatarUrl))
                {
                    user.AvatarUrl = model.AvatarUrl;
                }
                if (model.Roles != null && model.Roles.Any())
                {
                    user.Roles.Clear();
                    var roles = await _roleRepository.GetAllAsync();
                    foreach (var roleName in model.Roles)
                    {
                        var role = roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
                        if (role != null)
                        {
                            user.Roles.Add(role);
                        }
                    }
                }
                user.Status = model.Status;
                user.UpdatedAt = DateTime.UtcNow;
                _userRepository.UpdateAsync(user);
                await _unitOfWord.SaveChangesAsync();
                return user.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception("Update failed");
            }
        }
        public async Task<UserModel> GetByRefreshToken(string refreshToken)
        {
            try
            {
                var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
                if (user != null)
                {
                    return user.ReturnModel();
                }
                else
                {
                    AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - refreshToken = " + refreshToken + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            }
            catch (Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user ID.");
                }
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - Id = " + id.ToString() + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
                user.Status = UserStatus.Deleted; // Cập nhật trạng thái người dùng thành Deleted
                _userRepository.UpdateAsync(user);
                await _unitOfWord.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserModel> GetByToken(string token)
        {
            try
            {
                var user = await _userRepository.GetByTokenAsync(token);
                if (user != null)
                {
                    return user.ReturnModel();
                }
                else
                {
                    AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - token = " + token + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            }
            catch (Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
