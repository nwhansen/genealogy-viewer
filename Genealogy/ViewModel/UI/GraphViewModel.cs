//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Collections.Generic;
using System.ComponentModel;

using Genealogy.ViewModel.Configuration;

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

		public ColorConfigurationViewModel ColorConfiguration { get; }

		public HighlightAttributeConfigurationViewModel HighlightAttributeConfiguration { get; }

		public IEnumerable<IndividualViewModel> Individuals { get; }

		public IndividualManagerViewModel IndividualManager { get; }

		public GraphViewModel(ColorConfigurationViewModel colorConfiguration, HighlightAttributeConfigurationViewModel highlightAttributeConfiguration, IEnumerable<IndividualViewModel> individualViewModels, IndividualManagerViewModel individualManager) {
			HighlightAttributeConfiguration = highlightAttributeConfiguration;
			ColorConfiguration = colorConfiguration;
			Individuals = individualViewModels;
			IndividualManager = individualManager;
		}
	}
}
