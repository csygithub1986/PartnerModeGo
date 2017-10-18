using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PartnerModeGo.Game
{
    public class SettingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "AIDetail")
            {
                var type = (PlayerType)value;
                if (type == PlayerType.AI)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            if (parameter.ToString() == "RealBoardDetail")
            {
                var type = (PlayerType)value;
                if (type == PlayerType.RealBoard)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            if (parameter.ToString() == "Color")
            {
                int color = (int)value;
                if (color == 2)
                {
                    return Brushes.Black;
                }
                else
                {
                    return Brushes.White;
                }
            }
            if (parameter.ToString() == "IsConnected" || parameter.ToString() == "IsBoardRecognized")
            {
                return (bool)value ? Brushes.Green : Brushes.Red;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
