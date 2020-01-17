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
	public class IndividualAttributesFactoryViewModel {

		private class GuidAttributes : KeyedCollection<Guid, IndividualAttributeViewModel> {
			protected override Guid GetKeyForItem(IndividualAttributeViewModel item) {
				return item.AttributeIdentifer;
			}
		}

		private GuidAttributes attributes = new GuidAttributes();

		/// <summary>
		/// The Individual Attribute factory we are presenting
		/// </summary>
		public IndividualAttributesFactory Wrapped { get; }

		public IndividualAttributeViewModel this[IndividualAttribute attr] {
			get {
				if (attr != null && attributes.Contains(attr.AttributeIdentifer)) {
					return attributes[attr.AttributeIdentifer];
				}
				return null;
			}
		}
		/// <summary>
		/// All attributes in the system
		/// </summary>
		public IEnumerable<IndividualAttributeViewModel> AllAttributes => attributes;

		public IndividualAttributesFactoryViewModel(IndividualAttributesFactory wrapped) {
			Wrapped = wrapped;

			Wrapped.AllAttributes.Select(i => new IndividualAttributeViewModel(i)).ForEach(attributes.Add);
		}

	}
}
