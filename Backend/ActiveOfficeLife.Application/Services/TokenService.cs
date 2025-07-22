using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Models.Responses;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Domain;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly byte[] _secretKey;
        private readonly JwtTokens _jwtSettings;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly _IUnitOfWork _unitOfWork;

        public TokenService(IOptions<JwtTokens> jwtOptions, IUserRepository userRepository, _IUnitOfWork iUnitOfWork, IUserTokenRepository userTokenRepository)
        {
            _jwtSettings = jwtOptions.Value;
            _secretKey = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            _userRepository = userRepository;
            _unitOfWork = iUnitOfWork;
            _userTokenRepository = userTokenRepository;
        }

        public async Task<AuthResponse> CreateAsync(UserModel userModel, string ipAddress)
        {
            var userToken = await _userTokenRepository.GetByUserIdAsync(userModel.Id, ipAddress);
            if (userToken != null)
            {
                // If the user already, generate a new access token and refresh token save to the database
                userToken.AccessToken = GenerateAccessToken(userModel);
                userToken.AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
                userToken.RefreshToken = GenerateRefreshToken();
                userToken.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays); // Set refresh token expiration
                userToken.IpAddress = ipAddress;
                _userTokenRepository.UpdateAsync(userToken);
                await _unitOfWork.SaveChangesAsync();

                // If a token already exists for this user and IP address, return it
                return new AuthResponse
                {
                    AccessToken = userToken.AccessToken,
                    RefreshToken = userToken.RefreshToken,
                    User = userModel,
                    Role = string.Join(",", userModel.Roles.ToList()),
                    UserId = userModel.Id
                };
            }
            var token = GenerateAccessToken(userModel);
            var refreshToken = GenerateRefreshToken();
            var newUserToken = new UserToken
            {
                UserId = userModel.Id,
                AccessToken = token,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays), // Set refresh token expiration
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow,
            };
            await _userTokenRepository.AddAsync(newUserToken);
            await _unitOfWork.SaveChangesAsync();
            return new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                User = userModel,
                Role = string.Join(",", userModel.Roles.ToList()),
                UserId = userModel.Id
            };
        }



        public string GenerateAccessToken(UserModel user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("AvatarUrl", user.AvatarUrl ?? string.Empty),
                new Claim("Status", user.Status.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.Username),
                new Claim("Email", user.Email),
                new Claim("Roles", string.Join(",", user.Roles)),
                new Claim("Token", user.Token ?? string.Empty),
                new Claim("RefreshToken", user.RefreshToken ?? string.Empty),
                new Claim("CreatedAt", DateTime.UtcNow.ToString("o")), // ISO 8601 format
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //foreach (var role in user.Roles)
            //{
            //    authClaims.Add(new Claim(ClaimTypes.Role, role));
            //}

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_secretKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public string GenerateRefreshToken(int daysValid = 7)
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var refreshToken = Convert.ToBase64String(randomBytes);

            return refreshToken;
        }

        public async Task<List<UserTokenModel>> GetUserTokensAsync(Guid userId)
        {
            try
            {
                var userTokens = await _userTokenRepository.GetAllByUserIdAsync(userId);
                if (userTokens == null || !userTokens.Any())
                {
                    AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - userId = " + userId + " - " + MessageContext.NotFound);
                    throw new Exception("User tokens not found");
                }
                return userTokens.Select(ut => new UserTokenModel
                {
                    Id = ut.Id,
                    UserId = ut.UserId,
                    AccessToken = ut.AccessToken,
                    AccessTokenExpiresAt = ut.AccessTokenExpiresAt,
                    RefreshToken = ut.RefreshToken,
                    RefreshTokenExpiresAt = ut.RefreshTokenExpiresAt,
                    IpAddress = ut.IpAddress,
                    CreatedAt = ut.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetByUserNameAsync(loginRequest.UserName);
                if (user != null)
                {
                    var verified = DomainHelper.VerifyPassword(user.PasswordHash, loginRequest.Password);
                    if (verified.Success)
                    {
                        var authResponse = await CreateAsync(user.ReturnModel(), loginRequest.ipAddress);
                        return authResponse;
                    }
                    else
                    {
                   
                        AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - username = " + loginRequest.UserName + " - " + MessageContext.NotActive);
                        throw new UnauthorizedAccessException("Invalid credentials");
                    }
                    
                }
                else
                {
                    AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - username = " + loginRequest.UserName + " - " + MessageContext.NotFound);
                    throw new Exception("User " + MessageContext.NotFound);
                }
            }
            catch (Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod().Name + " - error = " + ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            var userToken = await _userTokenRepository.GetByRefreshTokenAsync(refreshToken);
            if (userToken == null)
            {
                throw new SecurityException("Invalid refresh token");
            }
            // Check if the refresh token is expired
            if (userToken.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                throw new SecurityException("logout");
            }
            // Generate new tokens
            var user = await _userRepository.GetByIdAsync(userToken.UserId);
            var newAccessToken = GenerateAccessToken(user.ReturnModel());
            var newRefreshToken = GenerateRefreshToken();
            // Update the user token
            userToken.AccessToken = newAccessToken;
            userToken.AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            userToken.RefreshToken = newRefreshToken;
            userToken.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            userToken.IpAddress = ipAddress;
            userToken.CreatedAt = DateTime.UtcNow;
            _userTokenRepository.UpdateAsync(userToken);
            await _unitOfWork.SaveChangesAsync();
            return new AuthResponse
            {
                AccessToken = userToken.AccessToken,
                RefreshToken = userToken.RefreshToken,
                User = user.ReturnModel(),
                Role = string.Join(",", user.Roles.Select(x => x.Name).ToList()),
                UserId = user.Id
            };
        }


    }
}
