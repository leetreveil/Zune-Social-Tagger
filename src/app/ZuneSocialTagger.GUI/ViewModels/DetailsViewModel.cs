using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class DetailsViewModel : ViewModelBaseExtended
    {
        private readonly ZuneWizardModel _model;

        public DetailsViewModel(ZuneWizardModel model)
        {
            _model = model;

            this.AlbumDetailsFromWebsite = _model.SelectedAlbum.WebAlbumMetaData;
            this.AlbumDetailsFromFile = _model.SelectedAlbum.ZuneAlbumMetaData;
   
            this.MoveToStartCommand = new RelayCommand(MoveToStart);
            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.SaveCommand = new RelayCommand(Save);
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; private set; }
        public RelayCommand MoveToStartCommand { get; private set; }
        public RelayCommand MoveBackCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public ObservableCollection<Song> Rows
        {
            get { return _model.SelectedAlbum.Tracks; }
        }

        public ObservableCollection<Track> SongsFromWebste
        {
            get
            {
                return _model.SelectedAlbum.SongsFromWebsite.ToObservableCollection();
            }
        }

        public int SongsFromWebsiteCount
        {
            get
            {
                return this.SongsFromWebste.Count;
            }
        }

        public bool UpdateAlbumInfo
        {
            get { return Settings.Default.UpdateAlbumInfo; }
            set
            {
                if (value != UpdateAlbumInfo)
                {
                    Settings.Default.UpdateAlbumInfo = value;
                }
            }
        }

        public void Save()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var uaeExceptions = new List<UnauthorizedAccessException>();

            foreach (var row in _model.SelectedAlbum.Tracks)
            {
                try
                {
                    var container = row.Container;

                    if (row.SelectedSong.HasAllZuneIds)
                    {
                        container.RemoveZuneAttribute("WM/WMContentID");
                        container.RemoveZuneAttribute("WM/WMCollectionID");
                        container.RemoveZuneAttribute("WM/WMCollectionGroupID");
                        container.RemoveZuneAttribute("ZuneCollectionID");
                        container.RemoveZuneAttribute("WM/UniqueFileIdentifier");

                        container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Album, row.SelectedSong.AlbumMediaID));
                        container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Artist, row.SelectedSong.ArtistMediaID));
                        container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Track, row.SelectedSong.MediaID));

                        if (Settings.Default.UpdateAlbumInfo)
                            container.AddMetaData(row.SelectedSong.MetaData);

                        container.WriteToFile(row.FilePath);
                    }

                    //TODO: run a verifier over whats been written to ensure that the tags have actually been written to file
                }
                catch (UnauthorizedAccessException uae)
                {
                    uaeExceptions.Add(uae);
                    //TODO: better error handling
                }
            }

            if (uaeExceptions.Count > 0)
                //usually occurs when a file is readonly
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Error,"One or more files could not be written to. Have you checked the files are not marked read-only?"));
            else
            {
                //must check that this is not null first because if we use the old mode of reading this will never get set
                if (_model.SelectedAlbum.AlbumDetails != null)
                    _model.SelectedAlbum.AlbumDetails.NeedsRefreshing = true;

                Messenger.Default.Send(typeof(SuccessViewModel));
                Messenger.Default.Send("ALBUMLINKED"); 
            }

            Mouse.OverrideCursor = null;
        }

        public void MoveBack()
        {
            Messenger.Default.Send(typeof(SearchViewModel));
        }

        public void MoveToStart()
        {
            Messenger.Default.Send("SWITCHTOFIRSTVIEW");
        }
    }
}
