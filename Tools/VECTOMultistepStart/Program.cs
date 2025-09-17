using System.Windows.Forms;
using System;
using VECTOStart;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				var startHelper = new StarterHelper(isConsoleApp: false, StarterHelper.NET48, StarterHelper.NET80);
				startHelper.Start(args);
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}
		}

	}
}
