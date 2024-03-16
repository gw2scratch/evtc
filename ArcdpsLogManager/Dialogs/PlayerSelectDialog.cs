using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Dialogs;

public class PlayerSelectDialog : Dialog
{
	private bool Confirmed { get; set; }

	private readonly PlayerList playerList;

	public PlayerSelectDialog(LogCache logCache, ApiData apiData, LogDataProcessor logProcessor,
		UploadProcessor uploadProcessor, ImageProvider imageProvider, ILogNameProvider logNameProvider,
		IEnumerable<LogData> logs
	)
	{
		Title = "Select a player";
		ShowInTaskbar = true;
		Padding = new Padding(10);
		Size = new Size(800, 600);

		var layout = new DynamicLayout();
		Content = layout;

		var confirmButton = new Button { Text = "Select" };
		PositiveButtons.Add(confirmButton);
		confirmButton.Click += (_, _) =>
		{
			Confirmed = true;
			Close();
		};

		AbortButton = new Button { Text = "Cancel" };
		NegativeButtons.Add(AbortButton);
		AbortButton.Click += (_, _) =>
		{
			Close();
		};

		playerList = new PlayerList(logCache, apiData, logProcessor, uploadProcessor, imageProvider, logNameProvider, false);
		playerList.UpdateDataFromLogs(logs);
		playerList.PlayerDoubleClicked += (sender, args) =>
		{
			if (playerList.SelectedPlayer != null)
			{
				confirmButton.PerformClick();
			}
		};

		layout.Add(playerList);
	}

	public record SelectedPlayer(string AccountName, IReadOnlyList<SelectedCharacter> Characters);

	public record SelectedCharacter(string Name, Profession Profession);

	public SelectedPlayer ShowDialog(Control owner)
	{
		this.ShowModal(owner);

		var selected = playerList.SelectedPlayer;
		if (Confirmed && selected != null)
		{
			return new SelectedPlayer(selected.AccountName, selected.FindCharacters().Select(x => new SelectedCharacter(x.Name, x.Profession)).ToList());
		}

		return null;
	}
}