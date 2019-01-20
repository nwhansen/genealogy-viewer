using Genealogy.Configuration;
using Genealogy.Extensions;
using Genealogy.Model;
using Genealogy.ViewModel;
using Genealogy.ViewModel.Configuration;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genealogy.UI {
	public class GraphDisplay : Form {

		private GViewer viewer;
		private Graph graph;
		private readonly HighlightConfiguration _graphConfiguration;
		private IEnumerable<Tuple<Node, Individual>> graphNodes;
		private Dictionary<Node, Individual> nodeToIndividual;
		private readonly bool customDisplayCode;


		public GraphDisplay(IEnumerable<IndividualViewModel> individuals, HighlightConfigurationViewModel graphConfiguration, bool customDisplayCode)
			: this(individuals.Cast<Individual>(), graphConfiguration.Wrapped, customDisplayCode) {

		}

		public GraphDisplay(IEnumerable<Individual> individuals, HighlightConfiguration graphConfiguration, bool customDisplayCode) {
			_graphConfiguration = graphConfiguration;
			this.customDisplayCode = customDisplayCode;
			InitializeComponent();
			SuspendLayout();
			viewer = new GViewer();
			graph = new Graph();
			((Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings)graph.LayoutAlgorithmSettings).LayerSeparation = 180;
			BuildGraph(individuals);
			viewer.CurrentLayoutMethod = LayoutMethod.SugiyamaScheme;
			viewer.Graph = graph;
			viewer.Dock = DockStyle.Fill;
			viewer.MouseUp += Viewer_MouseUp;
			Controls.Add(viewer);
			ResumeLayout();
		}

		private void BuildGraph(IEnumerable<Individual> individuals) {
			graph = new Graph();
			if (graphNodes != null) {
				foreach (var node in graphNodes)
					graph.RemoveNode(node.Item1);
			}
			var nodes = new List<Tuple<Node, Individual>>();
			var dictionary = new Dictionary<Node, Individual>();
			var existingNodes = new Dictionary<Individual, Node>();
			foreach (var individual in individuals) {
				Node node = new Node(individual.UniqueIdentifier.ToString()) {
					LabelText = customDisplayCode ? individual.CustomDisplayCode : individual.DisplayCode
				};
				existingNodes.Add(individual, node);
				if (existingNodes.TryGetSafe(individual.Father, out Node father)) {
					var edge = new Edge(father, node, ConnectionToGraph.Connected);
					father.AddOutEdge(edge);
				}
				if (existingNodes.TryGetSafe(individual.Mother, out Node mother)) {
					var edge = new Edge(mother, node, ConnectionToGraph.Connected);
					mother.AddOutEdge(edge);
				}
				foreach (Node child in individual.AllChildren.Where(i => existingNodes.ContainsKey(i)).Select(i => existingNodes[i])) {
					var edge = new Edge(node, child, ConnectionToGraph.Connected);
					node.AddOutEdge(edge);
				}
				node.Attr.FillColor = GetFill(individual);
				graph.AddNode(node);
				nodes.Add(new Tuple<Node, Individual>(node, individual));
				dictionary.Add(node, individual);

			}
			graphNodes = nodes;
			nodeToIndividual = dictionary;
		}

		private void Viewer_MouseUp(object sender, EventArgs e) {
			//Find the selected node
			foreach (var individuals in graphNodes) {
				individuals.Item1.Attr.FillColor = GetFill(individuals.Item2);
			}
			if (viewer.SelectedObject is Node node) {
				foreach (var childEdge in node.OutEdges) {
					childEdge.TargetNode.Attr.FillColor = _graphConfiguration.DirectChildrenColor;
				}
				//Color the parents yellow
				foreach (var parentEdge in node.InEdges) {
					parentEdge.SourceNode.Attr.FillColor = _graphConfiguration.InterestedIndividualColor;
				}
				//Find the "first" or equa distant founders
				ColorFirstFounder(node);
			}
			viewer.Invalidate();
		}


		private void ColorFirstFounder(Node selected) {
			var toProcess = new Queue<Tuple<int, Node>>();
			toProcess.Enqueue(new Tuple<int, Node>(0, selected));
			int founderDepth = int.MaxValue;
			int currentDepth = 0;
			do {
				var current = toProcess.Dequeue();
				currentDepth = current.Item1;
				if (currentDepth <= founderDepth) {
					Node currentNode = current.Item2;
					if (currentNode.InEdges.Count() == 0) {
						var individual = nodeToIndividual[currentNode];
						//Highlight all founders that are closest
						if (individual.IsFounder) {
							currentNode.Attr.FillColor = _graphConfiguration.InterestedFounderColor;
							founderDepth = currentDepth;
						} else {
							//They are an individual  at the "same depth" but not
							// a true founder.. if the graph was larger we might be able to see them.
							currentNode.Attr.FillColor = _graphConfiguration.InterestedIndividualColor;
						}
					} else {
						foreach (var inEdges in currentNode.InEdges) {
							toProcess.Enqueue(new Tuple<int, Node>(currentDepth + 1, inEdges.SourceNode));
						}
					}
				}
			} while (toProcess.Count > 0 || (currentDepth <= founderDepth && toProcess.Count > 0));
		}

		/// <summary>
		/// Returns the fill color for a given individual
		/// </summary>
		/// <param name="individual"></param>
		/// <returns></returns>
		private Color GetFill(Individual individual) {
			bool isFounder = individual.IsFounder;
			//See if any attributes of the individual are of the highlighed
			bool hasAttr = _graphConfiguration.IsHighlightIndividual(individual);
			Color fillColor;
			if (isFounder && hasAttr) {
				//If we are a founder with the attribute
				fillColor = _graphConfiguration.FounderHighlightColor;
			} else if (isFounder) {
				//If we are a founder
				fillColor = _graphConfiguration.FounderColor;
			} else if (hasAttr) {
				//If we ourselves have our attribute
				fillColor = _graphConfiguration.IndividualHighlightColor;
			} else if (individual.AllChildren.Where(i => _graphConfiguration.IsHighlightIndividual(i)).Any()) {
				//If we are the parent of a child with the attribute
				fillColor = _graphConfiguration.ParentIndividualHightlightColor;
			} else {
				//We are of little interest ;)
				fillColor = _graphConfiguration.IndividualColor;
			}

			return fillColor;
		}

		private void InitializeComponent() {
			this.SuspendLayout();
			// 
			// GraphDisplay
			// 
			this.ClientSize = new System.Drawing.Size(441, 367);
			this.Name = "GraphDisplay";
			this.ResumeLayout(false);

		}
	}
}
