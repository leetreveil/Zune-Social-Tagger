using System;
using System.Collections;
using System.Collections.Generic;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using System.Linq;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    /// <summary>
    /// Updates a pre-existing TagContainer with new zune PRIV tags
    /// </summary>
    public class ZuneTagContainer : IEnumerable<IFrame>
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
            return from frame in _container.OfType<PrivateFrame>()
                   where MediaIds.Ids.Contains(frame.Owner)
                   select new MediaIdGuid { MediaId = frame.Owner, Guid = new Guid(frame.Data) };
        }

        public void Add(MediaIdGuid guid)
        {
            PrivateFrame newFrame = new PrivateFrame(guid.MediaId, guid.Guid.ToByteArray());

            PrivateFrame existingFrame = (from frame in _container.OfType<PrivateFrame>()
                                          where frame.Owner == newFrame.Owner
                                          select frame).FirstOrDefault();

            //TODO: we are not checking whether the actual guid is the same before removing then adding
            //would be better to check than removing and adding the same data again

            //if the frame already exists then remove it as we are going to be updating it
            if (existingFrame != null)
                _container.Remove(existingFrame);

            _container.Add(newFrame);
        }

        public MetaData ReadMetaData()
        {
            string[] metaDataIds = new[]{"TALB","TPE1","TIT2","TYER"};

            var metaDataFrames = from frame in _container.OfType<TextFrame>()
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

        public IEnumerator<IFrame> GetEnumerator()
        {
            return this._container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}