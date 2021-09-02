﻿namespace Sudoku.Solving.Manual.Uniqueness.Rects;

/// <summary>
/// Provides a usage of <b>unique rectangle</b> (UR) or
/// <b>avoidable rectangle</b> (AR) type 3 technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Digit1">The digit 1.</param>
/// <param name="Digit2">The digit 2.</param>
/// <param name="Cells">All cells.</param>
/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
/// <param name="ExtraDigits">All extra digits.</param>
/// <param name="ExtraCells">All extra cells.</param>
/// <param name="Region">The region.</param>
/// <param name="IsNaked">Indicates whether the subset is naked.</param>
///  <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
public sealed record UrType3StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	int Digit1, int Digit2, int[] Cells, bool IsAvoidable, IReadOnlyList<int> ExtraDigits,
	IReadOnlyList<int> ExtraCells, int Region, bool IsNaked, int AbsoluteOffset
) : UrStepInfo(
	Conclusions, Views, IsAvoidable ? Technique.ArType3 : Technique.UrType3,
	Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal Difficulty => BaseDifficulty + SizeExtraDifficulty;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <summary>
	/// Indicates the base difficulty.
	/// </summary>
	private decimal BaseDifficulty => IsNaked ? 4.5M : 4.6M;

	/// <summary>
	/// Indicates the extra difficulty on size.
	/// </summary>
	private decimal SizeExtraDifficulty => .1M * ExtraDigits.Count;

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraDigits).ToString();
	}

	[FormatItem]
	private string OnlyKeyword
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => IsNaked ? string.Empty : "only ";
	}

	[FormatItem]
	private string OnlyKeywordZhCn
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current.Only;
	}

	[FormatItem]
	private string RegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(Region).ToString();
	}

	[FormatItem]
	private string AppearLimitKeyword
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current.Appear;
	}
}