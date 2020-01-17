//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;

namespace Genealogy.Model {
	internal class ParentChangedEventArgs : EventArgs {
		public Individual OldParent { get; private set; }
		public Individual NewParent { get; private set; }

		public ParentChangedEventArgs(Individual oldParent, Individual newParent) {
			OldParent = oldParent;
			NewParent = newParent;
		}

	}
}
