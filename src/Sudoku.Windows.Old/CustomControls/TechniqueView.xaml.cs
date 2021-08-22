﻿namespace Sudoku.Windows.CustomControls;

/// <summary>
/// Interaction logic for <c>TechniqueView.xaml</c>.
/// </summary>
public partial class TechniqueView : UserControl
{
	/// <summary>
	/// Initializes a default <see cref="TechniqueView"/> instance.
	/// </summary>
	public TechniqueView()
	{
		InitializeComponent();
		InitializeTechniqueBoxes();
	}


	/// <summary>
	/// Indicates the chosen techniques.
	/// </summary>
	public TechniqueCodeFilter ChosenTechniques { get; } = new();


	/// <summary>
	/// Initializes the techniques, and stores them into the list.
	/// </summary>
	private void InitializeTechniqueBoxes()
	{
		var list = new List<TechniqueBox>();
		foreach (var (name, technique, category) in
			from technique in Enum.GetValues<Technique>()
			let nullableCategory = LangSource[$"Group{technique}"] as string
			where nullableCategory is not null
			select (TextResources.Current[technique.ToString()], technique, nullableCategory))
		{
			var box = new TechniqueBox
			{
				Technique = new(name, technique),
				Category = $"{LangSource["Category"]}{category}"
			};

			box.CheckingChanged += (sender, [Discard] _) =>
			{
				if (
					sender is CheckBox
					{
						Content: KeyedTuple<string, Technique>(_, var item, _),
						IsChecked: var isChecked
					} box
				)
				{
					(
						isChecked switch
						{
							true => ChosenTechniques.Add,
							false => ChosenTechniques.Remove,
							_ => (Func<Technique, TechniqueCodeFilter>?)null
						}
					)?.Invoke(item);
				}
			};

			list.Add(box);
		}

		_listTechniques.ItemsSource = list;
	}
}