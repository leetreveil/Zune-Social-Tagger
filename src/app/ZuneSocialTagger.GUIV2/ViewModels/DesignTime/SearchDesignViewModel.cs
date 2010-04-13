using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels.DesignTime
{
    public class SearchDesignViewModel
    {

        public SearchDesignViewModel()
        {
            this.AlbumDetails = new ExpandedAlbumDetailsViewModel()
                                    {
                                        Artist = "Pendulum",
                                        ArtworkUrl = "http://images.play.com/covers/12691916x.jpg",
                                        SongCount = "10",
                                        Title = "Immersion",
                                        Year = "2010"
                                    };

            this.SearchText = "Pendulum Immersion";
            this.SearchResultsViewModel = new SearchResultsViewModel(new ZuneWizardModel());
            


            var foundAlbums = new ObservableCollection<Album>();

            foundAlbums.Add(new Album()
            {
                Artist = "Pendulum",
                Title = "In Silico",
                ReleaseYear = "2009",
                ArtworkUrl = "http://img.noiset.com/images/album/pendulum-in-silico-cd-cover-artwork-1841.jpeg"
            });

            foundAlbums.Add(new Album()
            {
                Artist = "Pendulum",
                Title = "Hold Your Color",
                ReleaseYear = "2006",
                ArtworkUrl = "http://upload.wikimedia.org/wikipedia/en/thumb/e/ea/Pendulum-hold_your_colour.jpg/200px-Pendulum-hold_your_colour.jpg"
            });

            this.SearchResultsViewModel.Albums = foundAlbums;
        }

        public ExpandedAlbumDetailsViewModel AlbumDetails { get; set; }
        public SearchResultsViewModel SearchResultsViewModel { get; set; }

        public string SearchText { get; set; }
    }

}