using System;
using System.IO;
using System.Windows.Forms;
using VECTOStart;
using Exception = System.Exception;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
                var startHelper = new StarterHelper(false,StarterHelper.NET48, StarterHelper.NET80);
				startHelper.Start(args);
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}
		}
	}
}
