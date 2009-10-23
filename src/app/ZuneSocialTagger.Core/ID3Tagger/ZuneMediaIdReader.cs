using System;
using System.Collections;
using System.Collections.Generic;
using ID3Tag;
using ID3Tag.HighLevel;
using System.Linq;
using ID3Tag.HighLevel.ID3Frame;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class ZuneMediaIdReader : IZuneMediaIdReader
    {
        private readonly string _filePath;

        public ZuneMediaIdReader(string filePath)
        {           
            //TODO: move the file path out the ctor
            _filePath = filePath;
        }

        public IEnumerable<MediaIdGuid> ReadMediaIds()
        {
            TagContainer container = Id3TagManager.ReadV2Tag(_filePath);

            //OfType instead of cast because the container could container other types other than private frames 
            //and we only want private frames
            //TODO: can probably remove the frametype check becuase of oftype
            return from frame in container.OfType<PrivateFrame>()
                         where frame.Type == FrameType.Private && ZuneFrameIds.Ids.Contains(frame.Owner)
                         select new MediaIdGuid{ MediaId = frame.Owner, Guid = new Guid(frame.Data)};
        }
    }

}


