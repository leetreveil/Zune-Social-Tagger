using System;
using System.Windows;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    public class ExpandedAlbumDetailsViewModel
    {
        private string _songCount;
        private string _artworkUrl;
        private string _artist;
        private string _title;
        private string _year;

        public ExpandedAlbumDetailsViewModel()
        {
            this.CopyArtworkToClipboardCommand = new RelayCommand(CopyArtworkToClipboard);
        }

        public RelayCommand CopyArtworkToClipboardCommand { get; private set; }

        public string Year
        {
            get { return string.IsNullOrEmpty(_year) || _year == "-1" ? "Unknown Year" : _year; }
            set { _year = value; }
        }

        public string Title
        {
            get { return string.IsNullOrEmpty(_title) ? "Unknown Title" : _title; }
            set { _title = value; }
        }

        public string Artist
        {
            get { return string.IsNullOrEmpty(_artist) ? "Unknown Artist" : _artist; }
            set { _artist = value; }
        }

        public string SongCount
        {
            get { return _songCount + " songs"; }
            set { _songCount = value; }
        }

        public string ArtworkUrl
        {
            get
            {
                return _artworkUrl ?? @"../../Resources/Assets/blankartwork.png";
            }
            set { _artworkUrl = value; }
        }

        public void CopyArtworkToClipboard()
        {
            if (ArtworkUrl != @"../../Resources/Assets/blankartwork.png")
            {
                try
                {
                    var image = new BitmapImage();

                    image.BeginInit();
                    image.UriSource = new Uri(this.ArtworkUrl, UriKind.RelativeOrAbsolute);
                    image.EndInit();

                    Clipboard.SetImage(image);
                }
                catch{ }
            }
        }
    }
}