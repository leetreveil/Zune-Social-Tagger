using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : ViewModelBase, IFirstPage
    {
        private readonly IZuneWizardModel _model;

        public event Action FinishedLoading = delegate { };

        public SelectAudioFilesViewModel(IZuneWizardModel model)
        {
            _model = model;

            this.CanSwitchToNewMode = true;

            this.SelectFilesCommand = new RelayCommand(SelectFiles);
            this.SwitchToNewModeCommand = new RelayCommand(SwitchToNewMode);    
        }

        public void ViewHasFinishedLoading()
        {
            FinishedLoading.Invoke();
        }

        public bool CanSwitchToNewMode { get; set; }
        public RelayCommand SelectFilesCommand { get; private set; }
        public RelayCommand SwitchToNewModeCommand { get; private set; }


        public void SwitchToNewMode()
        {
            Messenger.Default.Send(typeof(WebAlbumListViewModel));
        }

        public void SelectFiles()
        {
            var ofd = new OpenFileDialog { Multiselect = true, Filter = "Audio files |*.mp3;*.wma" };

            if (ofd.ShowDialog() == DialogResult.OK)
                ReadFiles(ofd.FileNames);
        }

        private void ReadFiles(IEnumerable<string> files)
        {
            _model.Rows = new ObservableCollection<DetailRow>();

            foreach (var file in files)
            {
                try
                {
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(file);
                    _model.Rows.Add(new DetailRow(file,container));
                    _model.Rows = _model.Rows.OrderBy(SharedMethods.SortByTrackNumber()).ToObservableCollection();
                }
                catch(AudioFileReadException ex)
                {
                    Messenger.Default.Send(new ErrorMessage(ErrorMode.Error,ex.Message));
                    return;
                }
            }

            MetaData ftMetaData = _model.Rows.First().MetaData;

            _model.SearchText = ftMetaData.AlbumArtist + " " + ftMetaData.AlbumName;

            _model.FileAlbumDetails = new ExpandedAlbumDetailsViewModel
            {
                Artist = ftMetaData.AlbumArtist,
                Title = ftMetaData.AlbumName,
                SongCount = _model.Rows.Count.ToString(),
                Year = ftMetaData.Year
            };

            Messenger.Default.Send(typeof(SearchViewModel));
        }
    }
}