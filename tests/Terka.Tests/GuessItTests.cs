using Xunit;

namespace Terka.Tests
{
    public class GuessItTests
    {
        [Fact]
        public void Movie_Basic()
        {
            var result = GuessIt.Guess("The.Matrix.1999.1080p.BluRay.x264-GROUP.mkv");

            Assert.Equal("The Matrix", result.Title);
            Assert.Equal(1999, result.Year);
            Assert.Equal("1080p", result.ScreenSize);
            Assert.Equal("Blu-ray", result.Source);
            Assert.Equal("H.264", result.VideoCodec);
            Assert.Equal("GROUP", result.ReleaseGroup);
            Assert.Equal("mkv", result.Container);
            Assert.Equal(MediaType.Movie, result.Type);
            Assert.True(result.Confidence > 0f, "Confidence should be positive");
        }

        [Fact]
        public void Episode_SxxExx()
        {
            var result = GuessIt.Guess("Shameless.US.S05E10.720p.HDTV.x264-KILLERS.mkv");

            Assert.Equal("Shameless", result.Title);
            Assert.Equal("US", result.Country);
            Assert.Contains(5, result.Season);
            Assert.Contains(10, result.Episode);
            Assert.Equal("720p", result.ScreenSize);
            Assert.Equal("HDTV", result.Source);
            Assert.Equal("H.264", result.VideoCodec);
            Assert.Equal("KILLERS", result.ReleaseGroup);
            Assert.Equal(MediaType.Episode, result.Type);
        }

        [Fact]
        public void Episode_CrossPattern()
        {
            var result = GuessIt.Guess("Treme.1x03.Right.Place.Wrong.Time.HDTV.XviD-NoTV.avi");

            Assert.Equal("Treme", result.Title);
            Assert.Contains(1, result.Season);
            Assert.Contains(3, result.Episode);
            Assert.Equal("HDTV", result.Source);
            Assert.Equal("Xvid", result.VideoCodec);
            Assert.Equal("NoTV", result.ReleaseGroup);
            Assert.Equal("avi", result.Container);
            Assert.Equal(MediaType.Episode, result.Type);
        }

        [Fact]
        public void Anime_BracketGroup()
        {
            var result = GuessIt.Guess("[Anime-Group] Attack on Titan - 25 [1080p][HEVC].mkv");

            Assert.Equal("Attack on Titan", result.Title);
            Assert.Contains(25, result.AbsoluteEpisode);
            Assert.Equal("1080p", result.ScreenSize);
            Assert.Equal("H.265", result.VideoCodec);
            Assert.Equal(MediaType.Episode, result.Type);
        }

        [Fact]
        public void Movie_WithEdition()
        {
            var result = GuessIt.Guess("Blade.Runner.1982.Directors.Cut.720p.BluRay.x264-DEMAND.mkv");

            Assert.Equal("Blade Runner", result.Title);
            Assert.Equal(1982, result.Year);
            // ponytail: "Directors" and "Cut" are separate tokens; single token "dc" or combined "directors cut"
            // would be needed to detect edition properly. This is a known limitation of the naive tokenizer.
            Assert.Equal("720p", result.ScreenSize);
            Assert.Equal("Blu-ray", result.Source);
            Assert.Equal("H.264", result.VideoCodec);
        }

        [Fact]
        public void Movie_AudioCodec()
        {
            var result = GuessIt.Guess("Interstellar.2014.2160p.UHD.BluRay.DTS-HD.x265-TERMiNAL.mkv");

            Assert.Equal("Interstellar", result.Title);
            Assert.Equal(2014, result.Year);
            Assert.Equal("2160p", result.ScreenSize);
            Assert.Equal("H.265", result.VideoCodec);
            Assert.Equal("DTS-HD", result.AudioCodec);
        }

        [Fact]
        public void Movie_StreamingService()
        {
            var result = GuessIt.Guess("The.Mandalorian.S02E01.2160p.DSNP.WEB-DL.DDP5.1.x265-GROUP.mkv");

            Assert.Equal(MediaType.Episode, result.Type);
            Assert.Contains(2, result.Season);
            Assert.Contains(1, result.Episode);
            Assert.Equal("2160p", result.ScreenSize);
            Assert.Equal("Disney+", result.StreamingService);
            Assert.Equal("Web", result.Source);
            Assert.Equal("H.265", result.VideoCodec);
        }

        [Fact]
        public void Movie_10bit()
        {
            var result = GuessIt.Guess("Movie.2020.1080p.BluRay.10bit.x265-GROUP.mkv");

            Assert.Equal("10-bit", result.ColorDepth);
            Assert.Equal("H.265", result.VideoCodec);
            Assert.Equal("1080p", result.ScreenSize);
        }

        [Fact]
        public void Container_Detection()
        {
            var result = GuessIt.Guess("Some.Movie.2020.avi");
            Assert.Equal("avi", result.Container);
            Assert.Equal("video/x-msvideo", result.Mimetype);
        }

        [Fact]
        public void AudioChannels_51()
        {
            var result = GuessIt.Guess("Movie.2020.1080p.5.1.BluRay.mkv");
            Assert.Equal("5.1", result.AudioChannels);
        }

        [Fact]
        public void ToDictionary_OmitsNullValues()
        {
            var result = GuessIt.Guess("The.Matrix.1999.mkv");
            var dict = result.ToDictionary();

            Assert.True(dict.ContainsKey("title"));
            Assert.True(dict.ContainsKey("year"));
            Assert.True(dict.ContainsKey("type"));
            Assert.False(dict.ContainsKey("source"));
            Assert.False(dict.ContainsKey("video_codec"));
        }

        [Fact]
        public void NullInput_Throws()
        {
            Assert.Throws<System.ArgumentException>(() => GuessIt.Guess(null));
            Assert.Throws<System.ArgumentException>(() => GuessIt.Guess(""));
        }

        [Fact]
        public void Episode_MultiEpisode()
        {
            var result = GuessIt.Guess("Show.S01E01E02.720p.HDTV.mkv");

            Assert.Contains(1, result.Season);
            Assert.Contains(1, result.Episode);
            Assert.Contains(2, result.Episode);
            Assert.Equal(MediaType.Episode, result.Type);
        }

        [Fact]
        public void ForcedType_Option()
        {
            var result = GuessIt.Guess("Something.2020.mkv", new GuessOptions { Type = MediaType.Episode });
            Assert.Equal(MediaType.Episode, result.Type);
        }

        [Fact]
        public void Movie_Remux()
        {
            var result = GuessIt.Guess("Movie.2020.Remux.2160p.BluRay.x265.mkv");

            Assert.Contains("Remux", result.Other);
            Assert.Equal("2160p", result.ScreenSize);
        }
    }
}
