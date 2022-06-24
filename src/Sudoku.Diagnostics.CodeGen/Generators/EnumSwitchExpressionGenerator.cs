﻿using DataTuple = System.ValueTuple<
	/*TypeSymbol*/ Microsoft.CodeAnalysis.INamedTypeSymbol,
	/*MethodName*/ string,
	/*RootAttributeData*/ Microsoft.CodeAnalysis.AttributeData,
	System.Collections.Generic.IEnumerable<
		System.ValueTuple<
			/*FieldSymbol*/ Microsoft.CodeAnalysis.IFieldSymbol,
			/*FieldAttributeData*/ Microsoft.CodeAnalysis.AttributeData>>>;

namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the method and the corresponding values,
/// forming a <see langword="switch"/> expression.
/// </summary>
/// <remarks>This source generator does not support generic types.</remarks>
[Generator(LanguageNames.CSharp)]
public sealed class EnumSwitchExpressionGenerator : IIncrementalGenerator
{
	private const string
		SwitchExprRootFullName = "System.Diagnostics.CodeGen.EnumSwitchExpressionRootAttribute",
		SwitchExprArmFullName = "System.Diagnostics.CodeGen.EnumSwitchExpressionArmAttribute";


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(NodePredicate, Transform)
				.Where(static pair => pair is not null)
				.Collect(),
			GenerateSource
		);

	private (INamedTypeSymbol, DataTuple[])? Transform(GeneratorSyntaxContext gsc, CancellationToken ct)
	{
		if (
			gsc is not
			{
				Node: EnumDeclarationSyntax enumDeclarationSyntaxNode,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return null;
		}

		var rawTypeSymbol = semanticModel.GetDeclaredSymbol(enumDeclarationSyntaxNode, ct);
		if (rawTypeSymbol is not INamedTypeSymbol typeSymbol)
		{
			return null;
		}

		var switchExprRoot = compilation.GetTypeByMetadataName(SwitchExprRootFullName);
		var typeAttributesData = (
			from attributeData in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, switchExprRoot)
			select attributeData
		).Distinct(new AttributeDataComparerDistinctByKey()).ToArray();
		if (typeAttributesData.Length == 0)
		{
			return null;
		}

		var result = new List<DataTuple>();
		var switchExprArm = compilation.GetTypeByMetadataName(SwitchExprArmFullName);
		var fieldAndItsCorrespondingAttributeData = new List<(IFieldSymbol, AttributeData)>();
		foreach (var attributeData in typeAttributesData)
		{
			string key = (string)attributeData.ConstructorArguments[0].Value!;
			foreach (var field in typeSymbol.GetMembers().OfType<IFieldSymbol>())
			{
				var fieldAttributeData = (
					from fad in field.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(fad.AttributeClass, switchExprArm)
					let construtorArgs = fad.ConstructorArguments
					where construtorArgs.Length >= 1
					let firstConstructorArg = (string)construtorArgs[0].Value!
					where firstConstructorArg == key
					select fad
				).FirstOrDefault();

				fieldAndItsCorrespondingAttributeData.Add((field, fieldAttributeData));
			}

			result.Add(new(typeSymbol, key, attributeData, fieldAndItsCorrespondingAttributeData.ToArray()));
			fieldAndItsCorrespondingAttributeData.Clear();
		}

		return (typeSymbol, result.ToArray());
	}

	/// <inheritdoc/>
	public void GenerateSource(SourceProductionContext spc, ImmutableArray<(INamedTypeSymbol, DataTuple[])?> collection)
	{
		// Iterates on each tuple array.
		// An array of tuples is a group, storing a set of tuples whose key and type are same.
		foreach (var pair in collection)
		{
			if (pair is not var (type, tuples))
			{
				continue;
			}

			string fullName = type.ToDisplayString(TypeFormats.FullName);

			var emittedMethods = new List<string>();

			// Iterates on each tuple element in the array.
			foreach (var (_, key, typeAttributeData, listOfFieldsAndTheirOwnAttributesData) in tuples)
			{
				var innerParts = new List<string>();

				foreach (var (fieldSymbol, attributeData) in listOfFieldsAndTheirOwnAttributesData)
				{
					string value = (string)attributeData.ConstructorArguments[1].Value!;
					innerParts.Add($"""{fullName}.{fieldSymbol.Name} => "{value}",""");
				}

				int notDefinedBehavior = typeAttributeData.GetNamedArgument<byte>("DefaultBehavior", 2);
				string notFoundBehaviorStr = notDefinedBehavior switch
				{
					// ReturnIntegerValue
					0 => "@this.ToString()",

					// ReturnNull
					1 => "null",

					// ThrowForNotDefined
					_ => "throw new global::System.ArgumentOutOfRangeException(nameof(@this))"
				};

				string methodDescription = typeAttributeData.GetNamedArgument<string>("MethodDescription")
					?? $"Method <c>{key}</c>";
				string thisParamDescription = typeAttributeData.GetNamedArgument<string>("ThisParameterDescription")
					?? "The current instance.";
				string returnValueDescription = typeAttributeData.GetNamedArgument<string>("ReturnValueDescription")
					?? "The result value.";
				string returnType = notDefinedBehavior == 1 ? "string?" : "string";

				emittedMethods.Add(
					$$"""
					/// <summary>
						/// {{methodDescription}}
						/// </summary>
						/// <param name="this">{{thisParamDescription}}</param>
						/// <returns>{{returnValueDescription}}</returns>
						[global::System.Runtime.CompilerServices.CompilerGenerated]
						[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
						[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						public static {{returnType}} {{key}}(this {{fullName}} @this)
							=> @this switch
							{
								{{string.Join("\r\n\t\t\t", innerParts)}}
								_ => {{notFoundBehaviorStr}}
							};
					"""
				);
			}

			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.EnumSwitchExpression}.cs",
				$$"""
				// <auto-generated/>
				
				#nullable enable
				
				namespace {{SymbolOutputInfo.FromSymbol(type).NamespaceName}};
				
				/// <summary>
				/// Provides with extension methods for switching on the current type.
				/// </summary>
				[global::System.Runtime.CompilerServices.CompilerGenerated]
				[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
				public static class {{type.Name}}_EnumSwitchExpressionExtensions
				{
					{{string.Join("\r\n\r\n\t", emittedMethods)}}
				}
				"""
			);
		}
	}

	private static bool NodePredicate(SyntaxNode node, CancellationToken _) => node is EnumDeclarationSyntax;


	/// <summary>
	/// Defines a comparer that compares for the inner key.
	/// </summary>
	private sealed class AttributeDataComparerDistinctByKey : IEqualityComparer<AttributeData>
	{
		/// <inheritdoc/>
		public bool Equals(AttributeData x, AttributeData y)
		{
			if (!SymbolEqualityComparer.Default.Equals(x.AttributeClass, y.AttributeClass))
			{
				return false;
			}

			string? key = (string?)x.ConstructorArguments[0].Value;
			string? another = (string?)y.ConstructorArguments[0].Value;
			return key == another;
		}

		/// <inheritdoc/>
		public int GetHashCode(AttributeData obj) => SymbolEqualityComparer.Default.GetHashCode(obj.AttributeClass);
	}
}
