using Algorithm;
using Algorithm.Algorithms;
using Genealogy.Configuration;
using Genealogy.Model;
using Genealogy.UIInteraction;
using Genealogy.ViewModel.Configuration;
using Genealogy.ViewModel.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Genealogy.ViewModel {
	public class MainViewModel : INotifyPropertyChanged {

		private FileInterfaceFactory interfaceFactory = new FileInterfaceFactory();
		private IndividualViewModel _selectedIndividual;
		private IndividualManager individualManager;
		private HighlightConfigurationViewModel globalHighlight;
		private readonly Action canGraphSelectedIndividualChanged;
		private readonly Action computeTrueNegativeChanged;
		private readonly Action graphWithAttributeChanged;
		private readonly Action graphEveryoneChanged;
		private readonly Action graphPossibleNegatives;

		#region Public Properties

		public ObservableCollection<IndividualViewModel> Individuals { get; } = new ObservableCollection<IndividualViewModel>();

		public HighlightConfigurationViewModel GlobalHighlight {
			get { return globalHighlight; }
			private set {
				if (globalHighlight != null) {
					GlobalHighlight.ToHighlight.CollectionChanged -= UpdateCanGraph;
				}
				globalHighlight = value;
				PropertyChanged?.Notify(this, "GlobalHighlight");
				GlobalHighlight.ToHighlight.CollectionChanged += UpdateCanGraph;
			}
		}

		public IndividualViewModel SelectedIndividual {
			get {
				return _selectedIndividual;
			}
			set {
				if (value != _selectedIndividual) {
					_selectedIndividual = value;
					PropertyChanged.Notify(this, "SelectedIndividual");
					canGraphSelectedIndividualChanged();

				}
			}
		}

		#region Command Wrappers

		/// <summary>
		/// The Command to graph an individual
		/// </summary>
		public CommandWrapper GraphSelectedIndividualCommand { get; }

		/// <summary>
		/// The command to trigger a read of a file
		/// </summary>
		public CommandWrapper OpenFileCommand { get; }

		/// <summary>
		/// A request from the user to configure the global highlight
		/// </summary>
		public CommandWrapper ConfigureGlobalHighlightCommand { get; }

		/// <summary>
		/// A Request from the user to configure the global highlight
		/// </summary>
		public CommandWrapper ComputeTrueNegativeCommand { get; }
		/// <summary>
		/// Graphs all individuals with a given attribute
		/// </summary>
		public CommandWrapper GraphWithAttributeCommand { get; }

		/// <summary>
		/// Graphs all individuals in a graph
		/// </summary>
		public CommandWrapper GraphEveryoneCommand { get; }

		/// <summary>
		/// Runs a possible negative computation
		/// </summary>
		public CommandWrapper GraphPossibleNegativeCommand { get; }
		/// <summary>
		/// Presents a the Global Population Command settings VM (basically a minimal wrapper around our individual manager)
		/// </summary>
		public CommandWrapper ConfigureGlobalPopulationSettingsCommand { get; }

		#endregion

		#endregion

		#region Events
		/// <summary>
		/// Sends a request for a simple view model to the view - used for odds and ends view models that have a simple workflow
		/// </summary>
		public event EventHandler<PresentEventArgs> PresentSimpleViewModel;

		/// <summary>
		/// Requests to display a graph to a user
		/// </summary>
		public event EventHandler<PresentViewModelEventArgs<GraphViewModel>> PresentDisplayGraph;

		/// <summary>
		/// Requests to open an Open File Dialog to the user
		/// </summary>
		public event EventHandler<PresentViewModelEventArgs<OpenFileViewModel>> PresentOpenFile;
		/// <summary>
		/// Requests to present a prompt to a user
		/// </summary>
		public event EventHandler<ConfirmationViewModelEventArgs> PresentPrompt;

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		/// Creates the main view model with a given individual manager
		/// </summary>
		/// <param name="individualManager">The individual manager to use for our population</param>
		public MainViewModel(IndividualManager individualManager) {
			this.individualManager = individualManager ?? new IndividualManager();
			GlobalHighlight = new HighlightConfigurationViewModel(new HighlightConfiguration(this.individualManager.AttributesFactory));
			GraphSelectedIndividualCommand = new CommandWrapper(() => SelectedIndividual != null, GraphIndividual, out canGraphSelectedIndividualChanged);
			ConfigureGlobalHighlightCommand = new CommandWrapper(() => PresentSimpleViewModel?.Present(this, GlobalHighlight, true));
			ConfigureGlobalPopulationSettingsCommand = new CommandWrapper(() => PresentSimpleViewModel?.Present(this, new PopulationConfigurationViewModel(individualManager)));
			OpenFileCommand = new CommandWrapper(ReadFileFile);
			ComputeTrueNegativeCommand = new CommandWrapper(HasAttributePopulation, GraphComputeTrueNegative, out computeTrueNegativeChanged);
			GraphWithAttributeCommand = new CommandWrapper(HasAttributePopulation, GraphWithAttribute, out graphWithAttributeChanged);
			GraphEveryoneCommand = new CommandWrapper(HasIndividuals, GraphEveryOne, out graphEveryoneChanged);
			GraphPossibleNegativeCommand = new CommandWrapper(HasAttributePopulation, GraphComputePossibleNegative, out graphPossibleNegatives);

			BuildList();
		}

		/// <summary>
		/// A function to test if we have anybody to work with
		/// </summary>
		/// <returns>If we have sombody to work with</returns>
		private bool HasIndividuals() => Individuals.Count > 0;

		/// <summary>
		/// A function to test if we have attributes to highlight
		/// </summary>
		/// <returns>If we have attributes</returns>
		private bool HasAttributePopulation() => GlobalHighlight.ToHighlight.Count > 0 && HasIndividuals();

		/// <summary>
		/// Responds to a NotifyCollectionChanged from the highlight view model (signling) that we can or cannot highlight something
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpdateCanGraph(object sender, EventArgs e) {
			computeTrueNegativeChanged?.Invoke();
			graphWithAttributeChanged?.Invoke();
			graphPossibleNegatives?.Invoke();
		}

		/// <summary>
		/// Represents the view->model interaction to read a file and update our collections
		/// </summary>
		private void ReadFileFile() {
			var viewModel = new OpenFileViewModel { FileFilter = interfaceFactory.FileFilter };
			if (PresentOpenFile?.Present(this, viewModel, true)) {
				IGenealogyFileInterface fileInterface;
				try {
					fileInterface = interfaceFactory.CreateFileInterface(viewModel.Path);
				} catch (Exception e) {
					PresentPrompt?.Invoke(this, new ConfirmationViewModelEventArgs(e.Message, "Unable to load file", ConfirmationViewModelEventArgs.ConfirmationType.Ok));
					return;
				}
				try {
					individualManager = fileInterface.ParseFile(viewModel.Path);
				} catch (Exception e) {
					PresentPrompt?.Invoke(this, new ConfirmationViewModelEventArgs(e.Message, "Unable to read file", ConfirmationViewModelEventArgs.ConfirmationType.Ok));
				}
				//Clone the colors but not the attributes
				GlobalHighlight = globalHighlight.Clone(individualManager.AttributesFactory);
				//Rebuild our list of individuals
				BuildList();
				var updateAttributes = new ConfirmationViewModelEventArgs("Do you want to set the highlighting settings",
																			"Update Highlighting",
																			ConfirmationViewModelEventArgs.ConfirmationType.YesNo);
				PresentPrompt?.Invoke(this, updateAttributes);
				if (updateAttributes.InteractionState == ViewModelInteractionState.Accepted) {
					PresentSimpleViewModel?.Invoke(this, new PresentViewModelEventArgs<HighlightConfigurationViewModel>(GlobalHighlight, true));
				}
				graphEveryoneChanged?.Invoke();
				graphPossibleNegatives?.Invoke();
			}
		}

		/// <summary>
		/// Creates the individual view models for a given individual manager
		/// </summary>
		private void BuildList() {
			Individuals.Clear();
			foreach (Individual individual in individualManager.AllIndividuals) {
				Individuals.Add(new IndividualViewModel(individual));
			}
		}

		#region Graph Functions

		/// <summary>
		/// View Model Logic to graph all individuals
		/// </summary>
		private void GraphEveryOne() {
			PresentDisplayGraph?.Present(this, GlobalHighlight.Clone(), Individuals);
		}

		/// <summary>
		/// View->Model logic to graph a single individual 
		/// </summary>
		private void GraphIndividual() {
			var args = PresentSimpleViewModel?.Present(this, new GraphTreeConfigurationViewModel { CanHideChildren = false, CanLimitAncestors = false }, true);
			if (args) {
				//They have accepted  the graph request. 
				int relatives = args.ViewModel.ShowRelatives ? args.ViewModel.RelationCount : 0;
				IEnumerable<Individual> individuals = SelectedIndividual.Wrapped.FamilyTree(relatives);
				//preserve global configuration
				PresentDisplayGraph?.Present(this, GlobalHighlight, individuals);
			}
		}

		/// <summary>
		/// View->Model logic to graph a "possible" negative
		/// </summary>
		private void GraphComputePossibleNegative() {
			//Need to make the number a option
			//Oh hey numieric
			var except = AttributeHighlighting.PossiblePositive(GlobalHighlight.ToHighlight.Select(i => i.Wrapped), 5);
			var individuals = individualManager.AllIndividuals.Except(except);
			PresentDisplayGraph?.Present(this, GlobalHighlight, individuals);
		}

		/// <summary>
		/// Computes a true Negative (AKA somebody with NO relation at all with the given attribute)
		/// </summary>
		private void GraphComputeTrueNegative() {
			//Generate except
			var except = AttributeHighlighting.PossiblePositive(GlobalHighlight.ToHighlight.Select(i => i.Wrapped));
			var individuals = individualManager.AllIndividuals.Except(except);
			PresentDisplayGraph?.Present(this, GlobalHighlight, individuals);
		}

		/// <summary>
		/// View->Model Logic to graph all individuals and their relatives with a given attribute set
		/// </summary>
		private void GraphWithAttribute() {
			var viewModel = new GraphTreeConfigurationViewModel();
			if (!PresentSimpleViewModel?.Present(this, viewModel, true))
				return;
			var individuals = AttributeHighlighting.HighlightAny(GlobalHighlight.ToHighlight.Select(i => i.Wrapped),
							viewModel.ShowChildren,
							viewModel.LimitAncestors ? viewModel.AncestorCount : -1,
							viewModel.ShowRelatives ? viewModel.RelationCount : 0);
			PresentDisplayGraph?.Present(this, GlobalHighlight, individuals);
		}

		#endregion
	}
}
