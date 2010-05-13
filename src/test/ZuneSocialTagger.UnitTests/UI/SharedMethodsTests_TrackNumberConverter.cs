using System;
using Machine.Specifications;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.UnitTests.UI
{
    [Subject("TrackNumberConverter")]
    public class when_converting_a_track_number_separated_by_a_forward_slash
    {
        It should_return_the_first_character_from_the_input = () =>
            SharedMethods.TrackNumberConverter("1/11").ShouldEqual("1");
    }

    [Subject("TrackNumberConverter")]
    public class when_converting_a_single_digit_track_number_with_no_seperator
    {
        It should_return_the_input = () =>
            SharedMethods.TrackNumberConverter("1").ShouldEqual("1");
    }

    [Subject("TrackNumberConverter")]
    public class when_converting_a_empty_string
    {
        It should_return_zero = () =>
            SharedMethods.TrackNumberConverter(String.Empty).ShouldEqual("0");
    }

    [Subject("TrackNumberConverter")]
    public class when_converting_a_null_input
    {
        It should_return_zero = () =>
            SharedMethods.TrackNumberConverter(null).ShouldEqual("0");
    }
}
