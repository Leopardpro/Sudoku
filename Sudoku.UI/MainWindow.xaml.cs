﻿using System.IO;
using System.Windows;
using Sudoku.Data;
using Sudoku.UI.Extensions;

namespace Sudoku.UI
{
	/// <summary>
	/// Interaction logic for <see cref="MainWindow"/>.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes a <see cref="MainWindow"/> instance with the default instantiation behavior.
		/// </summary>
		public MainWindow() => InitializeComponent();


		/// <summary>
		/// Triggers when the menu item that opens a file is clicked.
		/// </summary>
		/// <param name="sender">The sender triggers the event.</param>
		/// <param name="e">The event arguments provided.</param>
		private async void MenuItemFileOpen_ClickAsync(object sender, RoutedEventArgs e)
		{
			var dialog = OpenFileDialogs.PuzzleLoading;
			if (dialog.ShowDialog() is not true)
			{
				return;
			}

			string path = dialog.FileName;
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			string sudokuGridCode = await File.ReadAllTextAsync(path);

			if (SudokuGrid.TryParse(sudokuGridCode, out var grid))
			{
				// Load successful.
				ViewModel.Grid = grid;
			}
			else
			{
				// Load failed.
				MessageDialogs.PuzzleFileLoadFailed.ShowDialog();
			}
		}
	}
}
