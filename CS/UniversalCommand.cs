using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CS
{
    class UniversalCommand : ICommand
    {
        private Action<object> ExecuteObj;
        private Func<object, bool> CanExecuteObj;
        public UniversalCommand(Action<object> executeObj, Func<object, bool> canExecuteObj = null)
        {
            ExecuteObj=executeObj;
            CanExecuteObj=canExecuteObj;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? Parameter)
        {
            return CanExecuteObj == null || CanExecuteObj(Parameter);
        }

        public void Execute(object? Parameter)
        {
            ExecuteObj(Parameter);
        }
    }
}
