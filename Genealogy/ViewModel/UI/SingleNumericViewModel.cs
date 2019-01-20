using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Genealogy.ViewModel.UI {
	/// <summary>
	/// A View model for representing a single numeric value
	/// </summary>
	public class SingleNumericViewModel : INotifyPropertyChanged {

		private int intValue;

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The minimum value the user can enter
		/// </summary>
		public int Min { get; set; } = int.MinValue;
		/// <summary>
		/// The maximum number the user can enter
		/// </summary>
		public int Max { get; set; } = int.MaxValue;

		/// <summary>
		/// The Value that the user has entered
		/// </summary>
		public int Value {
			get { return intValue; }
			set {
				if (value != intValue && value >= Min && value <= Max) {
					intValue = value;
					PropertyChanged?.Notify(this, "Value");
				}
			}
		}

		/// <summary>
		/// An operator to allow for the entered value to be directly converted to an integer
		/// </summary>
		/// <param name="vm">The View Model to convert</param>
		public static implicit operator int(SingleNumericViewModel vm) {
			return vm.intValue;
		}
	}
}
