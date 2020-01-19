//==============================================
// Copyright (c) 2020 Nathan Hansen
//==============================================
using Genealogy.ViewModel.Configuration;

namespace Genealogy.ViewModel {

	public interface IHelpPresentGraph {
		/// <summary>
		/// The Color Configuration the Graph will use
		/// </summary>
		ColorConfigurationViewModel ColorConfiguration { get; }
		/// <summary>
		/// The Highlighted attribute configuration the graph will use
		/// </summary>
		HighlightAttributeConfigurationViewModel HighlightConfiguration { get; }
		/// <summary>
		/// The Individual manager the graph will use to work with individuals
		/// </summary>
		IndividualManagerViewModel IndividualManagerViewModel { get; }
	}
}