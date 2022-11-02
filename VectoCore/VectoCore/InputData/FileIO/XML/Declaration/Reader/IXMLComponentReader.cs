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

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader
{
	public interface IXMLComponentReader
	{
		IVehicleComponentsDeclaration ComponentInputData { get; }
		IAirdragDeclarationInputData AirdragInputData { get; }
		IGearboxDeclarationInputData GearboxInputData { get; }
		IAxleGearInputData AxleGearInputData { get; }
		IAngledriveInputData AngledriveInputData { get; }
		IEngineDeclarationInputData EngineInputData { get; }
		IAuxiliariesDeclarationInputData AuxiliaryData { get; }
		IRetarderInputData RetarderInputData { get; }
		IAxlesDeclarationInputData AxlesDeclarationInputData { get; }
		ITorqueConverterDeclarationInputData TorqueConverterInputData { get; }
		IBusAuxiliariesDeclarationData BusAuxiliariesInputData { get; }
		IElectricMachinesDeclarationInputData ElectricMachines { get; }
		IElectricStorageSystemDeclarationInputData ElectricStorageSystem { get; }
		IIEPCDeclarationInputData IEPCInputData { get; }
	}

	public interface IXMLAxlesReader
	{
		IAxleDeclarationInputData CreateAxle(XmlNode axleNode);
	}

	public interface IXMLAxleReader
	{
		ITyreDeclarationInputData Tyre { get; }
	}

	public interface IXMLGearboxReader
	{
		ITransmissionInputData CreateGear(XmlNode gearNode);
	}

	public interface IXMLAuxiliaryReader
	{
		IAuxiliaryDeclarationInputData CreateAuxiliary(XmlNode auxNode);

	}

	public interface IXMLElectricMachineSystemReader
	{
		IElectricMotorDeclarationInputData CreateElectricMachineSystem(XmlNode electricMachineSystem);
		IADCDeclarationInputData ADCInputData { get; }
	}

	public interface IXMLREESSReader
	{
		IREESSPackInputData CreateREESSInputData(XmlNode storageNode, REESSType reessType);
	}


}
