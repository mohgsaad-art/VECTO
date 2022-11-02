using System;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class GenericTorqueConverterData
	{
		public TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType, double ratio,
			CombustionEngineData engineData)
		{
			var genericTCData = gearboxType == GearboxType.ATPowerSplit
				? CreateGenericTorqueConverterCharacteristicsPowersplit(ratio, engineData.FullLoadCurves[0].RatedSpeed,
					engineData.FullLoadCurves[0].MaxTorque)
				: CreateGenericTorqueConverterCharacteristics(
					ratio, engineData.FullLoadCurves[0].RatedSpeed, engineData.FullLoadCurves[0].MaxTorque);

			return TorqueConverterDataReader.Create(
				genericTCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratio,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
		}

		public static DataTable CreateGenericTorqueConverterCharacteristics(double ratio, PerSecond ratedSpeed,
			NewtonMeter maxTorque)
		{
			var engRatedSpeed = ratedSpeed.AsRPM;
			var engMaxTorque = maxTorque.Value();

			const double speedAtStallPtFactor = 0.7;
			const double torqueAtStallPtFactor = 0.8;

			const double refTorqueIntermediatePtFactor = 0.8;
			const double refTorqueCouplingPtFactor = 0.5;
			const double refTorqueOverrunPtFactor = 0;


			const double speedRAtioOverrunPt = 1.0;
			const double speedRatioCouplingPt = 0.9;
			const double speedRatioIntermediatePt = 0.6;
			const double speedRatioStallPt = 0.0;

			const double torqueRatioStallPt = 1.8;
			const double torqueRatioCouplingPt = 0.95;
			const double torqueRatioOverrunPt = 0.94;

			const double tcRefSpeed = 1000;

			var retVal = new DataTable();
			var colSpeedRatio = retVal.Columns.Add(TorqueConverterDataReader.Fields.SpeedRatio);
			var colTqRatio = retVal.Columns.Add(TorqueConverterDataReader.Fields.TorqueRatio);
			var colRefTq = retVal.Columns.Add(TorqueConverterDataReader.Fields.CharacteristicTorque);



			var refTorqueStallPt = torqueAtStallPtFactor * engMaxTorque *
									Math.Pow(tcRefSpeed / (speedAtStallPtFactor * engRatedSpeed), 2);

			var torqueRatioIntermediatePt = VectoMath.Interpolate(
				speedRatioStallPt, speedRatioCouplingPt, torqueRatioStallPt, torqueRatioCouplingPt,
				speedRatioIntermediatePt);

			// stall point
			var stallPt = retVal.NewRow();
			stallPt[colSpeedRatio] = Math.Round(speedRatioStallPt * ratio, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			stallPt[colTqRatio] = Math.Round(torqueRatioStallPt / speedRAtioOverrunPt / ratio, 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			stallPt[colRefTq] = Math.Round(refTorqueStallPt, 2, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			retVal.Rows.Add(stallPt);

			// intermediate point
			var intermediatePt = retVal.NewRow();
			intermediatePt[colSpeedRatio] =
				Math.Round(speedRatioIntermediatePt * ratio, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			intermediatePt[colTqRatio] =
				Math.Round(torqueRatioIntermediatePt / ratio, 2, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			intermediatePt[colRefTq] = Math.Round(refTorqueStallPt * refTorqueIntermediatePtFactor, 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			retVal.Rows.Add(intermediatePt);

			//coupling point
			var couplingPt = retVal.NewRow();
			couplingPt[colSpeedRatio] = Math.Round(speedRatioCouplingPt * speedRAtioOverrunPt * ratio, 4,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			couplingPt[colTqRatio] = Math.Round(torqueRatioCouplingPt / speedRAtioOverrunPt / ratio, 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			couplingPt[colRefTq] = Math.Round(refTorqueStallPt * refTorqueCouplingPtFactor, 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			retVal.Rows.Add(couplingPt);

			// overrun point
			var overrunPt = retVal.NewRow();
			overrunPt[colSpeedRatio] = Math.Round(speedRAtioOverrunPt * ratio, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			overrunPt[colTqRatio] = Math.Round(torqueRatioOverrunPt / speedRAtioOverrunPt / ratio, 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			overrunPt[colRefTq] =
				Math.Round(refTorqueStallPt * refTorqueOverrunPtFactor, 2, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			retVal.Rows.Add(overrunPt);

			return retVal;
		}

		public static DataTable CreateGenericTorqueConverterCharacteristicsPowersplit(double ratio,
			PerSecond ratedSpeed,
			NewtonMeter maxTorque)
		{
			var engRatedSpeed = ratedSpeed.AsRPM;
			var engMaxTorque = maxTorque.Value();

			const double speedAtStallPtFactor = 0.7;
			const double torqueAtStallPtFactor = 0.8;

			const double refTorqueIntermediatePtFactor = 0.5;
			const double refTorqueCouplingPtFactor = 0.15;
			const double refTorqueOverrunPtFactor = 0;

			const double speedRatioCouplingPtFactor = 0.9;
			const double speedRatioIntermediatePtFactor = 0.6;
			const double speedRatioStallPt = 0.0;
			const double nue_step = 0.1;

			const double torqueRatioStallPtFactor = 3.8;
			const double torqueRatioCouplingPtFactor = 0.76;
			const double torqueRatioOverrunPtFactor = 0.65;


			var speedRatioOverrunPt = 1.0 / ratio;

			var torqueRatioStallPt = torqueRatioStallPtFactor / speedRatioOverrunPt;
			var torqueRatioCouplingPt = torqueRatioCouplingPtFactor / speedRatioOverrunPt;
			var torqueRatioOverrunPt = torqueRatioOverrunPtFactor / speedRatioOverrunPt;

			var speedRatioCouplingPt = speedRatioCouplingPtFactor * speedRatioOverrunPt;

			var c = (torqueRatioStallPt - torqueRatioCouplingPt) / Math.Pow(speedRatioCouplingPt, 0.5);
			const double tcRefSpeed = 1000;

			var retVal = new DataTable();
			var colSpeedRatio = retVal.Columns.Add(TorqueConverterDataReader.Fields.SpeedRatio);
			var colTqRatio = retVal.Columns.Add(TorqueConverterDataReader.Fields.TorqueRatio);
			var colRefTq = retVal.Columns.Add(TorqueConverterDataReader.Fields.CharacteristicTorque);

			var refTorqueStallPt = torqueAtStallPtFactor * engMaxTorque *
									Math.Pow(tcRefSpeed / (speedAtStallPtFactor * engRatedSpeed), 2);

			var speedRatioIntermediatePt = speedRatioIntermediatePtFactor * speedRatioOverrunPt;
			var refTorqueIntermediatePt = refTorqueStallPt * refTorqueIntermediatePtFactor;
			var refTorqueCouplingPt = refTorqueStallPt * refTorqueCouplingPtFactor;

            // stall point
            var stallPt = retVal.NewRow();
			stallPt[colSpeedRatio] = Math.Round(speedRatioStallPt, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			stallPt[colTqRatio] = Math.Round(torqueRatioStallPt - c * Math.Pow(speedRatioStallPt, 0.5), 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			stallPt[colRefTq] = Math.Round(refTorqueStallPt, 2, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			retVal.Rows.Add(stallPt);

			for (var i = speedRatioStallPt + nue_step; i.IsSmaller(speedRatioIntermediatePtFactor, nue_step / 10); i += nue_step) {
				var pt = retVal.NewRow();
				var nue = i * speedRatioOverrunPt;
				var mue = torqueRatioStallPt - c * Math.Pow(nue, 0.5);
				pt[colSpeedRatio] = Math.Round(nue, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
				pt[colTqRatio] = Math.Round(mue, 2, MidpointRounding.AwayFromZero).ToXMLFormat(6);
				pt[colRefTq] = Math.Round(
					refTorqueStallPt + (refTorqueIntermediatePt - refTorqueStallPt) / speedRatioIntermediatePt *
					nue, 2,
					MidpointRounding.AwayFromZero).ToXMLFormat(6);
				retVal.Rows.Add(pt);
			}

			// intermediate point
            var intermediatePt = retVal.NewRow();
			
			intermediatePt[colSpeedRatio] = Math.Round(speedRatioIntermediatePt, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			intermediatePt[colTqRatio] = Math.Round(torqueRatioStallPt - c * Math.Pow(speedRatioIntermediatePt, 0.5), 2,
					MidpointRounding.AwayFromZero).ToXMLFormat(6);
			intermediatePt[colRefTq] = Math.Round(refTorqueIntermediatePt, 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			retVal.Rows.Add(intermediatePt);

			for (var i = speedRatioIntermediatePtFactor + nue_step; i.IsSmaller(speedRatioCouplingPtFactor, nue_step/10); i += nue_step)
			{
				var pt = retVal.NewRow();
				var nue = i * speedRatioOverrunPt;
				var mue = torqueRatioStallPt - c * Math.Pow(nue, 0.5);
				pt[colSpeedRatio] = Math.Round(nue, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
				pt[colTqRatio] = Math.Round(mue, 2, MidpointRounding.AwayFromZero).ToXMLFormat(6);
				pt[colRefTq] = Math.Round(
					refTorqueIntermediatePt + (refTorqueCouplingPt - refTorqueIntermediatePt) / (speedRatioCouplingPt - speedRatioIntermediatePt ) *
					(nue - speedRatioIntermediatePt), 2,
					MidpointRounding.AwayFromZero).ToXMLFormat(6);
				retVal.Rows.Add(pt);
			}

            //coupling point
            var couplingPt = retVal.NewRow();
			couplingPt[colSpeedRatio] = Math.Round(speedRatioCouplingPt, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			couplingPt[colTqRatio] = Math.Round(torqueRatioStallPt - c * Math.Pow(speedRatioCouplingPt, 0.5), 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			couplingPt[colRefTq] = Math.Round(refTorqueCouplingPt, 2, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			retVal.Rows.Add(couplingPt);

			// overrun point
			var overrunPt = retVal.NewRow();
			overrunPt[colSpeedRatio] = Math.Round(speedRatioOverrunPt, 4, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			overrunPt[colTqRatio] = Math.Round(torqueRatioOverrunPt / speedRatioOverrunPt / ratio, 2,
				MidpointRounding.AwayFromZero).ToXMLFormat(6);
			overrunPt[colRefTq] =
				Math.Round(refTorqueStallPt * refTorqueOverrunPtFactor, 2, MidpointRounding.AwayFromZero).ToXMLFormat(6);
			retVal.Rows.Add(overrunPt);

			return retVal;
		}
	}
}
