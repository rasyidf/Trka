using System.Collections.Generic;

namespace Terka.Rebulk
{
    public interface IPattern
    {
        IEnumerable<Match> Match(string input, string Name);
    }
}