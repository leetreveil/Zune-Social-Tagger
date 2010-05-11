using System;
using Machine.Specifications;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.UnitTests.UI
{
    public class when_getting_the_link_status_for_a_valid_random_guid
    {
        It should_return_linked = () =>
             Guid.NewGuid().GetLinkStatusFromGuid();
    }

    public class when_getting_the_link_status_for_a_empty_guid
    {
        It should_return_unlinked = () =>
            Guid.Empty.GetLinkStatusFromGuid();
    }
}