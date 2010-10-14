using System;
using System.Collections.Generic;
using System.Text;
using Id3Tag;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using Machine.Specifications;
using ZuneSocialTagger.Core.IO.ID3Tagger;
using System.Linq;
using ZuneSocialTagger.Core.IO;

namespace ZuneSocialTagger.UnitTests.Core.ID3Tagger
{
    public static class SampleData
    {
        public static Guid SomeGuid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933");
        public static string SomeArtist = "Editors";
        public static string SomeAlbum = "In This Light And On This Evening";
        public static string SomeTitle = "The Boxer";
        public static string SomeYear = "2009";
    }

    public class when_a_tag_container_is_loaded_with_the_correct_media_ids_present
    {
        Because of = () => {
            TagContainer container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);
            container.Add(new PrivateFrame(ZuneIds.Artist, SampleData.SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(ZuneIds.Album, SampleData.SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(ZuneIds.Track, SampleData.SomeGuid.ToByteArray()));

            sut = new ZuneMP3TagContainer(container);
        };

        It should_have_three_items = () =>
            sut.ReadZuneAttributes().ShouldNotBeEmpty();

        It should_be_able_to_read_the_zune_album_artist_media_id = () =>
            sut.ReadZuneAttributes().Where(x => x.Name == ZuneIds.Artist).First()
            .Guid.ShouldEqual(SampleData.SomeGuid);

        It should_be_able_to_read_the_zune_album_media_id = () =>
            sut.ReadZuneAttributes().Where(x => x.Name == ZuneIds.Album).First()
            .Guid.ShouldEqual(SampleData.SomeGuid);

        It should_be_able_to_read_the_track_media_id = () =>
            sut.ReadZuneAttributes().Where(x => x.Name == ZuneIds.Track).First()
            .Guid.ShouldEqual(SampleData.SomeGuid);

        It should_not_be_able_to_add_the_same_media_and_should_replace_the_existing_one = () =>
        {
            sut.AddZuneAttribute(new ZuneAttribute(ZuneIds.Track, SampleData.SomeGuid));
            sut.ReadZuneAttributes().Count().ShouldEqual(3);
        };

        It should_be_able_to_remove_the_track_guid = () => {
            sut.RemoveZuneAttribute(ZuneIds.Track);
            sut.ReadZuneAttributes().Count().ShouldEqual(2);
        };

        static ZuneMP3TagContainer sut;
    }

    public class when_a_tag_container_is_loaded_with_one_reapeating_media_id
    {
        Because of = () => {
            var container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame(ZuneIds.Track, SampleData.SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(ZuneIds.Track, SampleData.SomeGuid.ToByteArray()));

            sut = new ZuneMP3TagContainer(container);
        };

        It should_be_able_to_remove_that_guid= () =>
        {
            sut.RemoveZuneAttribute(ZuneIds.Track);
            sut.ReadZuneAttributes().ShouldBeEmpty();
        };

        static ZuneMP3TagContainer sut;
    }

    public class when_an_empty_tag_container_is_loaded
    {
        Because of = () =>
            sut = new ZuneMP3TagContainer(Id3TagFactory.CreateId3Tag(TagVersion.Id3V23));

        It should_not_be_able_to_read_any_media_ids = () =>
            sut.ReadZuneAttributes().ShouldBeEmpty();

        It should_be_able_to_add_a_media_id = () =>
        {
            sut.AddZuneAttribute(new ZuneAttribute(ZuneIds.Track, SampleData.SomeGuid));
            sut.ReadZuneAttributes().Where(x => x.Name == ZuneIds.Track).First().Guid.ShouldEqual(
                SampleData.SomeGuid);
        };

        It should_not_do_anything_when_removing_a_media_id = () =>
        {
            sut.RemoveZuneAttribute(ZuneIds.Track);
            sut.ReadZuneAttributes().ShouldBeEmpty();
        };

        It should_return_an_object_with_empty_properties = () =>
        {
            MetaData metaData = sut.ReadMetaData();

            metaData.AlbumArtist.ShouldBeEmpty();
            metaData.AlbumName.ShouldBeEmpty();
            metaData.ContributingArtists.Count().ShouldEqual(1);
            metaData.DiscNumber.ShouldBeEmpty();
            metaData.Genre.ShouldBeEmpty();
            metaData.Title.ShouldBeEmpty();
            metaData.TrackNumber.ShouldBeEmpty();
            metaData.Year.ShouldBeEmpty();
        };

        It should_be_able_to_add_all_the_meta_data = () =>
        {

            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumName = "Forever",
                ContributingArtists = new List<string> { "U2", "AFI" },
                DiscNumber = "1/1",
                Genre = "Pop",
                Title = "Wallet",
                TrackNumber = "2",
                Year = "2009"
            };

            sut.AddMetaData(metaData);

            MetaData result = sut.ReadMetaData();

            result.AlbumArtist.ShouldEqual(metaData.AlbumArtist);
            result.AlbumName.ShouldEqual(metaData.AlbumName);
            result.ContributingArtists.ShouldEqual(metaData.ContributingArtists);
            result.DiscNumber.ShouldEqual(metaData.DiscNumber);
            result.Genre.ShouldEqual(metaData.Genre);
            result.Title.ShouldEqual(metaData.Title);
            result.TrackNumber.ShouldEqual(metaData.TrackNumber);
            result.Year.ShouldEqual(metaData.Year);
        };

        static ZuneMP3TagContainer sut;
    }

    public class when_a_tag_container_is_loaded_with_only_one_media_id_but_its_value_is_incorrect
    {
        Because of = () => {
            var container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame("ZuneAlbumArtistMediaID", Guid.NewGuid().ToByteArray()));

            sut = new ZuneMP3TagContainer(container);
        };

        It should_be_able_to_update_the_media_id = () => {
            sut.AddZuneAttribute(new ZuneAttribute(ZuneIds.Artist, SampleData.SomeGuid));
            sut.ReadZuneAttributes().Count().ShouldEqual(1);
            sut.ReadZuneAttributes().Where(x => x.Name == ZuneIds.Artist).First().Guid.ShouldEqual(
                SampleData.SomeGuid);
        };

        static ZuneMP3TagContainer sut;
    }

    public class when_a_tag_container_contains_meta_data_about_the_track
    {
        Because of = () => {
            var container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new TextFrame("TALB", SampleData.SomeAlbum, Encoding.UTF8));
            container.Add(new TextFrame("TPE1", SampleData.SomeArtist, Encoding.UTF8));
            container.Add(new TextFrame("TPE2", "Various Artists", Encoding.UTF8));
            container.Add(new TextFrame("TIT2", SampleData.SomeTitle, Encoding.UTF8));
            container.Add(new TextFrame("TYER", SampleData.SomeYear, Encoding.UTF8));
            container.Add(new TextFrame("TRCK", "2", Encoding.UTF8));
            container.Add(new TextFrame("TPOS", "2/2", Encoding.UTF8));
            container.Add(new TextFrame("TCON", "Pop", Encoding.UTF8));

            sut = new ZuneMP3TagContainer(container);
        };

        It should_be_able_to_read_the_album_artist = () =>
            sut.ReadMetaData().AlbumArtist.ShouldEqual("Various Artists");

        It should_be_able_to_read_the_first_contributing_artists = () =>
            sut.ReadMetaData().ContributingArtists.First()
            .ShouldEqual(SampleData.SomeArtist);

        It should_be_able_to_read_the_album_title = () =>
            sut.ReadMetaData().AlbumName
            .ShouldEqual(SampleData.SomeAlbum);

        It should_be_able_to_read_the_track_title = () =>
            sut.ReadMetaData().Title
            .ShouldEqual(SampleData.SomeTitle);

        It should_be_able_to_read_the_release_year = () =>
            sut.ReadMetaData().Year
            .ShouldEqual(SampleData.SomeYear);

        It should_be_able_to_read_the_track_number = () =>
            sut.ReadMetaData().TrackNumber
            .ShouldEqual("2");

        It should_be_able_to_read_the_disc_number = () =>
            sut.ReadMetaData().DiscNumber
            .ShouldEqual("2/2");

        It should_be_able_to_read_the_genre = () =>
            sut.ReadMetaData().Genre
            .ShouldEqual("Pop");

        static ZuneMP3TagContainer sut;
    }


    public class when_writing_meta_data_back_to_file_with_pre_existing_meta_data
    {
        Because of = () =>
        {
            var container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new TextFrame("TALB", SampleData.SomeAlbum, Encoding.UTF8));
            container.Add(new TextFrame("TPE1", SampleData.SomeArtist, Encoding.UTF8));
            container.Add(new TextFrame("TPE2", "Various Artists", Encoding.UTF8));
            container.Add(new TextFrame("TIT2", SampleData.SomeTitle, Encoding.UTF8));
            container.Add(new TextFrame("TYER", SampleData.SomeYear, Encoding.UTF8));
            container.Add(new TextFrame("TRCK", "2", Encoding.UTF8));
            container.Add(new TextFrame("TPOS", "2/2", Encoding.UTF8));
            container.Add(new TextFrame("TCON", "Pop", Encoding.UTF8));

            sut = new ZuneMP3TagContainer(container);

            metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumName = "Forever",
                ContributingArtists = new List<string> { "U2", "AFI" },
                DiscNumber = "1/1",
                Genre = "Pop",
                Title = "Wallet",
                TrackNumber = "2",
                Year = "2009"
            };

            sut.AddMetaData(metaData);
        };

        It should_be_able_to_read_the_album_artist = () =>
            sut.ReadMetaData().AlbumArtist.ShouldEqual(metaData.AlbumArtist);

        It should_be_able_to_read_the_first_contributing_artists = () =>
            sut.ReadMetaData().ContributingArtists
            .ShouldEqual(metaData.ContributingArtists);

        It should_be_able_to_read_the_album_title = () =>
            sut.ReadMetaData().AlbumName
            .ShouldEqual(metaData.AlbumName);

        It should_be_able_to_read_the_track_title = () =>
            sut.ReadMetaData().Title
            .ShouldEqual(metaData.Title);

        It should_be_able_to_read_the_release_year = () =>
            sut.ReadMetaData().Year
            .ShouldEqual(metaData.Year);

        It should_be_able_to_read_the_track_number = () =>
            sut.ReadMetaData().TrackNumber.ShouldEqual(metaData.TrackNumber);

        It should_be_able_to_read_the_disc_number = () =>
            sut.ReadMetaData().DiscNumber.ShouldEqual(metaData.DiscNumber);

        It should_be_able_to_read_the_genre = () =>
            sut.ReadMetaData().Genre.ShouldEqual(metaData.Genre);

        static MetaData metaData;
        static ZuneMP3TagContainer sut;
    }
}