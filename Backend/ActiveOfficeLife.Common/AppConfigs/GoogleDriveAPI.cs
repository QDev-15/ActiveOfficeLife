using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.AppConfigs
{
    public class GoogleDriveAPI
    {
        public string? FolderID {set;get; } = null;
        public bool AccountService {set;get; } = false;
        public bool OAuthClientService {set;get; } = false;
        public string AccountCredentialFileName {set;get; } = string.Empty;
        public string OAuthClienCredentialFileName { set; get; } = string.Empty;
    }
}
