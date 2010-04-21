using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SelectAudioFilesViewModel : ViewModelBase, IFirstPage
    {
        private readonly IZuneWizardModel _model;

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

        public void SwitchToNewMode()
        {
            Messenger.Default.Send(typeof(IFirstPage));
        }

        public void SelectFiles()
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var commonOpenFileDialog = new CommonOpenFileDialog("Select audio files");

                commonOpenFileDialog.Multiselect = true;
                commonOpenFileDialog.EnsureFileExists = true;
                commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("MP3 Files", "*.mp3"));
                commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("WMA Files", "*.wma"));

                if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.OK)
                    ReadFiles(commonOpenFileDialog.FileNames);
            }
            else
            {
                var ofd = new OpenFileDialog { Multiselect = true, Filter = "Audio files .mp3,.wma |*.mp3;*.wma" };

                if (ofd.ShowDialog() == DialogResult.OK)
                    ReadFiles(ofd.FileNames);
            }
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

            _model.SearchText = ftMetaData.AlbumName + " " + ftMetaData.AlbumArtist;

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