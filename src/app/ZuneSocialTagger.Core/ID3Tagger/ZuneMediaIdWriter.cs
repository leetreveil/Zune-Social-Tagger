using System;
using System.Collections.Generic;
using ID3Tag;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using System.Linq;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class ZuneMediaIdWriter
    {
        private readonly string _filePath;
        private readonly IZuneMediaIdReader _reader;

        public ZuneMediaIdWriter(string filePath, IZuneMediaIdReader reader)
        {
            _filePath = filePath;
            _reader = reader;
        }

        /// <summary>
        /// Returns the number of guids written or updated
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public int WriteMediaIdGuids(List<MediaIdGuid> guids)
        {
            int writtenOrUpdated = 0;

            IEnumerable<MediaIdGuid> needWriting = CheckWhichGuidsNeedWriting(guids);

            TagContainer container = Id3TagManager.ReadV2Tag(_filePath);

            foreach (var idGuid in needWriting)
            {
                PrivateFrame newFrame = new PrivateFrame(idGuid.MediaId, idGuid.Guid.ToByteArray());

                PrivateFrame existingFrame = (from frame in container.OfType<PrivateFrame>()
                                                where frame.Owner == newFrame.Owner
                                                select frame).FirstOrDefault();

                //if the frame already exists then remove it as we are going to be updating it
                if (existingFrame != null)
                    container.Remove(existingFrame);

                container.Add(newFrame);

                writtenOrUpdated++;
            }


            //NOTE: this is already wrapped in using statement under the covers so no need for my own
            Id3TagManager.WriteV2Tag(_filePath,container);

            return writtenOrUpdated;
        }

        private IEnumerable<MediaIdGuid> CheckWhichGuidsNeedWriting(IEnumerable<MediaIdGuid> guids)
        {
            IEnumerable<MediaIdGuid> enumerable = _reader.ReadMediaIds();
            return guids.Except(_reader.ReadMediaIds(),new MediaIdGuidComparer());
        }
    }
}