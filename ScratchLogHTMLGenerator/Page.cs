using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScratchLogHTMLGenerator
{
	public abstract class Page
	{
		public bool ShowInMenu { get; }
		public string MenuName { get; }

		protected Page(bool showInMenu, string menuName)
		{
			ShowInMenu = showInMenu;
			MenuName = menuName;
		}

		public IEnumerable<Page> Subpages { get; protected set; } = Enumerable.Empty<Page>();

		public abstract void WriteHtml(TextWriter writer);

		protected string MillisecondsToReadableFormat(long milliseconds)
		{
			return $"{milliseconds / 1000 / 60}m {milliseconds / 1000 % 60}s {milliseconds % 1000}ms";
		}
	}
}