﻿namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Extended;

/// <summary>
/// Provides with a step that is a <b>Extended Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="DigitsMask">Indicates the mask that contains the digits used.</param>
public abstract record ExtendedRectangleStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask
) : DeadlyPatternStep(Conclusions, Views)
{
	/// <summary>
	/// Indicates the type of the step. The value must be between 1 and 4.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public override decimal Difficulty => 4.5M + ((Cells.Count >> 1) - 2) * .1M;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Xr;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => base.TechniqueTags;

	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[FormatItem]
	protected string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	/// <summary>
	/// Indicates the cells string.
	/// </summary>
	[FormatItem]
	protected string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}