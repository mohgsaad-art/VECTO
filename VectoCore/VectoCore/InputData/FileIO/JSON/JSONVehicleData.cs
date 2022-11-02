/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONVehicleDataV11_IEPC : JSONVehicleDataV10_HEV_BEV
	{
		public JSONVehicleDataV11_IEPC(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing)
		{ }

		public override IIEPCEngineeringInputData IEPCEngineeringInputData => JSONInputDataFactory.ReadIEPCEngineeringInputData(
			Path.Combine(BasePath, Body.GetEx<string>("IEPC")), false);

		public override IIEPCDeclarationInputData IEPC => JSONInputDataFactory.ReadIEPCEngineeringInputData(
			Path.Combine(BasePath, Body.GetEx<string>("IEPC")), false);

		#region Overrides of JSONVehicleDataV10_HEV_BEV

		public override TableData BoostingLimitations => null;
		
		#endregion
	}

	public class JSONVehicleDataV10_HEV_BEV : JSONVehicleDataV9
	{
		private JSONElectricStorageSystemEngineeringInputData _batteries;
		private JSONElectricMotors _electricMotors;

		public JSONVehicleDataV10_HEV_BEV(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing)
		{ }

		#region Overrides of JSONVehicleDataV7

		public override double InitialSOC => Body.GetEx<double>("InitialSoC") / 100.0;

		protected override IRetarderInputData GetRetarder => _retarderInputData ?? (_retarderInputData = new JSONRetarderInputData(this));

		protected override IElectricMachinesEngineeringInputData GetElectricMachines()
		{
			return _electricMotors ?? (_electricMotors = ReadMotors());
		}

		protected override IElectricStorageSystemEngineeringInputData GetElectricStorage()
		{
			return _batteries ?? (_batteries = ReadBatteries());
		}

		public override VectoSimulationJobType VehicleType
		{
			get {
				switch (Body.GetEx<String>("PowertrainConfiguration")) {
					case "ParallelHybrid": return VectoSimulationJobType.ParallelHybridVehicle;
					case "BatteryElectric": return VectoSimulationJobType.BatteryElectricVehicle;
					case "SerialHybrid": return VectoSimulationJobType.SerialHybridVehicle;
					case "IEPC_E":
					case "IEPC": return VectoSimulationJobType.IEPC_E;
					case "IEPC_S":
					case "IEPC-S": return VectoSimulationJobType.IEPC_S;
					case "IHPC": return VectoSimulationJobType.IHPC;
					default: throw new VectoException("Invalid parameter value {0}", Body.GetEx<String>("PowertrainConfiguration"));
				}
			}
		}

		protected virtual JSONElectricMotors ReadMotors()
		{
			var retVal = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>();
			foreach (var entry in Body["ElectricMotors"]) {
				var tmp = new ElectricMachineEntry<IElectricMotorEngineeringInputData> {
					Position = PowertrainPositionHelper.Parse(entry.GetEx<string>("Position")),
					RatioADC = entry.GetEx<double>("Ratio"),
					RatioPerGear = entry["RatioPerGear"] != null
						? entry["RatioPerGear"].Select(x => x.Value<double>()).ToArray()
						: new double[] { },
					MechanicalTransmissionEfficiency = entry["MechanicalEfficiency"] != null
						? entry.GetEx<double>("MechanicalEfficiency")
						: double.NaN,
					MechanicalTransmissionLossMap = entry["MechanicalTransmissionLossMap"] != null
						? ReadTableData(Path.Combine(BasePath, entry.GetEx<string>("MechanicalTransmissionLossMap")),
							"EM ADC LossMap")
						: null,
					Count = entry.GetEx<int>("Count"),
					ElectricMachine = 
						JSONInputDataFactory.ReadElectricMotorData(
							Path.Combine(BasePath, entry.GetEx<string>("MotorFile")), false)
				};
				retVal.Add(tmp);
			}

			return new JSONElectricMotors(retVal);
		}

		protected override IAdvancedDriverAssistantSystemsEngineering GetADS()
		{
			if (_adasInputData != null)
				return _adasInputData;

			switch (VehicleType) {
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
					return _adasInputData = new JSONADASInputDataV10BEV(this);
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.IHPC:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IEPC_S:
					return _adasInputData = new JSONADASInputDataV10HEV(this);
				default:
					return base.GetADS();
			}
		}


		protected virtual JSONElectricStorageSystemEngineeringInputData ReadBatteries()
		{
			var entries = new List<IElectricStorageEngineeringInputData>();
			if (Body["Batteries"] != null) {
				foreach (var entry in Body["Batteries"]) {
					entries.Add(new JSONElectricStorageEngineeringInputData {
						Count = entry.GetEx<int>("NumPacks"),
						StringId = entry.GetEx<int>("StreamId"),
						REESSPack = JSONInputDataFactory.ReadREESSData(
							Path.Combine(BasePath, entry.GetEx<string>("BatteryFile")), false)
					});
				}
			} else {
				entries.Add(new JSONElectricStorageEngineeringInputData {
					Count = Body["Battery"].GetEx<int>("NumPacks"),
					StringId = 0,
					REESSPack = JSONInputDataFactory.ReadREESSData(Path.Combine(BasePath, Body["Battery"].GetEx<string>("BatteryFile")), false)
				});
			}

			return new JSONElectricStorageSystemEngineeringInputData(entries);
		}

		//public override Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits =>
		//	throw new NotImplementedException();
		public override Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits =>
			Body["EMTorqueLimits"] == null
				? null
				: new Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>>() {
					{
						GetElectricMachines().Entries.First().Position,
						new List<Tuple<Volt, TableData>>() {
							Tuple.Create((Volt)null, ReadTableData(
								Path.Combine(BasePath, Body.GetEx<string>("EMTorqueLimits")),
								"ElectricMotorTorqueLimits"))
						}
					}
				};

        public override TableData BoostingLimitations =>
			Body["MaxPropulsionTorque"] == null
				? null
				: ReadTableData(Path.Combine(BasePath, Body.GetEx<string>("MaxPropulsionTorque")),
					"MaxPropulsionTorque");

		#endregion
	}


	// ###################################################################
	// ###################################################################

	public class JSONVehicleDataV9 : JSONVehicleDataV7
	{
		protected IBusAuxiliariesDeclarationData _busAuxiliariesData;

		public JSONVehicleDataV9(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing)
		{ }

		#region Overrides of JSONVehicleDataV7

		public override IBusAuxiliariesDeclarationData BusAuxiliaries => _busAuxiliariesData ?? (_busAuxiliariesData = new JSONBusAuxiliariesData(this));

		#region Overrides of JSONVehicleDataV7

		public override bool Articulated => Body.GetEx<bool>("Articulated");

		protected override IAdvancedDriverAssistantSystemsEngineering GetADS()
		{
			return _adasInputData ?? (_adasInputData = new JSONADASInputDataV9(this));
		}

		#endregion

		#endregion
	}

	// ###################################################################
	// ###################################################################

	public class JSONVehicleDataV8 : JSONVehicleDataV7
	{
		public JSONVehicleDataV8(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false) :
			base(data, fileName, job, tolerateMissing)
		{ }



		public override TankSystem? TankSystem => Body["TankSystem"]?.ToString().ParseEnum<TankSystem>();

		protected override IAdvancedDriverAssistantSystemsEngineering GetADS()
		{
			return _adasInputData ?? (_adasInputData = new JSONADASInputDataV8(this));
		}
	}

	// ###################################################################
	// ###################################################################


	public class JSONVehicleDataV7 : JSONFile, IVehicleEngineeringInputData,
		IVehicleComponentsDeclaration, IVehicleComponentsEngineering, IAxlesEngineeringInputData, IAxlesDeclarationInputData
	//IAdvancedDriverAssistantSystemsEngineering, IAdvancedDriverAssistantSystemDeclarationInputData

	{
		public JSONVehicleDataV7(JObject data, string fileName, IJSONVehicleComponents job, bool tolerateMissing = false)
			: base(data, fileName, tolerateMissing)
		{
			Job = job;
		}

		private IJSONVehicleComponents Job;
		protected IRetarderInputData _retarderInputData;
		protected IAngledriveInputData _angledriveData;
		protected IAirdragEngineeringInputData _airdragInputData;
		protected IPTOTransmissionInputData _ptoInputData;
		protected IAdvancedDriverAssistantSystemsEngineering _adasInputData;

		#region IVehicleInputData

		public virtual string Identifier => Path.GetFileNameWithoutExtension(_sourceFile);

		public virtual bool ExemptedVehicle => false;

		public virtual string VIN => Constants.NOT_AVAILABLE;

		public virtual LegislativeClass? LegislativeClass =>
			Body["LegislativeClass"]?.Value<string>().ParseEnum<LegislativeClass>() ?? VectoCommon.Models.LegislativeClass.Unknown;

		public virtual VehicleCategory VehicleCategory => (VehicleCategory)Enum.Parse(typeof(VehicleCategory), Body[JsonKeys.Vehicle_VehicleCategory].Value<string>(), true);

		public virtual Kilogram CurbMassChassis => Body.GetEx<double>(JsonKeys.Vehicle_CurbWeight).SI<Kilogram>();

		public virtual Kilogram CurbMassExtra => Body.GetEx<double>(JsonKeys.Vehicle_CurbWeightExtra).SI<Kilogram>();

		public virtual Kilogram GrossVehicleMassRating => Body.GetEx<double>(JsonKeys.Vehicle_GrossVehicleMassRating).SI(Unit.SI.Ton).Cast<Kilogram>();

		public virtual IList<ITorqueLimitInputData> TorqueLimits
		{
			get {
				var retVal = new List<ITorqueLimitInputData>();
				if (Body["TorqueLimits"] == null) {
					return retVal;
				}

				foreach (var entry in (JObject)Body["TorqueLimits"]) {
					retVal.Add(
						new TorqueLimitInputData {
							Gear = entry.Key.ToInt(),
							MaxTorque = entry.Value.ToString().ToDouble(0).SI<NewtonMeter>()
						});
				}

				return retVal;
			}
		}

		public virtual Kilogram Loading => Body.GetEx<double>(JsonKeys.Vehicle_Loading).SI<Kilogram>();

		public virtual Meter DynamicTyreRadius => Body.GetEx<double>(JsonKeys.Vehicle_DynamicTyreRadius).SI(Unit.SI.Milli.Meter).Cast<Meter>();

		public virtual bool Articulated => false;

		public virtual Meter Height => Body["VehicleHeight"] == null ? null : Body.GetEx<double>("VehicleHeight").SI<Meter>();
		
		public virtual Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits => null;
		
		public virtual TableData BoostingLimitations => null;
		
		public virtual Meter Length => null;

		public virtual Meter Width => null;

		public virtual Meter EntranceHeight => null;
		public virtual ConsumerTechnology? DoorDriveTechnology => ConsumerTechnology.Unknown;
		public virtual VehicleDeclarationType VehicleDeclarationType { get; }

		IVehicleComponentsEngineering IVehicleEngineeringInputData.Components => this;

		public int? NumSteeredAxles => null;
		XmlNode IVehicleDeclarationInputData.XMLSource => null;
		public virtual string VehicleTypeApprovalNumber { get; }
		public ArchitectureID ArchitectureID { get; }
		public bool OvcHev { get; }
		public Watt MaxChargingPower { get; }

		public GearshiftPosition PTO_DriveGear => Body["GearDuringPTODrive"] != null ? new GearshiftPosition(Body["GearDuringPTODrive"].Value<uint>()) : null;

		public PerSecond PTO_DriveEngineSpeed => Body["EngineSpeedDuringPTODrive"] != null ? Body.GetEx<double>("EngineSpeedDuringPTODrive").RPMtoRad() : null;

		IAdvancedDriverAssistantSystemsEngineering IVehicleEngineeringInputData.ADAS => GetADS();

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADAS => GetADS();

		protected virtual IAdvancedDriverAssistantSystemsEngineering GetADS()
		{
			return _adasInputData ?? (_adasInputData = new JSONADASInputDataV7(this));
		}

		public virtual double InitialSOC => double.NaN;

		public virtual VectoSimulationJobType VehicleType => VectoSimulationJobType.ConventionalVehicle;

		public virtual AxleConfiguration AxleConfiguration =>
			AxleConfigurationHelper.Parse(
				Body.GetEx(JsonKeys.Vehicle_AxleConfiguration).GetEx<string>(JsonKeys.Vehicle_AxleConfiguration_Type));

		public virtual IList<IAxleEngineeringInputData> AxlesEngineering => AxleWheels().Cast<IAxleEngineeringInputData>().ToList();

		public virtual string ManufacturerAddress => Constants.NOT_AVAILABLE;

		public virtual PerSecond EngineIdleSpeed => Body["IdlingSpeed"] != null ? Body.GetEx<double>("IdlingSpeed").RPMtoRad() : null;

		IList<IAxleDeclarationInputData> IAxlesDeclarationInputData.AxlesDeclaration => AxleWheels().Cast<IAxleDeclarationInputData>().ToList();

		private IEnumerable<AxleInputData> AxleWheels()
		{
			return
				Body.GetEx(JsonKeys.Vehicle_AxleConfiguration).GetEx(JsonKeys.Vehicle_AxleConfiguration_Axles).Select(
					(axle, idx) => new AxleInputData {
						TwinTyres = axle.GetEx<bool>(JsonKeys.Vehicle_Axles_TwinTyres),
						AxleWeightShare = axle.GetEx<double>("AxleWeightShare"),
						AxleType = axle["Type"] != null
							? axle.GetEx<string>("Type").ParseEnum<AxleType>()
							: (idx == 1 ? AxleType.VehicleDriven : AxleType.VehicleNonDriven),
						Tyre = new TyreInputData {
							DataSource = new DataSource {
								SourceType = DataSourceType.JSONFile,
								SourceFile = Source,
								SourceVersion = Version,
							},
							Source = Source,
							AppVersion = AppVersion,
							Inertia = axle.GetEx<double>(JsonKeys.Vehicle_Axles_Inertia).SI<KilogramSquareMeter>(),
							Dimension = axle.GetEx<string>(JsonKeys.Vehicle_Axles_Wheels),
							RollResistanceCoefficient = axle.GetEx<double>(JsonKeys.Vehicle_Axles_RollResistanceCoefficient),
							TyreTestLoad = axle.GetEx<double>(JsonKeys.Vehicle_Axles_TyreTestLoad).SI<Newton>(),
						}
					});
		}

		#endregion

		#region "VehicleComponents"

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData => _airdragInputData ?? (_airdragInputData = new JSONAirdragInputData(this));

		IAirdragEngineeringInputData IVehicleComponentsEngineering.AirdragInputData => _airdragInputData ?? (_airdragInputData = new JSONAirdragInputData(this));

		IGearboxDeclarationInputData IVehicleComponentsDeclaration.GearboxInputData => Job.Gearbox;

		public virtual ITorqueConverterDeclarationInputData TorqueConverter => Job.TorqueConverter;

		IGearboxEngineeringInputData IVehicleComponentsEngineering.GearboxInputData => Job.Gearbox;

		ITorqueConverterDeclarationInputData IVehicleComponentsDeclaration.TorqueConverterInputData => Job.TorqueConverter;

		ITorqueConverterEngineeringInputData IVehicleComponentsEngineering.TorqueConverterInputData => Job.TorqueConverter;

		IAxleGearInputData IVehicleComponentsEngineering.AxleGearInputData => Job.AxleGear;

		IAngledriveInputData IVehicleComponentsEngineering.AngledriveInputData => _angledriveData ?? (_angledriveData = new JSONAngledriveInputData(this));

		public virtual IEngineEngineeringInputData EngineInputData => Job.Engine;

		IAxleGearInputData IVehicleComponentsDeclaration.AxleGearInputData => Job.AxleGear;

		IAngledriveInputData IVehicleComponentsDeclaration.AngledriveInputData => _angledriveData ?? (_angledriveData = new JSONAngledriveInputData(this));

		IEngineDeclarationInputData IVehicleComponentsDeclaration.EngineInputData => Job.Engine;

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => Job.DeclarationAuxiliaries;

		IRetarderInputData IVehicleComponentsEngineering.RetarderInputData => GetRetarder;

		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData => GetRetarder;

		protected virtual IRetarderInputData GetRetarder => _retarderInputData ?? (_retarderInputData = new JSONRetarderInputData(this));

		IPTOTransmissionInputData IVehicleComponentsEngineering.PTOTransmissionInputData => _ptoInputData ?? (_ptoInputData = new JSONPTOTransmissioninputData(this));

		IPTOTransmissionInputData IVehicleComponentsDeclaration.PTOTransmissionInputData => _ptoInputData ?? (_ptoInputData = new JSONPTOTransmissioninputData(this));


		IAxlesEngineeringInputData IVehicleComponentsEngineering.AxleWheels => this;

		IElectricStorageSystemEngineeringInputData IVehicleComponentsEngineering.ElectricStorage => GetElectricStorage();

		protected virtual IElectricStorageSystemEngineeringInputData GetElectricStorage()
		{
			return null;
		}

		IElectricMachinesEngineeringInputData IVehicleComponentsEngineering.ElectricMachines => GetElectricMachines();
		public virtual IIEPCEngineeringInputData IEPCEngineeringInputData => null;
		public virtual IIEPCDeclarationInputData IEPC => null;

		protected virtual IElectricMachinesEngineeringInputData GetElectricMachines()
		{
			return null;
		}

		public virtual IBusAuxiliariesDeclarationData BusAuxiliaries => null;

		IElectricStorageSystemDeclarationInputData IVehicleComponentsDeclaration.ElectricStorage => GetElectricStorage();

		IElectricMachinesDeclarationInputData IVehicleComponentsDeclaration.ElectricMachines => GetElectricMachines();

		IAxlesDeclarationInputData IVehicleComponentsDeclaration.AxleWheels => this;

		public virtual bool VocationalVehicle => DeclarationData.Vehicle.VocationalVehicleDefault;

		public virtual bool? SleeperCab => DeclarationData.Vehicle.SleeperCabDefault;

		public virtual bool? AirdragModifiedMultistep { get; }

		public virtual TankSystem? TankSystem => DeclarationData.Vehicle.TankSystemDefault;


		public virtual bool ZeroEmissionVehicle => DeclarationData.Vehicle.ZeroEmissionVehicleDefault;

		public virtual bool HybridElectricHDV => DeclarationData.Vehicle.HybridElectricHDVDefault;

		public virtual bool DualFuelVehicle => DeclarationData.Vehicle.DualFuelVehicleDefault;

		public virtual Watt MaxNetPower1 => null;

		public virtual Watt MaxNetPower2 => null;

		public virtual string ExemptedTechnology => null;

		public virtual RegistrationClass? RegisteredClass => RegistrationClass.unknown;

		public virtual int? NumberPassengerSeatsUpperDeck => 0;

		public virtual int? NumberPassengerSeatsLowerDeck => 0;

		public int? NumberPassengersStandingLowerDeck => 0;

		public int? NumberPassengersStandingUpperDeck => 0;

		public virtual CubicMeter CargoVolume => 0.SI<CubicMeter>();

		public virtual TableData PTOCycleDuringStop
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_Cycle] == null) {
					return null;
				}
				var cycle = pto[JsonKeys.Vehicle_PTO_Cycle];
				if (string.IsNullOrWhiteSpace(cycle.Value<string>())) {
					return null;
				}
				try {
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_Cycle), "PTO Cycle Standstill");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}
					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}

		public virtual TableData PTOCycleWhileDriving
		{
			get {
				var pto = Body[JsonKeys.Vehicle_PTO];
				if (pto == null || pto[JsonKeys.Vehicle_PTO_CycleDriving] == null) {
					return null;
				}
				var cycle = pto[JsonKeys.Vehicle_PTO_CycleDriving];
				if (string.IsNullOrWhiteSpace(cycle.Value<string>())) {
					return null;
				}
				try {
					return ReadTableData(Body.GetEx(JsonKeys.Vehicle_PTO).GetEx<string>(JsonKeys.Vehicle_PTO_CycleDriving), "PTO Cycle Driving");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}
					return new TableData(Path.Combine(BasePath, cycle.Value<string>()) + MissingFileSuffix, DataSourceType.Missing);
				}
			}
		}
		public virtual VehicleCode? VehicleCode => VectoCommon.Models.VehicleCode.NOT_APPLICABLE;

		public virtual bool? LowEntry => false;

		IVehicleComponentsDeclaration IVehicleDeclarationInputData.Components => this;

		IAuxiliariesEngineeringInputData IVehicleComponentsEngineering.AuxiliaryInputData => Job.EngineeringAuxiliaries;

		#endregion


		public virtual string Manufacturer => Constants.NOT_AVAILABLE;

		public virtual string Model => Constants.NOT_AVAILABLE;

		public virtual DateTime Date => DateTime.MinValue;

		public CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public virtual string CertificationNumber => Constants.NOT_AVAILABLE;

		public virtual DigestData DigestValue => null;

		public virtual XmlNode XMLSource => null;
	}



}
