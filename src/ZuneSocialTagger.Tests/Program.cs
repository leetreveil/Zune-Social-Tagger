using System;
using System.Collections.Generic;
using System.Text;
using ZuneSocialTagger.Tests.Unit;

namespace ZuneSocialTagger.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new ZuneXtraParserTests();
            //tests.Should_not_crash_if_part_cannot_be_parsed();
            tests.Should_be_able_to_create_the_raw_data_from_the_parsed_object();
        }
    }
}
