using System;
using System.ComponentModel;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class Album : INotifyPropertyChanged
    {
        private AlbumMetaData _webAlbumMetaData;
        private AlbumMetaData _zuneAlbumMetaData;
        private bool _isLinked;

        public string AlbumMediaId { get; set; }
        public bool Selected { get; set; }

        public bool IsLinked
        {
            //get { return !String.IsNullOrEmpty(this.AlbumMediaId); }
            get { return _isLinked; }
            set
            {
                _isLinked = value;
            }
        }

        public AlbumMetaData ZuneAlbumMetaData
        {
            get { return _zuneAlbumMetaData; }
            set
            {
                _zuneAlbumMetaData = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("ZuneAlbumMetaData"));
            }
        }
        public AlbumMetaData WebAlbumMetaData
        {
            get { return _webAlbumMetaData; }
            set
            {
                _webAlbumMetaData = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("WebAlbumMetaData"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }
    }
}
