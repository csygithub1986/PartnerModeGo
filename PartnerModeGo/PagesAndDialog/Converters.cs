using PartnerModeGo.WcfService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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
            if (parameter.ToString() == "BlackPlayerVisibility")
            {
                Collection<Player> players = value as Collection<Player>;
                return players.Where(p => p.Color == 2);
            }
            if (parameter.ToString() == "WhitePlayerVisibility")
            {
                Collection<Player> players = value as Collection<Player>;
                return players.Where(p => p.Color == 1);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PlayerType type = (PlayerType)value;
            switch (type)
            {
                case PlayerType.AI:
                    return "人工智能";
                case PlayerType.RealBoard:
                    return "真实棋盘";
                case PlayerType.Host:
                    return "本机";
                case PlayerType.Internet:
                    return "远程玩家";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = (string)value;
            switch (type)
            {
                case "人工智能":
                    return PlayerType.AI;
                case "真实棋盘":
                    return PlayerType.RealBoard;
                case "本机":
                    return PlayerType.Host;
                case "远程玩家":
                    return PlayerType.Internet;
            }
            return value;
        }
    }
}
