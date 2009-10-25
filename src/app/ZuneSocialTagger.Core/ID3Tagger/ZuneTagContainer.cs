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
                   select new MediaIdGuid {MediaId = frame.Owner, Guid = new Guid(frame.Data)};

            //TODO: could factor this class to filter the TagContainer because ReadMediaIds And Add
            //are both working on private frames
        }

        public void Add(MediaIdGuid guid)
        {
            PrivateFrame newFrame = new PrivateFrame(guid.MediaId, guid.Guid.ToByteArray());

            PrivateFrame existingFrame = (from frame in _container.OfType<PrivateFrame>()
                                          where frame.Owner == newFrame.Owner
                                          select frame).FirstOrDefault();

            //if the frame already exists and the data inside is different then remove it
            if (existingFrame != null)
                if (existingFrame.Data != newFrame.Data)
                    _container.Remove(existingFrame);

            _container.Add(newFrame);
        }

        public MetaData ReadMetaData()
        {
            IEnumerable<TextFrame> allTextFrames = from frame in _container.OfType<TextFrame>()
                                                   select frame;

            return new MetaData
            {
                AlbumArtist = GetValue(allTextFrames, "TPE1"),
                AlbumTitle = GetValue(allTextFrames, "TALB"),
                SongTitle = GetValue(allTextFrames, "TIT2"),
                Year = GetValue(allTextFrames, "TYER")
            };
        }

        private static string GetValue(IEnumerable<TextFrame> textFrames, string key)
        {
            return textFrames.Where(x => x.Descriptor.ID == key).SingleOrDefault().Content;
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