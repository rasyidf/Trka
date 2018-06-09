using Terka.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terka.Parsing.Tokenizers
{
    public interface ITokenizer
    {
        IEnumerable<MovieToken> Tokenize(string query);
    }
}