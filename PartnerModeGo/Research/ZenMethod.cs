using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public class ZenMethod : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _MethodName;

        public string MethodName
        {
            get { return _MethodName; }
            set
            {
                _MethodName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("MethodName"));
            }
        }

    }
}
