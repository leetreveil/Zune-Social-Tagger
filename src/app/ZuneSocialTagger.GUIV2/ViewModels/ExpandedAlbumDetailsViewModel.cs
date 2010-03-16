using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Core;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class ExpandedAlbumDetailsViewModel
    {
        private string _songCount;
        private string _artworkUrl;
        private string _artist;
        private string _title;
        private string _year;

        public string Year
        {
            get { return string.IsNullOrEmpty(_year) ? "Unknown Year" : _year; }
            set
            {
                _year = value;
            }
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
            get { return _artworkUrl ?? @"../Assets/blankartwork.png"; }
            set { _artworkUrl = value; }
        }

        public void CopyArtworkToClipboard()
        {
            //TODO: find a better way to copy the artwork, this is very very smelly

            //image.UriSource = new Uri("pack://application:,,,/Assets/blankartwork.png");
            var image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(this.ArtworkUrl);
            image.EndInit();

            if (ArtworkUrl != @"../Assets/blankartwork.png")
                Clipboard.SetImage(image);
        }
    }
}