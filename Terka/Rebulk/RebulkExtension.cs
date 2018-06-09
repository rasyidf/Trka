using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Terka.Rebulk
{
    public static class RebulkExtension
    {
        public static ReBulk When(this ReBulk rb, Func<string, bool> p)
        {
            if (rb.Validators == null)
                rb.Validators = new List<Validator>();
            rb.Validators.Add(new Validator(p, Validator.ValidationPriority.Pre));
            return rb;
        }

        public static ReBulk Then(this ReBulk rb, Func<string, bool> p)
        {
            if (rb.Validators == null)
                rb.Validators = new List<Validator>();
            rb.Validators.Add(new Validator(p, Validator.ValidationPriority.Post));
            return rb;
        }

        public static ReBulk Format(this ReBulk rb, Func<string, string> function)
        {
            if (rb.Formatters == null)
                rb.Formatters = new List<Formatter>();

            rb.Formatters.Add(new Formatter(function));
            return rb;
        }

        public static ReBulk Regex(this ReBulk rb, Regex regex)
        {
            if (rb.Patterns == null)
                rb.Patterns = new List<IPattern>();
            regex = new Regex(regex.ToString(), rb.DefaultOptions);
            rb.Patterns.Add(new RegexPattern(regex));
            return rb;
        }

        public static ReBulk Regex(this ReBulk rb, params string[] regex)
        {
            if (rb.Patterns == null)
                rb.Patterns = new List<IPattern>();
            foreach (var i in regex)
            {
                rb.Patterns.Add(new RegexPattern(new Regex(i, rb.DefaultOptions), rb.Name));
            }
            return rb;
        }

        public static ReBulk Regex(this ReBulk rb, string value, bool Replace, params string[] regex)
        {
            if (rb.Patterns == null)
                rb.Patterns = new List<IPattern>();
            foreach (var i in regex)
            {
                rb.Patterns.Add(new RegexPattern(new Regex(i, rb.DefaultOptions), value));
            }
            return rb;
        }

        public static ReBulk Options(this ReBulk rb, RegexOptions options)
        {
            rb.DefaultOptions = options;
            return rb;
        }

        public static ReBulk String(this ReBulk rb, string pattern)
        {
            if (rb.Patterns == null)
                rb.Patterns = new List<IPattern>();

            rb.Patterns.Add(new StringPattern(pattern));
            return rb;
        }

        public static ReBulk String(this ReBulk rb, string value, bool replace, string pattern)

        {
            if (rb.Patterns == null)
                rb.Patterns = new List<IPattern>();

            rb.Patterns.Add(new StringPattern(pattern));
            return rb;
        }

        public static ReBulk String(this ReBulk rb, string value, bool replace, params string[] patterns)
        {
            if (rb.Patterns == null)
                rb.Patterns = new List<IPattern>();
            foreach (var i in patterns)
            {
                rb.Patterns.Add(new StringPattern(i));
            }

            return rb;
        }
    }
}