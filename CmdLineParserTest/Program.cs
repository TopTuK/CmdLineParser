using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdLineParser;

namespace CmdLineParser
{
    class Program
    {
        public sealed class TestOptionClass
        {
            [CmdLineOption("key", true, "Required Key Param")]
            public bool KeyParam { get; set; }

            [CmdLineOption("keyNotRequired", false, "Not required Key should be false")]
            public bool NotRequiredKeyParam { get; set; }

            [CmdLineOption("s", true, "Required String Param")]
            public string StringParam { get; set; }

            [CmdLineOption("i", true, "Required Int Param")]
            public int IntParam { get; set; }

            [CmdLineOption("d", false, "Not Required Double Param")]
            public double DoubleParam { get; set; }
        }

        static void Main(string[] args)
        {
            args = new string[]
            {
                @"/key",
                @"/s",
                @"StringValue",
                @"/i",
                @"1488",
                @"/d",
                @"14,88"
            };

            CmdLineParser parser = new CmdLineParser();
            try
            {
                Console.WriteLine("Params:");
                Console.WriteLine("Params usage:\n\b{0}", parser.GetUsageText<TestOptionClass>());
                Console.WriteLine();

                parser.ParseCmdLine(args);
                TestOptionClass options = parser.GetCmdLineOptions<TestOptionClass>();
                Console.WriteLine("Test Class Object:");
                Console.WriteLine("Required Key param: {0}", options.KeyParam);
                Console.WriteLine("Not Required Key param: {0}", options.NotRequiredKeyParam);
                Console.WriteLine("Required String param: {0}", options.StringParam);
                Console.WriteLine("Required Int param: {0}", options.IntParam);
                Console.WriteLine("Not Required Double param: {0}", options.DoubleParam);
            }
            catch(ParseCmdLineException parseEx)
            {
                switch(parseEx.ParseError)
                {
                    case EParseError.E_KEYEXISTS:
                        break;
                    case EParseError.E_KEYNOTFOUND:
                        break;
                }
                Console.WriteLine("Parse cmd line exception!");
            }
            catch(OptionParserException optionEx)
            {
                switch(optionEx.ErrorCode)
                {
                    case EOptionParserError.E_INVALID_CONVERT_TYPE:
                        break;
                    case EOptionParserError.E_INVALID_KEY_PROPERTY_TYPE:
                        break;
                    case EOptionParserError.E_REQUIRED_OPTION_NOT_EXIST:
                        break;
                }
                Console.WriteLine("Option class creation exception!");
            }
        }
    }
}
