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
        public static string GenerateSlug(string title)
        {
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
