using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class UserToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; }

        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public string IpAddress { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
