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

using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringVehicleComponentsDataProviderV07 : AbstractEngineeringXMLComponentDataProvider, IXMLEngineeringVehicleComponentsData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "VehicleComponentsType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected IAirdragEngineeringInputData _airdragInputData;
		protected IGearboxEngineeringInputData _gearboxInputData;
		protected IAxleGearInputData _axleGearInputData;
		protected IAngledriveInputData _angledriveInputData;
		protected IEngineEngineeringInputData _engineInputData;
		protected IRetarderInputData _retarderInputData;
		protected IAuxiliariesEngineeringInputData _auxInputData;
		protected IAxlesEngineeringInputData _axleWheels;
		protected ITorqueConverterEngineeringInputData _torqueConverterInputData;

		public XMLEngineeringVehicleComponentsDataProviderV07(
			IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) : base(vehicle, baseNode, source)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IVehicleComponentsEngineering

		public override DataSource DataSource => ((IXMLResource)Vehicle).DataSource;

		public IXMLComponentsReader ComponentReader { protected get; set; }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		public virtual IAirdragEngineeringInputData AirdragInputData => _airdragInputData ?? (_airdragInputData = ComponentReader.AirdragInputData);

		public virtual IGearboxEngineeringInputData GearboxInputData => _gearboxInputData ?? (_gearboxInputData = ComponentReader.GearboxData);

		public virtual ITorqueConverterEngineeringInputData TorqueConverterInputData => _torqueConverterInputData ?? (_torqueConverterInputData = ComponentReader.TorqueConverter);

		public virtual IAxleGearInputData AxleGearInputData => _axleGearInputData ?? (_axleGearInputData = ComponentReader.AxleGearInputData);

		public virtual IAngledriveInputData AngledriveInputData => _angledriveInputData ?? (_angledriveInputData = ComponentReader.AngularGearInputData);

		public virtual IEngineEngineeringInputData EngineInputData => _engineInputData ?? (_engineInputData = ComponentReader.EngineInputData);

		public virtual IAuxiliariesEngineeringInputData AuxiliaryInputData => _auxInputData ?? (_auxInputData = ComponentReader.AuxiliaryData);

		public virtual IRetarderInputData RetarderInputData => _retarderInputData ?? (_retarderInputData = ComponentReader.RetarderInputData);

		public IPTOTransmissionInputData PTOTransmissionInputData => Vehicle;

		public IAxlesEngineeringInputData AxleWheels => _axleWheels ?? (_axleWheels = ComponentReader.AxlesEngineeringInputData);

		public virtual IElectricStorageSystemEngineeringInputData ElectricStorage => null;
		public virtual IElectricMachinesEngineeringInputData ElectricMachines => null;
		public IIEPCEngineeringInputData IEPCEngineeringInputData => null;

		#endregion

		#region Implementation of IXMLResource


		#endregion

	}

	internal class XMLEngineeringVehicleComponentsDataProviderV10 : XMLEngineeringVehicleComponentsDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "VehicleComponentsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringVehicleComponentsDataProviderV10(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) : base(vehicle, baseNode, source) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}
