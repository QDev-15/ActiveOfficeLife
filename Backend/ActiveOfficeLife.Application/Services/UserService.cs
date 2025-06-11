using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Requests;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Domain;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly _IUnitOfWork _unitOfWord;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, _IUnitOfWork unitOfWord)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWord = unitOfWord;
        }
        public async Task<UserModel> GetUser(Guid id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user != null)
                {
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

        public async Task<UserModel> Login(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetByUserPassAsync(loginRequest.UserName, loginRequest.Password);
                if (user != null)
                {
                    return user.ReturnModel();
                }
                else
                {
                    AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - username = " + loginRequest.UserName + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            } catch(Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserModel> Register(RegisterRequest registerRequest)
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
                await _unitOfWord.SaveChangeAsync();
                return user.ReturnModel();
            } catch(Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public Task<UserModel> Update(UserModel model)
        {
            throw new NotImplementedException();
        }
    }
}
