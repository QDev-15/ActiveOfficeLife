using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.Models
{
    public class SettingModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Logo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? GoogleClientId { get; set; }
        public string? GoogleClientSecretId { get; set; }
        public string? GoogleFolderId { get; set; }
        public string? GoogleToken { get; set; }
    }
}
