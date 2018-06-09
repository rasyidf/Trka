using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terka.Parsing.Tokens
{
    public enum TokenType
    {
        NotDefined,
        AudioCodec,
        AudioProfile,
        AudioChannel,
        CDS,
        VideoType,
        Subtitle,
        Torrent,
        CRC,
        Edition,
        Season,
        EpisodeDetail,
        Format,
        Part,
        SequenceTerminator,
        Size,
        Number,
        Match,
        Streammer,
        VideoCodec,
        Website,
        Title,
        Year,
        Quality,
    }
}