//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.Windows;
using System.Windows.Input;

namespace Genealogy.UI.ViewModel {
	/// <summary>
	/// Represents a view for configuring a graph's common settings
	/// </summary>
	public partial class ConfigureGraph : Window {
		public ConfigureGraph() {
			InitializeComponent();
			this.PreviewKeyDown += ConfigureGraph_PreviewKeyDown;
		}

		private void ConfigureGraph_PreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				CloseAndGraph();
				e.Handled = true;
			}
		}

		private void Graph(object sender, EventArgs e) {
			CloseAndGraph();
		}

		private void CloseAndGraph() {
			DialogResult = true;
			Close();
		}

	}
}
