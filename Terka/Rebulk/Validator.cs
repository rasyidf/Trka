using System;

namespace Terka.Rebulk
{
    public class Validator
    {
        private Func<string, bool> p;

        public ValidationPriority Priority { get; private set; }

        public Validator(Func<string, bool> p, ValidationPriority post)
        {
            this.p = p;
            this.Priority = post;
        }

        public enum ValidationPriority
        {
            Pre,
            Post
        }

        internal bool Validate(string v)
        {
            if (p == null)
            {
                p = (x) => true;
            }
            return p(v);
        }
    }
}