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
*   Vasileios Kouliaridis, vasilis.k@emisia.com, EMISIA
*/

using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using System.Linq;
using System.Xml;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using IXMLGearData = TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces.IXMLGearData;


namespace TUGraz.VectoCore.InputData.Reader.Impl
{

	public class DeclarationModeTrailerVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected static readonly object CyclesCacheLock = new object();

		protected static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache =
			new Dictionary<MissionType, DrivingCycleData>();

		protected readonly IDeclarationTrailerInputDataProvider TrailerInputDataProvider;
		protected readonly IDeclarationInputDataProvider InputDataProvider;
		protected IXMLDeclarationVehicleData GenericVehicle;
		protected IXMLEngineDeclarationInputData genericEngineDataProvider;
		protected IXMLAirdragDeclarationInputData genericAirdragData;
		protected IXMLGearboxDeclarationInputData genericGearboxInputData;

		protected IDeclarationReport Report;

		protected Segment _segment;
		protected DriverData _driverdata;
		protected AirdragData _airdragData;
		protected AxleGearData _axlegearData;
		protected AngledriveData _angledriveData;
		protected GearboxData _gearboxData;
		protected RetarderData _retarderData;
		protected PTOData _ptoTransmissionData;
		protected PTOData _municipalPtoTransmissionData;
		//protected Exception InitException;
		protected ShiftStrategyParameters _gearshiftData;

		private DeclarationDataAdapterCompletedBusGeneric _da = new DeclarationDataAdapterCompletedBusGeneric();
		internal DeclarationModeTrailerVectoRunDataFactory(
			IDeclarationTrailerInputDataProvider dataProvider, IDeclarationReport report)
		{
			TrailerInputDataProvider = dataProvider;
			Report = report;
		}

		#region Overrides of AbstractDeclarationVectoRunDataFactory

		protected IDeclarationDataAdapter DataAdapter { get { return _da; } }

		protected DeclarationTrailerDataAdapter TrailerDataAdapter = new DeclarationTrailerDataAdapter();

		#endregion

		protected Segment GetSegment(IVehicleDeclarationInputData vehicle)
		{
			var segment = DeclarationData.TruckSegments.Lookup(
				vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis,
				vehicle.VocationalVehicle);

			return segment;
		}

		protected IEnumerable<VectoRunData> GetNextRun()
		{
			//if (InitException != null) {
			//	throw InitException;
			//}
			
			foreach (var vectoRunData in VectoRunDataNonExempted()) {
				yield return vectoRunData;
			}
			yield return new VectoRunData();
		}

		private IEnumerable<VectoRunData> VectoRunDataNonExempted()
		{
			var vehicle = GenericVehicle;
			var trailer = TrailerInputDataProvider.JobInputData.Trailer;

			var engine = genericEngineDataProvider;
			var engineModes = engine.EngineModes;

			for(var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				
				foreach (var mission in _segment.Missions) {
					if (mission.MissionType.IsEMS() &&
						engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS)) {
						continue;
					}
					foreach (var loading in mission.Loadings) {
						var simulationRunData = CreateVectoRunData(vehicle, trailer, modeIdx, mission, loading);
						yield return simulationRunData;
					}
				}
			}
		}


		protected virtual PTOData CreateDefaultPTOData()
		{
			return new PTOData() {
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
				LossMap = PTOIdleLossMapReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
						CycleType.PTO, "PTO", false)
			};
		}

		protected virtual void InitializeReport()
		{
			VectoRunData powertrainConfig;
			List<List<FuelData.Entry>> fuels;
			var trailer = TrailerInputDataProvider.JobInputData.Trailer;

			if (GenericVehicle.ExemptedVehicle) {
				powertrainConfig = CreateVectoRunData(GenericVehicle, trailer, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
				fuels = new List<List<FuelData.Entry>>();
			} else {
				powertrainConfig = _segment.Missions.Select(
						mission => CreateVectoRunData(
							GenericVehicle, trailer, 0, mission, mission.Loadings.First()))
					.FirstOrDefault(x => x != null);
				fuels = genericEngineDataProvider.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, null)).ToList())
					.ToList();
			}
			Report.InitializeReport(powertrainConfig, fuels);
		}

		protected virtual void Initialize()
		{
			var trailer = TrailerInputDataProvider.JobInputData.Trailer;

			// *************************************** DEBUG START *********************************************
			//var test = InputDataProvider.JobInputData.Trailer.Axles;

			//1. Get trailer specific information from embedded resources using the lookup method
			var vehicleGroup = DeclarationData.TrailerSegments.Lookup(trailer.NumberOfAxles, trailer.TrailerType, trailer.BodyworkCode, false);

			//2. get generic vehicle data from predefined XML sources into a local input data provider
			string filePath = 
				"F:\\Vecto\\vecto-dev_bt\\Documentation\\VehiclesReleaseComparisonDeclarationMode\\Tractor_4x2_vehicle-class-5_EURO6_2018.xml";
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(filePath);

			IXMLDeclarationInputData genericInputDataProvider = new XMLDeclarationInputDataProviderV10(xmlDoc, filePath);
			genericInputDataProvider.Reader = new XMLDeclarationInputReaderV10(genericInputDataProvider, xmlDoc.ChildNodes[1]);

			IXMLDeclarationJobInputData genericJobInputData = new XMLDeclarationJobInputDataProviderV10(xmlDoc.ChildNodes[1], genericInputDataProvider, filePath);
			genericJobInputData.Reader = new XMLJobDataReaderV10(genericJobInputData, xmlDoc.ChildNodes[1]);

			GenericVehicle = new XMLDeclarationVehicleDataProviderV10(genericJobInputData, xmlDoc.ChildNodes[1], filePath);
			IXMLVehicleComponentsDeclaration genericVehicleComponents = new XMLDeclarationComponentsDataProviderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			genericVehicleComponents.ComponentReader = new XMLComponentReaderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19]);

			genericEngineDataProvider = new XMLDeclarationEngineDataProviderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			IXMLAxleGearInputData genericAxleGearInputData = new XMLDeclarationAxlegearDataProviderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			genericGearboxInputData = new XMLDeclarationGearboxDataProviderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			IXMLAngledriveInputData genericAngleInputData = new XMLDeclarationAngledriveDataProviderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			IXMLRetarderInputData genericRetarderInputData = new XMLDeclarationRetarderDataProviderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			IXMLPTOTransmissionInputData genericPTOInputData = new XMLDeclarationPTODataProviderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			genericAirdragData = new XMLDeclarationAirdragDataProviderV10(GenericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);

			//var genericDisplacement = genericEngineDataProvider.Displacement;

			// *************************************** DEBUG END *********************************************

			_segment = GetSegment(GenericVehicle);
			_driverdata = DataAdapter.CreateDriverData();
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
			var tempVehicle = TrailerDataAdapter.CreateVehicleData(trailer, _segment, _segment.Missions.First(),
													_segment.Missions.First().Loadings.First());
			//_airdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData,
			//									_segment.Missions.First(), _segment);
			if (GenericVehicle.AxleConfiguration.AxlegearIncludedInGearbox()) {
				_axlegearData = DataAdapter.CreateDummyAxleGearData(genericVehicleComponents.GearboxInputData);
			} else {
				_axlegearData = DataAdapter.CreateAxleGearData(genericAxleGearInputData);  //CreateAxleGearData(genericVehicleComponents.AxleGearInputData);
			} 

			_angledriveData = DataAdapter.CreateAngledriveData(genericAngleInputData);

			var tmpRunData = new VectoRunData() {
				ShiftStrategy = genericJobInputData.ShiftStrategy,
				GearboxData =  new GearboxData() {
					Type = genericGearboxInputData.Type,
				}
			};
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(tmpRunData, new SimplePowertrainContainer(tmpRunData));
			var tmpEngine = TrailerDataAdapter.CreateEngineData(
				genericEngineDataProvider, genericGearboxInputData, genericEngineDataProvider.EngineModes[0], _segment.Missions.First());

			//_gearboxData = TrailerDataAdapter.CreateGearboxData(
			//	genericGearboxInputData, genericVehicleComponents, new VectoRunData() { EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = tempVehicle },
			//	tmpStrategy);
				
			_retarderData = DataAdapter.CreateRetarderData(genericRetarderInputData);

			_ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(genericPTOInputData);

			_municipalPtoTransmissionData = CreateDefaultPTOData();
			//_gearshiftData = DataAdapter.CreateGearshiftData(
			//	_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), tmpEngine.IdleSpeed);

		}

		protected VectoRunData CreateVectoRunData(
			IVehicleDeclarationInputData vehicle, ITrailerDeclarationInputData trailer, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var engine = genericEngineDataProvider;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];
			DrivingCycleData cycle;
			lock (CyclesCacheLock)
			{
				if (CyclesCache.ContainsKey(mission.MissionType))
				{
					cycle = CyclesCache[mission.MissionType];
				}
				else
				{
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
					CyclesCache.Add(mission.MissionType, cycle);
				}
			}
			var simulationRunData = new VectoRunData
			{
				Loading = loading.Key,
				VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading),
				VehicleDesignSpeed = _segment.DesignSpeed,
				AirdragData = DataAdapter.CreateAirdragData(genericAirdragData, mission, _segment),
				EngineData = TrailerDataAdapter.CreateEngineData(
					genericEngineDataProvider, genericGearboxInputData, genericEngineDataProvider.EngineModes[0], _segment.Missions.First()), //_engineData.Copy(), // a copy is necessary because every run has a different correction factor!
				GearboxData = new GearboxData(),
				AxleGearData = _axlegearData,
				AngledriveData = new AngledriveData(),
				//Aux = new VectoRunData.AuxData[]{},
				//Aux = DataAdapter.CreateAuxiliaryData(
				//	vehicle.Components.AuxiliaryInputData,
				//	vehicle.Components.BusAuxiliaries, mission.MissionType,
				//	_segment.VehicleClass, vehicle.Length),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = TrailerInputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? string.Format("_EngineMode{0}_", modeIdx) : "") + loading.Key.ToString(),
				Report = Report,
				Mission = mission,
				PTO = mission.MissionType == MissionType.MunicipalUtility
					? _municipalPtoTransmissionData
					: _ptoTransmissionData,
				InputDataHash = TrailerInputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				ShiftStrategy = TrailerInputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			return simulationRunData;
		}

		public IEnumerable<VectoRunData> NextRun()
		{
		
			Initialize();
			if (Report != null) {
				InitializeReport();
			}

			return GetNextRun();
		}
	}
}
