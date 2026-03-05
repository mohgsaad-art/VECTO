using System.Collections.Generic;
using System.Linq;
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

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

			if (vehicle.Components.AxlePowertrainInputData?.Count > 0)
			{
				return GetMultipleRetarders(vehicle);
            }
			
			var result = new List<XElement>
			{
				new XElement(_vif + XMLNames.Vehicle_RetarderType, vehicle.Components.RetarderInputData.Type.ToXMLFormat()),
				vehicle.Components.RetarderInputData.Type.IsDedicatedComponent()
					? new XElement(_vif + XMLNames.Vehicle_RetarderRatio, vehicle.Components.RetarderInputData?.Ratio.ToXMLFormat(3))
					: null,
			};
			return result;
		}

		private IList<XElement> GetMultipleRetarders(IVehicleDeclarationInputData vehicle)
		{
			var result = new List<XElement>();

			foreach(var axlePt in vehicle.Components.AxlePowertrainInputData.Where(x => x.RetarderInputData != null))
			{
				result.Add(new XElement(_vif + XMLNames.Vehicle_RetarderType, 
					new XAttribute("axleNumber", axlePt.AxleNumber.Value),
					axlePt.RetarderInputData.Type.ToXMLFormat()));
			}

            foreach (var axlePt in vehicle.Components.AxlePowertrainInputData.Where(x => x.RetarderInputData?.Type.IsDedicatedComponent() ?? false))
            {
				result.Add(new XElement(_vif + XMLNames.Vehicle_RetarderRatio,
                    new XAttribute("axleNumber", axlePt.AxleNumber.Value),
                    axlePt.RetarderInputData.Ratio.ToXMLFormat(3)));
            }

            return result;
        }
    }

	public class PrimaryBusXeVParameterGroup : AbstractVIFGroupWriter
	{
		public PrimaryBusXeVParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;

            return new List<XElement> 
			{
				new XElement(_vif + "OVC", vehicle.OVC),
				new XElement(_vif + "BatteryOnlyMode", vehicle.BatteryOnlyMode),
				new XElement(_vif + "DynamicChargingTechnology", vehicle.DynamicChargingTechnology.ToXMLFormat())
			};
		}
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
			if (vehicle.H2StorageUsableCapacity != null)
			{
				result.Add(new XElement(_vif + "H2StorageUsableCapacity", vehicle.H2StorageUsableCapacity.ToXMLFormat(1)));
				result.Add(new XElement(_vif + "HydrogenStorageTechnology", vehicle.HydrogenStorageTechnology));
			}

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

	public class FCHV_Fx_VehicleParameterGroup : AbstractVIFGroupWriter
	{
        public FCHV_Fx_VehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

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
            if (motorTorqueLimits != null)
                result.Add(motorTorqueLimits);
            
			result.Add(new XElement(_vif + "H2StorageUsableCapacity", vehicle.H2StorageUsableCapacity.ToXMLFormat(1)));
            result.Add(new XElement(_vif + "HydrogenStorageTechnology", vehicle.HydrogenStorageTechnology));

            return result;
		}
	}

	public class FCHV_IEPC_VehicleParameterGroup : AbstractVIFGroupWriter
	{
        public FCHV_IEPC_VehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

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
            result.Add(new XElement(_vif + "H2StorageUsableCapacity", vehicle.H2StorageUsableCapacity.ToXMLFormat(1)));
            result.Add(new XElement(_vif + "HydrogenStorageTechnology", vehicle.HydrogenStorageTechnology));

            return result;
		}
	}

	public abstract class AbstractMultipleVehicleParameterGroup : AbstractVIFGroupWriter
	{
        public AbstractMultipleVehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		protected IList<XElement> GetMultipleAngledrives(IVehicleDeclarationInputData vehicle)
		{
            var result = new List<XElement>();

			foreach (var axlePt in vehicle.Components.AxlePowertrainInputData)
			{
                result.Add(new XElement(
					_vif + XMLNames.Vehicle_AngledriveType,
					new XAttribute("axleNumber", axlePt.AxleNumber.Value),
					(axlePt.AngledriveInputData?.Type ?? AngledriveType.None).ToXMLFormat())
				);
            }

			return result;
        }
    }

    public class Multiple_FCHV_VehicleParameterGroup : AbstractMultipleVehicleParameterGroup
	{
        public Multiple_FCHV_VehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

        public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
        {
            var vehicle = inputData.JobInputData.Vehicle;

            var result = new List<XElement>();

            result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
            result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
            result.AddRange(_vifReportFactory.GetPrimaryBusRetarderParameterGroup().GetElements(inputData));
            result.AddRange(GetMultipleAngledrives(vehicle));
            result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
            result.Add(new XElement(_vif + XMLNames.Vehicle_ArchitectureID, vehicle.ArchitectureID.GetLabel()));
            result.Add(new XElement(_vif + "ArchitectureIDPwt2", vehicle.ArchitectureIDPwt2.GetLabel()));
            result.AddRange(_vifReportFactory.GetPrimaryBusXevParameterGroup().GetElements(inputData));
            result.Add(_vifReportFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));

            var motorTorqueLimits = _vifReportFactory.GetElectricMotorTorqueLimitsType().GetElement(inputData);
            if (motorTorqueLimits != null)
            {
                result.Add(motorTorqueLimits);
            }

            result.Add(new XElement(_vif + "H2StorageUsableCapacity", vehicle.H2StorageUsableCapacity.ToXMLFormat(1)));
            result.Add(new XElement(_vif + "HydrogenStorageTechnology", vehicle.HydrogenStorageTechnology));

            return result;
        }
    }

	public class Multiple_SHEV_VehicleParameterGroup : AbstractMultipleVehicleParameterGroup
	{
        public Multiple_SHEV_VehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

        public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
        {
            var vehicle = inputData.JobInputData.Vehicle;

            var result = new List<XElement>();

            result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
            result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
            result.Add(new XElement(_vif + XMLNames.Vehicle_IdlingSpeed, vehicle.EngineIdleSpeed.AsRPM.ToXMLFormat(0)));
            result.AddRange(_vifReportFactory.GetPrimaryBusRetarderParameterGroup().GetElements(inputData));
            result.AddRange(GetMultipleAngledrives(vehicle));
            result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
            result.Add(new XElement(_vif + XMLNames.Vehicle_ArchitectureID, vehicle.ArchitectureID.GetLabel()));
            result.Add(new XElement(_vif + "ArchitectureIDPwt2", vehicle.ArchitectureIDPwt2.GetLabel()));
            result.AddRange(_vifReportFactory.GetPrimaryBusXevParameterGroup().GetElements(inputData));
            result.Add(_vifReportFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));

            var motorTorqueLimits = _vifReportFactory.GetElectricMotorTorqueLimitsType().GetElement(inputData);
            if (motorTorqueLimits != null)
            {
                result.Add(motorTorqueLimits);
            }

            if (vehicle.H2StorageUsableCapacity != null)
            {
                result.Add(new XElement(_vif + "H2StorageUsableCapacity", vehicle.H2StorageUsableCapacity.ToXMLFormat(1)));
                result.Add(new XElement(_vif + "HydrogenStorageTechnology", vehicle.HydrogenStorageTechnology));
            }

            return result;
        }
    }

    public class Multiple_PEV_VehicleParameterGroup : AbstractMultipleVehicleParameterGroup
    {
        public Multiple_PEV_VehicleParameterGroup(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

        public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
        {
            var vehicle = inputData.JobInputData.Vehicle;

            var result = new List<XElement>();

            result.AddRange(_vifReportFactory.GetPrimaryBusGeneralParameterGroup().GetElements(inputData));
            result.AddRange(_vifReportFactory.GetPrimaryBusChassisParameterGroup().GetElements(inputData));
            result.AddRange(_vifReportFactory.GetPrimaryBusRetarderParameterGroup().GetElements(inputData));
			result.AddRange(GetMultipleAngledrives(vehicle));
            result.Add(new XElement(_vif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicle.ZeroEmissionVehicle));
            result.Add(new XElement(_vif + XMLNames.Vehicle_ArchitectureID, vehicle.ArchitectureID.GetLabel()));
            result.Add(new XElement(_vif + "ArchitectureIDPwt2", vehicle.ArchitectureIDPwt2.GetLabel()));
            result.AddRange(_vifReportFactory.GetPrimaryBusXevParameterGroup().GetElements(inputData));
            result.Add(_vifReportFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
            
			var motorTorqueLimits = _vifReportFactory.GetElectricMotorTorqueLimitsType().GetElement(inputData);
			if (motorTorqueLimits != null)
			{
				result.Add(motorTorqueLimits);
			}

            return result;
        }
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
			result.Add(_vifReportFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS));
			
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
