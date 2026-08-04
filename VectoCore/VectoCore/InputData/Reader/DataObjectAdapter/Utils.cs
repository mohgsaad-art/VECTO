using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
    public static class Utils
    {
        public static void CrossValidatePowertrains(IVehicleDeclarationInputData vehicle, IList<AxlePowertrainData> axlePts)
        {
            CrossValidatePowertrainsAxleNumbers(vehicle, axlePts);
            CrossValidatePowertrainsArchitecture(vehicle, axlePts);
            CrossValidatePowertrainsElectricMotorTorqueLimits(vehicle, axlePts);
        }

        private static void CrossValidatePowertrainsArchitecture(IVehicleDeclarationInputData vehicle, IList<AxlePowertrainData> axlePts)
        {
            var axlePtArchs = axlePts.Select(x => x.Architecture).ToList();
            
            if (!axlePtArchs.Any(x => x == vehicle.ArchitectureID))
            {
                throw new VectoException($"ArchitectureID ({vehicle.ArchitectureID}) does not match any axle powertrain architecture.");
            }
            else
            {
                axlePtArchs.Remove(vehicle.ArchitectureID);
            }

            if (!axlePtArchs.Any(x => x == vehicle.ArchitectureIDPwt2))
            {
                throw new VectoException($"ArchitectureIDPwt2 ({vehicle.ArchitectureIDPwt2}) does not match any axle powertrain architecture (ArchitectureID has already been matched).");
            }
        }

        private static void CrossValidatePowertrainsAxleNumbers(IVehicleDeclarationInputData vehicle, IList<AxlePowertrainData> axlePts)
        {
            if (axlePts.Select(x => x.AxleNumber).Distinct().Count() < axlePts.Count)
            {
                throw new VectoException($"Some axle powertrains have the same axleNumber.");
            }

            var axleNumbers = vehicle.Components.AxleWheels.AxlesDeclaration.Select(x => x.AxleNumber).ToList();

            foreach (var item in axlePts)
            {
                if (!axleNumbers.Contains(item.AxleNumber))
                {
                    throw new VectoException($"Powertrain axleNumber ({item.AxleNumber}) is not found among the Axles of AxleWheels.");
                }
            }
        }

        public static void ValidatePowertrainEMPosition(PowertrainPosition position, IAxlePowertrainDeclarationInputData axlePtData)
        {
            if (axlePtData.Architecture.IsIEPC())
            {
                return;
            }

            if (position != PowertrainPositionHelper.Parse(axlePtData.Architecture.ToString()))
            {
                throw new VectoException($"ElectricMachine has mismatching PowertrainPosition ({position}) in powertrain with axleNumber: {axlePtData.AxleNumber}.");
            }
        }

        private static void CrossValidatePowertrainsElectricMotorTorqueLimits(IVehicleDeclarationInputData vehicle, IList<AxlePowertrainData> axlePts)
        {
            if (vehicle.ElectricMotorTorqueLimits == null)
            {
                return;
            }

            var emPlacements = axlePts.Select(x => new EMPlacement(x.ElectricMachineData.Item1, x.AxleNumber)).ToList();

            foreach (var item in vehicle.ElectricMotorTorqueLimits)
            {
                if (!emPlacements.Any(x => (x.AxleNumber == item.Key.AxleNumber) && (x.Position == item.Key.Position)))
                {
                    throw new VectoException(
                        $"ElectricMachine in ElectricMotorTorqueLimits with axleNumber: {item.Key.AxleNumber}, Position: {item.Key.Position}, " +
                        "cannot be found among the axle powertrains or it is a duplicate.");
                }
                else
                {
                    emPlacements.RemoveAll(x => (x.AxleNumber == item.Key.AxleNumber) && (x.Position == item.Key.Position));
                }
            }
        }
    }
}
