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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	/// <summary>
	/// Defines interfaces for all different cockpits to access shared data of the powertrain.
	/// </summary>
	public interface IDataBus
	{
		ExecutionMode ExecutionMode { get; }

		Second AbsTime { get; set; }

		IMileageCounter MileageCounter { get; }

		IGearboxInfo GearboxInfo(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);

		IList<IGearboxInfo> GearboxesInfo {  get; }

		IGearboxControl GearboxCtl(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);

		IList<IGearboxControl> GearboxesCtl { get; }

		IAxlegearInfo AxlegearInfo(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);

		IList<IAxlegearInfo> AxlegearsInfo { get; }

		IEngineInfo EngineInfo { get; }

		IEngineControl EngineCtl { get; }

		IVehicleInfo VehicleInfo { get; }

		IClutchInfo ClutchInfo(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);

		IList<IClutchInfo> ClutchesInfo { get; }

		IRetarder Retarder(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);	

		IBrakes Brakes { get; }

		IWheelsInfo WheelsInfo { get; }

		IDriverInfo DriverInfo { get; }

		IDrivingCycleInfo DrivingCycleInfo { get; }

		IElectricMotorInfo ElectricMotorInfo(PowertrainPosition position, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);

        IList<IElectricMotorInfo> ElectricMotorsInfo { get; }

		IRESSInfo BatteryInfo { get; }

		IElectricSystemInfo ElectricSystemInfo { get; }

		IElectricSystemInfo JunctionBox {  get; }

		ITorqueSplitter TorqueSplitter { get; }

		IWheelEnd WheelEnd { get; }

		ITorqueConverterInfo TorqueConverterInfo(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);

		ITorqueConverterControl TorqueConverterCtl(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);

		IPowertainInfo PowertrainInfo { get; }

		IHybridControllerInfo HybridControllerInfo { get; }

		IHybridControllerCtl HybridControllerCtl { get; }

		IAngledriveInfo AngledriveInfo(int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN);

        IList<IAngledriveInfo> AngledrivesInfo { get; }

		IDCDCConverter DCDCConverter { get; }

		IWHRCharger WHRCharger { get; }

		bool IsTestPowertrain { get; }
	}

	public interface IPowertainInfo
	{
		bool HasCombustionEngine { get; }

		bool HasElectricMotor { get; }

		bool HasGearbox { get; }

		VectoSimulationJobType VehicleArchitecture { get; }
	}
}