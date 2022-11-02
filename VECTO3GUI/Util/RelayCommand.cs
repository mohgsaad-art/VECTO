using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace VECTO3GUI.Util
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
	/// </summary>
	public class RelayCommand<T> : ICommand
	{
		#region Declarations

		readonly Predicate<T> _canExecute;
		readonly Action<T> _execute;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
		{
			if (execute == null) {
				throw new ArgumentNullException(nameof(execute));
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion

		#region ICommand Members

		public event EventHandler CanExecuteChanged
		{
			add {
				if (_canExecute != null) {
					CommandManager.RequerySuggested += value;
				}
			}
			remove {
				if (_canExecute != null) {
					CommandManager.RequerySuggested -= value;
				}
			}
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}


		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			if (_canExecute == null) {
				return true;
			}

			try {
				return _canExecute((T)parameter);
			} catch (Exception) {
				return false;
			}
		}

		public void Execute(object parameter)
		{
			try {
				_execute((T)parameter);
			} catch (Exception e) {
				MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#endregion
	}

	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		#region Declarations

		readonly Func<bool> _canExecute;
		readonly Action _execute;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			if (execute == null) {
				throw new ArgumentNullException(nameof(execute));
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion

		#region ICommand Members

		public event EventHandler CanExecuteChanged
		{
			add {
				if (_canExecute != null) {
					CommandManager.RequerySuggested += value;
				}
			}
			remove {
				if (_canExecute != null) {
					CommandManager.RequerySuggested -= value;
				}
			}
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			if (_canExecute == null) {
				return true;
			}

			try {
				return _canExecute();
			} catch (Exception) {
				return false;
			}
		}

		public void Execute(object parameter)
		{
			_execute();
		}

		#endregion
	}
}
