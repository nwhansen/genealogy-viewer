//==============================================
// Copyright (c) 2020 Nathan Hansen
//==============================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Genealogy.Configuration;
using Genealogy.Model;
using Genealogy.UIInteraction;

namespace Genealogy.ViewModel.Configuration {
	///<summary>
	///
	///</summary>
	public class HighlightAttributeConfigurationViewModel : INotifyPropertyChanged {

		private IndividualAttributeViewModel selectedRemaining;
		private IndividualAttributeViewModel selectedToHighlight;
		private readonly IndividualAttributesFactoryViewModel _attributesFactory;
		private readonly Action _canAddChanged;
		private readonly Action _canRemoveChanged;
		private readonly HighlightConfiguration _wrapped;

		#region Public Properties & Events
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// A command to add the individual to the To Highlight
		/// </summary>
		public CommandWrapper AddSelectedRemaining { get; private set; }

		/// <summary>
		/// A command to remove an individual from To Highlight
		/// </summary>
		public CommandWrapper RemoveToHighlight { get; private set; }

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
		/// The collection of Individual Attributes that we are highlighting
		/// </summary>
		public ObservableCollection<IndividualAttributeViewModel> ToHighlight { get; } = new ObservableCollection<IndividualAttributeViewModel>();

		/// <summary>
		/// The collection of individual attributes that we could highlight
		/// </summary>
		public ObservableCollection<IndividualAttributeViewModel> Remaining { get; } = new ObservableCollection<IndividualAttributeViewModel>();

		#endregion

		/// <summary>
		/// Creates an HighlightAttributeConfigurationViewModel creating the backing highlight model automatically
		/// </summary>
		/// <param name="attributesFactory">The attribute factory to be used to get the list of attributes</param>
		public HighlightAttributeConfigurationViewModel(IndividualAttributesFactoryViewModel attributesFactory)
			: this(new HighlightConfiguration(attributesFactory.Wrapped), attributesFactory) {

		}
		/// <summary>
		/// Creates the HighlightAttributeConfigurationViewModel with the model to wrap and the factory to get attributes from
		/// </summary>
		/// <param name="wrapped">The model to wrap</param>
		/// <param name="attributesFactory">The factory to get attributes from</param>
		public HighlightAttributeConfigurationViewModel(HighlightConfiguration wrapped, IndividualAttributesFactoryViewModel attributesFactory) {
			_wrapped = wrapped;
			_attributesFactory = attributesFactory;
			//Build the view model - Figure out the color mapping
			var highlighted = new HashSet<IndividualAttribute>(wrapped.ToHighlight);
			foreach (var attr in _attributesFactory.AllAttributes) {
				if (highlighted.Contains(attr.Wrapped))
					ToHighlight.Add(attr);
				else
					Remaining.Add(attr);
			}
			//Commands
			AddSelectedRemaining = new CommandWrapper(() => Remaining.Count > 0, AddSelected, out _canAddChanged);
			RemoveToHighlight = new CommandWrapper(() => ToHighlight.Count > 0, RemoveSelected, out _canRemoveChanged);
		}


		private void AddSelected() {
			//make a local copy
			MoveAttributes(SelectedRemaining, true);
			_canRemoveChanged?.Invoke();
		}

		private void RemoveSelected() {
			MoveAttributes(SelectedToHighlight, false);
			_canRemoveChanged?.Invoke();
			_canAddChanged?.Invoke();
		}

		private void MoveAttributes(IndividualAttributeViewModel toMove, bool add) {
			var from = add ? Remaining : ToHighlight;
			var to = add ? ToHighlight : Remaining;
			if (from.Remove(toMove)) {
				to.Add(toMove);
				if (add) {
					_wrapped.AddHightlight(toMove.Wrapped);
				} else {
					_wrapped.RemoveHighlight(toMove.Wrapped);
				}
			}
		}
		/// <summary>
		/// If the individual is highlighted
		/// </summary>
		/// <param name="individualViewModel">The Individual View Mode should be highlighted</param>
		/// <returns>If the individual VM should be highlighted</returns>
		public bool ShouldHighlightIndividual(IndividualViewModel individualViewModel) {
			return _wrapped.IsHighlightIndividual(individualViewModel.Wrapped);
		}


		/// <summary>
		/// Adds a attribute to be highlighted
		/// </summary>
		/// <param name="attribute">The attribute to be highlighted</param>
		public void AddHighlight(IndividualAttributeViewModel attribute) {
			MoveAttributes(attribute, true);
		}
	}
}
