using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terka.Parsing.Tokens;
using Terka.Tokenizers;

namespace Terka.Parsing.Tokenizers
{
    public class Tokenizer : ITokenizer
    {
        private readonly List<TokenDefinition> _tokenDefinitions;

        public Tokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>
            {
                new TokenDefinition(TokenType.Year, "(19|20)[0-9][0-9]",1),
                new TokenDefinition(TokenType.AudioCodec, "MP3|Dolby|Dolby-?(Atmos|Digital)|DD|AC3D?|AAC|EAC3|Flac|DTS|True-?HD", 1),
                new TokenDefinition(TokenType.AudioProfile, @"(HD|HD-?MA|LC|HQ)^\d", 1),
                new TokenDefinition(TokenType.AudioChannel, @"(7[\W_][01](?:ch)?)(?:[^\d]|$)|(5[\W_][01](?:ch)?)(?:[^\d]|$)|(2[\W_]0(?:ch)?)(?:[^\d]|$)|5ch|2ch|stereo|1ch|mono", 1),
                new TokenDefinition(TokenType.CDS, @"cd-?(?<cd>\d+)(?:-?of-?(?<cd_count>\d+))?", 1),
                new TokenDefinition(TokenType.VideoType, @"\.(3g2|3gp|3gp2|asf|avi|divx|flv|m4v|mk2|mka|mkv|mov|mp4|mp4a|mpeg|mpg|ogg|ogm|ogv|qt|ra|ram|rm|ts|wav|webm|wma|wmv|iso|vob)"),
                new TokenDefinition(TokenType.Subtitle, "srt|idx|sub|ssa|ass"),
                new TokenDefinition(TokenType.Torrent, "torrent"),
                new TokenDefinition(TokenType.CRC, "(?<crc>[a-fA-F]|[0-9]){8}"),
                new TokenDefinition(TokenType.Edition, "collector|special-edition|se|criterion-edition|deluxe|limited|theatrical-cut|theatrical-edition|director'?s?-cut|extended|alternat(e|ive)(?:-?Cut)|Remastered|Uncensored|Uncut|Unrated|Festival"),
                new TokenDefinition(TokenType.Season, @"S(?<season>\d+)@?E@?(?<episode>\d+)|(?<season>\d+)@?x@?(?<episode>\d+)"),
                new TokenDefinition(TokenType.EpisodeDetail, @"Special|Bonus|Omake|Ova|Oav|Pilot|Unaired"),
                new TokenDefinition(TokenType.Format, "TELESYNC|TS|HD-?TS|WORKPRINT|WP|TELECINE|TC|PPV|PPV-?Rip|SD-?TV|SD-?TV-?Rip|Rip-?SD-?TV|TV-?Rip|DVB-?Rip|DVB|PD-?TV|DVD|DVD-?Rip|VIDEO-?TS|DVD-?R(?:$|(?!E))|HD-?TV|TV-?RIP-?HD|HD-?TV-?RIP|HD-?RIP|VOD-?Rip|WEB-?Rip|WEB-?DL|HD-?DVD-?Rip|Blu-?ray(?:-?Rip)?|AHDTV|UHD-?TV|HDTC|DSR"),
                new TokenDefinition(TokenType.Part, new[] { "pt" + Regexes.digital_numeral, "part" + Regexes.digital_numeral }),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?360(?:i|p?x?)", Value: "360p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?368(?:i|p?x?)", Value: "368p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?480(?:i|p?x?)", Value: "480p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?576(?:i|p?x?)", Value: "576p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?720(?:i|p?(?:50|60)?x?)", Value: "720p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?720(?:p(?:50|60)?x?)", Value: "720p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?720p?hd", Value: "720p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?900(?:i|p?x?)", Value: "900p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?1080i", Value: "1080i"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?1080p?x?", Value: "1080p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?1080(?:p(?:50|60)?x?)", Value: "1080p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?1080p?hd", Value: "1080p"),
                new TokenDefinition(TokenType.Quality, @"(?:\d{3,}(?:x|\*))?2160(?:i|p?x?)", Value: "4K"),
                new TokenDefinition(TokenType.Quality, "4k", Value: "4K"),

                new TokenDefinition(TokenType.Size, new[] { @"\d+\.?[mgt]b", @"\d+\.\d+[mgt]b" }),

                new TokenDefinition(TokenType.Streammer, "Adult-Swim", Value: "Adult Swim"),
                new TokenDefinition(TokenType.Streammer, "Amazon-Prime", Value: "Amazon Prime"),
                new TokenDefinition(TokenType.Streammer, "BBC-iPlayer", Value: "BBC iPlayer"),
                new TokenDefinition(TokenType.Streammer, "Comedy-Central", Value: "Comedy Central"),
                new TokenDefinition(TokenType.Streammer, "Crunchy-Roll", Value: "Crunchy Roll"),
                new TokenDefinition(TokenType.Streammer, "HBO-Go", Value: "HBO Go"),
                new TokenDefinition(TokenType.Streammer, "NBA-TV", Value: "NBA TV"),
                new TokenDefinition(TokenType.Streammer, "National-Geographic", Value: "National Geographic"),
                new TokenDefinition(TokenType.Streammer, "The-CW", Value: "The CW"),
                new TokenDefinition(TokenType.Streammer, new[] { "AE", "A&E" }, Value: "A&E"),
                new TokenDefinition(TokenType.Streammer, new[] { "AMBC" }, Value: "ABC"),
                new TokenDefinition(TokenType.Streammer, new[] { "AMC" }, Value: "AMC"),
                new TokenDefinition(TokenType.Streammer, new[] { "AMZN", "AmazonPrime" }, Value: "Amazon Prime"),
                new TokenDefinition(TokenType.Streammer, new[] { "AS", "AdultSwim" }, Value: "Adult Swim"),
                new TokenDefinition(TokenType.Streammer, new[] { "CBS" }, Value: "CBS"),
                new TokenDefinition(TokenType.Streammer, new[] { "CC", "ComedyCentral" }, Value: "Comedy Central"),
                new TokenDefinition(TokenType.Streammer, new[] { "CR", "CrunchyRoll" }, Value: "Crunchy Roll"),
                new TokenDefinition(TokenType.Streammer, new[] { "CW", "TheCW" }, Value: "The CW"),
                new TokenDefinition(TokenType.Streammer, new[] { "DISC", "Discovery" }, Value: "Discovery"),
                new TokenDefinition(TokenType.Streammer, new[] { "DIY" }, Value: "DIY Network"),
                new TokenDefinition(TokenType.Streammer, new[] { "DSNY", "Disney" }, Value: "Disney"),
                new TokenDefinition(TokenType.Streammer, new[] { "EPIX", "ePix" }, Value: "ePix"),
                new TokenDefinition(TokenType.Streammer, new[] { "HBO", "HBOGo" }, Value: "HBO Go"),
                new TokenDefinition(TokenType.Streammer, new[] { "HIST", "History" }, Value: "History"),
                new TokenDefinition(TokenType.Streammer, new[] { "ID" }, Value: "Investigation Discovery"),
                new TokenDefinition(TokenType.Streammer, new[] { "IFC", "IFC" }, Value: "IFC"),
                new TokenDefinition(TokenType.Streammer, new[] { "NATG", "NationalGeographic" }, Value: "National Geographic"),
                new TokenDefinition(TokenType.Streammer, new[] { "NBA", "NBATV" }, Value: "NBA TV"),
                new TokenDefinition(TokenType.Streammer, new[] { "NBC" }, Value: "NBC"),
                new TokenDefinition(TokenType.Streammer, new[] { "NF", "Netflix" }, Value: "Netflix"),
                new TokenDefinition(TokenType.Streammer, new[] { "NFL" }, Value: "NFL"),
                new TokenDefinition(TokenType.Streammer, new[] { "NICK", "Nickelodeon" }, Value: "Nickelodeon"),
                new TokenDefinition(TokenType.Streammer, new[] { "PBS", "PBS" }, Value: "PBS"),
                new TokenDefinition(TokenType.Streammer, new[] { "RTE" }, Value: "RTÉ One"),
                new TokenDefinition(TokenType.Streammer, new[] { "SESO", "SeeSo" }, Value: "SeeSo"),
                new TokenDefinition(TokenType.Streammer, new[] { "SPKE", "SpikeTV", "Spike TV" }, Value: "Spike TV"),
                new TokenDefinition(TokenType.Streammer, new[] { "SYFY", "Syfy" }, Value: "Syfy"),
                new TokenDefinition(TokenType.Streammer, new[] { "TFOU", "TFou" }, Value: "TFou"),
                new TokenDefinition(TokenType.Streammer, new[] { "TLC" }, Value: "TLC"),
                new TokenDefinition(TokenType.Streammer, new[] { "TV3" }, Value: "TV3 Ireland"),
                new TokenDefinition(TokenType.Streammer, new[] { "TV4" }, Value: "TV4 Sweeden"),
                new TokenDefinition(TokenType.Streammer, new[] { "TVL", "TVLand", "TV Land" }, Value: "TV Land"),
                new TokenDefinition(TokenType.Streammer, new[] { "UFC" }, Value: "UFC"),
                new TokenDefinition(TokenType.Streammer, new[] { "USAN" }, Value: "USA Network"),
                new TokenDefinition(TokenType.Streammer, new[] { "iP", "BBCiPlayer" }, Value: "BBC iPlayer"),
                new TokenDefinition(TokenType.Streammer, new[] { "iTunes" }, Value: "iTunes"),

                new TokenDefinition(TokenType.VideoCodec, @"Rv\d{2}", Value: "Real"),
                new TokenDefinition(TokenType.VideoCodec, "Mpeg2", Value: "Mpeg2"),
                new TokenDefinition(TokenType.VideoCodec, new[] { "DVDivX", "DivX" }, Value: "DivX"),
                new TokenDefinition(TokenType.VideoCodec, "XviD", Value: "XviD"),
                new TokenDefinition(TokenType.VideoCodec, new[] { "[hx]-?264(?:-?AVC(HD)?)?", "MPEG-?4(?:-?AVC(HD)?)", "AVC(?:HD)?" }, Value: "h264"),
                new TokenDefinition(TokenType.VideoCodec, "[hx]-?265(?:-?HEVC)?|HEVC", Value: "h265"),
                new TokenDefinition(TokenType.VideoCodec, "(?<video_codec>hevc)(?<video_profile>10)", Value: "h265 10bit") ,
                new TokenDefinition(TokenType.VideoCodec, new[] { "10.?bits?", "Hi10P?", "YUV420P10" }, Value: "10bit"),
                new TokenDefinition(TokenType.VideoCodec, "8.?bits?", Value: "8bit"),
                new TokenDefinition(TokenType.VideoCodec, "BP", Value: "BP"),
                new TokenDefinition(TokenType.VideoCodec, "XP|EP", Value: "XP"),
                new TokenDefinition(TokenType.VideoCodec, "MP", Value: "MP"),
                new TokenDefinition(TokenType.VideoCodec, "HP|HiP", Value: "HP"),
                new TokenDefinition(TokenType.VideoCodec, "Hi422P", Value: "Hi422P"),
                new TokenDefinition(TokenType.VideoCodec, "Hi444PP", Value: "Hi444PP"),
                new TokenDefinition(TokenType.Website, @"(?:[^a-z0-9]|^)((?:www\.)*[a-z-]+)\.(?:com|org|net)(?:[^a-z0-9]|$)|(?:[^a-z0-9]|^)((?:www\.)*[a-z-]+)\.(?:co|com|org|net)\.(?:[a-z]+)(?:[^a-z0-9]|$)"),
        };
        }

        public IEnumerable<MovieToken> Tokenize(string query)
        {
            var tokenMatches = FindTokenMatches(query);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            var mavie = groupedByIndex[0].OrderBy(x => x.Precedence).First();
            yield return new MovieToken(TokenType.Title, query.Replace(".", " ").Substring(0, mavie.StartIndex - 1));
            for (int i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return new MovieToken(bestMatch.TokenType, bestMatch.Value);

                lastMatch = bestMatch;
            }

            yield return new MovieToken(TokenType.SequenceTerminator);
        }

        private List<TokenMatch> FindTokenMatches(string query)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _tokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(query).ToList());

            return tokenMatches;
        }
    }
}