using Genealogy.Model;
using Genealogy.UIInteraction;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Genealogy.ViewModel {
	/// <summary>
	/// Wraps an Individual Model presenting 
	/// </summary>
	public class IndividualViewModel : INotifyPropertyChanged {

		private IndividualViewModel _father;
		private IndividualViewModel _mother;

		public event PropertyChangedEventHandler PropertyChanged;

		#region Public Properties
		/// <summary>
		/// The Individual that is wrapped by this model
		/// </summary>
		public Individual Wrapped { get; }

		/// <summary>
		/// The DisplayName to show the user
		/// </summary>
		public string DisplayCode {

			get { return Wrapped.DisplayCode; }
			set {
				if (Wrapped.DisplayCode != value) {
					Wrapped.DisplayCode = value;
					PropertyChanged.Notify(this, "DisplayCode");
				}
			}
		}

		/// <summary>
		/// A more customized display name
		/// </summary>
		public string CustomDisplayCode { get => Wrapped.CustomDisplayCode; }

		/// <summary>
		/// The View Model for the Mother
		/// </summary>
		public IndividualViewModel Father {
			get {
				//Maintain state
				if (Wrapped.Father != null && Wrapped.Father != _father.Wrapped) {
					_father = new IndividualViewModel(Wrapped.Father);
				} else if (Wrapped.Father == null && _father != null) {
					_father = null;
					return null;
				}
				return _father;
			}
		}

		/// <summary>
		/// The View Model for the Mother
		/// </summary>
		public IndividualViewModel Mother {
			get {
				//Maintain state
				if (Wrapped.Mother != null && Wrapped.Mother != _mother.Wrapped) {
					_mother = new IndividualViewModel(Wrapped.Mother);
				} else if (Wrapped.Mother == null && _mother != null) {
					_mother = null;
					return null;
				}
				return _mother;
			}
		}

		/// <summary>
		/// The Attributes for the individual
		/// </summary>
		public ObservableCollection<IndividualAttributeViewModel> Attributes { get; } = new ObservableCollection<IndividualAttributeViewModel>();

		#endregion

		/// <summary>
		/// Creates a view model for an individual
		/// </summary>
		/// <param name="wrapped">The individual to wrap</param>
		public IndividualViewModel(Individual wrapped) : this(wrapped, true) { }

		/// <summary>
		/// Generates a view model controlling if we should wrap the parents (helpful for direct relationship viewing)
		/// </summary>
		/// <param name="wrapped">The view model to wrap</param>
		/// <param name="wrapParents">If we should wrap the parents</param>
		internal IndividualViewModel(Individual wrapped, bool wrapParents) {
			Wrapped = wrapped;
			//Copy the attributes
			foreach (var attr in Wrapped.Attributes) {
				Attributes.Add(new IndividualAttributeViewModel(attr));
			}
			if (wrapParents && Wrapped.Father != null) {
				//Don't cascade upwards (prevents insane model pollution)
				_father = new IndividualViewModel(Wrapped.Father, false);
			}
			if (wrapParents && Wrapped.Mother != null) {
				//Don't cascade upwards (prevents insane model pollution)
				_mother = new IndividualViewModel(Wrapped.Mother, false);
			}
		}

	}
}