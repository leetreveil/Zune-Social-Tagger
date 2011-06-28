using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Search;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using GalaSoft.MvvmLight.Threading;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    public class AlbumDetailsViewModel : ViewModelBase
    {
        private readonly IZuneDatabaseReader _dbReader;
        private readonly ViewLocator _locator;
        private readonly SharedModel _sharedModel;
        private int _mediaId;
        private Guid _albumMediaId;

        public AlbumDetailsViewModel(){}//for design time
        public AlbumDetailsViewModel(IZuneDatabaseReader dbReader,
                                ViewLocator locator,
                                SharedModel sharedModel)
        {
            _dbReader = dbReader;
            _locator = locator;
            _sharedModel = sharedModel;
            LinkCommand = new RelayCommand(LinkAlbum);
            RefreshCommand = new RelayCommand(RefreshAlbum);
            DelinkCommand = new RelayCommand(DelinkAlbum);
        }

        public void Init(int mediaId, Guid albumMediaId)
        {
            _mediaId = mediaId;
            _albumMediaId = albumMediaId;
        }

        public override event PropertyChangedEventHandler PropertyChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                _propertyChanged = (PropertyChangedEventHandler)Delegate.Combine(_propertyChanged, value);

                //Check to see if WPF is listening for PropertyChanged events on this object,
                //this only gets called on 'visible' objects 
                //this is a kind of cheap way to do data virtualization
                if (value.Target is PropertyChangedEventManager)
                {
                    if (LinkStatus == LinkStatus.Unknown)
                    {
                        GetAlbumDetailsFromWebsite(AlbumMediaId);
                    }
                }
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                _propertyChanged = (PropertyChangedEventHandler)Delegate.Remove(_propertyChanged, value);
            }
        }

        /// <summary>
        /// We have to store this per 'row' in the view so we can refer back to it later
        /// </summary>
        public int MediaId { get { return _mediaId; } }

        public Guid AlbumMediaId { get { return _albumMediaId; } }

        #region ViewBindings

        public AlbumThumbDetails Left { get; set; }

        public AlbumThumbDetails Right { get; set; }

        public DateTime DateAdded { get; set; }

        private LinkStatus _linkStatus;
        public LinkStatus LinkStatus
        {
            get { return _linkStatus; }
            set
            {
                _linkStatus = value;
                RaisePropertyChanged(() => this.LinkStatus);
            }
        }

        public bool CanDelink
        {
            get { return _linkStatus != LinkStatus.Unlinked && _linkStatus != LinkStatus.Unknown; }
        }

        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand DelinkCommand { get; private set; }

        #endregion

        public void RefreshAlbum()
        {
            LinkStatus = LinkStatus.Unknown;

            try
            {
                DbAlbum albumMetaData = _dbReader.GetAlbum(MediaId);

                if (albumMetaData != null)
                    GetAlbumDetailsFromWebsite(albumMetaData.AlbumMediaId);
                else
                    LinkStatus = LinkStatus.Unlinked;
            }
            catch (COMException)
            {
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Error,
                    "Could not find album, you may need to refresh the database."));
            }
            catch(Exception ex)
            {
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Error, ex.Message));
            }
        }

        public void DelinkAlbum()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            //TODO: fix bug where application crashes when removing an album that is currently playing

            var filePaths = _dbReader.GetTracksForAlbum(MediaId).Select(x => x.FilePath);

            var containers = SharedMethods.GetContainers(filePaths);

            foreach (var container in containers)
            {
                container.RemoveZuneAttribute("WM/WMContentID");
                container.RemoveZuneAttribute("WM/WMCollectionID");
                container.RemoveZuneAttribute("WM/WMCollectionGroupID");
                container.RemoveZuneAttribute("ZuneCollectionID");
                container.RemoveZuneAttribute("WM/UniqueFileIdentifier");
                container.RemoveZuneAttribute("ZuneCollectionID");
                container.RemoveZuneAttribute("ZuneUserEditedFields");
                container.RemoveZuneAttribute(ZuneIds.Album);
                container.RemoveZuneAttribute(ZuneIds.Artist);
                container.RemoveZuneAttribute(ZuneIds.Track);
                container.WriteToFile();
            }

            Mouse.OverrideCursor = null;

            Messenger.Default.Send(new ErrorMessage(ErrorMode.Warning,
                                                    "Album should now be de-linked. You may need to " +
                                                    "remove then re-add the album for the changes to take effect."));

            //force a refresh on the album to see if the de-link worked
            //this probably wont work because the zunedatabase does not correctly change the albums
            //details when delinking, but does when linking
            RefreshAlbum();
        }

        private void LinkAlbum()
        {
            try
            {
                DbAlbum dbAlbum = _dbReader.GetAlbum(MediaId);
                _sharedModel.DbAlbum = dbAlbum;

                IEnumerable<string> filePaths = dbAlbum.Tracks.Select(x => x.FilePath);

                var containers = SharedMethods.GetContainers(filePaths);
                containers = SharedMethods.SortByTrackNumber(containers);

                _sharedModel.SongsFromFile = containers;

                var searchVm = _locator.SwitchToView<SearchView, SearchViewModel>();
                searchVm.Search(dbAlbum.Artist, dbAlbum.Title);
            }
            catch (COMException)
            {
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Error,
                    "Could not find album, you may need to refresh the database."));
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Error, ex.Message));
            }
        }

        private void GetAlbumDetailsFromWebsite(Guid albumMediaId)
        {
            if (albumMediaId != Guid.Empty)
            {
                var url = String.Concat(Urls.Album, albumMediaId);
                AlbumDetailsDownloader.DownloadAsync(url, (exception, album) =>
                {
                    if (album != null)
                    {
                        LinkStatus = LinkStatus.Linked;

                        Right = new AlbumThumbDetails
                        {
                            Artist = album.Artist,
                            ArtworkUrl = album.ArtworkUrl,
                            Title = album.Title,
                        };
                    }
                    else
                    {
                        LinkStatus = LinkStatus.Unlinked;
                    }
                });
            }
            else
            {
                LinkStatus = LinkStatus.Unlinked;
            }
        }
    }
}
