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
                var startHelper = new StarterHelper(false,StarterHelper.NET48, StarterHelper.NET60, StarterHelper.NET80, StarterHelper.NET100);
				startHelper.Start(args);
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}
		}
	}
}
