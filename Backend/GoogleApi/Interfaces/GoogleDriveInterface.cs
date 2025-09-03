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
    public interface GoogleDriveInterface
    {
        
        Task<bool> CheckIsExpiredToken(string jsonToken, ClientSecrets clientSecrets);
        Task<TokenResponse> GetToken(string jsonToken);
        Task<TokenResponse> RefreshToken(string jsonToken, ClientSecrets clientSecrets);
        Task<UserCredential> GetUserCredentialAsync(string jsonToken, ClientSecrets clientSecrets);
        Task<DriveService> GetDriveServiceAsync(string jsonToken, ClientSecrets clientSecrets, string appName);
        Task<UpLoadResponse> UploadFileAndMakePublicAsync(IFormFile file, string folderId, string jsonToken, ClientSecrets clientSecrets, string appName);
    }
}
