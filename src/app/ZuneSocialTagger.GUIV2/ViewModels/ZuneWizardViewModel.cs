using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class ZuneWizardViewModel : NotifyPropertyChangedImpl
    {
        private ZuneWizardPageViewModelBase _currentPage;
        private RelayCommand _moveNextCommand;
        private RelayCommand _movePreviousCommand;
        private RelayCommand _aboutCommand;
        private ReadOnlyCollection<ZuneWizardPageViewModelBase> _pages;
        private ZuneWizardModel _sharedModel;

        public string NextButtonText
        {
            get { return this.CurrentPage.NextButtonText; }
        }

        public string BackButtonText
        {
            get { return this.CurrentPage.BackButtonText; }
        }

        public ZuneWizardViewModel()
        {
            this.CurrentPage = this.Pages[0];

            //set up the handler for overrides (this allows a page to move to the next page on demand without having to click next)
            foreach (var page in Pages)
                page.MoveNextOverride += PageMoveNextOverride;
        }

        private void PageMoveNextOverride(object sender, EventArgs e)
        {
            TryToMoveToNextPage();
        }

        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                    _aboutCommand = new RelayCommand(ShowAbout);

                return _aboutCommand;
            }
        }

        public void ShowAbout()
        {
            AboutView view = new AboutView();

            view.ShowInTaskbar = false;
            view.Show();
        }

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the previous page in the workflow.
        /// </summary>
        public ICommand MovePreviousCommand
        {
            get
            {
                if (_movePreviousCommand == null)
                    _movePreviousCommand = new RelayCommand(
                        () => this.MoveToPreviousPage(),
                        () => this.CanMoveToPreviousPage);

                return _movePreviousCommand;
            }
        }

        private bool CanMoveToPreviousPage
        {
            get { return 0 < this.CurrentPageIndex; }
        }

        private void MoveToPreviousPage()
        {
            if (this.CanMoveToPreviousPage)
                this.CurrentPage = this.Pages[this.CurrentPageIndex - 1];
        }

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the next page in the workflow.  If the user
        /// is viewing the last page in the workflow, this causes the Wizard
        /// to finish and be removed from the user interface.
        /// </summary>
        public ICommand MoveNextCommand
        {
            get
            {
                if (_moveNextCommand == null)
                    _moveNextCommand = new RelayCommand(
                        () => this.TryToMoveToNextPage(),
                        () => this.IsNextButtonVisible);

                return _moveNextCommand;
            }
        }

        public bool IsNextButtonVisible
        {
            get { return this.CurrentPage != null && this.CurrentPage.IsNextEnabled(); }
        }

        private void TryToMoveToNextPage()
        {
            if (this.CurrentPageIndex < this.Pages.Count - 1)
            {
                //if the current page allows us to move next
                this.CurrentPage = this.Pages[this.CurrentPageIndex + 1];
            }
            else
            {
                //when we try to move to the next page and we are on the last one
                //we want to run the save process
                Save();
            }
        }

        private void Save()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            foreach (var row in _sharedModel.Rows)
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
                catch
                {
                    //TODO: better error handling
                    Console.WriteLine("error saving {0}", row.FilePath);
                }
            }

            Mouse.OverrideCursor = null;

            new SuccessView(new SuccessViewModel(_sharedModel)).Show();
        }

        /// <summary>
        /// Returns the page ViewModel that the user is currently viewing.
        /// </summary>
        public ZuneWizardPageViewModelBase CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                if (value == _currentPage)
                    return;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = false;

                _currentPage = value;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = true;


                base.InvokePropertyChanged("CurrentPage");
                base.InvokePropertyChanged("IsOnLastPage");
                base.InvokePropertyChanged("NextButtonText");
                base.InvokePropertyChanged("BackButtonText");
            }
        }

        /// <summary>
        /// Returns true if the user is currently viewing the last page 
        /// in the workflow.  This property is used by CoffeeWizardView
        /// to switch the Next button's text to "Finish" when the user
        /// has reached the final page.
        /// </summary>
        public bool IsOnLastPage
        {
            get { return this.CurrentPageIndex == this.Pages.Count - 1; }
        }

        /// <summary>
        /// Returns a read-only collection of all page ViewModels.
        /// </summary>
        public ReadOnlyCollection<ZuneWizardPageViewModelBase> Pages
        {
            get
            {
                if (_pages == null)
                    this.CreatePages();

                return _pages;
            }
        }

        private void CreatePages()
        {
            _sharedModel = new ZuneWizardModel();

            var pages = new List<ZuneWizardPageViewModelBase>();

            pages.Add(new SelectAudioFilesViewModel(_sharedModel));
            pages.Add(new SearchViewModel(_sharedModel));
            pages.Add(new SearchResultsViewModel(_sharedModel));
            pages.Add(new DetailsViewModel(_sharedModel));

            _pages = new ReadOnlyCollection<ZuneWizardPageViewModelBase>(pages);
        }

        private int CurrentPageIndex
        {
            get
            {
                if (this.CurrentPage == null)
                {
                    Debug.Fail("Why is the current page null?");
                    return -1;
                }

                return this.Pages.IndexOf(this.CurrentPage);
            }
        }
    }
}