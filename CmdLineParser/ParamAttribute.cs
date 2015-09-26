using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdLineParser
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class CmdLineOptionAttribute : Attribute
    {
        public CmdLineOptionAttribute(string optionName, bool optionRequired = false, string helpText = null)
        {
            OptionName = optionName;
            OptionRequired = optionRequired;
            OptionHelpText = helpText;
        }

        public readonly string OptionName;
        public readonly bool OptionRequired;
        public readonly string OptionHelpText;
    }
}
