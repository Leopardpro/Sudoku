namespace Sudoku.Solving.Logical;

/// <summary>
/// Defines a step searcher.
/// </summary>
public interface IStepSearcher
{
	/// <summary>
	/// Determines whether the current step searcher is separated one, which mean it can be created
	/// as many possible instances in a same step searchers pool.
	/// </summary>
	sealed bool IsSeparated
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<SeparatedStepSearcherAttribute>() is not null;
	}

	/// <summary>
	/// Determines whether the current step searcher is a direct one.
	/// </summary>
	/// <remarks>
	/// If you don't know what is a direct step searcher, please visit the property
	/// <see cref="StepSearcherMetadataAttribute.IsDirect"/> to learn more information.
	/// </remarks>
	/// <seealso cref="StepSearcherMetadataAttribute.IsDirect"/>
	sealed bool IsDirect
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepSearcherMetadataAttribute>()?.IsDirect ?? false;
	}

	/// <summary>
	/// Determines whether we can adjust the ordering of the current step searcher
	/// as a customized configuration option before solving a puzzle.
	/// </summary>
	/// <remarks>
	/// If you don't know what is a direct step searcher, please visit the property
	/// <see cref="StepSearcherMetadataAttribute.IsOptionsFixed"/> to learn more information.
	/// </remarks>
	/// <seealso cref="StepSearcherMetadataAttribute.IsOptionsFixed"/>
	sealed bool IsOptionsFixed
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepSearcherMetadataAttribute>()?.IsOptionsFixed ?? false;
	}

	/// <summary>
	/// Determines whether the current step searcher is temporarily disabled.
	/// </summary>
	sealed bool IsTemporarilyDisabled
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepSearcherRunningOptionsAttribute>() is { Options: var options }
			&& options.Flags(StepSearcherRunningOptions.TemporarilyDisabled);
	}

	/// <summary>
	/// Determines whether the current step searcher is not supported for sukaku solving mode.
	/// </summary>
	sealed bool IsNotSupportedForSukaku
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepSearcherRunningOptionsAttribute>() is { Options: var options }
			&& options.Flags(StepSearcherRunningOptions.OnlyForStandardSudoku);
	}

	/// <summary>
	/// Determines whether the current step searcher is disabled
	/// by option <see cref="StepSearcherRunningOptions.SlowAlgorithm"/> being configured.
	/// </summary>
	/// <seealso cref="StepSearcherRunningOptions.SlowAlgorithm"/>
	sealed bool IsConfiguredSlow
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GetType().GetCustomAttribute<StepSearcherRunningOptionsAttribute>() is { Options: var options }
			&& options.Flags(StepSearcherRunningOptions.SlowAlgorithm);
	}

	/// <summary>
	/// Indicates the step searching options.
	/// </summary>
	SearcherInitializationOptions Options { get; set; }


	/// <summary>
	/// Accumulate all possible steps into property <see cref="LogicalAnalysisContext.Accumulator"/>
	/// of argument <paramref name="context"/> if property <see cref="LogicalAnalysisContext.OnlyFindOne"/>
	/// is <see langword="false"/>, or return the first found step if <see cref="LogicalAnalysisContext.OnlyFindOne"/>
	/// is <see langword="true"/>.
	/// </summary>
	/// <param name="context">
	/// The analysis context. This argument provides with the data that is used for analysis.
	/// </param>
	/// <returns>
	/// Returns the first found step. The nullability of the return value are as belows:
	/// <list type="bullet">
	/// <item>
	/// <see langword="null"/>:
	/// <list type="bullet">
	/// <item><c><see cref="LogicalAnalysisContext.OnlyFindOne"/> == <see langword="false"/></c>.</item>
	/// <item><c><see cref="LogicalAnalysisContext.OnlyFindOne"/> == <see langword="true"/></c>, but nothing found.</item>
	/// </list>
	/// </item>
	/// <item>
	/// Not <see langword="null"/>:
	/// <list type="bullet">
	/// <item>
	/// <c><see cref="LogicalAnalysisContext.OnlyFindOne"/> == <see langword="true"/></c>,
	/// and found <b>at least one step</b>. In this case the return value is the first found step.
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="LogicalAnalysisContext"/>
	IStep? GetAll(scoped in LogicalAnalysisContext context);
}
