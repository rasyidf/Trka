using System;
using System.Collections.Generic;

namespace Terka.Matchers
{
    /// <summary>
    /// Detects container format from the file extension.
    /// Unlike other matchers, this operates on the extension, not on tokens.
    /// </summary>
    internal class ContainerMatcher
    {
        private static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["mkv"] = "video/x-matroska",
            ["mk3d"] = "video/x-matroska-3d",
            ["mka"] = "audio/x-matroska",
            ["avi"] = "video/x-msvideo",
            ["mp4"] = "video/mp4",
            ["m4v"] = "video/mp4",
            ["mov"] = "video/quicktime",
            ["wmv"] = "video/x-ms-wmv",
            ["flv"] = "video/x-flv",
            ["webm"] = "video/webm",
            ["ogv"] = "video/ogg",
            ["ogg"] = "video/ogg",
            ["ogm"] = "video/ogg",
            ["mpg"] = "video/mpeg",
            ["mpeg"] = "video/mpeg",
            ["vob"] = "video/dvd",
            ["m2ts"] = "video/mp2t",
            ["ts"] = "video/mp2t",
            ["3gp"] = "video/3gpp",
            ["3g2"] = "video/3gpp2",
            ["asf"] = "video/x-ms-asf",
            ["rm"] = "application/vnd.rn-realmedia",
            ["divx"] = "video/x-msvideo",
            ["srt"] = "text/srt",
            ["ssa"] = "text/x-ssa",
            ["mp3"] = "audio/mpeg",
            ["wav"] = "audio/x-wav",
            ["wma"] = "audio/x-ms-wma",
            ["ra"] = "audio/vnd.rn-realaudio",
            ["ram"] = "audio/vnd.rn-realaudio",
            ["flac"] = "audio/flac",
        };

        private static readonly HashSet<string> KnownContainers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "3g2","3gp","3gp2","7z","avi","asf","bmp","bz2","cb7","cbr","cbz","divx","flv",
            "gif","gz","idx","iso","jpeg","jpg","m2ts","m4v","mk2","mk3d","mka","mkv","mov",
            "mp4","mp4a","mpeg","mpg","nfo","nzb","ogg","ogm","ogv","png","qt","r00","ra",
            "ram","rar","rm","srt","ssa","tar","tbn","tgz","torrent","ts","vob","wav","webm",
            "webp","wma","wmv","zip"
        };

        public void Match(string extension, GuessResult result)
        {
            if (string.IsNullOrEmpty(extension)) return;

            var ext = extension.TrimStart('.').ToLowerInvariant();
            if (KnownContainers.Contains(ext))
            {
                result.Container = ext;
                if (MimeTypes.TryGetValue(ext, out var mime))
                    result.Mimetype = mime;
            }
        }
    }
}
