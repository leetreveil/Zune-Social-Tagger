using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList.DesignTime
{
    public class WebAlbumListDesignViewModel
    {
        public List<AlbumDetailsViewModel> Albums { get; set; }
        public double LoadingProgress { get; set; }
        public string SearchText
        {
            get { return "AFI";}
        }

        public string ScanAllText
        {
            get { return "SCAN ALL"; }
        }

        public WebAlbumListDesignViewModel()
        {
            this.LoadingProgress = 50;
            this.Albums = new List<AlbumDetailsViewModel>();

            this.Albums.Add(new AlbumDetailsViewModel
                                {
                                    LinkStatus = LinkStatus.Unlinked,
                                    ZuneAlbumMetaData =
                                        new DbAlbum
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
                                        new DbAlbum
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music 2",
                                                ReleaseYear = "2010",
                                                ArtworkUrl =
                                                    "http://shop.hospitalrecords.com/images/product/NHS164/medium.jpg"
                                            },
                                    WebAlbumMetaData =
                                        new WebAlbum
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
                                        new DbAlbum
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music",
                                                ReleaseYear = "2010",
                                            },
                                    WebAlbumMetaData =
                                        new WebAlbum
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
                LinkStatus = LinkStatus.Unknown,
                ZuneAlbumMetaData =
                    new DbAlbum
                    {
                        Artist = "AFI",
                        Title = "A new AFI record",
                        ReleaseYear = "2010",
                    }
            });
        }
    }
}