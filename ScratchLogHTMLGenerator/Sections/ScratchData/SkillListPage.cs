using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScratchEVTCParser.Model.Skills;

namespace ScratchLogHTMLGenerator.Sections.ScratchData
{
	public class SkillListPage : Page
	{
		private readonly IEnumerable<Skill> skills;

		public SkillListPage(IEnumerable<Skill> skills, ITheme theme) : base("Skills", true, theme)
		{
			this.skills = skills.ToArray();
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th>Skill ID</th>
			<th>Skill name</th>
        </tr>
        </thead>
        <tbody>");
			foreach (var skill in skills)
			{
				writer.WriteLine($@"
            <tr>
                <td>{skill.Id}</td>
                <td>{skill.Name}</td>
            </tr>");
			}

			writer.WriteLine($@"
        </tbody>
		<tfoot>
            <tr>
                <td>Total</td>
                <td>{skills.Count()} skills</td>
            </tr>
		</tfoot>
    </table>");
		}
	}
}