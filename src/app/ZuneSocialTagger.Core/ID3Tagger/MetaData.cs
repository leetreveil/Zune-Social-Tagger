using System.Collections.Generic;
using ID3Tag.HighLevel.ID3Frame;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class MetaData
    {
        public string AlbumArtist { get; set; }
        public string AlbumTitle { get; set; }
        public string Year { get; set; }
        public string SongTitle { get; set; }


        public static MetaData CreateMetaDataFrom(IEnumerable<TextFrame> textFrames)
        {
            var metaData = new MetaData();
            
            foreach (var frame in textFrames)
            {
                switch (frame.Descriptor.ID)
                {
                    case "TPE1":
                        metaData.AlbumArtist = frame.Content;
                        break;

                    case "TALB":
                        metaData.AlbumTitle = frame.Content;
                        break;

                    case "TIT2":
                        metaData.SongTitle = frame.Content;
                        break;

                    case "TYER":
                        metaData.Year = frame.Content;
                        break;
                }
            }

            return metaData;
        }
    }
}