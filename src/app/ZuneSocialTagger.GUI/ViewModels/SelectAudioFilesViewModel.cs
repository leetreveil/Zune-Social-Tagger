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
    public class SelectAudioFilesViewModel : ViewModelBase
    {
        public SelectAudioFilesViewModel()
        {
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
                var commonOpenFileDialog = new CommonOpenFileDialog("Select the audio files that you want to link to the zune social");

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
                ofd.AutoUpgradeEnabled = true;
                ofd.Title = "Select the audio files that you want to link to the zune social";
                if (ofd.ShowDialog() == DialogResult.OK)
                    ReadFiles(ofd.FileNames);
            }
        }

        private void ReadFiles(IEnumerable<string> files)
        {
            ApplicationViewModel.SongsFromFile = new List<Song>();
            var tracks = ApplicationViewModel.SongsFromFile;

            foreach (var file in files)
            {
                var zuneTagContainer = SharedMethods.GetContainer(file);

                if (zuneTagContainer != null)
                    tracks.Add(new Song(file, zuneTagContainer));
                else
                    return;
            }

            tracks = tracks.OrderBy(SharedMethods.SortByTrackNumber()).ToList();

            MetaData firstTrackMetaData = tracks.First().MetaData;

            var albumMetaData = new ExpandedAlbumDetailsViewModel
            {
                Artist = firstTrackMetaData.AlbumArtist,
                Title = firstTrackMetaData.AlbumName,
                SongCount = tracks.Count.ToString(),
                Year = firstTrackMetaData.Year
            };

            ApplicationViewModel.AlbumDetailsFromFile = albumMetaData;

            Messenger.Default.Send<Type, ApplicationViewModel>(typeof(SearchViewModel));
            Messenger.Default.Send<string, SearchViewModel>(firstTrackMetaData.AlbumName + " " + firstTrackMetaData.AlbumArtist);
        }
    }
}