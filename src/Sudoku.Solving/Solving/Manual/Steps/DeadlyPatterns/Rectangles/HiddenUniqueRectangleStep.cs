﻿namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;

/// <summary>
/// Provides with a step that is a <b>Hidden Unique Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="ConjugatePairs"><inheritdoc/></param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
public sealed record HiddenUniqueRectangleStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int Digit1,
	int Digit2,
	in Cells Cells,
	bool IsAvoidable,
	ConjugatePair[] ConjugatePairs,
	int AbsoluteOffset
) : UniqueRectangleWithConjugatePairStep(
	Conclusions, Views, IsAvoidable ? Technique.HiddenAr : Technique.HiddenUr,
	Digit1, Digit2, Cells, IsAvoidable, ConjugatePairs, AbsoluteOffset
);