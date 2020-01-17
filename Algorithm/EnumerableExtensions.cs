//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.Collections.Generic;

namespace Algorithm {
	/// <summary>
	/// Extending the IEnumerable system with some utility classes I have learned to love from Java
	/// </summary>
	public static class IEnumerableExtensions {

		/// <summary>
		/// Runs a function against all elements in the array used to perform simple actions without a foreach loop
		/// </summary>
		/// <typeparam name="T">The type of items to enumerate</typeparam>
		/// <param name="enumeration">The enumeration to run the action against</param>
		/// <param name="action">The Action to execute</param>
		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
			foreach (var item in enumeration) {
				action(item);
			}
		}

		/// <summary>
		/// Runs a function against all elements in the array used to perform simple actions without a for each loop
		/// </summary>
		/// <typeparam name="T">The type of items to enumerate</typeparam>
		/// <param name="enumeration">The enumeration to run the function against</param>
		/// <param name="function">The Function to execute, its value is ignored in the enumeration</param>
		public static void ForEach<T, K>(this IEnumerable<T> enumeration, Func<T, K> function) {
			foreach (var item in enumeration) {
				function(item);
			}
		}



		/// <summary>
		/// Allows for daisy chaining calls helpful when you want to perform some minor action with a class
		/// </summary>
		/// <typeparam name="T">The enumerable type</typeparam>
		/// <param name="enumeration">The enumeration</param>
		/// <param name="action">The action to execute</param>
		/// <returns>The values passed into the foreach (if you want to change the value use select)</returns>
		public static IEnumerable<T> ForEachAnd<T>(this IEnumerable<T> enumeration, Action<T> action) {
			foreach (var item in enumeration) {
				action(item);
				yield return item;
			}
		}


		/// <summary>
		/// Allows for daisy chaining calls helpful when you want to perform some minor action with a class
		/// </summary>
		/// <typeparam name="T">The enumerable type</typeparam>
		/// <typeparam name="K">The return type of the function (ignored)</typeparam>
		/// <param name="enumeration">The enumeration</param>
		/// <param name="function">The function to execute, its value is ignored in the enumeration</param>
		/// <returns>The values passed into the foreach (if you want to change the value use select)</returns>
		public static IEnumerable<T> ForEachAnd<T, K>(this IEnumerable<T> enumeration, Func<T, K> function) {
			foreach (var item in enumeration) {
				function(item);
				yield return item;
			}
		}
	}
}
