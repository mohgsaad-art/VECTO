using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils.Algorithms;

public class MeanshiftClusteringTests
{
	[TestCase]
	public void TestClusteringClusters()
	{
		var centers = new[] { 100.0, 200, 300, 400 };
		var entries = new List<double>();
		var rnd = new Random(centers.Length);
		foreach (var center in centers) {
			var sum = 0.0;
			for (var i = 0; i < 30; i++) {
				var val = center + rnd.NextDouble() * 5 - 2.5;
				sum += val;
				entries.Add(val);
			}
			//Console.WriteLine(sum / 30);
		}

		//Console.WriteLine(string.Join(", ", entries));

		var clusterer = new MeanShiftClustering {
			ClusterCount = 8
		};
		var clusters = clusterer.FindClusters(entries.ToArray(), 10);

		//Console.WriteLine(clusterer.IterationCount);
		Console.WriteLine(centers.Join());
		Console.WriteLine(clusters.Join());

		Assert.AreEqual(centers.Length, clusters.Length);
		foreach (var center in centers) {
			Assert.IsTrue(clusters.Any(x => Math.Abs(center - x) < 2.5));
		}
	}

	[TestCase]
	public void TestClusteringRandom()
	{
		var entries = new double[100];
		var rnd = new Random(0);
		for (var i = 0; i < entries.Length; i++) {
			entries[i] = rnd.NextDouble() * 100;
		}

		var clusterer = new MeanShiftClustering();
		var clusters = clusterer.FindClusters(entries.ToArray(), 0.1);

		Console.WriteLine(clusters.Join());
	}
}