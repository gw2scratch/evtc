using System;
using Eto.Forms;

namespace GW2Scratch.HtmlLogMaker
{
	internal class Program
	{
		[STAThread]
		static void Main()
		{
			new Application().Run(new MakerForm());
		}
	}
}