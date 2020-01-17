//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.Collections.Generic;

namespace Genealogy.Model {
	/// <summary>
	/// An attribute of a individual that may be shared
	/// </summary>
	public class IndividualAttribute : IEquatable<IndividualAttribute> {
		/// <summary>
		/// The attribute name to be displayed
		/// </summary>
		public string AttributeName { get; set; }

		/// <summary>
		/// A short-ish human generated code for an attribute to reduce typing
		/// </summary>
		public string AttributeCode { get; }

		/// <summary>
		/// A unique identifier for this attribute 
		/// </summary>
		public Guid AttributeIdentifer { get; }
		/// <summary>
		/// The factory that owns the attribute
		/// </summary>
		public IndividualAttributesFactory OwningFactory { get; }

		/// <summary>
		/// The set for tracking individuals with this attribute (saves compute time)
		/// </summary>
		internal ISet<Individual> individuals { get; } = new HashSet<Individual>();
		/// <summary>
		/// The individuals that contain this attribute helpful for types of analysis
		/// </summary>
		public IEnumerable<Individual> Individuals { get { return individuals; } }

		/// <summary>
		/// Creates a new Attribute to 
		/// </summary>
		internal IndividualAttribute(string name, string code, Guid identifier, IndividualAttributesFactory owningFactory) {
			AttributeName = name;
			AttributeCode = code;
			AttributeIdentifer = identifier;
			OwningFactory = owningFactory;
		}

		public override bool Equals(object obj) {
			if (obj is IndividualAttribute other) {
				return other.AttributeIdentifer == this.AttributeIdentifer;
			}
			return false;
		}

		public bool Equals(IndividualAttribute other) {
			return other != null &&
				   AttributeIdentifer.Equals(other.AttributeIdentifer);
		}

		public override int GetHashCode() {
			//We are uniquely identifed by our attribute identifer
			return AttributeIdentifer.GetHashCode();
		}
	}
}
