using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private string _searchText;
        private bool _isSearching;
        private bool _canMoveNext;

        public SearchViewModel(IZuneWizardModel model,SearchResultsViewModel searchResultsViewModel)
        {
            this.SearchResultsViewModel = searchResultsViewModel;
            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.MoveNextCommand = new RelayCommand(MoveNext);

            this.AlbumDetails = model.SelectedAlbum.ZuneAlbumMetaData;
            this.SearchText = model.SearchText;

            Search();
        }

        public RelayCommand MoveBackCommand { get; private set; }
        public RelayCommand MoveNextCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetails { get; set; }
        public SearchResultsViewModel SearchResultsViewModel { get; set; }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");
            }
        }

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                RaisePropertyChanged("IsSearching");
            }
        }

        public bool CanMoveNext
        {
            get { return _canMoveNext; }
            set
            {
                _canMoveNext = value;
                RaisePropertyChanged("CanMoveNext");
            }
        }

        public void SearchClicked()
        {
            this.IsSearching = true;
        }

        public void Search()
        {
            this.CanMoveNext = false;
            this.IsSearching = true;
            this.SearchResultsViewModel.Albums.Clear();
            this.SearchResultsViewModel.SearchResultsDetailViewModel = null;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                IEnumerable<Album> results = AlbumSearch.SearchFor(this.SearchText);

                this.SearchResultsViewModel.ShowNoResultsMessage = results.Count() == 0;
                this.CanMoveNext = results.Count() > 0;

                UIDispatcher.GetDispatcher().Invoke(new Action(() =>
                   {
                       foreach (Album result in results)
                            this.SearchResultsViewModel.Albums.Add(result);
                   }));

                this.IsSearching = false;
            });
        }

        public void MoveBack()
        {
            Messenger.Default.Send(typeof(IFirstPage));
        }

        public void MoveNext()
        {
            Messenger.Default.Send(typeof(DetailsViewModel));
        }
    }
}