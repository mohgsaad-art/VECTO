using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{

	public class PrimaryBusGeneralParameterGroup : AbstractVIFGroupWriter
	{
		public PrimaryBusGeneralParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			
			var result = new List<XElement>
			{
				new XElement(_vif + XMLNames.Component_Manufacturer, vehicle.Manufacturer),
				new XElement(_vif + XMLNames.Component_ManufacturerAddress, vehicle.ManufacturerAddress),
				new XElement(_vif + XMLNames.Component_Model, vehicle.Model),
				new XElement(_vif + XMLNames.Vehicle_VIN, vehicle.VIN),
				new XElement(_vif + XMLNames.Component_Date, XmlConvert.ToString(vehicle.Date, XmlDateTimeSerializationMode.Utc)),
			};

			return result;
		}

		#endregion
	}

	public class PrimaryBusChassisParameterGroup : AbstractVIFGroupWriter
	{
		public PrimaryBusChassisParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var result = new List<XElement>
			{
				new XElement(_vif + XMLNames.Vehicle_LegislativeCategory, vehicle.LegislativeClass.ToXMLFormat()),
				new XElement(_vif + XMLNames.ChassisConfiguration, vehicle.VehicleCategory.ToXMLFormat()),
				new XElement(_vif + XMLNames.Vehicle_AxleConfiguration, vehicle.AxleConfiguration.ToXMLFormat()),
				new XElement(_vif + XMLNames.Vehicle_Articulated, vehicle.Articulated),
				new XElement(_vif + XMLNames.TPMLM, vehicle.GrossVehicleMassRating.ToXMLFormat(0)),
			};

			return result;
		}

		#endregion
	}

	public class PrimaryBusRetarderParameterGroup : AbstractVIFGroupWriter
	{
		public PrimaryBusRetarderParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			var result = new List<XElement>
			{
				new XElement(_vif + XMLNames.Vehicle_RetarderType, vehicle.Components.RetarderInputData.Type.ToXMLFormat()),
				vehicle.Components.RetarderInputData.Type.IsDedicatedComponent()
					? new XElement(_vif + XMLNames.Vehicle_RetarderRatio, vehicle.Components.RetarderInputData?.Ratio.ToXMLFormat(3))
					: null,
			};
			return result;
		}

		#endregion
	}

	public class PrimaryBusXeVParameterGroup : AbstractVIFGroupWriter
	{
		public PrimaryBusXeVParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var result = new List<XElement> {
				new XElement(_vif + XMLNames.Vehicle_OvcHev, vehicle.OVC),
			};
			if (vehicle.ArchitectureID.IsHybridVehicle() && vehicle.OVC) {
				result.Add(new XElement(_vif + XMLNames.Vehicle_MaxChargingPower, vehicle.MaxChargingPower.Value()));
			}
			return result;
		}

		#endregion
	}

	
	public class ConventionalVIFVehicleParameterGroup : AbstractVIFGroupWriter
	{
		public ConventionalVIFVehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }
		
		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var result = new List<XElement>();
			result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_IdlingSpeed, vehicle.EngineIdleSpeed.AsRPM.ToXMLFormat(0)));
			result.Add(new XElement(_vif + XMLNames.Vehicle_RetarderType, vehicle.Components.RetarderInputData.Type.ToXMLFormat()));
			if (vehicle.Components.RetarderInputData.Type.IsDedicatedComponent()) {
				result.Add(new XElement(_vif + XMLNames.Vehicle_RetarderRatio,
					vehicle.Components.RetarderInputData?.Ratio.ToXMLFormat(3)));
			}
			result.Add(new XElement(_vif + XMLNames.Vehicle_AngledriveType, vehicle.Components.AngledriveInputData.Type.ToXMLFormat()));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
			result.Add(_vifReportFactory.GetConventionalADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			result.Add(_vifReportFactory.GetTorqueLimitsType().GetElement(inputData));
			
			return result;
		}

		#endregion
	}


	public class HevIepcSVehicleParameterGroup : AbstractVIFGroupWriter
	{
		public HevIepcSVehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			
			var result = new List<XElement>();
			result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_IdlingSpeed, vehicle.EngineIdleSpeed.AsRPM.ToXMLFormat(0)));
			result.AddRange(_vifReportFactory.GetPrimaryBusRetarderParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_AngledriveType, (vehicle.Components.AngledriveInputData?.Type ?? AngledriveType.None).ToXMLFormat()));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ArchitectureID, vehicle.ArchitectureID.GetLabel()));
			result.AddRange(_vifReportFactory.GetPrimaryBusXevParameterGroup().GetElements(inputData));
			result.Add(_vifReportFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			var electricMotorTorque = _vifReportFactory.GetElectricMotorTorqueLimitsType().GetElement(inputData);
			if(electricMotorTorque != null)
				result.Add(electricMotorTorque);

			return result;
		}

		#endregion
	}

	public class HevSxVehicleParameterGroup : AbstractVIFGroupWriter
	{
		public HevSxVehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var result = new List<XElement>();
			result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_IdlingSpeed, vehicle.EngineIdleSpeed.AsRPM.ToXMLFormat(0)));
			result.AddRange(_vifReportFactory.GetPrimaryBusRetarderParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_AngledriveType, (vehicle.Components.AngledriveInputData?.Type ?? AngledriveType.None).ToXMLFormat()));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ArchitectureID, vehicle.ArchitectureID.GetLabel()));
			result.AddRange(_vifReportFactory.GetPrimaryBusXevParameterGroup().GetElements(inputData));
			result.Add(_vifReportFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			var electricMotorTorque = _vifReportFactory.GetElectricMotorTorqueLimitsType().GetElement(inputData);
			if (electricMotorTorque != null)
				result.Add(electricMotorTorque);

			return result;
		}

		#endregion
	}

	public class PevExVehicleParameterGroup : AbstractVIFGroupWriter
	{
		public PevExVehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var result = new List<XElement>();
			result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusRetarderParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_AngledriveType, (vehicle.Components.AngledriveInputData?.Type ?? AngledriveType.None).ToXMLFormat()));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ArchitectureID, vehicle.ArchitectureID.GetLabel()));
			result.AddRange(_vifReportFactory.GetPrimaryBusXevParameterGroup().GetElements(inputData));
			result.Add(_vifReportFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			var motorTorqueLimits = _vifReportFactory.GetElectricMotorTorqueLimitsType().GetElement(inputData);
			if(motorTorqueLimits != null)
				result.Add(motorTorqueLimits);
			
			return result;
		}

		#endregion
	}

	public class PevIEPCVehicleParameterGroup : AbstractVIFGroupWriter
	{
		public PevIEPCVehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var result = new List<XElement>();
			result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusRetarderParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_AngledriveType, (vehicle.Components.AngledriveInputData?.Type ?? AngledriveType.None).ToXMLFormat()));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ArchitectureID, vehicle.ArchitectureID.GetLabel()));
			result.AddRange(_vifReportFactory.GetPrimaryBusXevParameterGroup().GetElements(inputData));
			result.Add(_vifReportFactory.GetIEPCADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			
			return result;
		}

		#endregion
	}

	public class HevPxVehicleParameterGroup : AbstractVIFGroupWriter
	{
		public HevPxVehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var result = new List<XElement>();

			result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_IdlingSpeed, vehicle.EngineIdleSpeed.AsRPM.ToXMLFormat(0)));
			result.AddRange(_vifReportFactory.GetPrimaryBusRetarderParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_AngledriveType, vehicle.Components.AngledriveInputData.Type.ToXMLFormat()));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ArchitectureID, vehicle.ArchitectureID.GetLabel()));
			result.AddRange(_vifReportFactory.GetPrimaryBusXevParameterGroup().GetElements(inputData));
			result.Add(_vifReportFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			result.Add(_vifReportFactory.GetTorqueLimitsType().GetElement(inputData));
			result.Add(_vifReportFactory.GetElectricMotorTorqueLimitsType().GetElement(inputData));
			result.Add(_vifReportFactory.GetBoostingLimitationsType().GetElement(inputData));

			return result;
		}

		#endregion
	}

	public class ExemptedVehicleParameterGroup : AbstractVIFGroupWriter
	{
		public ExemptedVehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of AbstractVIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			var result = new List<XElement>();

			result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
			result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
			result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
			result.Add(new XElement(_vif + "SumNetPower", XMLHelper.ValueAsUnit(vehicle.MaxNetPower1, "W")));
			result.Add(new XElement(_vif + "Technology", vehicle.ExemptedTechnology));

			return result;
		}

		#endregion
	}
}
