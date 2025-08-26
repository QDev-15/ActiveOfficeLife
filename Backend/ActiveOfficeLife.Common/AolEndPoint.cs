namespace ActiveOfficeLife.Common
{
    public static class AOLEndPoint
    {
        // Base URL for the API
        // Authentication Endpoints
        public static string AuthLogin = "/api/Auth/login";
        public static string AuthLogout = "/api/Auth/logout";
        public static string AuthRefresh = "/api/Auth/refresh";
        public static string AuthResetPassword = "/api/Auth/resetpassword";
        public static string AuthMe = "/api/Auth/me";
        // User Endpoints
        public static string UserRegister = "/api/User/register";
        public static string UserGetUser = "/api/User/getuser";
        public static string UserGetHistory = "/api/User/login-history";
        public static string UserGetById = "/api/User/getbyid";
        public static string UserGetAll = "/api/User/getall";
        public static string UserUpdate = "/api/User/update";
        public static string UserDelete = "/api/User/delete";
        public static string UserChangePassword = "/api/User/changepassword";
        public static string UserForgotPassword = "/api/User/forgotpassword";
        public static string UserGetByEmail = "/api/User/getbyemail";
        public static string UserGetByUsername = "/api/User/getbyusername"; 
        public static string UserGetByPhoneNumber = "/api/User/getbyphonenumber";
        // Category Endpoints
        public static string CategoryGetAll = "/api/category/all";
        public static string CategoryGetById = "/api/category/getbyid";
        public static string CategoryCreate = "/api/category/create";
        public static string CategoryUpdate = "/api/category/update";
        public static string CategoryDelete = "/api/category/delete";
        // Post Endpoints
        public static string PostGetAll = "/api/post/getall";
        public static string PostGetById = "/api/post/getbyid";
        public static string PostCreate = "/api/post/create";
        public static string PostUpdate = "/api/post/update";
        public static string PostDelete = "/api/post/delete";
        // Comment Endpoints
        public static string CommentGetAll = "/api/comment/getall";
        public static string CommentGetById = "/api/comment/getbyid";
        public static string CommentCreate = "/api/comment/create";
        public static string CommentUpdate = "/api/comment/update";
        public static string CommentDelete = "/api/comment/delete";
        // Like Endpoints
        public static string LikeGetAll = "/api/like/getall";
        public static string LikeGetById = "/api/like/getbyid";
        public static string LikeCreate = "/api/like/create";
        public static string LikeUpdate = "/api/like/update";
        public static string LikeDelete = "/api/like/delete";
        // Notification Endpoints
        public static string NotificationGetAll = "/api/notification/getall";
        public static string NotificationGetById = "/api/notification/getbyid";
        public static string NotificationCreate = "/api/notification/create";
        public static string NotificationUpdate = "/api/notification/update";
        public static string NotificationDelete = "/api/notification/delete";
        // File Endpoints
        public static string FileUpload = "/api/file/upload";
        public static string FileDownload = "/api/file/download";
        public static string FileDelete = "/api/file/delete";
        // Settings Endpoints
        public static string SettingsGetAll = "/api/settings/getall";
        public static string SettingsGetById = "/api/settings/getbyid";
        public static string SettingsUpdate = "/api/settings/update";
        public static string SettingsDelete = "/api/settings/delete";
        // Analytics Endpoints
        public static string AnalyticsGetUserActivity = "/api/analytics/useractivity";
        public static string AnalyticsGetPostEngagement = "/api/analytics/postengagement";
        public static string AnalyticsGetCategoryPerformance = "/api/analytics/categoryperformance";
        // log Endpoints
        public static string LogGetAll = "/api/logs/all";
    }
}
