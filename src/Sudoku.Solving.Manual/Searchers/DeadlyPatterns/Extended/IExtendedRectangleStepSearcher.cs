﻿namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Extended Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Extended Rectangle Type 1</item>
/// <item>Extended Rectangle Type 2</item>
/// <item>Extended Rectangle Type 3</item>
/// <item>Extended Rectangle Type 4</item>
/// </list>
/// </summary>
public interface IExtendedRectangleStepSearcher : IDeadlyPatternStepSearcher
{
	/// <summary>
	/// Indicates all possible extended rectangle pattern combinations.
	/// </summary>
	/// <remarks>
	/// <para>The list contains two types of <b>Extended Rectangle</b>s:</para>
	/// <para>
	/// Fit type (2 blocks spanned):
	/// <code><![CDATA[
	/// ab | ab
	/// bc | bc
	/// ac | ac
	/// ]]></code>
	/// </para>
	/// <para>
	/// Fat type (3 blocks spanned):
	/// <code><![CDATA[
	/// ab | ac | bc
	/// ab | ac | bc
	/// ]]></code>
	/// </para>
	/// </remarks>
	protected static readonly IReadOnlyList<(CellMap Cells, IReadOnlyList<(int Left, int Right)> PairCells, int Size)> PatternInfos;


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static IExtendedRectangleStepSearcher()
	{
		int[,] houses =
		{
			{ 9, 10 }, { 9, 11 }, { 10, 11 },
			{ 12, 13 }, { 12, 14 }, { 13, 14 },
			{ 15, 16 }, { 15, 17 }, { 16, 17 },
			{ 18, 19 }, { 18, 20 }, { 19, 20 },
			{ 21, 22 }, { 21, 23 }, { 22, 23 },
			{ 24, 25 }, { 24, 26 }, { 25, 26 }
		},
		fitTableRow =
		{
			{ 0, 3 }, { 0, 4 }, { 0, 5 }, { 0, 6 }, { 0, 7 }, { 0, 8 },
			{ 1, 3 }, { 1, 4 }, { 1, 5 }, { 1, 6 }, { 1, 7 }, { 1, 8 },
			{ 2, 3 }, { 2, 4 }, { 2, 5 }, { 2, 6 }, { 2, 7 }, { 2, 8 },
			{ 3, 6 }, { 3, 7 }, { 3, 8 },
			{ 4, 6 }, { 4, 7 }, { 4, 8 },
			{ 5, 6 }, { 5, 7 }, { 5, 8 }
		},
		fitTableColumn =
		{
			{ 0, 27 }, { 0, 36 }, { 0, 45 }, { 0, 54 }, { 0, 63 }, { 0, 72 },
			{ 9, 27 }, { 9, 36 }, { 9, 45 }, { 9, 54 }, { 9, 63 }, { 9, 72 },
			{ 18, 27 }, { 18, 36 }, { 18, 45 }, { 18, 54 }, { 18, 63 }, { 18, 72 },
			{ 27, 54 }, { 27, 63 }, { 27, 72 },
			{ 36, 54 }, { 36, 63 }, { 36, 72 },
			{ 45, 54 }, { 45, 63 }, { 45, 72 }
		};

		var combinations = new List<(CellMap, IReadOnlyList<(int, int)>, int)>();

		// Initializes fit types.
		for (int j = 0; j < 3; j++)
		{
			for (int i = 0, length = fitTableRow.Length >> 1; i < length; i++)
			{
				int c11 = fitTableRow[i, 0] + j * 27, c21 = fitTableRow[i, 1] + j * 27;
				int c12 = c11 + 9, c22 = c21 + 9;
				int c13 = c11 + 18, c23 = c21 + 18;
				combinations.Add(
					(
						CellMap.Empty + c11 + c12 + c13 + c21 + c22 + c23,
						new[] { (c11, c21), (c12, c22), (c13, c23) },
						3
					)
				);
			}
		}
		for (int j = 0; j < 3; j++)
		{
			for (int i = 0, length = fitTableColumn.Length >> 1; i < length; i++)
			{
				int c11 = fitTableColumn[i, 0] + j * 3, c21 = fitTableColumn[i, 1] + j * 3;
				int c12 = c11 + 1, c22 = c21 + 1;
				int c13 = c11 + 2, c23 = c21 + 2;
				combinations.Add(
					(
						CellMap.Empty + c11 + c12 + c13 + c21 + c22 + c23,
						new[] { (c11, c21), (c12, c22), (c13, c23) },
						3
					)
				);
			}
		}

		// Initializes fat types.
		for (int size = 3; size <= 7; size++)
		{
			for (int i = 0, length = houses.Length >> 1; i < length; i++)
			{
				int house1 = houses[i, 0], house2 = houses[i, 1];
				foreach (short mask in new BitSubsetsGenerator(9, size))
				{
					// Check whether all cells are in same house.
					// If so, continue the loop immediately.
					if (size == 3 && (mask >> 6 == 7 || (mask >> 3 & 7) == 7 || (mask & 7) == 7))
					{
						continue;
					}

					var map = CellMap.Empty;
					var pairs = new List<(int, int)>();
					foreach (int pos in mask)
					{
						int cell1 = HouseCells[house1][pos], cell2 = HouseCells[house2][pos];
						map.Add(cell1);
						map.Add(cell2);
						pairs.Add((cell1, cell2));
					}

					combinations.Add((map, pairs, size));
				}
			}
		}

		PatternInfos = combinations;
	}
}

[StepSearcher]
[SukakuNotSupported]
internal sealed unsafe partial class ExtendedRectangleStepSearcher : IExtendedRectangleStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		foreach (var (allCellsMap, pairs, size) in IExtendedRectangleStepSearcher.PatternInfos)
		{
			if ((EmptyCells & allCellsMap) != allCellsMap)
			{
				continue;
			}

			// Check each pair.
			// Ensures all pairs should contains same digits
			// and the kind of digits must be greater than 2.
			bool checkKindsFlag = true;
			foreach (var (l, r) in pairs)
			{
				short tempMask = (short)(grid.GetCandidates(l) & grid.GetCandidates(r));
				if (tempMask == 0 || (tempMask & tempMask - 1) == 0)
				{
					checkKindsFlag = false;
					break;
				}
			}
			if (!checkKindsFlag)
			{
				// Failed to check.
				continue;
			}

			// Check the mask of cells from two houses.
			short m1 = 0, m2 = 0;
			foreach (var (l, r) in pairs)
			{
				m1 |= grid.GetCandidates(l);
				m2 |= grid.GetCandidates(r);
			}

			short resultMask = (short)(m1 | m2);
			short normalDigits = 0, extraDigits = 0;
			foreach (int digit in resultMask)
			{
				int count = 0;
				foreach (var (l, r) in pairs)
				{
					if (((grid.GetCandidates(l) & grid.GetCandidates(r)) >> digit & 1) != 0)
					{
						// Both two cells contain same digit.
						count++;
					}
				}

				(count >= 2 ? ref normalDigits : ref extraDigits) |= (short)(1 << digit);
			}

			if (PopCount((uint)normalDigits) != size)
			{
				// The number of normal digits are not enough.
				continue;
			}

			if (PopCount((uint)resultMask) == size + 1)
			{
				// Possible type 1 or 2 found.
				// Now check extra cells.
				int extraDigit = TrailingZeroCount(extraDigits);
				var extraCellsMap = allCellsMap & CandidatesMap[extraDigit];
				if (!extraCellsMap)
				{
					continue;
				}

				if (extraCellsMap.Count == 1)
				{
					if (CheckType1(accumulator, grid, allCellsMap, extraCellsMap, normalDigits, extraDigit, onlyFindOne) is { } step1)
					{
						return step1;
					}
				}

				if (CheckType2(accumulator, grid, allCellsMap, extraCellsMap, normalDigits, extraDigit, onlyFindOne) is { } step2)
				{
					return step2;
				}
			}
			else
			{
				var extraCellsMap = CellMap.Empty;
				foreach (int cell in allCellsMap)
				{
					foreach (int digit in extraDigits)
					{
						if (grid[cell, digit])
						{
							extraCellsMap.Add(cell);
							break;
						}
					}
				}

				if (!extraCellsMap.InOneHouse)
				{
					continue;
				}

				if (CheckType3Naked(accumulator, grid, allCellsMap, normalDigits, extraDigits, extraCellsMap, onlyFindOne) is { } step3)
				{
					return step3;
				}

				if (CheckType14(accumulator, grid, allCellsMap, normalDigits, extraCellsMap, onlyFindOne) is { } step14)
				{
					return step14;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="extraCells">The extra cells map.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigit">The extra digit.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private IStep? CheckType1(
		ICollection<IStep> accumulator, scoped in Grid grid, scoped in CellMap allCellsMap,
		scoped in CellMap extraCells, short normalDigits, int extraDigit, bool onlyFindOne)
	{
		var (conclusions, candidateOffsets) = (new List<Conclusion>(), new List<CandidateViewNode>());
		foreach (int cell in allCellsMap)
		{
			if (cell == extraCells[0])
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					if (digit != extraDigit)
					{
						conclusions.Add(new(Elimination, cell, digit));
					}
				}
			}
			else
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
				}
			}
		}

		if (conclusions.Count == 0)
		{
			goto ReturnNull;
		}

		var step = new ExtendedRectangleType1Step(
			ImmutableArray.CreateRange(conclusions),
			ImmutableArray.Create(View.Empty | candidateOffsets),
			allCellsMap,
			normalDigits
		);

		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="extraCells">The extra cells map.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigit">The extra digit.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private IStep? CheckType2(
		ICollection<IStep> accumulator, scoped in Grid grid, scoped in CellMap allCellsMap,
		scoped in CellMap extraCells, short normalDigits, int extraDigit, bool onlyFindOne)
	{
		if ((+extraCells & CandidatesMap[extraDigit]) is not (var elimMap and not []))
		{
			goto ReturnNull;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int cell in allCellsMap)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + digit)
				);
			}
		}

		var step = new ExtendedRectangleType2Step(
			from cell in elimMap select new Conclusion(Elimination, cell, extraDigit),
			ImmutableArray.Create(View.Empty | candidateOffsets),
			allCellsMap,
			normalDigits,
			extraDigit
		);
		if (onlyFindOne)
		{
			return step;
		}

		accumulator.Add(step);

	ReturnNull:
		return null;
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraDigits">The extra digits mask.</param>
	/// <param name="extraCellsMap">The map of extra cells.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private IStep? CheckType3Naked(
		ICollection<IStep> accumulator, scoped in Grid grid, scoped in CellMap allCellsMap,
		short normalDigits, short extraDigits, scoped in CellMap extraCellsMap, bool onlyFindOne)
	{
		foreach (int houseIndex in extraCellsMap.CoveredHouses)
		{
			var otherCells = (HousesMap[houseIndex] & EmptyCells) - allCellsMap;
			for (int size = 1, length = otherCells.Count; size < length; size++)
			{
				foreach (var cells in otherCells & size)
				{
					short mask = grid.GetDigitsUnion(cells);
					if ((mask & extraDigits) != extraDigits || PopCount((uint)mask) != size + 1)
					{
						continue;
					}

					var elimMap = (HousesMap[houseIndex] & EmptyCells) - allCellsMap - cells;
					if (!elimMap)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int digit in mask)
					{
						foreach (int cell in elimMap & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int cell in allCellsMap - extraCellsMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}
					foreach (int cell in extraCellsMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									(mask >> digit & 1) != 0 ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
									cell * 9 + digit
								)
							);
						}
					}
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new ExtendedRectangleType3Step(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(View.Empty | candidateOffsets | new HouseViewNode(0, houseIndex)),
						allCellsMap,
						normalDigits,
						cells,
						mask,
						houseIndex
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Check type 4 and a part of type 1 that the method
	/// <see cref="CheckType1(ICollection{IStep}, in Grid, in CellMap, in CellMap, short, int, bool)"/>
	/// cannot be found.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="allCellsMap">The map of all cells used.</param>
	/// <param name="normalDigits">The normal digits mask.</param>
	/// <param name="extraCellsMap">The map of extra cells.</param>
	/// <param name="onlyFindOne">Indicates whether the searcher only searches for one step.</param>
	/// <returns>The first found step if worth.</returns>
	private IStep? CheckType14(
		ICollection<IStep> accumulator, scoped in Grid grid, scoped in CellMap allCellsMap,
		short normalDigits, scoped in CellMap extraCellsMap, bool onlyFindOne)
	{
		switch (extraCellsMap)
		{
			case [var extraCell]:
			{
				// Type 1 found.
				// Check eliminations.
				var conclusions = new List<Conclusion>();
				foreach (int digit in normalDigits)
				{
					if (CandidatesMap[digit].Contains(extraCell))
					{
						conclusions.Add(new(Elimination, extraCell, digit));
					}
				}

				if (conclusions.Count == 0)
				{
					goto ReturnNull;
				}

				// Gather all highlight candidates.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (int cell in allCellsMap)
				{
					if (cell == extraCell)
					{
						continue;
					}

					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					}
				}

				var step = new ExtendedRectangleType1Step(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(View.Empty | candidateOffsets),
					allCellsMap,
					normalDigits
				);

				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);

				break;
			}
			case [var extraCell1, var extraCell2]:
			{
				// Type 4.
				short m1 = grid.GetCandidates(extraCell1), m2 = grid.GetCandidates(extraCell2);
				short conjugateMask = (short)(m1 & m2 & normalDigits);
				if (conjugateMask == 0)
				{
					goto ReturnNull;
				}

				foreach (int conjugateDigit in conjugateMask)
				{
					foreach (int houseIndex in extraCellsMap.CoveredHouses)
					{
						var map = HousesMap[houseIndex] & extraCellsMap;
						if (map != extraCellsMap || map != (CandidatesMap[conjugateDigit] & HousesMap[houseIndex]))
						{
							continue;
						}

						short elimDigits = (short)(normalDigits & ~(1 << conjugateDigit));
						var conclusions = new List<Conclusion>();
						foreach (int digit in elimDigits)
						{
							foreach (int cell in extraCellsMap & CandidatesMap[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						foreach (int cell in allCellsMap - extraCellsMap)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
							}
						}
						foreach (int cell in extraCellsMap)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + conjugateDigit));
						}

						var step = new ExtendedRectangleType4Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(View.Empty | candidateOffsets | new HouseViewNode(0, houseIndex)),
							allCellsMap,
							normalDigits,
							new(extraCellsMap, conjugateDigit)
						);

						if (onlyFindOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}

				break;
			}
		}

	ReturnNull:
		return null;
	}
}
