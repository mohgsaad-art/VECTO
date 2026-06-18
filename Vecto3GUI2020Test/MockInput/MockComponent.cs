using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Moq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;

namespace Vecto3GUI2020Test.MockInput;

internal static class MockComponent
{
	public static IVehicleDeclarationInputData AddAirdragComponent(this IVehicleDeclarationInputData mocked, XNamespace version)
	{
		Mock.Get(mocked.Components).SetupGet(c => c.AirdragInputData).Returns((IAirdragDeclarationInputData)GetAirdragComponentData(version));
		return mocked;
	}

	public static IVehicleDeclarationInputData AddBusAux(this IVehicleDeclarationInputData mocked, XNamespace version, string xsdType)
	{
		Mock.Get(mocked.Components).SetupGet(c => c.BusAuxiliaries)
			.Returns((IBusAuxiliariesDeclarationData)GetBusAuxiliariesDeclarationData(version, xsdType));
		return mocked;
	}

	public static IVehicleDeclarationInputData AddADAS(this IVehicleDeclarationInputData mocked, XNamespace version,
		string xsdType)
	{
		var mock = Mock.Get(mocked);
		mock.SetupGet(m => m.ADAS).Returns(GetADAS(version, xsdType));
		return mock.Object;
	}

	public static IAdvancedDriverAssistantSystemDeclarationInputData GetADAS(XNamespace version, string xsdType)
	{
		var mock = new Mock<IAdvancedDriverAssistantSystemDeclarationInputData>();
		mock.SetupGet(m => m.ATEcoRollReleaseLockupClutch).Returns(false);
		mock.SetupGet(m => m.EcoRoll).Returns(EcoRollType.WithEngineStop);
		mock.SetupGet(m => m.EngineStopStart).Returns(true);
		mock.SetupGet(m => m.PredictiveCruiseControl).Returns(PredictiveCruiseControlType.Option_1_2_3);
		return mock.Object;
	}

	public static IAirdragDeclarationInputData GetAirdragComponentData(XNamespace version)
	{
		var mock = new Mock<IAirdragDeclarationInputData>();
		mock.SetupGet(a => a.Manufacturer).Returns("Manufacturer");
		mock.SetupGet(a => a.Model).Returns("Model");
		mock.SetupGet(a => a.Date).Returns(DateTime.Today);
		mock.SetupGet(a => a.AppVersion).Returns("Vecto AirDrag x.y");
		mock.SetupGet(a => a.CertificationNumber).Returns("e12*0815/8051*2017/05E0000*00");
		mock.SetupGet(a => a.SavedInDeclarationMode).Returns(true);
		mock.SetupGet(a => a.AirDragArea).Returns(6.66.SI<SquareMeter>());
		mock.SetupGet(a => a.AirDragArea_0).Returns(7.77.SI<SquareMeter>());
		mock.SetupGet(a => a.TransferredAirDragArea).Returns(8.88.SI<SquareMeter>());

		if (version == XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24)
		{
			mock.SetupGet(a => a.DataSource).Returns(
				MockInput.GetMockDataSource(XMLDeclarationAirdragDataProviderV24.XSD_TYPE, version));
		}
		else
		{
			mock.SetupGet(a => a.DataSource).Returns(
				MockInput.GetMockDataSource(XMLDeclarationAirdragDataProviderV10.XSD_TYPE, version));
		}


		//mock.SetupGet(a => a.DataSource.SourceVersion).Returns(version.GetVersionFromNamespaceUri);
		//mock.SetupGet(a => a.DataSource.Type).Returns(XMLDeclarationAirdragDataProviderV20.XSD_TYPE);
		//mock.SetupGet(a => a.DataSource.TypeVersion).Returns(version.NamespaceName);
		return mock.Object;
	}


	public static IBusAuxiliariesDeclarationData GetBusAuxiliariesDeclarationData(XNamespace version, string xsdType)
	{
		var mocked = new Mock<IBusAuxiliariesDeclarationData>();
		mocked.SetupGet(m => m.DataSource).Returns(MockInput.GetMockDataSource(xsdType, version));

		#region HVAC
		mocked.SetupGet(m => m.HVACAux.AdjustableAuxiliaryHeater).Returns(true); 
		mocked.SetupGet(m => m.HVACAux.AdjustableCoolantThermostat).Returns(true);
		mocked.SetupGet(m => m.HVACAux.AirElectricHeater).Returns(true);
		mocked.SetupGet(m => m.HVACAux.AuxHeaterPower).Returns(50.SI<Watt>());
		mocked.SetupGet(m => m.HVACAux.DoubleGlazing).Returns(true);
		mocked.SetupGet(m => m.HVACAux.EngineWasteGasHeatExchanger).Returns(true);
		mocked.SetupGet(m => m.HVACAux.HeatPumpTypeCoolingDriverCompartment).Returns(HeatPumpType.non_R_744_2_stage);
		mocked.SetupGet(m => m.HVACAux.HeatPumpTypeCoolingPassengerCompartment).Returns(HeatPumpType.non_R_744_2_stage);
		mocked.SetupGet(m => m.HVACAux.HeatPumpTypeHeatingDriverCompartment).Returns(HeatPumpType.non_R_744_2_stage);
		mocked.SetupGet(m => m.HVACAux.HeatPumpTypeHeatingPassengerCompartment).Returns(HeatPumpType.non_R_744_2_stage);
		mocked.SetupGet(m => m.HVACAux.SystemConfiguration).Returns(BusHVACSystemConfiguration.Configuration0);
		mocked.SetupGet(m => m.HVACAux.SeparateAirDistributionDucts).Returns(true);
		mocked.SetupGet(m => m.HVACAux.WaterElectricHeater).Returns(true);
		mocked.SetupGet(m => m.HVACAux.OtherHeatingTechnology).Returns(true);
		
		#endregion

		#region ElectricConsumers
		mocked.SetupGet(m => m.ElectricConsumers.BrakelightsLED).Returns(true);
		mocked.SetupGet(m => m.ElectricConsumers.DayrunninglightsLED).Returns(true);
		mocked.SetupGet(m => m.ElectricConsumers.HeadlightsLED).Returns(true);
		mocked.SetupGet(m => m.ElectricConsumers.InteriorLightsLED).Returns(true);
		mocked.SetupGet(m => m.ElectricConsumers.PositionlightsLED).Returns(true);
        #endregion


		//mocked.SetupGet(m => m.ElectricSupply.AlternatorTechnology).Returns(AlternatorType.Smart);
		//mocked.SetupGet(m => m.ElectricSupply.Alternators).Returns(new List<IAlternatorDeclarationInputData>() {
		//	GetAlternatorDeclarationInputData(), GetAlternatorDeclarationInputData(),
		//});
		//mocked.SetupGet(m => m.ElectricSupply.ESSupplyFromHEVREESS).Returns(true);
		//mocked.SetupGet(m => m.ElectricSupply.ElectricStorage)



		return mocked.Object;
	}

	private static IAlternatorDeclarationInputData GetAlternatorDeclarationInputData()
	{
		var mocked = new Mock<IAlternatorDeclarationInputData>();
		mocked.SetupGet(m => m.RatedCurrent).Returns(10.SI<Ampere>());
		mocked.SetupGet(m => m.RatedVoltage).Returns(48.SI<Volt>());
		return mocked.Object;
	}



	public static IVehicleDeclarationInputData SetAirdragVersion(this IVehicleDeclarationInputData mocked, XNamespace version, [CallerMemberName] string creatingMethod = "mocked")
	{
		if (mocked.Components.AirdragInputData == null)
		{
			throw new Exception("Airdrag not mocked");
		};

		var airdrag = Mock.Get(mocked.Components.AirdragInputData);
		airdrag.SetupGet((a) => a.DataSource).Returns(new DataSource()
		{
			SourceFile = creatingMethod,
			SourceType = DataSourceType.XMLFile,
			Type = XMLNames.AirDrag_Data_Type_Attr,
			SourceVersion = version.GetVersionFromNamespaceUri(),
		});


		return mocked;
	}
}