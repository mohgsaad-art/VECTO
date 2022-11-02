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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory
{
	public interface IEngineeringInjectFactory
	{
		IXMLEngineeringInputData CreateInputProvider(string version, XmlDocument xmldoc, string fileName);

		IXMLEngineeringInputReader CreateInputReader(
			string version, IXMLEngineeringInputData inputData, XmlNode documentElement);

		IXMLJobDataReader CreateJobReader(
			string version, IXMLEngineeringJobInputData jobData, XmlNode jobNode);


		IXMLDriverDataReader CreateDriverReader(string version, IXMLEngineeringDriverData driverData, XmlNode driverDataNode);

		IXMLComponentsReader CreateComponentReader(string version, IXMLEngineeringVehicleData vehicle, XmlNode componentsNode);

		IXMLAxleReader CreateAxleReader(string version, IXMLEngineeringVehicleData vehicle, XmlNode componentsNode);


		IXMLEngineeringJobInputData CreateJobData(string version, XmlNode node, IXMLEngineeringInputData inputProvider, string fileName);

		IXMLEngineeringDriverData CreateDriverData(string version, IXMLEngineeringInputData inputData,
			XmlNode driverDataNode, string fsBasePath);

		IXMLEngineeringGearshiftData CreateShiftParametersData(string version, XmlNode node);

		IXMLCyclesDataProvider CreateCycleData(string version, IEngineeringJobInputData jobData, XmlNode baseNode, string basePath);

		IXMLEngineeringVehicleData CreateVehicleData(string version, IXMLEngineeringJobInputData jobProvider, XmlNode vehicleNode, string fullFilename);

		IXMLAxleEngineeringData CreateAxleData(string version, XmlNode node, IXMLEngineeringVehicleData vehicle);

		IXMLGearData CreateGearData(string version, XmlNode gearNode, string basePath);

		IXMLAuxiliaryData CreateAuxData(string version, XmlNode node, string basePath);

		IXMLAxlegearData CreateAxlegearData(string version, IXMLEngineeringVehicleData vehicle,
			XmlNode axlegearNode, string fsBasePath);

		IXMLAngledriveData CreateAngledriveData(string version, IXMLEngineeringVehicleData vehicle,
			XmlNode axlegearNode, string fsBasePath);

		IXMLEngineData CreateEngineData(string version, IXMLEngineeringVehicleData vehicle,
			XmlNode vehicleNode, string fsBasePath);

		IXMLRetarderData CreateRetarderData(string version, IXMLEngineeringVehicleData vehicle,
			XmlNode axlegearNode, string fsBasePath);

		IXMLAuxiliairesData CreateAuxiliariesData(string version, IXMLEngineeringVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLGearboxData CreateGearboxData(string version, IXMLEngineeringVehicleData vehicle,
			XmlNode axlegearNode, string fsBasePath);

		IXMLAirdragData CreateAirdragData(string version, IXMLEngineeringVehicleData vehicle,
			XmlNode axlegearNode, string fsBasePath);

		IXMLTorqueconverterData CreateTorqueconverterData(string version, IXMLEngineeringVehicleData vehicle, XmlNode node, string source);

		IXMLAxlesData CreateAxlesData(string version, IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source);

		IXMLTyreData CreateTyre(string version, IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source);

		IXMLEngineData CreateEngineOnlyEngine(string version, XmlNode node, string sourceFile);

		IXMLLookaheadData CreateLookAheadData(string version, IXMLEngineeringDriverData driverData, XmlNode node);

		IXMLOverspeedData CreateOverspeedData(string version, IXMLEngineeringDriverData driverData, XmlNode node);

		IXMLDriverAcceleration CreateAccelerationCurveData(string version, IXMLEngineeringDriverData driverData, XmlNode node);
		IXMLEngineeringVehicleComponentsData CreateComponentData(string version, IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source);
		IXMLAuxiliaryReader CreatAuxiliariesReader(string version, IXMLEngineeringVehicleData vehicle, XmlNode componentsNode);
		IXMLAxlesReader CreateAxlesReader(string version, IXMLEngineeringVehicleData vehicle, XmlNode componentsNode);
		IXMLGearboxReader CreateGearboxReader(string version, IXMLEngineeringVehicleData vehicle, XmlNode componentsNode);

		IXMLEngineeringEngineStopStartData CreateEngineStopStartData(string version, IXMLEngineeringDriverData driverData, XmlNode node);
		IXMLEngineeringEcoRollData CreateEcoRollData(string version, IXMLEngineeringDriverData driverData, XmlNode node);

		IXMLEngineeringPCCInputData CreatePCCData(string version, IXMLEngineeringDriverData driverData, XmlNode node);
	}
}
