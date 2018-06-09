using System;
using System.Text.RegularExpressions;
using Terka.Tokenizers;

namespace Terka.Rebulk.Rule
{
    public class PartRule : RuleBase
    {
        public PartRule()
        {
            var prefixes = new[]  {
                "pt",
                "part"
            };
            RB = new ReBulk(name: "part").Options(RegexOptions.IgnoreCase)
                        .Regex(RegexUtils.BuildOrPattern(prefixes, "prefixes") + @"-?(?<part>" + RegexUtils.BuildOrPattern(new[] { Regexes.digital_numeral, Regexes.roman_numeral }, "partnumber") + @")")
                        .Then(x => IsNumeric(x));
        }

        private bool IsNumeric(string x)
        {
            return Int32.TryParse(x, out int _);
        }
    }
}