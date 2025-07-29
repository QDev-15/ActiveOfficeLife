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
        public static string UserGetRoles = "/api/User/getroles";
        public static string UserAssignRole = "/api/User/assignrole";
        public static string UserRemoveRole = "/api/User/removerole";   
        public static string UserGetPermissions = "/api/User/getpermissions";
        public static string UserAssignPermission = "/api/User/assignpermission";
        public static string UserRemovePermission = "/api/User/removepermission";
        public static string UserGetUserPermissions = "/api/User/getuserpermissions";
        public static string UserGetUserRoles = "/api/User/getuserroles";
        public static string UserGetUserByEmail = "/api/User/getuserbyemail";
        public static string UserGetUserByUsername = "/api/User/getuserbyusername"; 
        public static string UserGetUserByPhoneNumber = "/api/User/getuserbyphonenumber";
        public static string CategoryGetAll = "";
    }
}
