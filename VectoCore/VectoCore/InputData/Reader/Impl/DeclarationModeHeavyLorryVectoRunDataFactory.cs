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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;


namespace TUGraz.VectoCore.InputData.Reader.Impl
{

	public class DeclarationModeTruckVectoRunDataFactory : AbstractDeclarationVectoRunDataFactory
	{
		DeclarationDataAdapterHeavyLorry _dao = new DeclarationDataAdapterHeavyLorry();

		internal DeclarationModeTruckVectoRunDataFactory(
			IDeclarationInputDataProvider dataProvider, IDeclarationReport report) : base(dataProvider, report)
		{ }

		#region Overrides of AbstractDeclarationVectoRunDataFactory

		protected override IDeclarationDataAdapter DataAdapter => _dao;

		#endregion

		protected override Segment GetSegment(IVehicleDeclarationInputData vehicle)
		{
			if (!vehicle.VehicleCategory.IsLorry()) {
				throw new VectoException("Invalid vehicle category for truck factory! {0}", vehicle.VehicleCategory.GetCategoryName());
			}

			Segment segment;
			try {
				segment = DeclarationData.TruckSegments.Lookup(
					vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
					vehicle.CurbMassChassis,
					vehicle.VocationalVehicle);
			} catch (VectoException) {
				_allowVocational = false;
				segment = DeclarationData.TruckSegments.Lookup(
					vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating,
					vehicle.CurbMassChassis,
					false);
			}

			if (!segment.Found) {
				throw new VectoException(
					"no segment found for vehicle configuration: vehicle category: {0}, axle configuration: {1}, GVMR: {2}",
					vehicle.VehicleCategory, vehicle.AxleConfiguration,
					vehicle.GrossVehicleMassRating);
			}

			return segment;
		}


		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			//if (InitException != null) {
			//	throw InitException;
			//}

			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				yield return CreateVectoRunData(InputDataProvider.JobInputData.Vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
			} else {
				foreach (var vectoRunData in VectoRunDataTruckNonExempted()) {
					yield return vectoRunData;
				}
			}
		}

		

		private IEnumerable<VectoRunData> VectoRunDataTruckNonExempted()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;

			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			for(var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				
				foreach (var mission in _segment.Missions) {
					if (mission.MissionType.IsEMS() &&
						engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS)) {
						continue;
					}
					foreach (var loading in mission.Loadings) {
						var simulationRunData = CreateVectoRunData(vehicle, modeIdx, mission, loading);
						yield return simulationRunData;
					}
				}
			}
		}


		protected override VectoRunData CreateVectoRunData(
			IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				return new VectoRunData {
					Exempted = true,
					Report = Report,
					Mission = new Mission() { MissionType = MissionType.ExemptedMission },
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, new Segment(), null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null)), _allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
			}

			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];

			var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));
			
			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational),
				VehicleDesignSpeed = _segment.DesignSpeed,
				AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, mission, _segment),
				EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission), // _engineData.Copy(), // a copy is necessary because every run has a different correction factor!
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, vehicle.Length,
					vehicle.Components.AxleWheels.NumSteeredAxles),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key,
				Report = Report,
				Mission = mission,
				PTO = mission.MissionType == MissionType.MunicipalUtility
					? _municipalPtoTransmissionData
					: _ptoTransmissionData,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			return simulationRunData;
		}

		
	}
}
