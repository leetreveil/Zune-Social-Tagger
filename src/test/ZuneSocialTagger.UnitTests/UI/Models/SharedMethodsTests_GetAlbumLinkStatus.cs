using Machine.Specifications;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.UnitTests.UI.Models
{
    [Subject("GetAlbumLinkStatus")]
    public class when_getting_then_album_link_status_for_shape_of_punk_to_come_with_THE_prefix
    {
        It should_return_linked = () =>
            SharedMethods.GetAlbumLinkStatus("Shape Of Punk To Come", "The Refused", "Shape Of Punk To Come", "Refused")
            .ShouldEqual(LinkStatus.Linked);
    }

    [Subject("GetAlbumLinkStatus")]
    public class when_getting_the_album_link_status_for_an_album_title_with_an_accent_in_and_the_source_does_not
    {
        It should_return_linked = () =>
            SharedMethods.GetAlbumLinkStatus("Takk", "Sigur Ros", "Takk", "Sigur Rós")
            .ShouldEqual(LinkStatus.Linked);
    }

    [Subject("GetAlbumLinkStatus")]
    public class when_getting_the_album_link_status_for_an_album_title_that_starts_with_A_prefix_and_the_source_does_not
    {
        It should_return_linked = () =>
            SharedMethods.GetAlbumLinkStatus("Takk", "A Sigur Ros", "Takk", "Sigur Ros")
            .ShouldEqual(LinkStatus.Linked);
    }

    [Subject("GetAlbumLinkStatus")]
    public class when_getting_the_album_link_status_for_an_album_title_that_has_full_stop_suffix_and_the_source_does_not
    {
        It should_return_linked = () =>
            SharedMethods.GetAlbumLinkStatus("Takk", "Sigur Ros.", "Takk", "Sigur Ros")
            .ShouldEqual(LinkStatus.Linked);
    }

    [Subject("GetAlbumLinkStatus")]
    public class when_getting_the_album_link_status_for_an_album_title_that_starts_has_question_mark_suffix_and_the_source_does_not
    {
        It should_return_linked = () =>
            SharedMethods.GetAlbumLinkStatus("Takk", "Sigur Ros?", "Takk", "Sigur Ros")
            .ShouldEqual(LinkStatus.Linked);
    }

    [Subject("GetAlbumLinkStatus")]
    public class when_getting_the_album_link_status_for_an_album_title_that_starts_with_a_disallowed_prefix
    {
        It should_return_mismatch = () =>
            SharedMethods.GetAlbumLinkStatus("Takk", "X Sigur Ros", "Takk", "Sigur Ros")
            .ShouldEqual(LinkStatus.AlbumOrArtistMismatch);
    }

    [Subject("GetAlbumLinkStatus")]
    public class when_getting_the_album_link_status_for_an_album_title_that_starts_with_a_disallowed_suffix
    {
        It should_return_mismatch = () =>
            SharedMethods.GetAlbumLinkStatus("Takk", "Sigur Ros%", "Takk", "Sigur Ros")
            .ShouldEqual(LinkStatus.AlbumOrArtistMismatch);
    }
}
