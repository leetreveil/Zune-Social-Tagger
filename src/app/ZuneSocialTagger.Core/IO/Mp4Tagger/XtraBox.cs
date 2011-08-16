using TagLib;
using TagLib.Mpeg4;

namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    class XtraBox : Box
    {
        private ByteVector _data;

        public XtraBox(ByteVector type) : base(type)
        {
            
        }

        public override ByteVector Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public new ByteVector Render()
        {
            ByteVector output = new ByteVector();
            output =  _data;
            output.Insert(0, Header.Render());
            return output;
        }
    }
}
