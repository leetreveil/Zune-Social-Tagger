using System;
using System.Collections.Generic;
using System.Linq;
using ASFTag.Net;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsite;

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

        public void Add(MediaIdGuid mediaIDGuid)
        {
            throw new NotImplementedException();
        }

        public Track ReadMetaData()
        {
            throw new NotImplementedException();
        }

        public void WriteMetaData(Track metaData)
        {
            throw new NotImplementedException();
        }

        public TagContainer GetContainer()
        {
            return _container;
        }
    }
}