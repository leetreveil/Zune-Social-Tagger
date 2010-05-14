using System.Collections.Generic;
using Machine.Specifications;
using Rhino.Mocks;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Details;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.UnitTests.UI.ViewsViewModels.Details
{
    [Subject(typeof(DetailsViewModel))]
    public class when_class_is_constructed_with_a_single_row
    {
        Establish context = () =>
        {
            avm = MockRepository.GenerateStub<IApplicationViewModel>();
            avm.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel();
            avm.AlbumDetailsFromWeb = new ExpandedAlbumDetailsViewModel();
            avm.SongsFromFile = new List<Song>{new Song{MetaData = new MetaData{TrackNumber = "01",Title = "FirstSong"}}};
            avm.SongsFromWebsite = new List<WebTrack>{new WebTrack{TrackNumber = "01",Title = "First Song (Web)"}};
        };

        Because of = () =>
        {
            sut = new DetailsViewModel(avm);
        };

        It should_set_the_album_details_from_file = () =>
            sut.AlbumDetailsFromFile.ShouldNotBeNull();

        It should_set_the_album_details_from_web = () =>
            sut.AlbumDetailsFromWebsite.ShouldNotBeNull();

        It should_add_a_single_row = () => 
            sut.Rows.ShouldNotBeEmpty();

        static DetailsViewModel sut;
        static IApplicationViewModel avm;
    }
}