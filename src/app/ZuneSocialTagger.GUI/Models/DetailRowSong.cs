namespace ZuneSocialTagger.GUI.Models
{
    public class DetailRowSong
    {
        private string _trackNumber;
        public string TrackNumber
        {
            get { return _trackNumber; }
            set
            {
                _trackNumber = SharedMethods.ConvertTrackNumberToDoubleDigits(value);
            }
        }

        public string TrackTitle { get; set; }
    }
}