using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TagLib;
using TagLib.Id3v2;
using PrivateFrame = TagLib.Id3v2.PrivateFrame;
using Tag = TagLib.Id3v2.Tag;

namespace ZuneSocialTagger.Core.IO.ID3Tagger
{
    public class ZuneMP3TagContainer : IZuneTagContainer
    {
        private readonly File _file;
        private readonly Tag _tag;

        public ZuneMP3TagContainer(File file)
        {
            _file = file;

            //if we can't find the id3v2 tag we will create it, this will handle cases
            //where we load a tag with id3v1 only
            _tag = (Tag) file.GetTag(TagTypes.Id3v2, true);
        }

        public void AddZuneAttribute(ZuneAttribute zuneAttribute)
        {
            var existingFrame = _tag.OfType<PrivateFrame>()
                .Where(frame => frame.Owner == zuneAttribute.Name).FirstOrDefault();

            if (existingFrame != null)
            {
                _tag.ReplaceFrame(existingFrame, new PrivateFrame(zuneAttribute.Name, zuneAttribute.Guid.ToByteArray()));
            }
            else
            {
                _tag.AddFrame(new PrivateFrame(zuneAttribute.Name, zuneAttribute.Guid.ToByteArray()));
            }
        }

        public void AddMetaData(MetaData metaData)
        {

            foreach (var textFrame in CreateTextFramesFromMetaData(metaData))
            {
                //TODO: needs testing

                //we are only allowing fields to be updated if what they are being updated with is not empty
                if (!string.IsNullOrEmpty(string.Join("", textFrame.Text)))
                {
                    TextInformationFrame tempTextFrame = textFrame;

                    var existingFrame = _tag
                        .Where(frame => frame.FrameId.ToString() == tempTextFrame.FrameId.ToString())
                        .FirstOrDefault();

                    if (existingFrame != null)
                    {
                        _tag.ReplaceFrame(existingFrame, textFrame);
                    }
                    else
                    {
                        _tag.AddFrame(textFrame);
                    }
                }
            }
        }

        private static IEnumerable<TextInformationFrame> CreateTextFramesFromMetaData(MetaData metaData)
        {
            //TODO: should we set the stringtype in the tinf ctor?

            yield return new TextInformationFrame(new ByteVector(Encoding.ASCII.GetBytes(ID3Frames.AlbumArtist)))
                                  { Text = new[] {metaData.AlbumArtist} };

            yield return new TextInformationFrame(new ByteVector(Encoding.ASCII.GetBytes(ID3Frames.ContributingArtists))) 
                                  { Text = new[] { string.Join("/", metaData.ContributingArtists.ToArray()) } };

            yield return new TextInformationFrame(new ByteVector(Encoding.ASCII.GetBytes(ID3Frames.AlbumName))) 
                                  { Text = new[] { metaData.AlbumName } };

            yield return new TextInformationFrame(new ByteVector(Encoding.ASCII.GetBytes(ID3Frames.DiscNumber))) 
                                  { Text = new[] { metaData.DiscNumber } };

            yield return new TextInformationFrame(new ByteVector(Encoding.ASCII.GetBytes(ID3Frames.Genre))) 
                                  { Text = new[] { metaData.Genre } };

            yield return new TextInformationFrame(new ByteVector(Encoding.ASCII.GetBytes(ID3Frames.Title))) 
                                  { Text = new[] { metaData.Title } };

            yield return new TextInformationFrame(new ByteVector(Encoding.ASCII.GetBytes(ID3Frames.TrackNumber))) 
                                  { Text = new[] { metaData.TrackNumber } };

            yield return new TextInformationFrame(new ByteVector(Encoding.ASCII.GetBytes(ID3Frames.Year))) 
                                  { Text = new[] { metaData.Year } };
        }

        public void WriteToFile()
        {
            _file.Save();
        }

        public void RemoveZuneAttribute(string name)
        {
            _tag.OfType<PrivateFrame>().Where(frame => frame.Owner == name).ToList().ForEach(
                privFrame => _tag.RemoveFrame(privFrame));
        }

        public MetaData MetaData
        {
            get
            {
                return new MetaData
                {
                    AlbumArtist = _tag.AlbumArtists.FirstOrDefault(),
                    ContributingArtists = _tag.Performers.ToList(),
                    AlbumName = _tag.Album,
                    DiscNumber = _tag.Disc.ToString(),
                    Genre = _tag.Genres.First(),
                    Title = _tag.Title,
                    TrackNumber = _tag.Track.ToString(),
                    Year = _tag.Year.ToString()
                };
            }
        }

        public IEnumerable<ZuneAttribute> ZuneAttributes
        {
            get 
            {
                return from frame in _tag.OfType<PrivateFrame>()
                       where ZuneIds.GetAll.Contains(frame.Owner)
                       select new ZuneAttribute(frame.Owner, new Guid(frame.PrivateData.ToArray()));
            }
        }

        public void Dispose()
        {
            _file.Dispose();
        }
    }
}
