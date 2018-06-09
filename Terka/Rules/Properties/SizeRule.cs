using System.Text.RegularExpressions;

namespace Terka.Rebulk.Rule
{
    public class SizeRule : RuleBase
    {
        public SizeRule()
        {
            var rb = new ReBulk(name: "size")
                        .Regex(@"\d+\.?[mgt]b", @"\d+\.\d+[mgt]b")
                        .Format((x) => Regex.Replace(x, @"[mgt]b", y => y.ToString().ToUpper()));
            RB = rb;
        }
    }

    public class ScreenSizeRule : RuleBase
    {
        public ScreenSizeRule()
        {
            RB = new ReBulk(name: "screen_size").Options(RegexOptions.IgnoreCase)
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?360(?:i|p?x?)", value: "360p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?368(?:i|p?x?)", value: "368p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?480(?:i|p?x?)", value: "480p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?576(?:i|p?x?)", value: "576p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?720(?:i|p?(?:50|60)?x?)", value: "720p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?720(?:p(?:50|60)?x?)", value: "720p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?720p?hd", value: "720p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?900(?:i|p?x?)", value: "900p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?1080i", value: "1080i")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?1080p?x?", value: "1080p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?1080(?:p(?:50|60)?x?)", value: "1080p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?1080p?hd", value: "1080p")
                                .Regex(Replace: true, regex: @"(?:\d{3,}(?:x|\*))?2160(?:i|p?x?)", value: "4K")
                                .String("4k", true, "4K");
        }
    }
}