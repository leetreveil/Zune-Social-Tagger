using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SelectAudioFilesViewModel : ViewModelBaseExtended
    {
        private readonly ZuneWizardModel _model;

        public SelectAudioFilesViewModel(ZuneWizardModel model)
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
            Messenger.Default.Send(typeof(WebAlbumListViewModel));
        }

        public void SelectFiles()
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var commonOpenFileDialog = new CommonOpenFileDialog("Select audio files");

                commonOpenFileDialog.Multiselect = true;
                commonOpenFileDialog.EnsureFileExists = true;
                commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("Audio Files", "*.mp3;*.wma"));
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
                var zuneTagContainer = SharedMethods.GetContainer(file);

                if (zuneTagContainer != null)
                    selectedAlbum.Tracks.Add(new Song(file, zuneTagContainer));
                else
                    return;
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