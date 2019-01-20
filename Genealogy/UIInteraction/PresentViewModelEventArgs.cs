using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genealogy.ViewModel.Configuration;

namespace Genealogy.UIInteraction {
	/// <summary>
	/// Present a view model to the view for consumption. Used when the ViewModel needs the user to populate something
	/// </summary>
	/// <typeparam name="T">The ViewModel to present</typeparam>
	public class PresentViewModelEventArgs<T> : PresentEventArgs where T : class {

		/// <summary>
		/// The View Model that is being presented
		/// </summary>
		public T ViewModel { get; private set; }


		/// <summary>
		/// Creates the Present View Model event arguments with the given view and state
		/// </summary>
		/// <param name="viewModel">The ViewModel to present</param>
		/// <param name="blocking">If this is blocking (in case of highly generic logic)</param>
		public PresentViewModelEventArgs(T viewModel, bool blocking) : base(blocking) {
			ViewModel = viewModel;
		}
	}

	/// <summary>
	/// An event arg that is used to present something to the view
	/// </summary>
	public class PresentEventArgs : EventArgs {

		/// <summary>
		/// If the view model being presented must be "accepted/rejected" before the view can continue
		/// </summary>
		public bool Blocking { get; private set; }

		/// <summary>
		/// What state of the view model presentation is in
		/// </summary>
		public ViewModelInteractionState InteractionState { get; set; } = ViewModelInteractionState.Waiting;
		/// <summary>
		/// Creates
		/// </summary>
		/// <param name="blocking"></param>
		protected PresentEventArgs(bool blocking) {
			Blocking = blocking;
		}

		/// <summary>
		/// Allows the arguments to be used in if statements
		/// </summary>
		/// <param name="args">The argument to test if accepted</param>
		public static implicit operator bool(PresentEventArgs args) {
			return args.InteractionState == ViewModelInteractionState.Accepted;
		}
	}

}
