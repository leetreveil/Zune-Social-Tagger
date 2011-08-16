using ZuneSocialTagger.Core.IO.Mp4Tagger;

namespace ZuneSocialTagger.IntegrationTests
{
    public class Program
    {
        public static void Main()
        {
            var container = new ZuneMp4TagContainer(
                new TagLib.Mpeg4.File(@"E:\Music\Albums\Andy C Nightlife, Vol. 5 (Bonus Edition)\02 Onslaught.m4a"));

            container.RemoveZuneAttribute("Test");
            //container.AddZuneAttribute(new ZuneAttribute("Test", Guid.NewGuid()));

            container.WriteToFile();
        }
    }
}