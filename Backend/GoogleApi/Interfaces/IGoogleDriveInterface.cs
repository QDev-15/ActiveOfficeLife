using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using GoogleApi.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi.Interfaces
{
    public interface IGoogleDriveInterface
    {
        
        Task<bool> CheckIsExpiredToken(string jsonToken, ClientSecrets clientSecrets);
        string GetAuthorizationUrl(ClientSecrets clientSecrets, string redirectUri, string? state, string? orgId);
        Task<TokenResponse> ExchangeCodeForTokenAsync(ClientSecrets clientSecrets,string code, string redirectUri, string userId = "user");
        TokenResponse GetToken(string jsonToken);
        Task<TokenResponse> RefreshToken(string jsonToken, ClientSecrets clientSecrets);
        Task<UserCredential> GetUserCredentialAsync(string jsonToken, ClientSecrets clientSecrets);
        Task<DriveService> GetDriveServiceAsync(string jsonToken, ClientSecrets clientSecrets, string appName);
        Task<UpLoadResponse> UploadFileAndMakePublicAsync(IFormFile file, string folderId, string jsonToken, ClientSecrets clientSecrets, string appName);
        Task<Stream> DownloadFileAsync(string fileId, string jsonToken, ClientSecrets clientSecrets, string appName);
        Task<ResultResponse> DeleteFileAsync(string fileId, string jsonToken, ClientSecrets clientSecrets, string appName);



    }
}
