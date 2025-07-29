using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application
{
    public static class Helper
    {
        /// <summary>
        /// Generates a URL-friendly slug from the specified title.
        /// </summary>
        /// <remarks>The method performs the following transformations to generate the slug: <list
        /// type="bullet"> <item><description>Converts the input string to lowercase.</description></item>
        /// <item><description>Removes diacritics and Unicode characters.</description></item>
        /// <item><description>Replaces non-alphanumeric characters with hyphens.</description></item>
        /// <item><description>Trims leading and trailing hyphens and reduces consecutive hyphens to a single
        /// one.</description></item> </list> This method is useful for creating SEO-friendly URLs or
        /// identifiers.</remarks>
        /// <param name="title">The input string to generate the slug from. If <paramref name="title"/> is <see langword="null"/> or empty,
        /// a default slug based on the current UTC time will be used.</param>
        /// <returns>A URL-friendly slug generated from the input string. If the input string contains only whitespace, an empty
        /// string is returned.</returns>
        public static string GenerateSlug(string? title)
        {
            if (string.IsNullOrEmpty(title))
                title = "aol-" + DateTime.UtcNow.ToShortTimeString();
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            // Chuyển về lowercase
            title = title.ToLowerInvariant();

            // Loại bỏ dấu tiếng Việt hoặc ký tự Unicode
            title = RemoveDiacritics(title);

            // Loại bỏ ký tự không phải chữ, số, dấu cách
            title = Regex.Replace(title, @"[^a-z0-9\s-]", "");

            // Thay thế khoảng trắng bằng dấu gạch ngang
            title = Regex.Replace(title, @"\s+", "-").Trim('-');

            // Giảm bớt dấu gạch nếu có nhiều cái liên tiếp
            title = Regex.Replace(title, @"-+", "-");

            return title;
        }

        /// <summary>
        /// Loại bỏ dấu tiếng Việt hoặc ký tự Unicode
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }           
        /// <summary>
        /// Generates a random alphanumeric string of the specified length.
        /// </summary>
        /// <param name="length">The length of the string to generate. Must be a non-negative integer. Defaults to 4 if not specified.</param>
        /// <returns>A randomly generated string consisting of uppercase letters, lowercase letters, and digits.</returns>
        public static string GenerateRandomString(int length = 4)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var sb = new StringBuilder();
            var rng = Random.Shared;

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[rng.Next(chars.Length)]);
            }

            return sb.ToString();
        }
    }
}
