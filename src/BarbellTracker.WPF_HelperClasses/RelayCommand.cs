using System;
using System.Windows.Input;

namespace BarbellTracker.WPF_HelperClasses
{

    public class RelayCommand<T> : RelayCommand where T : class
    {
        private readonly Func<T, bool> m_canExecute;
        private readonly Action<T> m_execute;

        public RelayCommand(Func<T, bool> canExecute, Action<T> execute)
        {
            m_canExecute = canExecute;
            m_execute = execute;
        }

        public override bool CanExecute(object parameter)
        {
            return m_canExecute((T)parameter);
        }

        public override void Execute(object parameter)
        {
            m_execute((T)parameter);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public RelayCommand() { }

        public RelayCommand(Func<bool> execute) : this(execute, null) { }

        public RelayCommand(Func<bool> canExecute, Action execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _canExecute = canExecute;
            _execute = execute;
        }

        #region ICommand Members

        public virtual bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public virtual void Execute(object parameter)
        {
            _execute();
        }

        #endregion
    }
}
