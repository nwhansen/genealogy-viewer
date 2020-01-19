//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Linq;

using Genealogy.ViewModel;
using Genealogy.ViewModel.Configuration;

namespace Genealogy.UI.Logic {

	/// <summary>
	/// A service that assigns a color to an individual
	/// </summary>
	public class ColorAssigner {


		private readonly ColorConfigurationViewModel highlightConfiguration;
		private readonly HighlightAttributeConfigurationViewModel highlightAttributeConfiguration;
		private readonly IndividualManagerViewModel individualManager;

		public ColorAssigner(ColorConfigurationViewModel highlightConfiguration, HighlightAttributeConfigurationViewModel highlightAttributeConfiguration, IndividualManagerViewModel individualManager) {
			this.highlightConfiguration = highlightConfiguration;
			this.highlightAttributeConfiguration = highlightAttributeConfiguration;
			this.individualManager = individualManager;
		}

		/// <summary>
		/// Returns the fill color for a given individual
		/// </summary>
		/// <param name="individual"></param>
		/// <returns></returns>
		public ColorsViewModel GetFill(IndividualViewModel individual, IndividualViewModel selectedIndividual) {
			//Direct Children are always highlighted
			if (selectedIndividual != null && selectedIndividual.Children.Where(i => i.Wrapped == individual.Wrapped).Any()) {
				return highlightConfiguration.DirectChildrenColor;
			}
			//If we are interesting in some fashion generate a color, otherwise default to standard colors
			var color = IsInterestingColor(individual, selectedIndividual);
			if (color != null) {
				return color;
			}
			if (individual.IsFounder) {
				return highlightConfiguration.FounderColor;
			}
			return highlightConfiguration.IndividualColor;

		}

		/// <summary>
		/// Are we in the "interesting" case aka, attribute or parent.
		/// </summary>
		/// <param name="individual">The individual to highlight</param>
		/// <param name="selectedIndividual">The selected individual</param>
		/// <returns>If we are interesting</returns>
		private ColorsViewModel IsInterestingColor(IndividualViewModel individual, IndividualViewModel selectedIndividual) {
			if (individual == selectedIndividual) {
				return highlightConfiguration.InterestedIndividualColor;
			}
			bool isFounder = individual.IsFounder;
			bool hasAttr = highlightAttributeConfiguration.ShouldHighlightIndividual(individual);
			//Check if we are the parent of the individual
			bool isParent = false;
			if (selectedIndividual != null) {
				isParent = selectedIndividual.Parents.Where(i => i.Wrapped == individual.Wrapped).Any();
			}
			//We may need a better highlight than this
			if (isFounder && (hasAttr || isParent)) {
				return highlightConfiguration.FounderHighlightColor;
			}
			//Parent is more interesting than nothing attribute if we are not a founder
			if (isParent) {
				return highlightConfiguration.SelectedParentColor;
			}
			if (hasAttr) {
				return highlightConfiguration.IndividualHighlightColor;
			}
			//Are we a parent of "interest"
			if (individual.Children.Where(highlightAttributeConfiguration.ShouldHighlightIndividual).Any()) {
				return highlightConfiguration.ParentIndividualHightlightColor;
			}
			//Not considered "interesting"
			return null;
		}


	}
}
