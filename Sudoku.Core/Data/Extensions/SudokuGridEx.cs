﻿using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SudokuGrid"/>.
	/// </summary>
	/// <seealso cref="SudokuGrid"/>
	public static class SudokuGridEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="empty">(<see langword="out"/> parameter) The map of all empty cells.</param>
		/// <param name="bivalue">(<see langword="out"/> parameter) The map of all bi-value cells.</param>
		/// <param name="candidates">
		/// (<see langword="out"/> parameter) The map of all cells that contain the candidate of that digit.
		/// </param>
		/// <param name="digits">
		/// (<see langword="out"/> parameter) The map of all cells that contain the candidate of that digit
		/// or that value in given or modifiable.
		/// </param>
		/// <param name="values">
		/// (<see langword="out"/> parameter) The map of all cells that is the given or modifiable value,
		/// and the digit is the specified one.
		/// </param>
		public static void Deconstruct(
			this in SudokuGrid @this, out Cells empty, out Cells bivalue,
			out Cells[] candidates, out Cells[] digits, out Cells[] values)
		{
			empty = e(@this);
			bivalue = b(@this);
			candidates = c(@this);
			digits = d(@this);
			values = v(@this);

			static Cells e(in SudokuGrid @this)
			{
				var result = Cells.Empty;
				for (int cell = 0; cell < 81; cell++)
				{
					if (@this.GetStatus(cell) == Empty)
					{
						result.AddAnyway(cell);
					}
				}

				return result;
			}

			static Cells b(in SudokuGrid @this)
			{
				var result = Cells.Empty;
				for (int cell = 0; cell < 81; cell++)
				{
					if (@this.GetCandidateMask(cell).PopCount() == 2)
					{
						result.AddAnyway(cell);
					}
				}

				return result;
			}

			static Cells[] c(in SudokuGrid @this)
			{
				var result = new Cells[9];
				for (int digit = 0; digit < 9; digit++)
				{
					ref var map = ref result[digit];
					for (int cell = 0; cell < 81; cell++)
					{
						if (@this.Exists(cell, digit) is true)
						{
							map.AddAnyway(cell);
						}
					}
				}

				return result;
			}

			static Cells[] d(in SudokuGrid @this)
			{
				var result = new Cells[9];
				for (int digit = 0; digit < 9; digit++)
				{
					ref var map = ref result[digit];
					for (int cell = 0; cell < 81; cell++)
					{
						if ((@this.GetCandidateMask(cell) >> digit & 1) != 0)
						{
							map.AddAnyway(cell);
						}
					}
				}

				return result;
			}

			static Cells[] v(in SudokuGrid @this)
			{
				var result = new Cells[9];
				for (int digit = 0; digit < 9; digit++)
				{
					ref var map = ref result[digit];
					for (int cell = 0; cell < 81; cell++)
					{
						if (@this[cell] == digit)
						{
							map.AddAnyway(cell);
						}
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Indicates whether the specified grid contains the digit in the specified cell.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="cell">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>
		/// The method will return a <see cref="bool"/>? value (contains three possible cases:
		/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
		/// All values corresponding to the cases are below:
		/// <list type="table">
		/// <item>
		/// <term><c><see langword="true"/></c></term>
		/// <description>
		/// The cell is an empty cell <b>and</b> contains the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c><see langword="false"/></c></term>
		/// <description>
		/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c><see langword="null"/></c></term>
		/// <description>The cell is <b>not</b> an empty cell.</description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// Note that the method will return a <see cref="bool"/>?, so you should use the code
		/// '<c>grid.Exists(candidate) is true</c>' or '<c>grid.Exists(candidate) == true</c>'
		/// to decide whether a condition is true.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool? Exists(this in SudokuGrid @this, int cell, int digit) =>
			@this.GetStatus(cell) == Empty ? @this[cell, digit] : null;

		/// <summary>
		/// Check whether the digit will be duplicate of its peers when it is filled in the specified cell.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The grid.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool Duplicate(this in SudokuGrid @this, int cell, int digit)
		{
			static bool duplicate(int c, in SudokuGrid grid, in int digit) => grid[c] == digit;
			unsafe
			{
				return PeerMaps[cell].Any(&duplicate, @this, digit);
			}
		}
	}
}
