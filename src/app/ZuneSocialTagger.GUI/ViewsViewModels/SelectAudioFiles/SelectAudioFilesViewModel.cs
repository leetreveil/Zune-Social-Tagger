using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Search;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;

namespace ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles
{
    public class SelectAudioFilesViewModel : ViewModelBase
    {
        private readonly ViewLocator _locator;
        private readonly SharedModel _sharedModel;

        public SelectAudioFilesViewModel(ViewLocator locator, SharedModel sharedModel)
        {
            _sharedModel = sharedModel;
            _locator = locator;
            CanSwitchToNewMode = true;
            SelectFilesCommand = new RelayCommand(SelectFiles);
            SwitchToNewModeCommand = new RelayCommand(SwitchToNewMode);    
        }

        public bool CanSwitchToNewMode { get; set; }
        public RelayCommand SelectFilesCommand { get; private set; }
        public RelayCommand SwitchToNewModeCommand { get; private set; }

        public void SwitchToNewMode()
        {
            _locator.SwitchToView<WebAlbumListView, WebAlbumListViewModel>();
        }

        public void SelectFiles()
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var commonOpenFileDialog = new CommonOpenFileDialog("Select the audio files that you want to link to the zune social")
                                               {
                                                   Multiselect = true,
                                                   EnsureFileExists = true
                                               };

                commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("Audio Files", "*.mp3;*.wma"));
                commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("MP3 Files", "*.mp3"));
                commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter("WMA Files", "*.wma"));

                if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.OK)
                    ReadFiles(commonOpenFileDialog.FileNames);
            }
            else
            {
                var ofd = new OpenFileDialog { 
                    Multiselect = true, 
                    Filter = "Audio files .mp3,.wma |*.mp3;*.wma", 
                    AutoUpgradeEnabled = true,
                    Title = "Select the audio files that you want to link to the zune social",
                    CheckFileExists = true
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                    ReadFiles(ofd.FileNames);
            }
        }

        private void ReadFiles(IEnumerable<string> files)
        {
            try
            {
                //get the files and sort by trackNumber
                var containers = SharedMethods.GetContainers(files);
                containers = SharedMethods.SortByTrackNumber(containers);

                //get the first tracks metadata which is used to set some details
                MetaData firstTrackMetaData = containers.First().MetaData;

                //set the album details that is used throughout the app
                _sharedModel.AlbumDetailsFromFile = SharedMethods.SetAlbumDetails(firstTrackMetaData, containers.Count);
                _sharedModel.SongsFromFile = containers;

                //as soon as the view has switched start searching
                var searchVm = _locator.SwitchToView<SearchView,SearchViewModel>();
                searchVm.Search(firstTrackMetaData.AlbumArtist, firstTrackMetaData.AlbumName);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Error, ex.Message));
            }
        }
    }
}