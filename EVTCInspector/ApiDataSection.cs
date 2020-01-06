using System.Linq;
using Eto.Forms;
using Eto.Generator;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Statistics;

namespace GW2Scratch.EVTCInspector
{
	public class ApiDataSection : Panel
	{
		private GW2ApiData apiData;

		public GW2ApiData ApiData
		{
			get => apiData;
			set
			{
				apiData = value;
				skillDataGridView.DataStore = apiData.SkillData.ToArray();
			}
		}

		private readonly GridView<SkillData> skillDataGridView;

		public ApiDataSection()
		{
			var tabControl = new TabControl();
			Content = tabControl;

			skillDataGridView = new GridViewGenerator().GetGridView<SkillData>();

			tabControl.Pages.Add(new TabPage(skillDataGridView) {Text = "Skill Data"});
		}
	}
}