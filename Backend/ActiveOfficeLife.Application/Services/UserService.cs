using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace ActiveOfficeLife.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly _IUnitOfWork _unitOfWord;
        private readonly IMemoryCache _memoryCache;
        private readonly AppConfigService _appConfigService;
        private string className = nameof(UserService);
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, _IUnitOfWork unitOfWord, IMemoryCache memoryCache, AppConfigService appConfigService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWord = unitOfWord;
            _memoryCache = memoryCache;
            _appConfigService = appConfigService;
        }
        public async Task<UserModel> GetUser(Guid id)
        {
            string actionName = className + nameof(GetUser);
            var cacheKey = $"{this.GetType().Name}-{actionName}:{id}";

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
                    AOLLogger.Error(actionName + " - Id = " + id.ToString() + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            } catch(Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        
        public async Task<UserModel> Create(RegisterRequest registerRequest)
        {
            // Log the method name and class name for debugging
            string methodName = className + nameof(Create);
            try
            {
                if (registerRequest == null)
                {
                    throw new Exception("Register request cannot be null.");
                }
                if (string.IsNullOrEmpty(registerRequest.Username) || string.IsNullOrEmpty(registerRequest.Password) || string.IsNullOrEmpty(registerRequest.Email))
                {
                    throw new Exception("Username, password, and email are required.");
                }
                if (registerRequest.Password != registerRequest.ConfirmPassword)
                {
                    throw new Exception("Passwords do not match.");
                }
                if (registerRequest.Username.Length < 6 || registerRequest.Password.Length < 6)
                {
                    throw new Exception("The username and password must each be at least 6 characters long.");
                }

                // check if user already exists by username or email
                var existingUserByUsername = await _userRepository.GetByUserNameAsync(registerRequest.Username.Trim());
                if (existingUserByUsername != null)
                {
                    AOLLogger.Error(methodName + " - username = " + registerRequest.Username + " - " + MessageContext.UserAlreadyExists);
                    throw new Exception("Username " + MessageContext.UserAlreadyExists);
                }
                var existingUserByEmail = await _userRepository.GetByEmailAsync(registerRequest.Email.Trim());
                if (existingUserByEmail != null)
                {
                    AOLLogger.Error(methodName + " - email = " + registerRequest.Email + " - " + MessageContext.UserAlreadyExists);
                    throw new Exception("Email " + MessageContext.UserAlreadyExists);
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
                AOLLogger.Error(methodName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserModel> Update(UserModel model)
        {
            string actionName = className + nameof(Update);
            try
            {
                if (model == null || model.Id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user model.");
                }
                var user = await _userRepository.GetByIdAsync(model.Id);
                if (user == null)
                {
                    AOLLogger.Error(actionName + " - Id = " + model.Id.ToString() + " - " + MessageContext.NotFound);
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
                // cập nhật trạng thái từ model.status là string sang enum UserStatus
                if (Enum.TryParse<UserStatus>(model.Status, true, out var userStatus))
                {
                    user.Status = userStatus;
                }
                else
                {
                    user.Status = UserStatus.Active; // Mặc định là Active nếu không parse được
                }
                user.UpdatedAt = DateTime.UtcNow;
                _userRepository.UpdateAsync(user);
                await _unitOfWord.SaveChangesAsync();
                return user.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception("Update failed");
            }
        }
        public async Task<UserModel> GetByRefreshToken(string refreshToken)
        {
            string actionName = className + nameof(GetByRefreshToken);
            try
            {
                var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
                if (user != null)
                {
                    return user.ReturnModel();
                }
                else
                {
                    AOLLogger.Error(actionName + " - refreshToken = " + refreshToken + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            string actionName = className + nameof(Delete);
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user ID.");
                }
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    AOLLogger.Error(actionName + " - Id = " + id.ToString() + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
                user.Status = UserStatus.Deleted; // Cập nhật trạng thái người dùng thành Deleted
                _userRepository.UpdateAsync(user);
                await _unitOfWord.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserModel> GetByToken(string token)
        {
            string actionName = className + nameof(GetByToken);
            try
            {
                var user = await _userRepository.GetByTokenAsync(token);
                if (user != null)
                {
                    return user.ReturnModel();
                }
                else
                {
                    AOLLogger.Error(actionName + " - token = " + token + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserModel>> GetAll(string searchText, int index = 1, int pageSize = 1000, bool desc = true)
        {
            string actionName = className + nameof(GetAll);
            try
            {
                if (index < 1 || pageSize < 1)
                {
                    throw new ArgumentException("Index and page size must be greater than 0.");
                }
                var users = await _userRepository.SearchAsync(searchText, index, pageSize, desc);
                if (users == null || !users.Any())
                {
                    AOLLogger.Error(actionName + " - No users found.");
                    return new List<UserModel>();
                }

                return users.Select(u => u.ReturnModel()).ToList();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserModel>> GetAll(int page, int pageSize)
        {
           return await GetAll(string.Empty, page, pageSize, true);
        }

        public async Task<UserModel> GetByUsername(string username)
        {
            string actionName = className + nameof(GetByUsername);
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("Username cannot be null or empty.");
                }
                var user = await _userRepository.GetByUserNameAsync(username.Trim());
                if (user != null)
                {
                    return user.ReturnModel();
                }
                else
                {
                    AOLLogger.Error(actionName + " - username = " + username + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }

        }
        public async Task<UserModel> GetByPhoneNumber(string phone)
        {
            string actionName = className + nameof(GetByPhoneNumber);
            try
            {
                if (string.IsNullOrEmpty(phone))
                {
                    throw new ArgumentException("Phone number cannot be null or empty.");
                }
                var user = await _userRepository.GetByPhoneNumberAsync(phone.Trim());
                if (user != null)
                {
                    return user.ReturnModel();
                }
                else
                {
                    AOLLogger.Error(actionName + " - phone = " + phone + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<UserModel> GetByEmail(string email)
        {
            string actionName = className + nameof(GetByEmail);
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email cannot be null or empty.");
                }
                var user = await _userRepository.GetByEmailAsync(email.Trim());
                if (user != null)
                {
                    return user.ReturnModel();
                }
                else
                {
                    AOLLogger.Error(actionName + " - email = " + email + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> ChangePassword(Guid id, ChangePasswordRequest changePasswordRequest)
        {
            string actionName = className + nameof(ChangePassword);
            try
            {
                if (id == Guid.Empty || changePasswordRequest == null)
                {
                    throw new ArgumentException("Invalid user ID or change password request.");
                }
                var user = _userRepository.GetByIdAsync(id).Result;
                if (user == null)
                {
                    AOLLogger.Error(actionName + " - Id = " + id.ToString() + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
                if (user.PasswordHash != DomainHelper.HashPassword(changePasswordRequest.OldPassword))
                {
                    throw new Exception("Old password is incorrect.");
                }
                user.PasswordHash = DomainHelper.HashPassword(changePasswordRequest.NewPassword);
                _userRepository.UpdateAsync(user);
                return await _unitOfWord.SaveChangesAsync().ContinueWith(t => true);
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserModel> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            string actionName = className + nameof(ForgotPassword);
            try
            {
                if (forgotPasswordRequest == null || string.IsNullOrEmpty(forgotPasswordRequest.Email))
                {
                    throw new ArgumentException("Invalid forgot password request.");
                }
                var user = await _userRepository.GetByEmailAsync(forgotPasswordRequest.Email.Trim());
                if (user == null)
                {
                    AOLLogger.Error(actionName + " - email = " + forgotPasswordRequest.Email + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
                return user.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ResetPassword(string email, ResetPasswordRequest changePasswordRequest)
        {
            string actionName = className + nameof(ResetPassword);
            try { 
                if (string.IsNullOrEmpty(email) || changePasswordRequest == null)
                {
                    throw new ArgumentException("Invalid user ID or reset password request.");
                }
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    AOLLogger.Error(actionName + " - Email = " + email + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
                user.PasswordHash = DomainHelper.HashPassword(changePasswordRequest.NewPassword);
                _userRepository.UpdateAsync(user);
                return await _unitOfWord.SaveChangesAsync().ContinueWith(t => true);
            } catch(Exception ex)
            {
                AOLLogger.Error(actionName + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
