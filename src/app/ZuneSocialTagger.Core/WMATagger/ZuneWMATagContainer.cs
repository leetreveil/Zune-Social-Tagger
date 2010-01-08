using System;
using System.Collections.Generic;
using System.Linq;
using ASFTag.Net;
using Attribute=ASFTag.Net.Attribute;

namespace ZuneSocialTagger.Core.WMATagger
{
    public class ZuneWMATagContainer : TagContainer, IZuneTagContainer
    {
        public IEnumerable<MediaIdGuid> ReadMediaIds()
        {
            return from tag in this
                   where MediaIds.Ids.Contains(tag.Name)
                   select new MediaIdGuid(tag.Name, new Guid(tag.Value));
        }

        public void AddZuneMediaId(MediaIdGuid mediaIDGuid)
        {
            this.Add(new Attribute(mediaIDGuid.Name,mediaIDGuid.Guid.ToString(),WMT_ATTR_DATATYPE.WMT_TYPE_GUID));
        }

        public MetaData ReadMetaData()
        {
            return new MetaData
                   {
                       AlbumArtist = GetValue("WM/AlbumArtist"),
                       AlbumName = GetValue("WM/AlbumTitle"),
                       ContributingArtists = GetValues("Author"),
                       DiscNumber = GetValue("WM/PartOfSet"),
                       Genre = GetValue("WM/Genre"),
                       Title = GetValue("Title"),
                       TrackNumber = GetValue("WM/TrackNumber"),
                       Year = GetValue("WM/Year")
                   };
        }

        public void AddMetaData(MetaData metaData)
        {
            IEnumerable<Attribute> attributes = CreateTextFramesFromMetaData(metaData);
            foreach (var attribute in attributes)
                this.Add(attribute);
        }

        public void WriteToFile(string filePath)
        {
            ASFTagManager.WriteTag(filePath,this);
        }

        public void RemoveMediaId(string name)
        {
            Attribute toBeRemoved = this.Where(x => x.Name == name).FirstOrDefault();

            if (toBeRemoved != null)
                this.Remove(toBeRemoved);
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

            foreach (var contributingArtist in metaData.ContributingArtists)
                yield return new Attribute("Author", contributingArtist, WMT_ATTR_DATATYPE.WMT_TYPE_STRING);
        }

        private IEnumerable<string> GetValues(string key)
        {
            return this.Where(x => x.Name == key).Select(x=> x.Value);
        }

        private string GetValue(string key)
        {
            Attribute result = this.Where(x => x.Name == key).SingleOrDefault();

            return result != null ? result.Value : string.Empty;
        }
    }
}