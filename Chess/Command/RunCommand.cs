using System.Windows.Input;
namespace Chess
{
    public class RunCommand(Action<object> execute, Func<object, bool> canExecute = null) : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter!);
        public void Execute(object parameter) => execute(parameter!);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
    public class RunCommand<T>(Action<T> execute, Func<T, bool> canExecute = null) : ICommand
    {
        public bool CanExecute(object parameter) => canExecute == null || canExecute((T)parameter);
        public void Execute(object parameter) => execute((T)parameter);
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
