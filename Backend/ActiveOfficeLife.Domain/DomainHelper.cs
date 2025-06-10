using ActiveOfficeLife.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
        }
    }
}
