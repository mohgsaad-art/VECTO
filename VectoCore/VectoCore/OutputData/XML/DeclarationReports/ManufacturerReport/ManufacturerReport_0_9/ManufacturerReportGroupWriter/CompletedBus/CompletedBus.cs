using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.Vehicle.CompletedBus
{
    internal class ConventionalCompletedBusGeneralVehicleOutputGroup : AbstractReportOutputGroup
    {
		
		public ConventionalCompletedBusGeneralVehicleOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var multiStageInputData = inputData as IMultistepBusInputDataProvider;
			if (multiStageInputData == null) {
				throw new ArgumentException($"inputdata must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			var consolidatedVehicleData = multiStageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle;
			var result = new List<XElement>();
			result.Add(GetManufacturers(multiStageInputData));
			result.AddRange(_mrfFactory.GetGeneralVehicleOutputGroup().GetElements(multiStageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle));
            result.AddRange(_mrfFactory.GetCompletedBusSequenceGroup().GetElements(consolidatedVehicleData));
			result.AddRange(_mrfFactory.GetCompletedBusDimensionSequenceGroup().GetElements(consolidatedVehicleData));
			result.Add(new XElement(_mrf + XMLNames.Bus_DoorDriveTechnology, consolidatedVehicleData.DoorDriveTechnology.ToXMLFormat()));
			result.Add(GetTankSystem(multiStageInputData));
			return result;
		}

		protected virtual XElement GetTankSystem(IMultistepBusInputDataProvider multiStageInputData)
		{
			var vehicle = multiStageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle;

            var tankSystem = vehicle.TankSystem.HasValue
                ? vehicle.TankSystem.Value.ToString()
                : (vehicle.HydrogenStorageTechnology.HasValue
                    ? vehicle.HydrogenStorageTechnology.Value.ToString()
                    : null);

            if (tankSystem == null) {
				return null;
			}

			return new XElement(_mrf + "TankSystem", tankSystem);
		}

		protected virtual XElement GetManufacturers(IMultistepBusInputDataProvider multiStageInputData)
		{
			var primaryVehicleData = multiStageInputData.JobInputData.PrimaryVehicle.Vehicle;
			var manufacturers = new XElement(_mrf + "Manufacturers");
			manufacturers.Add(GetManufacturerAndAddress(primaryVehicleData.Manufacturer, primaryVehicleData.ManufacturerAddress,
				1));
			foreach (var manufacturingStageInputData in multiStageInputData.JobInputData.ManufacturingStages) {
				manufacturers.Add(GetManufacturerAndAddress(manufacturingStageInputData.Vehicle.Manufacturer,
					manufacturingStageInputData.Vehicle.ManufacturerAddress,
					stepCount: manufacturingStageInputData.StepCount));
			}

			return manufacturers;
		}

		protected XElement GetManufacturerAndAddress(string manufacturer, string address, int stepCount)
		{
			return new XElement(_mrf + "Step",
				new XAttribute("Count", stepCount),
				new XElement(_mrf + XMLNames.Component_Manufacturer, manufacturer),
				new XElement(_mrf + XMLNames.Component_ManufacturerAddress, address));
		}

		#endregion
	}

	internal class HEVCompletedBusGeneralVehicleOutputGroup : ConventionalCompletedBusGeneralVehicleOutputGroup
	{

		public HEVCompletedBusGeneralVehicleOutputGroup(IManufacturerReportFactory mrfFactory) :
			base(mrfFactory) { }

	}



	internal class PEVCompletedBusGeneralVehicleOutputGroup : ConventionalCompletedBusGeneralVehicleOutputGroup
	{

		public PEVCompletedBusGeneralVehicleOutputGroup(IManufacturerReportFactory mrfFactory) :
			base(mrfFactory)
		{ }

		#region Overrides of ConventionalCompletedBusGeneralVehicleOutputGroup

		protected override XElement GetTankSystem(IMultistepBusInputDataProvider multiStageInputData)
		{
			return null;
		}

		#endregion
	}

	internal class CompletedBusSequenceOutputGroup : AbstractReportOutputGroup, IReportVehicleOutputGroup
	{
		public CompletedBusSequenceOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		[ExcludeFromCodeCoverage] // never called for completed bus
		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMrfVehicleGroup

		public IList<XElement> GetElements(IVehicleDeclarationInputData vehicleData)
		{
			var result = new List<XElement>() {
				new XElement(_mrf + XMLNames.CorrectedActualMass, vehicleData.CurbMassChassis.ValueAsUnit("kg")),
				new XElement(_mrf + XMLNames.Vehicle_ZeroEmissionVehicle, vehicleData.ZeroEmissionVehicle),
				new XElement(_mrf + XMLNames.Vehicle_HybridElectricHDV, vehicleData.HybridElectricHDV),
				new XElement(_mrf + XMLNames.Vehicle_RegisteredClass, vehicleData.RegisteredClass.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Bus_NumberPassengersUpperDeck, vehicleData.NumberPassengerSeatsUpperDeck + vehicleData.NumberPassengersStandingUpperDeck),
				new XElement(_mrf + XMLNames.Bus_NumberPassengersLowerDeck, vehicleData.NumberPassengerSeatsLowerDeck + vehicleData.NumberPassengersStandingLowerDeck),
				new XElement(_mrf + XMLNames.Vehicle_BodyworkCode, vehicleData.VehicleCode.ToXMLFormat()),
				new XElement(_mrf + XMLNames.Bus_LowEntry, vehicleData.LowEntry)
			};
			return result;
		}

		#endregion
	}

	internal class CompletedBusDimensionsSequenceOutputGroup : AbstractReportOutputGroup, IReportVehicleOutputGroup
	{
		public CompletedBusDimensionsSequenceOutputGroup(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlGroup

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IMrfVehicleGroup

		public IList<XElement> GetElements(IVehicleDeclarationInputData vehicleData)
		{
			var result = new List<XElement>() {
				new XElement(_mrf + XMLNames.Bus_HeightIntegratedBody, vehicleData.Height.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(_mrf + XMLNames.Bus_VehicleLength, vehicleData.Length.ConvertToMilliMeter().ToXMLFormat(0)),
				new XElement(_mrf + XMLNames.Bus_VehicleWidth, vehicleData.Width.ConvertToMilliMeter().ToXMLFormat(0))
			};
			return result;
		}

		#endregion
	}

}
