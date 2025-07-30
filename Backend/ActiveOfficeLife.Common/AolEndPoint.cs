using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common
{
    public static class AOLEndPoint
    {
        public static string AuthLogin = "/api/Auth/login";
        public static string AuthLogout = "/api/Auth/logout";
        public static string AuthRefresh = "/api/Auth/refresh";
        public static string AuthMe = "/api/Auth/me";
        public static string UserRegister = "/api/User/register";
        public static string UserGetUser = "/api/User/getuser";
        public static string UserGetHistory = "/api/User/login-history";
        public static string UserGetById = "/api/User/getbyid";
        public static string UserGetAll = "/api/User/getall";
        public static string UserUpdate = "/api/User/update";
        public static string UserDelete = "/api/User/delete";
        public static string UserChangePassword = "/api/User/changepassword";
        public static string UserForgotPassword = "/api/User/forgotpassword";
        public static string UserResetPassword = "/api/User/resetpassword";
        public static string UserGetByEmail = "/api/User/getbyemail";
        public static string UserGetByUsername = "/api/User/getbyusername"; 
        public static string UserGetByPhoneNumber = "/api/User/getbyphonenumber";
        public static string CategoryGetAll = "";
    }
}
