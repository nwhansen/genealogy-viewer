using System.Collections.Generic;
using System.Collections.ObjectModel;
using Genealogy.Model;

namespace Algorithm {
	/// <summary>
	/// A helper class that contains information about the family tree processing
	/// </summary>
	internal class IterationHelper {
		/// <summary>
		/// If this individual is a direct relation
		/// </summary>
		public bool DirectAncestor { get; set; }

		/// <summary>
		/// If this Individual is a direct descendant 
		/// </summary>
		public bool DirectDescendant { get; set; }

		/// <summary>
		/// How many steps in any given direction we are from the target individual
		/// </summary>
		public int RelationStep { get; set; }

		/// <summary>
		/// The Individual to map
		/// </summary>
		public Individual Individual { get; set; }

	}

	internal class KeyedIterationHelper : KeyedCollection<Individual, IterationHelper>, ISet<Individual> {
		public bool IsReadOnly => throw new System.NotImplementedException();

		public bool Add(Individual item) {
			throw new System.NotImplementedException();
		}

		public void CopyTo(Individual[] array, int arrayIndex) {
			throw new System.NotImplementedException();
		}

		public void ExceptWith(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public void IntersectWith(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public bool IsProperSubsetOf(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public bool IsProperSupersetOf(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public bool IsSubsetOf(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public bool IsSupersetOf(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public bool Overlaps(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public bool SetEquals(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public void SymmetricExceptWith(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		public void UnionWith(IEnumerable<Individual> other) {
			throw new System.NotImplementedException();
		}

		protected override Individual GetKeyForItem(IterationHelper item) {
			return item.Individual;
		}



		void ICollection<Individual>.Add(Individual item) {
			throw new System.NotImplementedException();
		}

		IEnumerator<Individual> IEnumerable<Individual>.GetEnumerator() {
			throw new System.NotImplementedException();
		}
	}

}
