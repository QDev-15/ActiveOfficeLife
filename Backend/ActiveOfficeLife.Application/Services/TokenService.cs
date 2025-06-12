using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ActiveOfficeLife.Application.Models.Responses;
using ActiveOfficeLife.Domain;
using Microsoft.AspNetCore.Identity;
using System.Security;
using ActiveOfficeLife.Application.ExtensitionModel;
using System.Text.Json;
using ActiveOfficeLife.Application.Interfaces;

namespace ActiveOfficeLife.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly byte[] _secretKey;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository _userRepository;
        private readonly _IUnitOfWork _unitOfWork;

        public TokenService(IOptions<JwtSettings> jwtOptions, IUserRepository userRepository, _IUnitOfWork iUnitOfWork)
        {
            _jwtSettings = jwtOptions.Value;
            _secretKey = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            _userRepository = userRepository;
            _unitOfWork = iUnitOfWork;
        }

        public string GenerateAccessToken(UserModel user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in user.Roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

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

            var token = Convert.ToBase64String(randomBytes);
            var refreshTokenData = new RefreshTokenData
            {
                Token = token,
                Expires = DateTime.UtcNow.AddDays(daysValid)
            };

            return JsonSerializer.Serialize(refreshTokenData);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKey),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, // Bỏ qua thời hạn token
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch (Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod()?.Name + " - error = " + ex.Message);
                return null;
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
                        return new AuthResponse
                        {
                            AccessToken = GenerateAccessToken(user.ReturnModel()),
                            RefreshToken = GenerateRefreshToken(),
                            User = user.ReturnModel(),
                            Role = string.Join(",", user.Roles.Select(x => x.Name).ToList())
                        };
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

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user == null)
            {
                throw new SecurityException("Invalid refresh token");
            }
            // Check if the refresh token is expired
            var tokenData = JsonSerializer.Deserialize<RefreshTokenData>(refreshToken);

            if (tokenData.Expires <= DateTime.UtcNow)
            {
                throw new SecurityException("logout");
            }

            user.RefreshToken = GenerateRefreshToken(); // generate a new refresh token
            user.Token = GenerateAccessToken(user.ReturnModel()); // generate a new access token
            _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangeAsync();
            return new AuthResponse
            {
                AccessToken = user.Token,
                RefreshToken = user.RefreshToken,
                User = user.ReturnModel(),
                Role = string.Join(",", user.Roles.Select(x => x.Name).ToList()),
            };
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_secretKey),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }

                return null;
            }
            catch (Exception ex)
            {
                AOLLogger.Error(MethodBase.GetCurrentMethod()?.Name + " - error = " + ex.Message);
                return null;
            }
        }

    }
}
