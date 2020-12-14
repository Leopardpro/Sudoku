﻿using Sudoku.Data;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving.Manual
{
	partial class StepSearcher
	{
		/// <summary>
		/// The empty cells map.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleStepSearcher"/>
		internal static Cells EmptyMap { get; private set; }

		/// <summary>
		/// The bi-value cells map.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleStepSearcher"/>
		internal static Cells BivalueMap { get; private set; }

		/// <summary>
		/// The candidate maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleStepSearcher"/>
		internal static Cells[] CandMaps { get; private set; } = null!;

		/// <summary>
		/// The digit maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		/// <seealso cref="SingleStepSearcher"/>
		internal static Cells[] DigitMaps { get; private set; } = null!;

		/// <summary>
		/// The value maps.
		/// </summary>
		/// <remarks>
		/// This map <b>should</b> be used after <see cref="InitializeMaps"/> called, and you<b>'d better</b>
		/// not use this field on <see cref="SingleStepSearcher"/> instance.
		/// </remarks>
		/// <seealso cref="InitializeMaps(in SudokuGrid)"/>
		internal static Cells[] ValueMaps { get; private set; } = null!;
	}
}
