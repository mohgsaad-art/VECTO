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
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONComponentInputData : IEngineeringInputDataProvider, IDeclarationInputDataProvider,
		IEngineeringJobInputData, IVehicleEngineeringInputData, IAdvancedDriverAssistantSystemDeclarationInputData,
		IAdvancedDriverAssistantSystemsEngineering, IVehicleComponentsDeclaration, IVehicleComponentsEngineering,
		IDriverEngineeringInputData, IAuxiliariesEngineeringInputData
	{
		protected IGearboxEngineeringInputData Gearbox;
		protected IAxleGearInputData AxleGear;
		protected ITorqueConverterEngineeringInputData TorqueConverterData;
		protected IAngledriveInputData Angledrive;
		protected IEngineEngineeringInputData Engine;
		protected IVehicleEngineeringInputData VehicleData;
		protected IRetarderInputData Retarder;
		protected IPTOTransmissionInputData PTOTransmission;
		private IAirdragEngineeringInputData AirdragData;
		protected IGearshiftEngineeringInputData GearshiftData;
		private string _filename;
		private IAxlesDeclarationInputData _axleWheelsDecl;
		private IAxlesEngineeringInputData _axleWheelsEng;
		private IBatteryPackEngineeringInputData Battery;
		private IElectricMotorEngineeringInputData ElectricMotor;
		private IBusAuxiliariesEngineeringData BusAux;
		private IIEPCEngineeringInputData IEPCData;

		public JSONComponentInputData(string filename, IJSONVehicleComponents job, bool tolerateMissing = false)
		{
			var extension = Path.GetExtension(filename);
			object tmp = null;
			switch (extension) {
				case Constants.FileExtensions.VehicleDataFile:
					tmp = JSONInputDataFactory.ReadJsonVehicle(filename, job, tolerateMissing);
					break;
				case Constants.FileExtensions.EngineDataFile:
					tmp = JSONInputDataFactory.ReadEngine(filename, tolerateMissing);
					break;
				case Constants.FileExtensions.GearboxDataFile:
					tmp = JSONInputDataFactory.ReadGearbox(filename, tolerateMissing);
					break;
				case Constants.FileExtensions.GearshiftDataFile:
					tmp = JSONInputDataFactory.ReadShiftParameters(filename, tolerateMissing);
					break;
				case Constants.FileExtensions.BatteryFile:
				case ".vbat":
					tmp = JSONInputDataFactory.ReadREESSData(filename, tolerateMissing);
					break;
				case Constants.FileExtensions.ElectricMotorFile:
					tmp = JSONInputDataFactory.ReadElectricMotorData(filename, tolerateMissing);
					break;
				case Constants.FileExtensions.HybridStrategyParameters:
					tmp = JSONInputDataFactory.ReadHybridStrategyParameters(filename, tolerateMissing);
					break;
				case ".vaux":
					tmp = JSONInputDataFactory.ReadEngineeringBusAuxiliaries(filename, tolerateMissing);
					break;
				case Constants.FileExtensions.IEPCDataFile:
					tmp = JSONInputDataFactory.ReadIEPCEngineeringInputData(filename, tolerateMissing);
					break;
			}

			if(tmp is IVehicleEngineeringInputData x1) VehicleData = x1;
			if(tmp is IAirdragEngineeringInputData x2) AirdragData = x2;
			if(tmp is IEngineEngineeringInputData x3) Engine = x3;
			if(tmp is IGearboxEngineeringInputData x4) Gearbox = x4;
			if(tmp is IAxleGearInputData x5) AxleGear = x5;
			if(tmp is IRetarderInputData x6) Retarder = x6;
			if(tmp is ITorqueConverterEngineeringInputData x7) TorqueConverterData = x7;
			if(tmp is IAngledriveInputData x8) Angledrive = x8;
			if(tmp is IPTOTransmissionInputData x9) PTOTransmission = x9;
			if(tmp is IGearshiftEngineeringInputData x10) GearshiftData = x10;
			if(tmp is IAxlesDeclarationInputData x11) _axleWheelsDecl = x11;
			if(tmp is IAxlesEngineeringInputData x12) _axleWheelsEng = x12;
			if(tmp is IBatteryPackEngineeringInputData x13) Battery = x13;
			if(tmp is IElectricMotorEngineeringInputData x14) ElectricMotor = x14;
			if(tmp is IHybridStrategyParameters x15) HybridStrategyParameters = x15;
			if(tmp is IBusAuxiliariesEngineeringData x16) BusAux = x16;
			if (tmp is IIEPCEngineeringInputData x17) IEPCData = x17;

			_filename = filename;
		}


		public IEngineeringJobInputData JobInputData => this;

		public virtual IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData => null;

		public XElement XMLHash => new XElement(XMLNames.DI_Signature);


		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData => this;

		public IDriverEngineeringInputData DriverInputData => this;

		public IOverSpeedEngineeringInputData OverSpeedData { get; }
		public IDriverAccelerationData AccelerationCurve { get; }
		public ILookaheadCoastingInputData Lookahead { get; }

		public IGearshiftEngineeringInputData GearshiftInputData => GearshiftData;

		public IEngineStopStartEngineeringInputData EngineStopStartData { get; }
		public IEcoRollEngineeringInputData EcoRollData { get; }
		public IPCCEngineeringInputData PCCData { get; }

		public DataSource DataSource => new DataSource { SourceType = DataSourceType.JSONFile, SourceFile = _filename };

		public string AppVersion => "VECTO-JSON";

		public string Source => _filename;

		public bool SavedInDeclarationMode { get; private set; }
		public string Manufacturer { get; private set; }
		public string Model { get; private set; }
		public DateTime Date { get; private set; }
		public CertificationMethod CertificationMethod { get; private set; }
		public string CertificationNumber { get; private set; }
		public DigestData DigestValue { get; private set; }

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle => Vehicle;

		public IHybridStrategyParameters HybridStrategyParameters { get; set; }

		public IVehicleEngineeringInputData Vehicle => VehicleData ?? this;

		public IList<ICycleData> Cycles { get; private set; }

		public VectoSimulationJobType JobType { get; private set; }

		public IEngineEngineeringInputData EngineOnly { get; private set; }
		public TableData PTOCycleWhileDrive { get; private set; }


		public string JobName => "";

		public string Identifier => Vehicle.Identifier;

		public bool ExemptedVehicle => false;

		public string VIN => VehicleData.VIN;

		public LegislativeClass? LegislativeClass => VehicleData.LegislativeClass;

		public VehicleCategory VehicleCategory => VehicleData.VehicleCategory;

		public AxleConfiguration AxleConfiguration => VehicleData.AxleConfiguration;

		public Kilogram CurbMassChassis => VehicleData.CurbMassChassis;

		public Kilogram GrossVehicleMassRating => VehicleData.GrossVehicleMassRating;

		public IList<ITorqueLimitInputData> TorqueLimits => VehicleData.TorqueLimits;

		IAxlesDeclarationInputData IVehicleComponentsDeclaration.AxleWheels => _axleWheelsDecl;

		public IBusAuxiliariesDeclarationData BusAuxiliaries => null;

		public IElectricStorageSystemEngineeringInputData ElectricStorage =>
			new JSONElectricStorageSystemEngineeringInputData(new List<IElectricStorageEngineeringInputData>() {
				new JSONElectricStorageEngineeringInputData {
					REESSPack = Battery,
					Count = 1,
					StringId = 0,
				}
			});

		public IElectricMachinesEngineeringInputData ElectricMachines { get
		{
			return new JSONElectricMotors(new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
				new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
					ElectricMachine = ElectricMotor, Count = 1, RatioADC = 1, RatioPerGear = new double[] { },
					MechanicalTransmissionEfficiency = 1,
					Position = PowertrainPosition.HybridPositionNotSet
				}
			});
		} }

		public IIEPCEngineeringInputData IEPCEngineeringInputData => IEPCData;

		public IIEPCDeclarationInputData IEPC => IEPCData;

		IElectricStorageSystemDeclarationInputData IVehicleComponentsDeclaration.ElectricStorage => 
			new JSONElectricStorageSystemEngineeringInputData(new List<IElectricStorageEngineeringInputData>() {
				new JSONElectricStorageEngineeringInputData {
					REESSPack = Battery,
					Count = 1,
					StringId = 0,
				}
			});

		IElectricMachinesDeclarationInputData IVehicleComponentsDeclaration.ElectricMachines => ElectricMachines;

		public Meter DynamicTyreRadius => VehicleData.DynamicTyreRadius;


		public bool Articulated => VehicleData.Articulated;

		public Meter Height => VehicleData.Height;

		public bool? ATEcoRollReleaseLockupClutch => VehicleData.ADAS.ATEcoRollReleaseLockupClutch;

		public XmlNode XMLSource => null;
		public string VehicleTypeApprovalNumber { get; }
		public ArchitectureID ArchitectureID { get; }
		public bool OvcHev { get; }
		public Watt MaxChargingPower { get; }

		public Meter Length => VehicleData.Length;

		public Meter Width => VehicleData.Width;

		public Meter EntranceHeight => null;

		public ConsumerTechnology? DoorDriveTechnology => VehicleData.DoorDriveTechnology;

		public VehicleDeclarationType VehicleDeclarationType { get; }

		public Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits => Vehicle.ElectricMotorTorqueLimits;
		public TableData BoostingLimitations => Vehicle.BoostingLimitations;
		

		IVehicleComponentsEngineering IVehicleEngineeringInputData.Components => this;

		XmlNode IVehicleDeclarationInputData.XMLSource => null;

		IAdvancedDriverAssistantSystemsEngineering IVehicleEngineeringInputData.ADAS => this;

		public GearshiftPosition PTO_DriveGear => VehicleData.PTO_DriveGear;

		public PerSecond PTO_DriveEngineSpeed => VehicleData.PTO_DriveEngineSpeed;

		public double InitialSOC => VehicleData.InitialSOC;

		public VectoSimulationJobType VehicleType => VehicleData.VehicleType;

		public IAirdragEngineeringInputData AirdragInputData => AirdragData;

		public IGearboxEngineeringInputData GearboxInputData => Gearbox;

		public ITorqueConverterDeclarationInputData TorqueConverter => TorqueConverterData;

		public ITorqueConverterEngineeringInputData TorqueConverterInputData => TorqueConverterData;

		IAxleGearInputData IVehicleComponentsDeclaration.AxleGearInputData => AxleGear;

		IAngledriveInputData IVehicleComponentsDeclaration.AngledriveInputData => Angledrive;

		public Kilogram CurbMassExtra => Vehicle.CurbMassExtra;

		public Kilogram Loading => Vehicle.Loading;

		IAxlesEngineeringInputData IVehicleComponentsEngineering.AxleWheels => _axleWheelsEng;

		public string ManufacturerAddress => VehicleData.ManufacturerAddress;

		public PerSecond EngineIdleSpeed => VehicleData.EngineIdleSpeed;

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData => AirdragInputData;

		IGearboxDeclarationInputData IVehicleComponentsDeclaration.GearboxInputData => GearboxInputData;

		ITorqueConverterDeclarationInputData IVehicleComponentsDeclaration.TorqueConverterInputData => TorqueConverterInputData;

		IAxleGearInputData IVehicleComponentsEngineering.AxleGearInputData => AxleGear;

		IAngledriveInputData IVehicleComponentsEngineering.AngledriveInputData => Angledrive;

		public IEngineEngineeringInputData EngineInputData => Engine;


		IEngineDeclarationInputData IVehicleComponentsDeclaration.EngineInputData => Engine;

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData => throw new NotImplementedException();

		IRetarderInputData IVehicleComponentsEngineering.RetarderInputData => Retarder;

		IPTOTransmissionInputData IVehicleComponentsEngineering.PTOTransmissionInputData => PTOTransmission;

		public bool VocationalVehicle => DeclarationData.Vehicle.VocationalVehicleDefault;

		public bool? SleeperCab => DeclarationData.Vehicle.SleeperCabDefault;

		public bool? AirdragModifiedMultistep { get; }

		public TankSystem? TankSystem => DeclarationData.Vehicle.TankSystemDefault;

		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS => this;

		public bool ZeroEmissionVehicle => DeclarationData.Vehicle.ZeroEmissionVehicleDefault;

		public bool HybridElectricHDV => DeclarationData.Vehicle.HybridElectricHDVDefault;

		public bool DualFuelVehicle => DeclarationData.Vehicle.DualFuelVehicleDefault;

		public Watt MaxNetPower1 => null;

		public Watt MaxNetPower2 => null;

		public string ExemptedTechnology => null;

		public RegistrationClass? RegisteredClass => RegistrationClass.unknown;

		public int? NumberPassengerSeatsUpperDeck => 0;

		public int? NumberPassengerSeatsLowerDeck => 0;

		public int? NumberPassengersStandingLowerDeck => 0;

		public int? NumberPassengersStandingUpperDeck => 0;

		public CubicMeter CargoVolume => VehicleData.CargoVolume;

		public VehicleCode? VehicleCode => VectoCommon.Models.VehicleCode.NOT_APPLICABLE;

		public bool? LowEntry => VehicleData.LowEntry;

		IVehicleComponentsDeclaration IVehicleDeclarationInputData.Components => this;

		IAuxiliariesEngineeringInputData IVehicleComponentsEngineering.AuxiliaryInputData => this;

		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData => Retarder;

		IPTOTransmissionInputData IVehicleComponentsDeclaration.PTOTransmissionInputData => PTOTransmission;

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public bool EngineStopStart => DeclarationData.Vehicle.ADAS.EngineStopStartDefault;

		public EcoRollType EcoRoll => DeclarationData.Vehicle.ADAS.EcoRoll;

		public PredictiveCruiseControlType PredictiveCruiseControl => DeclarationData.Vehicle.ADAS.PredictiveCruiseControlDefault;

		#endregion

		#region Implementation of IAuxiliariesEngineeringInputData

		public IAuxiliaryEngineeringInputData Auxiliaries => new EngineeringAuxiliaryDataInputData();

		public IBusAuxiliariesEngineeringData BusAuxiliariesData => BusAux;

		public Watt ElectricAuxPower => 0.SI<Watt>();

		#endregion
	}
}