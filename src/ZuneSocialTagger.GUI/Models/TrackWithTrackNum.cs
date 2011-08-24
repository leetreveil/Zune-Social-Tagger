using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Shared;

namespace ZuneSocialTagger.GUI.Models
{
    public class TrackWithTrackNum
    {
        private string _trackNumber;
        public string TrackNumber
        {
            get { return _trackNumber; }
            set
            {
                _trackNumber = value.ConvertTrackNumberToDoubleDigits();
            }
        }

        public string TrackTitle { get; set; }

        public object BackingData { get; set; }
    }
}