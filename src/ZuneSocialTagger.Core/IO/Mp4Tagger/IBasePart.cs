namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    public interface IBasePart
    {
        string Name { get; set; }
        byte[] Render();
    }
}