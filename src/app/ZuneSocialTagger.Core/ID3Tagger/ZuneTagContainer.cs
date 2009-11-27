using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using System.Linq;
using System.IO;
using Image=System.Drawing.Image;

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
                   select new MediaIdGuid(frame.Owner, new Guid(frame.Data));


            //TODO: could refactor this class to filter the TagContainer because ReadMediaIds And Add are both working on private frames
        }

        public void Add(MediaIdGuid mediaIDGuid)
        {
            PrivateFrame newFrame = new PrivateFrame(mediaIDGuid.MediaId, mediaIDGuid.Guid.ToByteArray());


            //frame owner is a unique id identifying a private field so we can
            //be sure that there's only one
            PrivateFrame existingFrame = (from frame in _container.OfType<PrivateFrame>()
                                          where frame.Owner == newFrame.Owner
                                          where frame.Data != newFrame.Data
                                          select frame).FirstOrDefault();

            //if the frame already exists and the data inside is different then remove it
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
                           Year = GetValue(allTextFrames, "TYER"),
                           Index = GetValue(allTextFrames,"TRCK"),
                           Picture = ReadImage()
                       };
        }

        public TagContainer GetContainer()
        {
            return this._container;
        }

        private Image ReadImage()
        {
            var pictureFrame = _container.OfType<PictureFrame>().Select(frame => frame).FirstOrDefault();

            if (pictureFrame != null)
                return pictureFrame.Type == FrameType.Picture
                           ? Image.FromStream(new MemoryStream(pictureFrame.PictureData))
                           : null;

            return null;
        }

        private static string GetValue(IEnumerable<TextFrame> textFrames, string key)
        {
            TextFrame result = textFrames.Where(x => x.Descriptor.ID == key).SingleOrDefault();

            return result != null ? result.Content : string.Empty;
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