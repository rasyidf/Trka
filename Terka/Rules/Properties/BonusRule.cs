using System.Text.RegularExpressions;

namespace Terka.Rebulk.Rule
{
    public class BonusRule : RuleBase
    {
        public BonusRule()
        {
            //var rb = new ReBulk(name: "bonus").Options(RegexOptions.IgnoreCase);
            //rb = rb.Regex(@"x(\d+)", "bonus");
            //RB = rb;
        }
    }

    public class VideoCodecRule : RuleBase
    {
        public VideoCodecRule()
        {
            RB = new ReBulk(name: "video_codec").Options(RegexOptions.IgnoreCase);

            RB = RB.Regex("Real", true, @"Rv\d{2}");
            RB = RB.Regex("Mpeg2", true, "Mpeg2");
            RB = RB.Regex("DivX", true, "DVDivX", "DivX");
            RB = RB.Regex("XviD", true, "XviD");
            RB = RB.Regex("h264", true, "[hx]-?264(?:-?AVC(HD)?)?", "MPEG-?4(?:-?AVC(HD)?)", "AVC(?:HD)?");
            RB = RB.Regex("h265", true, "[hx]-?265(?:-?HEVC)?", "HEVC");
            RB = RB.Regex("h26510bit", true, "(?<video_codec>hevc)(?<video_profile>10)");

            RB = RB.Regex("10bit", true, "10.?bits?", "Hi10P?", "YUV420P10");
            RB = RB.Regex("8bit", true, "8.?bits?");
            RB = RB.String("BP", true, "BP");
            RB = RB.String("XP", true, "EP", "XP");
            RB = RB.String("MP", true, "MP");
            RB = RB.String("HP", true, "HiP", "HP");
            RB = RB.Regex("Hi422P", true, "Hi422P");
            RB = RB.Regex("Hi444PP", true, "Hi444PP");
            RB = RB.String("DXVA", true, "DXVA");
        }
    }
}