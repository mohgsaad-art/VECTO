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
using System.Data.Common;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{
    public class DeclarationDataAdapterHeavyLorry
	{
		public abstract class LorryBase : AbstractSimulationDataAdapter, ILorryDeclarationDataAdapter
		{
            
            private readonly IDriverDataAdapter _driverDataAdapter = new LorryDriverDataAdapter();
            //protected readonly IVehicleDataAdapter _vehicleDataAdapter = new LorryVehicleDataAdapter();
			private readonly IAxleGearDataAdapter _axleGearDataAdapter = new AxleGearDataAdapter();
			private readonly RetarderDataAdapter _retarderDataAdapter = new RetarderDataAdapter();
			private readonly IAirdragDataAdapter _airdragDataAdapter = new AirdragDataAdapter();

			private IAngledriveDataAdapter _angleDriveDataAdapter = new AngledriveDataAdapter();

			protected virtual IVehicleDataAdapter VehicleDataAdapter { get; } = new LorryVehicleDataAdapter();
			protected abstract IEngineDataAdapter EngineDataAdapter { get; }
			protected abstract IGearboxDataAdapter GearboxDataAdapter { get; }
			protected abstract IAuxiliaryDataAdapter AuxDataAdapter { get; }

			protected abstract IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; }

			protected abstract IPTODataAdapter PtoDataAdapter { get; }

			protected virtual IElectricMachinesDataAdapter ElectricMachinesDataAdapter => throw new NotImplementedException();

			public virtual DriverData CreateDriverData(Segment segment)
			{
				return _driverDataAdapter.CreateDriverData(segment);
			}

			protected abstract GearboxType[] SupportedGearboxTypes { get; }

			public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return VehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading.Value.Item1,
					loading.Value.Item2, allowVocational);
			}

			public abstract void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData);


			// serial hybrids
			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, 
				SuperCapData runDataSuperCapData,
				Kilogram vehicleMass, 
				OvcHevMode ovcMode, 
				LoadingType loading, 
				VehicleClass vehicleClass, 
				MissionType missionType)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(runDataBatteryData,
					runDataSuperCapData, vehicleMass, ovcMode);
			}

			// parallel hybrids
			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
				SuperCapData runDataSuperCapData,
				Kilogram vehicleMass,
				OvcHevMode ovcMode,
				LoadingType loading,
				VehicleClass vehicleClass,
				MissionType missionType,
				TableData boostingLimitations, 
				GearboxData gearboxData, 
				CombustionEngineData engineData,
				IList<Tuple<PowertrainPosition, ElectricMotorData>> emData,
				ArchitectureID archId)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(
					batterySystemData: runDataBatteryData,
					superCap: runDataSuperCapData,
					ovcMode: ovcMode,
					loading: loading,
					vehicleClass: vehicleClass,
					missionType: missionType, archID: archId, engineData: engineData, emData, gearboxData: gearboxData, boostingLimitations: boostingLimitations);
			}


			public virtual AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission,
				Segment segment)
			{
				return _airdragDataAdapter.CreateAirdragData(airdragData, mission, segment);
			}

			public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				return _axleGearDataAdapter.CreateAxleGearData(axlegearData);
			}


			public AngledriveData CreateAngledriveData(IAngledriveInputData data)
			{
				return _angleDriveDataAdapter.CreateAngledriveData(data, false);
			}

			public virtual CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
				IEngineModeDeclarationInputData engineMode,
				Mission mission)
			{
				return EngineDataAdapter.CreateEngineData(vehicle, engineMode, mission);
			}

			public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return GearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, SupportedGearboxTypes);
			}

			public virtual ShiftStrategyParameters CreateGearshiftData(double axleRatio,
				PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
			{
				return GearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gearboxType, gearsCount);
			}

			public RetarderData CreateRetarderData(IRetarderInputData retarderData, ArchitectureID archID,
				IIEPCDeclarationInputData iepcInputData)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData, archID, iepcInputData);
			}

			public virtual PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto)
			{
				return PtoDataAdapter.CreateDefaultPTOData(pto, gbx);
			}

			public virtual PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData,
				IGearboxDeclarationInputData gbx)
			{
				return PtoDataAdapter.CreatePTOTransmissionData(ptoData, gbx);
			}


			public virtual IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				return AuxDataAdapter.CreateAuxiliaryData(auxData, null, missionType, vehicleClass, vehicleLength, numSteeredAxles,
					jobType);
			}

			public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
			{
				return _axleGearDataAdapter.CreateDummyAxleGearData(gbxData);
			}

			public virtual IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
				IElectricMachinesDeclarationInputData electricMachines,
				IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage,
				GearList gears = null)
			{
				return ElectricMachinesDataAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage,
					gears);
			}

			public virtual List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(
				IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				return ElectricMachinesDataAdapter.CreateIEPCElectricMachines(iepc, averageVoltage);
			}

			public RetarderData CreateGenericRetarderData(IRetarderInputData retarderData, VectoRunData vectoRun)
			{
				throw new NotImplementedException("Not applicable to Heavy Lorries");
			}
		}

		public class Conventional : LorryBase
		{
			public static GearboxType[] SupportsGearboxTypes = { GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType
				.ATSerial};

			protected override GearboxType[] SupportedGearboxTypes => new []
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			protected override IAuxiliaryDataAdapter AuxDataAdapter { get; } = new HeavyLorryAuxiliaryDataAdapter();
			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();
			protected override IPTODataAdapter PtoDataAdapter { get; } = new PTODataAdapterLorry();

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}
		}

		public abstract class Hybrid : LorryBase
		{
			private IElectricStorageAdapter _eletricStorageAdapter = new ElectricStorageAdapter();
			protected IAuxiliaryDataAdapter _auxAdapter = new HeavyLorryAuxiliaryDataAdapter();

			protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

			protected override IAuxiliaryDataAdapter AuxDataAdapter => _auxAdapter;

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _eletricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
				var superCapData = _eletricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

				if (batteryData != null) {
					setBatteryData(batteryData);
				}
				if (superCapData != null) {
					setSuperCapData(superCapData);
				}

				if (batteryData != null && superCapData != null) {
					throw new VectoException("Either battery or super cap must be provided");
				}
			}
		}

		public abstract class SerialHybrid : Hybrid
		{
			#region Overrides of LorryBase

			protected override GearboxType[] SupportedGearboxTypes { get; }

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				SerialHybridStrategyParameterDataAdapter();

			protected override IPTODataAdapter PtoDataAdapter { get; } = new ElectricPTODataAdapter();

			#endregion

		}

		public abstract class ParallelHybrid : Hybrid
		{
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				ParallelHybridStrategyParameterDataAdapter();

			protected override IPTODataAdapter PtoDataAdapter { get; } = new PTODataAdapterLorry();

		}

		public abstract class BatteryElectric : LorryBase
		{
			#region Overrides of LorryBase


			protected override GearboxType[] SupportedGearboxTypes { get; }

			//private readonly GearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(null);
			private readonly IElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();
			private readonly ElectricMachinesDataAdapter _electricMachineAdapter = new ElectricMachinesDataAdapter();
			
			private readonly HeavyLorryPEVAuxiliaryDataAdapter
				_auxDataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(null);

			protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			protected override IPTODataAdapter PtoDataAdapter { get; } = new ElectricPTODataAdapter();

			protected override IAuxiliaryDataAdapter AuxDataAdapter => _auxDataAdapter;

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
				var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

				
				if (batteryData == null) {
					throw new VectoException("Could not create BatterySystem for PEV");
				}
				setBatteryData(batteryData);
				
				
				if (superCapData != null) {
					throw new VectoException("Supercaps are not allowed for PEVs");
				}

			}

			#endregion
		}

		public class HEV_S2 : SerialHybrid
		{
			#region Overrides of LorryBase
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			#endregion
		}
		public class HEV_S3 : SerialHybrid { }
		public class HEV_S4 : SerialHybrid { }

		public class HEV_S_IEPC : SerialHybrid
		{
			
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };

			private ElectricMachinesDataAdapter _electricMachinesDataAdapter = new ElectricMachinesDataAdapter();

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

		}

		public class HEV_P1 : ParallelHybrid
		{
			
		}

		public class HEV_P2 : ParallelHybrid
		{
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.IHPC, };
		}

		public class HEV_P2_5 : ParallelHybrid
		{

		}
		public class HEV_P3 : ParallelHybrid { }
		public class HEV_P4 : ParallelHybrid { }

		public class PEV_E2 : BatteryElectric
		{
			#region Overrides of BatteryElectric

			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };

			#endregion


		}
		public class PEV_E3 : BatteryElectric { }
		public class PEV_E4 : BatteryElectric { }

		public class PEV_E_IEPC : BatteryElectric
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

		}
		public class Exempted : LorryBase
		{
			#region Overrides of LorryBase

			protected override GearboxType[] SupportedGearboxTypes { get; }

			protected override IVehicleDataAdapter VehicleDataAdapter { get; } = new ExemptedLorryVehicleDataAdapter();

			//public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
			//	KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			//{
			//	return _vehicleDataAdapter.CreateExemptedVehicleData(vehicle);
			//}

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				throw new NotImplementedException();
			}

			protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IAuxiliaryDataAdapter AuxDataAdapter => throw new NotImplementedException();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			protected override IPTODataAdapter PtoDataAdapter => throw new NotImplementedException();

			#endregion
		}
	}
}