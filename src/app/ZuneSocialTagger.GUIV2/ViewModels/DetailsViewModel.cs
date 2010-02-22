using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.Models;
using Caliburn.PresentationFramework.Screens;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    class DetailsViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;

        public DetailsViewModel(IUnityContainer container, IZuneWizardModel model)
        {
            _container = container;
            _model = model;
        }

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

                        //TODO: convert TrackNumbers that are imported as 1/1 to just 1 or 1/12 to just 1
                        //container.WriteToFile(row.FilePath);
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
                ErrorMessageBox.Show("One or more files could not be written to. Have you checked the files are not marked read-only?");
            else
                new SuccessView(new SuccessViewModel(_model)).Show();

            Mouse.OverrideCursor = null;

            _model.CurrentPage = _container.Resolve<SelectAudioFilesViewModel>();
        }

        public void MoveBack()
        {
            _model.CurrentPage = _container.Resolve<SearchResultsViewModel>();
        }

        public ObservableCollection<DetailRow> Rows 
        {
            get { return _model.Rows; }
        } 

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromWebsite
        {
            get { return _model.AlbumDetailsFromWebsite; }
        }

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile
        {
            get { return _model.AlbumDetailsFromFile; }
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
