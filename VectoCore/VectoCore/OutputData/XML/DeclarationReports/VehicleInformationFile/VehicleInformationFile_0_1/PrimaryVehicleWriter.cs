using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class VehicleWriter : IXmlTypeWriter
	{
		protected readonly IVIFReportFactory _vifReportFactory;

		protected XNamespace _vif = XMLDefinitions.VEHICLE_INTERIM_FILE_TARGET_VERSION;
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected VehicleWriter(IVIFReportFactory vifReportFactory)
		{
			_vifReportFactory = vifReportFactory;
		}
		

		#region Implementation of IXmlTypeWriter

		public abstract XElement GetElement(IDeclarationInputDataProvider inputData);

		#endregion
	}


	public class ConventionalVehicleType : VehicleWriter
	{
		public ConventionalVehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }
		
		#region Overrides of VehicleWriter
		
		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetConventionalVehicleGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetConventionalComponentType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
					new XAttribute(_xsi + XMLNames.XSIType, "ConventionalVehicleVIFType"),
					vehicleData);
		}

		#endregion
	}


	public class HevIepcSVehicleType : VehicleWriter
	{
		public HevIepcSVehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetHevIepcSVehicleParameterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetHevIepcSComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
					new XAttribute(_xsi + XMLNames.XSIType, "HEV-IEPC-S_VehicleVIFType"),
					vehicleData);
		}

		#endregion
	}


	public class HevPxVehicleType : VehicleWriter
	{
		public HevPxVehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{

			var vehicleData = _vifReportFactory.GetHevPxVehicleParameterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetHevPxComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV-Px_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}


	public class HevS2VehicleType : VehicleWriter
	{
		public HevS2VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetHevSxVehicleParameterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetHevS2ComponentVIFType().GetElement(inputData));
			
			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV-Sx_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}


	public class HevS3VehicleType : VehicleWriter
	{
		public HevS3VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetHevSxVehicleParameterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetHevS3ComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV-Sx_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}


	public class HevS4VehicleType : VehicleWriter
	{
		public HevS4VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetHevSxVehicleParameterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetHevS4ComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV-Sx_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}

	public class Multiple_SHEV_VehicleType : VehicleWriter
	{
        public Multiple_SHEV_VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

        public override XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var vehicleData = _vifReportFactory.Get_Multiple_SHEV_VehicleParmeterGroup().GetElements(inputData);
            vehicleData.Add(_vifReportFactory.Get_Multiple_SHEV_ComponentVIFType().GetElement(inputData));

            return new XElement(_vif + XMLNames.Component_Vehicle,
                new XAttribute(_xsi + XMLNames.XSIType, "Multiple_SHEV_VehicleType"),
                vehicleData
            );
        }
    }

	public class Multiple_PEV_VehicleType : VehicleWriter
	{
        public Multiple_PEV_VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

        public override XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var vehicleData = _vifReportFactory.Get_Multiple_PEV_VehicleParmeterGroup().GetElements(inputData);
            vehicleData.Add(_vifReportFactory.Get_Multiple_PEV_ComponentVIFType().GetElement(inputData));

            return new XElement(_vif + XMLNames.Component_Vehicle,
                new XAttribute(_xsi + XMLNames.XSIType, "Multiple_PEV_VehicleType"),
                vehicleData
            );
        }
    }

	public class Multiple_FCHV_VehicleType : VehicleWriter
	{
        public Multiple_FCHV_VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

        public override XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var vehicleData = _vifReportFactory.Get_Multiple_FCHV_VehicleParmeterGroup().GetElements(inputData);
            vehicleData.Add(_vifReportFactory.Get_Multiple_FCHV_ComponentVIFType().GetElement(inputData));

            return new XElement(_vif + XMLNames.Component_Vehicle,
                new XAttribute(_xsi + XMLNames.XSIType, "Multiple_FCHV_VehicleType"),
                vehicleData
			);
        }
    }

    public class FCHV_F2_VehicleType : VehicleWriter
	{
		public FCHV_F2_VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.Get_FCHV_Fx_VehicleParmeterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.Get_FCHV_F2_ComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "FCHV_Fx_VehicleVIFType"),
				vehicleData);
		}
	}

	public class FCHV_F3_VehicleType : VehicleWriter
	{
		public FCHV_F3_VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.Get_FCHV_Fx_VehicleParmeterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.Get_FCHV_F3_ComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "FCHV_Fx_VehicleVIFType"),
				vehicleData);
		}
	}

	public class FCHV_F4_VehicleType : VehicleWriter
	{
		public FCHV_F4_VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.Get_FCHV_Fx_VehicleParmeterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.Get_FCHV_F4_ComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "FCHV_Fx_VehicleVIFType"),
				vehicleData);
		}
	}

	public class FCHV_IEPC_VehicleType : VehicleWriter
	{
		public FCHV_IEPC_VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.Get_FCHV_IEPC_VehicleParmeterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetHevIepcFComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "FCHV_IEPC_VehicleVIFType"),
				vehicleData);
		}
	}

	public class PevE2VehicleType : VehicleWriter
	{
		public PevE2VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetPevExVehicleParmeterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetPevE2ComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "PEV_Ex_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}

	public class PevE3VehicleType : VehicleWriter
	{
		public PevE3VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetPevExVehicleParmeterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetPevE3ComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "PEV_Ex_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}

	public class PevE4VehicleType : VehicleWriter
	{
		public PevE4VehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetPevExVehicleParmeterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetPevE4ComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "PEV_Ex_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}

	public class PevIEPCVehicleType : VehicleWriter
	{
		public PevIEPCVehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetPevIEPCVehicleParmeterGroup().GetElements(inputData);
			vehicleData.Add(_vifReportFactory.GetPevIEPCComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "IEPC_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}

	public class ExemptedVehicleType : VehicleWriter
	{
		public ExemptedVehicleType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = _vifReportFactory.GetExemptedVehicleParameterGroup().GetElements(inputData);
			//vehicleData.Add(_vifReportFactory.GetPevIEPCComponentVIFType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "Exempted_VehicleVIFType"),
				vehicleData);
		}

		#endregion
	}
}
