using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genealogy.Configuration;
using Genealogy.Model;
using Genealogy.ViewModel.Configuration;
using Microsoft.Msagl.Drawing;

namespace Genealogy.ViewModel.UI {
	public class GraphViewModel {

		public HighlightConfiguration HighlightConfiguration { get; private set; }

		public IEnumerable<Individual> Individuals { get; private set; }

		/// <summary>
		/// Creates the graph view model used to create the graph
		/// </summary>
		/// <param name="colorConfiguration"></param>
		/// <param name="individuals"></param>
		public GraphViewModel(HighlightConfiguration highlightConfiguration, IEnumerable<Individual> individuals) {
			Individuals = individuals;
			HighlightConfiguration = highlightConfiguration;
		}

		public GraphViewModel(HighlightConfigurationViewModel highlightConfigurationViewModel, IEnumerable<IndividualViewModel> individualViewModels)
			: this(highlightConfigurationViewModel.CloneConfiguration(), individualViewModels.Select(i => i.Wrapped)) {

		}
	}
}
