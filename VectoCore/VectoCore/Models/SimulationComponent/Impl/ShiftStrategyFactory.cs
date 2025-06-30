using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

    public class ShiftStrategyFactory : IShiftStrategyFactory
    {
        protected readonly IInternalShiftStrategyFactory _internalFactory;

        public ShiftStrategyFactory(IInternalShiftStrategyFactory internalFactory)
        {
            _internalFactory = internalFactory;
        }

        public string GetShiftStrategyName(GearboxType gearboxType, VectoSimulationJobType jobType)
        {
            switch (gearboxType) {
                case GearboxType.AMT:
                    switch (jobType) {
                        case VectoSimulationJobType.ConventionalVehicle:
                        case VectoSimulationJobType.ParallelHybridVehicle:
                            return AMTShiftStrategyOptimized.Name;
                        case VectoSimulationJobType.BatteryElectricVehicle:
                        case VectoSimulationJobType.SerialHybridVehicle:
                        case VectoSimulationJobType.FCHV:
							return PEVAMTShiftStrategy.Name;
                        default:
                            throw new VectoException(
                                "no default gearshift strategy available for gearbox type {0} and job type {1}",
                                gearboxType, jobType);
                    }
                case GearboxType.MT:
                    return MTShiftStrategy.Name;

                case GearboxType.ATPowerSplit:
                case GearboxType.ATSerial:
                    switch (jobType) {
                        case VectoSimulationJobType.ParallelHybridVehicle:
                        case VectoSimulationJobType.ConventionalVehicle:
                            return ATShiftStrategyOptimized.Name;
                        case VectoSimulationJobType.SerialHybridVehicle:
                        case VectoSimulationJobType.BatteryElectricVehicle:
                        case VectoSimulationJobType.FCHV:
                        case VectoSimulationJobType.FCHV_IEPC:
                            return APTNShiftStrategy.Name;
                        default:
                            throw new VectoException(
                                "no default gearshift strategy available for gearbox type {0} and job type {1}",
                                gearboxType, jobType);
                    }
                case GearboxType.APTN:
                    switch (jobType) {
                        case VectoSimulationJobType.ParallelHybridVehicle:
                        case VectoSimulationJobType.SerialHybridVehicle:
                        case VectoSimulationJobType.BatteryElectricVehicle:
                        case VectoSimulationJobType.IEPC_E:
                        case VectoSimulationJobType.IEPC_S:
                        case VectoSimulationJobType.FCHV:
                        case VectoSimulationJobType.FCHV_IEPC:
                            return APTNShiftStrategy.Name;
                        //case VectoSimulationJobType.ConventionalVehicle when isTestPowerTrain:
                        //    return null;
                        default:
                            throw new ArgumentException(
                                "APT-N Gearbox is only applicable on hybrids and battery electric vehicles.");
                    }
                case GearboxType.IHPC:
                    switch (jobType) {
                        case VectoSimulationJobType.IHPC:
                            return AMTShiftStrategyOptimized.Name;
                        default:
                            throw new ArgumentException(
                                "IHPC Gearbox is only applicable on hybrid vehicle of type IHPC.");
                    }
                default:
                    throw new ArgumentOutOfRangeException("GearboxType", gearboxType,
                        "VECTO can not automatically derive shift strategy for GearboxType.");
            }
        }

        public IShiftStrategy GetShiftStrategy(string name, IVehicleContainer container)
        {
            try {
                return _internalFactory.CreateShiftStrategy(name, container);
            } catch (Exception ex) {
                throw new ArgumentOutOfRangeException($@"Could not create shift strategy {name}", ex);
            }
        }

        public IShiftPolygonCalculator CreateShiftPolygonCalculator(string shiftStrategyName,
            ShiftStrategyParameters shiftParams)
        {
            try {
                return _internalFactory.CreateShiftPolygonCalculator(shiftStrategyName, shiftParams);
            } catch (Exception ex) {
                throw new ArgumentOutOfRangeException(
                    $@"Could not create shift polygon calculator for shift strategy {shiftStrategyName}", ex);
            }
        }

    }
}