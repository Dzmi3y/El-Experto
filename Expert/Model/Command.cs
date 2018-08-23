using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Expert
{
    class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action Deleg;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Deleg?.Invoke();
        }
    }
}
