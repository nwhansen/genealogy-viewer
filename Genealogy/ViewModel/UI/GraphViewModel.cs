using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genealogy.Configuration;
using Genealogy.Model;
using Genealogy.ViewModel.Configuration;
using Microsoft.Msagl.Drawing;

namespace Genealogy.ViewModel.UI {
	public class GraphViewModel : INotifyPropertyChanged {

		private bool isGraphing;
		private IndividualViewModel selectedIndividual;

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The Currently Selected Individual
		/// </summary>
		public IndividualViewModel SelectedIndividual {
			get => selectedIndividual;
			set {
				if (value != selectedIndividual) {
					selectedIndividual = value;
					PropertyChanged.Notify(this, "SelectedIndividual");
				}
			}
		}

		public bool IsGraphing {
			get => isGraphing;
			set {
				if (value != isGraphing) {
					isGraphing = value;
					PropertyChanged.Notify(this, "IsGraphing");
				}
			}
		}

		public HighlightConfigurationViewModel HighlightConfiguration { get; }

		public IEnumerable<IndividualViewModel> Individuals { get; }

		public IndividualManagerViewModel IndividualManager { get; }

		public GraphViewModel(HighlightConfigurationViewModel highlightConfigurationViewModel, IEnumerable<IndividualViewModel> individualViewModels, IndividualManagerViewModel individualManager) {
			HighlightConfiguration = highlightConfigurationViewModel;
			Individuals = individualViewModels;
			IndividualManager = individualManager;
		}
	}
}
