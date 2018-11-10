using ScratchEVTCParser.Model.Agents;

namespace ScratchLogHTMLGenerator
{
	public interface ITheme
	{
		string GetProfessionColorMedium(Player player);
		string GetProfessionColorLight(Player player);
		string GetProfessionColorLightTransparent(Player player);
		string GetBigProfessionIconUrl(Player player);
		string GetTinyProfessionIconUrl(Player player);

		string GetMightIconUrl();
		string GetQuicknessIconUrl();
		string GetVulnerabilityIconUrl();
	}
}