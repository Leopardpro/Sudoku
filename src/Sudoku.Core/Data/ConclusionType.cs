﻿namespace Sudoku.Data
{
	/// <summary>
	/// Provides a conclusion type.
	/// </summary>
	[Closed]
	public enum ConclusionType : byte
	{
		/// <summary>
		/// Indicates the conclusion is a value filling into a cell.
		/// </summary>
		Assignment,

		/// <summary>
		/// Indicates the conclusion is a candidate being remove from a cell.
		/// </summary>
		Elimination,
	}
}
