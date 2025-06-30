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

using System.Collections.Generic;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.OutputData.XML
{
	internal static class AttributeMappings
	{
		public static readonly Dictionary<string, string> FuelConsumptionMapMapping = new Dictionary<string, string> {
			{ FuelConsumptionMapReader.Fields.EngineSpeed, XMLNames.Engine_FuelConsumptionMap_EngineSpeed_Attr },
			{ FuelConsumptionMapReader.Fields.Torque, XMLNames.Engine_FuelConsumptionMap_Torque_Attr },
			{ FuelConsumptionMapReader.Fields.FuelConsumption, XMLNames.Engine_FuelConsumptionMap_FuelConsumption_Attr }
		};

		public static readonly Dictionary<string, string> WHRPowerMapMapping = new Dictionary<string, string> {
			{ WHRPowerReader.Fields.EngineSpeed, XMLNames.Engine_FuelConsumptionMap_EngineSpeed_Attr },
			{ WHRPowerReader.Fields.Torque, XMLNames.Engine_FuelConsumptionMap_Torque_Attr },
			{ WHRPowerReader.Fields.ElectricPower, XMLNames.Engine_FuelConsumptionMap_WHRElPower_Attr },
			{ WHRPowerReader.Fields.MechanicalPower, XMLNames.Engine_FuelConsumptionMap_WHRMechPower_Attr }
		};

		public static readonly Dictionary<string, string> EngineFullLoadCurveMapping = new Dictionary<string, string> {
			{ FullLoadCurveReader.Fields.EngineSpeed, XMLNames.Engine_EngineFullLoadCurve_EngineSpeed_Attr },
			{ FullLoadCurveReader.Fields.TorqueFullLoad, XMLNames.Engine_FullLoadCurve_MaxTorque_Attr },
			{ FullLoadCurveReader.Fields.TorqueDrag, XMLNames.Engine_FullLoadCurve_DragTorque_Attr },
			{ "PT1", "PT1" }
		};

		public static readonly Dictionary<string, string> ShiftPolygonMapping = new Dictionary<string, string> {
			{ ShiftPolygonReader.Fields.Torque, XMLNames.Gear_ShiftPolygon_EngineTorque_Attr },
			{ ShiftPolygonReader.Fields.AngularSpeedDown, XMLNames.Gear_ShiftPolygonMapping_DownshiftSpeed_Attr },
			{ ShiftPolygonReader.Fields.AngularSpeedUp, XMLNames.Gear_ShiftPolygonMapping_UpshiftSpeed_Attr }
		};

		public static readonly Dictionary<string, string> RetarderLossmapMapping = new Dictionary<string, string> {
			{ RetarderLossMapReader.Fields.RetarderSpeed, XMLNames.Retarder_RetarderLossmap_RetarderSpeed_Attr },
			{ RetarderLossMapReader.Fields.TorqueLoss, XMLNames.Retarder_RetarderLossmap_TorqueLoss_Attr }
		};

		public static readonly Dictionary<string, string> TorqueConverterDataMapping = new Dictionary<string, string>() {
			{ TorqueConverterDataReader.Fields.SpeedRatio, XMLNames.TorqueConverterData_SpeedRatio_Attr },
			{ TorqueConverterDataReader.Fields.TorqueRatio, XMLNames.TorqueConverterData_TorqueRatio_Attr },
			{ TorqueConverterDataReader.Fields.CharacteristicTorque, XMLNames.TorqueConverterDataMapping_InputTorqueRef_Attr }
		};

		public static readonly Dictionary<string, string> TransmissionLossmapMapping = new Dictionary<string, string>() {
			{ TransmissionLossMapReader.Fields.InputSpeed, XMLNames.TransmissionLossmap_InputSpeed_Attr },
			{ TransmissionLossMapReader.Fields.InputTorque, XMLNames.TransmissionLossmap_InputTorque_Attr },
			{ TransmissionLossMapReader.Fields.TorqeLoss, XMLNames.TransmissionLossmap_TorqueLoss_Attr }
		};

		public static readonly Dictionary<string, string> DriverAccelerationCurveMapping = new Dictionary<string, string>() {
			{ AccelerationCurveReader.Fields.Velocity, XMLNames.Vehicle_CrosswindCorrectionMap_VehicleSpeed_Attr },
			{ AccelerationCurveReader.Fields.Acceleration, XMLNames.Vehicle_AccelerationCurve_MaxAcceleration_Attr },
			{ AccelerationCurveReader.Fields.Deceleration, XMLNames.Vehicle_AccelerationCurve_MaxDeceleration_Attr }
		};

		public static readonly Dictionary<string, string> CoastingDFTargetSpeedLookupMapping =
			new Dictionary<string, string>() {
				{
					LACDecisionFactor.LACDecisionFactorVTarget.Fields.TargetVelocity,
					XMLNames.Driver_CoastingDFTargetSpeedLookupMapping_TargetVelocity_Attr
				}, {
					LACDecisionFactor.LACDecisionFactorVTarget.Fields.DecisionFactor,
					XMLNames.Driver_CoastingDFTargetSpeedLookupMapping_DecisionFactor_Attr
				}
			};

		public static readonly Dictionary<string, string> CoastingDFVelocityDropLookupMapping =
			new Dictionary<string, string>() {
				{
					LACDecisionFactor.LACDecisionFactorVdrop.Fields.VelocityDrop,
					XMLNames.Driver_CoastingDFVelocityDropLookupMapping_VelocityDrop_Attr
				}, {
					LACDecisionFactor.LACDecisionFactorVdrop.Fields.DecisionFactor,
					XMLNames.Driver_CoastingDFVelocityDropLookupMapping_DecisionFactorDrop_Attr
				}
			};

		public static readonly Dictionary<string, string> CrossWindCorrectionMapping = new Dictionary<string, string>() {
			{
				CrossWindCorrectionCurveReader.FieldsSpeedDependent.Velocity,
				XMLNames.Vehicle_CrosswindCorrectionMap_VehicleSpeed_Attr
			},
			{ CrossWindCorrectionCurveReader.FieldsSpeedDependent.Cd, XMLNames.Vehicle_CrosswindCorrectionMap_CdScalingFactor },
			{ CrossWindCorrectionCurveReader.FieldsCdxABeta.Beta, XMLNames.Vehicle_CrosswindCorrectionMap_Beta },
			{ CrossWindCorrectionCurveReader.FieldsCdxABeta.DeltaCdxA, XMLNames.Vehicle_CrosswindCorrectionMap_DeltaCdxA },
		};

		public static readonly Dictionary<string, string> PTOLossMap = new Dictionary<string, string> {
			{ PTOIdleLossMapReader.Fields.EngineSpeed, XMLNames.Vehicle_PTOIdleLossMap_EngineSpeed_Attr },
			{ PTOIdleLossMapReader.Fields.PTOTorque, XMLNames.Vehicle_PTOIdleLossMap_TorqueLoss_Attr },
		};

		public static readonly Dictionary<string, string> PTOCycleMap = new Dictionary<string, string>() {
			{ DrivingCycleDataReader.Fields.Time, XMLNames.Vehicle_PTOCycle_Time_Attr },
			{ DrivingCycleDataReader.Fields.EngineSpeed, XMLNames.Vehicle_PTOCycle_EngineSpeed_Attr },
			{ DrivingCycleDataReader.Fields.PTOTorque, XMLNames.Vehicle_PTOCycle_Torque_Attr },
		};

		#region Battery

		public static readonly Dictionary<string, string> InternalResistanceMap = new Dictionary<string, string> {
			{ BatteryInternalResistanceReader.Fields.StateOfCharge, XMLNames.REESS_InternalResistanceCurve_SoC },
			{ BatteryInternalResistanceReader.Fields.InternalResistance_2, XMLNames.REESS_InternalResistanceCurve_R2 }, {
				BatteryInternalResistanceReader.Fields.InternalResistance_10, XMLNames.REESS_InternalResistanceCurve_R10
			}, {
				BatteryInternalResistanceReader.Fields.InternalResistance_20, XMLNames.REESS_InternalResistanceCurve_R20
			}, {
				BatteryInternalResistanceReader.Fields.InternalResistance_120,
				XMLNames.REESS_InternalResistanceCurve_R120
			}
		};

		public static readonly Dictionary<string, string> VoltageMap = new Dictionary<string, string> {
			{ BatterySOCReader.Fields.StateOfCharge, XMLNames.REESS_OCV_SoC },
			{ BatterySOCReader.Fields.BatteryVoltage, XMLNames.REESS_OCV_OCV }
		};

		public static readonly Dictionary<string, string> MaxCurrentMap = new Dictionary<string, string> {
			{ BatteryMaxCurrentReader.Fields.StateOfCharge, XMLNames.REESS_CurrentLimits_SoC },
			{ BatteryMaxCurrentReader.Fields.MaxChargeCurrent, XMLNames.REESS_CurrentLimits_MaxChargingCurrent },
			{ BatteryMaxCurrentReader.Fields.MaxDischargeCurrent, XMLNames.REESS_CurrentLimits_MaxDischargingCurrent }
		};

		#endregion
		public static readonly Dictionary<string, string> EMPowerMap = new Dictionary<string, string> {
			{ ElectricMotorMapReader.Fields.MotorSpeed, XMLNames.PowerMap_OutShaftSpeed },
			{ElectricMotorMapReader.Fields.Torque, XMLNames.PowerMap_Torque },
			{ ElectricMotorMapReader.Fields.PowerElectrical, XMLNames.PowerMap_ElectricPower }
		};

		public static readonly Dictionary<string, string> EMTorqueCurve = new Dictionary<string, string> {
			{ XMLNames.MaxTorqueCurve_OutShaftSpeed, XMLNames.MaxTorqueCurve_OutShaftSpeed },
            { XMLNames.MaxTorqueCurve_MaxTorque, XMLNames.MaxTorqueCurve_MaxTorque },
            { XMLNames.MaxTorqueCurve_MinTorque, XMLNames.MaxTorqueCurve_MinTorque }
        };

		public static readonly Dictionary<string, string> BoostingLimitsMapping = new Dictionary<string, string> {
				{MaxBoostingTorqueReader.Fields.MotorSpeed, XMLNames.BoostingLimitation_RotationalSpeed},
				{MaxBoostingTorqueReader.Fields.DrivingTorque, XMLNames.BoostingLimitation_BoostingTorque}
		};

		//ElectricMotorMapReader
	}
}