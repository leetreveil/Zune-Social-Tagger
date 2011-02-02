using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using System.Linq;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;

namespace ZuneSocialTagger.GUI.ViewsViewModels.MoreInfo
{
    public class MoreInfoViewModel : ViewModelBase
    {
        private readonly IViewLocator _locator;

        public MoreInfoViewModel(IViewLocator locator)
        {
            _locator = locator;
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }

        private RelayCommand _moveBackCommand;
        public RelayCommand MoveBackCommand 
        { 
            get 
            {
                if (_moveBackCommand == null)
                    _moveBackCommand = new RelayCommand(MoveBack);

                return _moveBackCommand;
            } 
        }

        private List<MoreInfoRow> _tracks;
        public List<MoreInfoRow> Tracks 
        {
            get
            {
                if (_tracks == null)
                    _tracks = new List<MoreInfoRow>();

                return _tracks;
            }
        }

        public void SetAlbumDetails(AlbumDetailsViewModel albumDetails)
        {
            this.AlbumDetailsFromFile = albumDetails.ZuneAlbumMetaData.GetAlbumDetailsFrom();
            this.AlbumDetailsFromWebsite = albumDetails.WebAlbumMetaData.GetAlbumDetailsFrom();

            foreach (DbTrack dbTrack in albumDetails.ZuneAlbumMetaData.Tracks)
            {
                var albumMoreInfoRow = new MoreInfoRow();

                albumMoreInfoRow.TrackFromFile = new TrackWithTrackNum
                {
                    TrackTitle = dbTrack.Title,
                    TrackNumber = dbTrack.TrackNumber
                };

                WebTrack webTrack =
                    albumDetails.WebAlbumMetaData.Tracks.Where(x => x.MediaId == dbTrack.MediaId).FirstOrDefault();

                if (webTrack != null)
                {
                    albumMoreInfoRow.TrackFromWeb = new TrackWithTrackNum
                    {
                        TrackNumber = webTrack.TrackNumber,
                        TrackTitle = webTrack.Title
                    };

                    albumMoreInfoRow.LinkStatusImage = new Uri("pack://application:,,,/Resources/Assets/yes.png");
                }
                else
                {
                    albumMoreInfoRow.LinkStatusImage = new Uri("pack://application:,,,/Resources/Assets/no.png");
                    albumMoreInfoRow.LinkStatusText = "UNLINKED";
                }

                this.Tracks.Add(albumMoreInfoRow);
            }
        }

        private void MoveBack()
        {
            _locator.SwitchToFirstView();
        }
    }
}