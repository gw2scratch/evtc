using System;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager
{
	internal class Program
	{
		[STAThread]
		static void Main()
		{
            new Application().Run(new LoadingForm());
		}
	}
}