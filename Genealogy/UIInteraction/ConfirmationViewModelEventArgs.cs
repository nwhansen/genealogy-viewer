using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Genealogy.UIInteraction.ConfirmationViewModelEventArgs;

namespace Genealogy.UIInteraction {
	/// <summary>
	/// A shorthand view mode
	/// </summary>
	public class ConfirmationViewModelEventArgs : PresentViewModelEventArgs<ConfirmationViewModel> {
		/// <summary>
		/// The Type of Confirmations to present
		/// </summary>
		public enum ConfirmationType {
			/// <summary>
			/// A Yes No Choice
			/// </summary>
			YesNo,
			/// <summary>
			/// Ok/Cancel Choice
			/// </summary>
			OkCancel,
			/// <summary>
			/// OK Only
			/// </summary>
			Ok
		}
		/// <summary>
		/// The Confirmation View Model
		/// </summary>
		public class ConfirmationViewModel {
			/// <summary>
			/// Creates the confirmation view model
			/// </summary>
			/// <param name="message">The Message to Confirm</param>
			/// <param name="caption">The Caption of the message</param>
			/// <param name="confirmation">The Confirmation Type (dictating what result we will receive)</param>
			public ConfirmationViewModel(string message, string caption, ConfirmationType confirmation) {
				Message = message;
				Caption = caption;
				Confirmation = confirmation;
			}

			/// <summary>
			/// The Message to Confirm
			/// </summary>
			public string Message { get; private set; }
			/// <summary>
			/// The Caption of the message
			/// </summary>
			public string Caption { get; private set; }

			/// <summary>
			/// The Confirmation Type (dictating what result we will receive)
			/// </summary>
			public ConfirmationType Confirmation { get; private set; }

		}


		public ConfirmationViewModelEventArgs(string message, string caption, ConfirmationType confirmation) : base(new ConfirmationViewModel(message, caption, confirmation), true) {
		}

	}
}
