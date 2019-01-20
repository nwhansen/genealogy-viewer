using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Genealogy.Model;
using Genealogy.UIInteraction;

namespace Genealogy.ViewModel.UI {
	/// <summary>
	/// Allows for the modification of the global population settings
	/// </summary>
	public class PopulationConfigurationViewModel : INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// %DisplayCode = The Display Code
		/// %SexCode = The individual's Sex code (M,F)
		/// %Sex = The Individuals Sex (Male,Female)
		/// %BirthDate = The Individuals BirthDate (in local date format) - Blank if not known
		/// %DeathDate = The Individuals DeathDate (in local date format) - Blank if not known
		/// </summary>
		private static readonly Tuple<string, string>[] FixedFormatCodes = new Tuple<string, string>[] {
			new Tuple<string, string>("Individual Code", "%DisplayCode"),
			new Tuple<string, string>("Sex Code", "%SexCode"),
			new Tuple<string, string>("Sex", "%Sex"),
			new Tuple<string, string>("Birth Date", "%BirthDate"),
			new Tuple<string, string>("Death Date", "%DeathDate"),
		};
		private Tuple<string, string> _selectedCode;
		private readonly Action canInsertFormatCodeChanged;

		public IndividualManager IndividualManager { get; }

		public string FormatCode {
			get => IndividualManager.CustomDisplayFormat;
			set {
				if (value != IndividualManager.CustomDisplayFormat) {
					IndividualManager.CustomDisplayFormat = value;
					PropertyChanged?.Notify(this, "FormatCode");
				}
			}
		}

		public bool EnableFormatCode {
			get => IndividualManager.EnableCustomFormat;
			set {
				if (value != IndividualManager.EnableCustomFormat) {
					IndividualManager.EnableCustomFormat = value;
					PropertyChanged?.Notify(this, "EnableFormatCode");
					canInsertFormatCodeChanged?.Invoke();
				}
			}
		}

		public ObservableCollection<Tuple<string, string>> FormatCodes { get; } = new ObservableCollection<Tuple<string, string>>(FixedFormatCodes);

		public Tuple<string, string> SelectedCode {
			get {
				return _selectedCode;
			}
			set {
				if (value != _selectedCode) {
					_selectedCode = value;
					PropertyChanged?.Notify(this, "SelectedCode");
					canInsertFormatCodeChanged?.Invoke();
				}
			}
		}

		public CommandWrapper InsertSelectedFormatCodeCommand { get; }


		public PopulationConfigurationViewModel(IndividualManager manager) {
			IndividualManager = manager;
			InsertSelectedFormatCodeCommand = new CommandWrapper(() => SelectedCode != null && EnableFormatCode, InsertSelectedFormatCode, out canInsertFormatCodeChanged);
		}

		public PopulationConfigurationViewModel Clone(IndividualManager manager) {
			manager.CustomDisplayFormat = IndividualManager.CustomDisplayFormat;
			manager.EnableCustomFormat = IndividualManager.EnableCustomFormat;
			return new PopulationConfigurationViewModel(manager);
		}

		private void InsertSelectedFormatCode() {
			FormatCode = FormatCode + SelectedCode.Item2;
		}

	}
}
