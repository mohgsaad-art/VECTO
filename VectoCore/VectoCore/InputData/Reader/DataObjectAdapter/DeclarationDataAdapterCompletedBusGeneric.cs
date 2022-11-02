using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterCompletedBusGeneric : DeclarationDataAdapterPrimaryBus
	{
		private readonly GenericTransmissionComponentData _genericPowertrainData = new GenericTransmissionComponentData();
		private readonly GenericBusRetarderData _genericRetarderData = new GenericBusRetarderData();
		private readonly GenericTorqueConverterData _genericTorqueConverterData = new GenericTorqueConverterData();

		public const double GearEfficiencyDirectGear = 0.98;
		public const double GearEfficiencyIndirectGear = 0.96;
		public const double GearEfficiencyAT = 0.925;


		#region Overrides of DeclarationDataAdapterPrimaryBus

		public override DriverData CreateDriverData()
		{
			var retVal = base.CreateDriverData();
			retVal.LookAheadCoasting.Enabled = false;
			retVal.OverSpeed.Enabled = false;
			return retVal;
		}

		public override VehicleData CreateVehicleData(
			IVehicleDeclarationInputData data, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			var retVal =  base.CreateVehicleData(data, segment, mission, loading, allowVocational);
			retVal.GrossVehicleMass = data.GrossVehicleMassRating;
			if (retVal.TotalVehicleMass.IsGreater(retVal.GrossVehicleMass)) {
				throw new VectoException("Total Vehicle Mass exceeds Gross Vehicle Mass for completed bus generic ({0}/{1})", retVal.TotalVehicleMass, retVal.GrossVehicleMass);
			}
			return retVal;
		}

		#endregion

		// The model parameters for the completed bus with generic power train and generic body is basically the same as the primary bus
		// only powertrain components are different

		
		public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
		{
			return GenericBusEngineData.Instance.CreateGenericBusEngineData(primaryVehicle, modeIdx, mission);
		}

		#region Overrides of DeclarationDataAdapterHeavyLorry

		public override AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
		{
			return _genericPowertrainData.CreateGenericBusAxlegearData(axlegearData);
		}

		public override AngledriveData CreateAngledriveData(IAngledriveInputData data)
		{
			return _genericPowertrainData.CreateGenericBusAngledriveData(data);
		}


		public override RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return _genericRetarderData.CreateGenericBusRetarderData(retarder);
		}

		#endregion

		#region Overrides of AbstractSimulationDataAdapter

		protected override TransmissionLossMap CreateGearLossMap(ITransmissionInputData gear, uint i, bool useEfficiencyFallback, VehicleCategory vehicleCategory, GearboxType gearboxType)
		{
			if (gearboxType.AutomaticTransmission()) {
				return TransmissionLossMapReader.Create(GearEfficiencyAT, gear.Ratio, $"Gear {i + 1}");
			}
			return TransmissionLossMapReader.Create(
				gear.Ratio.IsEqual(1) ? GearEfficiencyDirectGear : GearEfficiencyIndirectGear, gear.Ratio, $"Gear {i + 1}");
		}

		protected override void CretateTCFirstGearATPowerSplit(GearData gearData, uint i, ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = 1;
			//gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(GearEfficiencyIndirectGear, 1, string.Format("TCGear {0}", i + 1));
			gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(1.0, 1, $"TCGear {i + 1}");
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		#endregion

		#region Overrides of DeclarationDataAdapterHeavyLorry

		protected override TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
			ITorqueConverterDeclarationInputData torqueConverter, double ratio, CombustionEngineData engineData)
		{
			if (torqueConverter != null && torqueConverter.TCData != null) {
				return TorqueConverterDataReader.Create(
					torqueConverter.TCData,
					DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
					ExecutionMode.Engineering, ratio,
					DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
					DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
			}
			return _genericTorqueConverterData.CreateTorqueConverterData(gearboxType, ratio, engineData);
		}

		#endregion

		
	}
}
