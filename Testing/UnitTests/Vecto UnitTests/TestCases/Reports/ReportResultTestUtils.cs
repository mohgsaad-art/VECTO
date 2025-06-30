using System.Text;
using System.Xml;
using System.Xml.Linq;
using Moq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports;

public class ReportResultTestUtils
{
	public static void WriteToConsole(XDocument doc)
	{
		var m = new MemoryStream();
		var writer = new XmlTextWriter(m, Encoding.UTF8) { Formatting = Formatting.Indented };
		doc.WriteTo(writer);
		writer.Flush();
		m.Flush();
		m.Seek(0, SeekOrigin.Begin);
		Console.WriteLine(new StreamReader(m).ReadToEnd());
	}

	private void WriteToFile(string prefix, XDocument doc, VectoRunData runData, bool success, bool exempted)
	{
		lock (this) {
			var fileName = GetFilename(prefix, runData, success, exempted);
			var filePath = Path.Combine("TestDummyResults", fileName);
			if (!Directory.Exists(Path.GetDirectoryName(filePath))) {
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			}

			if (File.Exists(filePath)) {
				File.Delete(filePath);
			}

			var writer = new XmlTextWriter(filePath, Encoding.UTF8) { Formatting = Formatting.Indented };
			doc.WriteTo(writer);
			writer.Flush();
		}
	}

	private string GetFilename(string prefix, VectoRunData runData, bool success, bool exempted)
	{
		var arch = string.Empty;
		switch (runData.JobType.GetPowertrainArchitectureType()) {
			case VectoSimulationJobTypeHelper.Hybrid:
				arch = (runData.VehicleData.OffVehicleCharging ? "OVC" : "non-OVC") + "-HEV";
				break;
			case VectoSimulationJobTypeHelper.Conventional:
				arch = "Conv";
				break;
			case VectoSimulationJobTypeHelper.PureElectric:
				arch = "PEV";
				break;
		}

		var category = runData.VehicleData.VehicleCategory.IsLorry() ? "Lorry" : "Bus";
		var suffix = success ? null : "_ERR";
		var exept = exempted ? "_exempted" : null;

		var fuelSuffix = "";
		var fuels = runData.EngineData.Fuels;
		if (fuels.Any(x => x.FuelData.FuelType != FuelType.DieselCI)) {
			fuelSuffix = "_" + fuels.Select(x => x.FuelData.FuelType.ToXMLFormat().Replace(' ', '-')).Join("_");
		}
		return $"{prefix}_MockupResults_{arch}_{category}{fuelSuffix}{suffix}{exept}.xml";
	}

    public static XMLDeclarationReport.ResultEntry GetResultEntry(VectoRunData runData)
	{
		var resultEntry = new XMLDeclarationReport.ResultEntry();
		resultEntry.Initialize(runData);
		return resultEntry;
	}

	public static XMLValidator GetValidator(XDocument doc)
	{
		var ms = new MemoryStream();
		var writer = new XmlTextWriter(ms, Encoding.UTF8);
		doc.WriteTo(writer);
		writer.Flush();
		ms.Flush();
		ms.Seek(0, SeekOrigin.Begin);
		return new XMLValidator(new XmlTextReader(ms));
	}

	public static XDocument CreateXmlDocument(XElement results, string reportType, XNamespace ns)
	{
		var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		var doc = new XDocument();
		doc.Add(new XElement(ns + "VectoMockResults",
			new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
			new XAttribute(xsi + "schemaLocation", $@"{ns.NamespaceName} V:/VectoCore/VectoCore/Resources/XSD/{reportType}.xsd"),
			results));

		return doc;
	}

	public static VectoRunData GetMockRunData(VehicleCategory vehicleCategory, VectoSimulationJobType jobType,
		bool offVehicleCharging, bool exempted, OvcHevMode ovcMode, FuelType[] fuelTypes, IMCTechnology? imcTech = null)
	{
		var fuels = fuelTypes == null || fuelTypes.Length == 0 ? new[] { FuelType.DieselCI } : fuelTypes;
		var retVal =  new VectoRunData() {
			Mission = new Mission() {
				MissionType = MissionType.LongHaul
			},
			OVCMode = ovcMode,
			InMotionCharging = imcTech.HasValue && imcTech != IMCTechnology.NotApplicable,
			InMotionChargingTechnology = imcTech ?? IMCTechnology.NotApplicable,
            Exempted = exempted,
			JobType = jobType,
			Loading = LoadingType.LowLoading,
			MaxChargingPower = 250.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
			VehicleData = new VehicleData() {
				CurbMass = 7600.SI<Kilogram>(),
				Loading = 5000.SI<Kilogram>(),
				CargoVolume = 20.SI<CubicMeter>(),
				PassengerCount = 20,
				VehicleClass = VehicleClass.Class5,
				VehicleCategory = vehicleCategory,
				OffVehicleCharging = offVehicleCharging,
				H2StorageUsableCapacity =  30.SI<Kilogram>()
            },
			EngineData = new CombustionEngineData() {
				FuelMode = 0,
				Fuels = fuels.Select(x => new CombustionEngineFuelData()
					{ FuelData = DeclarationData.FuelData.Lookup(x, TankSystem.Liquefied) }).ToList(),
			},
			Retarder = new RetarderData() {
				Type = RetarderType.None,
			},
			BatteryData = new BatterySystemData() {
				Batteries = new List<Tuple<int, BatteryData>>() {
					Tuple.Create(1, new BatteryData() {
						BatteryId = 0,
						Capacity = 7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						ChargeDepletingBattery = true,
						MinSOC = 0.2,
						MaxSOC = 0.8,
						SOCMap = BatterySOCReader.Create("SoC, V\n0, 600\n100, 650\n".ToStream()),
						InternalResistance = BatteryInternalResistanceReader.Create(
							"SoC, Ri-2, Ri-10, Ri-20\n0, 20, 20, 20\n100, 20, 20, 20\n".ToStream(), true),
						MaxCurrent =
							BatteryMaxCurrentReader.Create("SoC, I_charge, I_discharge\n0, 300, 300\n100, 500, 500\n"
								.ToStream())

					})
				}
			}
		};
		if (jobType.IsBatteryElectric() || jobType.IsFCHV()) {
			retVal.EngineData = null;
		}
		return retVal;
    }

	public static IModalDataContainer GetMockModData(VectoRun.Status runStatus, FuelType[] fuelTypes, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
	{
		var fuels = fuelTypes == null || fuelTypes.Length == 0 ? new[] { FuelType.DieselCI } : fuelTypes;

		var modData = new Mock<IModalDataContainer>();
		modData.Setup(x => x.RunStatus).Returns(runStatus);
		modData.Setup(x => x.Duration).Returns(3600.SI<Second>());
		modData.Setup(x => x.Distance).Returns(30000.SI<Meter>());
		modData.Setup(x => x.GetValues<MeterPerSecond>(ModalResultField.v_act)).Returns(new[] { 0.KMPHtoMeterPerSecond(), 50.KMPHtoMeterPerSecond() });
		modData.Setup(x => x.GetValues<MeterPerSquareSecond>(ModalResultField.acc)).Returns(new[] { -1.SI<MeterPerSquareSecond>(), 0.SI<MeterPerSquareSecond>(), 1.SI<MeterPerSquareSecond>() });
		modData.Setup(x => x.GetValues<uint>(ModalResultField.Gear)).Returns(new[] { 0u, 2u, 0u, 3u, 0u });

		var e_gbxIn = 1000.SI<WattSecond>();
		var gbxEff = 0.98;
		var axlEff = 0.97;
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn);
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff);
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff * axlEff);

		var batChgEff = 0.95;
		var batDischgEff = 0.93;
		var batEnergy = 200.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
		var factorChg = ovcMode.IsOneOf(OvcHevMode.ChargeSustaining, OvcHevMode.NotApplicable) ? 1 : 0.1;
		var batteryEntries = new[] {
			// internal , terminal
			Tuple.Create(batEnergy * factorChg, batEnergy * factorChg / batChgEff),
			Tuple.Create(-batEnergy, -batEnergy * batDischgEff)
		};
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, It.IsAny<Func<SI, bool>>()))
			.Returns<ModalResultField, Func<SI, bool>>((_, f) =>
				batteryEntries.Select(x => x.Item1).Where(x => f == null || f(x)).Sum());
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_terminal, It.IsAny<Func<SI, bool>>()))
			.Returns<ModalResultField,
				Func<SI, bool>>((_, f) => batteryEntries.Select(x => x.Item2).Where(x => f(x)).Sum());
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_terminal_ES, It.IsAny<Func<SI, bool>>()))
			.Returns<ModalResultField,
				Func<SI, bool>>((_, f) => batteryEntries.Select(x => x.Item2).Where(x => f(x)).Sum());
		modData.Setup(x => x.GetValues<SI>(ModalResultField.REESSStateOfCharge)).Returns(() => new[] { 50.SI(), 50.SI() });

        if (runStatus != VectoRun.Status.Success) {
			modData.Setup(x => x.Error).Returns("TestCase Error!");
			modData.Setup(x => x.StackTrace).Returns("Testcase Stacktrace");
		}

		var mc = new Mock<ICorrectedModalData>();
		modData.Setup(x => x.CorrectedModalData).Returns(mc.Object);

		var fcCorrected = new Dictionary<FuelType, IFuelConsumptionCorrection>();
		var ovcFactor = ovcMode == OvcHevMode.ChargeDepleting ? 0.1 : 1.0;
		foreach (var fuelType in fuels) {
			var factor = fcCorrected.Count == 0 ? 1 : 0.1;
			var fc = new Mock<IFuelConsumptionCorrection>();
			fc.Setup(x => x.Fuel).Returns(DeclarationData.FuelData.Lookup(fuelType, TankSystem.Liquefied));
			fc.Setup(x => x.TotalFuelConsumptionCorrected).Returns(31.SI<Kilogram>() * factor * ovcFactor);
			fc.Setup(x => x.EnergyDemand).Returns(31.SI<Kilogram>() * factor * ovcFactor * FuelData.Diesel.LowerHeatingValueVecto);
			fc.Setup(x => x.FC_AUXHTR_KM).Returns(0.SI<KilogramPerMeter>());
			fcCorrected.Add(fuelType, fc.Object);
		}
		mc.Setup(x => x.FuelCorrection).Returns(fcCorrected);

		mc.Setup(x => x.CO2Total).Returns(20.SI<Kilogram>());
		mc.Setup(x => x.FuelEnergyConsumptionTotal).Returns(1e9.SI<Joule>());

		var elOvcFactor = ovcMode == OvcHevMode.ChargeSustaining ? 0 : 1.0;
		mc.Setup(x => x.ElectricEnergyConsumption_Final).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);
		mc.Setup(x => x.ElectricEnergyConsumption_SoC).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);
		mc.Setup(x => x.ElectricEnergyConsumption_SoC_Corr).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);

		return modData.Object;
	}

	public static IDeclarationInputDataProvider GetMockInputData(int amdm)
	{
		var xmlType = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24 + ":FOO";

		var mock = new Mock<IDeclarationInputDataProvider>();
		var inputDataSource = new DataSource() {
			SourceType = DataSourceType.XMLFile
		};
		mock.Setup(i => i.DataSource).Returns(inputDataSource);
		var xmlNS = "";
		switch (amdm) {
			case 2:
				xmlNS = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
				break;
			case 3:
				xmlNS = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
				break;

		}

		var vehicleDataSource = new DataSource() {
			TypeVersion = xmlNS,
		};
		mock.Setup(i => i.JobInputData.Vehicle.DataSource).Returns(vehicleDataSource);

		var pack = new Mock<IBatteryPackDeclarationInputData>();
		var b1 = new Mock<IElectricStorageDeclarationInputData>();
		var bat = new Mock<IElectricStorageSystemDeclarationInputData>();
		pack.Setup(p => p.StorageType).Returns(REESSType.Battery);
		pack.Setup(p => p.MinSOC).Returns(0.2);
		pack.Setup(p => p.MaxSOC).Returns(0.8);
		pack.Setup(p => p.MaxCurrentMap).Returns(InputDataHelper.InputDataAsTableData("SoC, I_charge, I_discharge", new[] {"0, 300, 300", "100, 500, 500"}));
		pack.Setup(p => p.Capacity).Returns(7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>());
		pack.Setup(p => p.InternalResistanceCurve)
			.Returns(InputDataHelper.InputDataAsTableData("SoC, Ri-2, Ri-10, Ri-20",
				new[] { "0, 20, 20, 20", "100, 20, 20, 20" }));
		pack.Setup(p => p.VoltageCurve)
			.Returns(InputDataHelper.InputDataAsTableData("SoC, V", new[] { "0, 600", "100, 650" }));
		pack.Setup(p => p.DataSource).Returns(new DataSource() {
			SourceType = DataSourceType.XMLFile
		});
		b1.Setup(b => b.Count).Returns(1);
		b1.Setup(b => b.StringId).Returns(0);
		b1.Setup(b => b.REESSPack).Returns(pack.Object);
		bat.Setup(b => b.ElectricStorageElements).Returns(new[] { b1.Object });

		mock.Setup(i => i.JobInputData.Vehicle.Components.ElectricStorage).Returns(bat.Object);

		return mock.Object;
	}
}