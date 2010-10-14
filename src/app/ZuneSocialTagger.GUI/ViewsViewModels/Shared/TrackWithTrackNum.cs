using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
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