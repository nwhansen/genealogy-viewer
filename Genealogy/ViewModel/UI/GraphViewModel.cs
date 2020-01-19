//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Genealogy.ViewModel.Configuration;

namespace Genealogy.ViewModel.UI {
	public class GraphViewModel : INotifyPropertyChanged {

		private bool isGraphing;
		private IndividualViewModel selectedIndividual;
		private readonly ObservableCollection<ColorLabelDescriptorsViewModel> normalColorLabelDescriptors;
		private readonly ObservableCollection<ColorLabelDescriptorsViewModel> selectedColorLabelDescriptors;

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
					PropertyChanged.Notify(this, "ColorLabelDescriptors");
				}
			}
		}

		public ObservableCollection<ColorLabelDescriptorsViewModel> ColorLabelDescriptors {
			get {
				return SelectedIndividual != null ? selectedColorLabelDescriptors : normalColorLabelDescriptors;
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

		public string Title { get; }

		public IndividualManagerViewModel IndividualManager { get; }

		public GraphViewModel(ColorConfigurationViewModel colorConfiguration, HighlightAttributeConfigurationViewModel highlightAttributeConfiguration, IndividualManagerViewModel individualManager, IEnumerable<IndividualViewModel> individualViewModels, string title) {
			HighlightAttributeConfiguration = highlightAttributeConfiguration;
			ColorConfiguration = colorConfiguration;
			Individuals = individualViewModels;
			Title = title;
			IndividualManager = individualManager;
			normalColorLabelDescriptors = new ObservableCollection<ColorLabelDescriptorsViewModel> {
				new ColorLabelDescriptorsViewModel { Color = ColorConfiguration.IndividualHighlightColor, Label = "Attribute" },
				new ColorLabelDescriptorsViewModel { Color = ColorConfiguration.FounderColor, Label = "Founder" },
				new ColorLabelDescriptorsViewModel { Color = ColorConfiguration.FounderHighlightColor, Label = "Founder w/ Attribute" },
				new ColorLabelDescriptorsViewModel { Color = ColorConfiguration.ParentIndividualHightlightColor, Label = "Parent of Individual w/ Attribute" },
			};
			selectedColorLabelDescriptors = new ObservableCollection<ColorLabelDescriptorsViewModel> {
				new ColorLabelDescriptorsViewModel { Color = ColorConfiguration.InterestedIndividualColor, Label = "Selected Individual" },
				new ColorLabelDescriptorsViewModel { Color = ColorConfiguration.SelectedParentColor, Label = "Parent" },
				new ColorLabelDescriptorsViewModel { Color = ColorConfiguration.InterestedFounderColor, Label = "Selected Parent & Founder" },
				new ColorLabelDescriptorsViewModel { Color = ColorConfiguration.DirectChildrenColor, Label = "Offspring Color" },
			};
		}
	}
}
