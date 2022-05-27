﻿namespace Sudoku.UI.Data;

/// <summary>
/// Defines a setting group item.
/// </summary>
public sealed class SettingGroupItem
{
	/// <summary>
	/// Indicates the name of the setting item. The default value is <see cref="string.Empty"/>.
	/// </summary>
	/// <seealso cref="string.Empty"/>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the description of the setting item. The default value is <see cref="string.Empty"/>.
	/// </summary>
	/// <seealso cref="string.Empty"/>
	public string Description { get; set; } = string.Empty;
}
