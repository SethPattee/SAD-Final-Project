using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFTesting
{
    class RelayCommand : ICommand
    {
        private readonly Predicate<object>? _command;
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> action, Predicate<object>? canExecute = null)
        {
            _execute = action ?? throw new ArgumentNullException(nameof(action));
            _command = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value;}
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null) throw new ArgumentNullException(paramName: nameof(parameter));

            return _command == null ? true : _command(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
