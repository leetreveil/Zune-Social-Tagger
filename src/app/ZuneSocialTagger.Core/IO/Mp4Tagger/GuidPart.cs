using System;

namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    public class GuidPart : RawPart
    {
        public GuidPart(string name, Guid guid)
        {
            base.Content = guid.ToByteArray();
            base.Name = name;
            base.Type = 72;
        }

        public GuidPart(RawPart rawPart)
        {
            base.Content = rawPart.Content;
            base.Name = rawPart.Name;
            base.Type = rawPart.Type;
        }

        public Guid MediaId 
        {
            get 
            { 
                return new Guid(base.Content);
            } 
        }
    }
}