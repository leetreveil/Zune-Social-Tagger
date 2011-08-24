using System.Text;
using System.Linq;

namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    public class RawPart : IBasePart
    {
        public byte[] Content { get; set; }
        public string Name { get; set; }

        public virtual byte[] Render()
        {
            var nameLength = ByteHelpers.GetBytesAsLi(Name.Length);
            var name = Encoding.Default.GetBytes(Name);

            var partContentWithoutLength = nameLength.Concat(name)
                                                     .Concat(Content);

            //+4 is for the part length
            byte[] totalPartLength = ByteHelpers.GetBytesAsLi(partContentWithoutLength.Count() + 4);

            return totalPartLength.Concat(partContentWithoutLength).ToArray();
        }
    }
}