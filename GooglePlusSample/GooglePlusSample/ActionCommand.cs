using System;
using System.Windows.Input;

namespace GooglePlusSample
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;

        public ActionCommand(Action action)
            : this(action, null)
        {

        }

        public ActionCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;
    }

    public class ActionCommand<T> : ICommand
    {
        private readonly Action<T> _action;
        private readonly Func<bool> _canExecute;

        public ActionCommand(Action<T> action)
            : this(action, null)
        {

        }

        public ActionCommand(Action<T> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            return true;
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}