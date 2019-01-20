using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.Drawing;
using System.Text;
using Genealogy.Model;

namespace Genealogy.Configuration {
	/// <summary>
	/// How Highlighting of Individuals is handled
	/// </summary>
	public class HighlightConfiguration {

		private readonly ISet<IndividualAttribute> toHighlight = new HashSet<IndividualAttribute>();


		#region Properties

		#region Colors
		/// <summary>
		/// Color for a founder
		/// </summary>
		public Color FounderColor { get; set; } = Color.IndianRed;
		/// <summary>
		/// Color for a founder that has an attribute
		/// </summary>
		public Color FounderHighlightColor { get; set; } = Color.HotPink;
		/// <summary>
		/// The color for an individual
		/// </summary>
		public Color IndividualColor { get; set; } = Color.Beige;
		/// <summary>
		/// The color of an parent of the selected individual
		/// </summary>
		public Color SelectedParentColor { get; set; } = Color.Yellow;
		/// <summary>
		/// Color for when an individual has an attribute
		/// </summary>
		public Color IndividualHighlightColor { get; set; } = Color.YellowGreen;
		/// <summary>
		/// Color for a parent of an individual with an attribute
		/// </summary>
		public Color ParentIndividualHightlightColor { get; set; } = Color.Cyan;

		//Colors for interest moments

		/// <summary>
		/// When a founder is interesting (such as the nearest founder)
		/// </summary>
		public Color InterestedFounderColor { get; set; } = Color.HotPink;
		/// <summary>
		/// When an individual is interesting (such as the at the same level as an individual founder)
		/// </summary>
		public Color InterestedIndividualColor { get; set; } = Color.Lavender;
		/// <summary>
		/// The color for the selected individuals direct children
		/// </summary>
		public Color DirectChildrenColor { get; set; } = Color.Orange;

		#endregion

		/// <summary>
		/// The attributes factory for all attribute
		/// </summary>
		public IndividualAttributesFactory AttributesFactory { get; }

		/// <summary>
		/// The Attributes to highlight
		/// </summary>
		public IEnumerable<IndividualAttribute> ToHighlight { get { return toHighlight; } }

		/// <summary>
		/// If all attributes are required to be highlighted
		/// </summary>
		public bool AllAttributesRequired { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a highlight to be highlighted
		/// </summary>
		/// <param name="attribute"></param>
		public void AddHightlight(IndividualAttribute attribute) {
			toHighlight.Add(attribute);
		}

		/// <summary>
		/// Removes an attribute to be highlighted
		/// </summary>
		/// <param name="attribute"></param>
		public void RemoveHighlight(IndividualAttribute attribute) {
			toHighlight.Remove(attribute);
		}

		/// <summary>
		/// If the individual should be highlighted due to its attributes
		/// </summary>
		/// <param name="individual">The individual to test</param>
		/// <returns>If they should be highlighted</returns>
		public bool IsHighlightIndividual(Individual individual) {
			foreach (var attribute in toHighlight) {
				if (AllAttributesRequired && !individual.HasAttribute(attribute)) {
					return false;
				} else if (individual.HasAttribute(attribute)) {
					return true;
				}
			}
			//If we require all attributes and we reach here
			// we succeeded, otherwise we have failed
			return AllAttributesRequired;
		}

		/// <summary>
		/// If this attribute is a highlight that is in the to be highlighted set
		/// </summary>
		/// <param name="individualAttribute">The attribute to test</param>
		/// <returns>If this highlight is in this set</returns>
		public bool IsHighlightAttribute(IndividualAttribute individualAttribute) {
			return toHighlight.Contains(individualAttribute);
		}

		#endregion

		public HighlightConfiguration(IndividualAttributesFactory attributesFactory) {
			AttributesFactory = attributesFactory;
		}
	}
}
