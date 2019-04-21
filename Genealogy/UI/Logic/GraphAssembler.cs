using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genealogy.Extensions;
using Genealogy.Model;
using Genealogy.ViewModel;
using Microsoft.Msagl.Drawing;

namespace Genealogy.UI.Logic {
	public class GraphAssembler {


		private readonly IndividualManagerViewModel individualManager;
		private readonly ColorAssigner assigner;
		private IEnumerable<IndividualViewModel> individuals;
		private CancellationToken cancellationToken;
		private bool customDisplayCode;
		private Graph graph;

		public GraphAssembler(IndividualManagerViewModel individualManager, ColorAssigner assigner) {
			this.individualManager = individualManager;
			this.assigner = assigner;
		}

		public Task<List<IndividualViewModelAndNode>> GenerateNodes(IEnumerable<IndividualViewModel> individuals,
			Graph graph,
			CancellationToken cancellationToken) {
			this.customDisplayCode = individualManager.EnableCustomFormat;
			this.individuals = individuals;
			this.graph = graph;
			this.cancellationToken = cancellationToken;
			return Task.Factory.StartNew(GenerateNodes, cancellationToken);
		}

		private List<IndividualViewModelAndNode> GenerateNodes() {
			List<IndividualViewModelAndNode> result = new List<IndividualViewModelAndNode>();
			Dictionary<IndividualViewModel, Node> existingNodes = new Dictionary<IndividualViewModel, Node>();
			foreach (var individual in individuals) {
				if (cancellationToken.IsCancellationRequested) {
					return null;
				}
				Node node = new Node(individual.UniqueIdentifier.ToString()) {
					LabelText = individual.Display
				};

				//Add them to our collection of problems
				result.Add(new IndividualViewModelAndNode { Node = node, Individual = individual });
				existingNodes.Add(individual, node);
				if (existingNodes.TryGetSafe(individualManager[individual.Father], out Node father)) {
					var edge = new Edge(father, node, ConnectionToGraph.Connected);
					father.AddOutEdge(edge);
				}
				if (existingNodes.TryGetSafe(individualManager[individual.Mother], out Node mother)) {
					var edge = new Edge(mother, node, ConnectionToGraph.Connected);
					mother.AddOutEdge(edge);
				}
				foreach (Node child in individual.Wrapped.AllChildren
						.ToViewModel(individualManager)
						.Where(i => existingNodes.ContainsKey(i))
						.Select(i => existingNodes[i])) {
					var edge = new Edge(node, child, ConnectionToGraph.Connected);
					node.AddOutEdge(edge);
				}
				node.Attr.FillColor = assigner.GetFill(individual, null);
				node.UserData = individual;
				graph.AddNode(node);
			}
			return result;
		}


	}
}

