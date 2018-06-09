using System.Text.RegularExpressions;

namespace Terka.Rebulk.Rule
{
    public class CDSRule : RuleBase
    {
        public CDSRule()
        {
            var rb = new ReBulk(name: "cds").Options(RegexOptions.IgnoreCase);
            rb = rb.Regex(@"cd-?(?<cd>\d+)(?:-?of-?(?<cd_count>\d+))?");
            rb = rb.Regex(@"(?<cd_count>\d+)-?cds?");
            RB = rb;
        }
    }
}