using System.Collections.Generic;

namespace Terka.Rebulk
{
    public class Match
    {
        public Match(int start, int end, string name, params string[] tags)
        {
            Name = name;
            Start = start;
            End = end;
            Tags = new List<string>(tags);
        }

        public string Name { get; set; }

        public int Start { get; set; }
        public int End { get; set; }

        public override string ToString()
        {
            return $"[<{Start},{End}> {Name} ({string.Join(",", Tags)})]";
        }

        public List<string> Tags { get; set; }
    }
}