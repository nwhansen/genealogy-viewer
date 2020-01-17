//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Windows;

namespace Genealogy.UI.ViewModel {
	/// <summary>
	/// Represents a view for configuring a graph's common settings
	/// </summary>
	public partial class ConfigureGraph : Window {
		public ConfigureGraph() {
			InitializeComponent();
		}

		private void Graph(object sender, RoutedEventArgs e) {
			DialogResult = true;
			Close();
		}
	}
}
