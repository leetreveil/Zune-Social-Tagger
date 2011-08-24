using System;
using System.Linq;
using TagLib;

namespace ZuneSocialTagger.Core.IO
{
    public abstract class BaseZuneTagContainer : IZuneTagContainer
    {
        private readonly File _file;
        private readonly Tag _tag;

        protected BaseZuneTagContainer(File file)
        {
            _file = file;
            _tag = file.Tag;
        }

        public abstract void AddZuneAttribute(ZuneAttribute zuneAttribute);
        public abstract void RemoveZuneAttribute(string name);

        public void UpdateMetaData(MetaData metaData)
        {
            //TODO: only update bits if the string being passed is not empty
            _tag.AlbumArtists = new[] { metaData.AlbumArtist };
            _tag.Album = metaData.AlbumName;
            _tag.Performers = metaData.ContributingArtists.ToArray();
            _tag.Genres = new[] { metaData.Genre };
            _tag.Title = metaData.Title;
            _tag.Track = (uint)metaData.TrackNumber.ToTrackNum();
            _tag.Disc = (uint)metaData.DiscNumber.ToDiscNum();

            int year = (int)_tag.Year;
            Int32.TryParse(metaData.Year, out year);
            _tag.Year = (uint)year;
        }
        
        public MetaData MetaData
        {
            get
            {
                //not object literals, too hard to debug
                var albumArtist = _tag.AlbumArtists.FirstOrDefault();
                var contributingArtists = _tag.Performers.ToList();
                var albumName = _tag.Album;
                var discNumber = _tag.Disc.ToString();
                var genre = _tag.Genres.FirstOrDefault();
                var title = _tag.Title;
                var trackNumber = _tag.Track.ToString();
                var year = _tag.Year.ToString();

                return new MetaData
                {
                    AlbumArtist = albumArtist,
                    AlbumName = albumName,
                    ContributingArtists = contributingArtists,
                    DiscNumber = discNumber,
                    Genre = genre,
                    Title = title,
                    TrackNumber = trackNumber,
                    Year = year
                };
            }
        }
        
        public void WriteToFile()
        {
            _file.Save();
        }

        public void Dispose()
        {
            _file.Dispose();
        }
    }
}
