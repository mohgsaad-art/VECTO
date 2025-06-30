using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter
{
    public class EngineGroup : AbstractCIFGroupWriter
    {
		public EngineGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = GetVehicle(inputData);
			var engine = vehicle.Components.EngineInputData;

			var fuelTypesXElement = new XElement(_cif + XMLNames.Report_Vehicle_FuelTypes);

			var fuelTypes = new HashSet<FuelType>();
            foreach (var engineMode in engine.EngineModes)
            {
				foreach (var fuel in engineMode.Fuels) {
					fuelTypes.Add(fuel.FuelType);
				}
			}
			var sortedFuels = fuelTypes.ToList();
			sortedFuels.Sort((fuelType1, fuelType2) => fuelType1.CompareTo(fuelType2));

			sortedFuels.ForEach(type => fuelTypesXElement.Add(new XElement(_cif + XMLNames.Engine_FuelType, type.ToXMLFormat())));

			return new List<XElement>() {
				new XElement(_cif + "EngineRatedPower", engine.RatedPowerDeclared.ValueAsUnit("kW")),
				new XElement(_cif + "EngineCapacity", engine.Displacement.ValueAsUnit("ltr", 1)),
				fuelTypesXElement
			};
		}

		#endregion
	}


	public class TransmissionGroupWithGearbox : AbstractCIFGroupWriter, IAxlePowertrainReportOutputGroup
    {
		public TransmissionGroupWithGearbox(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		public IList<XElement> GetElements(IAxlePowertrainDeclarationInputData axlePt)
		{
            return GetElements(axlePt.GearboxInputData);
        }

        public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = GetVehicle(inputData);
			var gearbox = vehicle.Components.GearboxInputData;

			return GetElements(gearbox);
		}

		private IList<XElement> GetElements(IGearboxDeclarationInputData gearbox)
		{
            return new List<XElement>() {
                new XElement(_cif + "TransmissionValues", gearbox.CertificationMethod.ToXMLFormat()),
                new XElement(_cif + XMLNames.Gearbox_TransmissionType, gearbox.Type.ToXMLFormat()),
                new XElement(_cif + "NrOfGears", gearbox.Gears.Count)
            };
        }

    }

	public class TransmissionGroupWithoutGearbox : AbstractCIFGroupWriter
	{
		public TransmissionGroupWithoutGearbox(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			//var vehicle = GetVehicle(inputData);
			//var gearbox = vehicle.Components.GearboxInputData;
			return new List<XElement>() {
				new XElement(_cif + "NrOfGears", 1)
			};
		}

		#endregion
	}

	public class IEPCTransmissionGroup : AbstractCIFGroupWriter, IAxlePowertrainReportOutputGroup
    {
		public IEPCTransmissionGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		public IList<XElement> GetElements(IAxlePowertrainDeclarationInputData axlePt)
		{
            return new List<XElement>() {
                new XElement(_cif + "NrOfGears", axlePt.IEPCInputData.Gears.Count)
            };
        }

        public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
            var vehicle = GetVehicle(inputData);
            var iepc = vehicle.Components.IEPC;

            return new List<XElement>() {
				new XElement(_cif + "NrOfGears", iepc.Gears.Count)
			};
		}
	}

	public class FuelCellGroup : AbstractCIFGroupWriter
	{
        public FuelCellGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

        public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
        {
            var vehicle = GetVehicle(inputData);
			var power = vehicle.Components.FuelCellSystem.FuelCellModules.Sum(x => VectoMath.Min(x.FuelCell.FCSRatedPower, x.MaxPower) * x.Count);

            return new List<XElement>() {
                new XElement(_cif + "FuelCellTotalRatedPower", power.ValueAsUnit("kW"))
            };
        }
    }

	public class AxleWheelsGroup : AbstractCIFGroupWriter
	{
		public AxleWheelsGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = GetVehicle(inputData);
			var axleWheels = vehicle.Components.AxleWheels;
			double averageRRC = 0;
			var result = new List<XElement>();
			int axleCount = 0;
			int wheelsCount = 0;
			foreach (var axle in axleWheels.AxlesDeclaration) {
				var nbrWheels = axle.TwinTyres ? 4 : 2;
				wheelsCount += nbrWheels;
				averageRRC += axle.Tyre.RollResistanceCoefficient * nbrWheels;
				result.Add(new XElement(_cif + XMLNames.AxleWheels_Axles_Axle, 
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, ++axleCount),
					new XElement(_cif + XMLNames.Report_Tyre_TyreDimension, axle.Tyre.Dimension),
					new XElement(_cif + "FuelEfficiencyClass", axle.Tyre.FuelEfficiencyClass),
					new XElement(_cif + XMLNames.Report_Tyre_TyreCertificationNumber, axle.Tyre.CertificationNumber)));
			}
			averageRRC /= wheelsCount;
			result.Insert(0, new XElement(_cif + "AverageRRC", averageRRC.ToXMLFormat(4)));


			return result;
		}

		#endregion
	}

	public class LorryAuxGroup : AbstractCIFGroupWriter
	{
		public LorryAuxGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			return inputData.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries
				.Single(aux => aux.Type == AuxiliaryType.SteeringPump).Technology.Select(x =>
					new XElement(_cif + "SteeringPumpTechnology", x)).ToList();
			//return new List<XElement>() {
			//	new XElement(_cif + "SteeringPumpTechnology",
			//		inputData.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries
			//			.Single(aux => aux.Type == AuxiliaryType.SteeringPump).Technology.Join())
			//};
		}

		#endregion
	}

	public class ElectricMachineGroup : AbstractCIFGroupWriter, IAxlePowertrainReportOutputGroup
    {
		public ElectricMachineGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

        public IList<XElement> GetElements(IAxlePowertrainDeclarationInputData axlePt)
		{
            var architecture = axlePt.Architecture;
            var iepc = axlePt.IEPCInputData;
			var electricMotors = new List<ElectricMachineEntry<IElectricMotorDeclarationInputData>>() { axlePt.ElectricMotor };

            return GetElements(architecture, iepc, electricMotors);
        }

        public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
            var vehicle = GetVehicle(inputData);
            
			var architecture = vehicle.ArchitectureID;
            var iepc = vehicle.Components.IEPC;
            var electricMotors = vehicle.Components.ElectricMachines?.Entries;

			return GetElements(architecture, iepc, electricMotors);
		}

        private IList<XElement> GetElements(
			ArchitectureID architecture, 
			IIEPCDeclarationInputData iepc, 
			IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> electricMotors)
		{
            var result = new XElement(_cif + "ElectricMachineSystem");

            Watt totalRatedPropulsionPower = null;
            IList<IElectricMotorVoltageLevel> voltageLevels = new List<IElectricMotorVoltageLevel>();
            var count = 0;

            if (architecture.IsOneOf(ArchitectureID.S_IEPC, ArchitectureID.E_IEPC, ArchitectureID.F_IEPC))
            {
                count = iepc.DesignTypeWheelMotor && iepc.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;
                totalRatedPropulsionPower = iepc.TotalRatedPowerCalculated;
                voltageLevels = iepc.VoltageLevels.ToList();
            }
            else
            {
                var propulsionElectricMachines = electricMotors.Where(e => e.Position != PowertrainPosition.GEN);

                count = 1;
                totalRatedPropulsionPower = propulsionElectricMachines.Sum((e => e.ElectricMachine.R85RatedPower * e.Count));

                var groupedVoltageLevels = propulsionElectricMachines
                    .SelectMany(electricMachine => electricMachine.ElectricMachine.VoltageLevels).GroupBy((level => level.VoltageLevel));

                foreach (IGrouping<Volt, IElectricMotorVoltageLevel> electricMotorVoltageLevels in groupedVoltageLevels)
                {
                    voltageLevels.Add(electricMotorVoltageLevels.MaxBy(level => level.ContinuousTorqueSpeed * level.ContinuousTorque));
                }
            }

            result.Add(new XElement(_cif + "TotalRatedPropulsionPower", totalRatedPropulsionPower.ValueAsUnit("kW", 0)));

            var voltageLevelsXElement = new XElement(_cif + "VoltageLevels");
            result.Add(voltageLevelsXElement);

            foreach (var electricMotorVoltageLevel in voltageLevels)
            {
                var voltageLevel = new XElement(_cif + XMLNames.ElectricMachine_VoltageLevel,
                    voltageLevels.Count > 1
                        ? new XAttribute("voltage", electricMotorVoltageLevel.VoltageLevel.ToXMLFormat(0))
                        : null,
                    new XElement(_cif + "MaxContinuousPropulsionPower",
                        (electricMotorVoltageLevel.ContinuousTorque * electricMotorVoltageLevel.ContinuousTorqueSpeed * count)
                        .ValueAsUnit("kW", 0)));

                voltageLevelsXElement.Add(voltageLevel);
            }

            return new List<XElement>() { result };
        }

    }

	public class REESSGroup : AbstractCIFGroupWriter
	{
		public REESSGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = GetVehicle(inputData);
			var reess = vehicle.Components.ElectricStorage;
			
			var batTotalCap = 0.SI<WattSecond>();
			var batUsableCap = 0.SI<WattSecond>();
			if (reess.ElectricStorageElements.Any(x => x.REESSPack.StorageType == REESSType.Battery)) {
				var eletricStorageAdapter = new ElectricStorageAdapter();
				var batData = eletricStorageAdapter.CreateBatteryData(reess, vehicle.VehicleType, vehicle.OVC);
				batUsableCap = batData.UseableStoredEnergy;
				batTotalCap = batData.TotalStoredEnergy;
			}

			var capacitors = reess.ElectricStorageElements.Where(es => es.REESSPack.StorageType == REESSType.SuperCap)
				.Select(es => es.REESSPack as ISuperCapDeclarationInputData).ToArray();

			var totalStorageCapacity =
				batTotalCap +
				(capacitors.Length > 0 ? capacitors.Sum(cap => GetStorageCapacity(cap)) : 0.SI<WattSecond>());
			var usableCapacity = batUsableCap +
									(capacitors.Length > 0
										? capacitors.Sum(cap => GetTotalUsableCapacityInSimulation(cap))
										: 0.SI<WattSecond>());

			return new List<XElement>() {
				new XElement(_cif + "TotalStorageCapacity", totalStorageCapacity.ValueAsUnit("kWh", 0)),
				new XElement(_cif + "UsableStorageCapacity", usableCapacity.ValueAsUnit("kWh", 0))
			};
		}

		private WattSecond GetTotalUsableCapacityInSimulation(ISuperCapDeclarationInputData cap)
		{
			return GetStorageCapacity(cap) * 0.8;
		}

		private WattSecond GetStorageCapacity(ISuperCapDeclarationInputData cap)
		{
            var voltageRange = cap.MaxVoltage - cap.MinVoltage;
            //var avgVoltage = (cap.MaxVoltage + cap.MinVoltage) / 2.0;
			return cap.Capacity * voltageRange * voltageRange / 2.0;
		}

		#endregion
	}

	public class ConventionalCompletedBusAuxGroup : AbstractCIFGroupWriter
	{
		public ConventionalCompletedBusAuxGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multistage = inputData as IMultistepBusInputDataProvider;
			if (multistage == null) {
				throw new VectoException("BusAuxGroupWriter requires MultistepInputData");
			}

			var completedBusAux = multistage.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries;
			var primaryBusAux = multistage.JobInputData.PrimaryVehicle.Vehicle.Components.BusAuxiliaries;
			var retVal = new List<XElement>();
			retVal.Add(new XElement(_cif + XMLNames.BusAux_SteeringPump, GetSteeringPumpTech(completedBusAux, primaryBusAux)));
			retVal.Add(new XElement(_cif + XMLNames.BusAux_ElectricSystem, GetElectricSystem(completedBusAux, primaryBusAux)));
			retVal.Add(new XElement(_cif + XMLNames.BusAux_PneumaticSystem, GetPneumaticSystem(completedBusAux, primaryBusAux)));
			retVal.Add(new XElement(_cif + XMLNames.BusAux_HVAC, GetHVAC(completedBusAux, primaryBusAux)));
			return retVal;
			
		}

		protected virtual IList<XElement> GetPneumaticSystem(
			IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return new[] {
				new XElement(_cif + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem,
					primaryBusAux.PneumaticSupply.SmartAirCompression),
				new XElement(_cif + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem,
					primaryBusAux.PneumaticSupply.SmartRegeneration)
			};
		}

		protected virtual IList<XElement> GetElectricSystem(
			IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			var retVal = new List<XElement>() {
				new XElement(_cif + "AlternatorTechnology", primaryBusAux.ElectricSupply.AlternatorTechnology.ToXMLFormat())
			};
			if (primaryBusAux.ElectricSupply.AlternatorTechnology == AlternatorType.Smart) {
				retVal.Add(new XElement(_cif + "MaxAlternatorPower",
					primaryBusAux.ElectricSupply.Alternators.Sum(x => x.RatedCurrent * x.RatedVoltage).ValueAsUnit("kW", 0)));
				retVal.Add(new XElement(_cif + "ElectricStorageCapacity", 
					DeclarationData.BusAuxiliaries.CalculateBatteryCapacity(primaryBusAux.ElectricSupply.ElectricStorage).ValueAsUnit("kWh", 0)));
			}

			return retVal;
		}

		protected virtual IList<XElement> GetSteeringPumpTech(
			IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return primaryBusAux.SteeringPumpTechnology.Select(x => new XElement(_cif + XMLNames.BusAux_Technology, x))
				.ToArray();
		}

		protected virtual IList<XElement> GetHVAC(IBusAuxiliariesDeclarationData completedBusAux,
			IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return new[] {
				new XElement(_cif + XMLNames.Bus_SystemConfiguration,
					completedBusAux.HVACAux.SystemConfiguration.ToXmlFormat()),
				new XElement(_cif + "AuxiliaryHeaterPower",
					completedBusAux.HVACAux.AuxHeaterPower.ValueAsUnit("kW", 0)),
				new XElement(_cif + XMLNames.Bus_DoubleGlazing, completedBusAux.HVACAux.DoubleGlazing)
			};
		}

		#endregion
	}

	public class HEV_Px_IHPCompletedBusAuxGroup : ConventionalCompletedBusAuxGroup
	{
		public HEV_Px_IHPCompletedBusAuxGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }
	}

	public class HEV_Sx_CompletedBusAuxGroup : ConventionalCompletedBusAuxGroup
	{
		public HEV_Sx_CompletedBusAuxGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		protected override IList<XElement> GetPneumaticSystem(
			IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return new[] {
				//new XElement(_cif + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem,
				//	primaryBusAux.PneumaticSupply.SmartAirCompression),
				new XElement(_cif + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem,
					primaryBusAux.PneumaticSupply.SmartRegeneration)
			};
		}
	}

	public class PEVCompletedBusAuxGroup : ConventionalCompletedBusAuxGroup
	{
		public PEVCompletedBusAuxGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of ConventionalCompletedBusAuxGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multistage = inputData as IMultistepBusInputDataProvider;
			if (multistage == null) {
				throw new VectoException("BusAuxGroupWriter requires MultistepInputData");
			}

			var completedBusAux = multistage.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries;
			var primaryBusAux = multistage.JobInputData.PrimaryVehicle.Vehicle.Components.BusAuxiliaries;
			var retVal = new List<XElement>();
			retVal.Add(new XElement(_cif + XMLNames.BusAux_SteeringPump, GetSteeringPumpTech(completedBusAux, primaryBusAux)));
			//retVal.Add(new XElement(_cif + XMLNames.BusAux_ElectricSystem, GetElectricSystem(completedBusAux, primaryBusAux)));
			retVal.Add(new XElement(_cif + XMLNames.BusAux_PneumaticSystem, GetPneumaticSystem(completedBusAux, primaryBusAux)));
			retVal.Add(new XElement(_cif + XMLNames.BusAux_HVAC, GetHVAC(completedBusAux, primaryBusAux)));
			return retVal;

		}

		protected override IList<XElement> GetElectricSystem(IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return new XElement[] { };
		}

		protected override IList<XElement> GetPneumaticSystem(
			IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return new[] {
				//new XElement(_cif + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem,
				//	primaryBusAux.PneumaticSupply.SmartAirCompression),
				new XElement(_cif + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem,
					primaryBusAux.PneumaticSupply.SmartRegeneration)
			};
		}

		#endregion
	}


	[ExcludeFromCodeCoverage] // not used for official calculations - no reports needed
	public class ConventionalSingleBusAuxGroup : AbstractCIFGroupWriter
	{
		public ConventionalSingleBusAuxGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var singleBus = inputData as ISingleBusInputDataProvider;
			if (singleBus == null) {
				throw new VectoException("BusAuxGroupWriter requires SingleBusInputData");
			}

			var completedBusAux = singleBus.CompletedVehicle.Components.BusAuxiliaries;
			var primaryBusAux = singleBus.PrimaryVehicle.Components.BusAuxiliaries;
			var retVal = new List<XElement>();
			retVal.Add(new XElement(_cif + XMLNames.BusAux_SteeringPump, GetSteeringPumpTech(completedBusAux, primaryBusAux)));
			retVal.Add(new XElement(_cif + XMLNames.BusAux_ElectricSystem, GetElectricSystem(completedBusAux, primaryBusAux)));
			retVal.Add(new XElement(_cif + XMLNames.BusAux_PneumaticSystem, GetPneumaticSystem(completedBusAux, primaryBusAux)));
			retVal.Add(new XElement(_cif + XMLNames.BusAux_HVAC, GetHVAC(completedBusAux, primaryBusAux)));
			return retVal;

		}

		protected virtual IList<XElement> GetPneumaticSystem(
			IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return new[] {
				new XElement(_cif + XMLNames.BusAux_PneumaticSystem_SmartcompressionSystem,
					primaryBusAux.PneumaticSupply.SmartAirCompression),
				new XElement(_cif + XMLNames.BusAux_PneumaticSystem_SmartRegenerationSystem,
					primaryBusAux.PneumaticSupply.SmartRegeneration)
			};
		}

		protected virtual IList<XElement> GetElectricSystem(
			IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			var retVal = new List<XElement>() {
				new XElement(_cif + "AlternatorTechnology", primaryBusAux.ElectricSupply.AlternatorTechnology.ToXMLFormat())
			};
			if (primaryBusAux.ElectricSupply.AlternatorTechnology == AlternatorType.Smart) {
				retVal.Add(new XElement(_cif + "MaxAlternatorPower",
					primaryBusAux.ElectricSupply.Alternators.Sum(x => x.RatedCurrent * x.RatedVoltage).ValueAsUnit("kW", 0)));
				retVal.Add(new XElement(_cif + "ElectricStorageCapacity",
					DeclarationData.BusAuxiliaries.CalculateBatteryCapacity(primaryBusAux.ElectricSupply.ElectricStorage).ValueAsUnit("kWh", 0)));
			}

			return retVal;
		}

		protected virtual IList<XElement> GetSteeringPumpTech(
			IBusAuxiliariesDeclarationData completedBusAux, IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return primaryBusAux.SteeringPumpTechnology.Select(x => new XElement(_cif + XMLNames.BusAux_Technology, x))
				.ToArray();
		}

		protected virtual IList<XElement> GetHVAC(IBusAuxiliariesDeclarationData completedBusAux,
			IBusAuxiliariesDeclarationData primaryBusAux)
		{
			return new[] {
				new XElement(_cif + XMLNames.Bus_SystemConfiguration,
					completedBusAux.HVACAux.SystemConfiguration.ToXmlFormat()),
				new XElement(_cif + "AuxiliaryHeaterPower",
					completedBusAux.HVACAux.AuxHeaterPower.ValueAsUnit("kW", 0)),
				new XElement(_cif + XMLNames.Bus_DoubleGlazing, completedBusAux.HVACAux.DoubleGlazing)
			};
		}

		#endregion
	}

}
