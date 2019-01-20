using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Genealogy.ViewModel {
	/// <summary>
	/// Represents a Color that the Graph Supports
	/// </summary>
	public class ColorsViewModel : IComparable<ColorsViewModel> {

		/// <summary>
		/// An Enumeration of all valid colors
		/// </summary>
		public static readonly IEnumerable<ColorsViewModel> AllColors;

		/// <summary>
		/// Generates an observable collection of all of the colors
		/// </summary>
		/// <returns></returns>
		public static ObservableCollection<ColorsViewModel> GetViewModelList() {
			return new ObservableCollection<ColorsViewModel>(AllColors);
		}

		/// <summary>
		/// The Color value we are wrapping
		/// </summary>
		public Color Wrapped { get; private set; }

		/// <summary>
		/// The WPF supported Color
		/// </summary>
		public System.Windows.Media.SolidColorBrush DisplayColor { get; private set; }

		/// <summary>
		/// A Color struct used to sort colors
		/// </summary>
		public HslColorStruct HslColor { get; }

		public string ColorName { get; }

		/// <summary>
		/// Creates a color view model from a given color
		/// </summary>
		/// <param name="wrapped">The Color Model to wrap</param>
		private ColorsViewModel(Color wrapped, string colorName) {
			Wrapped = wrapped;
			HslColor = RgbToHsl(wrapped);
			DisplayColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(wrapped.G, wrapped.R, wrapped.G, wrapped.B));
			ColorName = colorName;
		}


		public int CompareTo(ColorsViewModel other) {
			Color otherColor = other.Wrapped;
			Color color = Wrapped;
			if (color.A != otherColor.A) {
				return color.A.CompareTo(otherColor.A);
			}
			if (color.R != otherColor.R) {
				return color.R.CompareTo(otherColor.R);
			}
			if (color.G != otherColor.G) {
				return color.G.CompareTo(otherColor.G);
			}
			return color.B.CompareTo(otherColor.B);
		}

		#region Generate the View Model Collection
		/// <summary>
		/// A Static constructor to generate the view model list
		/// </summary>
		static ColorsViewModel() {
			List<ColorsViewModel> items = new List<ColorsViewModel> {
				new ColorsViewModel(Color.MediumSpringGreen, "Medium Spring Green"),
				new ColorsViewModel(Color.MediumTurquoise, "Medium Turquoise"),
				new ColorsViewModel(Color.MediumVioletRed, "Medium Violet Red"),
				new ColorsViewModel(Color.MidnightBlue, "Midnight Blue"),
				new ColorsViewModel(Color.MintCream, "Mint Cream"),
				new ColorsViewModel(Color.MistyRose, "Misty Rose"),
				new ColorsViewModel(Color.Moccasin, "Moccasin"),
				new ColorsViewModel(Color.NavajoWhite, "Navajo White"),
				new ColorsViewModel(Color.Navy, "Navy"),
				new ColorsViewModel(Color.OldLace, "Old Lace"),
				new ColorsViewModel(Color.Olive, "Olive"),
				new ColorsViewModel(Color.OliveDrab, "Olive Drab"),
				new ColorsViewModel(Color.Orange, "Orange"),
				new ColorsViewModel(Color.OrangeRed, "Orange Red"),
				new ColorsViewModel(Color.MediumSlateBlue, "Medium Slate Blue"),
				new ColorsViewModel(Color.MediumSeaGreen, "Medium Sea Green"),
				new ColorsViewModel(Color.MediumPurple, "Medium Purple"),
				new ColorsViewModel(Color.MediumOrchid, "Medium Orchid"),
				new ColorsViewModel(Color.LightGreen, "Light Green"),
				new ColorsViewModel(Color.LightPink, "Light Pink"),
				new ColorsViewModel(Color.LightSalmon, "Light Salmon"),
				new ColorsViewModel(Color.LightSeaGreen, "Light Sea Green"),
				new ColorsViewModel(Color.LightSkyBlue, "Light Sky Blue"),
				new ColorsViewModel(Color.LightSlateGray, "Light Slate Gray"),
				new ColorsViewModel(Color.LightSteelBlue, "Light Steel Blue"),
				new ColorsViewModel(Color.LightYellow, "Light Yellow"),
				new ColorsViewModel(Color.Lime, "Lime"),
				new ColorsViewModel(Color.LimeGreen, "Lime Green"),
				new ColorsViewModel(Color.Linen, "Linen"),
				new ColorsViewModel(Color.Magenta, "Magenta"),
				new ColorsViewModel(Color.Maroon, "Maroon"),
				new ColorsViewModel(Color.MediumAquamarine, "Medium Aquamarine"),
				new ColorsViewModel(Color.MediumBlue, "Medium Blue"),
				new ColorsViewModel(Color.Orchid, "Orchid"),
				new ColorsViewModel(Color.PaleGoldenrod, "Pale Goldenrod"),
				new ColorsViewModel(Color.PaleGreen, "Pale Green"),
				new ColorsViewModel(Color.PaleTurquoise, "Pale Turquoise"),
				new ColorsViewModel(Color.SlateBlue, "Slate Blue"),
				new ColorsViewModel(Color.SlateGray, "Slate Gray"),
				new ColorsViewModel(Color.Snow, "Snow"),
				new ColorsViewModel(Color.SpringGreen, "Spring Green"),
				new ColorsViewModel(Color.SteelBlue, "Steel Blue"),
				new ColorsViewModel(Color.Tan, "Tan"),
				new ColorsViewModel(Color.Teal, "Teal"),
				new ColorsViewModel(Color.Thistle, "Thistle"),
				new ColorsViewModel(Color.Tomato, "Tomato"),
				new ColorsViewModel(Color.Transparent, "Transparent"),
				new ColorsViewModel(Color.Turquoise, "Turquoise"),
				new ColorsViewModel(Color.Violet, "Violet"),
				new ColorsViewModel(Color.Wheat, "Wheat"),
				new ColorsViewModel(Color.White, "White"),
				new ColorsViewModel(Color.WhiteSmoke, "White Smoke"),
				new ColorsViewModel(Color.SkyBlue, "Sky Blue"),
				new ColorsViewModel(Color.LightGray, "Light Gray"),
				new ColorsViewModel(Color.Silver, "Silver"),
				new ColorsViewModel(Color.SeaShell, "Sea Shell"),
				new ColorsViewModel(Color.PaleVioletRed, "Pale Violet Red"),
				new ColorsViewModel(Color.PapayaWhip, "Papaya Whip"),
				new ColorsViewModel(Color.PeachPuff, "Peach Puff"),
				new ColorsViewModel(Color.Peru, "Peru"),
				new ColorsViewModel(Color.Pink, "Pink"),
				new ColorsViewModel(Color.Plum, "Plum"),
				new ColorsViewModel(Color.PowderBlue, "Powder Blue"),
				new ColorsViewModel(Color.Purple, "Purple"),
				new ColorsViewModel(Color.Red, "Red"),
				new ColorsViewModel(Color.RosyBrown, "Rosy Brown"),
				new ColorsViewModel(Color.RoyalBlue, "Royal Blue"),
				new ColorsViewModel(Color.SaddleBrown, "Saddle Brown"),
				new ColorsViewModel(Color.Salmon, "Salmon"),
				new ColorsViewModel(Color.SandyBrown, "Sandy Brown"),
				new ColorsViewModel(Color.SeaGreen, "Sea Green"),
				new ColorsViewModel(Color.Sienna, "Sienna"),
				new ColorsViewModel(Color.LightGoldenrodYellow, "Light Goldenrod Yellow"),
				new ColorsViewModel(Color.LightCyan, "Light Cyan"),
				new ColorsViewModel(Color.LightCoral, "Light Coral"),
				new ColorsViewModel(Color.DarkOrange, "Dark Orange"),
				new ColorsViewModel(Color.DarkOliveGreen, "Dark Olive Green"),
				new ColorsViewModel(Color.Yellow, "Yellow"),
				new ColorsViewModel(Color.DarkKhaki, "Dark Khaki"),
				new ColorsViewModel(Color.DarkGreen, "Dark Green"),
				new ColorsViewModel(Color.DarkGray, "Dark Gray"),
				new ColorsViewModel(Color.DarkGoldenrod, "Dark Goldenrod"),
				new ColorsViewModel(Color.DarkCyan, "Dark Cyan"),
				new ColorsViewModel(Color.DarkBlue, "Dark Blue"),
				new ColorsViewModel(Color.Cyan, "Cyan"),
				new ColorsViewModel(Color.Crimson, "Crimson"),
				new ColorsViewModel(Color.Cornsilk, "Cornsilk"),
				new ColorsViewModel(Color.CornflowerBlue, "Cornflower Blue"),
				new ColorsViewModel(Color.Coral, "Coral"),
				new ColorsViewModel(Color.DarkOrchid, "Dark Orchid"),
				new ColorsViewModel(Color.Chocolate, "Chocolate"),
				new ColorsViewModel(Color.CadetBlue, "Cadet Blue"),
				new ColorsViewModel(Color.BurlyWood, "Burly Wood"),
				new ColorsViewModel(Color.Brown, "Brown"),
				new ColorsViewModel(Color.BlueViolet, "Blue Violet"),
				new ColorsViewModel(Color.Blue, "Blue"),
				new ColorsViewModel(Color.BlanchedAlmond, "Blanched Almond"),
				new ColorsViewModel(Color.Black, "Black"),
				new ColorsViewModel(Color.Bisque, "Bisque"),
				new ColorsViewModel(Color.Beige, "Beige"),
				new ColorsViewModel(Color.Azure, "Azure"),
				new ColorsViewModel(Color.Aquamarine, "Aquamarine"),
				new ColorsViewModel(Color.Aqua, "Aqua"),
				new ColorsViewModel(Color.AntiqueWhite, "Antique White"),
				new ColorsViewModel(Color.AliceBlue, "Alice Blue"),
				new ColorsViewModel(Color.Chartreuse, "Chartreuse"),
				new ColorsViewModel(Color.DarkRed, "Dark Red"),
				new ColorsViewModel(Color.DarkMagenta, "Dark Magenta"),
				new ColorsViewModel(Color.DarkSeaGreen, "Dark Sea Green"),
				new ColorsViewModel(Color.LightBlue, "Light Blue"),
				new ColorsViewModel(Color.LemonChiffon, "Lemon Chiffon"),
				new ColorsViewModel(Color.LawnGreen, "Lawn Green"),
				new ColorsViewModel(Color.LavenderBlush, "Lavender Blush"),
				new ColorsViewModel(Color.Lavender, "Lavender"),
				new ColorsViewModel(Color.Khaki, "Khaki"),
				new ColorsViewModel(Color.Ivory, "Ivory"),
				new ColorsViewModel(Color.Indigo, "Indigo"),
				new ColorsViewModel(Color.IndianRed, "Indian Red"),
				new ColorsViewModel(Color.HotPink, "Hot Pink"),
				new ColorsViewModel(Color.DarkSalmon, "Dark Salmon"),
				new ColorsViewModel(Color.GreenYellow, "Green Yellow"),
				new ColorsViewModel(Color.Green, "Green"),
				new ColorsViewModel(Color.Gray, "Gray"),
				new ColorsViewModel(Color.Goldenrod, "Goldenrod"),
				new ColorsViewModel(Color.Honeydew, "Honeydew"),
				new ColorsViewModel(Color.GhostWhite, "Ghost White"),
				new ColorsViewModel(Color.DarkSlateBlue, "Dark Slate Blue"),
				new ColorsViewModel(Color.DarkSlateGray, "Dark Slate Gray"),
				new ColorsViewModel(Color.Gold, "Gold"),
				new ColorsViewModel(Color.DarkViolet, "Dark Violet"),
				new ColorsViewModel(Color.DeepPink, "Deep Pink"),
				new ColorsViewModel(Color.DeepSkyBlue, "Deep Sky Blue"),
				new ColorsViewModel(Color.DarkTurquoise, "Dark Turquoise"),
				new ColorsViewModel(Color.DodgerBlue, "Dodger Blue"),
				new ColorsViewModel(Color.Firebrick, "Firebrick"),
				new ColorsViewModel(Color.FloralWhite, "Floral White"),
				new ColorsViewModel(Color.ForestGreen, "Forest Green"),
				new ColorsViewModel(Color.Fuchsia, "Fuchsia"),
				new ColorsViewModel(Color.Gainsboro, "Gainsboro"),
				new ColorsViewModel(Color.DimGray, "Dim Gray"),
				new ColorsViewModel(Color.YellowGreen, "Yellow Green")
			};

			//Sort the collection
			AllColors = items.OrderBy(c => c.HslColor.Saturation)
								.OrderBy(c => c.HslColor.Hue)
								.OrderBy(c => c.HslColor.Lightness)
								.OrderBy(c => c.HslColor.Alpha)
								.ToList();
		}

		#endregion

		#region HSL For sorting 
		/// <summary>
		/// Creates the HSLColorStruct to allow for sorting of colors
		/// </summary>
		/// <param name="rgbColor">The color model to generate the HSL Struct</param>
		/// <returns>The HSL Struct from the color</returns>
		private HslColorStruct RgbToHsl(Color rgbColor) {
			// Initialize result
			var hslColor = new HslColorStruct();

			// Convert RGB values to percentages
			double r = (double)rgbColor.R / 255;
			var g = (double)rgbColor.G / 255;
			var b = (double)rgbColor.B / 255;
			var a = (double)rgbColor.A / 255;

			// Find min and max RGB values
			var min = Math.Min(r, Math.Min(g, b));
			var max = Math.Max(r, Math.Max(g, b));
			var delta = max - min;

			/* If max and min are equal, that means we are dealing with 
			 * a shade of gray. So we set H and S to zero, and L to either
			 * max or min (it doesn't matter which), and  then we exit. */

			//Special case: Gray
			if (max == min) {
				hslColor.Hue = 0;
				hslColor.Saturation = 0;
				hslColor.Lightness = max;
				return hslColor;
			}

			/* If we get to this point, we know we don't have a shade of gray. */

			// Set L
			hslColor.Lightness = (min + max) / 2;

			// Set S
			if (hslColor.Lightness < 0.5) {
				hslColor.Saturation = delta / (max + min);
			} else {
				hslColor.Saturation = delta / (2.0 - max - min);
			}

			// Set H
			if (r == max) hslColor.Hue = (g - b) / delta;
			if (g == max) hslColor.Hue = 2.0 + (b - r) / delta;
			if (b == max) hslColor.Hue = 4.0 + (r - g) / delta;
			hslColor.Hue *= 60;
			if (hslColor.Hue < 0) hslColor.Hue = 360;

			// Set A
			hslColor.Alpha = a;

			// Set return value
			return hslColor;


		}

		/// <summary>
		/// Represents a color with HSL
		/// </summary>
		public struct HslColorStruct {
			/// <summary>
			/// The alpha value
			/// </summary>
			public double Alpha;
			/// <summary>
			/// The Hue Value
			/// </summary>
			public double Hue;
			/// <summary>
			/// The Lightness Value
			/// </summary>
			public double Lightness;
			/// <summary>
			/// The Saturation Value
			/// </summary>
			public double Saturation;
		}

		#endregion
	}
}
