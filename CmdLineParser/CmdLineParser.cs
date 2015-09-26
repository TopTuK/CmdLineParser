using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CmdLineParserLib
{
    public enum EOptionParserError
    {
        E_UNKNOWN,
        E_REQUIRED_OPTION_NOT_EXIST,
        E_INVALID_KEY_PROPERTY_TYPE,
        E_INVALID_CONVERT_TYPE
    }

    public class OptionParserException : ApplicationException
    {
        public readonly EOptionParserError ErrorCode;

        public OptionParserException(EOptionParserError errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }

    public class CmdLineParser : BaseCmdLineParser
    {
        private sealed class OptionDescription
        {
            public readonly string PropertyName = null;
            public readonly string OptionName = null;
            public readonly bool OptionRequired = false;
            public readonly string OptionHelpText = null;

            public OptionDescription(string propertyName, string optionName,
                bool optionRequired = false, string optionhelpText = null)
            {
                PropertyName = propertyName;
                OptionName = optionName;
                OptionRequired = optionRequired;
                OptionHelpText = optionhelpText;
            }
        }

        private List<OptionDescription> m_optionList;
        private Type m_optionClassType;

        public CmdLineParser() :
            base()
        {
            m_optionList = new List<OptionDescription>();
            m_optionClassType = null;
        }

        private void SetOptionClassType(Type optionClassType)
        {
            if (m_optionClassType != optionClassType)
            {
                m_optionList.Clear();
                m_optionClassType = optionClassType;
                ParseOptionClass();
            }
        }

        private void ParseOptionClass()
        {
            m_optionList.Clear();

            // Получаем описание аттрибутов
            Type cmdLineParamAttrType = typeof(CmdLineOptionAttribute);
            FieldInfo attrNameField = cmdLineParamAttrType.GetField("OptionName");
            FieldInfo attrRequiredField = cmdLineParamAttrType.GetField("OptionRequired");
            FieldInfo attrHelpTextField = cmdLineParamAttrType.GetField("OptionHelpText");

            // Получаем тип переданного класса, где свойства - это параметры командной строки
            PropertyInfo[] propOptionList = m_optionClassType.GetProperties();

            // Цикл по всем свойства класса, пытаемся получить атрибуты у каждого свойства
            foreach (var optionProperty in propOptionList)
            {
                // Получаем атрибуты текущего свойства
                object[] attrList = optionProperty.GetCustomAttributes(cmdLineParamAttrType, false);
                if ((attrList != null) && (attrList.Length == 1)) // Массив содержит не более одного элемента
                {
                    m_optionList.Add(new OptionDescription(
                        optionProperty.Name,
                        attrNameField.GetValue(attrList[0]) as string,
                        (bool) attrRequiredField.GetValue(attrList[0]),
                        attrHelpTextField.GetValue(attrList[0]) as string
                        ));
                }
            }
        }

        private void CheckCmdLineOptions()
        {
            // Check all required params exist
            bool reqParamsExist = ((from opt in m_optionList where opt.OptionRequired select opt.OptionName).Except(
                from param in this.GetParams() select param.KeyName).Count()) == 0;

            if (!reqParamsExist) throw new OptionParserException(EOptionParserError.E_REQUIRED_OPTION_NOT_EXIST);
        }

        public T GetCmdLineOptions<T>()
            where T: class, new()
        {
            SetOptionClassType(typeof(T));
            CheckCmdLineOptions();

            T resultObj = Activator.CreateInstance<T>();

            foreach (var option in m_optionList)
            {
                CmdLineParam clParam = this[option.OptionName];
                if (clParam != null)
                {
                    PropertyInfo pi = m_optionClassType.GetProperty(option.PropertyName);

                    // Если параметр - ключ, то тип свойства всегда должен быть равен bool.
                    // Это должно проверяться в CheckOptions
                    if (clParam.IsKey)
                    {
                        if (pi.PropertyType == typeof(Boolean)) pi.SetValue(resultObj, true);
                        else
                            throw new OptionParserException(EOptionParserError.E_INVALID_KEY_PROPERTY_TYPE);
                    }
                    else
                    {
                        if (pi.PropertyType == typeof(String)) pi.SetValue(resultObj, this[option.OptionName].Value);
                        else
                        {
                            // Если тип свойства != string, то необходимо осуществить преобразования
                            var typeConverter = TypeDescriptor.GetConverter(pi.PropertyType);
                            if (typeConverter != null && typeConverter.CanConvertFrom(typeof(string)))
                            {
                                try
                                {
                                    pi.SetValue(resultObj, typeConverter.ConvertFrom(this[option.OptionName].Value));
                                }
                                catch
                                {
                                    throw new OptionParserException(EOptionParserError.E_INVALID_CONVERT_TYPE);
                                }
                            }
                            else
                                throw new OptionParserException(EOptionParserError.E_INVALID_CONVERT_TYPE);
                        }
                    }
                } // else where is no param in cmd line, but it is not required. Set to default.
            }

            return resultObj;
        }

        public StringBuilder GetUsageText<T>()
            where T : class
        {
            SetOptionClassType(typeof(T));

            StringBuilder result = new StringBuilder();
            var optionList = from option in m_optionList
                                  where option.OptionRequired
                                  orderby option.OptionRequired descending
                                  select new { option.OptionName, option.OptionHelpText };

            foreach(var opt in optionList)
            {
                result.AppendLine(String.Format("{0}\t{1}", opt.OptionName, opt.OptionHelpText));
            }

            return result;
        }
    }
}
