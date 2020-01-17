//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Algorithm;

using Genealogy.Model;

namespace Genealogy.ViewModel {

	public class IndividualManagerViewModel {

		private class GuidIndividuals : KeyedCollection<Guid, IndividualViewModel> {
			protected override Guid GetKeyForItem(IndividualViewModel item) {
				return item.UniqueIdentifier;
			}
		}

		#region Private Members ViewModels

		private readonly GuidIndividuals individualViewModels;

		#endregion

		#region Public Event and Properties


		public IEnumerable<IndividualViewModel> Individuals { get => individualViewModels; }

		public IndividualViewModel this[Individual individual] {
			get {
				if (individual != null && individualViewModels.Contains(individual.UniqueIdentifier)) {
					return individualViewModels[individual.UniqueIdentifier];
				}
				return null;
			}
		}
		public IndividualViewModel this[IndividualViewModel individual] {
			get {
				if (individual != null && individualViewModels.Contains(individual.UniqueIdentifier)) {
					return individualViewModels[individual.UniqueIdentifier];
				}
				return null;
			}
		}

		public IndividualManager Wrapped { get; }
		/// <summary>
		/// If we should display a custom name format
		/// </summary>
		public bool EnableCustomFormat { get => Wrapped.EnableCustomFormat; }

		/// <summary>
		/// The attributes factory which contains the attributes we are concerned about
		/// </summary>
		public IndividualAttributesFactoryViewModel AttributeFactory { get; }

		/// <summary>
		/// The attributes that are highlighted by default
		/// </summary>
		public IEnumerable<IndividualAttributeViewModel> DefaultHighlighted => Wrapped.DefaultHighlighted.ToViewModel(AttributeFactory);

		#endregion

		public IndividualManagerViewModel() : this(new IndividualManager()) { }

		public IndividualManagerViewModel(IndividualManager individualManager) {
			individualViewModels = new GuidIndividuals();
			Wrapped = individualManager ?? new IndividualManager();
			//Generate Attributes first
			AttributeFactory = new IndividualAttributesFactoryViewModel(Wrapped.AttributesFactory);
			//TODO: We cannot create VM's for people we don't know about. So we almost need to delay the computation of relatives until after we are done.
			AddIndividuals(Wrapped.AllIndividuals);
		}

		private void AddIndividuals(IEnumerable<Individual> individuals) {
			//Add individuals we don't have
			individuals.Where(i => this[i] == null).ForEach(AddIndividual);
		}


		private void AddIndividual(Individual individual) {
			individualViewModels.Add(new IndividualViewModel(individual, this));
		}

	}
}
