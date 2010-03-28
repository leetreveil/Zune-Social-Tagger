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

        public bool CanSwitchToNewMode { get; set; }
        public RelayCommand SelectFilesCommand { get; private set; }
        public RelayCommand SwitchToNewModeCommand { get; private set; }

        public void ViewHasFinishedLoading()
        {
            FinishedLoading.Invoke();
        }

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
            var selectedAlbum = new SelectedAlbum();

            foreach (var file in files)
            {
                try
                {
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(file);
                    selectedAlbum.Tracks.Add(new Song(file,container));
                }
                catch(AudioFileReadException ex)
                {
                    Messenger.Default.Send(new ErrorMessage(ErrorMode.Error,ex.Message));
                    return;
                }
            }

            selectedAlbum.Tracks = selectedAlbum.Tracks.OrderBy(SharedMethods.SortByTrackNumber()).ToObservableCollection();

            MetaData ftMetaData = selectedAlbum.Tracks.First().MetaData;

            _model.SearchText = ftMetaData.AlbumArtist + " " + ftMetaData.AlbumName;

            selectedAlbum.ZuneAlbumMetaData = new ExpandedAlbumDetailsViewModel
            {
                Artist = ftMetaData.AlbumArtist,
                Title = ftMetaData.AlbumName,
                SongCount = selectedAlbum.Tracks.Count.ToString(),
                Year = ftMetaData.Year
            };

            _model.SelectedAlbum = selectedAlbum;

            Messenger.Default.Send(typeof(SearchViewModel));
        }
    }
}