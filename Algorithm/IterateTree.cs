//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
namespace Algorithm {
	/// <summary>
	/// A set of helper methods that define how to iterate a given tree
	/// </summary>
	/*public static class IterateTree
	{

		public static IEnumerable<Individual> Self(this Individual individual)
		{
			yield return individual;
		}

		/// <summary>
		/// Returns the entire family excluding the individual including all relatives
		/// </summary>
		/// <param name="individual">The individual to get the entire family from</param>
		/// <returns>The enumeration of the entire family</returns>
		public static IEnumerable<Individual> EntireFamily(this Individual individual)
		{
			return individual.Ancestors().Concat(individual.Ancestors().SelectMany(i => i.Lineage()).Concat(individual.Lineage())).Distinct();
		}

		/// <summary>
		/// Returns only the direct family of the individual
		/// </summary>
		/// <param name="individual">The individual to get the family of</param>
		/// <returns>The Direct Family in both directions</returns>
		public static IEnumerable<Individual> DirectFamily(Individual individual)
		{
			return individual.Ancestors().Concat(individual.Lineage()).Distinct();
		}

		private static bool AddIfCan(HashSet<Individual> processed, Individual toAdd)
		{
			if (toAdd != null && !processed.Contains(toAdd))
			{
				processed.Add(toAdd);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns th lineage of the individual
		/// </summary>
		/// <param name="individual">The individual who's lineage to iterate</param>
		/// <returns>The individual's lineage</returns>
		public static IEnumerable<Individual> Lineage(this Individual individual)
		{
			var toProcess = new Stack<Individual>();
			var processed = new HashSet<Individual>();
			processed.Add(individual);
			toProcess.Push(individual);
			do
			{
				Individual current = toProcess.Pop();
				foreach (var child in current.AllChildren)
				{
					if (AddIfCan(processed, child))
					{
						yield return child;
						toProcess.Push(child);
					}
				}
			} while (toProcess.Count > 0);
		}
	
		/// <summary>
		/// Returns the siblings of the given individual
		/// </summary>
		/// <param name="individual">The individual to get the siblings of</param>
		/// <returns>The Enumeration of the siblings</returns>
		public static IEnumerable<Individual> Siblings(this Individual individual, bool returnParents = false)
		{
			
			if(individual.Mother != null && individual.Father != null)
			{

				return individual.Mother.ChildrenWith(individual.Father).Concat(individual.Mother.Self()).Concat(individual.Father.Self()).Except(new[] { individual }).Distinct();
			}
			else
			{
				return Enumerable.Empty<Individual>();
			}
		}

		/// <summary>
		/// Returns the ancestors of the individual up to a given generation
		/// </summary>
		/// <param name="individual"></param>
		/// <param name="generations"></param>
		/// <returns></returns>
		public static IEnumerable<Individual> AncestorsUpTo(this Individual individual, int generations)
		{
			var toProcess = new Stack<Tuple<Individual, int>>();
			var processed = new HashSet<Individual>();
			processed.Add(individual);
			toProcess.Push(new Tuple<Individual, int>(individual, generations));
			while (toProcess.Count > 0)
			{
				var current = toProcess.Pop();
				if (AddIfCan(processed, current.Item1.Father))
				{
					yield return current.Item1.Father;
					if(current.Item2 > 0) 
						toProcess.Push(new Tuple<Individual, int>(current.Item1.Father, current.Item2-1));
				}
				if (AddIfCan(processed, current.Item1.Mother))
				{
					yield return current.Item1.Mother;
					if (current.Item2 > 0)
						toProcess.Push(new Tuple<Individual, int>(current.Item1.Mother, current.Item2 - 1));
				}
			}
		}

		/// <summary>
		/// Returns all ancestors of this individual
		/// </summary>
		/// <param name="individual">The individual to get the ancestors of</param>
		/// <returns>The enumeration of all the ancestors</returns>
		public static IEnumerable<Individual> Ancestors(this Individual individual)
		{
			var toProcess = new Stack<Individual>();
			var processed = new HashSet<Individual>();
			processed.Add(individual);
			toProcess.Push(individual);
			do
			{
				//Process mother
				Individual current = toProcess.Pop();
				if (AddIfCan(processed, current.Mother)) {
					yield return current.Mother;
					toProcess.Push(current.Mother);
				}
				//Then father
				if(AddIfCan(processed, current.Father))
				{
					yield return current.Father;
					toProcess.Push(current.Father);
				}
			} while (toProcess.Count > 0);
		}

	}*/
}
