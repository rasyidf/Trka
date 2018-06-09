using System.Collections.Generic;
using Terka.Rebulk.Rule;

namespace Terka.Rebulk
{
    public abstract class RuleBase
    {
        internal ReBulk RB { get; set; }

        public IEnumerable<Rebulk.Match> Match(string input)
        {
            return RB.Match(RB.Reformat(input));
        }

        public RuleBase()
        {
            RB = new ReBulk("");
        }

        public static IEnumerable<RuleBase> GetRules()
        {
            yield return new VideoCodecRule();
            yield return new BonusRule();
            yield return new CDSRule();
            yield return new CRCRule();
            yield return new PartRule();
            yield return new DateRule();
            yield return new SizeRule();
            yield return new ScreenSizeRule();
        }
    }
}