using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terka.Parsing.Tokens;

namespace Terka.Parsing.Tokenizers
{
    public class TokenDefinition
    {
        private readonly Regex _regex;
        private readonly TokenType _returnsToken;
        private readonly int _precedence;
        public string Value { get; set; }

        public TokenDefinition(TokenType returnsToken, string regexPattern, int precedence, string Value = "")
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _returnsToken = returnsToken;
            _precedence = precedence;
            this.Value = Value;
        }

        public TokenDefinition(TokenType returnsToken, string regexPattern, string Value = "") : this(returnsToken, regexPattern, 1, Value)
        {
        }

        public TokenDefinition(TokenType returnsToken, string[] regexPatterns)
        {
            var regexPattern = String.Join("|", regexPatterns);

            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _returnsToken = returnsToken;
            _precedence = 1;
        }

        public TokenDefinition(TokenType returnsToken, string[] regexPatterns, string Value) : this(returnsToken, regexPatterns)
        {
            this.Value = Value;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = _regex.Matches(inputString);
            for (int i = 0; i < matches.Count; i++)
            {
                yield return new TokenMatch()
                {
                    StartIndex = matches[i].Index,
                    EndIndex = matches[i].Index + matches[i].Length,
                    TokenType = _returnsToken,
                    Value = String.IsNullOrEmpty(Value) ? matches[i].Value : Value,
                    Precedence = _precedence
                };
            }
        }
    }
}