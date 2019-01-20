﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Genealogy.Model;
using Genealogy.UIInteraction;
using Genealogy.ViewModel.Configuration;
using Genealogy.ViewModel.UI;

namespace Genealogy.ViewModel {
	/// <summary>
	/// A set of extensions to make working with the events more tolerable (since the compiler is really new a lot of these are hardly more convienent)
	/// </summary>
	public static class EventHandlerExtensions {
		/// <summary>
		/// Raising the On Property Changed event creating the appropriate event args
		/// </summary>
		/// <param name="handler">The handler</param>
		/// <param name="self">The object raising the event</param>
		/// <param name="propertyName">The property that is being changed</param>
		public static void Notify(this PropertyChangedEventHandler handler, object self, string propertyName) {
			handler?.Invoke(self, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Presents a View Model to the view, used to show without blocking
		/// </summary>
		/// <typeparam name="T">The type of the view model</typeparam>
		/// <param name="presenter">the handler</param>
		/// <param name="self">object sending the event</param>
		/// <param name="vm">The vm to preset</param>
		public static void Present<T>(this EventHandler<PresentEventArgs> presenter, object self, T vm) where T : class {
			Present(presenter, self, vm, false);
		}

		/// <summary>
		/// Presents a view Model to the view, returning the arguments
		/// </summary>
		/// <typeparam name="T">The type of the view model</typeparam>
		/// <param name="presenter">the handler</param>
		/// <param name="self">the object sending the event</param>
		/// <param name="vm">the vm to present</param>
		/// <param name="blocking">if we should block (usually true if you are calling this)</param>
		/// <returns>The Event Arguments generated</returns>
		public static PresentViewModelEventArgs<T> Present<T>(this EventHandler<PresentEventArgs> presenter, object self, T vm, bool blocking) where T : class {
			var args = new PresentViewModelEventArgs<T>(vm, blocking);
			presenter?.Invoke(self, args);
			return args;
		}
		/// <summary>
		/// Presents a view Model to the view, returning the arguments
		/// </summary>
		/// <typeparam name="T">The type of the view model</typeparam>
		/// <param name="presenter">the handler</param>
		/// <param name="self">the object sending the event</param>
		/// <param name="vm">the vm to present</param>
		/// <param name="blocking">if we should block (usually true if you are calling this)</param>
		/// <returns>The Event Arguments generated</returns>
		public static PresentViewModelEventArgs<T> Present<T>(this EventHandler<PresentViewModelEventArgs<T>> presenter, object self, T vm, bool blocking) where T : class {
			var args = new PresentViewModelEventArgs<T>(vm, blocking);
			presenter?.Invoke(self, args);
			return args;
		}

		/// <summary>
		/// Presents a graph view Model to the view, returning the arguments
		/// </summary>
		/// <param name="presenter">the handler</param>
		/// <param name="self">the object sending the event</param>
		/// <param name="hgvm">The highlight configuration vm</param>
		/// <param name="individuals">The model level individuals</param>
		public static void Present(this EventHandler<PresentViewModelEventArgs<GraphViewModel>> presenter, object self, HighlightConfigurationViewModel hgvm, IEnumerable<Individual> individuals) {
			var vm = new GraphViewModel(hgvm.CloneConfiguration(), individuals);
			var args = new PresentViewModelEventArgs<GraphViewModel>(vm, false);
			presenter?.Invoke(self, args);
		}

		/// <summary>
		/// Presents a graph view Model to the view, returning the arguments
		/// </summary>
		/// <param name="presenter">the handler</param>
		/// <param name="self">the object sending the event</param>
		/// <param name="hgvm">The highlight configuration vm</param>
		/// <param name="individuals">The view model level individuals</param>
		public static void Present(this EventHandler<PresentViewModelEventArgs<GraphViewModel>> presenter, object self, HighlightConfigurationViewModel hgvm, IEnumerable<IndividualViewModel> individuals) {
			var vm = new GraphViewModel(hgvm, individuals);
			var args = new PresentViewModelEventArgs<GraphViewModel>(vm, false);
			presenter?.Invoke(self, args);
		}
	}
}