using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.AppConfigs
{
    public class FileStorage
    {
        public string RootPath { set; get; } = string.Empty;
        public string BaseUrl { set; get; } = string.Empty;
        public bool UseDateSubfolders { set; get; }
        public int MaxFileSizeMB { set; get; } = 5; // default 10MB
    }
}
