using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoHashing;

namespace VectoHashingTest
{
	public class VectoComponentHashSortTest
	{
		private const string UnsortedComponentPath = @"Testdata\XML\Sort\Component\Unsorted\";
		private const string SortedComponentPath = @"Testdata\XML\Sort\Component\Sorted\";
		
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}
		
		[TestCase("sYOwPn3VlMw71bjQFfACuekdAjZ8QRT3S8c71w7810A=", SortedComponentPath + "Engine.xml"),
		TestCase("sYOwPn3VlMw71bjQFfACuekdAjZ8QRT3S8c71w7810A=",  UnsortedComponentPath + "Engine.xml")]
		public void TestEngineHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Engine));
		}

		[TestCase("lYoUAB6Xob1azFaCPJBLK1HIT5Jr0K24H2jJec3r5BM=", SortedComponentPath + "Gearbox_APT-N.xml"),
		TestCase("lYoUAB6Xob1azFaCPJBLK1HIT5Jr0K24H2jJec3r5BM=", UnsortedComponentPath + "Gearbox_APT-N.xml")]
		public void TestGearboxAPT_NComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Gearbox));
		}
		
		[TestCase("BlXQyrK6qVsW5MHt74jB1Y1+a9toEwNC5KXYUmKawfQ=", SortedComponentPath + "Gearbox_FWD.xml"),
		TestCase("BlXQyrK6qVsW5MHt74jB1Y1+a9toEwNC5KXYUmKawfQ=", UnsortedComponentPath + "Gearbox_FWD.xml")]
		public void TestGearboxFWDComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Gearbox));
		}

		[TestCase("QVPf1HhUClM3JoxJHTVqcp2gPXMiqaZ5vFTVyjnG1Co=", SortedComponentPath + "Gearbox_IHPC.xml"),
		TestCase("QVPf1HhUClM3JoxJHTVqcp2gPXMiqaZ5vFTVyjnG1Co=", UnsortedComponentPath + "Gearbox_IHPC.xml")]
		public void TestGearboxIHPCComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Gearbox));
		}
		
		[TestCase("MCdQW6mfKSC6WVU+1A5UVLkxKc7eBbg4GWQp6KpzjEw=", SortedComponentPath + "Axlegear.xml"),
		TestCase("MCdQW6mfKSC6WVU+1A5UVLkxKc7eBbg4GWQp6KpzjEw=", UnsortedComponentPath + "Axlegear.xml")]
		public void TestAxlegearComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Axlegear));
		}
		
		[TestCase("lFLmCC4J39gaYU+VNw4q6ScyWOBzKoeVqmQ8/mQizBQ=", SortedComponentPath + "Angledrive.xml"),
		TestCase("lFLmCC4J39gaYU+VNw4q6ScyWOBzKoeVqmQ8/mQizBQ=", UnsortedComponentPath + "Angledrive.xml")]
		public void TestAngledriveComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Angledrive));
		}
		
		[TestCase("HWRjRcjJ/S1JBoUv+xjhWQDAsRn5c7D4LbwB04yyxrQ=", SortedComponentPath + "ADC.xml"),
		 TestCase("HWRjRcjJ/S1JBoUv+xjhWQDAsRn5c7D4LbwB04yyxrQ=", UnsortedComponentPath + "ADC.xml")]
		public void TestADCHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.ADC));
		}

		[TestCase("MNKcycaIzgndCWPEFDp84PlujKPkb9QaOxA8yKGNiKI=", SortedComponentPath + "BatterySystem_1.xml"),
		TestCase("MNKcycaIzgndCWPEFDp84PlujKPkb9QaOxA8yKGNiKI=", UnsortedComponentPath + "BatterySystem_1.xml")]
		public void TestBatterySystemHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.BatterySystem));
		}

		[TestCase("dBadIN60l8Iqcanj/nrx1EbD+KixtDxLAusUcutITk8=", SortedComponentPath + "CapacitorSystem_1.xml"),
		 TestCase("dBadIN60l8Iqcanj/nrx1EbD+KixtDxLAusUcutITk8=", UnsortedComponentPath + "CapacitorSystem_1.xml")]
		public void TestCapacitorHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.CapacitorSystem));
		}

		[TestCase("6VgGv3QToPDXpr7nSg1fupTLZ8qACAOEshZ8l5pUNI4=", SortedComponentPath + "ElectricMachineSystem-IHPC_1.xml"),
		TestCase("6VgGv3QToPDXpr7nSg1fupTLZ8qACAOEshZ8l5pUNI4=", UnsortedComponentPath + "ElectricMachineSystem-IHPC_1.xml")]
		public void TestElectricMachineIHPCHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.ElectricMachineSystem));
		}
		
		[TestCase("wLFLpJxFZ6mDXeqdlZCGVOLCoXTCf7XTL0q9ZKkmt7o=", SortedComponentPath + "ElectricMachineSystem_1.xml"),
		TestCase("wLFLpJxFZ6mDXeqdlZCGVOLCoXTCf7XTL0q9ZKkmt7o=", UnsortedComponentPath + "ElectricMachineSystem_1.xml")]
		public void TestElectricMachineHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.ElectricMachineSystem));
		}

		[TestCase("qP2WjwhL0iXRHZN7OK1Bz3jEpdP4cuYvOWSeJUu6Rl8=", SortedComponentPath + "ElectricMachineSystem_StdValues.xml"),
		TestCase("qP2WjwhL0iXRHZN7OK1Bz3jEpdP4cuYvOWSeJUu6Rl8=", UnsortedComponentPath + "ElectricMachineSystem_StdValues.xml")]
		public void TestElectricMachineStdValuesHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.ElectricMachineSystem));
		}

		[TestCase("3L/fYxKTdIwzADHQMnUBPxcNwZNEHM+sKEC2M32UnEA=", SortedComponentPath + "IEPC_1.xml"),
		TestCase("3L/fYxKTdIwzADHQMnUBPxcNwZNEHM+sKEC2M32UnEA=", UnsortedComponentPath + "IEPC_1.xml")]
		public void TestIEPCHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.IEPC));
		}
		
		[TestCase("BTHs/Hh2SgycIwU5OSuTgU/2SptMvmRFvPXr2X1Y7XQ=", SortedComponentPath + "IEPC_StdValues.xml"),
		TestCase("BTHs/Hh2SgycIwU5OSuTgU/2SptMvmRFvPXr2X1Y7XQ=", UnsortedComponentPath + "IEPC_StdValues.xml")]
		public void TestIEPCStdHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.IEPC));
		}

		[TestCase("fo5vyZY6IeQgfFpGmNV49YdfJlcEsGycJoNa3qqVChM=", SortedComponentPath + "Retarder.xml"),
		TestCase("fo5vyZY6IeQgfFpGmNV49YdfJlcEsGycJoNa3qqVChM=", UnsortedComponentPath + "Retarder.xml")]
		public void TestRetarderComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.Retarder));
		}
		
		[TestCase("mxPXYmGbF6eUzjxqAp9KXjy96DzMCGm5Xq9WLvNHwVA=", SortedComponentPath + "TorqueConverter.xml"),
		TestCase("mxPXYmGbF6eUzjxqAp9KXjy96DzMCGm5Xq9WLvNHwVA=", UnsortedComponentPath + "TorqueConverter.xml")]
		public void TestTorqueConverterComponentHashSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.TorqueConverter));
		}
		
		[TestCase("3YVNlu+1souB/4IwePLPoBwhiJormfqMNRxQaZ75wvM=", SortedComponentPath + "TrailerAerodynamicDevice.xml"),
		TestCase("3YVNlu+1souB/4IwePLPoBwhiJormfqMNRxQaZ75wvM=", UnsortedComponentPath + "TrailerAerodynamicDevice.xml")]
		public void TestTrailerAerodynamicDeviceSort(string expectedHash, string filePath)
		{
			var loadedFile = VectoHash.Load(filePath);
			Assert.AreEqual(expectedHash, loadedFile.ComputeHash(VectoComponents.CertifiedAeroReduction));
		}
	}
}
