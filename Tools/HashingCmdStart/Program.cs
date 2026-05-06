using VECTOStart;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main(string[] args)
		{
			var startHelper = new StarterHelper(isConsoleApp: true, StarterHelper.NET48, StarterHelper.NET60, StarterHelper.NET80, StarterHelper.NET100);
			startHelper.Start(args);
        }
	}
}