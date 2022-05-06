﻿namespace System.Diagnostics.CodeGen;

/// <summary>
/// Indicates an attribute type that can control the implementation case on interfaces.
/// </summary>
internal interface IInterfaceImplementingCaseController : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// Indicates whether the source generator will emit explicit implmentation to implement the method.
	/// The default value is <see langword="false"/>.
	/// </summary>
	bool UseExplicitImplementation { get; init; }
}