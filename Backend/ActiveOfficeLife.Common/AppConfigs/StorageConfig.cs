using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.AppConfigs
{
    public class StorageConfig
    {
        public bool SaveToGoogle {set;get;} = false;
        public bool SaveToLocal {set;get;} = false;
        public bool SaveToAzure {set;get;} = false;
        public bool SaveToAws {set;get;} = false;
        public bool SaveToDropbox {set;get;} = false;
        public bool SaveToOneDrive {set;get;} = false;
        public bool SaveToBox {set;get;} = false;
        public bool SaveToFtp {set;get;} = false;
        public bool SaveToHost {set;get;} = false;
        public string GoogleFolderId { set; get; } = string.Empty;
    }
}
