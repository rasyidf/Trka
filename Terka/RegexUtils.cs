using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Terka
{
    internal static class RegexUtils
    {
        public static string BuildOrPattern(string[] patterns, string name = "", bool escape = false)
        {
            StringBuilder newpattern = new StringBuilder();
            foreach (var pattern in patterns)
            {
                if (newpattern.Length == 0)
                {
                    newpattern.Append("(?");
                    var namepattern = string.IsNullOrEmpty(name) ? $"<{name}>" : ":";
                    newpattern.Append(namepattern);
                }
                else
                    newpattern.Append('|');
                var rex = escape ? Regex.Escape(pattern) : pattern;
                newpattern.Append($"(?:{rex})");
            }
            newpattern.Append(')');
            return newpattern.ToString();
        }
    }
}