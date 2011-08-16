using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TagLib;
using TagLib.Mpeg4;
using File = TagLib.File;

namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    public class ZuneMp4TagContainer : BaseZuneTagContainer
    {
        private readonly File _mp4File;

        public ZuneMp4TagContainer(File file) : base(file)
        {
            _mp4File = file;
        }

        public override void AddZuneAttribute(ZuneAttribute zuneAttribute)
        {
            var parts = GetParts().ToList();

            var existingPart = parts.OfType<GuidPart>()
                .Where(x => x.Name == zuneAttribute.Name)
                .FirstOrDefault();

            if (existingPart != null)
                parts.Remove(existingPart);

            parts.Add(new GuidPart(zuneAttribute.Name, zuneAttribute.Guid));

            var udataBox = GetUdataBox();

            if (udataBox == null)
                return;

            udataBox.RemoveChild(new ByteVector("Xtra"));

            var newXtraBox = new XtraBox(new ByteVector("Xtra"));
            newXtraBox.Data = ZuneXtraParser.ConstructRawData(parts);

            udataBox.AddChild(newXtraBox);
        }

        public override void RemoveZuneAttribute(string name)
        {
            var parts = GetParts().ToList();

            var toRemove = parts.Where(x => x.Name == name);

            if (toRemove.Count() == 0)
                return;

            parts.Remove(toRemove.First());

            var udataBox = GetUdataBox();

            if (udataBox == null)
                return;

            udataBox.RemoveChild(new ByteVector("Xtra"));

            var newXtraBox = new XtraBox(new ByteVector("Xtra"));
            newXtraBox.Data = ZuneXtraParser.ConstructRawData(parts);

            udataBox.AddChild(newXtraBox);
        }

        private IEnumerable<IBasePart> GetParts()
        {
            var attribs = new List<IBasePart>();

            var udataBox = GetUdataBox();

            if (udataBox == null)
                return attribs;

            var xtraBox = udataBox.GetChild(new ByteVector("Xtra"));

            if (xtraBox == null)
                return attribs;

            return ZuneXtraParser.ParseRawData(xtraBox.Data.ToArray());
        }

        private IsoUserDataBox GetUdataBox()
        {
            FieldInfo fi = _mp4File.GetType().GetField("udta_boxes", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var boxes = fi.GetValue(_mp4File) as List<IsoUserDataBox>;

            if (boxes != null)
            {
                return boxes[0];
            }

            return null;
        }
    }
}
