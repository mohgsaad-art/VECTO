using System;
using System.Windows.Input;

namespace VECTO3GUI2020.Util
{
    public class RelayCommand<T> : ICommand
    {

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;


        public event EventHandler CanExecuteChanged {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;     
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute != null ?_canExecute((T)parameter) : true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }





    public class RelayCommand : ICommand
    {


        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;


        }

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

        public bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute() : true ;
        }

        public void Execute(object parameter)
        {
            _execute();
        }

    }
}
