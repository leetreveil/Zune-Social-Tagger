using System;
using ZuneSocialTagger.GUI.Models;
using Machine.Specifications;

namespace ZuneSocialTagger.UnitTests.UI.Models
{
    [Subject("DiscNumberConverter")]
    public class when_converting_a_disc_number_separated_by_a_forward_slash
    {
        It should_return_the_first_character_from_the_input = () =>
            SharedMethods.DiscNumberConverter("1/11").ShouldEqual("1");
    }

    [Subject("DiscNumberConverter")]
    public class when_converting_a_single_digit_disc_number_with_no_seperator
    {
        It should_return_the_input = () =>
            SharedMethods.DiscNumberConverter("1").ShouldEqual("1");
    }

    [Subject("DiscNumberConverter")]
    public class when_the_input_is_an_empty_string
    {
        It should_return_zero = () =>
            SharedMethods.DiscNumberConverter(String.Empty).ShouldEqual("1");
    }

    [Subject("DiscNumberConverter")]
    public class when_the_input_is_null
    {
        It should_return_zero = () =>
            SharedMethods.DiscNumberConverter(null).ShouldEqual("1");
    }
}
