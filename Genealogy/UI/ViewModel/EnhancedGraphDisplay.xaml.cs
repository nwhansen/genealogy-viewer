//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

using Genealogy.UI.Logic;
using Genealogy.ViewModel;
using Genealogy.ViewModel.UI;

using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

namespace Genealogy.UI.ViewModel {
	/// <summary>
	/// Interaction logic for EnhancedGraphDisplay.xaml
	/// </summary>
	public partial class EnhancedGraphDisplay : Window {
		private const System.Windows.Forms.MouseButtons LeftButton = System.Windows.Forms.MouseButtons.Left;
		private double width;
		private GViewer viewer;
		private GraphViewModel viewModel;
		private ColorAssigner assigner;
		private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
		private List<IndividualViewModelAndNode> individualNodes;
		private Dictionary<Node, DNode> nodeMapping = new Dictionary<Node, DNode>();

		public GraphViewModel ViewModel { get => viewModel ?? (viewModel = (GraphViewModel)DataContext); }

		public EnhancedGraphDisplay() {
			InitializeComponent();
		}

		private async void InjectLoader(object sender, RoutedEventArgs e) {
			assigner = new ColorAssigner(ViewModel.ColorConfiguration, ViewModel.HighlightAttributeConfiguration, ViewModel.IndividualManager);
			// Create the interop host control.
			System.Windows.Forms.Integration.WindowsFormsHost host =
				new System.Windows.Forms.Integration.WindowsFormsHost();
			ViewModel.IsGraphing = true;
			viewer = new GViewer {
				AsyncLayout = true,
				LayoutEditingEnabled = false,
				EdgeInsertButtonVisible = false,
				ForwardEnabled = false,
				BackwardEnabled = false,
				InsertingEdge = false,
				UndoRedoButtonsVisible = false,
				SaveGraphButtonVisible = false,
				SaveButtonVisible = false,
				LayoutAlgorithmSettingsButtonVisible = true,
				NavigationVisible = false,
				CurrentLayoutMethod = LayoutMethod.SugiyamaScheme
			};
			viewer.AsyncLayoutProgress += AsyncProgress;
			viewer.MouseUp += Viewer_MouseUp;
			viewer.MouseDown += Viewer_MouseDown;
			Graph graph = new Graph();
			((Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings)graph.LayoutAlgorithmSettings).LayerSeparation = 60;
			var graphAssembler = new GraphAssembler(ViewModel.IndividualManager, assigner);
			individualNodes = await graphAssembler.GenerateNodes(ViewModel.Individuals, graph, cancellation.Token);
			if (cancellation.IsCancellationRequested) {
				Close();
				return;
			}
			viewer.Graph = graph;
			// Assign the MaskedTextBox control as the host control's child.
			host.Child = viewer;
			Airspace.Content = host;
			//Fix the airspace while we render
			Airspace.FixAirspace = true;
		}

		private void Viewer_MouseDown(object sender, MouseEventArgs e) {
			//Test if the left button is pressed
			if (LeftPressed(e) && CtrlPressed()) {
				viewer.PanButtonPressed = true;
			}
		}

		private void Viewer_MouseUp(object sender, MouseEventArgs e) {
			//They were panning with left + ctrl
			if (LeftPressed(e) && CtrlPressed()) {
				viewer.PanButtonPressed = false;
			} else if (LeftPressed(e) && viewer.ObjectUnderMouseCursor is DNode node) {
				viewer.Invalidate(node);
				//Assign our selected view model. 
				ViewModel.SelectedIndividual = node.Node.UserData as IndividualViewModel;
				UpdateColors();
			}
		}

		private static bool LeftPressed(MouseEventArgs e) {
			return (e.Button & LeftButton) == LeftButton;
		}
		private static bool CtrlPressed() {
			return System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl);
		}

		private void UpdateColors() {
			foreach (var indvNode in individualNodes) {
				Color newColor = assigner.GetFill(indvNode.Individual, ViewModel.SelectedIndividual);
				if (newColor != indvNode.Node.Attr.FillColor) {
					//Update the fill
					indvNode.Node.Attr.FillColor = newColor;
					viewer.Invalidate(nodeMapping[indvNode.Node]);
				}
			}
		}

		private void AsyncProgress(object sender, LayoutProgressEventArgs e) {
			if (e.Progress == LayoutProgress.Finished) {
				nodeMapping.Clear();
				//Get all nodes
				Airspace.FixAirspace = false;
				foreach (var entity in viewer.Entities) {
					if (entity is DNode node) {
						nodeMapping.Add(node.Node, node);
					}
				}
				ViewModel.IsGraphing = false;
			} else if (e.Progress == LayoutProgress.Aborted) {
				Dispatcher.BeginInvoke((Action)(() => Close()));
			} else if (e.Progress == LayoutProgress.LayingOut || e.Progress == LayoutProgress.Rendering) {
				ViewModel.IsGraphing = true;
			}
		}

		private void CancelLoading(object sender, RoutedEventArgs e) {
			cancellation.Cancel();
			viewer.AbortAsyncLayout();
		}

		private void ToggleDetail(object sender, RoutedEventArgs e) {
			if (Details.Width == 0) {
				//Was 0
				Details.Width = width;
			} else {
				//Non Zero Width				
				width = Details.Width;
				Details.Width = 0;
			}
			((System.Windows.Controls.Button)sender).Content = Details.Width == 0 ? "Show" : "Collapse";
		}

	}
}
