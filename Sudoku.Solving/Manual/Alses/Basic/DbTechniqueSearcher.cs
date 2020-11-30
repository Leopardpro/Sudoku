﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates a <b>death blossom</b> technique.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.DeathBlossom))]
	public sealed class DbTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <summary>
		/// Indicates the max petals to search.
		/// </summary>
		private readonly int _maxPetals;


		/// <summary>
		/// Initialize an instance with the specified information.
		/// </summary>
		/// <param name="allowOverlapping">
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </param>
		/// <param name="alsShowRegions">
		/// Indicates whether all ALSes shows their regions rather than cells.
		/// </param>
		/// <param name="maxPetals">
		/// Indicates the max petals of instance to search.
		/// </param>
		public DbTechniqueSearcher(int maxPetals) => _maxPetals = maxPetals;


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(80)
		{
			IsEnabled = false,
			DisabledReason = DisabledReason.HasBugs
		};


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			var alses = Als.GetAllAlses(grid);
			foreach (int cell in EmptyMap)
			{
				foreach (int digit in grid.GetCandidateMask(cell))
				{
					// Get all ALSes relative to the current candidate.
					var relativeAlses = new List<Als>();
					foreach (var als in alses)
					{
						var appearing = als.Map;
						foreach (int c in als.Map)
						{
							if (grid.Exists(c, digit) is not true)
							{
								appearing.Remove(c);
							}
						}

						if (appearing.PeerIntersection[cell])
						{
							relativeAlses.Add(als);
						}
					}

					// Enumerate all combinations of possible ALSes from the list created just now.
					for (int size = 2, min = Math.Min(relativeAlses.Count, _maxPetals); size <= min; size++)
					{
						foreach (var combination in relativeAlses.ToArray().GetSubsets(size))
						{
							unsafe
							{
								// All regions cannot be the same.
								int region;
								bool flag = false;
								for (int i = 0; i < combination.Length; i++)
								{
									int r = combination[i].Region;
									if (i == 0)
									{
										region = r;
									}
									else if (*&region == r)
									{
										flag = true;
										break;
									}
								}
								if (flag)
								{
									continue;
								}
							}

							// Throw-when-use-out mode.
							var tempGrid = grid;
							tempGrid[cell] = digit;

							// Create the link combination list that all possible link cases are in this
							// huge array.
							// Note that the variable is of type 'int[][]', because ALSes are multiple,
							// and we can extract multiple links (digit 'a' to 'b') in a single ALS.
							int[][] linkCombinations = new int[size][];
							int alsIndex = 0;
							foreach (var als in combination)
							{
								short otherDigitsMask = (short)(als.DigitsMask & ~digit);
								int[] otherDigitsList = new int[otherDigitsMask.PopCount()];
								int otherDigitIndex = 0;
								foreach (int otherDigit in otherDigitsMask)
								{
									otherDigitsList[otherDigitIndex++] = otherDigit;
								}

								linkCombinations[alsIndex++] = otherDigitsList;
							}

							// Enumerate all digit combinations, and one digit always come from one ALS.
							foreach (int[] digitSeries in GetCombinations(linkCombinations))
							{
								// Get each element by the specified index, and remove proper candidates.
								for (int index = 0; index < size; index++)
								{
									int currDigit = digitSeries[index];
									var currAls = combination[index];

									// Find all cells that contain this digit ('currentDigit').
									var cellsContainingThatDigit = currAls.Map;
									foreach (int c in currAls.Map)
									{
										if (tempGrid.Exists(c, currDigit) is not true)
										{
											cellsContainingThatDigit.Remove(c);
										}
									}

									foreach (int c in
										cellsContainingThatDigit.PeerIntersection & DigitMaps[currDigit])
									{
										// Remove that digit.
										tempGrid[c, currDigit] = false;
									}
								}

								// Check the grid.
								// If the grid contains a empty cell that contains no candidates,
								// a basic Death Blossom will be formed.
								bool containNullCell = false;
								int nullCell = -1;
								for (int c = 0; c < 81; c++)
								{
									if (tempGrid.GetCandidateMask(c) == 0)
									{
										containNullCell = true;
										nullCell = c;
										break;
									}
								}
								if (containNullCell)
								{
									// Death Blossom (Basic type) found. Here 'nullCell' is the blossom center.
									// Check full eliminations.
									var cellsFromAllAlsesContainingDigit = GridMap.Empty;
									foreach (var als in combination)
									{
										foreach (int c in als.Map)
										{
											if (grid.Exists(c, digit) is true)
											{
												cellsFromAllAlsesContainingDigit.AddAnyway(c);
											}
										}
									}
									var elimMap = cellsFromAllAlsesContainingDigit.PeerIntersection;

									var alsMappingRelation = new Dictionary<int, Als>();
									//foreach (int d in grid.GetCandidateMask(nullCell))
									//{
									//}

									var candidateOffsets = new List<DrawingInfo>();
									for (int i = 0; i < combination.Length; i++)
									{
										var als = combination[i];
										foreach (int c in als.Map)
										{
											foreach (int d in grid.GetCandidateMask(c))
											{
												int cand = c * 9 + d;
												candidateOffsets.Add(new(d == digitSeries[i] ? -~i : ~i, cand));
											}
										}
									}

									foreach (int d in grid.GetCandidateMask(nullCell))
									{
										candidateOffsets.Add(new(0, nullCell * 9 + d));
									}

									accumulator.Add(
										new DbTechniqueInfo(
											(
												from c in elimMap
												select new Conclusion(ConclusionType.Elimination, c, digit)
											).ToArray(),
											new View[] { new(candidateOffsets) },
											nullCell,
											alsMappingRelation));
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Get all combinations that each sub-array only choose one.
		/// </summary>
		/// <param name="array">The jigsaw array.</param>
		/// <returns>
		/// All combinations that each sub-array choose one. For example, if the array is
		/// <c>{ { 1, 2, 3 }, { 1, 3 }, { 1, 4, 7, 10 } }</c>, all combinations are:
		/// <list type="table">
		/// <item><c>{ 1, 1, 1 }</c>, <c>{ 1, 1, 4 }</c>, <c>{ 1, 1, 7 }</c>, <c>{ 1, 1, 10 }</c>,</item>
		/// <item><c>{ 1, 3, 1 }</c>, <c>{ 1, 3, 4 }</c>, <c>{ 1, 3, 7 }</c>, <c>{ 1, 3, 10 }</c>,</item>
		/// <item><c>{ 2, 1, 1 }</c>, <c>{ 2, 1, 4 }</c>, <c>{ 2, 1, 7 }</c>, <c>{ 2, 1, 10 }</c>,</item>
		/// <item><c>{ 2, 3, 1 }</c>, <c>{ 2, 3, 4 }</c>, <c>{ 2, 3, 7 }</c>, <c>{ 2, 3, 10 }</c>,</item>
		/// <item><c>{ 3, 1, 1 }</c>, <c>{ 3, 1, 4 }</c>, <c>{ 3, 1, 7 }</c>, <c>{ 3, 1, 10 }</c>,</item>
		/// <item><c>{ 3, 3, 1 }</c>, <c>{ 3, 3, 4 }</c>, <c>{ 3, 3, 7 }</c>, <c>{ 3, 3, 10 }</c></item>
		/// </list>
		/// 24 cases in total.
		/// </returns>
		/// <remarks>
		/// Please note that each return values unit (an array) contains the same number of elements
		/// with the whole array.
		/// </remarks>
		[SkipLocalsInit]
		private static IEnumerable<int[]> GetCombinations(int[][] array)
		{
			int length = array.GetLength(0), resultCount = 1;
			var tempArray = (stackalloc int[length]);
			tempArray.Fill(-1);
			for (int i = 0; i < length; i++)
			{
				resultCount *= array[i].Length;
			}

			int[][] result = new int[resultCount][];
			int m = -1, n = -1;
			do
			{
				if (m < length - 1)
				{
					m++;
				}

				ref int value = ref tempArray[m];
				value++;
				if (value > array[m].Length - 1)
				{
					value = -1;
					m -= 2;
				}

				if (m == length - 1)
				{
					n++;
					result[n] = new int[m + 1];
					for (int i = 0; i <= m; i++)
					{
						result[n][i] = array[i][tempArray[i]];
					}
				}
			} while (m >= -1);

			return result;
		}
	}
}
