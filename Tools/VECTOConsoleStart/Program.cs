using System.Linq;
using System.Text;
using VECTOStart;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main(string[] args)
		{
			var startHelper = new StarterHelper(isConsoleApp: true, StarterHelper.NET48, StarterHelper.NET80);
			startHelper.Start(args);
		}
	}
}
