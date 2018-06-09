using System.Text.RegularExpressions;

namespace Terka.Rebulk.Rule
{
    public class EditionRule : RuleBase
    {
        public EditionRule()
        {
            var rb = new ReBulk(name: "edition").Options(RegexOptions.IgnoreCase);
            rb = rb.Regex("Collector Edition", true, "collector", "collector-edition", "edition-collector")
                    .Regex("Special Edition", true, "special-edition", "edition-special")
                    .Regex("Criterion Edition", true, "criterion-edition", "edition-criterion")
                    .Regex("Deluxe Edition", true, "deluxe", "deluxe-edition", "edition-deluxe")
                    .Regex("Limited Edition", true, "limited", "limited-edition")
                    .Regex("Theatrical Edition", true, @"theatrical-cut", @"theatrical-edition", @"theatrical")
                    .Regex("Director's Cut", true, @"director'?s?-cut", @"director'?s?-cut-edition", @"edition-director'?s?-cut")
                    .Regex("Extended", true, "extended", "extended-?cut", "extended-?version")
                    .Regex("Alternative Cut", true, "alternat(e|ive)(?:-?Cut)?")
                    .String("Director's Cut", true, "DC")
                    .String("Special Edition", true, "se")
                    .String("Festival", true, "Festival");

            foreach (var item in new[] { "Remastered", "Uncensored", "Uncut", "Unrated" })
            {
                rb = rb.String(item, true, item);
            } 
            RB = rb;
        }

    }

}