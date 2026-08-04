using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific
{
    public abstract class DeclarationDataAdapterSpecificCompletedBus
	{
		public abstract class CompletedBusDeclarationBase : AbstractSimulationDataAdapter, ISpecificCompletedBusDeclarationDataAdapter
        {
			protected virtual IAirdragDataAdapter AirdragDataAdapter => new CompletedBusSpecificAirdragDataAdapter();

			protected virtual IVehicleDataAdapter VehicleDataAdapter { get; } = new CompletedBusSpecificVehicleDataAdapter();

			protected virtual ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } =
				new SpecificCompletedBusAuxiliaryDataAdapter();

            #region Implementation of ISpecificCompletedBusDeclarationDataAdapter

            public virtual IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
				IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
			{
                return AuxDataAdapter.CreateBusAuxiliariesData(mission, primaryVehicle, completedVehicle, runData);
            }

			public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
				IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				return VehicleDataAdapter.CreateVehicleData(primaryVehicle, completedVehicle, segment, mission,
					loading);
			}

			public virtual IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType, bool batteryOnlyHybridMode)
			{
				return AuxDataAdapter.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass,
					vehicleLength, numSteeredAxles, jobType, batteryOnlyHybridMode);
			}

			public virtual AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission,
				Segment segment, OvcHevMode ovcMode)
			{
				return AirdragDataAdapter.CreateAirdragData(completedVehicle, mission, segment, ovcMode);
			}

            #endregion
        }


        public class Conventional : CompletedBusDeclarationBase { }

		public abstract class Hybrid : CompletedBusDeclarationBase { }

		public abstract class SerialHybrid : Hybrid {}

		public class HEV_S2 : SerialHybrid { }
        
		public class HEV_S3 : SerialHybrid { }
        
		public class HEV_S4 : SerialHybrid { }
        
		public class HEV_S_IEPC : SerialHybrid { }

		public abstract class ParallelHybrid : Hybrid { }
        
		public class HEV_P1 : ParallelHybrid { }
        
		public class HEV_P2 : ParallelHybrid { }
        
		public class HEV_P2_5 : ParallelHybrid { }
        
		public class HEV_P3 : ParallelHybrid { }
        
		public class HEV_P4 : ParallelHybrid { }

		public abstract class BatteryElectric : CompletedBusDeclarationBase
		{

			protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } =
				new SpecificCompletedPEVBusAuxiliaryDataAdapter();

		}
        
		public class PEV_E2 : BatteryElectric { }
        
		public class PEV_E3 : BatteryElectric { }
        
		public class PEV_E4 : BatteryElectric { }
        
		public class PEV_E_IEPC : BatteryElectric { }

		public abstract class FCHV : BatteryElectric { }

		public class FCHV_F2 : FCHV { }

        public class FCHV_F3 : FCHV { }

        public class FCHV_F4 : FCHV { }

        public class FCHV_IEPC : FCHV { }

		public abstract class MultiplePowertrains : CompletedBusDeclarationBase
		{
			protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter => null;
        }

		public class MultiplePEV : MultiplePowertrains
		{
            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new SpecificCompletedPEVBusAuxiliaryDataAdapter();
        }

		public class MultipleFCHV : MultiplePEV { }

		public class MultipleSHEV : MultiplePowertrains
		{
            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new SpecificCompletedBusAuxiliaryDataAdapter();
        }

        public class Exempted : CompletedBusDeclarationBase
		{
			protected override IVehicleDataAdapter VehicleDataAdapter { get; } =
				new ExemptedCompletedBusSpecificVehicleDataAdapter();

			#region Overrides of CompletedBusDeclarationBase

			protected override IAirdragDataAdapter AirdragDataAdapter => null;

			protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter => null;

			#endregion
		}
	}
}