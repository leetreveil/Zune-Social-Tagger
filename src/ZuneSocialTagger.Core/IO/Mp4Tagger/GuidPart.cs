using System;
using System.Text;
using System.Linq;

namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    public class GuidPart : RawPart
    {
        public GuidPart(string name, byte[] content)
        {
            base.Name = name;
            base.Content = content;
            Guid = new Guid(content.Skip(10).ToArray());
        }
        public GuidPart(string name, Guid guid)
        {
            base.Name = name;
            Guid = guid;
        }

        public Guid Guid { get; set; }

        public override byte[] Render()
        {
            var nameLength = ByteHelpers.GetBytesAsLi(Name.Length);
            var name = Encoding.Default.GetBytes(Name);
            var guidPartFlag = new byte[] { 0x00, 0x00, 0x00, 0x01 }; // 4byte flag
            var guidLength = new byte[] { 0x00, 0x00, 0x00, 0x16 }; //16 bytes guid length
            var partType = new byte[] { 0x00, 0x48 }; //2byte guid type
            var guid = Guid.ToByteArray(); // guid

            var partContentWithoutLength = nameLength.Concat(name)
                                                     .Concat(guidPartFlag)
                                                     .Concat(guidLength)
                                                     .Concat(partType)
                                                     .Concat(guid);

            byte[] totalPartLength = ByteHelpers.GetBytesAsLi(partContentWithoutLength.Count() + 4);

            return totalPartLength.Concat(partContentWithoutLength).ToArray();
        }
    }
}