using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CmdLineParser
{
    public enum EParseError
    {
        E_KEYNOTFOUND,
        E_KEYEXISTS,
        E_UNKNOWN
    }

    public class ParseCmdLineException : ApplicationException
    {
        public readonly EParseError ParseError;

        public ParseCmdLineException(EParseError parseError)
            : base()
        {
            ParseError = parseError;
        }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }

    public class BaseCmdLineParser
    {
        private IDictionary<string, CmdLineParam> m_params = null;

        public BaseCmdLineParser()
        {
            m_params = new Dictionary<string, CmdLineParam>();
        }

        public virtual void ParseCmdLine(string[] args)
        {
            Regex keyDetector = new Regex(@"^-{1,2}\w|^/\w",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex valueEnclosingRemover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string param = null;
            string[] argParts = null;

            foreach (string arg in args)
            {
                if (keyDetector.IsMatch(arg))
                {
                    // Found Key
                    if (param != null) AddParam(CmdLineParamType.KEY, param); // Save previos key

                    Regex valueDetector = new Regex(@":|=",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled);

                    argParts = Regex.Split(arg, @"^-{1,2}|^/",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled); // Delete key prefix

                    argParts = valueDetector.Split(argParts[1], 2);
                    param = argParts[0];

                    if(argParts.Length == 2) // Found key with value [Key{:|=}Value]
                    {
                        if (!m_params.ContainsKey(param))
                            AddParam(CmdLineParamType.PARAMETR, param, valueEnclosingRemover.Replace(argParts[1], "$1"));
                    }
                }
                else
                {
                    // found value
                    if (param != null)
                    {
                        if (!m_params.ContainsKey(param))
                            AddParam(CmdLineParamType.PARAMETR, param, valueEnclosingRemover.Replace(arg, "$1"));

                        param = null;
                    }
                    else
                        ProccessError(EParseError.E_KEYNOTFOUND); // Error: no parameter waiting for a value (skipped)
                }
            }

            // In case a parameter is still waiting
            if (param != null) AddParam(CmdLineParamType.KEY, param);
        }

        private void AddParam(CmdLineParamType paramType, string keyName, string value = null)
        {
            if (!m_params.ContainsKey(keyName))
                m_params.Add(keyName, new CmdLineParam(paramType, keyName, value));
            else 
                ProccessError(EParseError.E_KEYEXISTS); // Key is already exists
        }

        protected virtual void ProccessError(EParseError errorType)
        {
            m_params.Clear();
            throw new ParseCmdLineException(errorType);
        }

        // Retrieve a parameter value if it exists 
        public CmdLineParam this[string paramName]
        {
            get
            {
                if (m_params.ContainsKey(paramName)) return m_params[paramName];
                else return null;
            }
        }

        public ICollection<CmdLineParam> GetParams()
        {
            return m_params.Values;
        }
    }
}
