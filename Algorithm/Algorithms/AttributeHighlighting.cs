//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Collections.Generic;
using System.Linq;

using Genealogy.Model;

namespace Algorithm.Algorithms {
	/// <summary>
	/// Algorithm for finding individuals based on attribute highlighting
	/// </summary>
	public static class AttributeHighlighting {


		/// <summary>
		/// Generates a list of individuals that have any of the given attributes
		/// </summary>
		/// <param name="attributes">The attributes to have highlighted</param>
		/// <param name="includeChildren">If we should include children always of an individual that is highlighted</param>
		/// <param name="maxAncestors">The max number of ancestors to include</param>
		/// <param name="relationship">The max relative distance we should show (aunts uncles, nephews etc)</param>
		/// <returns>An enumeration of individuals who have any of the attributes up to max ancestors</returns>
		public static IEnumerable<Individual> HighlightAny(IEnumerable<IndividualAttribute> attributes, bool includeChildren, int maxAncestors = -1, int relationship = -1) {
			HashSet<Individual> returned = new HashSet<Individual>();
			bool all = maxAncestors < 0 || maxAncestors == int.MaxValue;
			foreach (var individual in attributes.SelectMany(i => i.Individuals)) {
				//Don't process a given tree twice
				if (!returned.Has(individual)) {
					returned.Add(individual);
					yield return individual;

				}

				//False for the individual as they were already processed (saves a logical check)
				IEnumerable<IterationHelper> possibleReturn = all ? individual.AncestorsHelper(false) : individual.AncestorsHelper(maxAncestors, false);
				foreach (var toReturn in possibleReturn) {
					if (!returned.Has(toReturn.Individual)) {
						returned.Add(toReturn.Individual);
						yield return toReturn.Individual;
					}
					//Process for the rare event the "relatives" are new
					if (relationship > toReturn.RelationStep) {
						foreach (var children in toReturn.Individual.AllChildren.Where(i => !returned.Has(i)).ForEachAnd(i => returned.Add(i))) {
							yield return children;
						}
					}
				}
				if (includeChildren) {
					foreach (var toReturn in individual.DescendantsHelper(false)) {
						if (!returned.Has(toReturn.Individual)) {
							returned.Add(toReturn.Individual);
							yield return toReturn.Individual;
						}
						//Capture the Individual's partner
						if (relationship > toReturn.RelationStep) {
							foreach (var parent in toReturn.Individual.Parents.Where(i => !returned.Has(i)).ForEachAnd(i => returned.Add(i))) {
								yield return parent;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Generates a list of individuals that have all of the given attributes
		/// </summary>
		/// <param name="attributes">The attributes to have highlighted</param>
		/// <param name="includeChildren">If we should include children always of an individual that is highlighted</param>
		/// <param name="maxAncestors">The max number of ancestors to include</param>
		/// <returns>An enumeration of individuals who have all of the attributes up to max ancestors</returns>
		public static IEnumerable<Individual> HighlightAll(IEnumerable<IndividualAttribute> attributes, bool includeChildren, int maxAncestors = -1) {
			//Make a local copy - in case they gave us a non-re-enumerable enumeration
			List<IndividualAttribute> individualAttributes = new List<IndividualAttribute>(attributes);
			HashSet<Individual> processed = new HashSet<Individual>();
			bool all = maxAncestors < 0 || maxAncestors == int.MaxValue;
			//This gets all individuals with any of the given attributes
			foreach (var individual in individualAttributes.SelectMany(i => i.Individuals)) {
				//If we have tried this individual don't try again (they usually don't spontaneously change attributes)
				if (processed.Has(individual))
					continue;
				//Check if the individual matches
				if (!individualAttributes.All(i => individual.HasAttribute(i))) {
					processed.Add(individual);
					continue;
				}
				processed.Add(individual);
				yield return individual;
				//False for the individual as they were already processed (saves a logical check)
				IEnumerable<Individual> possibleReturn = all ? individual.Ancestors(false) : individual.Ancestors(maxAncestors, false);
				foreach (var toReturn in possibleReturn.Where(i => !processed.Has(i))) {
					yield return toReturn;
				}
				if (includeChildren) {
					foreach (var toReturn in individual.Descendants(false).Where(i => !processed.Has(i))) {
						yield return toReturn;
					}
				}
			}
		}
		/// <summary>
		/// Returns all individuals which have any descendant to somebody with the all the given attributes
		/// </summary>
		/// <param name="attributes">The filtering attributes</param>
		/// <returns>The Individuals who "could" have this as a possible positive</returns>
		public static IEnumerable<Individual> PossiblePositive(IEnumerable<IndividualAttribute> attributes) {
			//Make a local copy - in case they gave us a non-re-enumerable enumeration
			List<IndividualAttribute> individualAttributes = new List<IndividualAttribute>(attributes);

			//The individuals whom we have processed their parents and children
			HashSet<Individual> processedCompletly = new HashSet<Individual>();

			//A Queue of individuals who's relatives have not been processed
			// prepopulate with any individuals which have "all" the attributes
			// this gives us a seed to work with
			HashSet<Individual> currentProcessList = new HashSet<Individual>();
			Queue<Individual> toProcess = new Queue<Individual>(attributes.SelectMany(i => i.Individuals)
											//Filter known positives
											.Where(i => !currentProcessList.Has(i))
											//Validate they have the all attributes
											.Where(i => individualAttributes.All(attr => i.HasAttribute(attr)))
											//Queue them into the proceess list
											.ForEachAnd(i => currentProcessList.Add(i)));
			//Individuals in this collection remove everyone related to them.
			while (toProcess.Count > 0) {
				var individual = toProcess.Dequeue();
				yield return individual;
				//As far as we are concerned this individual has been processed
				processedCompletly.Add(individual);
				//Get the children & parents who haven't been processed, or will be processed
				individual.AllChildren.Concat(individual.Parents).Where(c => !c.In(processedCompletly, currentProcessList))
					//Add to process list (to prevent duplicate adds
					.ForEachAnd(c => currentProcessList.Add(c))
					.ForEach(c => toProcess.Enqueue(c));
			}
		}



		public static IEnumerable<Individual> PossiblePositive(IEnumerable<IndividualAttribute> attributes, int maxDistance) {
			//Make a local copy - in case they gave us a non-re-enumerable enumeration
			List<IndividualAttribute> individualAttributes = new List<IndividualAttribute>(attributes);

			//The individuals whom we have processed their parents and children
			// We keep track of the iteration helper we used for the indivual incase we found a lower number and need to re-evaulate the tree
			//  inbreeding and cross breeding are a thing
			KeyedIterationHelper processedCompletly = new KeyedIterationHelper();

			//A Queue of individuals who's relatives have not been processed
			// prepopulate with any individuals which have "all" the attributes
			// this gives us a seed to work with
			KeyedIterationHelper currentProcessList = new KeyedIterationHelper();
			IEnumerable<IterationHelper> positives = attributes.SelectMany(i => i.Individuals)
											//Filter known positives
											.Where(i => !i.In(currentProcessList))
											//Validate they have the all attributes
											.Where(i => individualAttributes.All(attr => i.HasAttribute(attr)))
											//Queue them into the process list
											.Select(i => new IterationHelper { Individual = i, RelationStep = 0 })
											.ForEachAnd(i => currentProcessList.Add(i));
			Queue<Individual> toProcess = new Queue<Individual>(positives.Select(i => i.Individual));
			while (toProcess.Count > 0) {
				var individual = toProcess.Dequeue();
				//This will always get the lowest relationship we have currently found - Processing children may update a value in this set
				var iterationHelp = currentProcessList[individual];
				//we may need to 'reprocess' a tree
				if (!individual.In(processedCompletly)) {
					yield return individual;
					processedCompletly.Add(iterationHelp);
				} else {
					IterationHelper oldHelper = processedCompletly[individual];
					//if some how our old helper has a closer relation to our individuals we skip this processing
					// Its most likely a data validation problem.
					if (oldHelper.RelationStep < iterationHelp.RelationStep) {
						continue;
					}
					processedCompletly.Remove(individual);
					processedCompletly.Add(iterationHelp);
				}
				//NewRelationDistance
				int nrd = iterationHelp.RelationStep + 1;
				//If our relation distance is less than the max
				if (nrd <= maxDistance) {
					foreach (var toAdd in individual.AllChildren.Concat(individual.Parents)
						.Where(i => IsMissingOrLower(i, nrd, currentProcessList, processedCompletly))) {
						if (toAdd.In(currentProcessList)) {
							//We need to update
							currentProcessList.Remove(toAdd);
						} else {
							//this individual is not queued
							toProcess.Enqueue(toAdd);
						}
						//Update with the new relation index (we will be processed normally)
						currentProcessList.Add(new IterationHelper { Individual = toAdd, RelationStep = nrd });
					}

				}
			}
		}

		private static bool IsMissingOrLower(Individual indiv, int relation, params KeyedIterationHelper[] toTest) {
			foreach (var set in toTest) {
				if (indiv.In(set)) {
					IterationHelper helper = set[indiv];
					if (helper.RelationStep > relation)
						return true;
				}
			}
			//If we are in any of the sets we are not
			return !toTest.Any(i => indiv.In(i));
		}

	}
}
