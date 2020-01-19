//==============================================
// Copyright (c) 2020 Nathan Hansen
//==============================================
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Genealogy {
	/// <summary>
	/// Interaction logic for AboutDialog.xaml
	/// </summary>
	public partial class AboutDialog : Window {
		public AboutDialog() {
			InitializeComponent();
			VersionLabel.Text = GetVersion().ToString();
		}

		private static Version GetVersion() {
			try {
				return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
			} catch (Exception) {
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}



		private void Hyperlink_RequestNavigate(object sender, EventArgs e) {
			Process.Start(new ProcessStartInfo(github.NavigateUri.AbsoluteUri));
		}
	}
}
