﻿namespace Sudoku.Diagnostics.CodeGen.Reflection;

/// <summary>
/// Indicates the accessibility of a member.
/// </summary>
[Flags]
public enum MemberAccessibility : byte
{
	/// <summary>
	/// Indicates the accessibility is none.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the <see langword="public"/> modifier.
	/// </summary>
	Public = 1,

	/// <summary>
	/// Indicates the <see langword="internal"/> modifier.
	/// </summary>
	Internal = 2,

	/// <summary>
	/// Indicates the <see langword="protected"/> modifier.
	/// </summary>
	Protected = 4,

	/// <summary>
	/// Indicates the <see langword="protected internal"/> modifier.
	/// </summary>
	ProtectedInternal = Protected | Internal,

	/// <summary>
	/// Indicates the <see langword="private"/> modifier.
	/// </summary>
	Private = 8,

	/// <summary>
	/// Indicates the <see langword="private protected"/> modifier.
	/// </summary>
	PrivateProtected = Private | Protected
}