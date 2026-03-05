using System.Collections.Generic;
using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class PrimaryComponentVIFType : IXmlTypeWriter
	{
		protected readonly IVIFReportFactory _vifReportFactory;
		protected XNamespace _vif = XMLDefinitions.VEHICLE_INTERIM_FILE_TARGET_VERSION;
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected PrimaryComponentVIFType(IVIFReportFactory vifReportFactory)
		{
			_vifReportFactory = vifReportFactory;
		}

		#region Implementation of IXmlTypeWriter

		public abstract XElement GetElement(IDeclarationInputDataProvider inputData);

		#endregion
	}
	

	public class ConventionalComponentVIFType : PrimaryComponentVIFType
	{
		public ConventionalComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
				return new XElement(_vif + XMLNames.Vehicle_Components,
					new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_Conventional_ComponentsVIFType"),
					_vifReportFactory.GetEngineType().GetElement(inputData),
					_vifReportFactory.GetTransmissionType().GetElement(inputData),
					_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
					_vifReportFactory.GetAngelDriveType().GetElement(inputData),
					_vifReportFactory.GetRetarderType().GetElement(inputData), 
					_vifReportFactory.GetAxlegearType().GetElement(inputData),
					_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
					_vifReportFactory.GetAuxiliaryType().GetElement(inputData));
		}

		#endregion
	}


	public class HevIepcSComponentVIFType : PrimaryComponentVIFType
	{
		public HevIepcSComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-IEPC-S_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetIepcType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryIEPC_SType().GetElement(inputData));
		}

		#endregion
	}

	public class HevPxComponentVIFType : PrimaryComponentVIFType
	{
		public HevPxComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-Px_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetTransmissionType().GetElement(inputData),
				_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
				_vifReportFactory.GetAngelDriveType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevPType().GetElement(inputData));
		}

		#endregion
	}

	public class HevS2ComponentVIFType: PrimaryComponentVIFType
	{
		public HevS2ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-S2_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetTransmissionType().GetElement(inputData),
				_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
				_vifReportFactory.GetAngelDriveType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}


	public class HevS3ComponentVIFType : PrimaryComponentVIFType
	{
		public HevS3ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-S3_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}


	public class HevS4ComponentVIFType : PrimaryComponentVIFType
	{
		public HevS4ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV-S4_ComponentsVIFType"),
				_vifReportFactory.GetEngineType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineGENType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
		}

		#endregion
	}

	public abstract class MultipleComponentVIFType : PrimaryComponentVIFType
	{
        protected readonly Dictionary<ArchitectureID, Func<IAxlePowertrainDeclarationInputData, XElement>> _getPowertrain;

        public MultipleComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) 
		{
            _getPowertrain = new Dictionary<ArchitectureID, Func<IAxlePowertrainDeclarationInputData, XElement>>();
        }

        protected XElement GetEM2Powertrain(IAxlePowertrainDeclarationInputData axlePt)
        {
            return new XElement(_vif + "Powertrain",
                new XAttribute("axleNumber", axlePt.AxleNumber.Value),
                new XAttribute(AbstractVehicleInformationFile.XSI + XMLNames.XSIType, "xEV_EM2_Powertrain_VIFType"),
                _vifReportFactory.GetAxlePowertrainElectricMachineType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainTransmissionType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainTorqueConverterType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainAngleDriveType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainRetarderType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainAxleGearType().GetElement(axlePt)
            );
        }

        protected XElement GetEM3Powertrain(IAxlePowertrainDeclarationInputData axlePt)
        {
            return new XElement(_vif + "Powertrain",
                new XAttribute("axleNumber", axlePt.AxleNumber.Value),
                new XAttribute(AbstractVehicleInformationFile.XSI + XMLNames.XSIType, "xEV_EM3_Powertrain_VIFType"),
                _vifReportFactory.GetAxlePowertrainElectricMachineType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainAngleDriveType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainRetarderType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainAxleGearType().GetElement(axlePt)
            );
        }

        protected XElement GetEM4Powertrain(IAxlePowertrainDeclarationInputData axlePt)
        {
            return new XElement(_vif + "Powertrain",
                new XAttribute("axleNumber", axlePt.AxleNumber.Value),
                new XAttribute(AbstractVehicleInformationFile.XSI + XMLNames.XSIType, "xEV_EM4_Powertrain_VIFType"),
                _vifReportFactory.GetAxlePowertrainElectricMachineType().GetElement(axlePt)
            );
        }

        protected XElement GetIEPCPowertrain(IAxlePowertrainDeclarationInputData axlePt)
        {
            return new XElement(_vif + "Powertrain",
                new XAttribute("axleNumber", axlePt.AxleNumber.Value),
                new XAttribute(AbstractVehicleInformationFile.XSI + XMLNames.XSIType, "xEV_IEPC_Powertrain_VIFType"),
                _vifReportFactory.GetAxlePowertrainIEPCType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainRetarderType().GetElement(axlePt),
                _vifReportFactory.GetAxlePowertrainAxleGearType().GetElement(axlePt)
            );
		}
	}

	public class Multiple_FCHV_ComponentVIFType : MultipleComponentVIFType
	{
        public Multiple_FCHV_ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory)
        {
            _getPowertrain.Add(ArchitectureID.F2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.F3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.F4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.F_IEPC, GetIEPCPowertrain);
        }

        public override XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

            return new XElement(_vif + XMLNames.Vehicle_Components,
                new XAttribute(_xsi + XMLNames.XSIType, "Components_Multiple_FCHV_VIFType"),
				_vifReportFactory.GetFuelCellType().GetElement(inputData),
                _vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
                _getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _vifReportFactory.GetAxleWheelsType().GetElement(inputData),
                _vifReportFactory.GetAuxiliaryFCHVType().GetElement(inputData));
		}
	}

	public class Multiple_SHEV_ComponentVIFType : MultipleComponentVIFType
	{
        public Multiple_SHEV_ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory)
        {
            _getPowertrain.Add(ArchitectureID.S2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.S3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.S4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.S_IEPC, GetIEPCPowertrain);
        }

        public override XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

            return new XElement(_vif + XMLNames.Vehicle_Components,
                new XAttribute(_xsi + XMLNames.XSIType, "Components_Multiple_SHEV_VIFType"),
                _vifReportFactory.GetEngineType().GetElement(inputData),
                _vifReportFactory.GetGeneratorType().GetElement(inputData),
                _vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
                _getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _vifReportFactory.GetAxleWheelsType().GetElement(inputData),
                _vifReportFactory.GetAuxiliaryHevSType().GetElement(inputData));
        }
    }

	public class Multiple_PEV_ComponentVIFType : MultipleComponentVIFType
    {
        public Multiple_PEV_ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) 
		{
            _getPowertrain.Add(ArchitectureID.E2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.E3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.E4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.E_IEPC, GetIEPCPowertrain);
        }

        public override XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

            return new XElement(_vif + XMLNames.Vehicle_Components,
                new XAttribute(_xsi + XMLNames.XSIType, "Components_Multiple_PEV_VIFType"),
                _vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
                _getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _vifReportFactory.GetAxleWheelsType().GetElement(inputData),
                _vifReportFactory.GetAuxiliaryPEVType().GetElement(inputData));
        }
    }

	public class FCHV_F2_ComponentVIFType : PrimaryComponentVIFType
	{
		public FCHV_F2_ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_FCHV_F2_ComponentsVIFType"),
				_vifReportFactory.GetFuelCellType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetTransmissionType().GetElement(inputData),
				_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
				_vifReportFactory.GetAngelDriveType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData),
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryFCHVType().GetElement(inputData));
		}
	}

	public class FCHV_F3_ComponentVIFType : PrimaryComponentVIFType
	{
		public FCHV_F3_ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_FCHV_F3_ComponentsVIFType"),
				_vifReportFactory.GetFuelCellType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData),
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryFCHVType().GetElement(inputData));
		}
	}

	public class FCHV_F4_ComponentVIFType : PrimaryComponentVIFType
	{
		public FCHV_F4_ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_FCHV_F4_ComponentsVIFType"),
				_vifReportFactory.GetFuelCellType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryFCHVType().GetElement(inputData));
		}
	}

	public class FCHV_IEPC_ComponentVIFType : PrimaryComponentVIFType
	{
		public FCHV_IEPC_ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_FCHV_IEPC_ComponentsVIFType"),
				_vifReportFactory.GetFuelCellType().GetElement(inputData),
                _vifReportFactory.GetIepcType().GetElement(inputData),
                _vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData),
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryFCHVType().GetElement(inputData));
		}
	}

	public class PevE2ComponentVIFType : PrimaryComponentVIFType
	{
		public PevE2ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV-E2_ComponentsVIFType"),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetTransmissionType().GetElement(inputData),
				_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
				_vifReportFactory.GetAngelDriveType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryPEVType().GetElement(inputData));
		}

		#endregion
	}


	public class PevE3ComponentVIFType : PrimaryComponentVIFType
	{
		public PevE3ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV-E3_ComponentsVIFType"),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryPEVType().GetElement(inputData));
		}

		#endregion
	}


	public class PevE4ComponentVIFType : PrimaryComponentVIFType
	{
		public PevE4ComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV-E4_ComponentsVIFType"),
				_vifReportFactory.GetElectricMachineType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryPEVType().GetElement(inputData));
		}

		#endregion
	}


	public class IepcComponentVIFType : PrimaryComponentVIFType
	{
		public IepcComponentVIFType(IVIFReportFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of ComponentVIFType

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_vif + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV-IEPC_ComponentsVIFType"),
				_vifReportFactory.GetIepcType().GetElement(inputData),
				_vifReportFactory.GetElectricEnergyStorageType().GetElement(inputData),
				_vifReportFactory.GetRetarderType().GetElement(inputData), 
				_vifReportFactory.GetAxlegearType().GetElement(inputData),
				_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
				_vifReportFactory.GetAuxiliaryIEPCType().GetElement(inputData));
		}

		#endregion
	}

}
