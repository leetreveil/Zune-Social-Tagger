using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using ZuneSocialTagger.GUIV2.Commands;
using System.Linq;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class ZuneWizardViewModel : INotifyPropertyChanged
    {

        RelayCommand _cancelCommand;
        ZuneWizardPageViewModelBase _currentPage;
        RelayCommand _moveNextCommand;
        RelayCommand _movePreviousCommand;
        ReadOnlyCollection<ZuneWizardPageViewModelBase> _pages;

        public string NextButtonText 
        {
            get { return this.CurrentPage.NextButtonText; }
        }

        public string BackButtonText
        {
            get { return this.CurrentPage.BackButtonText; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ZuneWizardViewModel()
        {
            this.CurrentPage = this.Pages[0];

            //set up the handler for overrides (this allows a page to move to the next page on demand without having to click next)
            foreach (var page in Pages)
                page.MoveNextOverride += page_MoveNextOverride;
        }

        void page_MoveNextOverride(object sender, EventArgs e)
        {
            TryToMoveToNextPage();
        }

        /// <summary>
        /// Returns the command which, when executed, cancels the order 
        /// and causes the Wizard to be removed from the user interface.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new RelayCommand(() => this.CloseWizard());

                return _cancelCommand;
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

        bool CanMoveToPreviousPage
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
                        () => this.IsButtonEnabled);

                return _moveNextCommand;
            }
        }

        bool CanMoveToNextPage
        {
            get { return this.CurrentPage != null && this.CurrentPage.IsValid(); }
        }

        bool IsButtonEnabled
        {
            get { return this.CurrentPage != null && this.CurrentPage.CanMoveNext(); }
        }

        void TryToMoveToNextPage()
        {
            if (this.CanMoveToNextPage)
            {
                if (this.CurrentPageIndex < this.Pages.Count - 1)
                {
                    //if the current page allows us to move next
                    if (this.CurrentPage.IsValid())
                    {
                        this.CurrentPage = this.Pages[this.CurrentPageIndex + 1];
                    }
                }
                else
                {
                    //do whatever you want at the very end
                    foreach (var row in ZuneWizardModel.GetInstance().Rows)
                    {
                        row.UpdateContainer();
                        SaveContainerToFile saveContainerToFile = new SaveContainerToFile(row.SongPathAndContainer);

                        saveContainerToFile.Save();
                    }

                }

            }
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

                this.CurrentPage.MoveNextOverride += delegate
                                                         {
                                                             Console.WriteLine("Override invoked");
                                                         };

                this.OnPropertyChanged("CurrentPage");
                this.OnPropertyChanged("IsOnLastPage");
                this.OnPropertyChanged("NextButtonText");
                this.OnPropertyChanged("BackButtonText");
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

        /// <summary>
        /// Raised when the wizard should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        void CreatePages()
        {
            var pages = new List<ZuneWizardPageViewModelBase>();
            pages.Add(new SelectAudioFilesViewModel());
            pages.Add(new SearchViewModel());
            pages.Add(new SearchResultsViewModel());
            pages.Add(new DetailsViewModel());

            _pages = new ReadOnlyCollection<ZuneWizardPageViewModelBase>(pages);
        }

        int CurrentPageIndex
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

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }


        void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CloseWizard()
        {
            this.OnRequestClose();
        }
    }
}