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
		protected ITheme Theme { get; }

		protected Page(string menuName, bool showInMenu, ITheme theme)
		{
			ShowInMenu = showInMenu;
			MenuName = menuName;
			Theme = theme;
		}

		public IEnumerable<Page> Subpages { get; protected set; } = Enumerable.Empty<Page>();

		public abstract void WriteHtml(TextWriter writer);

		protected string MillisecondsToReadableFormat(long milliseconds)
		{
			return $"{milliseconds / 1000 / 60}m {milliseconds / 1000 % 60}s {milliseconds % 1000}ms";
		}

		public virtual void WriteStyleHtml(TextWriter writer)
		{
		}

		public virtual void WriteHeadHtml(TextWriter writer)
		{

		}

	}
}