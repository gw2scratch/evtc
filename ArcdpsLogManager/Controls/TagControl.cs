using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using static GW2Scratch.ArcdpsLogManager.Logs.LogData;
using Image = Eto.Drawing.Image;
using Label = Eto.Forms.Label;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public class TagControl : DynamicLayout
	{
		public IEnumerable<TagInfo> Tags
		{
			get => tags;
			set
			{
				var valueArray = value?.ToArray();
				Array.Sort(valueArray, (a, b) => StringComparer.CurrentCultureIgnoreCase.Compare(a.Name, b.Name));
				tags = valueArray;
				RecreateLayout();
			}
		}

		private TagInfo[] tags;

		public TagControl(IEnumerable<TagInfo> tags)
		{
			Tags = tags.ToArray();
		}

		public event EventHandler<TagAddedEventArgs> RaiseTagAdded;
		public event EventHandler<TagRemovedEventArgs> RaiseTagRemoved;

		public class TagAddedEventArgs : EventArgs
		{
			public string Name { get; set; }
			public TagAddedEventArgs(string tagName)
			{
				Name = tagName;
			}
		}

		public class TagRemovedEventArgs : EventArgs
		{
			public string Name { get; set; }
			public TagRemovedEventArgs(string tagName)
			{
				Name = tagName;
			}
		}

		private void RecreateLayout()
		{
			if (tags == null)
			{
				SuspendLayout();
				Clear();
				ResumeLayout();
				return;
			}

			// TODO:  a nice "+" icon?
			var addTagButton = new Button { Text = "+" };

			var tagTextBox = new TextBox();

			addTagButton.Click += (sender, args) => {
				var tagText = tagTextBox.Text;
				if (tagText == "") return;

				RaiseTagAdded?.Invoke(this, new TagAddedEventArgs(tagText));
				Tags = Tags.Append(new TagInfo(tagText, "user"));

				tagTextBox.Text = "";
			};

			SuspendLayout();
			Clear();
			BeginVertical();
			{
				var group = BeginGroup("Tags", new Padding(5), new Size(5, 5));
				foreach (var tag in tags)
				{
					var removeButton = new Button { Text = "-" };

					if (tag.Type == "user")
					{
						var row = AddRow(new Label { Text = tag.Name }, removeButton);

						removeButton.Click += (s, e) =>
						{
							RaiseTagRemoved?.Invoke(this, new TagRemovedEventArgs(tag.Name));
							group.Rows.Remove(row);
						};
					} else
					{
						var row = AddRow(new Label { Text = tag.Name });
					}
				}
				Add(null);
				BeginHorizontal();
				{
					Add(new Label { Text = "Add a tag: " }, xscale: true);
					Add(null);
				}
				EndHorizontal();
				BeginHorizontal();
				{
					Add(tagTextBox, xscale: true);
					Add(addTagButton, false);
				}
				EndHorizontal();
//				AddRow(new Label { Text = "Add a tag: " }, tagTextBox, addTagButton);
				Add(null);
				EndGroup();
			}
			EndVertical();
			AddRow(null);
			Create();
			ResumeLayout();
		}
	}
}