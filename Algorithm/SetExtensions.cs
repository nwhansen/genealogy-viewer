﻿//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Collections.Generic;
using System.Linq;

using Genealogy.Model;

namespace Algorithm {
	/// <summary>
	/// Extensions to the "set" category of classes that are of questionable quality
	/// </summary>
	public static class SetExtensions {

		/// <summary>
		/// Returns if the individual is in the set (in a null safe manor)
		/// </summary>
		/// <param name="individuals">The set to test</param>
		/// <param name="individual">The individual to test</param>
		/// <returns>True if they are in the set and not null</returns>
		internal static bool Has(this ISet<Individual> individuals, Individual individual) {
			if (individual == null) {
				return false;
			}
			return individuals.Contains(individual);
		}

		/// <summary>
		/// Returns if the individual is in the set (in a null safe manor)
		/// </summary>
		/// <param name="individuals">The set to test</param>
		/// <param name="individual">The individual to test</param>
		/// <returns>True if they are in the set and not null</returns>
		internal static bool HasNot(this ISet<Individual> individuals, Individual individual) {
			//Nobody can have null.
			if (individual == null) {
				return false;
			}
			return !individuals.Contains(individual);
		}


		internal static bool In(this Individual individual, params ISet<Individual>[] collections) {
			return collections.All(i => i.Contains(individual));
		}
		internal static bool InAny(this Individual individual, params ISet<Individual>[] collections) {
			return collections.Any(i => i.Contains(individual));
		}

	}
}
