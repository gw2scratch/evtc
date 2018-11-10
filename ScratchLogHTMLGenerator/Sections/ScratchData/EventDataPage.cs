using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScratchLogHTMLGenerator.Sections.ScratchData
{
	public class EventDataPage : Page
	{
		private readonly IReadOnlyDictionary<string, int> eventCounts;

		public EventDataPage(IReadOnlyDictionary<string, int> eventCounts, ITheme theme) : base("Event data", true, theme)
		{
			this.eventCounts = eventCounts;
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th>Event name</th>
			<th>Count</th>
        </tr>
        </thead>
        <tbody>");
			foreach (var eventCount in eventCounts.OrderByDescending(x => x.Value))
			{
				var eventName = eventCount.Key;
				var count = eventCount.Value;
				writer.WriteLine($@"
            <tr>
                <td>{eventName}</td>
                <td>{count}</td>
            </tr>");
			}

			writer.WriteLine($@"
        </tbody>
		<tfoot>
            <tr>
                <td>Total</td>
                <td>{eventCounts.Sum(x => x.Value)}</td>
            </tr>
		</tfoot>
    </table>");
		}
	}
}