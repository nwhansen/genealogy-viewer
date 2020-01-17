//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Collections.Generic;
using System.Linq;

using Genealogy.Model;

namespace Algorithm {
	/// <summary>
	/// Extension methods to iterate a given family tree
	/// </summary>
	public static partial class FamilyTreeExtension {

		/// <summary>
		/// Generates an Individuals Family tree
		/// </summary>
		/// <param name="individual">The Individual to show the family tree for</param>
		/// <param name="maxDistance">The max distance to show non-direct family members. For example 1 will show siblings and aunts and uncles but not cousins</param>
		/// <param name="returnSelf">If we should return the individual in the enumeration</param>
		/// <returns>The Family Tree of the individual</returns>
		/// <remarks>This will return the individual that is being processed</remarks>
		public static IEnumerable<Individual> FamilyTree(this Individual individual, int maxDistance, bool returnSelf = true) {
			if (maxDistance < 0)
				maxDistance = int.MaxValue;
			Queue<IterationHelper> toProcess = new Queue<IterationHelper>();
			//Surprisingly due to logic, the individual is both their own direct ancestor and descendant. Mostly this avoids complicated first case code
			var processing = new IterationHelper { DirectAncestor = true, RelationStep = 0, Individual = individual, DirectDescendant = true };
			//Queue up the parents and children of thie individual
			individual.AllChildren.ForEach(c => Enqueue(toProcess, processing, c, false, true));
			individual.Parents.ForEach(p => Enqueue(toProcess, processing, p, true, false));
			//The individual we just started with is always processed
			HashSet<Individual> returned = new HashSet<Individual> { individual };
			if (returnSelf) {
				yield return individual;
			}
			do {
				processing = toProcess.Dequeue();
				individual = processing.Individual;
				//There is a scenario where we can discover somebody via a non-direct descendant before
				// we discover them via direct relationships. In this case we will need to process 
				// them not return them in this tree a second time (the tree implies no order, just that they are in the family)
				if (!returned.Has(individual)) {
					yield return individual;
					//Mark them as returned
					returned.Add(individual);
				}
				//We always process direct ancestors
				if (processing.DirectAncestor) {
					//We always enqueue direct parents as we may have processed them via another relation (inbreeding or cousins, may be more related than expected)
					individual.Parents.ForEach(p => Enqueue(toProcess, processing, p, true, false));
					//This will inject direct descendants
					if (processing.RelationStep <= maxDistance) {
						//Add the children if they are "within" max Distance
						individual.AllChildren
							.Where(c => !returned.Has(c))
							.ForEach(c => Enqueue(toProcess, processing, c, false, false));
					}
				} else if (processing.DirectDescendant) {
					//Always process direct descendants
					//Check if the parents should be included
					bool includeParents = processing.RelationStep <= maxDistance;
					foreach (var child in individual.AllChildren) {
						Enqueue(toProcess, processing, child, false, true);
						//Include if not processed
						if (includeParents)
							child.Parents.Where(p => !returned.Has(p))
								.ForEach(p => Enqueue(toProcess, processing, p, false, false));
					}
				} else if (processing.RelationStep <= maxDistance) {
					//Here we only add individuals who are under the limit we have set (and only if we haven't encountered them before)
					individual.Parents.Where(p => !returned.Has(p))
						.ForEach(p => Enqueue(toProcess, processing, p, false, false));
					individual.AllChildren
						.Where(i => !returned.Has(i))
						.ForEach(c => Enqueue(toProcess, processing, c, false, false));
				}
			} while (toProcess.Count > 0);
		}

		/// <summary>
		/// Returns all ancestors of this individual
		/// </summary>
		/// <param name="individual">The individual to get the ancestors for</param>
		/// <param name="returnSelf">If we should return the individual in the enumeration</param>
		/// <returns>The individuals ancestors</returns>
		public static IEnumerable<Individual> Ancestors(this Individual individual, bool returnSelf = true) {
			HashSet<Individual> returned = new HashSet<Individual>();
			if (!returnSelf) {
				returned.Add(individual);
			}
			Queue<Individual> toProcess = new Queue<Individual>();
			toProcess.Enqueue(individual);
			do {
				var current = toProcess.Dequeue();
				if (!returned.Has(current))
					yield return current;
				current.Parents.ForEach(toProcess.Enqueue);
			} while (toProcess.Count > 0);
		}

		/// <summary>
		/// Returns all ancestor of this individual up to generations up
		/// </summary>
		/// <param name="individual">The individual to get the ancestors of</param>
		/// <param name="generations">The Number of generations upwards to include</param>
		/// <param name="returnSelf">If we should return the individual in the enumeration</param>
		/// <returns>The indivduals ancestors up to n generations</returns>
		public static IEnumerable<Individual> Ancestors(this Individual individual, int generations, bool returnSelf = true) {
			HashSet<Individual> returned = new HashSet<Individual>();
			if (!returnSelf) {
				returned.Add(individual);
			}
			Queue<IterationHelper> toProcess = new Queue<IterationHelper>();
			toProcess.Enqueue(new IterationHelper { Individual = individual, RelationStep = 0 });
			do {
				var current = toProcess.Dequeue();
				if (!returned.Has(current.Individual))
					yield return current.Individual;
				if (current.RelationStep <= generations)
					current.Individual.Parents.ForEach(i => Enqueue(toProcess, current, i, true, false));
			} while (toProcess.Count > 0);
		}
		/// <summary>
		/// Returns all ancestors of this individual
		/// </summary>
		/// <param name="individual">The individual to get the ancestors for</param>
		/// <param name="returnSelf">If we should return the individual in the enumeration</param>
		/// <returns>The individuals ancestors</returns>
		internal static IEnumerable<IterationHelper> AncestorsHelper(this Individual individual, bool returnSelf = true) {
			HashSet<Individual> returned = new HashSet<Individual>();
			if (!returnSelf) {
				returned.Add(individual);
			}
			Queue<IterationHelper> toProcess = new Queue<IterationHelper>();
			toProcess.Enqueue(new IterationHelper { Individual = individual, RelationStep = 0 });
			do {
				var current = toProcess.Dequeue();
				if (!returned.Has(current.Individual))
					yield return current;
				current.Individual.Parents.ForEach(i => Enqueue(toProcess, current, i, true, false));
			} while (toProcess.Count > 0);
		}

		/// <summary>
		/// Returns all ancestor of this individual up to generations up to max 
		/// </summary>
		/// <param name="individual">The individual to get the ancestors of</param>
		/// <param name="generations">The Number of generations upwards to include</param>
		/// <param name="returnSelf">If we should return the individual in the enumeration</param>
		/// <returns>The indivduals ancestors up to n generations</returns>
		internal static IEnumerable<IterationHelper> AncestorsHelper(this Individual individual, int generations, bool returnSelf = true) {
			HashSet<Individual> returned = new HashSet<Individual>();
			if (!returnSelf) {
				returned.Add(individual);
			}
			Queue<IterationHelper> toProcess = new Queue<IterationHelper>();
			toProcess.Enqueue(new IterationHelper { Individual = individual, RelationStep = 0 });
			do {
				var current = toProcess.Dequeue();
				if (!returned.Has(current.Individual))
					yield return current;
				if (current.RelationStep <= generations)
					current.Individual.Parents.ForEach(i => Enqueue(toProcess, current, i, true, false));
			} while (toProcess.Count > 0);
		}
		/// <summary>
		/// Returns all Descendants of an individual
		/// </summary>
		/// <param name="individual">The individual to get the descendants for</param>
		/// <param name="returnSelf">If we should return the individual in the enumeration</param>
		/// <returns>All descendants of an individual</returns>
		internal static IEnumerable<IterationHelper> DescendantsHelper(this Individual individual, bool returnSelf = true) {
			HashSet<Individual> returned = new HashSet<Individual>();
			if (!returnSelf) {
				returned.Add(individual);
			}
			Queue<IterationHelper> toProcess = new Queue<IterationHelper>();
			toProcess.Enqueue(new IterationHelper { Individual = individual, RelationStep = 0 });
			do {
				var current = toProcess.Dequeue();
				if (!returned.Has(current.Individual))
					yield return current;
				current.Individual.AllChildren.ForEach(i => Enqueue(toProcess, current, i, false, true));
			} while (toProcess.Count > 0);
		}
		/// <summary>
		/// Returns all Descendants of an individual
		/// </summary>
		/// <param name="individual">The individual to get the descendants for</param>
		/// <param name="returnSelf">If we should return the individual in the enumeration</param>
		/// <returns>All descendants of an individual</returns>
		public static IEnumerable<Individual> Descendants(this Individual individual, bool returnSelf = true) {
			HashSet<Individual> returned = new HashSet<Individual>();
			if (!returnSelf) {
				returned.Add(individual);
			}
			Queue<Individual> toProcess = new Queue<Individual>();
			toProcess.Enqueue(individual);
			do {
				var current = toProcess.Dequeue();
				if (!returned.Has(current))
					yield return current;
				current.AllChildren.ForEach(toProcess.Enqueue);
			} while (toProcess.Count > 0);
		}

		private static void Enqueue(Queue<IterationHelper> queue, IterationHelper prior, Individual individual, bool directAncestor, bool directDescendant) {

			queue.Enqueue(new IterationHelper { DirectAncestor = directAncestor, RelationStep = prior.RelationStep + 1, Individual = individual, DirectDescendant = directDescendant });
		}
	}
}
