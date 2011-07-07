using System;
using System.Windows;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ZuneSocialTagger.GUI.Models
{
    public class ExpandedAlbumDetailsViewModel
    {
        private ICommand _copyArtworkToClipboardCommand;
        public ICommand CopyArtworkToClipboardCommand
        {
            get
            {
                if (_copyArtworkToClipboardCommand == null)
                    _copyArtworkToClipboardCommand = new RelayCommand(CopyArtworkToClipboard);

                return _copyArtworkToClipboardCommand;
            }
        }

        private string _year;
        public string Year
        {
            get { return string.IsNullOrEmpty(_year) || _year == "-1" ? "Unknown Year" : _year; }
            set { _year = value; }
        }

        private string _title;
        public string Title
        {
            get { return string.IsNullOrEmpty(_title) ? "Unknown Title" : _title; }
            set { _title = value; }
        }

        private string _artist;
        public string Artist
        {
            get { return string.IsNullOrEmpty(_artist) ? "Unknown Artist" : _artist; }
            set { _artist = value; }
        }

        private string _songCount;
        public string SongCount
        {
            get { return _songCount + " songs"; }
            set { _songCount = value; }
        }

        private string _artworkUrl;
        public string ArtworkUrl
        {
            get { return _artworkUrl ?? @"../../Resources/Assets/blankartwork.png"; }
            set { _artworkUrl = value; }
        }

        public void CopyArtworkToClipboard()
        {
            if (ArtworkUrl != @"../../Resources/Assets/blankartwork.png")
            {
                try
                {
                    var imageUri = this.ArtworkUrl;

                    if (this.ArtworkUrl.Contains("?"))
                    {
                        imageUri = this.ArtworkUrl.Remove(this.ArtworkUrl.IndexOf('?'));
                        imageUri += "?width=320&height=320";
                    }

                    var image = new BitmapImage(new Uri(imageUri, UriKind.RelativeOrAbsolute));

                    if (!image.IsDownloading)
                    {
                        Clipboard.SetImage(image);
                        return;
                    }

                    Mouse.OverrideCursor = Cursors.Wait;

                    image.DownloadCompleted += delegate {
                        Clipboard.SetImage(image);
                        Mouse.OverrideCursor = null;
                    };

                    image.DownloadFailed += delegate {
                        Mouse.OverrideCursor = null;
                    };
                    
                }
                catch{ }
            }
        }
    }
}