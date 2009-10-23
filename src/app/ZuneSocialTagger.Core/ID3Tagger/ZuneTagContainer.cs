using System;
using System.Collections.Generic;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using System.Linq;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class ZuneTagContainer
    {
        private readonly TagContainer _container;

        public ZuneTagContainer(TagContainer container)
        {
            _container = container;
        }

        public IEnumerable<MediaIdGuid> ReadMediaIds()
        {
            //OfType instead of cast because the container could contain other types other than private frames 
            //and we only want private frames
            //TODO: can probably remove the frametype check becuase of oftype
            return from frame in _container.OfType<PrivateFrame>()
                   where frame.Type == FrameType.Private && MediaIds.Ids.Contains(frame.Owner)
                   select new MediaIdGuid { MediaId = frame.Owner, Guid = new Guid(frame.Data) };
        }

        /// <summary>
        /// Returns the number of guids added or updated
        /// </summary>
        /// <param name="guids">The guids that you want writing to file</param>
        /// <returns></returns>
        public int WriteMediaIdGuidsToContainer(List<MediaIdGuid> guids)
        {
            int tagsAddedOrUpdated = 0;



            foreach (var idGuid in CheckWhichGuidsNeedWriting(guids))
            {
                PrivateFrame newFrame = new PrivateFrame(idGuid.MediaId, idGuid.Guid.ToByteArray());

                PrivateFrame existingFrame = (from frame in _container.OfType<PrivateFrame>()
                                              where frame.Owner == newFrame.Owner
                                              select frame).FirstOrDefault();

                //if the frame already exists then remove it as we are going to be updating it
                if (existingFrame != null)
                    _container.Remove(existingFrame);

                _container.Add(newFrame);
                tagsAddedOrUpdated++;
            }

            return tagsAddedOrUpdated;
        }

        private IEnumerable<MediaIdGuid> CheckWhichGuidsNeedWriting(IEnumerable<MediaIdGuid> guids)
        {
            return guids.Except(this.ReadMediaIds(), new MediaIdGuidComparer());
        }
    }
}