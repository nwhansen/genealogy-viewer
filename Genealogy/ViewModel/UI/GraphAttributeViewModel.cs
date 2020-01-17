//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.ComponentModel;

namespace Genealogy.ViewModel.UI {
	/// <summary>
	/// A view model for configuring a graph
	/// </summary>
	public class GraphTreeConfigurationViewModel : INotifyPropertyChanged {

		private bool showChildren = true;
		private bool limitAncestors = true;
		private bool showRelatives;

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Can the User Limit the number of ancestors being graphed
		/// </summary>
		public bool CanLimitAncestors { get; set; } = true;

		/// <summary>
		/// Can the user control if the children are graphed
		/// </summary>
		public bool CanHideChildren { get; set; } = true;

		/// <summary>
		/// Should the Graph Show an individual Children
		/// </summary>
		public bool ShowChildren {
			get { return showChildren; }
			set {
				if (value != showChildren) {
					showChildren = value;
					PropertyChanged?.Notify(this, "ShowChildren");
				}
			}
		}

		/// <summary>
		/// Should the Graph limit the number of ancestors
		/// </summary>
		public bool LimitAncestors {
			get { return limitAncestors; }
			set {
				if (value != limitAncestors) {
					limitAncestors = value;
					PropertyChanged?.Notify(this, "LimitAncestors");
				}
			}
		}

		/// <summary>
		/// Should the graph show relatives
		/// </summary>
		public bool ShowRelatives {
			get { return showRelatives; }
			set {
				if (value != showRelatives) {
					showRelatives = value;
					PropertyChanged?.Notify(this, "ShowRelatives");
				}
			}
		}
		/// <summary>
		/// The Number of ancestors to show
		/// </summary>
		public SingleNumericViewModel AncestorCount { get; set; } = new SingleNumericViewModel { Max = 100, Min = 0, Value = 3 };

		/// <summary>
		/// The number of "relatives" to show
		/// </summary>
		public SingleNumericViewModel RelationCount { get; set; } = new SingleNumericViewModel { Max = 100, Min = 0, Value = 1 };

	}
}
