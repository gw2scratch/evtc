using System;
using Eto.Forms;

namespace ScratchLogMaker
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