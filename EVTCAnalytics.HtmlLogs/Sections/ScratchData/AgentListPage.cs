using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace ScratchLogHTMLGenerator.Sections.ScratchData
{
	public class AgentListPage : Page
	{
		private readonly IEnumerable<Agent> agents;

		public AgentListPage(IEnumerable<Agent> agents, ITheme theme) : base("Agents", true, theme)
		{
			this.agents = agents.ToArray();
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th>Agent type</th>
            <th>ID</th>
			<th>Name</th>
			<th>Master</th>
			<th><abbr title='Toughness'>Tgh</abbr></th>
			<th><abbr title='Condition damage'>Cnd</abbr></th>
			<th><abbr title='Healing'>Heal</abbr></th>
			<th><abbr title='Concentration'>Conc</abbr></th>
			<th>Profession</th>
			<th><abbr title='Elite Specialization'>Elite</abbr></th>
			<th>Species ID</th>
			<th>Hitbox Width</th>
			<th>Hitbox Height</th>
        </tr>
        </thead>
        <tbody>");
			foreach (var agent in agents)
			{
				if (agent is Player player)
				{
					writer.WriteLine($@"
            <tr>
                <td>Player</td>
                <td>{player.Id}</td>
                <td>{player.Name}</td>
                <td>{player.Master?.Name ?? ""}</td>
                <td>{player.Toughness}</td>
                <td>{player.Condition}</td>
                <td>{player.Healing}</td>
                <td>{player.Concentration}</td>
                <td>{player.Profession}</td>
                <td>{player.EliteSpecialization}</td>
                <td></td>
                <td>{player.HitboxWidth}</td>
                <td>{player.HitboxHeight}</td>
            </tr>");
				}
				else if (agent is NPC npc)
				{
					writer.WriteLine($@"
            <tr>
                <td>NPC</td>
                <td>{npc.Id}</td>
                <td>{npc.Name}</td>
                <td>{npc.Master?.Name ?? ""}</td>
                <td>{npc.Toughness}</td>
                <td>{npc.Condition}</td>
                <td>{npc.Healing}</td>
                <td>{npc.Concentration}</td>
                <td></td>
                <td></td>
                <td>{npc.SpeciesId}</td>
                <td>{npc.HitboxWidth}</td>
                <td>{npc.HitboxHeight}</td>
            </tr>");
				}
				else if (agent is Gadget gadget)
				{
					writer.WriteLine($@"
            <tr>
                <td>Gadget</td>
                <td>{gadget.Id}</td>
                <td>{gadget.Name}</td>
                <td>{gadget.Master?.Name ?? ""}</td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td>{gadget.HitboxWidth}</td>
                <td>{gadget.HitboxHeight}</td>
            </tr>");
				}
			}

			writer.WriteLine($@"
        </tbody>
    </table>");
		}
	}
}