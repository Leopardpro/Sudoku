﻿namespace Sudoku.Solving.Manual.Steps.Wings;

/// <summary>
/// Provides with a step that is a <b>W-Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="StartCell">Indicates the start cell.</param>
/// <param name="EndCell">Indicates the end cell.</param>
/// <param name="ConjugatePair">
/// Indicates the conjugate pair that connects cells <see cref="StartCell"/> and <see cref="EndCell"/>.
/// </param>
public sealed record WWingStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int StartCell,
	int EndCell,
	in ConjugatePair ConjugatePair
) : WingStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.4M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.WWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	[FormatItem]
	private string StartCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { StartCell }.ToString();
	}

	[FormatItem]
	private string EndCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { EndCell }.ToString();
	}

	[FormatItem]
	private string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}
}