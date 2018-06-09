using System;
using System.Collections.Generic;

namespace Terka.Rebulk
{
    public class StringPattern : IPattern
    {
        public StringPattern(string pattern)
        {
            this.Pattern = pattern;
        }

        public string Pattern { get; set; }

        public IEnumerable<Match> Match(string input, string Name = "")
        {
            var tokens = input.Split("[]{}() ,.".ToCharArray());
            foreach (var index in Find_All(input, Pattern, 0, input.Length, true))
            {
                yield return new Match(index, tokens.Length, Name, Pattern);
            }
        }

        public static IEnumerable<int> Find_All(string input, string pattern, int start = 0, int end = 0, bool ignore_case = false)
        {
            //pylint: disable=unused-argument
            if (ignore_case)
            {
                pattern = pattern.ToLower();
                input = input.ToLower();
            }
            var tokens = input.Split("[]{}() ,.".ToCharArray());
            while (true)
            {
                start = Array.FindIndex(tokens, start, (tokens.Length < end ? tokens.Length - 1 : end - 1), x => x == pattern);
                if (start == -1)
                {
                    break;
                }
                yield return start;
                start += pattern.Length;
            }
        }
    }
}