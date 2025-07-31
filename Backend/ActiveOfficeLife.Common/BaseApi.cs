using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common
{
    public class BaseApi
    {
        public string? Url { set; get;}
        public string AccessToken { set; get;} = "AccessToken";
        public int TimeoutSeconds { set; get;} = 30;
        public int AccessTokenExpireHours { set; get;} = 1;
    }
}
