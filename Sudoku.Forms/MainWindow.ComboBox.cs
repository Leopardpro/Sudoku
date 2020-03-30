﻿using w = System.Windows;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ComboBoxMode_SelectionChanged(object sender, w::Controls.SelectionChangedEventArgs e)
		{
			// When initializing, the selection index will be changed to 0.
			// During changing, the label or combo box may be null in this case.
			// So here need null checking.
			if (sender is w::Controls.ComboBox comboBox
				&& !(_labelSymmetry is null) && !(_comboBoxSymmetry is null))
			{
				switch (comboBox.SelectedIndex)
				{
					case 0: // Symmetry mode.
					{
						_labelSymmetry.Visibility = w::Visibility.Visible;
						_comboBoxSymmetry.Visibility = w::Visibility.Visible;
						return;
					}
					case 1: // Hard pattern mode.
					{
						_labelSymmetry.Visibility = w::Visibility.Hidden;
						_comboBoxSymmetry.Visibility = w::Visibility.Hidden;
						return;
					}
					default:
					{
						// What the hell is this selection???
						e.Handled = true;
						return;
					}
				}
			}
		}
	}
}
