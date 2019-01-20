using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genealogy.Model;

namespace Algorithm.Algorithms {
	public static class FamilyTree {

		/// <summary>
		/// Performs an entire family tree intersection only showing individuals that show up in both family trees. (this may end up excluding the individual themselves)
		/// </summary>
		/// <param name="left">The left half the intersection</param>
		/// <param name="right">The right half of the intersection</param>
		/// <param name="maxDistance">The distance of non-direct lineage to include</param>
		/// <returns>The Intersection of two family trees</returns>
		public static IEnumerable<Individual> Intersection(Individual left, Individual right, int maxDistance = 0) {
			HashSet<Individual> leftSet = new HashSet<Individual>(left.FamilyTree(maxDistance));
			foreach (var test in right.FamilyTree(maxDistance)) {
				if (leftSet.Has(test)) {
					yield return test;
				}
			}
		}

		/// <summary>
		/// Returns all individuals in Lefts family that is not in rights family tree
		/// </summary>
		/// <param name="left">The Family tree to return the values not in right</param>
		/// <param name="right">The Family tree to remove from the Left</param>
		/// <param name="maxDistance">The number of non-direct lineage to include</param>
		/// <returns>All individuals in Left but not Right</returns>
		public static IEnumerable<Individual> Subtract(Individual left, Individual right, int maxDistance = 0) {
			HashSet<Individual> rightSet = new HashSet<Individual>(right.FamilyTree(maxDistance));
			foreach (var test in left.FamilyTree(maxDistance)) {
				if (!rightSet.Has(test)) {
					yield return test;
				}
			}
		}
	}
}
