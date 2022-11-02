using System.IO;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	public class XMLDeclarationInputv24VehicleDataTest
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		private const string Optional_TESTS_DIR =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\WithoutOptionalEntries";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		private IXMLDeclarationVehicleData ReadVehicleData(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			var vehicle = dataProvider.JobInputData.Vehicle as IXMLDeclarationVehicleData;
			
			Assert.IsNotNull(vehicle);

			return vehicle;
		}

		#region Test group parameters

		private void TestChassisLorryParametersSequenceGroup(IXMLDeclarationVehicleData vehicle)
		{
			Assert.AreEqual(6000.SI<Kilogram>(), vehicle.CurbMassChassis);
			Assert.AreEqual(12000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
		}
		
		private void TestRetarderSequenceGroup(IXMLDeclarationVehicleData vehicle)
		{
			Assert.AreEqual(RetarderType.LossesIncludedInTransmission, vehicle.RetarderType);
			Assert.AreEqual(2.000, vehicle.RetarderRatio);
		}

		private void TestxEvParametersSequenceGroup(IXMLDeclarationVehicleData vehicle)
		{
			Assert.AreEqual(true, vehicle.OvcHev);
			Assert.AreEqual(11.SI<Watt>(), vehicle.MaxChargingPower);
		}


		#endregion
		
		#region Heavy lorry group parmeters

		private void TestHeavyLorryParametersSequenceGroup1(IXMLDeclarationVehicleData vehicle)
		{
			Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
		}

		private void TestHeavyLorryParametersSequenceGroup2(IXMLDeclarationVehicleData vehicle)
		{
			Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.AngledriveType);
			Assert.AreEqual("None", vehicle.PTOTransmissionInputData.PTOTransmissionType);
			Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual(true, vehicle.VocationalVehicle);
			Assert.IsNull(vehicle.TankSystem);
			Assert.AreEqual(true, vehicle.SleeperCab);
			Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
		}

		#endregion

		#region Test medium lorry group parameters

		private void TestMediumLorryParametersSequenceGroup1(IXMLDeclarationVehicleData vehicle)
		{
			Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_6x2, vehicle.AxleConfiguration);
		}

		private void TestMediumLorryParameterSequenceGroup2(IXMLDeclarationVehicleData vehicle)
		{
			if (vehicle.VehicleCategory == VehicleCategory.Van) {
				Assert.AreEqual(20, vehicle.CargoVolume.Value());
			} else {
				Assert.IsNull(vehicle.CargoVolume);
			}

			Assert.AreEqual(AngledriveType.None, vehicle.AngledriveType);
			Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
			Assert.IsNull(vehicle.TankSystem);
			Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
		}

		#endregion

		#region Test primary bus group parameters

		private void TestPrimaryBusParametersSequenceGroup(IXMLDeclarationVehicleData vehicle)
		{
			Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
			Assert.AreEqual(true, vehicle.Articulated);
			Assert.AreEqual(26000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
		}

		private void TestChassisPrimaryBusParametersSequenceGroup(IXMLDeclarationVehicleData vehicle)
		{
			Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
			Assert.AreEqual(true, vehicle.Articulated);
			Assert.AreEqual(25000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
		}


		#endregion


		[TestCase(@"Conventional_heavyLorry_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalHeavyLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestHeavyLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestRetarderSequenceGroup(vehicle);
			TestHeavyLorryParametersSequenceGroup2(vehicle);
		}

		
		[TestCase(@"Conventional_mediumLorry_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalMediumLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestMediumLorryParametersSequenceGroup1(vehicle);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestChassisLorryParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			TestMediumLorryParameterSequenceGroup2(vehicle);
		}
		

		[TestCase(@"Conventional_primaryBus_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalPrimaryBusVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestPrimaryBusParametersSequenceGroup(vehicle);
			Assert.AreEqual(600.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestRetarderSequenceGroup(vehicle);
			Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.AngledriveType);
			Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
		}
		

		[TestCase(@"HEV_heavyLorry_AMT_Px_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVPxHeavyLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestHeavyLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestRetarderSequenceGroup(vehicle);
			TestHeavyLorryParametersSequenceGroup2(vehicle);
			Assert.AreEqual(ArchitectureID.P2_5, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}


		[TestCase(@"HEV_mediumLorry_AMT_Px_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVPxMediumLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestMediumLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestRetarderSequenceGroup(vehicle);
			TestMediumLorryParameterSequenceGroup2(vehicle);
			Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}
		

		[TestCase(@"HEV_primaryBus_AMT_Px_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVPxPrimaryBusVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestChassisPrimaryBusParametersSequenceGroup(vehicle);
			Assert.AreEqual(600.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestRetarderSequenceGroup(vehicle);
			Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.AngledriveType);
			Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}

		
		[TestCase(@"HEV-S_heavyLorry_AMT_S2_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"HEV-S_heavyLorry_S3_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"HEV-S_heavyLorry_S4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVSxHeavyLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestHeavyLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			TestHeavyLorryParametersSequenceGroup2(vehicle);
			switch (jobfile) {
				case "HEV-S_heavyLorry_AMT_S2_n_opt.xml":
					Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
					Assert.AreEqual(660.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
				case "HEV-S_heavyLorry_S3_n_opt.xml": 
					Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
					Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
				case "HEV-S_heavyLorry_S4_n_opt.xml":
					Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
					Assert.AreEqual(640.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
			}
			
			TestxEvParametersSequenceGroup(vehicle);
		}
		

		[TestCase(@"HEV-S_mediumLorry_AMT_S2_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"HEV-S_mediumLorry_S3_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"HEV-S_mediumLorry_S4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVSxMediumLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestMediumLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			TestMediumLorryParameterSequenceGroup2(vehicle);

			switch (jobfile)
			{
				case "HEV-S_mediumLorry_AMT_S2_n_opt.xml":
					Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
					Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
				case "HEV-S_mediumLorry_S3_n_opt.xml":
					Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
					Assert.AreEqual(660.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
				case "HEV-S_mediumLorry_S4_n_opt.xml":
					Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
					Assert.AreEqual(640.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
			}

			TestxEvParametersSequenceGroup(vehicle);
		}


		[TestCase(@"HEV-S_primaryBus_AMT_S2_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"HEV-S_primaryBus_S3_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"HEV-S_primaryBus_S4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVSxPrimaryBusVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestChassisPrimaryBusParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.AngledriveType);
			Assert.IsNull(vehicle.PTONode);
			Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);

			switch (jobfile)
			{
				case "HEV-S_primaryBus_AMT_S2_n_opt.xml":
					Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
					Assert.AreEqual(600.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
				case "HEV-S_primaryBus_S3_n_opt.xml":
					Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
					Assert.AreEqual(610.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
				case "HEV-S_primaryBus_S4_n_opt.xml":
					Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
					Assert.AreEqual(620.00.RPMtoRad(), vehicle.EngineIdleSpeed);
					break;
			}

			TestxEvParametersSequenceGroup(vehicle);
		}

		[TestCase(@"HEV-S_heavyLorry_IEPC-S_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEV_IEPC_S_HeavyLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestHeavyLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestRetarderSequenceGroup(vehicle);
			TestHeavyLorryParametersSequenceGroup2(vehicle);
			Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}
		

		[TestCase(@"HEV-S_mediumLorry_IEPC-S_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEV_IEPC_S_MediumLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestMediumLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestRetarderSequenceGroup(vehicle);
			TestMediumLorryParameterSequenceGroup2(vehicle);
			Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}

		[TestCase(@"HEV-S_primaryBus_IEPC-S_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEV_IEPC_S_PrimaryBusVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestChassisPrimaryBusParametersSequenceGroup(vehicle);
			Assert.AreEqual(600.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			TestRetarderSequenceGroup(vehicle);
			Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.AngledriveType);
			Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}

		[TestCase(@"PEV_heavyLorry_AMT_E2_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"PEV_heavyLorry_E3_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"PEV_heavyLorry_E4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVHeavyLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestHeavyLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			TestHeavyLorryParametersSequenceGroup2(vehicle);

			switch (jobfile)
			{
				case "PEV_heavyLorry_AMT_E2_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
					break;
				case "PEV_heavyLorry_E3_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
					break;
				case "PEV_heavyLorry_E4_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
					break;
			}

			TestxEvParametersSequenceGroup(vehicle);
		}


		[TestCase(@"PEV_mediumLorry_AMT_E2_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"PEV_mediumLorry_E3_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"PEV_mediumLorry_E4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVMediumLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestMediumLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			TestMediumLorryParameterSequenceGroup2(vehicle);


			switch (jobfile)
			{
				case "PEV_mediumLorry_AMT_E2_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
					break;
				case "PEV_mediumLorry_E3_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
					break;
				case "PEV_mediumLorry_E4_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
					break;
			}

			TestxEvParametersSequenceGroup(vehicle);
		}


		[TestCase(@"PEV_primaryBus_AMT_E2_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"PEV_primaryBus_E3_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"PEV_primaryBus_E4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVPrimaryBusVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestChassisPrimaryBusParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.AngledriveType);
			Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);

			switch (jobfile)
			{
				case "PEV_primaryBus_AMT_E2_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
					break;
				case "PEV_primaryBus_E3_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
					break;
				case "PEV_primaryBus_E4_n_opt.xml":
					Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
					break;
			}

			TestxEvParametersSequenceGroup(vehicle);
		}


		[TestCase(@"IEPC_heavyLorry_n_opt.xml", Optional_TESTS_DIR)]
		public void TestIEPCHeavyLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestHeavyLorryParametersSequenceGroup1(vehicle);
			TestChassisLorryParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			TestHeavyLorryParametersSequenceGroup2(vehicle);
			Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}


		[TestCase(@"IEPC_mediumLorry_n_opt.xml", Optional_TESTS_DIR)]
		public void TestIEPCMediumLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);

			TestChassisLorryParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			TestMediumLorryParameterSequenceGroup2(vehicle);
			Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}


		[TestCase(@"IEPC_primaryBus_n_opt.xml", Optional_TESTS_DIR)]
		public void TestIEPCPrimaryBusVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			TestChassisPrimaryBusParametersSequenceGroup(vehicle);
			TestRetarderSequenceGroup(vehicle);
			Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.AngledriveType);
			Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
			TestxEvParametersSequenceGroup(vehicle);
		}
	}
}
