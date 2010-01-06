using System;
using System.Collections.Generic;
using System.Linq;
using ASFTag.Net;
using ZuneSocialTagger.Core.ID3Tagger;
using Attribute=ASFTag.Net.Attribute;

namespace ZuneSocialTagger.Core.WMATagger
{
    public class ZuneWMATagContainer : IZuneTagContainer
    {
        private readonly TagContainer _container;

        public ZuneWMATagContainer(TagContainer container)
        {
            _container = container;
        }

        public IEnumerable<MediaIdGuid> ReadMediaIds()
        {
            return from tag in _container
                   where MediaIds.Ids.Contains(tag.Name)
                   select new MediaIdGuid(tag.Name, new Guid(tag.Value));
        }

        public void AddZuneMediaId(MediaIdGuid mediaIDGuid)
        {
            _container.Add(new Attribute(mediaIDGuid.MediaId,mediaIDGuid.Guid.ToString(),WMT_ATTR_DATATYPE.WMT_TYPE_GUID));
        }

        public MetaData ReadMetaData()
        {
            return new MetaData
                   {
                       AlbumArtist = GetValue(_container, "WM/AlbumArtist"),
                       AlbumName = GetValue(_container, "WM/AlbumTitle"),
                       ContributingArtists = GetValues(_container, "Author"),
                       DiscNumber = GetValue(_container, "WM/PartOfSet"),
                       Genre = GetValue(_container, "WM/Genre"),
                       Title = GetValue(_container, "Title"),
                       TrackNumber = GetValue(_container, "WM/TrackNumber"),
                       Year = GetValue(_container, "WM/Year")
                   };
        }

        public void AddMetaData(MetaData metaData)
        {
            IEnumerable<Attribute> attributes = CreateTextFramesFromMetaData(metaData);
            foreach (var attribute in attributes)
                _container.Add(attribute);
        }

        public void WriteToFile(string filePath)
        {
            ASFTagManager.WriteTag(filePath,_container);
        }

        private static IEnumerable<Attribute> CreateTextFramesFromMetaData(MetaData metaData)
        {
            yield return new Attribute("WM/AlbumArtist", metaData.AlbumArtist,WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            yield return new Attribute("WM/AlbumTitle", metaData.AlbumName,WMT_ATTR_DATATYPE.WMT_TYPE_STRING );
            yield return new Attribute("WM/PartOfSet", metaData.DiscNumber, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            yield return new Attribute("WM/Genre", metaData.Genre, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            yield return new Attribute("Title", metaData.Title, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            yield return new Attribute("WM/TrackNumber", metaData.TrackNumber, WMT_ATTR_DATATYPE.WMT_TYPE_DWORD);
            yield return new Attribute("WM/Year", metaData.Year, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);

            var contribArtists = string.Join("/", metaData.ContributingArtists.ToArray());

            yield return new Attribute("Author", contribArtists, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);

        }

        public TagContainer GetContainer()
        {
            return _container;
        }

        private static IEnumerable<string> GetValues(IEnumerable<Attribute> attributes,string key)
        {
            return attributes.Where(x => x.Name == key).Select(x=> x.Value);
        }

        private static string GetValue(IEnumerable<Attribute> attributes, string key)
        {
            Attribute result = attributes.Where(x => x.Name == key).SingleOrDefault();

            return result != null ? result.Value : string.Empty;
        }
    }
}