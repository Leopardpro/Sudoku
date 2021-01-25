﻿using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Sudoku.CodeGen
{
	/// <summary>
	/// Define a keyed tuple generator.
	/// </summary>
	[Generator]
	public sealed class KeyedTupleGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the separator that is used for separating multiple values.
		/// </summary>
		private const string CommaToken = ", ";


		/// <summary>
		/// Indicates whether the project uses tabs <c>'\t'</c> as indenting characters.
		/// </summary>
		private static readonly bool UsingTabsAsIndentingCharacters = true;

		/// <summary>
		/// Indicates the new line character in this current environment.
		/// </summary>
		private static readonly string NewLine = Environment.NewLine;


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var sb = new StringBuilder();
			for (int length = 2; length <= 8; length++)
			{
				sb.Clear();
				sb.AppendLine(PrintHeader());
				sb.AppendLine();
				sb.AppendLine(PrintNullableEnable());
				sb.AppendLine();
				sb.AppendLine(PrintUsingDirectives());
				sb.AppendLine();
				sb.AppendLine(PrintNamespace());
				sb.AppendLine(PrintOpenBracketToken());
				sb.AppendLine(PrintRecordDocComment(length));
				sb.AppendLine(PrintCompilerGenerated());
				sb.AppendLine(PrintRecordStatement(length));
				sb.AppendLine(PrintOpenBracketToken(1));
				sb.AppendLine(PrintUserDefinedConstructorDocComment(length));
				sb.AppendLine(PrintUserDefinedConstructor(length));
				sb.AppendLine(PrintOpenBracketToken(2));
				sb.AppendLine(PrintClosedBracketToken(2));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(PrintInheritDoc());
				sb.AppendLine(PrintLength(length));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(PrintInheritDoc());
				sb.AppendLine(PrintIndexerWithValue(length));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(PrintInheritDoc());
				sb.AppendLine(PrintToString());
				sb.AppendLine(PrintToStringValue());
				sb.AppendLine(PrintClosedBracketToken(1));
				sb.AppendLine(PrintClosedBracketToken());

				context.AddSource(
					hintName: $"KeyedTuple_{length.ToString()}.cs",
					sourceText: SourceText.From(
						text: sb.ToString(),
						encoding: Encoding.UTF8
					)
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


		private static string PrintOpenBracketToken(int indentingCount = 0) => indentingCount == 0 ? "{" : $"{new string('\t', indentingCount)}{{";
		private static string PrintClosedBracketToken(int indentingCount = 0) => indentingCount == 0 ? "}" : $"{new string('\t', indentingCount)}}}";
		private static string PrintHeader()
		{
			var sb = new StringBuilder();
			sb.AppendLine("//");
			sb.AppendLine("// <auto-generated>");
			sb.AppendLine("//     This file is generated by compiler, powered by source generator.");
			sb.AppendLine("//");
			sb.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
			sb.AppendLine("//     the code is regenerated.");
			sb.AppendLine("// </auto-generated>");
			sb.AppendLine("//");
			return sb.ToString();
		}
		private static string PrintUsingDirectives() => "using System.Runtime.CompilerServices";
		private static string PrintNullableEnable() => "#nullable enable";
		private static string PrintNamespace() => "namespace System.Collections.Generic";
		private static string PrintCompilerGenerated()
		{
			const int o = 9;

#if NETSTANDARD2_1
			return $"{(UsingTabsAsIndentingCharacters ? "\t" : "    ")}[{nameof(CompilerGeneratedAttribute)[..^o]}]";
#else
			const string name = nameof(CompilerGeneratedAttribute);
			return $"{(UsingTabsAsIndentingCharacters ? "\t" : "    ")}[{name.Substring(0, name.Length - o)}]";
#endif
		}

		private static string PrintRecordDocComment(int length)
		{
			var sb = new StringBuilder();
			sb.Append(UsingTabsAsIndentingCharacters ? "\t" : "    ");
			sb.AppendLine("/// <summary>");
			sb.Append(UsingTabsAsIndentingCharacters ? "\t" : "    ");
			sb.AppendLine("/// Provides a tuple with a primary element, which means the tuple contains multiple items,");
			sb.Append(UsingTabsAsIndentingCharacters ? "\t" : "    ");
			sb.AppendLine("/// but the only specified item can be output as <see cref=\"string\"/> text.");
			sb.Append(UsingTabsAsIndentingCharacters ? "\t" : "    ");
			sb.AppendLine("/// </summary>");

			for (int i = 1; i <= length; i++)
			{
				sb
					.Append(UsingTabsAsIndentingCharacters ? "\t" : "    ")
					.Append("/// <typeparam name=\"T")
					.Append(i)
					.Append("\">The type of the property <see cref=\"KeyedTuple{");

				for (int j = 1; j <= length; j++)
				{
					sb.Append('T').Append(j).Append(CommaToken);
				}

				sb
					.Remove(sb.Length - CommaToken.Length, CommaToken.Length)
					.Append("}.Item")
					.Append(i)
					.AppendLine("\"/>.</typeparam>");
			}

			sb
				.Append(UsingTabsAsIndentingCharacters ? "\t" : "    ")
				.Append("/// <param name=\"PriorKey\">The prior key.</param>");

			return sb.ToString();
		}
		private static string PrintRecordStatement(int length)
		{
			var sb = new StringBuilder();
			sb.Append(UsingTabsAsIndentingCharacters ? "\t" : "    ");
			sb.Append("public sealed record KeyedTuple<");

			for (int i = 1; i <= length; i++)
			{
				sb.Append('T').Append(i).Append(CommaToken);
			}
			sb.Remove(sb.Length - CommaToken.Length, CommaToken.Length).Append(">(");
			for (int i = 1; i <= length; i++)
			{
				sb.Append('T').Append(i).Append(' ').Append("Item").Append(i).Append(CommaToken);
			}
			sb.Append("int PriorKey) : ITuple");
			return sb.ToString();
		}
		private static string PrintUserDefinedConstructorDocComment(int length)
		{
			var sb = new StringBuilder();
			sb.Append(UsingTabsAsIndentingCharacters ? "\t\t" : "        ");
			sb.AppendLine("/// <summary>");
			sb.Append(UsingTabsAsIndentingCharacters ? "\t\t" : "        ");
			sb.AppendLine($"/// Initializes an instance with the specified {length.ToString()} items, and the first one is the prior key.");
			sb.Append(UsingTabsAsIndentingCharacters ? "\t\t" : "        ");
			sb.AppendLine("/// </summary>");
			for (int i = 1; i <= length; i++)
			{
				sb
					.Append(UsingTabsAsIndentingCharacters ? "\t\t" : "        ")
					.Append("/// <param name=\"item")
					.Append(i)
					.Append("\">The item ")
					.Append(i)
					.AppendLine(".</param>");
			}

			sb.Remove(sb.Length - NewLine.Length, NewLine.Length);

			return sb.ToString();
		}
		private static string PrintUserDefinedConstructor(int length)
		{
			var sb = new StringBuilder();
			sb.Append(UsingTabsAsIndentingCharacters ? "\t\t" : "        ");
			sb.Append("public KeyedTuple(");
			for (int i = 1; i <= length; i++)
			{
				sb.Append('T').Append(i).Append(' ').Append("item").Append(i).Append(CommaToken);
			}
			sb.Remove(sb.Length - CommaToken.Length, CommaToken.Length).Append(')');
			if (length <= 4)
			{
				sb.Append(" : this(");
			}
			else
			{
				sb
					.AppendLine()
					.Append(UsingTabsAsIndentingCharacters ? "\t\t\t" : "            ")
					.Append(": this(");
			}

			for (int i = 1; i <= length; i++)
			{
				sb.Append("item").Append(i).Append(CommaToken);
			}
			sb.Append(1).Append(')');
			return sb.ToString();
		}
		private static string PrintInheritDoc() => UsingTabsAsIndentingCharacters ? "\t\t/// <inheritdoc/>" : "        /// <inheritdoc/>";
		private static string PrintLength(int length) => $"{(UsingTabsAsIndentingCharacters ? "\t\t" : "        ")}int ITuple.Length => {length.ToString()};";
		private static string PrintIndexerWithValue(int length)
		{
			var sb = new StringBuilder();
			sb.Append(UsingTabsAsIndentingCharacters ? "\t\t" : "        ");
			sb.Append("object? ITuple.this[int index] =>");
			if (length <= 4)
			{
				sb.Append(' ').Append(PrintIndexerValue(false, length));
			}
			else
			{
				sb.AppendLine().Append(PrintIndexerValue(true, length));
			}

			return sb.ToString();
		}
		private static string PrintIndexerValue(bool withIndent, int length)
		{
			var sb = new StringBuilder();
			if (withIndent)
			{
				sb.Append(UsingTabsAsIndentingCharacters ? "\t\t\t" : "            ");
			}
			sb.Append("index switch { ");
			for (int i = 1; i <= length; i++)
			{
				sb.Append(i).Append(" => ").Append("Item").Append(i).Append(CommaToken);
			}
			sb.Remove(sb.Length - CommaToken.Length, CommaToken.Length).Append(" };");

			return sb.ToString();
		}
		private static string PrintToString() => UsingTabsAsIndentingCharacters ? "\t\tpublic override string ToString() =>" : "        public override string ToString() =>";
		private static string PrintToStringValue() => UsingTabsAsIndentingCharacters ? "\t\t\t((ITuple)this)[PriorKey]?.ToString() ?? string.Empty;" : "            ((ITuple)this)[PriorKey]?.ToString() ?? string.Empty;";
	}
}
