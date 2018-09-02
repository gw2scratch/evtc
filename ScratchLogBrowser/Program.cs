using System;
using Eto.Forms;

namespace ScratchLogBrowser
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			new Application().Run(new BrowserForm());
		}
	}
}