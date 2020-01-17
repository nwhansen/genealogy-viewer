//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.Collections.Generic;

namespace Genealogy.Model {
	/// <summary>
	/// A class that manages all individuals for a given study or population
	/// </summary>
	public class IndividualManager {

		private Dictionary<string, Individual> _individuals = new Dictionary<string, Individual>();

		private ISet<Individual> _founders = new HashSet<Individual>();
		private List<IndividualAttribute> defaultHighlighted = new List<IndividualAttribute>();

		/// <summary>
		/// All individuals being managed
		/// </summary>
		public IEnumerable<Individual> AllIndividuals { get { return _individuals.Values; } }

		/// <summary>
		/// Returns an enumeration of all founders 
		/// </summary>
		public IEnumerable<Individual> Founders { get { return _founders; } }

		/// <summary>
		/// The attribute factory for this population
		/// </summary>
		public IndividualAttributesFactory AttributesFactory { get; } = new IndividualAttributesFactory();

		public IEnumerable<IndividualAttribute> DefaultHighlighted => defaultHighlighted;

		/// <summary>
		/// The Custom Display Format
		/// </summary>
		/// <remarks>
		/// How to format the custom display code. for example %DisplayCode%Sex\n%BirthDate
		/// 
		/// %DisplayCode = The Display Code
		/// %SexCode = The individual's Sex code (M,F)
		/// %Sex = The Individuals Sex (Male,Female)
		/// %BirthDate = The Individuals BirthDate (in local date format) - Blank if not known
		/// %DeathDate = The Individuals DeathDate (in local date format) - Blank if not known
		/// </remarks>
		public string CustomDisplayFormat { get; set; } = "%DisplayCode";

		/// <summary>
		/// If we should display the custom format
		/// </summary>
		public bool EnableCustomFormat { get; set; }

		/// <summary>
		/// Creates an individual manager
		/// </summary>
		public IndividualManager() {
		}

		/// <summary>
		/// Attempts to find an existing individual
		/// </summary>
		/// <param name="displayName"></param>
		/// <returns></returns>
		public Individual Find(string displayName) {
			if (_individuals.TryGetValue(displayName, out Individual individual))
				return individual;
			return null;
		}

		/// <summary>
		/// Attempts to create an individual or returns the existing individual. Updates the parents if desired.
		/// </summary>
		/// <param name="displayCode">The Display Code</param>
		/// <param name="isFemale">If the individual is female</param>
		/// <param name="fatherCode">The Father Code</param>
		/// <param name="motherCode">The Mother Code</param>
		/// <param name="allowReassign">Allows the father or mother to be re-assigned</param>
		/// <returns>The individual created (or found)</returns>
		public Individual CreateIndividual(string displayCode, bool isFemale, string fatherCode, string motherCode, bool allowReassign) {
			if (!_individuals.TryGetValue(displayCode, out Individual target)) {
				target = new Individual(isFemale, this) { DisplayCode = displayCode };
				_individuals.Add(displayCode, target);
				target.DisplayCodeChanged += IndividualDisplayCodeChanged;
			} else if (target.IsFemale != isFemale) {
				throw new InvalidOperationException(string.Format("Individual {0} was found to be a {1} by earlier relationships. Cannot assign to {2}", target.DisplayCode, GetSexString(target.IsFemale), GetSexString(isFemale)));
			}
			//Don't process father if it wasn't provided
			if (!string.IsNullOrEmpty(fatherCode) && (target.Father == null || target.Father.DisplayCode != fatherCode)) {
				if (!allowReassign && target.Father != null)
					throw new InvalidOperationException(string.Format("Unable to reassign father from {0} to {1}", target.Father.DisplayCode, fatherCode));
				//This is effectively a GetOrAdd ;)
				var father = CreateIndividual(fatherCode, false, null, null, false);
				father.AddChild(Find(motherCode), target);
			}
			//Don't mother if it wasn't provided
			if (!string.IsNullOrEmpty(motherCode) && (target.Mother == null || target.Mother.DisplayCode != motherCode)) {
				if (!allowReassign && target.Mother != null)
					throw new InvalidOperationException(string.Format("Unable to reassign mother from {0} to {1}", target.Mother.DisplayCode, motherCode));
				//This is effectively a GetOrAdd ;)
				var mother = CreateIndividual(motherCode, true, null, null, false);
				mother.AddChild(Find(fatherCode), target);
			}
			if (target.Mother == null && target.Father == null) {
				_founders.Add(target);
			}
			target.ParentChanged += IndividualParentChanged;
			return target;
		}

		public void AddDefaultHighlighted(IndividualAttribute attribute) {
			if (AttributesFactory.Contains(attribute)) {
				defaultHighlighted.Add(attribute);
			}
		}

		private void IndividualParentChanged(object sender, ParentChangedEventArgs e) {
			if (sender is Individual individual) {
				if (e.OldParent == null) {
					//Hey they are gaining a parent.
					// we can just blindly remove them since it wont effect much
					_founders.Remove(individual);
				} else if (e.NewParent == null) {
					//Hey they are loosing a parent.
					if (individual.Father == null && individual.Mother == null) {
						_founders.Add(individual);
					}
				}
			}
		}

		private void IndividualDisplayCodeChanged(object sender, DisplayCodeChanged e) {
			//Don't update if its not an individual  or we are not managing them
			if (sender is Individual individual) {
				if (Find(e.OldCode) != individual) {
					throw new InvalidOperationException("Received event for an individual we are not managing!");
				}
				//Check that the new code is unused.
				if (Find(e.NewCode) == null) {
					//Update our reference. 
					_individuals.Remove(e.OldCode);
					_individuals.Add(e.NewCode, individual);
				} else {
					e.Cancel();
				}
			}
		}

		/// <summary>
		/// Returns a string that can be used to interpret the sex of a individual
		/// </summary>
		/// <param name="isFemale">If they are female</param>
		/// <returns>The string representing their sex</returns>
		public static string GetSexString(bool isFemale) {
			return isFemale ? "Female" : "Male";
		}

	}
}
