﻿//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Genealogy.UI.ViewModel;
using Genealogy.UIInteraction;
using Genealogy.ViewModel;
using Genealogy.ViewModel.Configuration;
using Genealogy.ViewModel.UI;

namespace Genealogy {
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainWindow : Window {

        //We will default to female first because my Data.csv was that way
        private MainViewModel _viewModel;
        private MainViewModel ViewModel { get { return _viewModel ?? (_viewModel = (MainViewModel)DataContext); } }

        private string filterText {
            set {
                if (string.IsNullOrEmpty(value)) {
                    _filterElements = null;
                } else if (value.Contains(",")) {
                    _filterElements = value.Split(',');
                } else {
                    _filterElements = new[] { value };
                }
            }
        }

        private string[] _filterElements;
        ICollectionView filteredView;

        public MainWindow() {
            InitializeComponent();
        }
        public MainWindow(MainViewModel model) {
            InitializeComponent();
            DataContext = _viewModel = model;
            model.PresentOpenFile += OpenFileDialog;
            model.PresentPrompt += PresentPrompt;
            model.PresentSimpleViewModel += PresentSimpleViewModel;
        }

        private void PresentSimpleViewModel(object sender, PresentEventArgs e) {
            Window window;
            switch (e) {
                case PresentViewModelEventArgs<ColorConfigurationViewModel> vm:
                    window = new HighlightConfiguration { DataContext = vm.ViewModel };
                    break;
                case PresentViewModelEventArgs<GraphTreeConfigurationViewModel> vm:
                    window = new ConfigureGraph { DataContext = vm.ViewModel };
                    break;
                case PresentViewModelEventArgs<PopulationConfigurationViewModel> vm:
                    window = new AdvancedSettings { DataContext = vm.ViewModel };
                    break;
                case PresentViewModelEventArgs<GraphViewModel> vm:
                    window = new EnhancedGraphDisplay() { DataContext = vm.ViewModel };
                    break;
                case PresentViewModelEventArgs<HighlightAttributeConfigurationViewModel> vm:
                    window = new HighlightedAttributeConfiguration { DataContext = vm.ViewModel };
                    break;
                default: return; //Do nothing with the event we don't understand
            }
            if (e.Blocking) {
                e.InteractionState = ViewModelInteractionState.Presented;
                var result = window.ShowDialog();
                e.InteractionState = result.HasValue && result.Value ? ViewModelInteractionState.Accepted : ViewModelInteractionState.Rejected;
            } else {
                window.Show();
                e.InteractionState = ViewModelInteractionState.Presented;
            }


        }

        private void PresentPrompt(object sender, ConfirmationViewModelEventArgs e) {
            MessageBoxButton button = MessageBoxButton.OK;
            switch (e.ViewModel.Confirmation) {
                case ConfirmationViewModelEventArgs.ConfirmationType.OkCancel:
                    button = MessageBoxButton.OKCancel;
                    break;
                case ConfirmationViewModelEventArgs.ConfirmationType.YesNo:
                    button = MessageBoxButton.YesNo;
                    break;
            }
            var result = MessageBox.Show(e.ViewModel.Message, e.ViewModel.Caption, button);
            e.InteractionState = result == MessageBoxResult.OK || result == MessageBoxResult.Yes ? ViewModelInteractionState.Accepted : ViewModelInteractionState.Rejected;
        }

        private void OpenFileDialog(object sender, PresentViewModelEventArgs<OpenFileViewModel> e) {
            using (var dialog = new System.Windows.Forms.OpenFileDialog()) {
                dialog.Filter = e.ViewModel.FileFilter;
                switch (dialog.ShowDialog()) {
                    case System.Windows.Forms.DialogResult.OK:
                        e.InteractionState = ViewModelInteractionState.Accepted;
                        e.ViewModel.Path = dialog.FileName;
                        break;
                    default:
                        e.InteractionState = ViewModelInteractionState.Rejected;
                        break;
                }
            }
        }

        private ViewModelInteractionState ConvertToInteractionState(System.Windows.Forms.DialogResult result) {
            switch (result) {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    return ViewModelInteractionState.Accepted;
                default:
                    return ViewModelInteractionState.Rejected;
            }
        }

        private bool Filter(object toFilter) {
            if (_filterElements == null)
                return true;
            if (toFilter is IndividualViewModel ivm) {
                if (_filterElements.Length == 1)
                    return ivm.Display.Contains(_filterElements[0]);
                return _filterElements.Any(f => ivm.DisplayCode.Equals(f));
            }
            return false;
        }

        private void SearchKeyUp(object sender, KeyEventArgs e) {
            if (sender is TextBox searchBox) {
                if (filteredView == null) {
                    filteredView = CollectionViewSource.GetDefaultView(ViewModel.Individuals);
                }
                if (string.IsNullOrEmpty(searchBox.Text)) {
                    filteredView.Filter = null;
                } else {
                    filterText = searchBox.Text;
                    if (filteredView.Filter == null) {
                        filteredView.Filter = Filter;
                    } else {
                        filteredView.Refresh();
                    }
                }
            }
        }

        private void ShowAboutMenu(object sender, RoutedEventArgs e) {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }
    }
}
