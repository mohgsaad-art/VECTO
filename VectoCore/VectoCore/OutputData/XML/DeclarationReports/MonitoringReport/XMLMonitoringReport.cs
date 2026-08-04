/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.MonitoringReport
{
    public class XMLMonitoringReport : IXMLMonitoringReport
    {
        protected static readonly XNamespace _tns = XMLDefinitions.MONITORING_NAMESPACE_URI;
        
        protected const string MRF_OUTPUT_PREFIX = "m";
        
        protected readonly IXMLManufacturerReport _manufacturerReport;

        protected XElement _additionalFields;

        protected string _outputType;

        protected VectoRunData _modelData;

        private Dictionary<ArchitectureID, Action<IAxlePowertrainDeclarationInputData>> _powertrainDataWriters;

        protected enum OutputType {
            ConventionalLorryDataType,
            ConventionalPrimaryBusDataType,
            ConventionalCompletedBusDataType,
            HEV_Px_IHPC_LorryDataType,
            HEV_Px_IHPC_PrimaryBusDataType,
            HEV_S2_LorryDataType,
            HEV_S2_PrimaryBusDataType,
            HEV_S3_LorryDataType,
            HEV_S3_PrimaryBusDataType,
            HEV_S4_LorryDataType,
            HEV_S4_PrimaryBusDataType,
            HEV_IEPC_S_LorryDataType,
            HEV_IEPC_S_PrimaryBusDataType,
            HEVCompletedBusDataType,
            PEV_E2_LorryDataType,
            PEV_E2_PrimaryBusDataType,
            PEV_E3_LorryDataType,
            PEV_E3_PrimaryBusDataType,
            PEV_E4_LorryDataType,
            PEV_E4_PrimaryBusDataType,
            PEV_IEPC_LorryDataType,
            PEV_IEPC_PrimaryBusDataType,
            PEVCompletedBusDataType,
			FCHV_F2_LorryDataType,
			FCHV_F2_PrimaryBusDataType,
			FCHV_F3_LorryDataType,
			FCHV_F3_PrimaryBusDataType,
			FCHV_F4_LorryDataType,
			FCHV_F4_PrimaryBusDataType,
			FCHV_IEPC_F_LorryDataType,
			FCHV_IEPC_F_PrimaryBusDataType,
			FCHVCompletedBusDataType,
			ExemptedLorryDataType,
            ExemptedPrimaryBusDataType,
            ExemptedCompletedBusDataType,
            FCHV_Multiple_Fx_LorryDataType,
            FCHV_Multiple_Fx_PrimaryBusDataType,
            PEV_Multiple_Ex_LorryDataType,
            PEV_Multiple_Ex_PrimaryBusDataType,
            HEV_Multiple_Sx_LorryDataType,
            HEV_Multiple_Sx_PrimaryBusDataType
        }

        protected Dictionary<OutputType, Action> _additionalDataWriters;

        protected enum PlaceHolder {
            VEHICLE_MAKE,
            VECTO_LICENSE_NUMBER,
            ENGINE,
            GEARBOX,
            AXLEGEAR,
            TYRE,
            MANUFACTURER,
            MANUFACTURERADDRESS,
            MAKE,
            TECHNOLOGY_BRAND_NAME,
            TYPE_APPROVAL_NUMBER,
            ELECTRIC_MACHINE,
            ELECTRIC_ENERGY_STORAGE,
            ELECTRIC_MACHINE_GEN,
            FUEL_CELL
        }

        public XMLMonitoringReport(IXMLManufacturerReport manufacturerReport)
        {
            _manufacturerReport = manufacturerReport;

            _powertrainDataWriters = new Dictionary<ArchitectureID, Action<IAxlePowertrainDeclarationInputData>>()
            {
                { ArchitectureID.E2, WriteEM2_Powertrain_Data },
                { ArchitectureID.E3, WriteEM3_Powertrain_Data },
                { ArchitectureID.E4, WriteEM4_Powertrain_Data },
                { ArchitectureID.E_IEPC, WriteIEPC_Powertrain_Data },
                { ArchitectureID.F2, WriteEM2_Powertrain_Data },
                { ArchitectureID.F3, WriteEM3_Powertrain_Data },
                { ArchitectureID.F4, WriteEM4_Powertrain_Data },
                { ArchitectureID.F_IEPC, WriteIEPC_Powertrain_Data },
                { ArchitectureID.S2, WriteEM2_Powertrain_Data },
                { ArchitectureID.S3, WriteEM3_Powertrain_Data },
                { ArchitectureID.S4, WriteEM4_Powertrain_Data },
                { ArchitectureID.S_IEPC, WriteIEPC_Powertrain_Data },
            };

            _additionalDataWriters = new Dictionary<OutputType, Action>() 
            {
                { OutputType.ConventionalLorryDataType, WriteConventional_Data },
                { OutputType.ConventionalPrimaryBusDataType, WriteConventional_Data },
                { OutputType.ConventionalCompletedBusDataType, WriteCompleted_Data },
                { OutputType.HEV_Px_IHPC_LorryDataType, WriteHEV_Px_IHPC_Data },
                { OutputType.HEV_Px_IHPC_PrimaryBusDataType, WriteHEV_Px_IHPC_Data },
                { OutputType.HEV_S2_LorryDataType, WriteHEV_S2_Data },
                { OutputType.HEV_S2_PrimaryBusDataType, WriteHEV_S2_Data },
                { OutputType.HEV_S3_LorryDataType, WriteHEV_S3_Data },
                { OutputType.HEV_S3_PrimaryBusDataType, WriteHEV_S3_Data },
                { OutputType.HEV_S4_LorryDataType, WriteHEV_S4_Data },
                { OutputType.HEV_S4_PrimaryBusDataType, WriteHEV_S4_Data },
                { OutputType.HEV_IEPC_S_LorryDataType, WriteHEV_IEPC_S_Data },
                { OutputType.HEV_IEPC_S_PrimaryBusDataType, WriteHEV_IEPC_S_Data },
                { OutputType.HEVCompletedBusDataType, WriteCompleted_Data },
                { OutputType.PEV_E2_LorryDataType, WritePEV_E2_Data },
                { OutputType.PEV_E2_PrimaryBusDataType, WritePEV_E2_Data },
                { OutputType.PEV_E3_LorryDataType, WritePEV_E3_Data },
                { OutputType.PEV_E3_PrimaryBusDataType, WritePEV_E3_Data },
                { OutputType.PEV_E4_LorryDataType, WritePEV_E4_Data },
                { OutputType.PEV_E4_PrimaryBusDataType, WritePEV_E4_Data },
                { OutputType.PEV_IEPC_LorryDataType, WritePEV_IEPC_Data },
                { OutputType.PEV_IEPC_PrimaryBusDataType, WritePEV_IEPC_Data },
                { OutputType.PEVCompletedBusDataType, WriteCompleted_Data },
				{ OutputType.FCHV_F2_LorryDataType, WriteFCHV_F2_Data },
				{ OutputType.FCHV_F2_PrimaryBusDataType, WriteFCHV_F2_Data },
				{ OutputType.FCHV_F3_LorryDataType, WriteFCHV_F3_Data },
				{ OutputType.FCHV_F3_PrimaryBusDataType, WriteFCHV_F3_Data },
				{ OutputType.FCHV_F4_LorryDataType, WriteFCHV_F4_Data },
				{ OutputType.FCHV_F4_PrimaryBusDataType, WriteFCHV_F4_Data },
				{ OutputType.FCHV_IEPC_F_LorryDataType, WriteFCHV_IEPC_Data },
                { OutputType.FCHV_IEPC_F_PrimaryBusDataType, WriteFCHV_IEPC_Data },
                { OutputType.FCHVCompletedBusDataType, WriteCompleted_Data },
                { OutputType.ExemptedLorryDataType, WriteExempted_Data },
                { OutputType.ExemptedPrimaryBusDataType, WriteExempted_Data },
                { OutputType.ExemptedCompletedBusDataType, WriteExempted_Data },
                { OutputType.FCHV_Multiple_Fx_LorryDataType, WriteMultiple_FCHV_Data },
                { OutputType.FCHV_Multiple_Fx_PrimaryBusDataType, WriteMultiple_FCHV_Data },
                { OutputType.PEV_Multiple_Ex_LorryDataType, WriteMultiple_PEV_Data },
                { OutputType.PEV_Multiple_Ex_PrimaryBusDataType, WriteMultiple_PEV_Data },
                { OutputType.HEV_Multiple_Sx_LorryDataType, WriteMultiple_SHEV_Data },
                { OutputType.HEV_Multiple_Sx_PrimaryBusDataType, WriteMultiple_SHEV_Data }
            };

            _additionalFields = new XElement(_tns + XMLNames.MonitoringDataNode);
        }

        public XDocument Report { get; protected set; }

        public void GenerateReport()
        {
            if (ManufacturerReportMissing) {
                return;
            }

            ValidateManufacturerReport();

            DetectOutputType();

            CreateRootNode();
            CreateChildNodes();
        }

        public virtual void Initialize(VectoRunData modelData)
        {
            _modelData = modelData;
        }

        protected OutputType GetOutputType()
        {
            Enum.TryParse<OutputType>(_outputType.Replace('-', '_'), out var outputType);

            return outputType;
        }

        protected void WriteConventional_Data()
        { 
            WriteBaseVehicleData();
            WriteFullConventionalComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WriteCompleted_Data()
        { 
            WriteBaseVehicleData();
            WriteAdvancedReducingTechnologiesForCompletedVehicle();
        }

        protected void WriteExempted_Data()
        {
            WriteBaseVehicleData();
        }

        protected void WriteHEV_Px_IHPC_Data()
        {
            WriteBaseVehicleData();
            WriteFullConventionalComponents();
            WriteEV_ElectricComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WriteHEV_S2_Data()
        {
            WriteBaseVehicleData();
            WriteFullConventionalComponents();
            WriteHEV_Sx_ElectricComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WriteHEV_S3_Data()
        {
            WriteBaseVehicleData();
            WriteNoGearboxConventionalComponents();
            WriteHEV_Sx_ElectricComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WriteHEV_S4_Data()
        {
            WriteBaseVehicleData();
            WriteNoGearboxNoAxlegearConventionalComponents();
            WriteHEV_Sx_ElectricComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WriteHEV_IEPC_S_Data()
        {
            WriteBaseVehicleData();
            WriteNoGearboxOptionalAxlegearConventionalComponents();
            WriteHEV_Sx_ElectricComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WritePEV_E2_Data()
        {
            WriteBaseVehicleData();
            WriteNoEngineConventionalComponents();
            WriteEV_ElectricComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WritePEV_E3_Data()
        {
            WriteBaseVehicleData();
            WriteNoEngineNoGearboxConventionalComponents();
            WriteEV_ElectricComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WritePEV_E4_Data()
        {
            WriteBaseVehicleData();
            WriteAxleWheels();
            WriteEV_ElectricComponents();
            WriteAdvancedReducingTechnologies();
        }

        protected void WritePEV_IEPC_Data()
        {
            WriteBaseVehicleData();
            WriteOptionalAxlegearConventionalComponents();
            WriteEV_ElectricComponents();
            WriteAdvancedReducingTechnologies();
		}
        
        private void WriteMultiple_PEV_Data()
        {
            WriteBaseVehicleData();
            WriteElectricEnergyStorage();
            WritePowertrains();
            WriteAxleWheels();
            WriteAdvancedReducingTechnologies();
        }

        private void WriteMultiple_FCHV_Data()
        {
            WriteBaseVehicleData();
            WriteFuelCellComponent();
            WriteElectricEnergyStorage();
            WritePowertrains();
            WriteAxleWheels();
            WriteAdvancedReducingTechnologies();
        }

        private void WriteMultiple_SHEV_Data()
        {
            WriteBaseVehicleData();
            WriteEngine();
            WriteElectricMachineGenerator();
            WriteElectricEnergyStorage();
            WritePowertrains();
            WriteAxleWheels();
            WriteAdvancedReducingTechnologies();
        }

        private void WritePowertrains()
        {
            var components = (_modelData.InputData is IMultistepBusInputDataProvider multistage)
                ? multistage.JobInputData.PrimaryVehicle.Vehicle.Components
                : _modelData.InputData.JobInputData.Vehicle.Components;

            foreach (var axlePt in components.AxlePowertrainInputData)
            {
                _powertrainDataWriters[axlePt.Architecture](axlePt);
            }
        }

        private void WriteEM2_Powertrain_Data(IAxlePowertrainDeclarationInputData axlePt)
        {
            _additionalFields.Add(
                new XElement(_tns + "Powertrain",
                    new XAttribute("axleNumber", axlePt.AxleNumber),
                    new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "xEV_EM2_Powertrain_DataType"),
                    GetElectricMachine(),
                    GetGearbox(),
                    GetAxlegear()
                )
            );
        }

        private void WriteEM3_Powertrain_Data(IAxlePowertrainDeclarationInputData axlePt)
        {
            _additionalFields.Add(
                new XElement(_tns + "Powertrain",
                    new XAttribute("axleNumber", axlePt.AxleNumber),
                    new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "xEV_EM3_Powertrain_DataType"),
                    GetElectricMachine(),
                    GetAxlegear()
                )
            );
        }

        private void WriteEM4_Powertrain_Data(IAxlePowertrainDeclarationInputData axlePt)
        {
            _additionalFields.Add(
                new XElement(_tns + "Powertrain",
                    new XAttribute("axleNumber", axlePt.AxleNumber),
                    new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "xEV_EM4_Powertrain_DataType"),
                    GetElectricMachine()
                )
            );
        }

        private void WriteIEPC_Powertrain_Data(IAxlePowertrainDeclarationInputData axlePt)
        {
            _additionalFields.Add(
                new XElement(_tns + "Powertrain",
                    new XAttribute("axleNumber", axlePt.AxleNumber),
                    new XAttribute(AbstractManufacturerReport.XSI + XMLNames.XSIType, "xEV_IEPC_Powertrain_DataType"),
                    GetElectricMachine(),
                    (axlePt.AxleGearInputData != null) ? GetAxlegear() : null
                )
            );
        }

        protected void WriteFCHV_F2_Data()
		{
			WriteBaseVehicleData();
			WriteNoEngineConventionalComponents();
			WriteEV_ElectricComponents();
            WriteFuelCellComponent();
            WriteAdvancedReducingTechnologies();
		}

		protected void WriteFCHV_F3_Data()
		{
			WriteBaseVehicleData();
			WriteNoEngineNoGearboxConventionalComponents();
			WriteEV_ElectricComponents();
            WriteFuelCellComponent();
            WriteAdvancedReducingTechnologies();
		}

		protected void WriteFCHV_F4_Data()
		{
			WriteBaseVehicleData();
			WriteAxleWheels();
			WriteEV_ElectricComponents();
            WriteFuelCellComponent();
            WriteAdvancedReducingTechnologies();
		}

		protected void WriteFCHV_IEPC_Data()
		{
			WriteBaseVehicleData();
			WriteOptionalAxlegearConventionalComponents();
			WriteEV_ElectricComponents();
            WriteFuelCellComponent();
            WriteAdvancedReducingTechnologies();
		}

		protected void WriteBaseVehicleData()
        { 
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringLicenseNumber, GetPlaceholder(PlaceHolder.VECTO_LICENSE_NUMBER)),
                new XElement(_tns + XMLNames.MonitoringVehicle,
                    new XElement(_tns + XMLNames.MonitoringMake, GetPlaceholder(PlaceHolder.VEHICLE_MAKE))
                )
            );
        }

        private void WriteGearbox()
        {
            _additionalFields.Add(GetGearbox());
        }

        private XElement GetGearbox()
        {
            return new XElement(_tns + XMLNames.MonitoringGearbox, GetStandardFields(PlaceHolder.GEARBOX.ToString()));
        }

        private void WriteAxlegear()
        {
            _additionalFields.Add(GetAxlegear());
        }

        private XElement GetAxlegear()
        {
            return new XElement(_tns + XMLNames.MonitoringAxlegear, GetStandardFields(PlaceHolder.AXLEGEAR.ToString()));
        }

        protected void WriteFullConventionalComponents()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringEngine, GetEngineData()),
                new XElement(_tns + XMLNames.MonitoringGearbox, GetStandardFields(PlaceHolder.GEARBOX.ToString())),
                new XElement(_tns + XMLNames.MonitoringAxlegear, GetStandardFields(PlaceHolder.AXLEGEAR.ToString())),
                new XElement(_tns + XMLNames.MonitoringAxleWheels, GetAxleData())
            );
        }

        protected void WriteNoEngineConventionalComponents()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringGearbox, GetStandardFields(PlaceHolder.GEARBOX.ToString())),
                new XElement(_tns + XMLNames.MonitoringAxlegear, GetStandardFields(PlaceHolder.AXLEGEAR.ToString())),
                new XElement(_tns + XMLNames.MonitoringAxleWheels, GetAxleData())
            );
        }

        protected void WriteNoEngineNoGearboxConventionalComponents()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringAxlegear, GetStandardFields(PlaceHolder.AXLEGEAR.ToString())),
                new XElement(_tns + XMLNames.MonitoringAxleWheels, GetAxleData())
            );
        }

        protected void WriteNoGearboxConventionalComponents()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringEngine, GetEngineData()),
                new XElement(_tns + XMLNames.MonitoringAxlegear, GetStandardFields(PlaceHolder.AXLEGEAR.ToString())),
                new XElement(_tns + XMLNames.MonitoringAxleWheels, GetAxleData())
            );
        }

        protected void WriteNoGearboxNoAxlegearConventionalComponents()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringEngine, GetEngineData()),
                new XElement(_tns + XMLNames.MonitoringAxleWheels, GetAxleData())
            );
        }

        protected void WriteAxleWheels()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringAxleWheels, GetAxleData())
            );
        }

        private void WriteEngine()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringEngine, GetEngineData())
            );
        }

        protected void WriteNoGearboxOptionalAxlegearConventionalComponents()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringEngine, GetEngineData())    
            );

            if (_modelData.AxleGearSinglePwt != null) {
                _additionalFields.Add(
                    new XElement(_tns + XMLNames.MonitoringAxlegear, GetStandardFields(PlaceHolder.AXLEGEAR.ToString()))
                );
            }

            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringAxleWheels, GetAxleData())
            );
        }

        protected void WriteOptionalAxlegearConventionalComponents()
        {
            if (_modelData.AxleGearSinglePwt != null) {
                _additionalFields.Add(
                    new XElement(_tns + XMLNames.MonitoringAxlegear, GetStandardFields(PlaceHolder.AXLEGEAR.ToString()))
                );
            }

            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringAxleWheels, GetAxleData())
            );
        }

        protected void WriteAdvancedReducingTechnologies()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringAdvReducingTech, new XComment(GetReducingTechnologies()))
            );
        }

        protected void WriteAdvancedReducingTechnologiesForCompletedVehicle()
        {
            _additionalFields.Add(
                new XElement(_tns + XMLNames.MonitoringAdvReducingTech, new XComment(GetReducingTechnologies()))
            );
        }

        protected void WriteEV_ElectricComponents()
        {
            WriteElectricMachine();
            WriteElectricEnergyStorage();
        }

        private void WriteElectricMachine()
        {
            _additionalFields.Add(GetElectricMachine());
        }

        private XElement GetElectricMachine()
        {
            return new XElement(_tns + XMLNames.MonitoringElectricMachine, GetStandardFields(PlaceHolder.ELECTRIC_MACHINE.ToString()));
        }

        private void WriteElectricEnergyStorage()
        {
            _additionalFields.Add(
                new XElement(
                    _tns + XMLNames.MonitoringElectricEnergyStorage,
                    GetStandardFields(PlaceHolder.ELECTRIC_ENERGY_STORAGE.ToString())
                )
            );
        }

        private void WriteFuelCellComponent()
        {
            var components = (_modelData.InputData is IMultistepBusInputDataProvider multistage)
                ? multistage.JobInputData.PrimaryVehicle.Vehicle.Components
                : _modelData.InputData.JobInputData.Vehicle.Components;

            for (int fc = 0; fc < components.FuelCellSystem.FuelCellModules.Count; fc++)
            {
                _additionalFields.Add(
                    new XElement(_tns + "FuelCell", GetStandardFields($"{PlaceHolder.FUEL_CELL.ToString()}_{fc + 1}"))
                );
            }
        }

        private void WriteElectricMachineGenerator()
        {
            _additionalFields.Add(
                new XElement(
                    _tns + XMLNames.MonitoringElectricMachineGEN,
                    GetStandardFields(PlaceHolder.ELECTRIC_MACHINE_GEN.ToString())
                )
            );
        }

        protected void WriteHEV_Sx_ElectricComponents()
        {
            WriteEV_ElectricComponents();
            WriteElectricMachineGenerator();
        }

        protected bool ManufacturerReportMissing => _manufacturerReport == null;

        protected void ValidateManufacturerReport()
        {
            var errors = new List<string>();
            
            _manufacturerReport.Report.Validate(
                XMLValidator.GetXMLSchema(XmlDocumentType.ManufacturerReport), 
                (o, e) => { errors.Add(e.Message); }, 
                true);

            if (errors.Count > 0) {
                LogManager.GetLogger(GetType().FullName).Warn(
                    "XML Validation of manufacturer record failed! errors: {0}", 
                    string.Join(Environment.NewLine, errors));
            }
        }

        protected void CreateRootNode()
        { 
            Report = new XDocument();
            
            Report.Add(
                new XElement(
                    _tns + XMLNames.MonitoringRootNode,
                    new XAttribute(XMLNames.XMLNS, _tns),
                    new XAttribute(XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, _outputType),
                    new XAttribute(XNamespace.Xmlns + XMLNames.XSI, XMLDeclarationNamespaces.Xsi.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + XMLNames.DI, XMLDeclarationNamespaces.Di),
                    new XAttribute(XNamespace.Xmlns + MRF_OUTPUT_PREFIX, XMLDefinitions.DECLARATION_OUTPUT_NAMESPACE_URI)
                )
            );
        }

        private bool IsCompletedVehicle()
        {
            return GetOutputType().IsOneOf(
                OutputType.ConventionalCompletedBusDataType,
                OutputType.ExemptedCompletedBusDataType,
                OutputType.FCHVCompletedBusDataType,
                OutputType.HEVCompletedBusDataType,
                OutputType.PEVCompletedBusDataType
            );
        }

        protected void CreateChildNodes()
        {
            var vehicleMonitoringData = (_modelData.InputData is IMultistepBusInputDataProvider multistage)
                ? IsCompletedVehicle() 
                    ? _modelData.MultistageVifInputData?.MonitoringData 
                    : multistage.JobInputData.PrimaryVehicle.Vehicle?.VehicleMonitoringData
                : _modelData.InputData.JobInputData.Vehicle?.VehicleMonitoringData;

            if (vehicleMonitoringData == null)
            {
                _additionalDataWriters[GetOutputType()].Invoke();
            }
            else
            {
                CopyMonitoringDataFromInput(vehicleMonitoringData);
            }

            Report.Root.Add(
                new XElement(_tns + XMLNames.ManufacturerRecord,
                    GetManufacturerData()
                ),
                _additionalFields
            );
        }

        private void CopyMonitoringDataFromInput(string vehicleMonitoringData)
        {
            var document = XDocument.Parse(vehicleMonitoringData);

            foreach (var node in document.Root.Descendants())
            {
                node.Name = _tns + node.Name.LocalName;
            }

            foreach (var node in document.Root.Elements())
            {
                _additionalFields.Add(node);
            }
        }

        protected void DetectOutputType()
        {
            var mrfData = _manufacturerReport.Report.XPathSelectElement(XMLHelper.QueryLocalName("Data"));
            string dataType = mrfData.Attributes().First(x => x.Name.LocalName == XMLDefinitions.XSI_TYPE_LOCALNAME).Value;

            if ((dataType.Contains("PEV-Ex-IEPC") || dataType.Contains("FCHV") || dataType.Contains("HEV-Sx")) && !dataType.Contains("Completed"))
            {
                var mrfComponents = _manufacturerReport.Report.XPathSelectElement(XMLHelper.QueryLocalName("Data", "Components"));
                string componentsType = mrfComponents.Attributes().First(x => x.Name.LocalName == XMLDefinitions.XSI_TYPE_LOCALNAME).Value;
                _outputType = componentsType.Replace("Components", "Data");
            }
            else
            {
                _outputType = dataType.Substring(0, dataType.Length - XMLNames.MRFDataTypeSuffix.Length) + XMLNames.MonitoringDataTypeSuffix;
            }
        }

        protected static string GetPlaceholder(string item)
        {
            return $"##{item}##";
        }

        protected static string GetPlaceholder(PlaceHolder item)
        { 
            return GetPlaceholder(item.ToString());
        }

        protected object[] GetEngineData()
        {
            var elements = new List<object>() { GetStandardFields(PlaceHolder.ENGINE.ToString()) };

            foreach (var fuel in _modelData.EngineData.Fuels)
            {
                elements.Add(
                    new XElement(
                        _tns + XMLNames.MonitoringWHTC,
                        new XElement(_tns + XMLNames.MonitoringFuelType, fuel.FuelData.FuelType.ToXMLFormat()),
                        new XElement(_tns + XMLNames.MonitoringCO2, double.NaN.ValueAsUnit(XMLNames.GramsPerKWattHour, 0)),
                        new XElement(_tns + XMLNames.MonitoringFuelConsumption, double.NaN.ValueAsUnit(XMLNames.GramsPerKWattHour, 0))
                    )
                );
            }

            foreach (var fuel in _modelData.EngineData.Fuels)
            {
                elements.Add(
                    new XElement(
                        _tns + XMLNames.MonitoringWHSC,
                        new XElement(_tns + XMLNames.MonitoringFuelType, fuel.FuelData.FuelType.ToXMLFormat()),
                        new XElement(_tns + XMLNames.MonitoringCO2, double.NaN.ValueAsUnit(XMLNames.GramsPerKWattHour, 0)),
                        new XElement(_tns + XMLNames.MonitoringFuelConsumption, double.NaN.ValueAsUnit(XMLNames.GramsPerKWattHour, 0))
                    )
                );
            }

            elements.Add(new XElement(_tns + XMLNames.MonitoringTypeApprovalNumber, GetPlaceholder(PlaceHolder.TYPE_APPROVAL_NUMBER)));

            return elements.ToArray();
        }

        protected object[] GetAxleData()
        {
            var components = (_modelData.InputData is IMultistepBusInputDataProvider multistage)
                ? multistage.JobInputData.PrimaryVehicle.Vehicle.Components
                : _modelData.InputData.JobInputData.Vehicle.Components;

            var numAxles = components.AxleWheels?.AxlesDeclaration.Count(x => x.AxleType != AxleType.Trailer) ?? 0;

            var axleData = new object[numAxles];

            for (var i = 0; i < axleData.Length; i++) {
                axleData[i] = new XElement(
                    _tns + XMLNames.MonitoringAxle,
                    new XAttribute(XMLNames.MonitoringAxleNumber, i + 1),
                    new XElement(_tns + XMLNames.MonitoringTyre, GetStandardFields($"{PlaceHolder.TYRE}_{i + 1}")));
            }

            return axleData;
        }

        protected object[] GetManufacturerData()
        {
            var mrfCopy = new XDocument(_manufacturerReport.Report);
            var elements = mrfCopy.Root.XPathSelectElements("./*");
            
            foreach (var element in elements) {
                element.Name = XName.Get(element.Name.LocalName, _tns.NamespaceName);
            }

            var dataDescendants = elements.First(x => x.Name.LocalName == XMLNames.MRFDataNode).DescendantsAndSelf();
            
            foreach (var node in dataDescendants) {
                var attrs = node.Attributes();
                
                var typeAttrs = attrs.Where(x => (x.Name.LocalName == XMLDefinitions.XSI_TYPE_LOCALNAME)
                    && !String.IsNullOrEmpty(x.Name.Namespace.ToString()));

                if (typeAttrs.Count() > 0) {
                    var type = typeAttrs.First();
                    type.Value = MRF_OUTPUT_PREFIX + ":" + type.Value;
                }
            }
           
            return elements.ToArray<object>();
        }

        protected object[] GetStandardFields(string prefix)
        {
            return new[] {
                new XElement(_tns + XMLNames.MonitoringManufacturer, 
                    GetPlaceholder($"{prefix}_{PlaceHolder.MANUFACTURER}")),
                new XElement(_tns + XMLNames.MonitoringManufacturerAddress, 
                    GetPlaceholder($"{prefix}_{PlaceHolder.MANUFACTURERADDRESS}")),
                new XElement(_tns + XMLNames.MonitoringMake, 
                    GetPlaceholder($"{prefix}_{PlaceHolder.MAKE}"))
            };
        }

        protected string GetReducingTechnologies()
        {
            var categories = new[] {
                XMLNames.MonitoringAdvAero,
                XMLNames.MonitoringAdvRoll,
                XMLNames.MonitoringAdvDrivetrain,
                XMLNames.MonitoringAdvEngine,
                XMLNames.MonitoringAdvAux,
                XMLNames.MonitoringAddADAs,
                XMLNames.MonitoringAdvPowertrain,
                XMLNames.MonitoringOtherTech
            };

            var retVal = new object[categories.Length];
            
            for (var i = 0; i < retVal.Length; i++) {
                retVal[i] = new XElement(
                    XMLNames.MonitoringEntry,
                    new XAttribute(XMLNames.MonitoringCategory, categories[i]),
                    GetPlaceholder(PlaceHolder.TECHNOLOGY_BRAND_NAME));
            }

            return Environment.NewLine + string.Join(
                Environment.NewLine, 
                retVal.Select(x => x.ToString())) + Environment.NewLine;
        }

    }
}
