using System.Xml;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.InputData.XML.DataProvider;

public class DataProviderFactory_v23_Tests
{
	private StandardKernel _kernel;
	private IDeclarationInjectFactory _declarationFactory;

	[SetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
	}


	[TestCase("Vehicle_Conventional_HeavyLorryDeclarationType", typeof(XMLDeclarationConventionalHeavyLorryDataProviderV24), VectoSimulationJobType.ConventionalVehicle)]
	[TestCase("Vehicle_HEV-Px_HeavyLorryDeclarationType", typeof(XMLDeclarationHevPxHeavyLorryDataProviderV24), null)] // can be parallel or ihpc - depends on components which are not available
	[TestCase("Vehicle_HEV-Sx_HeavyLorryDeclarationType", typeof(XMLDeclarationHevSxHeavyLorryDataProviderV24), VectoSimulationJobType.SerialHybridVehicle)]
	[TestCase("Vehicle_PEV_HeavyLorryDeclarationType", typeof(XMLDeclarationPevHeavyLorryDataProviderV24), VectoSimulationJobType.BatteryElectricVehicle)]
	[TestCase("Vehicle_IEPC_HeavyLorryDeclarationType", typeof(XMLDeclarationIEPCHeavyLorryDataProviderV24), VectoSimulationJobType.IEPC_E)]
	[TestCase("Vehicle_HEV-IEPC-S_HeavyLorryDeclarationType", typeof(XMLDeclarationHevIEPCSHeavyLorryDataProviderV24), VectoSimulationJobType.IEPC_S)]

	[TestCase("Vehicle_Conventional_MediumLorryDeclarationType", typeof(XMLDeclarationConventionalMediumLorryVehicleDataProviderV24), VectoSimulationJobType.ConventionalVehicle)]
	[TestCase("Vehicle_HEV-Px_MediumLorryDeclarationType", typeof(XMLDeclarationHevPxMediumLorryDataProviderV24), VectoSimulationJobType.ParallelHybridVehicle)]
	[TestCase("Vehicle_HEV-Sx_MediumLorryDeclarationType", typeof(XMLDeclarationHevSxMediumLorryDataProviderV24), VectoSimulationJobType.SerialHybridVehicle)]
	[TestCase("Vehicle_PEV_MediumLorryDeclarationType", typeof(XMLDeclarationPevMediumLorryExDataProviderV24), VectoSimulationJobType.BatteryElectricVehicle)]
	[TestCase("Vehicle_IEPC_MediumLorryDeclarationType", typeof(XMLDeclarationIepcMediumLorryDataProviderV24), VectoSimulationJobType.IEPC_E)]
	[TestCase("Vehicle_HEV-IEPC-S_MediumLorryDeclarationType", typeof(XMLDeclarationHeviepcsMediumLorryDataProviderV24), VectoSimulationJobType.IEPC_S)]

	[TestCase("Vehicle_Conventional_PrimaryBusDeclarationType", typeof(XMLDeclarationConventionalPrimaryBusVehicleDataProviderV24), VectoSimulationJobType.ConventionalVehicle)]
	[TestCase("Vehicle_HEV-Px_PrimaryBusDeclarationType", typeof(XMLDeclarationHevPxPrimaryBusDataProviderV24), null)] // can be parallel or ihpc - depends on components which are not available
    [TestCase("Vehicle_HEV-Sx_PrimaryBusDeclarationType", typeof(XMLDeclarationHevSxPrimaryBusDataProviderV24), VectoSimulationJobType.SerialHybridVehicle)]
	[TestCase("Vehicle_PEV_PrimaryBusDeclarationType", typeof(XMLDeclarationPevPrimaryBusDataProviderV24), VectoSimulationJobType.BatteryElectricVehicle)]
	[TestCase("Vehicle_IEPC_PrimaryBusDeclarationType", typeof(XMLDeclarationIepcPrimaryBusDataProviderV24), VectoSimulationJobType.IEPC_E)]
	[TestCase("Vehicle_HEV-IEPC-S_PrimaryBusDeclarationType", typeof(XMLDeclarationHeviepcsPrimaryBusDataProviderV24), VectoSimulationJobType.IEPC_S)]

	[TestCase("Vehicle_Conventional_CompletedBusDeclarationType", typeof(XMLDeclarationConventionalCompletedBusDataProviderV24), VectoSimulationJobType.ConventionalVehicle)]
	[TestCase("Vehicle_HEV_CompletedBusDeclarationType", typeof(XMLDeclarationHevCompletedBusDataProviderV24), null)] // depends on architecture id which is not available
	[TestCase("Vehicle_PEV_CompletedBusDeclarationType", typeof(XMLDeclarationPEVCompletedBusDataProviderV24), VectoSimulationJobType.BatteryElectricVehicle)]
	[TestCase("Vehicle_IEPC_CompletedBusDeclarationType", typeof(XMLDeclarationIepcCompletedBusDataProviderV24), VectoSimulationJobType.IEPC_E)]
	
	[TestCase("Vehicle_Exempted_HeavyLorryDeclarationType", typeof(XMLDeclarationExemptedHeavyLorryDataProviderV24), VectoSimulationJobType.ConventionalVehicle)]
	[TestCase("Vehicle_Exempted_MediumLorryDeclarationType", typeof(XMLDeclarationExemptedMediumLorryDataProviderV24), VectoSimulationJobType.ConventionalVehicle)]
	[TestCase("Vehicle_Exempted_PrimaryBusDeclarationType", typeof(XMLDeclarationExemptedPrimaryBusDataProviderV24), VectoSimulationJobType.ConventionalVehicle)]
	[TestCase("Vehicle_Exempted_CompletedBusDeclarationType", typeof(XMLDeclarationExemptedCompletedBusDataProviderV24), VectoSimulationJobType.ConventionalVehicle)]

    public void TestXMLVehicleDataProviderFactory_v24(string xmlTypeName, Type expectedType, VectoSimulationJobType? expectedJobType)
	{
		XmlNode xmlNode = null;
		var jobData = new Mock<IXMLDeclarationJobInputData>();
		var version = XMLHelper.CombineNamespace(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, xmlTypeName);
		var vehicleData = _declarationFactory.CreateVehicleData(version, jobData.Object, xmlNode, "", false);

		Assert.AreEqual(expectedType, vehicleData.GetType());

		if (expectedJobType.HasValue) {
			Assert.AreEqual(expectedJobType, vehicleData.VehicleType);
		}
	}


	[TestCase("Components_Conventional_LorryType", typeof(XMLDeclarationComponentsDataProviderV24_Lorry))]
	[TestCase("Components_Conventional_PrimaryBusType", typeof(XMLDeclarationPrimaryBusComponentsDataProviderV24))]
	[TestCase("Components_xEV_CompletedBusType", typeof(XMLDeclarationCompletedBusComponentsDataProviderV24))]
	[TestCase("Components_HEV-Px_LorryType", typeof(XMLDeclarationHEVPxLorryComponentsDataProviderV24))]
	[TestCase("Components_HEV-S2_LorryType", typeof(XMLDeclarationHevs2LorryComponentsDataProviderV24))]
	[TestCase("Components_HEV-S3_LorryType", typeof(XMLDeclarationHEVSXLorryComponentsDataProviderV24))]
	[TestCase("Components_HEV-S4_LorryType", typeof(XMLDeclarationHEVSXLorryComponentsDataProviderV24))]
	[TestCase("Components_HEV-Px_PrimaryBusType", typeof(XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV24))]
	[TestCase("Components_HEV-S2_PrimaryBusType", typeof(XMLDeclarationPrimaryBusHEVS2ComponentDataProviderV24))]
	[TestCase("Components_HEV-S3_PrimaryBusType", typeof(XMLDeclarationPrimaryBusHEVSxComponentDataProviderV24))]
	[TestCase("Components_HEV-S4_PrimaryBusType", typeof(XMLDeclarationPrimaryBusHEVSxComponentDataProviderV24))]
	[TestCase("Components_HEV-IEPC-S_LorryType", typeof(XMLDeclarationHeavyLorryHEVIEPCSComponentDataV24))]
	[TestCase("Components_HEV-IEPC-S_PrimaryBus", typeof(XMLDeclarationPrimaryBusHEVIEPCSComponentDataV24))]
	[TestCase("Components_PEV-E2_LorryType", typeof(XMLDeclarationHeavyLorryPEVE2ComponentDataV24))]
	[TestCase("Components_PEV-E3_LorryType", typeof(XMLDeclarationHeavyLorryPevExComponentDataV24))]
	[TestCase("Components_PEV-E4_LorryType", typeof(XMLDeclarationHeavyLorryPevExComponentDataV24))]
	[TestCase("Components_IEPC_LorryType", typeof(XMLDeclarationIEPCHeavyLorryComponentDataV24))]
	[TestCase("Components_IEPC_PrimaryBusType", typeof(XMLDeclarationIEPCPrimaryBusComponentDataV24))]
	public void TestXMLVehicleComponentDataProviderFactory_v24(string xmltypeName, Type expectedType)
	{
		var version = XMLHelper.CombineNamespace(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, xmltypeName);
		var componentData = _declarationFactory.CreateComponentData(version, null, null, "");

		Assert.AreEqual(expectedType, componentData.GetType());
	}


	[TestCase("Vehicle_Conventional_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_Conventional_DataProviderV01))]
	[TestCase("Vehicle_HEV-Px_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_Px_DataProviderV01))]
	[TestCase("Vehicle_HEV-S2_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S2_DataProviderV01))]
	[TestCase("Vehicle_HEV-S3_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S3_DataProviderV01))]
	[TestCase("Vehicle_HEV-S4_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_S4_DataProviderV01))]
	[TestCase("Vehicle_HEV-IEPC-S_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_HEV_IEPC_S_DataProviderV01))]
	[TestCase("Vehicle_PEV-E2_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E2_DataProviderV01))]
	[TestCase("Vehicle_PEV-E3_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E3_DataProviderV01))]
	[TestCase("Vehicle_PEV-E4_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_E4_DataProviderV01))]
	[TestCase("Vehicle_PEV-IEPC_ComponentsVIFType", typeof(XMLDeclarationComponentsMultistagePrimaryVehicleBus_PEV_IEPC_DataProviderV01))]
	public void TestXMLVehicleComponentDataProviderFactory_VIF_v01(string xmltypeName, Type expectedType)
	{
		var version = XMLHelper.CombineNamespace(XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1, xmltypeName);
		var componentData = _declarationFactory.CreateComponentData(version, null, null, "");

		Assert.AreEqual(expectedType, componentData.GetType());
	}

	[TestCase("ADAS_Conventional_Type", typeof(XMLDeclarationADASDataConventionalProviderV24), null, null)]
	[TestCase("ADAS_HEV_Type", typeof(XMLDeclarationADASDataHEVProviderV24), EcoRollType.None, null)]
	[TestCase("ADAS_PEV_Type", typeof(XMLDeclarationADASDataPEVProviderV24), EcoRollType.None, false)]
	[TestCase("ADAS_IEPC_Type", typeof(XMLDeclarationADASDataIEPCProviderV24), EcoRollType.None, false)]
	public void TestXMLVehicleADASDataProviderFactory_VIF_v01(string xmltypeName, Type expectedType, EcoRollType? expectedEcoRoll, bool? expectedEngineStopStart)
	{
		var version = XMLHelper.CombineNamespace(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, xmltypeName);
		var adasData = _declarationFactory.CreateADASData(version, null, null, "");

		Assert.AreEqual(expectedType, adasData.GetType());

		if (expectedEcoRoll.HasValue) {
			Assert.AreEqual(expectedEcoRoll, adasData.EcoRoll);
		}

		if (expectedEngineStopStart.HasValue) {
			Assert.AreEqual(expectedEngineStopStart, adasData.EngineStopStart);
		}
	}
}