using ActiveOfficeLife.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain
{
    public static class DomainHelper
    {
        public static string HashPassword(string password)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.HashPassword(null, password);
        }

        /// <summary>
        /// Ex: string hashed = Helper.HashPassword("123456"); ===> call: VerifyPassword(hashed, "123456"); 
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <param name="providedPassword"></param>
        /// <returns></returns>
        public static PasswordVerifyResult VerifyPassword(string hashedPassword, string providedPassword)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return new PasswordVerifyResult
            {
                Success = result == PasswordVerificationResult.Success,
                SuccessRehashNeeded = result == PasswordVerificationResult.SuccessRehashNeeded,
                Failed = result == PasswordVerificationResult.Failed
            };
        }
    }
    public class PasswordVerifyResult
    {
        public bool Success { get; set; }
        public bool SuccessRehashNeeded { get; set; }
        public bool Failed { get; set; }
    }
}
