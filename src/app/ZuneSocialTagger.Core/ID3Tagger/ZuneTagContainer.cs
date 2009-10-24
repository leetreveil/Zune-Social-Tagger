using System;
using System.Collections.Generic;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using System.Linq;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class ZuneTagContainer
    {
        //TODO: Look into inherting from the Container instead of composition, may make things easier
        public ICollection<IFrame> UnderlyingContainer { get; private set; }

        public ZuneTagContainer(ICollection<IFrame> container)
        {
            UnderlyingContainer = container;
        }

        public IEnumerable<MediaIdGuid> ReadMediaIds()
        {
            //OfType instead of cast because the container could contain other types other than private frames 
            //and we only want private frames
            return from frame in UnderlyingContainer.OfType<PrivateFrame>()
                   where MediaIds.Ids.Contains(frame.Owner)
                   select new MediaIdGuid { MediaId = frame.Owner, Guid = new Guid(frame.Data) };
        }

        /// <summary>
        /// Returns the number of guids added or updated
        /// </summary>
        /// <param name="guids">The guids that you want writing to file</param>
        /// <returns></returns>
        public int WriteMediaIdGuidsToContainer(List<MediaIdGuid> guids)
        {
            //TODO: change this method to add, and not write a group of guids,
            //would be much simpler

            int tagsAddedOrUpdated = 0;

            foreach (var idGuid in CheckWhichGuidsNeedWriting(guids))
            {
                PrivateFrame newFrame = new PrivateFrame(idGuid.MediaId, idGuid.Guid.ToByteArray());

                PrivateFrame existingFrame = (from frame in UnderlyingContainer.OfType<PrivateFrame>()
                                              where frame.Owner == newFrame.Owner
                                              select frame).FirstOrDefault();

                //if the frame already exists then remove it as we are going to be updating it
                if (existingFrame != null)
                    UnderlyingContainer.Remove(existingFrame);

                UnderlyingContainer.Add(newFrame);
                tagsAddedOrUpdated++;
            }

            return tagsAddedOrUpdated;
        }

        private IEnumerable<MediaIdGuid> CheckWhichGuidsNeedWriting(IEnumerable<MediaIdGuid> guids)
        {
            return guids.Except(this.ReadMediaIds(), new MediaIdGuidComparer());
        }

        public MetaData ReadMetaData()
        {
            string[] metaDataIds = new string[]{"TALB","TPE1","TIT2","TYER"};

            var metaDataFrames = from frame in UnderlyingContainer.OfType<TextFrame>()
                                 where metaDataIds.Contains(frame.Descriptor.ID)
                                 select new {Id = frame.Descriptor.ID, Text = frame.Content};

            //TODO: Code smell, do not like this whole switchy business at all

            var metaData = new MetaData();

            foreach (var frame in metaDataFrames)
            {
                switch (frame.Id)
                {
                    case "TPE1":
                        metaData.AlbumArtist = frame.Text;
                        break;

                    case "TALB":
                        metaData.AlbumTitle = frame.Text;
                        break;

                    case "TIT2":
                        metaData.SongTitle = frame.Text;
                        break;

                    case "TYER":
                        metaData.Year = frame.Text;
                        break;
                }
            }

            return metaData;
        }
    }
}