using System;
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
        private readonly SelectedAlbum _selectedAlbum;

        public SelectAudioFilesViewModel(SelectedAlbum selectedAlbum)
        {
            _selectedAlbum = selectedAlbum;
            this.CanSwitchToNewMode = true;
            this.SelectFilesCommand = new RelayCommand(SelectFiles);
            this.SwitchToNewModeCommand = new RelayCommand(SwitchToNewMode);    
        }

        public bool CanSwitchToNewMode { get; set; }
        public RelayCommand SelectFilesCommand { get; private set; }
        public RelayCommand SwitchToNewModeCommand { get; private set; }

        public void SwitchToNewMode()
        {
            Messenger.Default.Send<Type, ApplicationViewModel>(typeof(WebAlbumListViewModel));
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
            foreach (var file in files)
            {
                var zuneTagContainer = SharedMethods.GetContainer(file);

                if (zuneTagContainer != null)
                    _selectedAlbum.Tracks.Add(new Song(file, zuneTagContainer));
                else
                    return;
            }

            _selectedAlbum.Tracks = _selectedAlbum.Tracks.OrderBy(SharedMethods.SortByTrackNumber()).ToObservableCollection();

            MetaData ftMetaData = _selectedAlbum.Tracks.First().MetaData;

            _selectedAlbum.ZuneAlbumMetaData = new ExpandedAlbumDetailsViewModel
            {
                Artist = ftMetaData.AlbumArtist,
                Title = ftMetaData.AlbumName,
                SongCount = _selectedAlbum.Tracks.Count.ToString(),
                Year = ftMetaData.Year
            };

            Messenger.Default.Send<Type, ApplicationViewModel>(typeof(SearchViewModel));
            Messenger.Default.Send<string, SearchViewModel>(ftMetaData.AlbumName + " " + ftMetaData.AlbumArtist);
            Messenger.Default.Send<ExpandedAlbumDetailsViewModel,SearchViewModel>(_selectedAlbum.ZuneAlbumMetaData);
        }
    }
}