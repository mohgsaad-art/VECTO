using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter
{
    public class GeneralVehicleOutputSequenceGroupCif : AbstractCIFGroupWriter, IReportVehicleOutputGroup, IReportCompletedBusOutputGroup
	{
		public GeneralVehicleOutputSequenceGroupCif(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			return GetElements(inputData.JobInputData.Vehicle);
		}

		#endregion

		#region Implementation of IReportVehicleOutputGroup

		public IList<XElement> GetElements(IVehicleDeclarationInputData vehicleData)
		{
			var result = new List<XElement>() {
				new XElement(_cif + XMLNames.Vehicle_VIN, vehicleData.VIN),
				new XElement(_cif + XMLNames.Vehicle_VehicleCategory, vehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_AxleConfiguration, vehicleData.AxleConfiguration.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_TPMLM, XMLHelper.ValueAsUnit(vehicleData.GrossVehicleMassRating, "kg")),
                new XElement(_cif + "TotalPropulsionPower", XMLHelper.ValueAsUnit(DeclarationData.GetReferencePropulsionPower(vehicleData), "kW")),
                new XElement(_cif + XMLNames.Report_Vehicle_VehicleGroup, DeclarationData.GetVehicleGroupGroup(vehicleData).Item1.GetClassNumber()),
                new XElement(_cif + XMLNames.VehicleGroupCO2, string.Empty), //the value of this element will be replaced later, write empty string to let the validation fail if the value is not replaced
				//new XElement(_cif + XMLNames.VehicleGroupCO2, DeclarationData.GetVehicleGroupCO2StandardsGroup(vehicleData).ToXMLFormat()),

			};
			return result;
		}

		#endregion

		#region Implementation of IReportCompletedBusOutputGroup

		public IList<XElement> GetElements(IMultistepBusInputDataProvider multiStageInputDataProvider)
		{
			var consolidatedVehicleData = multiStageInputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle;
			var primary = multiStageInputDataProvider.JobInputData.PrimaryVehicle.Vehicle;
			var result = new List<XElement>() {
				new XElement(_cif + XMLNames.Vehicle_VIN, consolidatedVehicleData.VIN),
				new XElement(_cif + XMLNames.Vehicle_VehicleCategory, consolidatedVehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_AxleConfiguration, primary.AxleConfiguration.ToXMLFormat()),
				new XElement(_cif + XMLNames.Vehicle_TPMLM, XMLHelper.ValueAsUnit(consolidatedVehicleData.GrossVehicleMassRating, "kg")),
                new XElement(_cif + "TotalPropulsionPower", XMLHelper.ValueAsUnit(DeclarationData.GetReferencePropulsionPower(primary), "kW")),
                new XElement(_cif + XMLNames.Report_Vehicle_VehicleGroup, DeclarationData.GetVehicleGroupGroup(consolidatedVehicleData).Item1.GetClassNumber()),
                new XElement(_cif + XMLNames.VehicleGroupCO2, DeclarationData.GetVehicleGroupCO2StandardsGroup(multiStageInputDataProvider).ToXMLFormat()),
			};
			return result;
		}

		#endregion
	}


	public class LorryGeneralVehicleSequenceGroupCIF : AbstractCIFGroupWriter
	{
		public LorryGeneralVehicleSequenceGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var vehicleGroup = DeclarationData.GetVehicleGroupGroup(vehicleData);
			var result = new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Manufacturer, vehicleData.Manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, vehicleData.ManufacturerAddress),
				new XElement(_cif + XMLNames.Component_Model, vehicleData.Model),
				!vehicleData.VehicleTypeApprovalNumber.IsNullOrEmpty()
					? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, vehicleData.VehicleTypeApprovalNumber)
					: null,
				//new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, !vehicleData.VehicleTypeApprovalNumber.IsNullOrEmpty()
				//	? vehicleData.VehicleTypeApprovalNumber
				//	: null),
				new XElement(_cif + XMLNames.CorrectedActualMass, vehicleData.CurbMassChassis.ValueAsUnit("kg")),
				new XElement(_cif + XMLNames.Vehicle_SleeperCab, vehicleData.SleeperCab),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicleData.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_HybridElectricHDV, vehicleData.HybridElectricHDV)
			};
			return result;
		}

		#endregion
	}

	public class ConventionalLorryVehicleSequenceGroupCIF : AbstractCIFGroupWriter
	{
		public ConventionalLorryVehicleSequenceGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var dualFuel = vehicleData.Components.EngineInputData.EngineModes.Any(x => x.Fuels.Count > 1);
			return new List<XElement>() {
				new XElement(_cif + "WasteHeatRecovery",
					vehicleData.Components.EngineInputData.WHRType != WHRType.None),
				new XElement(_cif + XMLNames.Vehicle_DualFuelVehicle, dualFuel)
			};
		}

		#endregion
	}

	public class FuelCellLorryVehicleSequenceGroupCIF : AbstractCIFGroupWriter
	{
		public FuelCellLorryVehicleSequenceGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			return new List<XElement>()
			{
				// todo amogoda: 2.9 - fill fc commmon components data
			};
		}

		#endregion
	}

	public class HEV_LorryVehicleTypeGroupCIF : AbstractCIFGroupWriter
	{
		public HEV_LorryVehicleTypeGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(inputData.JobInputData.Vehicle));
			result.AddRange(_cifFactory.GetLorryGeneralVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(_cifFactory.GetHEV_LorryVehicleSequenceGroupWriter().GetElements(inputData));

			return result;
		}

		#endregion
	}

	public class FuelCell_LorryVehicleTypeGroupCIF : AbstractCIFGroupWriter
	{
		public FuelCell_LorryVehicleTypeGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			// todo amogoda: 2.5. check and fix inconsistencies
			var result = new List<XElement>();
			result.AddRange(_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(inputData.JobInputData.Vehicle));
			result.AddRange(_cifFactory.GetLorryGeneralVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(_cifFactory.GetFuelCell_LorryVehicleSequenceGroupWriter().GetElements(inputData));

			return result;
		}

		#endregion
	}

	public class PEV_LorryVehicleTypeGroupCIF : AbstractCIFGroupWriter
	{
		public PEV_LorryVehicleTypeGroupCIF(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			result.AddRange(_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(inputData.JobInputData.Vehicle));
			result.AddRange(_cifFactory.GetLorryGeneralVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(_cifFactory.GetPEV_LorryVehicleSequenceGroupWriter().GetElements(inputData));

			return result;
		}

		#endregion
	}


	public class HEV_LorryVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public HEV_LorryVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			var ovCc = inputData.JobInputData.Vehicle.OVC;
			result.AddRange(_cifFactory.GetConventionalLorryVehicleSequenceGroupWriter().GetElements(inputData));
			var vehicleData = inputData.JobInputData.Vehicle;
			var ihpc = vehicleData.Components?.GearboxInputData?.Type == GearboxType.IHPC;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "HEVArchitecture", ihpc ? GearboxType.IHPC.ToXMLFormat() : vehicleData.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc),
				new XElement(_cif + "DynamicChargingTechnology", vehicleData.DynamicChargingTechnology.ToXMLFormat())
			});

			return result;
		}

		#endregion
	}

	public class FCHV_LorryVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public FCHV_LorryVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			var ovCc = inputData.JobInputData.Vehicle.OVC;
            var vehicleData = inputData.JobInputData.Vehicle;

            result.AddRange(_cifFactory.GetFCHVLorryVehicleSequenceGroupWriter().GetElements(inputData));
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "FCHVArchitecture", vehicleData.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc),
                new XElement(_cif + "DynamicChargingTechnology", vehicleData.DynamicChargingTechnology.ToXMLFormat())
            });

			return result;
		}
	}


	public class PEV_LorryVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public PEV_LorryVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			var vehicle = GetVehicle(inputData);
			var ovCc = vehicle.OVC;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "PEVArchitecture", vehicle.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc),
                new XElement(_cif + "DynamicChargingTechnology", vehicle.DynamicChargingTechnology.ToXMLFormat())
            });
			return result;
		}

		#endregion
	}


	public class HEV_CompletedBusVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public HEV_CompletedBusVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multistep = inputData as IMultistepBusInputDataProvider;
			if (multistep == null) {
				throw new VectoException("Completed Bus CIF requires bus input data");
			}
			var result = new List<XElement>();
			var ovCc = multistep.JobInputData.PrimaryVehicle.Vehicle.OVC;
			//result.AddRange(_cifFactory.GetConventionalCompletedBusVehicleSequenceGroupWriter().GetElements(inputData));
			var vehicleData = multistep.JobInputData.PrimaryVehicle.Vehicle;
			var ihpc = vehicleData.Components?.GearboxInputData?.Type == GearboxType.IHPC;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "HEVArchitecture", ihpc ? GearboxType.IHPC.ToXMLFormat() : multistep.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc),
				new XElement(_cif + "DynamicChargingTechnology", vehicleData.DynamicChargingTechnology.ToXMLFormat())
			});

			return result;
		}

		#endregion
	}


	public class PEV_CompletedBusVehicleSequenceGroupWriter : AbstractCIFGroupWriter
	{
		public PEV_CompletedBusVehicleSequenceGroupWriter(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multistep = inputData as IMultistepBusInputDataProvider;
			if (multistep == null) {
				throw new VectoException("Completed Bus CIF requires bus input data");
			}
			var result = new List<XElement>();
			var ovCc = multistep.JobInputData.PrimaryVehicle.Vehicle.OVC;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + "PEVArchitecture",  multistep.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID.GetLabel()),
				new XElement(_cif + "OffVehicleChargingCapability", ovCc),
                new XElement(_cif + "DynamicChargingTechnology", multistep.JobInputData.PrimaryVehicle.Vehicle.DynamicChargingTechnology.ToXMLFormat())
            });
			return result;
		}

		#endregion
	}

	public class CompletedBusVehicleTypeGroup : AbstractCIFGroupWriter
	{
		private XElement GetManufacturerAndAddress(string manufacturer, string manufacturerAddress, int stepCount)
		{
			return new XElement(_cif + "Step",
				new XAttribute(XMLNames.ManufacturingStep_StepCount, stepCount),
				new XElement(_cif + XMLNames.Component_Manufacturer, manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, manufacturerAddress));
		}
		public CompletedBusVehicleTypeGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var completedBusData = inputData as IMultistepBusInputDataProvider;
			if (completedBusData == null) {
				throw new ArgumentException(
					$"{nameof(inputData)} must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			var result = new List<XElement>();
			result.AddRange(
				_cifFactory.GetGeneralVehicleSequenceGroupWriterCompletedBus().GetElements(completedBusData));
			result.Add(GetManufacturers(completedBusData));



			var consolidatedVehicle = completedBusData.JobInputData.ConsolidateManufacturingStage.Vehicle;
			var dualFuel = completedBusData.JobInputData.PrimaryVehicle.Vehicle.Components.EngineInputData.EngineModes.Any(x => x.Fuels.Count > 1);

			result.AddRange(new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Model, consolidatedVehicle.Model),
				new XElement(_cif + XMLNames.CorrectedActualMass, consolidatedVehicle.CurbMassChassis.ValueAsUnit("kg", 0)),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, consolidatedVehicle.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_HybridElectricHDV, consolidatedVehicle.HybridElectricHDV),
				new XElement(_cif + "WasteHeatRecovery", completedBusData.JobInputData.PrimaryVehicle.Vehicle.Components.EngineInputData.WHRType != WHRType.None),
				new XElement(_cif + XMLNames.Vehicle_DualFuelVehicle, dualFuel),
				new XElement(_cif + XMLNames.Vehicle_RegisteredClass, consolidatedVehicle.RegisteredClass.ToXMLFormat()),
				new XElement(_cif + "TotalNumberOfPassengers", consolidatedVehicle.NumberPassengerSeatsLowerDeck
																+ consolidatedVehicle.NumberPassengerSeatsUpperDeck
																+ consolidatedVehicle.NumberPassengersStandingLowerDeck
																+ consolidatedVehicle.NumberPassengersStandingUpperDeck),
				!consolidatedVehicle.VehicleTypeApprovalNumber.IsNullOrEmpty() ? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, consolidatedVehicle.VehicleTypeApprovalNumber) : null
			});

			return result;
		}

		protected XElement GetManufacturers(IMultistepBusInputDataProvider completedBusData)
		{
			var manufacturers = new XElement(_cif + "Manufacturers",
				GetManufacturerAndAddress(completedBusData.JobInputData.PrimaryVehicle.Vehicle.Manufacturer,
					completedBusData.JobInputData.PrimaryVehicle.Vehicle.ManufacturerAddress, 1));
			foreach (var step in completedBusData.JobInputData.ManufacturingStages) {
				manufacturers.Add(GetManufacturerAndAddress(step.Vehicle.Manufacturer, step.Vehicle.ManufacturerAddress,
					step.StepCount));
			}

			return manufacturers;
		}

		#endregion
	}

	public class PEVCompletedBusVehicleTypeGroup : AbstractCIFGroupWriter
	{
		private XElement GetManufacturerAndAddress(string manufacturer, string manufacturerAddress, int stepCount)
		{
			return new XElement(_cif + "Step",
				new XAttribute(XMLNames.ManufacturingStep_StepCount, stepCount),
				new XElement(_cif + XMLNames.Component_Manufacturer, manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, manufacturerAddress));
		}
		public PEVCompletedBusVehicleTypeGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var completedBusData = inputData as IMultistepBusInputDataProvider;
			if (completedBusData == null) {
				throw new ArgumentException(
					$"{nameof(inputData)} must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			var result = new List<XElement>();
			result.AddRange(
				_cifFactory.GetGeneralVehicleSequenceGroupWriterCompletedBus().GetElements(completedBusData));
			result.Add(GetManufacturers(completedBusData));



			var consolidatedVehicle = completedBusData.JobInputData.ConsolidateManufacturingStage.Vehicle;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Model, consolidatedVehicle.Model),
				new XElement(_cif + XMLNames.CorrectedActualMass, consolidatedVehicle.CurbMassChassis.ValueAsUnit("kg", 0)),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, consolidatedVehicle.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_HybridElectricHDV, consolidatedVehicle.HybridElectricHDV),
				new XElement(_cif + XMLNames.Vehicle_RegisteredClass, consolidatedVehicle.RegisteredClass.ToXMLFormat()),
				new XElement(_cif + "TotalNumberOfPassengers", consolidatedVehicle.NumberPassengerSeatsLowerDeck
																+ consolidatedVehicle.NumberPassengerSeatsUpperDeck
																+ consolidatedVehicle.NumberPassengersStandingLowerDeck
																+ consolidatedVehicle.NumberPassengersStandingUpperDeck),
				!consolidatedVehicle.VehicleTypeApprovalNumber.IsNullOrEmpty() ? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, consolidatedVehicle.VehicleTypeApprovalNumber) : null
			});

			return result;
		}

		private XElement GetManufacturers(IMultistepBusInputDataProvider completedBusData)
		{
			var manufacturers = new XElement(_cif + "Manufacturers",
				GetManufacturerAndAddress(completedBusData.JobInputData.PrimaryVehicle.Vehicle.Manufacturer,
					completedBusData.JobInputData.PrimaryVehicle.Vehicle.ManufacturerAddress, 1));
			foreach (var step in completedBusData.JobInputData.ManufacturingStages) {
				manufacturers.Add(GetManufacturerAndAddress(step.Vehicle.Manufacturer, step.Vehicle.ManufacturerAddress,
					step.StepCount));
			}

			return manufacturers;
		}

		#endregion
	}


	public class ExemptedCompletedBusVehicleTypeGroup : CompletedBusVehicleTypeGroup
	{
		public ExemptedCompletedBusVehicleTypeGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var completedBusData = inputData as IMultistepBusInputDataProvider;
			if (completedBusData == null) {
				throw new ArgumentException(
					$"{nameof(inputData)} must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			var result = new List<XElement>();
			result.AddRange(
				_cifFactory.GetGeneralVehicleSequenceGroupWriterCompletedBus().GetElements(completedBusData));
			result.Add(GetManufacturers(completedBusData));



			var consolidatedVehicle = completedBusData.JobInputData.ConsolidateManufacturingStage.Vehicle;
			result.AddRange(new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Model, consolidatedVehicle.Model),
				new XElement(_cif + XMLNames.CorrectedActualMass, consolidatedVehicle.CurbMassChassis.ValueAsUnit("kg", 0)),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, consolidatedVehicle.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_RegisteredClass, consolidatedVehicle.RegisteredClass.ToXMLFormat()),
				new XElement(_cif + "TotalNumberOfPassengers", consolidatedVehicle.NumberPassengerSeatsLowerDeck
																+ consolidatedVehicle.NumberPassengerSeatsUpperDeck
																+ consolidatedVehicle.NumberPassengersStandingLowerDeck
																+ consolidatedVehicle.NumberPassengersStandingUpperDeck),
				!consolidatedVehicle.VehicleTypeApprovalNumber.IsNullOrEmpty() ? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, consolidatedVehicle.VehicleTypeApprovalNumber) : null
			});

			return result;
		}

	}

	[ExcludeFromCodeCoverage] // not used for official calculations - no reports needed
	public class SingleBusVehicleTypeGroup : AbstractCIFGroupWriter
	{
		private XElement GetManufacturerAndAddress(string manufacturer, string manufacturerAddress, int stepCount)
		{
			return new XElement(_cif + "Step",
				new XAttribute(XMLNames.ManufacturingStep_StepCount, stepCount),
				new XElement(_cif + XMLNames.Component_Manufacturer, manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, manufacturerAddress));
		}
		public SingleBusVehicleTypeGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var singleBusData = inputData as ISingleBusInputDataProvider;
			if (singleBusData == null) {
				throw new ArgumentException(
					$"{nameof(inputData)} must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			var result = new List<XElement>();
			result.AddRange(
				_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(singleBusData.JobInputData.Vehicle));
			result.Add(GetManufacturers(singleBusData));



			var primaryVehicle = singleBusData.PrimaryVehicle;
			var completedVehicle = singleBusData.CompletedVehicle;
			var dualFuel = singleBusData.JobInputData.Vehicle.Components.EngineInputData.EngineModes.Any(x => x.Fuels.Count > 1);

			result.AddRange(new List<XElement>() {
				new XElement(_cif + XMLNames.Component_Model, primaryVehicle.Model),
				new XElement(_cif + XMLNames.CorrectedActualMass, completedVehicle.CurbMassChassis.ValueAsUnit("kg", 0)),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, primaryVehicle.ZeroEmissionVehicle),
				new XElement(_cif + XMLNames.Vehicle_HybridElectricHDV, primaryVehicle.HybridElectricHDV),
				new XElement(_cif + "WasteHeatRecovery", singleBusData.JobInputData.Vehicle.Components.EngineInputData.WHRType != WHRType.None),
				new XElement(_cif + XMLNames.Vehicle_DualFuelVehicle, dualFuel),
				new XElement(_cif + XMLNames.Vehicle_RegisteredClass, primaryVehicle.RegisteredClass.ToXMLFormat()),
				new XElement(_cif + "TotalNumberOfPassengers", primaryVehicle.NumberPassengerSeatsLowerDeck
																+ primaryVehicle.NumberPassengerSeatsUpperDeck
																+ primaryVehicle.NumberPassengersStandingLowerDeck
																+ primaryVehicle.NumberPassengersStandingUpperDeck),
				!primaryVehicle.VehicleTypeApprovalNumber.IsNullOrEmpty() ? new XElement(_cif + XMLNames.VehicleTypeApprovalNumber, primaryVehicle.VehicleTypeApprovalNumber) : null
			});

			return result;
		}

		protected XElement GetManufacturers(ISingleBusInputDataProvider completedBusData)
		{
			var manufacturers = new XElement(_cif + "Manufacturers",
				GetManufacturerAndAddress(completedBusData.JobInputData.Vehicle.Manufacturer,
					completedBusData.JobInputData.Vehicle.ManufacturerAddress, 1));
			//foreach (var step in completedBusData.JobInputData.ManufacturingStages) {
			//	manufacturers.Add(GetManufacturerAndAddress(step.Vehicle.Manufacturer, step.Vehicle.ManufacturerAddress,
			//		step.StepCount));
			//}

			return manufacturers;
		}

		#endregion
	}
}
