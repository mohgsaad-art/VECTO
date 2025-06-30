using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public interface IXmlMultistepTypeWriter
	{
		XElement GetElement(IMultistageVIFInputData inputData);
	}

	public interface IReportMultistepCompletedBusOutputGroup
	{
		IList<XElement> GetElements(IMultistageVIFInputData multiStageInputDataProvider);
	}

	public interface IReportMultistepCompletedBusTypeWriter
	{
		XElement GetElement(IMultistageVIFInputData inputData);

	}

	public interface IVIFReportInterimFactory
	{
		IXMLMultistepIntermediateReport GetInterimVIFReport(VehicleCategory vehicleType,
			VectoSimulationJobType jobType, ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		#region Vehicle

		IXmlMultistepTypeWriter GetConventionalVehicleType();

		IXmlMultistepTypeWriter GetHEVVehicleType();

		IXmlMultistepTypeWriter GetPEVVehicleType();

		IXmlMultistepTypeWriter GetIEPCVehicleType();

		IXmlMultistepTypeWriter GetExemptedVehicleType();

		#endregion

		IReportMultistepCompletedBusOutputGroup GetCompletedBusGeneralParametersGroup();
		IReportMultistepCompletedBusOutputGroup GetCompletedBusParametersGroup();
		IReportMultistepCompletedBusOutputGroup GetCompletedBusPassengerCountGroup();
		IReportMultistepCompletedBusOutputGroup GetCompletedBusDimensionsGroup();

		IReportMultistepCompletedBusTypeWriter GetConventionalInterimComponentsType();

		IReportMultistepCompletedBusTypeWriter GetxEVInterimComponentsType();


		IReportMultistepCompletedBusTypeWriter GetInterimAirdragType();
		IReportMultistepCompletedBusTypeWriter GetInterimConventionalAuxiliariesType();

		IReportMultistepCompletedBusTypeWriter GetInterimxEVAuxiliariesType();

		IVIFFAdasType GetConventionalInterimADASType();

		IVIFFAdasType GetHEVInterimADASType();

		IVIFFAdasType GetPEVInterimADASType();

		IVIFFAdasType GetIEPCInterimADASType();

	}


	public interface IVIFReportFactory
	{
		IXMLVehicleInformationFile GetVIFReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		
		#region Vehicle

		IXmlTypeWriter GetConventionalVehicleType();
		IXmlTypeWriter GetHevIepcSVehicleType();
		IXmlTypeWriter GetHevPxVehicleType();
		IXmlTypeWriter GetHevS2VehicleType();
		IXmlTypeWriter GetHevS3VehicleType();
		IXmlTypeWriter GetHevS4VehicleType();
		IXmlTypeWriter GetHevIepcFVehicleType();
        IXmlTypeWriter GetMultipleFCHVVehicleType();
        IXmlTypeWriter GetMultiplePEVVehicleType();
        IXmlTypeWriter GetMultipleSHEVVehicleType();
        IXmlTypeWriter GetHevF2VehicleType();
		IXmlTypeWriter GetHevF3VehicleType();
		IXmlTypeWriter GetHevF4VehicleType();
		IXmlTypeWriter GetPevE2VehicleType();
		IXmlTypeWriter GetPevE3VehicleType();
		IXmlTypeWriter GetPevE4VehicleType();

		IXmlTypeWriter GetPevIEPCVehicleType();

		IXmlTypeWriter GetExemptedVehicleType();


		#endregion

		#region Componenet Group

		IXmlTypeWriter GetConventionalComponentType();
		IXmlTypeWriter GetHevIepcSComponentVIFType();
		IXmlTypeWriter GetHevPxComponentVIFType();
		IXmlTypeWriter GetHevS2ComponentVIFType();
		IXmlTypeWriter GetHevS3ComponentVIFType();
		IXmlTypeWriter GetHevS4ComponentVIFType();
		IXmlTypeWriter GetHevF2ComponentVIFType();
		IXmlTypeWriter GetHevF3ComponentVIFType();
		IXmlTypeWriter GetHevF4ComponentVIFType();
		IXmlTypeWriter GetPevE2ComponentVIFType();
		IXmlTypeWriter GetPevE3ComponentVIFType();
		IXmlTypeWriter GetPevE4ComponentVIFType();
		IXmlTypeWriter GetPevIEPCComponentVIFType();
		IXmlTypeWriter GetHevIepcFComponentVIFType();

		#endregion


		#region Components

		IVIFFAdasType GetConventionalADASType();
		IVIFFAdasType GetHEVADASType();
		IVIFFAdasType GetPEVADASType();
		IVIFFAdasType GetIEPCADASType();

		IXmlTypeWriter GetAngelDriveType();

		IXmlTypeWriter GetRetarderType();
		IXmlTypeWriter GetAuxiliaryType();
		IXmlTypeWriter GetAuxiliaryHevSType();
		IXmlTypeWriter GetAuxiliaryHevFType();

		IXmlTypeWriter GetAuxiliaryIEPC_SType();

		IXmlTypeWriter GetAuxiliaryHevPType();
		IXmlTypeWriter GetAuxiliaryIEPCType();
		IXmlTypeWriter GetAuxiliaryPEVType();
		IXmlTypeWriter GetAxlegearType();
		IXmlTypeWriter GetAxleWheelsType();
		IXmlTypeWriter GetBoostingLimitationsType();
		IXmlTypeWriter GetElectricEnergyStorageType();
		IXmlTypeWriter GetElectricMachineGENType();
		IXmlElectricMachineSystemType GetElectricMachineSystemType();
		IXmlTypeWriter GetElectricMachineType();
		IXmlTypeWriter GetElectricMotorTorqueLimitsType();
		IXmlTypeWriter GetEngineType();
		IXmlTypeWriter GetTorqueConvertType();
		IXmlTypeWriter GetIepcType();
		IXmlTypeWriter GetTorqueLimitsType();
		IXmlTypeWriter GetTransmissionType();
		IXmlTypeWriter GetFuelCellType();

		#endregion


		#region Parameter Groups

		IReportOutputGroup GetConventionalVehicleGroup();
		IReportOutputGroup GetPrimaryBusGeneralParameterGroup();
		IReportOutputGroup GetPrimaryBusChassisParameterGroup();
		IReportOutputGroup GetPrimaryBusRetarderParameterGroup();
		IReportOutputGroup GetPrimaryBusXevParameterGroup();

		IReportOutputGroup GetHevIepcSVehicleParameterGroup();
		IReportOutputGroup GetHevIepcFVehicleParameterGroup();
		IReportOutputGroup GetHevSxVehicleParameterGroup();
		IReportOutputGroup GetPevExVehicleParmeterGroup();
		IReportOutputGroup GetPevIEPCVehicleParmeterGroup();
		IReportOutputGroup GetHevPxVehicleParameterGroup();

		IReportOutputGroup GetExemptedVehicleParameterGroup();

		#endregion

	}
}
