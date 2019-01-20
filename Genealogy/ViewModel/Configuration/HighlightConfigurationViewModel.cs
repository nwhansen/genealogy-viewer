using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Genealogy.Configuration;
using Genealogy.Model;
using Genealogy.UIInteraction;
using Microsoft.Msagl.Drawing;

namespace Genealogy.ViewModel.Configuration {
	/// <summary>
	/// Describes the highlighting settings of a Graph
	/// </summary>
	public class HighlightConfigurationViewModel : INotifyPropertyChanged {

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
		private IndividualAttributeViewModel selectedRemaining;
		private IndividualAttributeViewModel selectedToHighlight;
		private readonly Action _canAddChanged;
		private readonly Action _canRemoveChanged;

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The Selected items from the remaining collection
		/// </summary>
		public IndividualAttributeViewModel SelectedRemaining {
			get { return selectedRemaining; }
			set {
				if (value != selectedRemaining) {
					selectedRemaining = value;
					PropertyChanged?.Notify(this, "SelectedRemaining");
				}
			}
		}
		/// <summary>
		/// The Selected items from the to highlight collection
		/// </summary>
		public IndividualAttributeViewModel SelectedToHighlight {
			get { return selectedToHighlight; }
			set {
				if (value != selectedToHighlight) {
					selectedToHighlight = value;
					PropertyChanged?.Notify(this, "SelectedToHighlight");
				}
			}
		}

		/// <summary>
		/// A command to add the individual to the To Highlight
		/// </summary>
		public CommandWrapper AddSelectedRemaining { get; private set; }

		/// <summary>
		/// A command to remove an individual from To Highlight
		/// </summary>
		public CommandWrapper RemoveToHighlight { get; private set; }

		/// <summary>
		/// Exposes the highlight configuration this wraps
		/// </summary>
		public HighlightConfiguration Wrapped { get; private set; }

		/// <summary>
		/// The collection of Individual Attributes that we are highlighting
		/// </summary>
		public ObservableCollection<IndividualAttributeViewModel> ToHighlight { get; } = new ObservableCollection<IndividualAttributeViewModel>();

		/// <summary>
		/// The collection of individual attributes that we could highlight
		/// </summary>
		public ObservableCollection<IndividualAttributeViewModel> Remaining { get; } = new ObservableCollection<IndividualAttributeViewModel>();

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
		/// Creates a Highlight Configuration view model wrapping a given configuration
		/// </summary>
		/// <param name="wrapped">The configuration this view model should wrapp</param>
		public HighlightConfigurationViewModel(HighlightConfiguration wrapped) {
			//Build the view model - Figure out the color mapping
			HashSet<IndividualAttribute> highlighted = new HashSet<IndividualAttribute>(wrapped.ToHighlight);
			foreach (IndividualAttribute attr in wrapped.AttributesFactory.AllAttributes) {
				if (highlighted.Contains(attr))
					ToHighlight.Add(new IndividualAttributeViewModel(attr));
				else
					Remaining.Add(new IndividualAttributeViewModel(attr));
			}
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
			//Commands
			AddSelectedRemaining = new CommandWrapper(() => Remaining.Count > 0, AddSelected, out _canAddChanged);
			RemoveToHighlight = new CommandWrapper(() => ToHighlight.Count > 0, RemoveSelected, out _canRemoveChanged);
		}

		/// <summary>
		/// Creates a deep-ish Clone of the view model for instances where we want to use this VM as a base for another highlighting configuraiton
		/// </summary>
		/// <returns>A copy of this Highlight Configuration</returns>
		public HighlightConfigurationViewModel Clone() {
			var configuration = new HighlightConfiguration(Wrapped.AttributesFactory);
			foreach (var highlight in ToHighlight) {
				configuration.AddHightlight(highlight.Wrapped);
			}
			CopyColorConfiguration(configuration);
			//Copied the configuration
			return new HighlightConfigurationViewModel(configuration);
		}
		/// <summary>
		/// Clones this configuration exactly
		/// </summary>
		/// <returns>The cloned configuration not wrapped</returns>
		public HighlightConfiguration CloneConfiguration() {
			var configuration = new HighlightConfiguration(Wrapped.AttributesFactory);
			foreach (var highlight in ToHighlight) {
				configuration.AddHightlight(highlight.Wrapped);
			}
			CopyColorConfiguration(configuration);
			//Copied the configuration
			return configuration;
		}

		/// <summary>
		/// Clones the Color Configuration and assigns it a new Attributes Factory, used when opening a new file (new attributes, but same coloring settings)
		/// </summary>
		/// <param name="attributesFactory">The new attributes factory to be used to get attributes</param>
		/// <returns>The Configuration View Model that has only retained the color mapping</returns>
		public HighlightConfigurationViewModel Clone(IndividualAttributesFactory attributesFactory) {
			var configuration = new HighlightConfiguration(attributesFactory);
			CopyColorConfiguration(configuration);
			return new HighlightConfigurationViewModel(configuration);
		}



		#region Private Methods

		private void CopyColorConfiguration(HighlightConfiguration configuration) {
			configuration.FounderColor = Wrapped.FounderColor;
			configuration.FounderHighlightColor = Wrapped.FounderHighlightColor;
			configuration.IndividualColor = Wrapped.IndividualColor;
			configuration.SelectedParentColor = Wrapped.SelectedParentColor;
			configuration.IndividualHighlightColor = Wrapped.IndividualHighlightColor;
			configuration.ParentIndividualHightlightColor = Wrapped.ParentIndividualHightlightColor;
			configuration.InterestedFounderColor = Wrapped.InterestedFounderColor;
			configuration.InterestedIndividualColor = Wrapped.InterestedIndividualColor;
			configuration.DirectChildrenColor = Wrapped.DirectChildrenColor;
		}
		private void AddSelected() {
			//make a local copy
			MoveAttributes(SelectedRemaining, Remaining, ToHighlight, true);

			_canRemoveChanged?.Invoke();
		}

		private void RemoveSelected() {
			MoveAttributes(SelectedToHighlight, ToHighlight, Remaining, false);
			_canRemoveChanged?.Invoke();
			_canAddChanged?.Invoke();
		}

		private ColorsViewModel FindColor(Color individualColor) {
			return AllColors.Where(i => i.Wrapped == individualColor).First();
		}

		private void MoveAttributes(IndividualAttributeViewModel toMove, ObservableCollection<IndividualAttributeViewModel> from, ObservableCollection<IndividualAttributeViewModel> to, bool add) {
			if (from.Remove(toMove)) {
				to.Add(toMove);
				if (add) {
					Wrapped.AddHightlight(toMove.Wrapped);
				} else {
					Wrapped.RemoveHighlight(toMove.Wrapped);
				}
			}
		}

		#endregion
	}
}

