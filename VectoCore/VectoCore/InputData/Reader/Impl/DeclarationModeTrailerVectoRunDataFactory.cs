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
using System.Net;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Bson;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using IXMLGearData = TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces.IXMLGearData;
using Wheels = TUGraz.VectoCore.Models.Declaration.Wheels;


namespace TUGraz.VectoCore.InputData.Reader.Impl
{

	public class DeclarationModeTrailerVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected static readonly object CyclesCacheLock = new object();
		protected static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache = new Dictionary<MissionType, DrivingCycleData>();
		protected readonly IDeclarationTrailerInputDataProvider TrailerInputDataProvider;
		protected readonly IDeclarationInputDataProvider InputDataProvider;
		protected IDeclarationReport Report; 
		protected IDeclarationDataAdapter DataAdapter => _da;
		protected DeclarationTrailerDataAdapter TrailerDataAdapter = new DeclarationTrailerDataAdapter();

		private readonly DeclarationDataAdapterCompletedBusGeneric _da = new DeclarationDataAdapterCompletedBusGeneric();

		private Segment _segment;
		private DriverData _driverdata;
		private AirdragData _airdragData;
		private AxleGearData _axlegearData;
		private AngledriveData _angledriveData;
		private GearboxData _gearboxData;
		private RetarderData _retarderData;
		private PTOData _ptoTransmissionData;
		private PTOData _municipalPtoTransmissionData;
		private ShiftStrategyParameters _gearshiftData;

		private IXMLDeclarationVehicleData _genericVehicle;
		private IXMLEngineDeclarationInputData _genericEngineDataProvider;
		private IXMLAirdragDeclarationInputData _genericAirdragData;
		private IXMLGearboxDeclarationInputData _genericGearboxInputData;
		private XMLDeclarationInputDataProviderV10 _genericInputDataProvider;
		private XMLDeclarationJobInputDataProviderV10 _genericJobInputData;
		private XMLDeclarationComponentsDataProviderV10 _genericVehicleComponents;
		private XMLDeclarationAxlegearDataProviderV10 _genericAxleGearInputData;
		private XMLDeclarationAngledriveDataProviderV10 _genericAngleInputData;
		private XMLDeclarationRetarderDataProviderV10 _genericRetarderInputData;
		private XMLDeclarationPTODataProviderV10 _genericPTOInputData;

		internal DeclarationModeTrailerVectoRunDataFactory(
			IDeclarationTrailerInputDataProvider dataProvider, IDeclarationReport report)
		{
			TrailerInputDataProvider = dataProvider;
			Report = report;
		}

		protected Segment GetSegment(IVehicleDeclarationInputData vehicle)
		{
			var segment = DeclarationData.TruckSegments.Lookup(
				vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis,
				vehicle.VocationalVehicle);

			return segment;
		}

		protected IEnumerable<VectoRunData> GetNextRun()
		{
			return VectoRunDataNonExempted();
		}

		private IEnumerable<VectoRunData> VectoRunDataNonExempted()
		{
			var vehicle = _genericVehicle;
			var engine = _genericEngineDataProvider;
			var engineModes = engine.EngineModes;
			var trailer = TrailerInputDataProvider.JobInputData.Trailer;

			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				
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
			return new PTOData {
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

			if (_genericVehicle.ExemptedVehicle) {
				powertrainConfig = CreateVectoRunData(_genericVehicle, trailer, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
				fuels = new List<List<FuelData.Entry>>();
			} else {
				powertrainConfig = _segment.Missions.Select(
						mission => CreateVectoRunData(
							_genericVehicle, trailer, 0, mission, mission.Loadings.First()))
					.FirstOrDefault(x => x != null);
				fuels = _genericEngineDataProvider.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, null)).ToList())
					.ToList();
			}
			Report.InitializeReport(powertrainConfig, fuels);
		}

		[TestMethod]
		private void DoTests()
		{
			var trailer = TrailerInputDataProvider.JobInputData.Trailer;
			Assert.IsNotNull(trailer);

			var trailerSegment = DeclarationData.TrailerSegments.Lookup(trailer.NumberOfAxles, trailer.TrailerType, trailer.BodyworkCode, false);
			Assert.AreEqual(211, trailerSegment.VehicleGroup);

			var aeroFeatures = new List<AeroFeatureTechnology> { AeroFeatureTechnology.SideSkirtsShort };

			var combination = DeclarationData.AeroFeatures.Lookup(aeroFeatures);
			Assert.AreEqual(1, combination);
		}

		private void ValidateTrailerInput()
		{
			#region Aero features combination validation

			var aeroFeatures = new List<AeroFeatureTechnology>();
			if (TrailerInputDataProvider.JobInputData.Trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.SideSkirtsShort)) aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsShort);
			if (TrailerInputDataProvider.JobInputData.Trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.SideSkirtsLong)) aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsLong);
			if (TrailerInputDataProvider.JobInputData.Trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.BoatTailShort)) aeroFeatures.Add(AeroFeatureTechnology.BoatTailShort);
			if (TrailerInputDataProvider.JobInputData.Trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.BoatTailLong)) aeroFeatures.Add(AeroFeatureTechnology.BoatTailLong);

			var combination = DeclarationData.AeroFeatures.Lookup(aeroFeatures);
			if (combination < 0)
				throw new Exception("invalid aero feature combination");

			#endregion

			#region Tyre dimension validation

			if (TrailerInputDataProvider.JobInputData.Trailer.Axles != null) return; //TODO fix issue with Ninject first

			var trailerAxles = TrailerInputDataProvider.JobInputData.Trailer.Axles;
			if (trailerAxles.Any(x => x.Tyre.Dimension != trailerAxles.FirstOrDefault()?.Tyre.Dimension))
				throw new Exception("invalid trailer configurations - different tyre dimensions specified");

			#endregion
		}

		private void GetSpecificTrailerData()
		{
			var trailer = TrailerInputDataProvider.JobInputData.Trailer;

			var volumeOrientation = (trailer.InternalBodyHeight >= 2.9 && trailer.TotalTrailerHeight <= 4);
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(trailer.NumberOfAxles, trailer.TrailerType, trailer.BodyworkCode, false);
			var vehicleGroup = trailerSegment.VehicleGroup;
			var vehicleGroupActive = trailerSegment.VehicleGroupActive;
			var genericVehicleCode = trailerSegment.GenericCode;
			var crossSectionAreaCorrection = trailerSegment.CrossSectionAreaCorrection;
			
			double CdxA0 = 0;

			if (crossSectionAreaCorrection) {
				var acrMin = trailerSegment.AcrMin;
				var acrBase = trailerSegment.AcrBase;
				var acr = trailer.TotalTrailerHeight.ToBasicUnits().Value() * trailer.ExternalBodyWidth.ToBasicUnits().Value() > acrMin ? 
					trailer.TotalTrailerHeight.ToBasicUnits().Value() * trailer.ExternalBodyWidth.ToBasicUnits().Value() : acrMin;
				CdxA0 = acrBase * acr / acrBase;
			}

			var lengthcorrection = trailerSegment.LengthCorrection;

			GetGetGenericVehicleData(genericVehicleCode);

			if (lengthcorrection)
			{
				var wheels = new Wheels();
				var axleWheelDimension = _genericVehicleComponents.AxleWheels.XMLSource.ChildNodes[0].ChildNodes[0].FirstChild.ChildNodes[3].FirstChild.ChildNodes[5].InnerText;
				var wheelRadius = wheels.Lookup(axleWheelDimension).WheelsDiameter / 2;
			}

			var aeroFeatures = new List<AeroFeatureTechnology>();
			if (TrailerInputDataProvider.JobInputData.Trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.SideSkirtsShort)) aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsShort);
			if (TrailerInputDataProvider.JobInputData.Trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.SideSkirtsLong)) aeroFeatures.Add(AeroFeatureTechnology.SideSkirtsLong);
			if (TrailerInputDataProvider.JobInputData.Trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.BoatTailShort)) aeroFeatures.Add(AeroFeatureTechnology.BoatTailShort);
			if (TrailerInputDataProvider.JobInputData.Trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.BoatTailLong)) aeroFeatures.Add(AeroFeatureTechnology.BoatTailLong);

			var combination = DeclarationData.AeroFeatures.Lookup(aeroFeatures);

			var aeroFeatureCorrection = 0.0;

			switch (combination) {
				case 0:
					aeroFeatureCorrection = trailerSegment.Aero0;
					break;
				case 1:
					aeroFeatureCorrection = trailerSegment.Aero1;
					break;
				case 2:
					aeroFeatureCorrection = trailerSegment.Aero2;
					break;
				case 3:
					aeroFeatureCorrection = trailerSegment.Aero3;
					break;
				case 4:
					aeroFeatureCorrection = trailerSegment.Aero4;
					break;
				case 5:
					aeroFeatureCorrection = trailerSegment.Aero5;
					break;
				case 6:
					aeroFeatureCorrection = trailerSegment.Aero6;
					break;
				case 7:
					aeroFeatureCorrection = trailerSegment.Aero7;
					break;
				case 8:
					aeroFeatureCorrection = trailerSegment.Aero8;
					break;
			}
				
		}

		private void GetGetGenericVehicleData(string vehicleCode)
		{
			//TODO Get generic vehicle according to vehicleCode
			var filePath = @"F:\Vecto\vecto-dev_bt\VectoCore\VectoCore\Resources\Declaration\GenericVehicles\" + vehicleCode + ".xml";
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(filePath);

			_genericInputDataProvider = new XMLDeclarationInputDataProviderV10(xmlDoc, filePath);
			_genericInputDataProvider.Reader = new XMLDeclarationInputReaderV10(_genericInputDataProvider, xmlDoc.ChildNodes[1]);

			_genericJobInputData = new XMLDeclarationJobInputDataProviderV10(xmlDoc.ChildNodes[1], _genericInputDataProvider, filePath);
			_genericJobInputData.Reader = new XMLJobDataReaderV10(_genericJobInputData, xmlDoc.ChildNodes[1]);

			_genericVehicle = new XMLDeclarationVehicleDataProviderV10(_genericJobInputData, xmlDoc.ChildNodes[1], filePath);
			_genericVehicleComponents = new XMLDeclarationComponentsDataProviderV10(_genericVehicle,
				xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath) {
				ComponentReader = new XMLComponentReaderV10(_genericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19])
			};

			_genericEngineDataProvider = new XMLDeclarationEngineDataProviderV10(_genericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			_genericAxleGearInputData = new XMLDeclarationAxlegearDataProviderV10(_genericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			_genericGearboxInputData = new XMLDeclarationGearboxDataProviderV10(_genericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			_genericAngleInputData = new XMLDeclarationAngledriveDataProviderV10(_genericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			_genericRetarderInputData = new XMLDeclarationRetarderDataProviderV10(_genericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			_genericPTOInputData = new XMLDeclarationPTODataProviderV10(_genericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
			_genericAirdragData = new XMLDeclarationAirdragDataProviderV10(_genericVehicle, xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[19], filePath);
		}

		protected virtual void Initialize()
		{
			DoTests();
			ValidateTrailerInput();
			GetSpecificTrailerData();

			_segment = GetSegment(_genericVehicle);
			_driverdata = DataAdapter.CreateDriverData();
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);

			var tempVehicle = DataAdapter.CreateVehicleData(_genericVehicle, _segment, _segment.Missions.First(), _segment.Missions.First().Loadings.First());

			_airdragData = DataAdapter.CreateAirdragData(_genericAirdragData,_segment.Missions.First(), _segment);

			_axlegearData = _genericVehicle.AxleConfiguration.AxlegearIncludedInGearbox() ? 
				DataAdapter.CreateDummyAxleGearData(_genericVehicleComponents.GearboxInputData) : DataAdapter.CreateAxleGearData(_genericAxleGearInputData);
			
			_angledriveData = DataAdapter.CreateAngledriveData(_genericAngleInputData);

			var tmpRunData = new VectoRunData {
				ShiftStrategy = _genericJobInputData.ShiftStrategy,
				GearboxData =  new GearboxData {
					Type = _genericGearboxInputData.Type,
				}
			};
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(tmpRunData, new SimplePowertrainContainer(tmpRunData));
			var tmpEngine = TrailerDataAdapter.CreateEngineData(_genericEngineDataProvider, _genericGearboxInputData, _genericEngineDataProvider.EngineModes[0], _segment.Missions.First());

			_gearboxData = TrailerDataAdapter.CreateGearboxData(
				_genericGearboxInputData, _genericVehicleComponents, new VectoRunData() { EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = tempVehicle },
				tmpStrategy);
				
			_retarderData = DataAdapter.CreateRetarderData(_genericRetarderInputData);

			_ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(_genericPTOInputData);

			_municipalPtoTransmissionData = CreateDefaultPTOData();

			_gearshiftData = DataAdapter.CreateGearshiftData(_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), tmpEngine.IdleSpeed);
		}

		protected VectoRunData CreateVectoRunData(
			IVehicleDeclarationInputData vehicle, ITrailerDeclarationInputData trailer, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var engine = _genericEngineDataProvider;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];
			DrivingCycleData cycle;
			lock (CyclesCacheLock)
			{
				if (CyclesCache.ContainsKey(mission.MissionType))
					cycle = CyclesCache[mission.MissionType];
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
				AirdragData = DataAdapter.CreateAirdragData(_genericAirdragData, mission, _segment),
				EngineData = TrailerDataAdapter.CreateEngineData(
					_genericEngineDataProvider, _genericGearboxInputData, _genericEngineDataProvider.EngineModes[0], _segment.Missions.First()), 
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
				ModFileSuffix = (engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key.ToString(),
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
			if (Report != null) 
				InitializeReport();

			return GetNextRun();
		}
	}
}
