using System;

namespace Terka.Rebulk
{
    public class Formatter
    {
        private Func<string, string> function;

        public Formatter(Func<string, string> function)
        {
            this.function = function;
        }

        public string Apply(string input)
        {
            if (function == null)
            {
                function = (x => x);
            }
            return function(input);
        }
    }
}