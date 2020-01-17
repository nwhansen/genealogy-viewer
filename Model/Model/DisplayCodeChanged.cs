//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;

namespace Genealogy.Model {
	internal class DisplayCodeChanged : EventArgs {
		public string OldCode { get; }

		public string NewCode { get; }

		public bool Cancelled { get; private set; }

		public DisplayCodeChanged(string oldCode, string newCode) {
			OldCode = oldCode;
			NewCode = newCode;
			Cancelled = false;
		}

		public void Cancel() {
			Cancelled |= true;
		}
	}
}
