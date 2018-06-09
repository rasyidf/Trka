using System.Text;

namespace Terka.Rules.Common
{
    internal class Formatter
    {
        public string seps = "";

        public Formatter()
        {
            var sb = new StringBuilder();
            foreach (var sep in seps)
                if (!_excluded_clean_chars.Contains(sep.ToString()))
                    sb.Append(sep);

            clean_chars = sb.ToString();
        }

        private static string _excluded_clean_chars = ",:;-/\\";
        private string clean_chars = "";

        public object _potential_before(int i, string input_string)
        {
            return (i - 2 >= 0) && (input_string[i] == input_string[i - 2]) &&
                  !seps.Contains(input_string[i - 1].ToString());
        }

        public object _potential_after(int i, string input_string)
        {
            return i + 2 >= input_string.Length ||
                   input_string[i + 2] == input_string[i] && !seps.Contains(input_string[i + 1].ToString());
        }

        public object Cleanup(string input_string)
        {
            return null;
        }

        public string Strip(string input_string, string[] chars)
        {
            // not implemented yet
            return "";
        }

        public object Reorder_Title(string title, string[] articles, string[] separators)
        {
            {
                var ltitle = title.ToLower();
                foreach (var article in articles)
                    foreach (var separator in separators)
                    {
                        var suffix = separator + article;
                        if (ltitle.Substring(ltitle.Length - suffix.Length) == suffix)
                            return title.Substring(ltitle.Length - suffix.Length + separator.Length) + " " + title.Substring(ltitle.Length - suffix.Length);
                    }

                return title;
            }
        }
    }
}