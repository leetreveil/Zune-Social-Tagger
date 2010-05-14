using Machine.Specifications;
using Rhino.Mocks;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.Success;

namespace ZuneSocialTagger.UnitTests.UI.ViewsViewModels.Success
{
    [Subject(typeof(SuccessViewModel))]
    public class when_class_is_constructed
    {
        Establish context = () => {
            avm = MockRepository.GenerateStub<IApplicationViewModel>();
            avm.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel();
            avm.AlbumDetailsFromWeb = new ExpandedAlbumDetailsViewModel();
        };
        Because of = () => {
            sut = new SuccessViewModel(avm);
        };

        It should_set_the_album_details_from_file = () => 
            sut.AlbumDetailsFromFile.ShouldNotBeNull();

        It should_set_the_album_details_from_web = () => 
            sut.AlbumDetailsFromWebsite.ShouldNotBeNull();

        static SuccessViewModel sut;
        static IApplicationViewModel avm;
    }

    [Subject(typeof(SuccessViewModel))]
    public class when_ok_button_is_clicked
    {
        Establish context = () =>
        {
            avm = MockRepository.GenerateMock<IApplicationViewModel>();
            sut = new SuccessViewModel(avm);
        };

        Because of = () =>
        {
            avm.Expect(x => x.SwitchToFirstView());
            avm.Expect(x => x.NotifyAppThatAnAlbumHasBeenLinked());
            sut.OKCommand.Execute(null);
        };

        It should_tell_the_application_to_switch_to_the_first_view = () => 
            avm.AssertWasCalled(x=> x.SwitchToFirstView());

        It should_tell_the_application_that_an_album_has_been_linked = () => 
            avm.AssertWasCalled(x=> x.NotifyAppThatAnAlbumHasBeenLinked());

        static SuccessViewModel sut;
        static IApplicationViewModel avm;
    }
}