using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Controls;

public class DirectoryListControl : DynamicLayout
{
	private List<string> directories;

	public IEnumerable<string> Directories
	{
		get => directories;
		set
		{
			directories = value?.ToList() ?? new List<string>();
			RecreateLayout();
		}
	}
	
	private void RecreateLayout()
	{
		if (directories == null)
		{
			SuspendLayout();
			Clear();
			ResumeLayout();
			return;
		}
		
		SuspendLayout();
		Clear();
		
		BeginVertical(spacing: new Size(5, 5));
		{
			foreach ((int i, string directory) in directories.Select((x, i) => (i, x)))
			{
				BeginHorizontal();
				Add(new TextBox { Text = directory, ReadOnly = true }, true);
				var removeButton = new Button { Text = "Remove" };
				removeButton.Click += (_, _) =>
				{
					directories.RemoveAt(i);
					// This will trigger the setter which rebuilds the layout.
					Directories = directories;
				};
				Add(removeButton, false);
				EndHorizontal();
			}

			AddRow(ConstructAddButton());
		}
		EndVertical();
		
		Create();
		ResumeLayout();
	}

	private Button ConstructAddButton()
	{
		var button = new Button { Text = "Add a log directory" };
		button.Click += (_, _) =>
		{
			var dialog = new SelectFolderDialog();
			if (dialog.ShowDialog(this) == DialogResult.Ok)
			{
				if (!Directories.Contains(dialog.Directory))
				{
					Directories = Directories.Append(dialog.Directory);
				}
			}
		};
		return button;
	}
}