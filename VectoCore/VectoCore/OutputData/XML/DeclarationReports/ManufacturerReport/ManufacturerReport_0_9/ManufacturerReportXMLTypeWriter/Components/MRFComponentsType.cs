using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
	#region Lorry
	internal class ConventionalLorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public ConventionalLorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of MRFComponentType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var addTorqueConverterData =
				inputData.JobInputData.Vehicle.Components.GearboxInputData.Type == GearboxType.ATPowerSplit ||
				inputData.JobInputData.Vehicle.Components.GearboxInputData.Type == GearboxType.ATSerial;
			var addRetarderInputData = inputData.JobInputData.Vehicle.Components.RetarderInputData != null;
			var addAngleDriveData = inputData.JobInputData.Vehicle.Components.AngledriveInputData != null;
			//var addAirdragdata = inputData.JobInputData.Vehicle.Components.AirdragInputData != null;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				(addTorqueConverterData ? _mrfFactory.GetTorqueConverterType().GetElement(inputData) : null),
				(addAngleDriveData ? _mrfFactory.GetAngleDriveType().GetElement(inputData) : null),
				(addRetarderInputData ? _mrfFactory.GetRetarderType().GetElement(inputData) : null),
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetConventionalLorryAuxType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
			);
			//return new XElement(_mrf + XMLNames.Vehicle_Components,
			//	_mrfFactory.GetEngineType())
		}

		#endregion
	}

	internal class MrfhevPxIhpcLorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevPxIhpcLorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var result = new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
                _mrfFactory.GetTransmissionType().GetElement(inputData),
				_mrfFactory.GetTorqueConverterType().GetElement(inputData),
				_mrfFactory.GetAngleDriveType().GetElement(inputData),
				_mrfFactory.GetRetarderType().GetElement(inputData),
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
			);
			return result;
		}

		#endregion
	}

	internal class MrfhevS2LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS2LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-S2-LorryComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachineGenType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
                _mrfFactory.GetTransmissionType().GetElement(inputData),
				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetElement(inputData)
					: null,
				components.AngledriveInputData != null ? _mrfFactory.GetAngleDriveType().GetElement(inputData) : null,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,

				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
				);
		}

		#endregion
	}

	internal class MrfhevS3LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS3LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

        #region Overrides of AbstractMrfXmlType
        
        public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-S3-LorryComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachineGenType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
                components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData));
		}

		#endregion
	}

	internal class MrfhevS4LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS4LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-S4-LorryComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachineGenType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
                components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData));
		}
		#endregion
	}

	internal class MrfhevIepcSLorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevIepcSLorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, $"HEV-IEPC-S-LorryComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachineGenType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData));
		}

		#endregion
	}

	internal class MRF_Multiple_ComponentsTypeWriter : AbstractMrfXmlType
	{
        protected readonly Dictionary<ArchitectureID, Func<IAxlePowertrainDeclarationInputData, XElement>> _getPowertrain;

        public MRF_Multiple_ComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory)
		{
			_getPowertrain = new Dictionary<ArchitectureID, Func<IAxlePowertrainDeclarationInputData, XElement>>();
        }

        protected XElement GetEM2Powertrain(IAxlePowertrainDeclarationInputData axlePt)
        {
            return new XElement(_mrf + "Powertrain",
                new XAttribute("axleNumber", axlePt.AxleNumber),
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PowertrainEM2Type"),
                _mrfFactory.GetAxlePowertrainElectricMachineType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainTransmissionType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainTorqueConverterType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainAngleDriveType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainRetarderType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainAxleGearType().GetElement(axlePt)
            );
        }

        protected XElement GetEM3Powertrain(IAxlePowertrainDeclarationInputData axlePt)
        {
            return new XElement(_mrf + "Powertrain",
                new XAttribute("axleNumber", axlePt.AxleNumber),
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PowertrainEM3Type"),
                _mrfFactory.GetAxlePowertrainElectricMachineType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainAngleDriveType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainRetarderType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainAxleGearType().GetElement(axlePt)
            );
        }

        protected XElement GetEM4Powertrain(IAxlePowertrainDeclarationInputData axlePt)
        {
            return new XElement(_mrf + "Powertrain",
                new XAttribute("axleNumber", axlePt.AxleNumber),
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PowertrainEM4Type"),
                _mrfFactory.GetAxlePowertrainElectricMachineType().GetElement(axlePt)
            );
        }

        protected XElement GetIEPCPowertrain(IAxlePowertrainDeclarationInputData axlePt)
        {
            return new XElement(_mrf + "Powertrain",
                new XAttribute("axleNumber", axlePt.AxleNumber),
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PowertrainIEPCType"),
                _mrfFactory.GetAxlePowertrainIEPCSpecifications().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainRetarderType().GetElement(axlePt),
                _mrfFactory.GetAxlePowertrainAxleGearType().GetElement(axlePt)
            );
        }
    }

	internal class MRF_Multiple_FCHV_LorryComponentsTypeWriter : MRF_Multiple_ComponentsTypeWriter, IXmlTypeWriter
	{
        public MRF_Multiple_FCHV_LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) 
		{
			_getPowertrain.Add(ArchitectureID.F2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.F3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.F4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.F_IEPC, GetIEPCPowertrain);
		}

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var components = inputData.JobInputData.Vehicle.Components;
			var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-Multiple-Fx-LorryComponentsType"),
                _mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
                _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
            );
        }
    }

    internal class MRF_Multiple_PEV_LorryComponentsTypeWriter : MRF_Multiple_ComponentsTypeWriter, IXmlTypeWriter
	{
		public MRF_Multiple_PEV_LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory)
		{
            _getPowertrain.Add(ArchitectureID.E2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.E3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.E4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.E_IEPC, GetIEPCPowertrain);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var components = inputData.JobInputData.Vehicle.Components;
            var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

            return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-Multiple-Ex-LorryComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
                _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
            );
        }
    }

    internal class MRF_Multiple_SHEV_LorryComponentsTypeWriter : MRF_Multiple_ComponentsTypeWriter, IXmlTypeWriter
    {
        public MRF_Multiple_SHEV_LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory)
        {
            _getPowertrain.Add(ArchitectureID.S2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.S3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.S4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.S_IEPC, GetIEPCPowertrain);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var components = inputData.JobInputData.Vehicle.Components;
            var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

            return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-Multiple-Sx-LorryComponentsType"),
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetGeneratorType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
                _mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
            );
        }
    }

    internal class MRF_FCHV_F2_LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MRF_FCHV_F2_LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-F2-LorryComponentsType"),

				_mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				components.TorqueConverterInputData != null ? _mrfFactory.GetTorqueConverterType().GetElement(inputData) : null,
				components.AngledriveInputData != null ? _mrfFactory.GetAngleDriveType().GetElement(inputData) : null,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
			);
		}
	}

	internal class MRF_FCHV_F3_LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MRF_FCHV_F3_LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory)
			: base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-F3-LorryComponentsType"),

                _mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData));
		}
	}

	internal class MRF_FCHV_F4_LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MRF_FCHV_F4_LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-F4-LorryComponentsType"),

                _mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData));
		}
	}

	internal class MRF_FCHV_IEPC_LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MRF_FCHV_IEPC_LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-IEPC-F-LorryComponentsType"),

                _mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData));
		}
	}

	internal class MrfpevE2LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE2LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-E2-LorryComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetElement(inputData)
					: null,
				components.RetarderInputData != null
					? _mrfFactory.GetRetarderType().GetElement(inputData)
					: null,

				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
			);
		}
	}

	internal class MrfpevE3LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE3LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-E3-LorryComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
			);
		}

	}

	internal class MrfpevE4LorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE4LorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-E4-LorryComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
			);
		}
	}

	internal class MrfPevIEPCLorryComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfPevIEPCLorryComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-IEPC-LorryComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPEV_LorryAuxiliariesType().GetXmlType(inputData.JobInputData.Vehicle.Components.AuxiliaryInputData),
				_mrfFactory.GetAirdragType().GetXmlType(inputData.JobInputData.Vehicle, inputData.JobInputData.Vehicle.Components.AirdragInputData)
			);
		}
	}

	#endregion Lorry


	#region PrimaryBus
	internal class ConventionalPrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public ConventionalPrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;

			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				components.TorqueConverterInputData != null ? _mrfFactory.GetTorqueConverterType().GetElement(inputData) : null,
				(components.AngledriveInputData != null) && (components.AngledriveInputData.Type == AngledriveType.SeparateAngledrive)
					? _mrfFactory.GetAngleDriveType().GetElement(inputData) : null,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_Conventional().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}


	internal class MrfhevPxIhpcPrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevPxIhpcPrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
                _mrfFactory.GetTransmissionType().GetElement(inputData),
				_mrfFactory.GetTorqueConverterType().GetElement(inputData),
				_mrfFactory.GetAngleDriveType().GetElement(inputData),
				_mrfFactory.GetRetarderType().GetElement(inputData),
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_P().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfhevS2PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS2PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-S2-PrimaryBusComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachineGenType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
                _mrfFactory.GetTransmissionType().GetElement(inputData),

				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetElement(inputData)
					: null,

				components.AngledriveInputData != null
					? _mrfFactory.GetAngleDriveType().GetElement(inputData)
					: null,

				components.RetarderInputData != null
					? _mrfFactory.GetRetarderType().GetElement(inputData)
					: null,

				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfhevS3PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS3PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-S3-PrimaryBusComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachineGenType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),

                components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries));
		}

		#endregion
	}

	internal class MrfhevS4PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevS4PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-S4-PrimaryBusComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachineGenType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
                _mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries));
		}

		#endregion
	}

	internal class MrfhevIepcSPrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevIepcSPrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-IEPC-S-PrimaryBusComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
				_mrfFactory.GetElectricMachineGenType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MRF_Multiple_SHEV_PrimaryBusComponentsTypeWriter : MRF_Multiple_ComponentsTypeWriter, IXmlTypeWriter
	{
        public MRF_Multiple_SHEV_PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory)
        {
            _getPowertrain.Add(ArchitectureID.S2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.S3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.S4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.S_IEPC, GetIEPCPowertrain);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var components = inputData.JobInputData.Vehicle.Components;
            var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

            return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "HEV-Multiple-Sx-PrimaryBusComponentsType"),
                _mrfFactory.GetEngineType().GetElement(inputData),
                _mrfFactory.GetGeneratorType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPrimaryBusAuxType_HEV_S().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
            );
        }
    }

	internal class MRF_Multiple_PEV_PrimaryBusComponentsTypeWriter : MRF_Multiple_ComponentsTypeWriter, IXmlTypeWriter
	{
        public MRF_Multiple_PEV_PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory)
        {
            _getPowertrain.Add(ArchitectureID.E2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.E3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.E4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.E_IEPC, GetIEPCPowertrain);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var components = inputData.JobInputData.Vehicle.Components;
            var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

            return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-Multiple-Ex-PrimaryBusComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
            );
        }
    }

	internal class MRF_Multiple_FCHV_PrimaryBusComponentsTypeWriter : MRF_Multiple_ComponentsTypeWriter, IXmlTypeWriter
	{
        public MRF_Multiple_FCHV_PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) 
		{
            _getPowertrain.Add(ArchitectureID.F2, GetEM2Powertrain);
            _getPowertrain.Add(ArchitectureID.F3, GetEM3Powertrain);
            _getPowertrain.Add(ArchitectureID.F4, GetEM4Powertrain);
            _getPowertrain.Add(ArchitectureID.F_IEPC, GetIEPCPowertrain);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var components = inputData.JobInputData.Vehicle.Components;
            var axlePts = inputData.JobInputData.Vehicle.Components.AxlePowertrainInputData;

            return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-Multiple-Fx-PrimaryBusComponentsType"),
                _mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _getPowertrain[axlePts[0].Architecture](axlePts[0]),
                _getPowertrain[axlePts[1].Architecture](axlePts[1]),
                _mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
            );
        }
    }

	internal class MRF_FCHV_F2_PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MRF_FCHV_F2_PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-F2-PrimaryBusComponentsType"),

				_mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
                _mrfFactory.GetTransmissionType().GetElement(inputData),
				(components.TorqueConverterInputData != null) ? _mrfFactory.GetTorqueConverterType().GetElement(inputData) : null,
				(components.AngledriveInputData != null) ? _mrfFactory.GetAngleDriveType().GetElement(inputData) : null,
				(components.RetarderInputData != null) ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
            );
		}
	}

	internal class MRF_FCHV_F3_PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MRF_FCHV_F3_PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-F3-PrimaryBusComponentsType"),

                _mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				(components.RetarderInputData != null) ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
            );
		}
	}

	internal class MRF_FCHV_F4_PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MRF_FCHV_F4_PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-F4-PrimaryBusComponentsType"),

                _mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
            );
		}
	}

	internal class MRF_FCHV_IEPC_PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MRF_FCHV_IEPC_PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "FCHV-IEPC-F-PrimaryBusComponentsType"),

                _mrfFactory.GetFuelCellSystemType().GetElement(inputData),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
				_mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				(components.RetarderInputData != null) ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				(components.AxleGearInputData != null) ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
                _mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
            );
		}
	}

	internal class MrfpevE2PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE2PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-E2-PrimaryBusComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				_mrfFactory.GetTransmissionType().GetElement(inputData),
				components.TorqueConverterInputData != null ? _mrfFactory.GetTorqueConverterType().GetElement(inputData) : null,
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfpevE3PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE3PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-E3-PrimaryBusComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				_mrfFactory.GetAxleGearType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
				);
		}

		#endregion
	}


	internal class MrfpevE4PrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevE4PrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-E4-PrimaryBusComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetElectricMachineType().GetElement(inputData),
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV().GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfpevIepcPrimaryBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevIepcPrimaryBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public  XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
                new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "PEV-IEPC-PrimaryBusComponentsType"),
                _mrfFactory.GetREESSSpecificationsType().GetElement(inputData),
                _mrfFactory.GetIEPCSpecifications().GetElement(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetElement(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetElement(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetElement(inputData),
				_mrfFactory.GetPrimaryBusAuxType_PEV()
					.GetElement(inputData.JobInputData.Vehicle.Components.BusAuxiliaries));
		}

		#endregion
	}

	#endregion
	#region CompletedBus
	internal class ConventionalCompletedBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public ConventionalCompletedBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputData = inputData as IMultistepBusInputDataProvider;
			if (multistageInputData == null) {
				throw new ArgumentException($"inputData must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetAirdragType().GetXmlType(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle, multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData),
				_mrfFactory.GetConventionalCompletedBusAuxType().GetElement(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfhevCompletedBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfhevCompletedBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputData = inputData as IMultistepBusInputDataProvider;
			if (multistageInputData == null) {
				throw new ArgumentException($"inputData must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetAirdragType().GetXmlType(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle, multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData),
				_mrfFactory.GetHEVCompletedBusAuxType().GetElement(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	internal class MrfpevCompletedBusComponentsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfpevCompletedBusComponentsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var multistageInputData = inputData as IMultistepBusInputDataProvider;
			if (multistageInputData == null) {
				throw new ArgumentException($"inputData must implement {nameof(IMultistepBusInputDataProvider)}");
			}
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetAirdragType().GetXmlType(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle, multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.AirdragInputData),
				_mrfFactory.GetPEVCompletedBusAuxType().GetElement(multistageInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries)
			);
		}

		#endregion
	}

	#endregion
}
