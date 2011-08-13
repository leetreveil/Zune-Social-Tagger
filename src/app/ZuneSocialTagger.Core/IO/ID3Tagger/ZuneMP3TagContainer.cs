using System;
using System.Collections.Generic;
using System.Linq;
using TagLib;
using PrivateFrame = TagLib.Id3v2.PrivateFrame;
using Tag = TagLib.Id3v2.Tag;

namespace ZuneSocialTagger.Core.IO.ID3Tagger
{
    public class ZuneMP3TagContainer : BaseZuneTagContainer
    {
        private readonly File _file;
        private readonly Tag _tag;

        public ZuneMP3TagContainer(File file) : base(file)
        {
            _file = file;

            //if we can't find the id3v2 tag we will create it, this will handle cases
            //where we load a tag with id3v1 only
            _tag = (Tag) file.GetTag(TagTypes.Id3v2, true);
        }

        public override void AddZuneAttribute(ZuneAttribute zuneAttribute)
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

        public override void RemoveZuneAttribute(string name)
        {
            _tag.OfType<PrivateFrame>()
                .Where(frame => frame.Owner == name)
                .ToList()
                .ForEach(privFrame => _tag.RemoveFrame(privFrame));
        }

        public override IEnumerable<ZuneAttribute> ZuneAttributes
        {
            get 
            {
                return from frame in _tag.OfType<PrivateFrame>()
                       where ZuneIds.GetAll.Contains(frame.Owner)
                       select new ZuneAttribute(frame.Owner, new Guid(frame.PrivateData.ToArray()));
            }
        }
    }
}
