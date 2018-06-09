using System.Text.RegularExpressions;

namespace Terka.Rebulk.Rule
{
    public class DateRule : RuleBase
    {
        public DateRule()
        {
            RB = new ReBulk(name: "year").Options(RegexOptions.IgnoreCase)
                         .Regex(@"\d{4}").Then(x => Regex.Match(x, "19|20([0-9]{2})").Success);
        }
    }
}