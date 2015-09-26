using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdLineParser
{
    public enum CmdLineParamType
    {
        KEY,
        PARAMETR
    }

    public class CmdLineParam
    {
        public readonly CmdLineParamType ParamType;
        public readonly string KeyName = null;
        public readonly string Value = null;

        public CmdLineParam(CmdLineParamType paramType, string keyName, string keyValue = null)
        {
            ParamType = paramType;
            KeyName = keyName;
            Value = keyValue;
        }

        public bool IsKey
        {
            get
            {
                return (ParamType == CmdLineParamType.KEY);
            }
        }
    }
}
