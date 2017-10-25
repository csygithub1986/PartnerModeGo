using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PartnerModeGo
{
    public class PlayerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strParam = parameter.ToString();
            if (strParam == "BlackPlayer")
            {
                return (value as WcfService.Player[]).Where(p => p.Color == 2).ToArray();
            }
            if (strParam == "WhitePlayer")
            {
                return (value as WcfService.Player[]).Where(p => p.Color == 1).ToArray();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
