//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Genealogy.Configuration;

using Microsoft.Msagl.Drawing;

namespace Genealogy.ViewModel.Configuration {
	/// <summary>
	/// Describes the highlighting settings of a Graph
	/// </summary>
	public class ColorConfigurationViewModel : INotifyPropertyChanged {

		#region Private Member Variables

		private ColorsViewModel _founderColor;
		private ColorsViewModel _founderHighlightColor;
		private ColorsViewModel _individualColor;
		private ColorsViewModel _selectedParentColor;
		private ColorsViewModel _individualHighlightColor;
		private ColorsViewModel _parentIndividualHightlightColor;
		private ColorsViewModel _interestedFounderColor;
		private ColorsViewModel _interestedIndividualColor;
		private ColorsViewModel _directChildrenColor;

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;


		/// <summary>
		/// Exposes the highlight configuration this wraps
		/// </summary>
		public ColorConfiguration Wrapped { get; private set; }

		// The Colors have been further reduced to reduce visual clutter
		#region Color Properties

		/// <summary>
		/// All available colors supported by our graphing view
		/// </summary>
		public ObservableCollection<ColorsViewModel> AllColors { get; } = ColorsViewModel.GetViewModelList();

		/// <summary>
		/// Color for a founder
		/// </summary>
		public ColorsViewModel FounderColor {
			get { return _founderColor; }
			set {
				if (FounderColor.Wrapped != value.Wrapped) {
					_founderColor = value;
					Wrapped.FounderColor = value.Wrapped;
					PropertyChanged.Notify(this, "FounderColor");
				}
			}
		}
		/// <summary>
		/// Color for a founder that has an attribute
		/// </summary>
		public ColorsViewModel FounderHighlightColor {
			get { return _founderHighlightColor; }
			set {
				if (FounderHighlightColor.Wrapped != value.Wrapped) {
					_founderHighlightColor = value;
					Wrapped.FounderHighlightColor = value.Wrapped;
					PropertyChanged.Notify(this, "FounderHighlightColor");
				}
			}
		}
		/// <summary>
		/// The color for an individual
		/// </summary>
		public ColorsViewModel IndividualColor {
			get { return _individualColor; }
			set {
				if (IndividualColor.Wrapped != value.Wrapped) {
					_individualColor = value;
					Wrapped.IndividualColor = value.Wrapped;
					PropertyChanged.Notify(this, "IndividualColor");
				}
			}
		}
		/// <summary>
		/// The color of an parent of the selected individual
		/// </summary>
		public ColorsViewModel SelectedParentColor {
			get { return _selectedParentColor; }
			set {
				if (SelectedParentColor.Wrapped != value.Wrapped) {
					_selectedParentColor = value;
					Wrapped.SelectedParentColor = value.Wrapped;
					PropertyChanged.Notify(this, "SelectedParentColor");
				}
			}
		}
		/// <summary>
		/// Color for when an individual has an attribute
		/// </summary>
		public ColorsViewModel IndividualHighlightColor {
			get { return _individualHighlightColor; }
			set {
				if (IndividualHighlightColor.Wrapped != value.Wrapped) {
					_individualHighlightColor = value;
					Wrapped.IndividualHighlightColor = value.Wrapped;
					PropertyChanged.Notify(this, "IndividualHighlightColor");
				}
			}
		}
		/// <summary>
		/// Color for a parent of an individual with an attribute
		/// </summary>
		public ColorsViewModel ParentIndividualHightlightColor {
			get { return _parentIndividualHightlightColor; }
			set {
				if (ParentIndividualHightlightColor.Wrapped != value.Wrapped) {
					_parentIndividualHightlightColor = value;
					Wrapped.ParentIndividualHightlightColor = value.Wrapped;
					PropertyChanged.Notify(this, "ParentIndividualHightlightColor");
				}
			}
		}
		/// <summary>
		/// When a founder is interesting (such as the nearest founder)
		/// </summary>
		public ColorsViewModel InterestedFounderColor {
			get { return _interestedFounderColor; }
			set {
				if (InterestedFounderColor.Wrapped != value.Wrapped) {
					_interestedFounderColor = value;
					Wrapped.InterestedFounderColor = value.Wrapped;
					PropertyChanged.Notify(this, "InterestedFounderColor");
				}
			}
		}
		/// <summary>
		/// When an individual is interesting (such as the at the same level as an individual founder)
		/// </summary>
		public ColorsViewModel InterestedIndividualColor {
			get { return _interestedIndividualColor; }
			set {
				if (InterestedIndividualColor.Wrapped != value.Wrapped) {
					_interestedIndividualColor = value;
					Wrapped.InterestedIndividualColor = value.Wrapped;
					PropertyChanged.Notify(this, "InterestedIndividualColor");
				}
			}
		}
		/// <summary>
		/// The color for the selected individuals direct children
		/// </summary>
		public ColorsViewModel DirectChildrenColor {
			get { return _directChildrenColor; }
			set {
				if (value.Wrapped != _directChildrenColor.Wrapped) {
					_directChildrenColor = value;
					Wrapped.DirectChildrenColor = value.Wrapped;
					PropertyChanged.Notify(this, "DirectChildrenColor");
				}
			}
		}

		#endregion
		/// <summary>
		/// Creates a Color Configuration View Model
		/// </summary>
		public ColorConfigurationViewModel()
			: this(new ColorConfiguration()) {

		}

		/// <summary>
		/// Creates a color Configuration view model wrapping a given configuration
		/// </summary>
		/// <param name="wrapped">The configuration this view model should wrap</param>
		public ColorConfigurationViewModel(ColorConfiguration wrapped) {

			Wrapped = wrapped;
			//Set the color properties from the list
			_founderColor = FindColor(wrapped.FounderColor);
			_founderHighlightColor = FindColor(wrapped.FounderHighlightColor);
			_individualColor = FindColor(wrapped.IndividualColor);
			_selectedParentColor = FindColor(wrapped.SelectedParentColor);
			_individualHighlightColor = FindColor(wrapped.IndividualHighlightColor);
			_parentIndividualHightlightColor = FindColor(wrapped.ParentIndividualHightlightColor);
			_interestedFounderColor = FindColor(wrapped.InterestedFounderColor);
			_interestedIndividualColor = FindColor(wrapped.InterestedIndividualColor);
			_directChildrenColor = FindColor(wrapped.DirectChildrenColor);
		}




		#region Private Methods

		private ColorsViewModel FindColor(Color individualColor) {
			return AllColors.Where(i => i.Wrapped == individualColor).First();
		}

		#endregion
	}
}

