using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : ZuneWizardPageViewModelBase
    {
        private RelayCommand<string> _searchCommand;
        private WebsiteAlbumMetaDataViewModel _websiteAlbumMetaDataViewModel;
        private bool _canMoveNext;

        public SearchViewModel()
        {
            WebsiteAlbumMetaDataViewModel = new WebsiteAlbumMetaDataViewModel();
            base.IsMovingNext += SearchViewModel_IsMovingNext;
        }

        private void SearchViewModel_IsMovingNext(object sender, EventArgs e)
        {
            SearchFor(SearchText);
        }

        public WebsiteAlbumMetaDataViewModel WebsiteAlbumMetaDataViewModel
        {
            get { return _websiteAlbumMetaDataViewModel; }
            set
            {
                _websiteAlbumMetaDataViewModel = value;
                OnPropertyChanged("WebsiteAlbumMetaDataViewModel");
            }
        }

        private bool _isSearching;

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                OnPropertyChanged("IsSearching");
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand<string>(SearchFor);
                }

                return _searchCommand;
            }
        }

        private void SearchFor(string artist)
        {
            this.IsSearching = true;

            AlbumSearch.SearchForAsync(artist, results =>
                                                   {
                                                       var tempList = new List<AlbumArtistAndTitleWithUrl>();

                                                       foreach (var result in results)
                                                           tempList.Add(new AlbumArtistAndTitleWithUrl()
                                                                            {
                                                                                Title = result.Title,
                                                                                Artist = result.Artist,
                                                                                Url = result.Url
                                                                            });

                                                       ZuneWizardModel.GetInstance().InvokeNewAlbumsAvailable(tempList);

                                                       this.IsSearching = false;
                                                       this._canMoveNext = true;

                                                       MoveNext();
                                                   });
        }

        public override string NextButtonText
        {
            get { return "Search"; }
        }

        public string SearchText { get; set; }

        internal override bool IsValid()
        {
            return this.FlagCanMoveNext;
        }

        internal override bool CanMoveNext()
        {
            return this._canMoveNext;
        }

        private int _moveNextAttempts;

        private void MoveNext()
        {
            _moveNextAttempts++;

            if (_moveNextAttempts <= 1)
                base.OnMoveNextOverride();
        }

        //this is invoked when the view is loaded, different to when the view is constructed
        public void ViewShown()
        {
        }
    }
}