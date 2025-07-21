using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.AppConfigs
{
    public class Sessions
    {
        public int IdleTimeout { set; get; }
        public bool CookieHttpOnly { set; get; }
        public bool CookieIsEssential { set; get; }
    }
}
