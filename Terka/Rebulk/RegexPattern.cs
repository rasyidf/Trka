using System.Collections.Generic;
using re = System.Text.RegularExpressions;

namespace Terka.Rebulk
{
    public class RegexPattern : IPattern
    {
        public RegexPattern(re.Regex regex, string value = "")
        {
            this.Value = value;
            this.Pattern = regex;
        }

        public re.Regex Pattern { get; set; }
        public string Value { get; set; }

        public IEnumerable<Match> Match(string input, string Name = "")
        {
            foreach (re.Match item in Pattern.Matches(input))
            {
                yield return new Match(item.Index, item.Index + item.Length, Name, item.Value);
            }
        }
    }
}