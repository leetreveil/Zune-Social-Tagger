﻿using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;
using System.Collections.Generic;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneWizardModel
    {
        ObservableCollection<DetailRow> Rows { get; set; }
        ObservableCollection<Album> FoundAlbums { get; set; }

        /// <summary>
        /// The details of the selected album from file
        /// </summary>
        ExpandedAlbumDetailsViewModel FileAlbumDetails { get; set; }

        ExpandedAlbumDetailsViewModel WebAlbumDetails { get; set; }

        string SearchText { get; set; }

        ObservableCollection<AlbumDetailsViewModel> AlbumsFromDatabase { get; set; }

        int SelectedItemInListView { get; set; }
    }
}