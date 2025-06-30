using System.Diagnostics;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils;

public class EnumerableExtensionMethodsTests
{
	public class Entry
	{
		public readonly PerSecond EngineSpeed;
		public NewtonMeter Torque;

		public Entry(PerSecond engineSpeed, NewtonMeter torque)
		{
			EngineSpeed = engineSpeed;
			Torque = torque;
		}
	}

	[TestCase]
	public void TestGetSection()
	{
		var entries = new List<Entry>();
		for (var i = 0; i < 10; i++) {
			entries.Add(new Entry(i.RPMtoRad(), i.SI<NewtonMeter>()));
		}
		var entryArr = entries.ToArray();

		foreach (var val in new[] { -1, 0, 1, 5, 8, 9, 10 }) {
			var sw = Stopwatch.StartNew();
			var s = entries.GetSection(e => val > e.EngineSpeed);
			sw.Stop();
			//Console.WriteLine("Iterator: " + sw.Elapsed);

			sw.Restart();
			var s1 = entryArr.GetSection(e => val > e.EngineSpeed);
			sw.Stop();
			//Console.WriteLine("Array:    " + sw.Elapsed);

			Assert.AreSame(s.Item1, s1.Item1);
			Assert.AreSame(s.Item2, s1.Item2);
		}

		foreach (var val in new[] { -1, 0, 1, 5, 8, 9, 10 }) {
			var sw = Stopwatch.StartNew();
			var s = entries.GetSection(e => val < e.EngineSpeed);
			sw.Stop();
			//Console.WriteLine("Iterator: " + sw.Elapsed);

			sw.Restart();
			var s1 = entryArr.GetSection(e => val < e.EngineSpeed);
			sw.Stop();
			//Console.WriteLine("Array:    " + sw.Elapsed);

			Assert.AreSame(s.Item1, s1.Item1);
			Assert.AreSame(s.Item2, s1.Item2);
		}
	}
}