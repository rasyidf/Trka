using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Terka.Rebulk
{
    public class ReBulk
    {
        public string Name { get; set; }

        public string[] Tags { get; set; }

        internal IEnumerable<Match> Match(string v)
        {
            var a = v;
            if (Patterns == null)
                Patterns = new List<IPattern>();
            foreach (var item in Patterns)
            {
                foreach (var m in item.Match(a, Name))
                {
                    if (Validate(m.Tags[0]))
                    {
                        yield return m;
                    }
                }
            }
        }

        public bool Validate(string v)
        {
            var Validates = true;
            if (Validators.Count == 0) return true;
            foreach (Validator i in Validators)
            {
                if (i.Priority == Validator.ValidationPriority.Post)
                {
                    Validates = Validates && i.Validate(v);
                }
            }
            return Validates;
        }

        public string Reformat(string input)
        {
            var s = input;
            foreach (var i in Formatters)
            {
                s = i.Apply(s);
            }
            return s;
        }

        public ReBulk(string name, params string[] tags)
        {
            this.Name = name;
            Formatters = new List<Formatter>();
            Validators = new List<Validator>();
            Tags = tags;
        }

        public ReBulk()
        {
            Formatters = new List<Formatter>();
            Validators = new List<Validator>();
        }

        public List<IPattern> Patterns { get; set; }
        public List<Formatter> Formatters { get; set; }
        public RegexOptions DefaultOptions { get; internal set; }
        public List<Validator> Validators { get; internal set; }
    }
}