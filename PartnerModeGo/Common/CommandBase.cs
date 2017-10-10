using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PartnerModeGo
{
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public Func<object, bool> CanExecuteAction;
        public Action<object> ExecuteAction;

        public bool CanExecute(object parameter)
        {
            if (CanExecuteAction != null)
            {
                return CanExecuteAction.Invoke(parameter);
            }
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }
    }
}
