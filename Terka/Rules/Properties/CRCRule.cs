using System.Text.RegularExpressions;

namespace Terka.Rebulk.Rule
{
    public class CRCRule : RuleBase{
        public CRCRule()
        {
            var rb = new ReBulk(name: "crc32").Options(RegexOptions.IgnoreCase);
            rb = rb.Regex("(?:[a-fA-F]|[0-9]){8}");
            RB = rb;
        }
    }

}