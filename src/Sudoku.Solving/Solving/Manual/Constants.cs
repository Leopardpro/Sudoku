﻿namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides with the constants.
/// </summary>
internal static class Constants
{
	/// <summary>
	/// Indicates the invalid first set value
	/// after called <see cref="TrailingZeroCount(int)"/> and <see cref="TrailingZeroCount(uint)"/>.
	/// </summary>
	/// <remarks>
	/// For more details you want to learn about, please visit
	/// <see href="https://github.com/dotnet/runtime/blob/a67d5680186ead0c9afdab7e004389c979d5fc1f/src/libraries/System.Private.CoreLib/src/System/Numerics/BitOperations.cs#L467">this link</see>
	/// to get the inner code.
	/// </remarks>
	/// <seealso cref="TrailingZeroCount(int)"/>
	/// <seealso cref="TrailingZeroCount(uint)"/>
	public const int InvalidFirstSet = 32;
}
