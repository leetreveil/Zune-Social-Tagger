using Machine.Specifications;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.UnitTests.UI.Models
{
    [Subject("ConvertTrackNumberToDoubleDigits")]
    public class when_converting_a_track_number_with_a_single_digit
    {
        It should_return_the_input_with_a_zero_before_it = () =>
            SharedMethods.ConvertTrackNumberToDoubleDigits("1").ShouldEqual("01");
    }

    [Subject("ConvertTrackNumberToDoubleDigits")]
    public class when_converting_a_track_number_with_double_digits
    {
        It should_return_the_input = () =>
            SharedMethods.ConvertTrackNumberToDoubleDigits("12").ShouldEqual("12");
    }

    [Subject("ConvertTrackNumberToDoubleDigits")]
    public class when_converting_a_track_number_with_triple_digits
    {
        It should_return_the_input = () =>
            SharedMethods.ConvertTrackNumberToDoubleDigits("121").ShouldEqual("121");
    }

    [Subject("ConvertTrackNumberToDoubleDigits")]
    public class when_converting_a_text_string
    {
        It should_throw_an_argument_exception = () =>
            SharedMethods.ConvertTrackNumberToDoubleDigits("HELLO")
            .ShouldBeEmpty();
    }

    [Subject("ConvertTrackNumberToDoubleDigits")]
    public class when_converting_a_null_value
    {
        It should_return_an_empty_string = () =>
            SharedMethods.ConvertTrackNumberToDoubleDigits(null)
            .ShouldBeEmpty();
    }

    [Subject("ConvertTrackNumberToDoubleDigits")]
    public class when_converting_an_empty_string
    {
        It should_return_a_empty_string = () =>
            SharedMethods.ConvertTrackNumberToDoubleDigits("")
            .ShouldBeEmpty();
    }
}