using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genealogy.Model;

namespace Genealogy.ViewModel {
	/// <summary>
	/// Provides some extensions that make working with enumerations simpler (in the context of IndividualManagerViewModel)
	/// </summary>
	public static class IndividualManagerViewModelExtensions {

		/// <summary>
		/// Converts an enumeration of individuals to their "canonical" IndividualViewModel 
		/// </summary>
		/// <param name="enumeration">The enumeration to convert</param>
		/// <param name="manager">The Manager to use to get the individual view models from</param>
		/// <returns>The Individuals from the manager, non-existent individuals are filtered</returns>
		public static IEnumerable<IndividualViewModel> ToViewModel(this IEnumerable<Individual> enumeration, IndividualManagerViewModel manager) => enumeration.Select(i => manager[i]).Where(i => i != null);

		/// <summary>
		/// Converts an enumeration of attributes to their "canonical" IndividualAttributeViewModels
		/// </summary>
		/// <param name="enumeration">The enumeration to convert</param>
		/// <param name="factoryViewModel">The Attribute Factory View Model to retrieve from</param>
		/// <returns>The attributes from the factory, non-existent individuals are filtered</returns>
		public static IEnumerable<IndividualAttributeViewModel> ToViewModel(this IEnumerable<IndividualAttribute> enumeration, IndividualAttributesFactoryViewModel factoryViewModel) => enumeration.Select(i => factoryViewModel[i]).Where(i => i != null);

	}
}
