using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(Guid userId, string username, IList<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
