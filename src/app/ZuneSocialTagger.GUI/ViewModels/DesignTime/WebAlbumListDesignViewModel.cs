using System.Collections.Generic;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels.DesignTime
{
    public class WebAlbumListDesignViewModel
    {
        public List<AlbumDetailsViewModel> Albums { get; set; }

        public string ScanAllText
        {
            get { return "SCAN ALL"; }
        }

        public WebAlbumListDesignViewModel()
        {
            this.Albums = new List<AlbumDetailsViewModel>();

            this.Albums.Add(new AlbumDetailsViewModel
                                {
                                    LinkStatus = LinkStatus.Unlinked,
                                    ZuneAlbumMetaData =
                                        new Album
                                            {
                                                Artist = "Pendulum",
                                                Title = "Immersion",
                                                ReleaseYear = "2010"
                                            }
                                });


            this.Albums.Add(new AlbumDetailsViewModel
                                {
                                    LinkStatus = LinkStatus.Linked,
                                    ZuneAlbumMetaData =
                                        new Album
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music 2",
                                                ReleaseYear = "2010",
                                                ArtworkUrl =
                                                    "http://shop.hospitalrecords.com/images/product/NHS164/medium.jpg"
                                            },
                                    WebAlbumMetaData =
                                        new Album
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music 2",
                                                ReleaseYear = "2010",
                                                ArtworkUrl =
                                                    "http://shop.hospitalrecords.com/images/product/NHS164/medium.jpg"
                                            }
                                });

            this.Albums.Add(new AlbumDetailsViewModel
                                {
                                    LinkStatus = LinkStatus.AlbumOrArtistMismatch,
                                    ZuneAlbumMetaData =
                                        new Album
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music",
                                                ReleaseYear = "2010",
                                            },
                                    WebAlbumMetaData =
                                        new Album
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music 2",
                                                ReleaseYear = "2010",
                                                ArtworkUrl =
                                                    "http://shop.hospitalrecords.com/images/product/NHS164/medium.jpg"
                                            }
                                });

            this.Albums.Add(new AlbumDetailsViewModel
                                {
                                    LinkStatus = LinkStatus.Unavailable,
                                    ZuneAlbumMetaData =
                                        new Album
                                            {
                                                Artist = "AFI",
                                                Title = "A new AFI record",
                                                ReleaseYear = "2010",
                                            }
                                });
        }
    }
}