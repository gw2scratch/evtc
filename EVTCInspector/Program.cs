using System;
using Eto.Forms;

namespace GW2Scratch.EVTCInspector
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