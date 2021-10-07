﻿namespace Sudoku.Solving.Manual.Steps.SingleDigitPatterns;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="Block">Indicates the block that the empty rectangle structure formed.</param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
public sealed record EmptyRectangleStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int Digit,
	int Block,
	in ConjugatePair ConjugatePair
) : SingleDigitPatternStep(Conclusions, Views, Digit)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.6M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.EmptyRectangle;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.EmptyRectangle;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	private string RegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(Block).ToString();
	}

	[FormatItem]
	private string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}
}
