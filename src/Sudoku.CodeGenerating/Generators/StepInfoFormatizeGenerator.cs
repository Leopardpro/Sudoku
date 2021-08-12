﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;
using static Sudoku.CodeGenerating.Constants;

namespace Sudoku.CodeGenerating.Generators
{
	/// <summary>
	/// Indicates the source generator that generates the code about the attributes to mark onto
	/// the method <c>Formatize</c> in the type <c>StepInfo</c>.
	/// </summary>
	[Generator]
	public sealed class StepInfoFormatizeGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.IsNotInProject(ProjectNames.Solving))
			{
				return;
			}

			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			var compilation = context.Compilation;
			var symbol = compilation.GetTypeByMetadataName("Sudoku.Solving.Manual.StepInfo");
			var attributeSymbol = compilation.GetTypeByMetadataName("Sudoku.Solving.Text.FormatItemAttribute");

			string attributes = string.Join(
				"\r\n\t\t",
				from INamedTypeSymbol type in compilation.GetSymbolsWithName(static _ => true, SymbolFilter.Type, context.CancellationToken)
				where type is { IsAbstract: false, IsGenericType: false }
				let baseType = type.GetBaseTypes()
				where baseType.Any(baseType => f(baseType, symbol))
				let properties = type.GetMembers().OfType<IPropertySymbol>()
				let fullName = type.ToDisplayString(FormatOptions.TypeFormat)
				where properties.Any(p => p.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol)))
				select $"[global::System.Diagnostics.CodeAnalysis.DynamicDependency(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicProperties, typeof({fullName}), Condition = \"SOLUTION_WIDE_CODE_ANALYSIS\")]"
			);

			context.AddSource(
				"Sudoku.Solving.Manual.StepInfo",
				"DynamicDependencies",
				$@"#pragma warning disable 1591

namespace Sudoku.Solving.Manual
{{
	partial record StepInfo
	{{
		{attributes}
		public partial string Formatize(bool handleEscaping = false);
	}}
}}"
			);
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}