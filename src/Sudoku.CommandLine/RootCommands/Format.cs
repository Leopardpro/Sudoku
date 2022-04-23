﻿namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a format command.
/// </summary>
[Usage("format -g <grid> -f <format>", IsFact = true)]
[Usage($"""format -g "{SampleGrid}" -f 0""", """Formats the specified grid, using the string "0" as the format one, which means the grid only displays the given cells, modifiables are treated as the empty ones, and all empty cells will be displayed as a zero character '0'.""")]
public sealed class Format : IRootCommand
{
	/// <summary>
	/// Indicates the format string.
	/// </summary>
	[Command('f', "format", "Indicates the format string used.")]
	public string FormatString { get; set; } = "0";

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[Command('g', "grid", "Indicates the grid to be formatted.", IsRequired = true)]
	[CommandConverter(typeof(GridConverter))]
	public Grid Grid { get; set; }

	/// <inheritdoc/>
	public static string Name => "format";

	/// <inheritdoc/>
	public static string Description => "To format a sudoku grid using string as the result representation.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "format" };


	/// <inheritdoc/>
	public void Execute()
	{
		string format = FormatString;
		try
		{
			Terminal.WriteLine(
				$"""
				Grid: '{Grid.ToString("0")}'
				Format: '{c(format)}'
				Result: {Grid.ToString(format)}
				"""
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string c(string f) => f is null ? "<null>" : string.IsNullOrWhiteSpace(f) ? "<empty>" : f;
		}
		catch (FormatException)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgFormatIsInvalid);
		}
	}
}