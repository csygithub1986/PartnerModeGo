using System;
using System.Globalization;
using System.Windows.Data;

namespace PartnerModeGo
{
    public class Convertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ("ParamName".Equals(parameter))
            {
                return (value as System.Reflection.ParameterInfo).ParameterType.Name;
            }
            else if ("ParamReadOnly".Equals(parameter))
            {
                //如果是ref参数，则禁用textbox
                return (value as System.Reflection.ParameterInfo).IsRetval;
            }
            else if ("ParamEnable".Equals(parameter))
            {
                System.Reflection.ParameterInfo param = value as System.Reflection.ParameterInfo;
                return param.ParameterType == typeof(int) || param.ParameterType == typeof(float) || param.ParameterType == typeof(bool) || param.ParameterType == typeof(string);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            bool result;
            bool isBool = bool.TryParse(value.ToString(), out result);
            if (isBool)
            {
                return result;
            }
            return value;
        }
    }
}
