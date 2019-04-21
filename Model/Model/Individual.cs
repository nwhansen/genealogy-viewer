using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Model {
	/// <summary>
	/// Represents an individual in a genetic pool
	/// </summary>
	public class Individual {

		#region Private Member Variables

		private IndividualManager individualManager;
		private Individual mother, father;
		private string displayCode;
		private Dictionary<Individual, ISet<Individual>> allChildren;
		private ISet<Individual> unknownPartner;
		private ISet<IndividualAttribute> attributes;

		#endregion

		#region Public Properties

		/// <summary>
		/// A unique identifier created to track individual
		/// </summary>
		public Guid UniqueIdentifier { get; private set; }

		/// <summary>
		/// A code to display
		/// </summary>
		public string DisplayCode {
			get { return displayCode; }
			set {
				if (value != displayCode) {
					var args = new DisplayCodeChanged(displayCode, value);
					DisplayCodeChanged?.Invoke(this, args);
					if (!args.Cancelled) {
						displayCode = value;
					}
				}
			}
		}

		/// <summary>
		/// When the Individual was born
		/// </summary>
		public DateTime BirthDate { get; set; } = DateTime.MinValue;

		/// <summary>
		/// When the Individual died
		/// </summary>
		public DateTime DeathDate { get; set; } = DateTime.MinValue;

		/// <summary>
		/// The Custom Display Code (combining elements)
		/// </summary>
		public string CustomDisplayCode {
			get {
				return individualManager.CustomDisplayFormat
					.Replace("%DisplayCode", DisplayCode)
					.Replace("%SexCode", IsFemale ? "F" : "M")
					.Replace("%Sex", IsFemale ? "Female" : "Male")
					.Replace("%BirthDate", GetDate(BirthDate))
					.Replace("%DeathDate", GetDate(DeathDate));
			}
		}

		/// <summary>
		/// The Biological mother of this individual
		/// </summary>
		public Individual Mother {
			get { return mother; }
			set {
				if (value != mother) {
					//You cannot set a father as a mother (this tracks biological info)
					if (value != null && !value.IsFemale)
						return;
					Individual old = mother;
					mother = value;
					ParentChanged?.Invoke(this, new ParentChangedEventArgs(old, value));
				}
			}
		}

		/// <summary>
		/// The Biological father of this individual
		/// </summary>
		public Individual Father {
			get { return father; }
			set {
				if (value != father) {
					//You cannot set a mother as a father (this tracks biological info)
					if (value != null && value.IsFemale)
						return;
					Individual old = father;
					father = value;
					ParentChanged?.Invoke(this, new ParentChangedEventArgs(old, value));
				}
			}
		}

		/// <summary>
		/// Returns an enumeration of the individual parents
		/// </summary>
		public IEnumerable<Individual> Parents {
			get {
				if (Mother != null) {
					yield return Mother;
				}
				if (Father != null) {
					yield return Father;
				}
			}
		}

		/// <summary>
		/// Returns if this individual is a founder
		/// </summary>
		public bool IsFounder {
			get { return Father == null && Mother == null; }
		}

		/// <summary>
		/// Is fired when the parent of this individual changes, used to update parents child collection
		/// </summary>
		internal event EventHandler<ParentChangedEventArgs> ParentChanged;

		/// <summary>
		/// Is fired when the display code is attempted to be changed this can be canceled
		/// </summary>
		internal event EventHandler<DisplayCodeChanged> DisplayCodeChanged;

		/// <summary>
		/// If this individual is female
		/// </summary>
		public bool IsFemale { get; private set; }

		/// <summary>
		/// The attributes of this individual
		/// </summary>
		public IEnumerable<IndividualAttribute> Attributes { get { return attributes; } }

		/// <summary>
		/// Enumerates all children that this individual has
		/// </summary>
		public IEnumerable<Individual> AllChildren {
			get {
				//iterate over each partner in the all children collection 
				// and return them. A child can only have 2 parents so this should 
				// never return duplicates
				foreach (Individual child in allChildren.Values.SelectMany(i => i))
					yield return child;
				foreach (Individual child in unknownPartner)
					yield return child;
			}
		}

		#endregion

		#region Constructors
		/// <summary>
		/// Creates an individual for a population
		/// </summary>
		/// <param name="isFemale">If this individual is female</param>
		/// <param name="individualManager">The individual manager this individual belongs to</param>
		internal Individual(bool isFemale, IndividualManager individualManager) : this(isFemale, individualManager, Guid.NewGuid()) {

		}

		/// <summary>
		/// Creates an individual with an already existing identifier used to deserialization entries
		/// </summary>
		/// <param name="isFemale">If this individual is female</param>
		/// <param name="individualManager">The individual manager this individual belongs to</param>
		/// <param name="uniqueIdentifier">The unique identifier for this individual</param>
		internal Individual(bool isFemale, IndividualManager individualManager, Guid uniqueIdentifier) {
			IsFemale = isFemale;
			UniqueIdentifier = uniqueIdentifier;
			this.individualManager = individualManager;
			attributes = new HashSet<IndividualAttribute>();
			allChildren = new Dictionary<Individual, ISet<Individual>>();
			unknownPartner = new HashSet<Individual>();
		}


		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the children this individual had with this spouse
		/// </summary>
		/// <param name="partner">The partner to get the children with</param>
		/// <returns>A collection of the children with this individual, or an empty enumeration</returns>
		public IEnumerable<Individual> ChildrenWith(Individual partner) {
			allChildren.TryGetValue(partner, out ISet<Individual> partnerChildren);
			return partnerChildren ?? new HashSet<Individual>();
		}

		/// <summary>
		/// Adds a child to the this individual for a partner
		/// </summary>
		/// <param name="partner">The partner of this child</param>
		/// <param name="child">The child to add</param>
		public void AddChild(Individual partner, Individual child) {
			//Try to get the collection
			ISet<Individual> children;
			if (partner == null) {
				children = unknownPartner;
			} else if (!allChildren.TryGetValue(partner, out children)) {
				children = new HashSet<Individual>();
				allChildren.Add(partner, children);
			}
			if (IsFemale) {
				child.Mother = this;
			} else {
				child.Father = this;
			}
			child.ParentChanged += ChildParentChanged;
			children.Add(child);
		}

		/// <summary>
		/// Adds a collection of children to this individual for this partnet
		/// </summary>
		/// <param name="partner">The partner these children belong to</param>
		/// <param name="newChildren">The new children to add</param>
		public void AddChildren(Individual partner, IEnumerable<Individual> newChildren) {
			//Try to get the collection
			if (!allChildren.TryGetValue(partner, out ISet<Individual> children)) {
				children = new HashSet<Individual>();
				allChildren.Add(partner, children);
			}
			foreach (Individual child in newChildren) {
				children.Add(child);
				if (IsFemale) {
					child.Mother = this;
				} else {
					child.Father = this;
				}
			}
		}

		/// <summary>
		/// Adds an attribute to this individual 
		/// </summary>
		/// <param name="attribute">The attribute to give this individual</param>
		public void AddAttribute(IndividualAttribute attribute) {
			attributes.Add(attribute);
			attribute.individuals.Add(this);
		}

		/// <summary>
		/// Removes an attribute from this individual
		/// </summary>
		/// <param name="attribute"></param>
		public void RemoteAttribute(IndividualAttribute attribute) {
			if (attributes.Remove(attribute)) {
				attribute.individuals.Remove(this);
			}

		}


		/// <summary>
		/// Returns if this individual has the attribute
		/// </summary>
		/// <param name="attribute">The attribute to test</param>
		/// <returns>If the individual has said attribute</returns>
		public bool HasAttribute(IndividualAttribute attribute) {
			return attributes.Contains(attribute);
		}

		#endregion

		/// <summary>
		/// Handles when a child has its parent changed
		/// </summary>
		/// <param name="sender">The child to which we had parent change</param>
		/// <param name="e">The parent changed event</param>
		private void ChildParentChanged(object sender, ParentChangedEventArgs e) {
			//Get the spouse and remove that
			Individual child = sender as Individual;
			Individual partner = IsFemale ? child.Father : child.Mother;
			if (e.OldParent == this) {
				//Remove this is no longer our child
				RemoveChild(partner, child);
			} else {
				//Our spouse has changed update the child reference
				if (e.OldParent != null) {
					RemoveChild(e.OldParent, child);
				} else {
					//Remove from unknown
					unknownPartner.Remove(child);
				}
				//Add the child with new parent
				AddChild(e.NewParent, child);
			}
		}

		/// <summary>
		/// Returns a string for a given date
		/// </summary>
		/// <param name="time">The time to get a string for</param>
		/// <returns>The Short Date for the time or blank</returns>
		private string GetDate(DateTime time) {
			if (time == DateTime.MinValue) {
				return string.Empty;
			}
			return time.ToShortDateString();
		}

		/// <summary>
		/// Removes a child from this individual for a given partner 
		/// </summary>
		/// <param name="partner">The partner this child was from</param>
		/// <param name="child">The child to remove</param>
		private void RemoveChild(Individual partner, Individual child) {
			if (allChildren.TryGetValue(partner, out ISet<Individual> children)) {
				children.Remove(child);
				if (children.Count == 0) {
					//If there is no children remove them
					allChildren.Remove(partner);
				}
			}
		}

	}
}
