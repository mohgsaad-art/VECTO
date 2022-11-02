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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public abstract class BaseShiftStrategy : LoggingObject, IShiftStrategy
	{
		protected readonly IDataBus DataBus;
		protected readonly GearboxData GearboxModelData;
		protected readonly ShiftStrategyParameters GearshiftParams;

		protected readonly GearList Gears;

		protected BaseShiftStrategy(IVehicleContainer dataBus)
		{
			GearboxModelData = dataBus.RunData.GearboxData;
			GearshiftParams = dataBus.RunData.GearshiftParameters;
			DataBus = dataBus;

			Gears = GearboxModelData.GearList;
		}

		public virtual bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response)
		{
			CheckGearshiftRequired = true;
			var retVal = DoCheckShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, response);
			CheckGearshiftRequired = false;
			return retVal;
		}

		protected abstract bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response);

		public abstract GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity);
		public abstract GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity);
		public abstract void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity);
		public abstract IGearbox Gearbox { get; set; }
		public abstract GearshiftPosition NextGear { get; }

		public bool CheckGearshiftRequired { get; protected set; }
		public GearshiftPosition MaxStartGear { get; protected set; }

		public virtual void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{ }

		public virtual void WriteModalResults(IModalDataContainer container)
		{
			
		}

		public abstract ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null);

		protected MeterPerSquareSecond EstimateAccelerationForGear(GearshiftPosition gear, PerSecond gbxAngularVelocityOut)
		{
			if (!Gears.Contains(gear)) {
				throw new VectoSimulationException("EstimateAccelerationForGear: invalid gear: {0}", gear);
			}

			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;

			var nextEngineSpeed = gbxAngularVelocityOut * GearboxModelData.Gears[gear.Gear].Ratio;
			var maxEnginePower = DataBus.EngineInfo.EngineStationaryFullPower(nextEngineSpeed);

			var avgSlope =
				((DataBus.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
				DataBus.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = DataBus.VehicleInfo.RollingResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var gearboxLoss = GearboxModelData.Gears[gear.Gear].LossMap.GetTorqueLoss(gbxAngularVelocityOut,
				maxEnginePower / nextEngineSpeed * GearboxModelData.Gears[gear.Gear].Ratio).Value * nextEngineSpeed;
			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.VehicleInfo.SlopeResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var axleLoss = DataBus.AxlegearInfo.AxlegearLoss();

			var accelerationPower = maxEnginePower - gearboxLoss - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;

			var acceleration = accelerationPower / DataBus.VehicleInfo.VehicleSpeed / (DataBus.VehicleInfo.TotalMass + DataBus.WheelsInfo.ReducedMassWheels);

			return acceleration.Cast<MeterPerSquareSecond>();
		}

	}
}