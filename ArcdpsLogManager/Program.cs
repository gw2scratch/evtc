using System;
using Eto.Forms;

namespace ArcdpsLogManager
{
	internal class Program
	{
		[STAThread]
		static void Main()
		{
            new Application().Run(new ManagerForm());
		}
	}
}