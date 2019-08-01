using System;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Commands
{
	public class About : Command
	{
		public About()
		{
			MenuText = "About";
			Shortcut = Keys.F11;
		}

		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);

			var about = new AboutDialog
			{
				Logo = Resources.GetProgramIcon(),
				Website = new Uri("https://discord.gg/TnHpN34"),
				WebsiteLabel = "Discord Server"
			};
			about.ShowDialog(Application.Instance.MainForm);
		}
	}
}