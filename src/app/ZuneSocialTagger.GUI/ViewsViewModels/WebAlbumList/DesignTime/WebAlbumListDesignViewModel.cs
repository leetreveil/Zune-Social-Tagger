using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList.DesignTime
{
    public class WebAlbumListDesignViewModel
    {
        public List<AlbumDetailsViewModel> AlbumsView { get; set; }
        public double LoadingProgress { get; set; }

        public int UnlinkedTotal { get { return 200; } }

        public WebAlbumListDesignViewModel()
        {
            this.LoadingProgress = 50;
            this.AlbumsView = new List<AlbumDetailsViewModel>();

           

            this.AlbumsView.Add(new AlbumDetailsViewModel
                                {
                                    LinkStatus = LinkStatus.Unlinked,
                                    Left = 
                                        new AlbumThumbDetails
                                            {
                                                Artist = "Pendulum",
                                                Title = "Immersion",
                                            }
                                });


            this.AlbumsView.Add(new AlbumDetailsViewModel
                                {
                                    LinkStatus = LinkStatus.Linked,
                                    Left = 
                                        new AlbumThumbDetails
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music 2",
                                                ArtworkUrl =
                                                    "http://shop.hospitalrecords.com/images/product/NHS164/medium.jpg"
                                            },
                                    Right =
                                        new AlbumThumbDetails
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music 2",
                                                ArtworkUrl =
                                                    "http://shop.hospitalrecords.com/images/product/NHS164/medium.jpg"
                                            }
                                });

            this.AlbumsView.Add(new AlbumDetailsViewModel
                                {
                                    LinkStatus = LinkStatus.Linked,
                                    Left =
                                        new AlbumThumbDetails
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music",
                                            },
                                    Right =
                                        new AlbumThumbDetails
                                            {
                                                Artist = "Various Artists",
                                                Title = "Sick Music 2",
                                                ArtworkUrl =
                                                    "http://shop.hospitalrecords.com/images/product/NHS164/medium.jpg"
                                            }
                                });


            this.AlbumsView.Add(new AlbumDetailsViewModel
            {
                LinkStatus = LinkStatus.Unknown,
                Left = 
                    new AlbumThumbDetails()
                    {
                        Artist = "AFI",
                        Title = "A new AFI record",
                    }
            });
        }
    }
}