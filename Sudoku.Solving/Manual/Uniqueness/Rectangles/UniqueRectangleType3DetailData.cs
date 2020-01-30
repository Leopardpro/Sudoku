﻿using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Indicates the detail data of UR type 3.
	/// </summary>
	public sealed class UniqueRectangleType3DetailData : UniqueRectangleDetailData
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="cells">All cells.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="subsetDigits">All subset digits.</param>
		/// <param name="subsetCells">All subset cells.</param>
		/// <param name="isNaked">Indicates whether the subset is naked or not.</param>
		public UniqueRectangleType3DetailData(
			int[] cells, int[] digits, int[] subsetDigits, int[] subsetCells, bool isNaked)
			: base(cells, digits) =>
			(SubsetDigits, SubsetCells, IsNaked) = (subsetDigits, subsetCells, isNaked);


		/// <inheritdoc/>
		public override int Type => 3;

		/// <summary>
		/// Indicates all subset digits in this pattern.
		/// </summary>
		public int[] SubsetDigits { get; }

		/// <summary>
		/// Indicates all subset cells in this pattern.
		/// </summary>
		public int[] SubsetCells { get; }

		/// <summary>
		/// Indicates whether this subset is naked or not.
		/// </summary>
		public bool IsNaked { get; }


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = CellCollection.ToString(Cells);
			string digitsStr = DigitCollection.ToString(Digits);
			string subsetDigitsStr = DigitCollection.ToString(SubsetDigits);
			string subsetCellsStr = CellCollection.ToString(SubsetCells);
			return $"{digitsStr} in cells {cellsStr} with digits {subsetDigitsStr} in cells {subsetCellsStr}";
		}
	}
}
