using System.Collections.Generic;

namespace Voidwell.FileWell
{
    public static class MimeTypeMappings
    {
        public static string GetMimeType(string extension)
        {
            return _map.GetValueOrDefault(extension.ToLower());
        }

        private static Dictionary<string, string> _map = new Dictionary<string, string> {
            { "bmp", "image/bmp" },
            { "csv", "text/csv" },
            { "doc", "application/msword" },
            { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "epub", "application/epub+zip" },
            { "gif", "image/gif" },
            { "htm", "text/html" },
            { "html", "text/html" },
            { "ico", "image/x-icon"},
            { "jpg", "image/jpeg"},
            { "jpeg", "image/jpeg"},
            { "js", "application/javascript"},
            { "json", "application/json"},
            { "mpeg", "video/mpeg"},
            { "png", "image/png"},
            { "pdf", "application/pdf"},
            { "rar", "application/x-rar-compressed"},
            { "sh", "application/x-sh"},
            { "svg", "image/svg+xml"},
            { "tif", "image/tiff"},
            { "tiff", "image/tiff"},
            { "ts", "application/typescript"},
            { "txt", "text/plain"},
            { "wav", "audio/wav"},
            { "weba", "audio/webm"},
            { "webm", "video/webm"},
            { "webp", "image/webp"},
            { "xls", "application/vnd.ms-excel"},
            { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            { "xml", "application/xml"},
            { "zip", "application/zip"},
            { "7z", "application/x-7z-compressed"}
        };
    }
}
