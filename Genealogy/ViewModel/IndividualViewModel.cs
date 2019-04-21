using Algorithm;
using Genealogy.Model;
using Genealogy.UIInteraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Genealogy.ViewModel {
	/// <summary>
	/// Wraps an Individual Model presenting 
	/// </summary>
	public class IndividualViewModel : INotifyPropertyChanged {

		private readonly IndividualManagerViewModel individualManager;
		private IndividualViewModel mother, father;
		private ObservableCollection<IndividualViewModel> children;
		private ObservableCollection<IndividualAttributeViewModel> attributes;


		public event PropertyChangedEventHandler PropertyChanged;

		#region Public Properties

		/// <summary>
		/// The Individual that is wrapped by this model
		/// </summary>
		public Individual Wrapped { get; }

		/// <summary>
		/// The Display Name to show taking into consideration if the custom display is enabled
		/// </summary>
		public string Display => individualManager.EnableCustomFormat ? CustomDisplayCode : DisplayCode;

		/// <summary>
		/// The DisplayName to show the user
		/// </summary>
		public string DisplayCode {

			get { return Wrapped.DisplayCode; }
			set {
				if (Wrapped.DisplayCode != value) {
					Wrapped.DisplayCode = value;
					PropertyChanged.Notify(this, "DisplayCode");
					PropertyChanged.Notify(this, "CustomDisplayCode");
				}
			}
		}

		/// <summary>
		/// A more customized display name
		/// </summary>
		public string CustomDisplayCode => Wrapped.CustomDisplayCode;

		/// <summary>
		/// The unique identifier for this individual
		/// </summary>
		public Guid UniqueIdentifier => Wrapped.UniqueIdentifier;


		/// <summary>
		/// If this individual is a founder
		/// </summary>
		public bool IsFounder => Wrapped.IsFounder;


		/// <summary>
		/// Returns an enumeration of the parents 
		/// </summary>
		public IEnumerable<IndividualViewModel> Parents {
			get {
				if (Father != null) {
					yield return Father;
				}
				if (Mother != null) {
					yield return Mother;
				}
			}
		}

		/// <summary>
		/// The View Model for the Father
		/// </summary>
		public IndividualViewModel Father {
			get {
				if (father == null && Wrapped.Father != null) {
					father = individualManager[Wrapped.Father];
				}
				return father;
			}
		}

		/// <summary>
		/// The View Model for the Mother
		/// </summary>
		public IndividualViewModel Mother {
			get {
				if (mother == null && Wrapped.Mother != null) {
					mother = individualManager[Wrapped.Mother];
				}
				return mother;
			}
		}

		/// <summary>
		/// All of the children for the individual
		/// </summary>
		public ObservableCollection<IndividualViewModel> Children {
			get {
				//Lazy initialization
				if (children == null) {
					children = new ObservableCollection<IndividualViewModel>();
					Wrapped.AllChildren.ToViewModel(individualManager).ForEach(children.Add);
				}
				return children;
			}
		}

		/// <summary>
		/// The Attributes for the individual
		/// </summary>
		public ObservableCollection<IndividualAttributeViewModel> Attributes {
			get {
				if (attributes == null) {
					attributes = new ObservableCollection<IndividualAttributeViewModel>();
					Wrapped.Attributes.ToViewModel(individualManager.AttributeFactory).ForEach(attributes.Add);
				}
				return attributes;
			}
		}

		#endregion

		/// <summary>
		/// Creates a view model for an individual
		/// </summary>
		/// <param name="wrapped">The view model to wrap</param>
		/// <param name="individualManager">The individual view manager to be used to process relatives</param>
		public IndividualViewModel(Individual wrapped, IndividualManagerViewModel individualManager) {
			Wrapped = wrapped;
			this.individualManager = individualManager;
			//We initialize lazily since our related VM's may not exist yet. 
		}

	}
}