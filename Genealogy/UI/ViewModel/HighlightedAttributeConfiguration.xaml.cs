//==============================================
// Copyright (c) 2020 Nathan Hansen
//==============================================
using System.ComponentModel;
using System.Windows;

namespace Genealogy.UI.ViewModel {
	/// <summary>
	/// Interaction logic for HighlightedAttributeConfiguration.xaml
	/// </summary>
	public partial class HighlightedAttributeConfiguration : Window {
		public HighlightedAttributeConfiguration() {
			InitializeComponent();
		}

		protected override void OnClosing(CancelEventArgs e) {
			//We never fail with a non-accepted state
			DialogResult = true;
			base.OnClosing(e);
		}
	}
}
