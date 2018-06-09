using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terka.Parsing.Tokens;

namespace Terka.Parsing
{
    public class Parser
    {
        private Stack<MovieToken> _tokenSequence;
        private MovieToken _lookaheadFirst;
        private MovieToken _lookaheadSecond;

        public Movie Parse(List<MovieToken> tokens)
        {
            Movie _movie;
            LoadSequenceStack(tokens);
            PrepareLookaheads();
            _movie = new Movie();

            Match();

            DiscardToken(TokenType.SequenceTerminator);

            return _movie;
        }

        private void Match()
        {
            // Left blank
        }

        private void LoadSequenceStack(List<MovieToken> tokens)
        {
            _tokenSequence = new Stack<MovieToken>();
            int count = tokens.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                _tokenSequence.Push(tokens[i]);
            }
        }

        private void PrepareLookaheads()
        {
            _lookaheadFirst = _tokenSequence.Pop();
            _lookaheadSecond = _tokenSequence.Pop();
        }

#pragma warning disable S1144 // Unused private types or members should be removed

        private MovieToken ReadToken(TokenType tokenType)
        {
            if (_lookaheadFirst.TokenType != tokenType)
                throw new ParserException(string.Format("Expected {0} but found: {1}", tokenType.ToString().ToUpper(), _lookaheadFirst.Value));

            return _lookaheadFirst;
        }

#pragma warning restore S1144 // Unused private types or members should be removed

        private void DiscardToken()
        {
            _lookaheadFirst = _lookaheadSecond.Clone();

            if (_tokenSequence.Any())
                _lookaheadSecond = _tokenSequence.Pop();
            else
                _lookaheadSecond = new MovieToken(TokenType.SequenceTerminator, string.Empty);
        }

        private void DiscardToken(TokenType tokenType)
        {
            if (_lookaheadFirst.TokenType != tokenType)
                throw new ParserException(string.Format("Expected {0} but found: {1}", tokenType.ToString().ToUpper(), _lookaheadFirst.Value));

            DiscardToken();
        }
    }
}