﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models.Requests
{
    public class LoginRequest
    {
        public string UserName { set; get; }
        public string Password { set; get; }
        public string ipAddress { set; get; }
        public bool Remember { set; get; }
    }
}
