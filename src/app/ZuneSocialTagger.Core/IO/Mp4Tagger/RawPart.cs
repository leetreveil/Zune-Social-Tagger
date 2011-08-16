namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    public class RawPart : IBasePart
    {
        public byte[] Content { get; set; }
        public short Type { get; set; }
        public string Name { get; set; }
    }
}