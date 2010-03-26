using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class DetailsViewModel : ViewModelBase
    {
        private readonly IZuneWizardModel _model;

        public DetailsViewModel(IZuneWizardModel model)
        {
            _model = model;

            this.AlbumDetailsFromWebsite = model.WebAlbumDetails;
            this.AlbumDetailsFromFile = model.FileAlbumDetails;

            this.MoveToStartCommand = new RelayCommand(MoveToStart);
            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.SaveCommand = new RelayCommand(Save);
        }

        public ObservableCollection<DetailRow> Rows
        {
            get { return _model.Rows; }
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }

        public RelayCommand MoveToStartCommand { get; private set; }
        public RelayCommand MoveBackCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public void Save()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var uaeExceptions = new List<UnauthorizedAccessException>();

            foreach (var row in _model.Rows)
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

                        if (Properties.Settings.Default.UpdateAlbumInfo)
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
                throw new NotImplementedException("show success view model");

            Mouse.OverrideCursor = null;

            Messenger.Default.Send(typeof(IFirstPage));
        }

        public void MoveBack()
        {
            Messenger.Default.Send(typeof(SearchResultsViewModel));
        }

        public void MoveToStart()
        {
            Messenger.Default.Send(typeof(IFirstPage));
        }

        public bool UpdateAlbumInfo
        {
            get { return Properties.Settings.Default.UpdateAlbumInfo; }
            set
            {
                if (value != UpdateAlbumInfo)
                {
                    Properties.Settings.Default.UpdateAlbumInfo = value;
                    Properties.Settings.Default.Save();
                }

            }
        }
    }
}
