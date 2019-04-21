using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genealogy.ViewModel;
using Genealogy.ViewModel.Configuration;
using Microsoft.Msagl.Drawing;

namespace Genealogy.UI.Logic {

	/// <summary>
	/// A service that assigns a color to an individual
	/// </summary>
	public class ColorAssigner {


		private readonly HighlightConfigurationViewModel highlightConfiguration;
		private readonly IndividualManagerViewModel individualManager;

		public ColorAssigner(HighlightConfigurationViewModel highlightConfiguration, IndividualManagerViewModel individualManager) {
			this.highlightConfiguration = highlightConfiguration;
			this.individualManager = individualManager;
		}

		/// <summary>
		/// Returns the fill color for a given individual
		/// </summary>
		/// <param name="individual"></param>
		/// <returns></returns>
		public Color GetFill(IndividualViewModel individual, IndividualViewModel selectedIndividual) {
			//Direct Children are always highlighted
			if (selectedIndividual != null && selectedIndividual.Children.Where(i => i.Wrapped == individual.Wrapped).Any()) {
				return highlightConfiguration.DirectChildrenColor;
			}
			//If we are interesting in some fashion generate a color, otherwise default to standard colors
			var color = IsInterestingColor(individual, selectedIndividual);
			if (color.HasValue) {
				return color.Value;
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
		private Color? IsInterestingColor(IndividualViewModel individual, IndividualViewModel selectedIndividual) {
			if (individual == selectedIndividual) {
				return highlightConfiguration.InterestedIndividualColor;
			}
			bool isFounder = individual.IsFounder;
			bool hasAttr = highlightConfiguration.ShouldHighlightIndividual(individual);
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
			if (individual.Children.Where(highlightConfiguration.ShouldHighlightIndividual).Any()) {
				return highlightConfiguration.ParentIndividualHightlightColor;
			}
			//Not considered "interesting"
			return null;
		}


	}
}
