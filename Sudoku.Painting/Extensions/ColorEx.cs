﻿using System.Drawing;
using Sudoku.DocComments;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Color"/>.
	/// </summary>
	/// <seealso cref="Color"/>
	public static class ColorEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <param name="a">(<see langword="out"/> parameter) The alpha value.</param>
		/// <param name="r">(<see langword="out"/> parameter) The red value.</param>
		/// <param name="g">(<see langword="out"/> parameter) The green value.</param>
		/// <param name="b">(<see langword="out"/> parameter) The blue value.</param>
		public static void Deconstruct(this Color @this, out int a, out int r, out int g, out int b)
		{
			a = @this.A;
			r = @this.R;
			g = @this.G;
			b = @this.B;
		}
	}
}
