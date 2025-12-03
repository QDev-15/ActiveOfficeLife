using System.Runtime.CompilerServices;

namespace ActiveOfficeLife.Common
{
    public static class AOLEndPoint
    {
        // Base URL for the API
        // Authentication Endpoints
        public const string AuthLogin = "/api/Auth/login";
        public const string AuthLogout = "/api/Auth/logout";
        public const string AuthRefresh = "/api/Auth/refresh";
        public const string AuthResetPassword = "/api/Auth/resetpassword";
        public const string AuthMe = "/api/Auth/me";
        // Ad Endpoints
        public const string AdGetAll = "/api/ad/all";
        public const string AdGetById = "/api/ad/getbyid";
        public const string AdCreate = "/api/ad/create";
        public const string AdUpdate = "/api/ad/update";
        public const string AdDelete = "/api/ad/delete";
        // User Endpoints
        public const string UserRegister = "/api/User/register";
        public const string UserGetUser = "/api/User/getuser";
        public const string UserGetHistory = "/api/User/login-history";
        public const string UserGetById = "/api/User/getbyid";
        public const string UserGetAll = "/api/User/getall";
        public const string UserUpdate = "/api/User/update";
        public const string UserDelete = "/api/User/delete";
        public const string UserChangePassword = "/api/User/changepassword";
        public const string UserForgotPassword = "/api/User/forgotpassword";
        public const string UserGetByEmail = "/api/User/getbyemail";
        public const string UserGetByUsername = "/api/User/getbyusername"; 
        public const string UserGetByPhoneNumber = "/api/User/getbyphonenumber";
        // Category Endpoints
        public const string CategoryGetAll = "/api/category/all";
        public const string CategoryGetById = "/api/category/getbyid";
        public const string CategoryCreate = "/api/category/create";
        public const string CategoryUpdate = "/api/category/update";
        public const string CategoryDelete = "/api/category/delete";
        // Category Type Endpoints
        public const string CategoryTypeGetAll = "/api/category/types";
        public const string CategoryTypeGetById = "/api/category/gettype-id";
        public const string CategoryTypeCreate = "/api/category/create-type";
        public const string CategoryTypeUpdate = "/api/category/update-type";
        // Post Endpoints
        public const string PostGetAll = "/api/post/getall";
        public const string PostGetById = "/api/Post/get";
        public const string PostGetBySlug = "/api/Post/get-by-slug";
        public const string PostCreate = "/api/post/create";
        public const string PostUpdate = "/api/post/update";
        public const string PostDelete = "/api/post/delete";
        // tag Endpoints
        /// <summary>
        /// api/tag/all?search=&pageNumber=1&pageSize=10
        /// </summary>
        public const string TagGetAll = "/api/tag/all";
        /// <summary>
        /// /api/tag/getbyid?id=...
        /// </summary>
        public const string TagGetById = "/api/tag/getbyid";
        /// <summary>
        /// /api/tag/create
        /// </summary>
        public const string TagCreate = "/api/tag/create";
        public const string TagUpdate = "/api/tag/update";
        public const string TagDelete = "/api/tag/delete";
        public const string PatchTag = "/api/tag/patch";
        // Comment Endpoints
        public const string CommentGetAll = "/api/comment/getall";
        public const string CommentGetById = "/api/comment/getbyid";
        public const string CommentCreate = "/api/comment/create";
        public const string CommentUpdate = "/api/comment/update";
        public const string CommentDelete = "/api/comment/delete";
        // Like Endpoints
        public const string LikeGetAll = "/api/like/getall";
        public const string LikeGetById = "/api/like/getbyid";
        public const string LikeCreate = "/api/like/create";
        public const string LikeUpdate = "/api/like/update";
        public const string LikeDelete = "/api/like/delete";
        // Notification Endpoints
        public const string NotificationGetAll = "/api/notification/getall";
        public const string NotificationGetById = "/api/notification/getbyid";
        public const string NotificationCreate = "/api/notification/create";
        public const string NotificationUpdate = "/api/notification/update";
        public const string NotificationDelete = "/api/notification/delete";
        // File Endpoints
        public const string FileUpload = "/api/file/upload";
        public const string FileDownload = "/api/file/download";
        public const string FileDelete = "/api/file/delete";
        // Settings Endpoints
        public const string SettingsGetAll = "/api/settings/getall";
        public const string SettingsGetById = "/api/settings/getbyid";
        public const string SettingsUpdate = "/api/settings/update";
        public const string SettingsDelete = "/api/settings/delete";
        // Analytics Endpoints
        public const string AnalyticsGetUserActivity = "/api/analytics/useractivity";
        public const string AnalyticsGetPostEngagement = "/api/analytics/postengagement";
        public const string AnalyticsGetCategoryPerformance = "/api/analytics/categoryperformance";
        // log Endpoints
        public const string LogGetAll = "/api/logs/all";
        // get name api endpoints ex: /api/controller/action return action
        
    }

}
