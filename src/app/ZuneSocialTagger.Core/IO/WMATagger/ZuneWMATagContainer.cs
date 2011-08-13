using System;
using System.Collections.Generic;
using System.Linq;
using TagLib;
using TagLib.Asf;
using File = TagLib.File;
using Tag = TagLib.Asf.Tag;

namespace ZuneSocialTagger.Core.IO.WMATagger
{
    public class ZuneWMATagContainer : BaseZuneTagContainer
    {
        private readonly File _file;
        private readonly Tag _tag;

        public ZuneWMATagContainer(File file) : base(file)
        {
            _file = file;
            _tag = (Tag)file.GetTag(TagTypes.Asf);
        }

        public override void AddZuneAttribute(ZuneAttribute zuneAttribute)
        {
            var descRecord = new DescriptionRecord(0, 0, zuneAttribute.Name, zuneAttribute.Guid);

            var attrib = _tag.MetadataLibraryObject.Where(x => x.Name == zuneAttribute.Name).FirstOrDefault();

            if (attrib != null)
            {
                _tag.MetadataLibraryObject.SetRecords(0, 0, attrib.Name, descRecord);
            }
            else
            {
                _tag.MetadataLibraryObject.AddRecord(descRecord);
            }
        }

        public override void RemoveZuneAttribute(string name)
        {
            var attribs = _tag.MetadataLibraryObject.Where(x => x.Name == name).ToList();

            foreach (var attrib in attribs)
            {
                _tag.MetadataLibraryObject.RemoveRecords(0, 0, name);
            }
        }


        public override IEnumerable<ZuneAttribute> ZuneAttributes
        {
            get 
            {
                return from atrrib in _tag.MetadataLibraryObject
                       where ZuneIds.GetAll.Contains(atrrib.Name)
                       select new ZuneAttribute(atrrib.Name, atrrib.ToGuid());
            }
        }
    }
}