using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi
{
    public static class Helper
    {
        public static string GetMimeType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType(fileName, out var contentType))
                return contentType;

            return "application/octet-stream";
        }
    }
}
