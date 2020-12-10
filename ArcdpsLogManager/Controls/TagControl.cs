using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs.Tagging;
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
				var hashSet = value?.ToHashSet() ?? new HashSet<TagInfo>();
				tags = hashSet;
				RecreateLayout();
			}
		}

		private HashSet<TagInfo> tags;

		public event EventHandler<TagAddedEventArgs> TagAdded;
		public event EventHandler<TagRemovedEventArgs> TagRemoved;

		public class TagAddedEventArgs : EventArgs
		{
			public string Name { get; }

			public TagAddedEventArgs(string tagName)
			{
				Name = tagName;
			}
		}

		public class TagRemovedEventArgs : EventArgs
		{
			public string Name { get; }

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

			// TODO: a nice "+" icon?
			var addTagButton = new Button {Text = "+"};

			var tagTextBox = new TextBox();
			tagTextBox.KeyDown += (sender, args) =>
			{
				if (args.Key == Keys.Enter)
				{
					addTagButton.PerformClick();
				}
			};

			addTagButton.Click += (sender, args) =>
			{
				string tagName = tagTextBox.Text.Trim();
				if (tagName == "") return;

				tags.Add(new TagInfo(tagName));
				RecreateLayout();
				TagAdded?.Invoke(this, new TagAddedEventArgs(tagName));

				tagTextBox.Text = "";
			};

			SuspendLayout();
			Clear();
			BeginVertical();
			{
				BeginGroup("Tags", new Padding(5), new Size(5, 5));
				{
					foreach (var tag in tags.OrderBy(x => x.Name))
					{
						var removeButton = new Button {Text = "-"};

						AddRow(new Label {Text = tag.Name, VerticalAlignment = VerticalAlignment.Center}, removeButton);

						removeButton.Click += (s, e) =>
						{
							tags.Remove(tag);
							TagRemoved?.Invoke(this, new TagRemovedEventArgs(tag.Name));
							RecreateLayout();
						};
					}

					Add(null);
					BeginHorizontal();
					{
						// This double-embedding is required to have both the label and the TextBox only occupy one horizontal slot in the table
						BeginVertical(xscale: true);
						{
							BeginHorizontal();
							{
								Add(new Label {Text = "New tag: ", VerticalAlignment = VerticalAlignment.Center});
								Add(tagTextBox, xscale: true);
							}
							EndHorizontal();
						}
						EndVertical();
						Add(addTagButton, false);
					}
					EndHorizontal();
				}
				EndGroup();
			}
			EndVertical();
			Create();
			ResumeLayout();
		}
	}
}