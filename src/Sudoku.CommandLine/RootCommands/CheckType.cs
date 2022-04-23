﻿namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines a check type.
/// </summary>
public enum CheckType
{
	/// <summary>
	/// Indicates the validity.
	/// </summary>
	[SupportedNames("validity", "v")]
	Validity
}