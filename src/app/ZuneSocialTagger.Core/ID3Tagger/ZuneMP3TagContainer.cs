using System;
using System.Collections.Generic;
using ID3Tag;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using System.Linq;
using System.Text;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    /// <summary>
    /// Updates a pre-existing TagContainer with new zune PRIV tags
    /// </summary>
    public class ZuneMP3TagContainer : IZuneTagContainer
    {
        private readonly TagContainer _container;

        public ZuneMP3TagContainer(TagContainer container)
        {
            _container = container;
        }

        public IEnumerable<ZuneAttribute> ReadZuneAttributes()
        {
            //OfType instead of cast because the container could contain other types other than private frames 
            //and we only want private frames

            //select all available zune attributes from the mp3 file
            return from frame in _container.OfType<PrivateFrame>()
                   where ZuneAttributes.Ids.Contains(frame.Owner)
                   select new ZuneAttribute(frame.Owner, new Guid(frame.Data));

        }

        public void AddZuneAttribute(ZuneAttribute zuneAttribute)
        {
            var newFrame = new PrivateFrame(zuneAttribute.Name, zuneAttribute.Guid.ToByteArray());

            //frame owner is a unique id identifying a private field so we can
            //be sure that there's only one
            PrivateFrame existingFrame = (from frame in _container.OfType<PrivateFrame>()
                                          where frame.Owner == newFrame.Owner
                                          where frame.Data != newFrame.Data
                                          select frame).FirstOrDefault();

            //if the frame already exists and the data inside is different then remove it
            if (existingFrame != null)
                _container.Remove(existingFrame);

            _container.Add(newFrame);
        }

        public void RemoveZuneAttribute(string name)
        {
            PrivateFrame existingFrame = (from frame in _container.OfType<PrivateFrame>()
                                          where frame.Owner == name
                                          select frame).FirstOrDefault();


            if (existingFrame != null)
                _container.Remove(existingFrame);
        }

        public MetaData ReadMetaData()
        {
            return new MetaData
               {
                   AlbumArtist = GetValue(ID3Frames.AlbumArtist),
                   ContributingArtists = GetValue(ID3Frames.ContributingArtists).Split('/'),
                   AlbumName = GetValue(ID3Frames.AlbumName),
                   Title = GetValue(ID3Frames.Title),
                   Year = GetValue(ID3Frames.Year),
                   DiscNumber = GetValue(ID3Frames.DiscNumber),
                   Genre = GetValue(ID3Frames.Genre),
                   TrackNumber = GetValue(ID3Frames.TrackNumber)
               };
        }


        public void AddMetaData(MetaData metaData)
        {
            foreach (var textFrame in CreateTextFramesFromMetaData(metaData))
            {
                TextFrame tempTextFrame = textFrame;

                TextFrame existingFrame = (from frame in _container.OfType<TextFrame>()
                                           where frame.Descriptor.ID == tempTextFrame.Descriptor.ID
                                           select frame).FirstOrDefault();


                if (existingFrame != null)
                    _container.Remove(existingFrame);

                _container.Add(textFrame);
            }
        }

        public void WriteToFile(string filePath)
        {
            Id3TagManager.WriteV2Tag(filePath, _container);
        }

        private static IEnumerable<TextFrame> CreateTextFramesFromMetaData(MetaData metaData)
        {
            var contribArtists = string.Join("/", metaData.ContributingArtists.ToArray());

            yield return new TextFrame(ID3Frames.AlbumArtist, metaData.AlbumArtist, Encoding.Default);
            yield return new TextFrame(ID3Frames.ContributingArtists, contribArtists, Encoding.Default);
            yield return new TextFrame(ID3Frames.AlbumName, metaData.AlbumName, Encoding.Default);
            yield return new TextFrame(ID3Frames.DiscNumber, metaData.DiscNumber, Encoding.Default);
            yield return new TextFrame(ID3Frames.Genre, metaData.Genre, Encoding.Default);
            yield return new TextFrame(ID3Frames.Title, metaData.Title, Encoding.Default);
            yield return new TextFrame(ID3Frames.TrackNumber, metaData.TrackNumber, Encoding.Default);
            yield return new TextFrame(ID3Frames.Year, metaData.Year, Encoding.Default);
        }

 
        private string GetValue(string key)
        {
            IEnumerable<TextFrame> allTextFrames = from frame in _container.OfType<TextFrame>()
                                                   select frame;

            TextFrame result = allTextFrames.Where(x => x.Descriptor.ID == key).SingleOrDefault();

            return result != null ? result.Content : string.Empty;
        }
    }
}