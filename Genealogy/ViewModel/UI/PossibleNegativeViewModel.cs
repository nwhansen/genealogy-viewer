//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
namespace Genealogy.ViewModel.UI {
	/// <summary>
	/// Represents the View Model logic required for when 
	/// </summary>
	public class PossibleNegativeViewModel {

		/// <summary>
		/// When do we "fall" off aka somebody is not related enough to somebody with an attribute to be considered a possible positive
		/// </summary>
		public SingleNumericViewModel FallOffValue { get; } = new SingleNumericViewModel();

	}
}
