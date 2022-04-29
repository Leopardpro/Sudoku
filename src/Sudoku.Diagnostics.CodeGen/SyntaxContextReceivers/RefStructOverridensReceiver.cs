﻿namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="RefStructOverridensGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="RefStructOverridensGenerator"/>
internal sealed record class RefStructOverridensReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<INamedTypeSymbol>
{
	/// <inheritdoc/>
	public ICollection<INamedTypeSymbol> Collection { get; } = new List<INamedTypeSymbol>();

	/// <summary>
	/// Indicates the diagnostic results found.
	/// </summary>
	internal List<Diagnostic> Diagnostics { get; } = new();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: StructDeclarationSyntax
				{
					Identifier: var identifier,
					Modifiers: { Count: not 0 } modifiers
				} n,
				SemanticModel: { Compilation: { } compilation } semanticModel
			}
		)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.RefKeyword))
		{
			return;
		}

		if (semanticModel.GetDeclaredSymbol(n, CancellationToken) is not { } typeSymbol)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.PartialKeyword))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0001, identifier.GetLocation(), messageArgs: null));
			return;
		}

		Collection.Add(typeSymbol);
	}
}
