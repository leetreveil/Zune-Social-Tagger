using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.DetailsReadyOnly;
using ZuneSocialTagger.GUI.ViewsViewModels.Details;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    /// <summary>
    /// Selects which view to create at the end
    /// </summary>
    public class DetailsViewSwitcher
    {
        private readonly IViewModelLocator _locator;
        private readonly WebAlbum _webAlbum;
        private readonly IEnumerable<IZuneTagContainer> _fileTracks;

        public DetailsViewSwitcher(IViewModelLocator locator, SharedModel sharedModel)
        {
            _locator = locator;
            _webAlbum = sharedModel.WebAlbum;
            _fileTracks = sharedModel.SongsFromFile;
        }

        public void SwitchToCorrectView()
        {
            if (DoAllTracksHaveMatchingTrackTitles())
            {
                var detailsReadOnlyViewModel = _locator.SwitchToViewModel<DetailsReadOnlyViewModel>();

                detailsReadOnlyViewModel.Genre = _webAlbum.Genre;
                detailsReadOnlyViewModel.ImageUrl = _webAlbum.ArtworkUrl;
                detailsReadOnlyViewModel.ReleaseYear = _webAlbum.ReleaseYear;
                detailsReadOnlyViewModel.AlbumTitle = _webAlbum.Title;
                detailsReadOnlyViewModel.Artist = _webAlbum.Artist;
                detailsReadOnlyViewModel.TrackCount = _webAlbum.Tracks.Count().ToString();

                foreach (var downloadedTrack in _webAlbum.Tracks)
                {
                    detailsReadOnlyViewModel.Tracks.Add(new TrackWithTrackNum
                    {
                        TrackTitle = downloadedTrack.Title,
                        TrackNumber = downloadedTrack.TrackNumber
                    });
                }
            }
            else
            {
                _locator.SwitchToViewModel<DetailsViewModel>();
            }
        }

        private bool DoAllTracksHaveMatchingTrackTitles()
        {
            return
                _fileTracks.Any(
                    track =>
                    SharedMethods.DoesAlbumTitleMatch(_webAlbum.Tracks.Select(x => x.Title), track.MetaData.Title));
        }
    }
}