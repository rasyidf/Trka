using System;
using System.Collections.Generic;
using System.Linq;
using Terka.Shared;

namespace Terka.Matchers
{
    /// <summary>
    /// Detects container format from the file extension.
    /// Unlike other matchers, this operates on the extension, not on tokens.
    /// </summary>
    internal class ContainerMatcher
    {
        // Start from shared vocabulary, add extras specific to base Terka
        private static readonly Dictionary<string, string> MimeTypes = BuildMimeTypes();

        private static Dictionary<string, string> BuildMimeTypes()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in Vocabulary.Containers)
                map[entry.Key] = entry.Value;

            // Additional entries not in shared vocabulary
            map["mk3d"] = "video/x-matroska-3d";
            map["mka"] = "audio/x-matroska";
            map["ogg"] = "video/ogg";
            map["ogm"] = "video/ogg";
            map["3g2"] = "video/3gpp2";
            map["asf"] = "video/x-ms-asf";
            map["rm"] = "application/vnd.rn-realmedia";
            map["divx"] = "video/x-msvideo";
            map["srt"] = "text/srt";
            map["ssa"] = "text/x-ssa";
            map["mp3"] = "audio/mpeg";
            map["wav"] = "audio/x-wav";
            map["wma"] = "audio/x-ms-wma";
            map["ra"] = "audio/vnd.rn-realaudio";
            map["ram"] = "audio/vnd.rn-realaudio";
            map["flac"] = "audio/flac";
            return map;
        }

        // Shared known extensions + extras (archives, images, etc.)
        private static readonly HashSet<string> KnownContainers = BuildKnownSet();

        private static HashSet<string> BuildKnownSet()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var ext in Vocabulary.KnownExtensions)
                set.Add(ext);

            // Additional non-video containers the base library recognizes
            foreach (var ext in new[]
            {
                "3gp2", "7z", "bmp", "bz2", "cb7", "cbr", "cbz",
                "gif", "gz", "idx", "iso", "jpeg", "jpg", "mk2",
                "mp4a", "nfo", "nzb", "png", "qt", "r00", "ra",
                "ram", "rar", "tar", "tbn", "tgz", "torrent",
                "webp", "wma", "zip"
            })
            {
                set.Add(ext);
            }

            return set;
        }

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
