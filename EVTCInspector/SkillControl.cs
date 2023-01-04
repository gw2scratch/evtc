using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCInspector
{
	public class SkillControl : Panel
	{
		public Skill Skill
		{
			get => skill;
			set
			{
				skill = value;
				nameLabel.Text = skill == null ? "No skill selected." : $"Name: {skill.Name}";
				jsonControl.Object = skill;
			}
		}

		private Skill skill;

		private readonly JsonSerializationControl jsonControl;
		private readonly Label nameLabel = new Label();

		public SkillControl()
		{
			var dataLayout = new DynamicLayout();
			Content = dataLayout;

			jsonControl = new JsonSerializationControl {Height = 200};

			dataLayout.BeginVertical(new Padding(5));
			dataLayout.AddRow(nameLabel);
			dataLayout.AddRow(jsonControl);
			dataLayout.EndVertical();
		}
	}
}