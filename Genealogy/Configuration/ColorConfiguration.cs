//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using Microsoft.Msagl.Drawing;

namespace Genealogy.Configuration {
	/// <summary>
	/// Stores the Color configuration for the graph
	/// </summary>
	public class ColorConfiguration {


		#region Normal Colors
		/// <summary>
		/// The color for an individual
		/// </summary>
		public Color IndividualColor { get; set; } = Color.Beige;
		/// <summary>
		/// Color for when an individual has an attribute
		/// </summary>
		public Color IndividualHighlightColor { get; set; } = Color.YellowGreen;
		/// <summary>
		/// Color for a parent of an individual with an attribute
		/// </summary>
		public Color ParentIndividualHightlightColor { get; set; } = Color.Cyan;
		/// <summary>
		/// Color for a founder
		/// </summary>
		public Color FounderColor { get; set; } = Color.IndianRed;
		/// <summary>
		/// Color for a founder that has an attribute
		/// </summary>
		public Color FounderHighlightColor { get; set; } = Color.HotPink;

		#endregion


		#region Interested (Selected) Colors

		/// <summary>
		/// When an individual is interesting (such as the at the same level as an individual founder)
		/// </summary>
		public Color InterestedIndividualColor { get; set; } = Color.DarkOrchid;

		/// <summary>
		/// The color of an parent of the selected individual
		/// </summary>
		public Color SelectedParentColor { get; set; } = Color.Yellow;

		/// <summary>
		/// When a founder is interesting (such as the nearest founder)
		/// </summary>
		public Color InterestedFounderColor { get; set; } = Color.Navy;
		/// <summary>
		/// The color for the selected individuals direct children
		/// </summary>
		public Color DirectChildrenColor { get; set; } = Color.Orange;

		#endregion



	}
}
