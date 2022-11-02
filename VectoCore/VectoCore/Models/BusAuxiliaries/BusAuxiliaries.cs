// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries
{
	public class BusAuxiliariesNoAlternator : BusAuxiliaries
	{
		private Ampere _totalAverageDemandAmpsIncludingBaseLoad;
		private Ampere _totalAverageDemandAmpsEngineOffStandstill;
		private Ampere _totalAverageDemandAmpsEngineOffDriving;
		private ISSMPowerDemand ssmTool;

		public BusAuxiliariesNoAlternator(ISimpleBatteryInfo battery) : base(battery) { }

		public override void Initialise(IAuxiliaryConfig auxCfg)
		{
			Signals.CurrentCycleTimeInSeconds = 0;
			auxConfig = auxCfg; //new AuxiliaryConfig(auxPath);

			var compressorMap = auxConfig.PneumaticUserInputsConfig.CompressorMap;

			_totalAverageDemandAmpsIncludingBaseLoad = auxConfig.ElectricalUserInputsConfig.AverageCurrentDemandInclBaseLoad(false, false);
			_totalAverageDemandAmpsEngineOffStandstill = auxConfig.ElectricalUserInputsConfig.AverageCurrentDemandInclBaseLoad(true, true);
			_totalAverageDemandAmpsEngineOffDriving = auxConfig.ElectricalUserInputsConfig.AverageCurrentDemandInclBaseLoad(true, false);

			// SSM HVAC
			//var ssmPath = FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.HvacUserInputsConfig.SSMFilePath);
			//var BusDatabase = FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.HvacUserInputsConfig.BusDatabasePath);
			ssmTool = auxConfig.SSMInputs is ISSMEngineeringInputs ?
				new SimpleSSMTool(auxConfig.SSMInputs)
				: (ISSMPowerDemand)new SSMTOOL(auxConfig.SSMInputs);


			var electricUserInputConfigNoAlternator = new ElectricsUserInputsConfig() {
				PowerNetVoltage = auxCfg.ElectricalUserInputsConfig.PowerNetVoltage,
				AlternatorType = AlternatorType.Conventional,
				DoorActuationTimeSecond = auxCfg.ElectricalUserInputsConfig.DoorActuationTimeSecond,
				AlternatorMap = auxCfg.ElectricalUserInputsConfig.AlternatorMap,
				AlternatorGearEfficiency = auxCfg.ElectricalUserInputsConfig.AlternatorGearEfficiency
			};

			M0 = new M00Impl(electricUserInputConfigNoAlternator, Signals, 0.SI<Watt>());

			//M0_5 = new M0_5Impl(
			//	M0, auxConfig.ElectricalUserInputsConfig, Signals);

			M1 = new M01Impl(
				M0, electricUserInputConfigNoAlternator.AlternatorGearEfficiency,
				auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency, 0.SI<Watt>(),
				ssmTool.MechanicalWBaseAdjusted);

			M2 = new M02Impl(M0, electricUserInputConfigNoAlternator, Signals);

			M3 = new M03Impl(auxConfig, compressorMap, auxCfg.Actuations, Signals);

			M4 = new M04Impl(
				compressorMap, auxConfig.PneumaticUserInputsConfig.CompressorGearRatio,
				auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency, Signals);

			M5 = new M05Impl_P0(M0, M1, M2, ElectricStorage, electricUserInputConfigNoAlternator, Signals);
			M6 = new M06Impl(electricUserInputConfigNoAlternator, M1, M2, M3, M4, M5, Signals);
			M7 = new M07Impl(M0, M1, M2, M5, M6, ElectricStorage,
				electricUserInputConfigNoAlternator.AlternatorGearEfficiency, Signals);
			M8 = new M08Impl(auxConfig, M1, M6, M7, Signals);

		}

		public override Watt ElectricPowerConsumer => AveragePowerDemandAtAlternatorFromElectrics;

		public override Watt HVACElectricalPowerConsumer => ssmTool.ElectricalWAdjusted;


		public override Watt ElectricPowerConsumerSum => ssmTool.ElectricalWAdjusted + AveragePowerDemandAtAlternatorFromElectrics;

		protected Watt AveragePowerDemandAtAlternatorFromElectrics
		{
			get {
				var current = _totalAverageDemandAmpsIncludingBaseLoad;
				if (Signals.EngineStopped) {
					current = Signals.VehicleStopped
						? _totalAverageDemandAmpsEngineOffStandstill
						: _totalAverageDemandAmpsEngineOffDriving;
				}
				return auxConfig.ElectricalUserInputsConfig.PowerNetVoltage * current;
			}
		}
	}


	// #####################################################################
	// #####################################################################


	/// <summary>
	/// ''' Main entry point for the advanced auxiliary module. 
	/// ''' This class represents slide number 17 titled Calculations of Cycle FC accounting for Smart Auxiliaries.
	/// ''' </summary>
	/// ''' <remarks></remarks>
	public class BusAuxiliaries : IBusAuxiliaries
	{
		protected internal IAuxiliaryConfig auxConfig;

		// Supporting classes which may generate event messages
		//private ICompressorMap compressorMap;

		//private SSMTOOL ssmTool;
		//private SSMTOOL ssmToolModule14;

		//private IAlternatorMap alternatorMap;

		//private IFuelConsumptionMap fuelMap;

		// Classes which compose the model.
		protected IM0_NonSmart_AlternatorsSetEfficiency M0;

		//private IM0_5_SmartAlternatorSetEfficiency M0_5;
		protected IM1_AverageHVACLoadDemand M1;
		protected IM2_AverageElectricalLoadDemand M2;
		protected IM3_AveragePneumaticLoadDemand M3;
		protected IM4_AirCompressor M4;
		protected IM5_SmartAlternatorSetGeneration M5;
		protected IM6 M6;
		protected IM7 M7;
		protected IM8 M8;
		public ISimpleBatteryInfo ElectricStorage { get; protected set; }


		//private IM9 M9;
		//private IM10 M10;
		//private IM11 M11;
		//private IM12 M12;
		//private IM13 M13;
		//private IM14 M14;


		public BusAuxiliaries(ISimpleBatteryInfo battery)
		{
			Signals = new Signals();
			ElectricStorage = battery;
		}

		public virtual Joule AuxHeaterDemandCalculation(Second cycleTime, Joule engineWasteHeatTotal)
		{
			if (auxConfig == null) {
				throw new VectoException("Auxiliary configuration missing!");
			}

			if (auxConfig.SSMInputs is ISSMEngineeringInputs ssmEngineeringInputs) {
				var M14eng = new M14bImpl(ssmEngineeringInputs);
				return M14eng.AuxHeaterDemand(cycleTime, engineWasteHeatTotal);
			}

			var M14 = new M14aImpl(new SSMTOOL(auxConfig.SSMInputs));
			return M14.AuxHeaterDemand(cycleTime, engineWasteHeatTotal);
		}

		public virtual void Initialise(IAuxiliaryConfig auxCfg)
		{
			Signals.CurrentCycleTimeInSeconds = 0;
			auxConfig = auxCfg; //new AuxiliaryConfig(auxPath);

			var compressorMap = auxConfig.PneumaticUserInputsConfig.CompressorMap;

			// SSM HVAC
			//var ssmPath = FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.HvacUserInputsConfig.SSMFilePath);
			//var BusDatabase = FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.HvacUserInputsConfig.BusDatabasePath);
			var ssmTool = auxConfig.SSMInputs is ISSMEngineeringInputs ?
				new SimpleSSMTool(auxConfig.SSMInputs)
				: (ISSMPowerDemand)new SSMTOOL(auxConfig.SSMInputs);


			M0 = new M00Impl(auxConfig.ElectricalUserInputsConfig, Signals, ssmTool.ElectricalWAdjusted);

			//M0_5 = new M0_5Impl(
			//	M0, auxConfig.ElectricalUserInputsConfig, Signals);

			M1 = new M01Impl(
				M0, auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency,
				auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency, ssmTool.ElectricalWAdjusted,
				ssmTool.MechanicalWBaseAdjusted);

			M2 = new M02Impl(M0, auxConfig.ElectricalUserInputsConfig, Signals);

			M3 = new M03Impl(auxConfig, compressorMap, auxCfg.Actuations, Signals);

			M4 = new M04Impl(
				compressorMap, auxConfig.PneumaticUserInputsConfig.CompressorGearRatio,
				auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency, Signals);

			//M5 = new M05Impl(
			//	M0_5, auxConfig.ElectricalUserInputsConfig.PowerNetVoltage,
			//	auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency);
			M5 = new M05Impl_P0(M0, M1, M2, ElectricStorage, auxCfg.ElectricalUserInputsConfig, Signals);
			M6 = new M06Impl(auxCfg.ElectricalUserInputsConfig, M1, M2, M3, M4, M5, Signals);
			M7 = new M07Impl(M0, M1, M2, M5, M6, ElectricStorage,
				auxCfg.ElectricalUserInputsConfig.AlternatorGearEfficiency, Signals);
			M8 = new M08Impl(auxConfig, M1, M6, M7, Signals);

			//M9 = new M09Impl(M1, M4, M6, M8, fuelMap, auxConfig.PneumaticAuxillariesConfig, Signals);
			//M10 = new M10Impl(M3, M9);
			//M11 = new M11Impl(M1, M3, M6, M8, fuelMap, Signals);
			//M12 = new M12Impl(M10, M11);
			//M13 = new M13Impl(auxCfg, M10, M11, M12, Signals);

			// This duplicate SSM is being created for use in M14 as its properties will be dynamically changed at that point
			// to honour EngineWaste Heat Usage in Fueling calculations.
			//var ssmToolModule14 = new SSMTOOL(auxCfg.SSMInputs);

			//M14 = new M14Impl(M13, ssmToolModule14, fuelProperties, Signals);
		}


		public ISignals Signals { get; set; }

		public virtual Watt ElectricPowerConsumer => M2.AveragePowerDemandAtAlternatorFromElectrics;

		public virtual Watt HVACElectricalPowerConsumer => M1.AveragePowerDemandAtAlternatorFromHVACElectrics;


		public virtual Watt ElectricPowerConsumerSum => M1.AveragePowerDemandAtAlternatorFromHVACElectrics + M2.AveragePowerDemandAtAlternatorFromElectrics;

		public virtual Watt ElectricPowerDemandMech
		{
			get {
				if (auxConfig.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart) {
					return auxConfig.PneumaticUserInputsConfig.SmartAirCompression
						? M7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
						: M7.SmartElectricalOnlyAuxAltPowerGenAtCrank;
				}

				return M2.GetAveragePowerAtCrankFromElectrics() + M1.AveragePowerDemandAtCrankFromHVACElectrics;
			}
		}

		public virtual Watt ElectricPowerGenerated
		{
			get {
				if (auxConfig.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart) {
					var retVal = auxConfig.PneumaticUserInputsConfig.SmartAirCompression
						? M7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
						: M7.SmartElectricalOnlyAuxAltPowerGenAtCrank;
					return retVal * M0.AlternatorsEfficiency * auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency;
				}

				return M1.AveragePowerDemandAtAlternatorFromHVACElectrics + M2.AveragePowerDemandAtAlternatorFromElectrics;
			}
		}

		public virtual NormLiter PSDemandConsumer => M3.AverageAirConsumed * Signals.SimulationInterval;

		public virtual NormLiter PSAirGenerated
		{
			get {
				if (auxConfig.PneumaticUserInputsConfig.SmartAirCompression && M6.OverrunFlag && Signals.ClutchEngaged &&
					!Signals.InNeutral) {
					if (M8.CompressorFlag) {
						return M4.GetFlowRate() *
								auxConfig.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction
								* Signals.SimulationInterval;
					}

					return 0.SI<NormLiter>();
				}

				return PSDemandConsumer;
			}
		}

		public virtual NormLiter PSAirGeneratedAlwaysOn => M4.GetFlowRate() * Signals.SimulationInterval;


		public virtual Watt PSPowerDemandAirGenerated
		{
			get {
				if (auxConfig.PneumaticUserInputsConfig.SmartAirCompression) {
					return auxConfig.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
						? M7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
						: M7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank;
				}

				return M3.GetAveragePowerDemandAtCrankFromPneumatics();
			}
		}

		public virtual Watt PSPowerCompressorAlwaysOn => M4.GetPowerCompressorOn();

		public virtual Watt PSPowerCompressorDragOnly => M4.GetPowerCompressorOff();

		public virtual Watt HVACMechanicalPowerConsumer => M1.AveragePowerDemandAtCrankFromHVACMechanicals;

		public virtual Watt HVACMechanicalPowerGenerated => M1.AveragePowerDemandAtCrankFromHVACMechanicals;


		//public string AuxiliaryName
		//{
		//	get { return "BusAuxiliaries"; }
		//}

		//public string AuxiliaryVersion
		//{
		//	get { return "Version 2.0 DEV"; }
		//}

		public virtual Watt AuxiliaryPowerAtCrankWatts => M8.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries;

		public WattSecond BatteryEnergyDemand(Second dt, double essFactor)
		{
			if (auxConfig.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart) {
				var generatedElPower =
					(auxConfig.PneumaticUserInputsConfig.SmartAirCompression
						? M7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
						: M7.SmartElectricalOnlyAuxAltPowerGenAtCrank) * M0.AlternatorsEfficiency *
					auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency;
				var energy = (generatedElPower - ElectricPowerConsumerSum) * essFactor * dt;
				return energy;
			}

			return 0.SI<WattSecond>();
		}

		public virtual void CycleStep(Second seconds) =>
			Signals.CurrentCycleTimeInSeconds += seconds.Value();

		public virtual void ResetCalculations()
		{
			var modules = new IAbstractModule[] { M0, M1, M2, M3, M4, M5, M6, M7, M8 };
			foreach (var module in modules) {
				module.ResetCalculations();
			}
		}
	}
}
