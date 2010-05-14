using Machine.Specifications;
using Rhino.Mocks;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.Success;

namespace ZuneSocialTagger.UnitTests.UI.ViewsViewModels.Success
{
    //[Subject(typeof(SuccessViewModel))]
    //public class when_class_is_constructed
    //{
    //    Establish context = () => {
    //        avm = MockRepository.GenerateStub<IApplicationViewModel>();
    //        avm.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel();
    //        avm.AlbumDetailsFromWeb = new ExpandedAlbumDetailsViewModel();
    //    };

    //    Because of = () => {
    //        sut = new SuccessViewModel(avm);
    //    };

    //    It should_set_the_album_details_from_file = () => 
    //        sut.AlbumDetailsFromFile.ShouldNotBeNull();

    //    It should_set_the_album_details_from_web = () => 
    //        sut.AlbumDetailsFromWebsite.ShouldNotBeNull();

    //    static SuccessViewModel sut;
    //    static IApplicationViewModel avm;
    //}

    [Subject(typeof(SuccessViewModel))]
    public class when_ok_button_is_clicked
    {
        Establish context = () =>
        {
            locator = MockRepository.GenerateMock<IViewModelLocator>();
            sut = new SuccessViewModel(locator);
        };

        Because of = () =>
        {
            locator.Expect(x => x.SwitchToFirstViewModel());
            sut.OKCommand.Execute(null);
        };

        It should_tell_the_application_to_switch_to_the_first_view = () =>
            locator.AssertWasCalled(x => x.SwitchToFirstViewModel());


        static SuccessViewModel sut;
        static IViewModelLocator locator;
    }
}