using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    internal abstract class DriverDataAdapter : IDriverDataAdapter, IDriverDataAdapterBus
	{
		protected DriverDataAdapter() { }

		#region Implementation of IDriverDataAdapter

		public DriverData CreateDriverData(Segment segment)
		{
			var data = DoCreateDriverData(segment);
			data.EngineStopStart = GetEngineStopStartData(null, null, null);
			return data;
		}

		#endregion

		#region Implementation of IDriverDataAdapterBus

		public DriverData CreateBusDriverData(Segment segment, VectoSimulationJobType jobType, ArchitectureID arch, CompressorDrive compressorDrive)
		{
			var data = DoCreateDriverData(segment);
			data.EngineStopStart = GetEngineStopStartData(jobType, arch, compressorDrive);
			return data;
		}

		#endregion

		protected virtual DriverData DoCreateDriverData(Segment segment)
		{
            var lookAheadData = new DriverData.LACData
            {
                Enabled = DeclarationData.Driver.LookAhead.Enabled,

                //Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
                MinSpeed = DeclarationData.Driver.LookAhead.MinimumSpeed,
                LookAheadDecisionFactor = new LACDecisionFactor(),
                LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
            };
            var overspeedData = new DriverData.OverSpeedData
            {
                Enabled = true,
                MinSpeed = DeclarationData.Driver.OverSpeed.MinSpeed,
                OverSpeed = DeclarationData.Driver.OverSpeed.AllowedOverSpeed,
            };

            var retVal = new DriverData
            {
                LookAheadCoasting = lookAheadData,
                OverSpeed = overspeedData,
               
                EcoRoll = new DriverData.EcoRollData()
                {
                    UnderspeedThreshold = DeclarationData.Driver.EcoRoll.UnderspeedThreshold,
                    MinSpeed = DeclarationData.Driver.EcoRoll.MinSpeed,
                    ActivationPhaseDuration = DeclarationData.Driver.EcoRoll.ActivationDelay,
                    AccelerationLowerLimit = DeclarationData.Driver.EcoRoll.AccelerationLowerLimit,
                    AccelerationUpperLimit = DeclarationData.Driver.EcoRoll.AccelerationUpperLimit,
                },
                PCC = new DriverData.PCCData()
                {
                    PCCEnableSpeed = DeclarationData.Driver.PCC.PCCEnableSpeed,
                    MinSpeed = DeclarationData.Driver.PCC.MinSpeed,
                    PreviewDistanceUseCase1 = DeclarationData.Driver.PCC.PreviewDistanceUseCase1,
                    PreviewDistanceUseCase2 = DeclarationData.Driver.PCC.PreviewDistanceUseCase2,
                    UnderSpeed = DeclarationData.Driver.PCC.Underspeed,
                    OverspeedUseCase3 = DeclarationData.Driver.PCC.OverspeedUseCase3
                }
            };

            retVal.AccelerationCurve = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            return retVal;
        }

		protected abstract DriverData.EngineStopStartData GetEngineStopStartData(VectoSimulationJobType? jobType,
			ArchitectureID? arch, CompressorDrive? compressorDrive);



	}

	internal sealed class LorryDriverDataAdapter : DriverDataAdapter
	{

		#region Overrides of DriverDataAdapter

		protected override DriverData.EngineStopStartData GetEngineStopStartData(VectoSimulationJobType? jobType,
			ArchitectureID? arch, CompressorDrive? compressorDrive)
		{
			var engineStopStartLorry = DeclarationData.Driver.GetEngineStopStartLorry();
			
			
			return new DriverData.EngineStopStartData()
			{
				EngineOffStandStillActivationDelay = engineStopStartLorry.ActivationDelay,
				MaxEngineOffTimespan = engineStopStartLorry.MaxEngineOffTimespan,
				UtilityFactorStandstill = engineStopStartLorry.UtilityFactor,
				UtilityFactorDriving = engineStopStartLorry.UtilityFactor,
			};
		}

        #endregion
	}

    internal abstract class BusDriverDataAdapter : DriverDataAdapter
	{
		#region Overrides of DriverDataAdapter

		protected override DriverData.EngineStopStartData GetEngineStopStartData(VectoSimulationJobType? jobType,
			ArchitectureID? arch, CompressorDrive? compressorDrive)
		{
			DeclarationData.Driver.IEngineStopStart busEngineStartStop;
			try {
				busEngineStartStop =
					DeclarationData.Driver.GetEngineStopStartBus(jobType.Value, arch.Value, compressorDrive.Value);
			} catch (InvalidOperationException ioe) {
				throw new VectoException($"Driver input: JobType, Architecture and Compressor Drive must be provided for Buses.{Environment.NewLine}{ioe.Message}", 
					ErrorCodes.SYSTEM_INVALID_OPERATION,
					$"Make sure to provide valid JobType, Architecture and Compressor Drive.");
			}

			return new DriverData.EngineStopStartData() {
				EngineOffStandStillActivationDelay = busEngineStartStop.ActivationDelay,
				MaxEngineOffTimespan = busEngineStartStop.MaxEngineOffTimespan,
				UtilityFactorStandstill = busEngineStartStop.UtilityFactor,
				UtilityFactorDriving = busEngineStartStop.UtilityFactor,
			};
		}

		#endregion
	}



    internal sealed class PrimaryBusDriverDataAdapter : BusDriverDataAdapter
	{
		#region Overrides of DriverDataAdapter

		protected override DriverData DoCreateDriverData(Segment segment)
		{
			var retVal = base.DoCreateDriverData(segment);
			retVal.LookAheadCoasting.Enabled = true;
			return retVal;
        }

		#endregion
	}

	internal sealed class CompletedBusGenericDriverDataAdapter : BusDriverDataAdapter
	{
		protected override DriverData DoCreateDriverData(Segment segment)
		{
			var retVal = base.DoCreateDriverData(segment);
			retVal.LookAheadCoasting.Enabled = false;
			retVal.OverSpeed.Enabled = false;
			return retVal;
		}
    }

	internal sealed class CompletedBusSpecificDriverDataAdapter : BusDriverDataAdapter
	{
		protected override DriverData DoCreateDriverData(Segment segment)
		{
			var retVal = base.DoCreateDriverData(segment);
			retVal.LookAheadCoasting.Enabled = false;
			retVal.OverSpeed.Enabled = false;
			return retVal;
        }
	}
}

