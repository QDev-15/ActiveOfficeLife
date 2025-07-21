using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.AppConfigs
{
    public class HostingConfigs
    {
        public string Domain { get; set; }
        public string HostDirectory { get; set; }
        public string UploadDirectory { get; set; }
        public List<string> AllowedExtensions { get; set; }
        public int MaxFileSizeMB { get; set; }
        public string WebHost { get; set; }
        public string AdminHost { get; set; }
        public string HostAddress { get; set; }
        public string UserFTP { get; set; }
        public string PassWordFTP { get; set; }
        public string PortFTP { get; set; }
        public string ServerType { get; set; } = "server";
    }
}
