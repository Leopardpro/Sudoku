﻿#pragma warning disable IDE0005
#pragma warning disable IDE1006

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Diagnostics;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Windows;
using static System.Console;

#if GENERATE_BRUTE_FORCE_TABLE || false
WriteLine("private static readonly int[] TryAndErrorOrder =");
WriteLine("{");
for (int i = 0; i < 9; i++)
{
	Write("    ");
	for (int j = 0; j < 9; j++)
	{
		int index = i * 9 + j, k = Array.IndexOf(BruteForceTechniqueSearcher.TryAndErrorOrder, index);
		Write($"{k,2}");
		if (index != 80)
		{
			Write(", ");
		}
	}
	WriteLine();
}

WriteLine("};");
#endif

#if TEST_FOR_VALUE_GRID || false
var vGrid = ValueGrid.Empty;
var iGrid = Grid.Empty.Clone();
vGrid[0] = 4;
iGrid[0] = 4;
vGrid[1, 3] = true;
vGrid[3, 5] = true;
iGrid[1, 3] = true;
iGrid[3, 5] = true;
WriteLine($"{vGrid:.+:}");
WriteLine($"{iGrid:.+:}");
#endif

#if FILE_COUNTER || true
string root = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

WriteLine(new FileCounter(root, "cs", withBinOrObjDirectory: false).CountUp());
#endif

#if VALUE_HAS_FLAG_TESTER || false
var e = E.Easy | E.Hard;
bool result = e.HasFlag<E>(E.Moderate);
Console.WriteLine(result);

[Flags]
enum E : short
{
	Easy = 1,
	Moderate = 2,
	Hard = 4,
	Insane = 8,
	LastResort = 16
}
#endif

#if RANGE_ITERATION_TESTER || false
var stopwatch = new Stopwatch();
stopwatch.Start();
foreach (int current in 1..2147483647)
{
}
WriteLine($"{"Range iterator",-14}: {stopwatch.Elapsed:hh\\.mm\\.ss\\:ffffff}");
stopwatch.Restart();
for (int i = 1; i < 2147483647; i++)
{
}
stopwatch.Stop();
WriteLine($"{"For loop",-14}: {stopwatch.Elapsed:hh\\.mm\\.ss\\:ffffff}");
#endif