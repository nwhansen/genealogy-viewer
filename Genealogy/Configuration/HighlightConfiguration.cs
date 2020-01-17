//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Collections.Generic;

using Genealogy.Model;

namespace Genealogy.Configuration {
	///<summary>
	/// A class to store what attributes are highlighted
	///</summary>
	public sealed class HighlightConfiguration {

		private readonly ISet<IndividualAttribute> toHighlight = new HashSet<IndividualAttribute>();

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
