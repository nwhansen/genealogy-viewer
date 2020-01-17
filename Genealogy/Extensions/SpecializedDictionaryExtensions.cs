//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System.Collections.Generic;

namespace Genealogy.Extensions {
	public static class SpecializedDictionaryExtensions {
		/// <summary>
		/// Retrieves from a dictionary safely not exploding if null is passed in
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="collection"></param>
		/// <param name="individual"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public static bool TryGetSafe<T, K>(this Dictionary<T, K> collection, T individual, out K node) {
			node = default(K);
			return individual != null && collection.TryGetValue(individual, out node);
		}
	}
}
