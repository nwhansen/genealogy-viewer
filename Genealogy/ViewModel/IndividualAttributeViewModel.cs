﻿using Genealogy.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.ViewModel {
	/// <summary>
	/// Represents an individual attribute (which is a read only object)
	/// </summary>
	public class IndividualAttributeViewModel {
		/// <summary>
		/// The Attribute Code
		/// </summary>
		public string AttributeCode => Wrapped.AttributeCode;

		/// <summary>
		/// The Attribute Long Name (description)
		/// </summary>
		public string AttributeName => Wrapped.AttributeName;

		/// <summary>
		/// The Attribute Model which we are wrapping
		/// </summary>
		public IndividualAttribute Wrapped { get; }
		/// <summary>
		/// A unique identifier for this Attribute
		/// </summary>
		public Guid AttributeIdentifer => Wrapped.AttributeIdentifer;

		/// <summary>
		/// Creates a IndividualAttributeViewModel from an IndividualAttribute
		/// </summary>
		/// <param name="wrapped">The Attribute to wrap</param>
		public IndividualAttributeViewModel(IndividualAttribute wrapped) {
			Wrapped = wrapped;
		}

	}
}
