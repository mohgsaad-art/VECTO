using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoHashing;

namespace VectoHashingTest
{
    public class VectoJobHashSortTest
    {
        private const string UnsortedJobPath = @"Testdata/XML/Sort/Job/Unsorted/";
        private const string SortedJobPath = @"Testdata/XML/Sort/Job/Sorted/";

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [TestCase("e70PsDLXiOBmXvfS3yLWcWQ7HFqjebg55TN8K/6d+vw=", SortedJobPath + "Multiple_FCHV_F2_IEPC_HeavyLorry.xml")]
        [TestCase("e70PsDLXiOBmXvfS3yLWcWQ7HFqjebg55TN8K/6d+vw=", UnsortedJobPath + "Multiple_FCHV_F2_IEPC_HeavyLorry.xml")]
        public void TestJobMultipleFCHV_HeavyLorryHashSort(string expectedJobHash, string filePath)
        {
            var loadedFile = VectoHash.Load(filePath);

            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.IEPC));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ADC));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.FuelCell, 1));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.FuelCell, 3));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Gearbox));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.TorqueConverter));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Angledrive));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder, 0));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder, 1));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Axlegear, 0));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Axlegear, 1));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Airdrag));

            Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());
        }

        [TestCase("039IS21z16yaKG4X9O4O+/UQtG1DJkvaAmwbhir3o2A=", SortedJobPath + "v24_SHEV_S2_HeavyLorry.xml")]
        [TestCase("039IS21z16yaKG4X9O4O+/UQtG1DJkvaAmwbhir3o2A=", UnsortedJobPath + "v24_SHEV_S2_HeavyLorry.xml")]
        public void TestJobHEV_S2_HeavyLorryHashSort(string expectedJobHash, string filePath)
        {
            var loadedFile = VectoHash.Load(filePath);

            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Engine));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Gearbox));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem, 0));//ElectricMachineGEN
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem, 1));//ElectricMachineE2
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage));//ElectricEnergyStorage
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Axlegear));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));//AxelWheels
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));//AxelWheels
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Airdrag));

            Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());
        }

        [TestCase("4DxfjPZXXphbl0ijjToCwGNxDQaGiV90U8b39+LuW7o=", SortedJobPath + "HEV-S_heavyLorry_IEPC-S.xml")]
        [TestCase("4DxfjPZXXphbl0ijjToCwGNxDQaGiV90U8b39+LuW7o=", UnsortedJobPath + "HEV-S_heavyLorry_IEPC-S.xml")]
        public void TestJobHEV_S_HeavyLorryHashSort(string expectedJobHash, string filePath)
        {
            var loadedFile = VectoHash.Load(filePath);

            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Engine));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem));//ElectricMachineGEN
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage, 0));//ElectricEnergyStorage
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage, 1));//ElectricEnergyStorage
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.IEPC));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Axlegear));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));//AxelWheels
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));//AxelWheels
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Airdrag));

            Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());
        }

        [TestCase("gn9Zq6pNRbb+cwWh2m08uUDbWBUufMrQmdICqqWL/Y8=", SortedJobPath + "Conventional_primaryBus_AMT.xml")]
        [TestCase("gn9Zq6pNRbb+cwWh2m08uUDbWBUufMrQmdICqqWL/Y8=", UnsortedJobPath + "Conventional_primaryBus_AMT.xml")]
        public void TestJobPrimaryBusHashSort(string expectedJobHash, string filePath)
        {
            var loadedFile = VectoHash.Load(filePath);

            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Engine));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Gearbox));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Axlegear));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.TorqueConverter));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Angledrive));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));

            Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());
		}


		[TestCase("ZlCKn8KI9PG4I7qWuNxf0jQXITCHxQDc1unNprZn76A=", SortedJobPath + "PEV_mediumLorry_AMT_E2.xml")]
		[TestCase("ZlCKn8KI9PG4I7qWuNxf0jQXITCHxQDc1unNprZn76A=", UnsortedJobPath + "PEV_mediumLorry_AMT_E2.xml")]

        public void TestJobPEVMediumLorry(string expectedJobHash, string filePath)
		{

			var loadedFile = VectoHash.Load(filePath);

			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage,0));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage,1));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Gearbox));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.TorqueConverter));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Angledrive));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Airdrag));

			Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());

        }

		[TestCase("l1Hf6GqKcAh5pZJ0TH3A0gV3qWQfwsQOmVrA/3sjjOE=", UnsortedJobPath + "PEV_mediumLorry_AMT_E2_3Bat.xml")]

		public void TestJobPEVMediumLorryBattery(string expectedJobHash, string filePath)
		{

			var loadedFile = VectoHash.Load(filePath);

			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage, 0), loadedFile.ComputeHash(VectoComponents.ElectricEnergyStorage, 0));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage, 1), loadedFile.ComputeHash(VectoComponents.ElectricEnergyStorage, 1));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage, 2), loadedFile.ComputeHash(VectoComponents.ElectricEnergyStorage, 2));
            Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Gearbox));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.TorqueConverter));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Angledrive));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Airdrag));

			Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());

            Assert.AreEqual("uoQkX7zZBul57xMVc4ztT/YkPoI2Y9/l1OUpT2C6eZg=", loadedFile.ReadHash(VectoComponents.ElectricEnergyStorage, 0));
            Assert.AreEqual("uoQkX7zZBul57xMVc4ztT/YkPoI2Y9/l1OUpT2C6eZg=", loadedFile.ReadHash(VectoComponents.ElectricEnergyStorage, 1));
            Assert.AreEqual("bqolF4NKnQMMf9Kvc4Xj0nzMdbeIyAR4/Ov2USB5CPs=", loadedFile.ReadHash(VectoComponents.ElectricEnergyStorage, 2));
		}

		[TestCase("AgbzuBUFTRB9b1XWfWf5VBQdPJlTsNJL+is/fQEeaUM=", UnsortedJobPath + "PEV_mediumLorry_AMT_E2_SuperCap.xml")]

		public void TestJobPEVMediumLorrySuperCap(string expectedJobHash, string filePath)
		{

			var loadedFile = VectoHash.Load(filePath);

			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricMachineSystem));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.ElectricEnergyStorage, 0), loadedFile.ComputeHash(VectoComponents.ElectricEnergyStorage, 0));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Gearbox));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.TorqueConverter));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Angledrive));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Retarder));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 0));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Tyre, 1));
			Assert.IsTrue(loadedFile.ValidateHash(VectoComponents.Airdrag));

			Assert.AreEqual(expectedJobHash, loadedFile.ComputeHash());

            Assert.AreEqual("4fFjMBbayKbJEq/7sJTUi3C4i006qzDQCqVOEL8y6RU=", loadedFile.ReadHash(VectoComponents.ElectricEnergyStorage, 0));
        }
    }
}
