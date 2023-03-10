namespace Sudoku.Solving.Logical.Steps.Specialized;

/// <summary>
/// Defines a chain step.
/// </summary>
public interface IChainStep : IStep
{
	/// <summary>
	/// Indicates the flat complexity.
	/// </summary>
	int FlatComplexity { get; }

	/// <summary>
	/// Indicates the sort key. The result will be a <see cref="ChainTypeCode"/> instance to describe
	/// what and how the chain behaves. Please visit the type <see cref="ChainTypeCode"/>
	/// to learn about more details for a chain.
	/// </summary>
	/// <seealso cref="ChainTypeCode"/>
	ChainTypeCode SortKey { get; }


	/// <summary>
	/// Determines whether two different list of <see cref="Conclusion"/>s holds the same values.
	/// </summary>
	/// <param name="lConclusions">The first conclusion list to compare.</param>
	/// <param name="rConclusions">The second conclusion list to compare.</param>
	/// <param name="shouldSort">Indicates whether the method will sort the lists firstly.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	protected static bool ConclusionsEquals(ConclusionList lConclusions, ConclusionList rConclusions, bool shouldSort)
	{
		if (lConclusions.Length != rConclusions.Length)
		{
			return false;
		}

		// Sort the array.
		if (shouldSort)
		{
			var lc = lConclusions.Sort();
			var rc = rConclusions.Sort();
			for (var i = 0; i < lc.Length; i++)
			{
				if (lc[i] != rc[i])
				{
					return false;
				}
			}

			return true;
		}
		else
		{
			for (var i = 0; i < lConclusions.Length; i++)
			{
				if (lConclusions[i] != rConclusions[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}
