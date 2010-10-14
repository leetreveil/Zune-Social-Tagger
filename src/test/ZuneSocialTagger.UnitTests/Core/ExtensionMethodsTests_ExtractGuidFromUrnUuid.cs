using System;
using Machine.Specifications;

namespace ZuneSocialTagger.UnitTests.Core
{
    public class when_converting_a_valid_uuid_to_a_guid
    {
        It should_return_a_valid_guid = () =>
           ZuneSocialTagger.Core.ExtensionMethods
           .ExtractGuidFromUrnUuid("urn:uuid:c14c4e00-0300-11db-89ca-0019b92a3933")
           .ShouldEqual(new Guid("c14c4e00-0300-11db-89ca-0019b92a3933"));
    }

    public class when_converting_an_invalid_uuid_to_a_guid
    {
        It should_return_an_empty_guid= () =>
           ZuneSocialTagger.Core.ExtensionMethods
           .ExtractGuidFromUrnUuid("c14c4e00-0300-11db-89ca-0019b92a3933")
           .ShouldEqual(Guid.Empty);
    }

    public class when_converting_an_random_string_t_a_guid
    {
        It should_return_an_empty_guid = () =>
           ZuneSocialTagger.Core.ExtensionMethods
           .ExtractGuidFromUrnUuid("SOME TEXT:SOMETEXT")
           .ShouldEqual(Guid.Empty);
    }
}