using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terka.Parsing.Tokens
{
    public class MovieToken
    {
        public MovieToken(TokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public MovieToken(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public TokenType TokenType { get; set; }
        public string Value { get; set; }

        public MovieToken Clone()
        {
            return new MovieToken(TokenType, Value);
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Value))
            {
                return $"{ Enum.GetName(typeof(Tokens.TokenType), this.TokenType)}";
            }
            return $"{Enum.GetName(typeof(Tokens.TokenType), this.TokenType)}\t\t: {Value}";
        }
    }
}