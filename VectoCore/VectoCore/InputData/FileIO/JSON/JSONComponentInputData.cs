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
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONComponentInputData : IEngineeringInputDataProvider, IDeclarationInputDataProvider,
		IEngineeringJobInputData, IVehicleEngineeringInputData, IAdvancedDriverAssistantSystemDeclarationInputData,
		IAdvancedDriverAssistantSystemsEngineering, IVehicleComponentsDeclaration, IVehicleComponentsEngineering
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
		private string _filename;
		private IAxlesDeclarationInputData _axleWheelsDecl;
		private IAxlesEngineeringInputData _axleWheelsEng;


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
			}

			tmp.Switch()
				.If<IVehicleEngineeringInputData>(c => VehicleData = c)
				.If<IAirdragEngineeringInputData>(c => AirdragData = c)
				.If<IEngineEngineeringInputData>(c => Engine = c)
				.If<IGearboxEngineeringInputData>(c => Gearbox = c)
				.If<IAxleGearInputData>(c => AxleGear = c)
				.If<IRetarderInputData>(c => Retarder = c)
				.If<ITorqueConverterEngineeringInputData>(c => TorqueConverterData = c)
				.If<IAngledriveInputData>(c => Angledrive = c)
				.If<IPTOTransmissionInputData>(c => PTOTransmission = c)
				.If<IAxlesDeclarationInputData>(c => _axleWheelsDecl = c)
				.If<IAxlesEngineeringInputData>(c => _axleWheelsEng = c);
			;
			_filename = filename;
		}


		public IEngineeringJobInputData JobInputData
		{
			get { return this; }
		}

		public void ValidateComponentHashes()
		{
			throw new NotImplementedException();
		}

		public XElement XMLHash
		{
			get { return new XElement(XMLNames.DI_Signature); }
		}


		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData
		{
			get { return this; }
		}

		public IDriverEngineeringInputData DriverInputData
		{
			get { return null; }
		}

		public DataSource DataSource
		{
			get { return new DataSource { SourceType = DataSourceType.JSONFile, SourceFile = _filename }; }
		}

		public string Source
		{
			get { return _filename; }
		}

		public bool SavedInDeclarationMode { get; private set; }
		public string Manufacturer { get; private set; }
		public string Model { get; private set; }
		public string Date { get; private set; }
		public CertificationMethod CertificationMethod { get; private set; }
		public string CertificationNumber { get; private set; }
		public DigestData DigestValue { get; private set; }

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle
		{
			get { return Vehicle; }
		}

		public IVehicleEngineeringInputData Vehicle
		{
			get { return VehicleData ?? this; }
		}

		public IList<ICycleData> Cycles { get; private set; }

		public bool EngineOnlyMode { get; private set; }

		public IEngineEngineeringInputData EngineOnly { get; private set; }
		public TableData PTOCycleWhileDrive { get; private set; }


		public string JobName
		{
			get { return ""; }
		}

		public string ShiftStrategy
		{
			get { return ""; }
		}
		public VectoSimulationJobType JobType { get; private set; }

		public string Identifier
		{
			get { return Vehicle.Identifier; }
		}

		public bool ExemptedVehicle
		{
			get { return false; }
		}

		public string VIN
		{
			get { return VehicleData.VIN; }
		}

		public LegislativeClass LegislativeClass
		{
			get { return VehicleData.LegislativeClass; }
		}

		public VehicleCategory VehicleCategory
		{
			get { return VehicleData.VehicleCategory; }
		}

		public AxleConfiguration AxleConfiguration
		{
			get { return VehicleData.AxleConfiguration; }
		}

		public Kilogram CurbMassChassis
		{
			get { return VehicleData.CurbMassChassis; }
		}

		public Kilogram GrossVehicleMassRating
		{
			get { return VehicleData.GrossVehicleMassRating; }
		}

		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get { return VehicleData.TorqueLimits; }
		}

		IAxlesDeclarationInputData IVehicleComponentsDeclaration.AxleWheels
		{
			get { return _axleWheelsDecl; }
		}

		public bool? ATEcoRollReleaseLockupClutch => VehicleData.ADAS.ATEcoRollReleaseLockupClutch;


		public Meter DynamicTyreRadius
		{
			get { return VehicleData.DynamicTyreRadius; }
		}

		public Meter Height
		{
			get { return VehicleData.Height; }
		}

		IVehicleComponentsEngineering IVehicleEngineeringInputData.Components
		{
			get { return this; }
		}

		IAdvancedDriverAssistantSystemsEngineering IVehicleEngineeringInputData.ADAS
		{
			get { return this; }
		}

		public GearshiftPosition PTO_DriveGear
		{
			get { return VehicleData.PTO_DriveGear; }
		}

		public PerSecond PTO_DriveEngineSpeed
		{
			get { return VehicleData.PTO_DriveEngineSpeed; }
		}

		public IAirdragEngineeringInputData AirdragInputData
		{
			get { return AirdragData; }
		}

		public IGearboxEngineeringInputData GearboxInputData
		{
			get { return Gearbox; }
		}

		public ITorqueConverterDeclarationInputData TorqueConverter
		{
			get { return TorqueConverterData; }
		}

		public ITorqueConverterEngineeringInputData TorqueConverterInputData
		{
			get { return TorqueConverterData; }
		}

		IAxleGearInputData IVehicleComponentsDeclaration.AxleGearInputData
		{
			get { return AxleGear; }
		}

		IAngledriveInputData IVehicleComponentsDeclaration.AngledriveInputData
		{
			get { return Angledrive; }
		}

		public Kilogram CurbMassExtra
		{
			get { return Vehicle.CurbMassExtra; }
		}

		public Kilogram Loading
		{
			get { return Vehicle.Loading; }
		}

		IAxlesEngineeringInputData IVehicleComponentsEngineering.AxleWheels
		{
			get { return _axleWheelsEng; }
		}

		public string ManufacturerAddress
		{
			get { return VehicleData.ManufacturerAddress; }
		}

		public PerSecond EngineIdleSpeed
		{
			get { return VehicleData.EngineIdleSpeed; }
		}

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData
		{
			get { return AirdragInputData; }
		}

		IGearboxDeclarationInputData IVehicleComponentsDeclaration.GearboxInputData
		{
			get { return GearboxInputData; }
		}

		ITorqueConverterDeclarationInputData IVehicleComponentsDeclaration.TorqueConverterInputData
		{
			get { return TorqueConverterInputData; }
		}

		IAxleGearInputData IVehicleComponentsEngineering.AxleGearInputData
		{
			get { return AxleGear; }
		}

		IAngledriveInputData IVehicleComponentsEngineering.AngledriveInputData
		{
			get { return Angledrive; }
		}

		public IEngineEngineeringInputData EngineInputData
		{
			get { return Engine; }
		}


		IEngineDeclarationInputData IVehicleComponentsDeclaration.EngineInputData
		{
			get { return Engine; }
		}

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData
		{
			get { throw new NotImplementedException(); }
		}

		IRetarderInputData IVehicleComponentsEngineering.RetarderInputData
		{
			get { return Retarder; }
		}

		IPTOTransmissionInputData IVehicleComponentsEngineering.PTOTransmissionInputData
		{
			get { return PTOTransmission; }
		}

		public bool VocationalVehicle
		{
			get { return DeclarationData.Vehicle.VocationalVehicleDefault; }
		}

		public bool? SleeperCab
		{
			get { return DeclarationData.Vehicle.SleeperCabDefault; }
		}

		public TankSystem? TankSystem
		{
			get { return DeclarationData.Vehicle.TankSystemDefault; }
		}

		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return this; }
		}

		public bool ZeroEmissionVehicle
		{
			get { return DeclarationData.Vehicle.ZeroEmissionVehicleDefault; }
		}

		public bool HybridElectricHDV
		{
			get { return DeclarationData.Vehicle.HybridElectricHDVDefault; }
		}

		public bool DualFuelVehicle
		{
			get { return DeclarationData.Vehicle.DualFuelVehicleDefault; }
		}

		public Watt MaxNetPower1
		{
			get { return null; }
		}

		public Watt MaxNetPower2
		{
			get { return null; }
		}

		IVehicleComponentsDeclaration IVehicleDeclarationInputData.Components
		{
			get { return this; }
		}

		IAuxiliariesEngineeringInputData IVehicleComponentsEngineering.AuxiliaryInputData
		{
			get { throw new NotImplementedException(); }
		}

		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData
		{
			get { return Retarder; }
		}

		IPTOTransmissionInputData IVehicleComponentsDeclaration.PTOTransmissionInputData
		{
			get { return PTOTransmission; }
		}

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public bool EngineStopStart
		{
			get { return DeclarationData.Vehicle.ADAS.EngineStopStartDefault; }
		}

		public EcoRollType EcoRoll
		{
			get { return DeclarationData.Vehicle.ADAS.EcoRoll; }
		}

		public PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return DeclarationData.Vehicle.ADAS.PredictiveCruiseControlDefault; }
		}

		#endregion
	}
}
