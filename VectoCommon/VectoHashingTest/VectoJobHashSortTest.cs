using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoHashing;

namespace VectoHashingTest
{
    public class VectoJobHashSortTest
    {
        private const string UnsortedJobPath = @"Testdata\XML\Sort\Job\Unsorted\";
        private const string SortedJobPath = @"Testdata\XML\Sort\Job\Sorted\";

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [TestCase("HpFMjk3vmSp7FuZD6FEyBmDyrix7ifdcrKI26a5qEiw=", SortedJobPath + "HEV-S_heavyLorry_IEPC-S.xml")]
        [TestCase("HpFMjk3vmSp7FuZD6FEyBmDyrix7ifdcrKI26a5qEiw=", UnsortedJobPath + "HEV-S_heavyLorry_IEPC-S.xml")]
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


		[TestCase("xf+qFgxVz77cGsAoamoIPMCHztxIXqD1vFwz8sGbyiU=", SortedJobPath + "PEV_mediumLorry_AMT_E2.xml")]
		[TestCase("xf+qFgxVz77cGsAoamoIPMCHztxIXqD1vFwz8sGbyiU=", UnsortedJobPath + "PEV_mediumLorry_AMT_E2.xml")]

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

		[TestCase("OLkdJQk/V5cvGFqPahsKDIVV3RUxFGgbl4z1EABouac=", UnsortedJobPath + "PEV_mediumLorry_AMT_E2_3Bat.xml")]

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

		[TestCase("uEtvb6ULv7SX4tV1e5zmcl02Yl3/AFibGnt5T2oCCUg=", UnsortedJobPath + "PEV_mediumLorry_AMT_E2_SuperCap.xml")]

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
