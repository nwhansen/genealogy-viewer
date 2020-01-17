//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.ComponentModel;

namespace Genealogy.ViewModel.UI {
	public class OpenFileViewModel : INotifyPropertyChanged {

		private string path;
		private string fileFilter;

		public event PropertyChangedEventHandler PropertyChanged;


		public string Path {
			get { return path; }
			set {
				if (value != path) {
					path = value;
					PropertyChanged?.Notify(this, "Path");
				}
			}
		}

		public string FileFilter {
			get { return fileFilter; }
			set {
				if (value != fileFilter) {
					fileFilter = value;
					PropertyChanged?.Notify(this, "FileFilter");
				}
			}
		}
	}
}