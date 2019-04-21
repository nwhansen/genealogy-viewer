using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genealogy.ViewModel;
using Microsoft.Msagl.Drawing;

namespace Genealogy.UI.Logic {
	/// <summary>
	/// A simple data structure to represent node and individual
	/// </summary>
	public struct IndividualViewModelAndNode {
		/// <summary>
		/// The individual associated with the given node
		/// </summary>
		public IndividualViewModel Individual { get; set; }
		/// <summary>
		/// The node this individual
		/// </summary>
		public Node Node { get; set; }
	}
}
