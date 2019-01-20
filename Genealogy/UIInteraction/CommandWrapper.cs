using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Genealogy.UIInteraction {
	/// <summary>
	/// Provides a command wrapper to make ICommands much simpler to use (as well as enabled "smart" buttons)
	/// </summary>
	public class CommandWrapper : ICommand {
		private readonly Func<bool> canExecute;
		private readonly Action execute;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="label">The label of the action</param>
		/// <param name="canExecute">The function controls if the function</param>
		/// <param name="execute">The action to execute</param>
		/// <param name="canExecuteChanged">The function to call to update if the action has become non operational</param>
		public CommandWrapper(Func<bool> canExecute, Action execute, out Action canExecuteChanged) {
			canExecuteChanged = ExecuteCanExecuteChanged;
			this.canExecute = canExecute;
			this.execute = execute;
		}

		/// <summary>
		/// An always executable action
		/// </summary>
		/// <param name="label">The label of the action</param>
		/// <param name="execute">The action to execute</param>
		public CommandWrapper(Action execute) {
			this.execute = execute;
			//We can always executet
			canExecute = AlwaysTrue;
		}

		public event EventHandler CanExecuteChanged;


		public bool CanExecute(object parameter) {
			return canExecute != null ? canExecute() : true;
		}

		public void Execute(object parameter) {
			execute();
		}


		private void ExecuteCanExecuteChanged() {
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		private static bool AlwaysTrue() {
			return true;
		}
	}
}
