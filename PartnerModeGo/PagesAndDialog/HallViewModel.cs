using PartnerModeGo.WcfService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public class HallViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 属性
        public Game SelectedGame
        {
            get { return _SelectedGame; }
            set { _SelectedGame = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGame")); }
        }
        private Game _SelectedGame;
        #endregion

        public HallViewModel()
        {
            ServiceProxy.Instance.GetAllGames();
        }
    }
}
