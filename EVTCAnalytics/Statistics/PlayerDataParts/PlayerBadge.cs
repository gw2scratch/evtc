namespace GW2Scratch.EVTCAnalytics.Statistics.PlayerDataParts
{
	public class PlayerBadge
	{
		public BadgeType Type { get; }
		public string Text { get; }

		public PlayerBadge(string text, BadgeType type)
		{
			Text = text;
			Type = type;
		}
	}
}