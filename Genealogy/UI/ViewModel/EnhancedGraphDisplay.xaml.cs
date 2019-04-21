using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Genealogy.UI.Logic;
using Genealogy.ViewModel;
using Genealogy.ViewModel.UI;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using static Genealogy.UI.Logic.GraphAssembler;

namespace Genealogy.UI.ViewModel {
	/// <summary>
	/// Interaction logic for EnhancedGraphDisplay.xaml
	/// </summary>
	public partial class EnhancedGraphDisplay : Window {

		const int stage = 0;
		const int stages = 2;
		const double percentPerStage = 100.0 / stages;
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
			assigner = new ColorAssigner(ViewModel.HighlightConfiguration, ViewModel.IndividualManager);
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
				LayoutAlgorithmSettingsButtonVisible = false,
				NavigationVisible = false,
				CurrentLayoutMethod = LayoutMethod.SugiyamaScheme
			};
			viewer.AsyncLayoutProgress += AsyncProgress;
			viewer.MouseUp += Viewer_MouseUp;
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
			GridHost.Children.Add(host);
		}

		private void Viewer_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (viewer.ObjectUnderMouseCursor is DNode node) {
				viewer.Invalidate(node);
				//Assign our selected view model. 
				ViewModel.SelectedIndividual = node.Node.UserData as IndividualViewModel;
				UpdateColors();
			}
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
				//Get all nodes
				foreach (var entity in viewer.Entities) {
					if (entity is DNode node) {
						nodeMapping.Add(node.Node, node);
					}
				}
				ViewModel.IsGraphing = false;
			} else if (e.Progress == LayoutProgress.Aborted) {
				Close();
			}
		}

		private void CancelLoading(object sender, RoutedEventArgs e) {
			cancellation.Cancel();
			viewer.AbortAsyncLayout();
		}

	}
}
