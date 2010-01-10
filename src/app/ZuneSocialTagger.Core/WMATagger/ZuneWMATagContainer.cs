using System;
using System.Collections.Generic;
using System.Linq;
using ASFTag;
using Attribute=ASFTag.Attribute;

namespace ZuneSocialTagger.Core.WMATagger
{
    public class ZuneWMATagContainer : IZuneTagContainer
    {
        private readonly TagContainer _container;

        public ZuneWMATagContainer(TagContainer container)
        {
            _container = container;
        }

        public IEnumerable<ZuneAttribute> ReadZuneAttributes()
        {
            return from tag in _container
                   where ZuneAttributes.Ids.Contains(tag.Name)
                   select new ZuneAttribute(tag.Name, new Guid(tag.Value));
        }

        public void AddZuneAttribute(ZuneAttribute zuneAttribute)
        {
            _container.Add(new Attribute(zuneAttribute.Name, zuneAttribute.Guid.ToString(), WMT_ATTR_DATATYPE.WMT_TYPE_GUID));
        }

        public MetaData ReadMetaData()
        {
            return new MetaData
                   {
                       AlbumArtist = GetValue(ASFAttributes.AlbumArtist),
                       AlbumName = GetValue(ASFAttributes.AlbumName),
                       ContributingArtists = GetValues(ASFAttributes.ContributingArtists),
                       DiscNumber = GetValue(ASFAttributes.DiscNumber),
                       Genre = GetValue(ASFAttributes.Genre),
                       Title = GetValue(ASFAttributes.Title),
                       TrackNumber = GetValue(ASFAttributes.TrackNumber),
                       Year = GetValue(ASFAttributes.Year)
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
            ASFTagManager.WriteTag(filePath, _container);
        }

        public void RemoveZuneAttribute(string name)
        {
            Attribute zuneAttrib = _container.Where(x => x.Name == name).FirstOrDefault();

            if (zuneAttrib != null)
                _container.Remove(zuneAttrib);
        }

        private static IEnumerable<Attribute> CreateTextFramesFromMetaData(MetaData metaData)
        {
            yield return new Attribute(ASFAttributes.AlbumArtist, metaData.AlbumArtist,WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            yield return new Attribute(ASFAttributes.AlbumName, metaData.AlbumName,WMT_ATTR_DATATYPE.WMT_TYPE_STRING );
            yield return new Attribute(ASFAttributes.DiscNumber, metaData.DiscNumber, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            yield return new Attribute(ASFAttributes.Genre, metaData.Genre, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            yield return new Attribute(ASFAttributes.Title, metaData.Title, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
            yield return new Attribute(ASFAttributes.TrackNumber, metaData.TrackNumber, WMT_ATTR_DATATYPE.WMT_TYPE_DWORD);
            yield return new Attribute(ASFAttributes.Year, metaData.Year, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);

            foreach (var contributingArtist in metaData.ContributingArtists)
                yield return new Attribute(ASFAttributes.ContributingArtists, contributingArtist, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
        }

        private IEnumerable<string> GetValues(string key)
        {
            return _container.Where(x => x.Name == key).Select(x => x.Value);
        }

        private string GetValue(string key)
        {
            Attribute result = _container.Where(x => x.Name == key).SingleOrDefault();

            return result != null ? result.Value : string.Empty;
        }
    }
}