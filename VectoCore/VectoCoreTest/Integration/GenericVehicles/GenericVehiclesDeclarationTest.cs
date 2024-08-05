using Ninject;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.GenericVehicles
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class GenericVehiclesDeclarationTest
	{
		private const string DeclarationBasePath = "TestData/Generic Vehicles/BaseGenericVehicles/DeclarationMode/";

		// ICE
		private const string GROUP_1s       = @$"{DeclarationBasePath}ICE/Group 1s/Group 1s_Rigid_DECL.vecto";
		private const string GROUP_2        = @$"{DeclarationBasePath}ICE/Group2_RigidTruck_4x2/Class2_RigidTruck_DECL.vecto";
		private const string GROUP_5        = @$"{DeclarationBasePath}ICE/Group5_Tractor_4x2/Class5_Tractor_DECL.vecto";
		private const string GROUP_53       = @$"{DeclarationBasePath}ICE/Group 53/ML3r.vecto";
		private const string GROUP_54_ML3   = @$"{DeclarationBasePath}ICE/Group 54/ML3van.vecto";
		private const string GROUP_54       = @$"{DeclarationBasePath}ICE/Group 54_xml/vecto_vehicle-medium_lorry_4x2.xml";
		private const string GROUP_9        = @$"{DeclarationBasePath}ICE/Group9_RigidTruck_6x2/Class9_RigidTruck_DECL.vecto";
		private const string GROUP_9_DF     = @$"{DeclarationBasePath}ICE/Group9_RigidTruck_6x2/Class9_RigidTruck_DECL_DF.vecto";
		private const string GROUP_9_DF_WHR = @$"{DeclarationBasePath}ICE/Group9_RigidTruck_6x2/Class9_RigidTruck_DECL_DF_WHR.vecto";
		private const string GROUP_9_WHR    = @$"{DeclarationBasePath}ICE/Group9_RigidTruck_6x2/Class9_RigidTruck_DECL_WHR.vecto";
		private const string GROUP_9_AT     = @$"{DeclarationBasePath}ICE/Group9_RigidTruck_AT_6x2/Class_9_RigidTruck_AT_Decl.vecto";
		private const string GROUP_9_AT_ECOROLL = @$"{DeclarationBasePath}ICE/Group9_RigidTruck_AT_6x2_EcoRoll/Class_9_RigidTruck_AT_Decl_EcoRoll.vecto";

		// BUS
		private const string PRIMARYBUS_P31_32 = @$"{DeclarationBasePath}ICE/Group P31_32_xml/primary_heavyBus_group_P31_32_Smart_ES.xml";
		private const string PRIMARYBUS_P33_34 = @$"{DeclarationBasePath}ICE/Group P33_34_xml/primary_heavyBus_group_P33_34_SmartPS.xml";
		private const string PRIMARYBUS_P35_36 = @$"{DeclarationBasePath}ICE/Group P35_36_xml/primary_heavyBus_group_P35_36_nonSmart.xml";
		private const string PRIMARYBUS_P37_38 = @$"{DeclarationBasePath}ICE/Group P37_38_xml/primary_heavyBus_group_P37_38_SmartES_PS.xml";
		private const string PRIMARYBUS_P39_40 = @$"{DeclarationBasePath}ICE/Group P39_40_xml/primary_heavyBus_group_P39_40_nonSmart_ESS.xml";
		private const string SINGLEBUS_31B	   = @$"{DeclarationBasePath}ICE/SingleBus_31b_vecto2xml/SingleBus31b.vecto";
		private const string SINGLEBUS_34F	   = @$"{DeclarationBasePath}ICE/SingleBus_34f_vecto2xml/SingleBus34f.vecto";
		
		// VTP
		private const string VTP_TRUCK = @$"{DeclarationBasePath}ICE/VTP_Truck_vecto2xml/VTP.vecto";
		
		// E2
		private const string E2_JOB         = @$"{DeclarationBasePath}PEV/GenericVehicleE2/BEV_E2.vecto";
		private const string E2_CONST30_JOB = @$"{DeclarationBasePath}PEV/GenericVehicleE2/BEV_E2_Cont30kW.vecto";
		private const string E2_PTO_JOB     = @$"{DeclarationBasePath}PEV/GenericVehicleE2/BEV_E2_PTO.vecto";

		// IEPC GBX1
		private const string IEPC_GBX1      = @$"{DeclarationBasePath}PEV/IEPC_Gbx1Speed/IEPC__Gbx1.vecto";
		private const string IEPC_GBX1_AXLE = @$"{DeclarationBasePath}PEV/IEPC_Gbx1Speed+Axle/IEPC__Gbx1Axl.vecto";
		private const string IEPC_GBX1_WHL1 = @$"{DeclarationBasePath}PEV/IEPC_Gbx1Speed-Whl1/IEPC__Gbx1Whl1.vecto";
		private const string IEPC_GBX1_WHL2 = @$"{DeclarationBasePath}PEV/IEPC_Gbx1Speed-Whl2/IEPC__Gbx1Whl2.vecto";

		// IEPC GBX3
		private const string IEPC_GBX3      = @$"{DeclarationBasePath}PEV/IEPC_Gbx3Speed/IEPC__Gbx3.vecto";
		private const string IEPC_GBX3_DRAG = @$"{DeclarationBasePath}PEV/IEPC_Gbx3Speed_drag/IEPC__Gbx3_drag.vecto";
		private const string IEPC_GBX3_AXLE = @$"{DeclarationBasePath}PEV/IEPC_Gbx3Speed+Axle/IEPC__Gbx3Axl.vecto";
		private const string IEPC_GBX3_WHL1 = @$"{DeclarationBasePath}PEV/IEPC_Gbx3Speed-Whl1/IEPC__Gbx3Whl1.vecto";
		private const string IEPC_GBX3_WHL2 = @$"{DeclarationBasePath}PEV/IEPC_Gbx3Speed-Whl2/IEPC__Gbx3Whl2.vecto";

		[OneTimeSetUp]
		public void TestInitialize()
		{
			GenericVehiclesInputFilesAdapter.CopyDeclarationInputFiles(DeclarationBasePath);
		}

		[Category("Integration")]
		[
			TestCase(GROUP_1s, "RegionalDelivery", 657.041),
			TestCase(GROUP_1s, "UrbanDelivery", 754.9479),

			TestCase(GROUP_2, "LongHaul", 776.7099),
			TestCase(GROUP_2, "RegionalDelivery", 567.5876),
			TestCase(GROUP_2, "UrbanDelivery", 768.7973),

			TestCase(GROUP_5, "LongHaul", 891.7875),
			TestCase(GROUP_5, "RegionalDelivery", 910.4309),
			TestCase(GROUP_5, "UrbanDelivery", 1572.4234),

			TestCase(GROUP_53, "RegionalDelivery", 468.9388),
			TestCase(GROUP_53, "UrbanDelivery", 487.5429),

			//TestCase(GROUP_54, "RegionalDelivery", 468.9388),
			//TestCase(GROUP_54, "UrbanDelivery", 487.5429),

			TestCase(GROUP_54_ML3, "RegionalDelivery", 285.7974),
			TestCase(GROUP_54_ML3, "UrbanDelivery", 373.9248),

			TestCase(GROUP_9, "LongHaul", 940.6617),
			TestCase(GROUP_9, "RegionalDelivery", 680.4892),

			TestCase(GROUP_9_DF, "LongHaul", 1003.3424),
			TestCase(GROUP_9_DF, "RegionalDelivery", 725.8333),

			TestCase(GROUP_9_DF_WHR, "LongHaul", 1001.4114),
			TestCase(GROUP_9_DF_WHR, "RegionalDelivery", 723.4348),

			TestCase(GROUP_9_WHR, "LongHaul", 938.0753),
			TestCase(GROUP_9_WHR, "RegionalDelivery", 677.2767),

			//TestCase(GROUP_9_AT, "LongHaul", 1136.1705),
			//TestCase(GROUP_9_AT, "RegionalDelivery", 845.8791),

			//TestCase(GROUP_9_AT_ECOROLL, "LongHaul", 1136.1705),
			//TestCase(GROUP_9_AT_ECOROLL, "RegionalDelivery", 845.8791)
		]
		public void ICE_DistanceRun(string jobFile, string cycleName, double expectedCO2_KM)
		{
			Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SumDataFields.CO2_KM, expectedCO2_KM }
			};

			RunDistanceCycle(jobFile, cycleName, LoadingType.ReferenceLoad, metrics);
		}

		[Category("Integration")]
		[
			TestCase(E2_JOB , "LongHaul", 155.3338),
			TestCase(E2_JOB , "RegionalDelivery", 106.2072),
			TestCase(E2_JOB, "UrbanDelivery", 85.2745),

			//TestCase(E2_CONST30_JOB , "LongHaul", 754.9479),
			//TestCase(E2_CONST30_JOB , "RegionalDelivery", 754.9479),
			//TestCase(E2_CONST30_JOB , "UrbanDelivery", 754.9479),

			//TestCase(E2_PTO_JOB, "LongHaul", 776.7099),
			//TestCase(E2_PTO_JOB, "RegionalDelivery", 776.7099),
			//TestCase(E2_PTO_JOB, "UrbanDelivery", 776.7099),

			//TestCase(IEPC_GBX1, "LongHaul", 567.5876),
			//TestCase(IEPC_GBX1, "RegionalDelivery", 567.5876),
			//TestCase(IEPC_GBX1, "UrbanDelivery", 567.5876),

			//TestCase(IEPC_GBX1_AXLE, "LongHaul", 768.7973),
			//TestCase(IEPC_GBX1_AXLE, "RegionalDelivery", 768.7973),
			//TestCase(IEPC_GBX1_AXLE, "UrbanDelivery", 768.7973),

			//TestCase(IEPC_GBX1_WHL1, "LongHaul", 891.7875),
			//TestCase(IEPC_GBX1_WHL1, "RegionalDelivery", 891.7875),
			//TestCase(IEPC_GBX1_WHL1, "UrbanDelivery", 891.7875),

			TestCase(IEPC_GBX1_WHL2, "LongHaul", 143.2381),
			TestCase(IEPC_GBX1_WHL2, "RegionalDelivery", 94.2429),
			TestCase(IEPC_GBX1_WHL2, "UrbanDelivery", 69.7222),

			//TestCase(IEPC_GBX3, "LongHaul", 1572.4234),
			//TestCase(IEPC_GBX3, "RegionalDelivery", 1572.4234),
			//TestCase(IEPC_GBX3, "UrbanDelivery", 1572.4234),

			//TestCase(IEPC_GBX3_DRAG, "LongHaul", 468.9388),
			//TestCase(IEPC_GBX3_DRAG, "RegionalDelivery", 468.9388),
			//TestCase(IEPC_GBX3_DRAG, "UrbanDelivery", 468.9388),

			//TestCase(IEPC_GBX3_AXLE, "LongHaul", 487.5429),
			//TestCase(IEPC_GBX3_AXLE, "RegionalDelivery", 487.5429),
			//TestCase(IEPC_GBX3_AXLE, "UrbanDelivery", 487.5429),

			//TestCase(IEPC_GBX3_WHL1, "LongHaul", 285.7974),
			//TestCase(IEPC_GBX3_WHL1, "RegionalDelivery", 285.7974),
			//TestCase(IEPC_GBX3_WHL1, "UrbanDelivery", 285.7974),

			TestCase(IEPC_GBX3_WHL2, "LongHaul", 122.8651),
			TestCase(IEPC_GBX3_WHL2, "RegionalDelivery", 77.1886),
			TestCase(IEPC_GBX3_WHL2, "UrbanDelivery", 60.5621),
		]
		public void PEV_DistanceRun(string jobFile, string cycleName, double expectedECFinal)
		{
			Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SumDataFields.EC_el_final, expectedECFinal }
			};

			RunDistanceCycle(jobFile, cycleName, LoadingType.ReferenceLoad, metrics);
		}

		public void RunDistanceCycle(
			string jobFile,
			string cycleName,
			LoadingType loading,
			Dictionary<string, double> metrics)
		{
			// Arrange.
			var inputProvider = Path.GetExtension(jobFile) == ".xml" ?
				new XMLInputDataFactory().Create(jobFile) :
				JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var fileWriter = new FileOutputWriter(outputFile);

			var ninjectKernel = new StandardKernel(new VectoNinjectModule());
			var factory = ninjectKernel
				.Get<ISimulatorFactoryFactory>()
				.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, false);
			factory.Validate = false;

			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter, new JobArchiveBuilder());
			jobContainer.AddRuns(factory);

			IVectoRun cycleToRun = factory.SimulationRuns()
				.Where(r => r.CycleName == cycleName && r.RunSuffix == loading.ToString())
				.FirstOrDefault();

			Assert.IsNotNull(cycleToRun, $"Cycle {cycleName} is not configured for the current vehicle.");

			// Act.
			cycleToRun.Run();

			// Assert.
			Assert.IsTrue(cycleToRun.FinishedWithoutErrors);

			AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
		}
	}
}
