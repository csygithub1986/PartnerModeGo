using PartnerModeGo.WcfService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public class GlobalData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //ObservableCollection<  Game> GameList


        public ObservableCollection<Game> GameList
        {
            get { return _GameList; }
            set { _GameList = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GameList")); }
        }
        private ObservableCollection<Game> _GameList;

    }
}
