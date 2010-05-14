using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
{
    public interface IApplicationViewModel
    {
        ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        ExpandedAlbumDetailsViewModel AlbumDetailsFromWeb { get; set; }

        /// <summary>
        /// The downloaded album songs after searching
        /// </summary>
        List<WebTrack> SongsFromWebsite { get; set; }

        /// <summary>
        /// The actual track details from the audio files
        /// </summary>
        List<Song> SongsFromFile { get; set; }

        RelayCommand UpdateCommand { get; }
        RelayCommand ShowAboutSettingsCommand { get; }
        RelayCommand CloseAppCommand { get; }
        string ErrorMessageText { get; set; }
        ErrorMode ErrorMessageMode { get; set; }
        bool ShouldShowErrorMessage { get; set; }
        bool UpdateAvailable { get; set; }
        ViewModelBase CurrentPage { get; }

        void ViewHasLoaded();
        void DisplayMessage(ErrorMessage message);
        void AlbumBeenLinked();

        void SortData();
    }
}