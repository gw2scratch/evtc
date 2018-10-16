using System.Collections.Generic;

namespace ScratchLogHTMLGenerator
{
	public class Section
	{
		public string Name { get; }
		public IEnumerable<Page> Pages { get; }

		public Section(string name, params Page[] pages)
		{
			Name = name;
			Pages = pages;
		}
	}
}