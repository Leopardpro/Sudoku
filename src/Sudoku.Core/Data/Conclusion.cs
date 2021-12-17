﻿namespace Sudoku.Data;

/// <summary>
/// Encapsulates a conclusion representation while solving in logic.
/// </summary>
/// <param name="Mask">
/// Indicates the mask that holds the information for the cell, digit and the conclusion type.
/// The bits distribution is like:
/// <code><![CDATA[
/// 16      8       0
/// |-------|-------|
/// |     |---------|
/// |/////|   used  |
/// ]]></code>
/// </param>
/// <remarks>
/// Two <see cref="Conclusion"/>s can be compared with each other. If one of those two is an elimination
/// (i.e. holds the value <see cref="ConclusionType.Elimination"/> as the type), the instance
/// will be greater; if those two hold same conclusion type, but one of those two holds
/// the global index of the candidate position is greater, it is greater.
/// </remarks>
/// <seealso cref="ConclusionType.Elimination"/>
[AutoDeconstructLambda(nameof(ConclusionType), nameof(Candidate))]
[AutoDeconstructLambda(nameof(ConclusionType), nameof(Cell), nameof(Digit))]
[AutoEquality(nameof(ConclusionType), nameof(Cell), nameof(Digit))]
public readonly partial record struct Conclusion(int Mask)
: IComparable<Conclusion>
, IEquatable<Conclusion>
, IValueEquatable<Conclusion>
, IValueComparable<Conclusion>
, IJsonSerializable<Conclusion, Conclusion.JsonConverter>
{
	/// <summary>
	/// Initializes an instance with a conclusion type and a candidate offset.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="candidate">The candidate offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int candidate) : this(((int)type << 1) + candidate)
	{
	}

	/// <summary>
	/// Initializes the <see cref="Conclusion"/> instance via the specified cell, digit and the conclusion type.
	/// </summary>
	/// <param name="type">The conclusion type.</param>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion(ConclusionType type, int cell, int digit) : this(((int)type << 1) + cell * 9 + digit)
	{
	}


	/// <summary>
	/// Indicates the cell.
	/// </summary>
	public int Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public int Candidate
	{
		[LambdaBody]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Mask & ((1 << 10) - 1);
	}

	/// <summary>
	/// The conclusion type to control the action of applying.
	/// If the type is <see cref="ConclusionType.Assignment"/>,
	/// this conclusion will be set value (Set a digit into a cell);
	/// otherwise, a candidate will be removed.
	/// </summary>
	public ConclusionType ConclusionType
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ConclusionType)(Mask >> 10 & 1);
	}


	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTo(ref Grid grid)
	{
		switch (ConclusionType)
		{
			case ConclusionType.Assignment:
			{
				grid[Cell] = Digit;
				break;
			}
			case ConclusionType.Elimination:
			{
				grid[Cell, Digit] = false;
				break;
			}
		}
	}

	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete($"Please use the method '{nameof(ApplyTo)}(ref {nameof(Grid)})' instead.", false)]
	public void ApplyTo(ref SudokuGrid grid)
	{
		switch (ConclusionType)
		{
			case ConclusionType.Assignment:
			{
				grid[Cell] = Digit;
				break;
			}
			case ConclusionType.Elimination:
			{
				grid[Cell, Digit] = false;
				break;
			}
		}
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => ((int)ConclusionType + 1) * (Cell * 9 + Digit);

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conclusion other) => GetHashCode() == other.GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Conclusion other) => GetHashCode() - other.GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"r{Cell / 9 + 1}c{Cell % 9 + 1} {ConclusionType switch
		{
			ConclusionType.Assignment => "=",
			ConclusionType.Elimination => "<>"
		}} {Digit + 1}";

	/// <inheritdoc/>
	bool IValueEquatable<Conclusion>.Equals(in Conclusion other) => GetHashCode() == other.GetHashCode();

	/// <inheritdoc/>
	int IValueComparable<Conclusion>.CompareTo(in Conclusion other) => GetHashCode() - other.GetHashCode();


	/// <inheritdoc/>
	public static bool operator <(Conclusion left, Conclusion right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	public static bool operator <=(Conclusion left, Conclusion right) => left.CompareTo(right) <= 0;

	/// <inheritdoc/>
	public static bool operator >(Conclusion left, Conclusion right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	public static bool operator >=(Conclusion left, Conclusion right) => left.CompareTo(right) >= 0;
}
