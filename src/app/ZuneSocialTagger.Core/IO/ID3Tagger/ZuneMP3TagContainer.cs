using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Id3Tag;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;

namespace ZuneSocialTagger.Core.IO.ID3Tagger
{
    /// <summary>
    /// Updates a pre-existing TagContainer with new zune PRIV tags
    /// </summary>
    public class ZuneMP3TagContainer : IZuneTagContainer
    {
        private readonly TagContainer _container;
        private readonly string _filePath;

        public ZuneMP3TagContainer(TagContainer container)
        {
            _container = container;
        }

        public ZuneMP3TagContainer(TagContainer container,string filePath)
        {
            _container = container;
            _filePath = filePath;
        }

        public MetaData MetaData
        {
            get
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
        }

        public IEnumerable<ZuneAttribute> ZuneAttributes
        {
            get
            {
                return from frame in _container.OfType<PrivateFrame>()
                       where ZuneIds.GetAll.Contains(frame.Owner)
                       select new ZuneAttribute(frame.Owner, new Guid(frame.Data.ToArray()));
            }
        }


        public void AddZuneAttribute(ZuneAttribute zuneAttribute)
        {
            RemoveZuneAttribute(zuneAttribute.Name);

            _container.Add(new PrivateFrame(zuneAttribute.Name,zuneAttribute.Guid.ToByteArray()));
        }

        public void RemoveZuneAttribute(string name)
        {
            _container.OfType<PrivateFrame>().Where(frame => frame.Owner == name).ToList().ForEach(
                privFrame => _container.Remove(privFrame));
        }

        public void AddMetaData(MetaData metaData)
        {
            foreach (var textFrame in CreateTextFramesFromMetaData(metaData))
            {
                //TODO: needs testing

                //we are only allowing fields to be updated if what they are being updated with is not empty
                if (!string.IsNullOrEmpty(textFrame.Content))
                {
                    TextFrame tempTextFrame = textFrame;

                    TextFrame existingFrame = (from frame in _container.OfType<TextFrame>()
                                               where frame.Descriptor.Id == tempTextFrame.Descriptor.Id
                                               select frame).FirstOrDefault();


                    if (existingFrame != null)
                        _container.Remove(existingFrame);

                    _container.Add(textFrame);
                }
            }
        }

        public void WriteToFile()
        {
            if (String.IsNullOrEmpty(_filePath))
                throw new ArgumentException("filePath is not set");

            new Id3TagManager().WriteV2Tag(_filePath, _container);
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

            TextFrame result = allTextFrames.Where(x => x.Descriptor.Id == key).FirstOrDefault();

            return result != null ? result.Content : string.Empty;
        }
    }
}