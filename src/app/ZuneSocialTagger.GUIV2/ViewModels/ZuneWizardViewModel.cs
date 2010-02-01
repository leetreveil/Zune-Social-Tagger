using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;
using leetreveil.AutoUpdate.Framework;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Properties;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    /// <summary>
    /// Orchestrates the moving between pages (wizard)
    /// </summary>
    public class ZuneWizardViewModel : NotifyPropertyChangedImpl
    {
        private ZuneWizardPageViewModelBase _currentPage;
        private RelayCommand _moveNextCommand;
        private RelayCommand _movePreviousCommand;
        private ReadOnlyCollection<ZuneWizardPageViewModelBase> _pages;
        private ZuneWizardModel _sharedModel;
        private bool _updateAvailable;

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

            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                  Settings.Default.UpdateExeName);

            if (Settings.Default.CheckForUpdates)
            {
                try
                {
                    //do update checking stuff here
                    var updateManager = UpdateManager.Instance;

                    updateManager.UpdateExePath = updaterPath;
                    updateManager.AppFeedUrl = Settings.Default.UpdateFeedUrl;
                    updateManager.UpdateExe = Resources.socialtaggerupdater;

                    //always clean up at startup because we cant do it at the end
                    updateManager.CleanUp();

                    ThreadPool.QueueUserWorkItem(state =>
                                                     {
                                                         if (updateManager.CheckForUpdate())
                                                             this.UpdateAvailable = true;
                                                     });
                }
                catch (Exception e)
                {
                    //TODO: log that we could not check for updates
                }
            }
        }

        private void PageMoveNextOverride(object sender, EventArgs e)
        {
            this.CurrentPage = this.Pages[this.CurrentPageIndex + 1];
        }

        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set
            {
                if (value != _updateAvailable)
                {
                    _updateAvailable = value;
                    base.InvokePropertyChanged("UpdateAvailable");
                }

            }
        }

        private ICommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand =
                        new RelayCommand(
                            () => new UpdateView(new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version)).Show());
                }

                return _updateCommand;
            }
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
                        () => this.TellPageThatNextHasBeenClicked(),
                        () => this.IsNextButtonEnabled);

                return _moveNextCommand;
            }
        }

        public bool IsNextButtonEnabled
        {
            get { return this.CurrentPage != null && this.CurrentPage.IsNextEnabled(); }
        }

        public bool IsNextButtonVisible
        {
            get { return this.CurrentPage != null && this.CurrentPage.IsNextVisible(); }
        }

        public bool IsBackButtonVisible
        {
            get { return this.CurrentPage != null && this.CurrentPage.IsBackVisible(); }
        }

        private void TellPageThatNextHasBeenClicked()
        {
            this.CurrentPage.InvokeMoveNextClicked();
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


                //force the ui to check for changes when the current page has changed
                base.InvokePropertyChanged("CurrentPage");
                base.InvokePropertyChanged("IsOnLastPage");
                base.InvokePropertyChanged("NextButtonText");
                base.InvokePropertyChanged("BackButtonText");
                base.InvokePropertyChanged("IsBackButtonVisible");
                base.InvokePropertyChanged("IsNextButtonVisible");
            }
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