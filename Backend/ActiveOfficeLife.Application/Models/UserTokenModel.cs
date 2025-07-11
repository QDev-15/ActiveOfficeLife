using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models
{
    public class UserTokenModel
    {
        public Guid? Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; }
        public UserModel? User { get; set; }

        public string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiresAt { get; set; }
        public string? IpAddress { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public bool AccessTokenIsValid
        {
            get
            {
                return AccessTokenExpiresAt.HasValue && AccessTokenExpiresAt.Value > DateTime.UtcNow;
            }
        }
        public bool RefreshTokenIsValid
        {
            get
            {
                return RefreshTokenExpiresAt.HasValue && RefreshTokenExpiresAt.Value > DateTime.UtcNow;
            }
        }
    }
}
