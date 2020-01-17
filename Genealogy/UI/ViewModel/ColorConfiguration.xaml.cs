//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.ComponentModel;
using System.Windows;

namespace Genealogy.UI.ViewModel {
	/// <summary>
	/// A view for configuring the coloration of individuals on a graph
	/// </summary>
	public partial class HighlightConfiguration : Window {
		public HighlightConfiguration() {
			InitializeComponent();
		}

		protected override void OnClosing(CancelEventArgs e) {
			//We never fail with a non-accepted state
			DialogResult = true;
			base.OnClosing(e);
		}
	}
}
