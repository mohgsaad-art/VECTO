using System;
using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockVehicleInputData : IVehicleDeclarationInputData
	{
		public DataSource DataSource { get; set; }

		public bool SavedInDeclarationMode { get; set; }

		public string Manufacturer { get; set; }

		public string Model { get; set; }

		public DateTime Date { get; set; }

		public string AppVersion { get; }

		public CertificationMethod CertificationMethod { get; set; }

		public string CertificationNumber { get; set; }

		public DigestData DigestValue { get; set; }

		public string Identifier { get; set; }

		public bool ExemptedVehicle { get; set; }

		public string VIN { get; set; }

		public TUGraz.VectoCommon.Models.LegislativeClass? LegislativeClass { get; set; }

		public VehicleCategory VehicleCategory { get; set; }

		public AxleConfiguration AxleConfiguration { get; set; }

		public Kilogram CurbMassChassis { get; set; }

		public Kilogram GrossVehicleMassRating { get; set; }

		public IList<ITorqueLimitInputData> TorqueLimits { get; set; }

		public string ManufacturerAddress { get; set; }

		public PerSecond EngineIdleSpeed { get; set; }

		public bool VocationalVehicle { get; set; }

		public bool? SleeperCab { get; set; }

		public bool? AirdragModifiedMultistep { get; }

		public TUGraz.VectoCommon.InputData.TankSystem? TankSystem { get; set; }

		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get; set; }

		public bool ZeroEmissionVehicle { get; set; }

		public bool HybridElectricHDV { get; set; }

		public bool DualFuelVehicle { get; set; }

		public Watt MaxNetPower1 { get; set; }

		public Watt MaxNetPower2 { get; set; }

		public string ExemptedTechnology { get; }

		public RegistrationClass? RegisteredClass { get; }

		public int? NumberPassengerSeatsUpperDeck { get; }

		public int? NumberPassengerSeatsLowerDeck { get; }

		public int? NumberPassengersStandingLowerDeck { get; }

		public int? NumberPassengersStandingUpperDeck { get; }

		public CubicMeter CargoVolume { get; }

		public TUGraz.VectoCommon.Models.VehicleCode? VehicleCode { get; }

		public bool? LowEntry { get; }

		public bool Articulated { get; }

		public Meter Height { get; }

		public Meter Length { get; }

		public Meter Width { get; }

		public Meter EntranceHeight { get; }

		public ConsumerTechnology? DoorDriveTechnology { get; }

		public VehicleDeclarationType VehicleDeclarationType { get; }

		public Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits { get; }

		public TableData BoostingLimitations => throw new NotImplementedException();

		public IVehicleComponentsDeclaration Components { get; set; }

		public XmlNode XMLSource { get; }

		public string VehicleTypeApprovalNumber { get; }

		public ArchitectureID ArchitectureID { get; }

		public bool OvcHev { get; }

		public Watt MaxChargingPower { get; }
	}
}