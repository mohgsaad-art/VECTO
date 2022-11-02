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

using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces {
	public interface IXMLEngineeringComponentWriter
	{
		IXMLEngineeringWriter Writer { set; }

		XAttribute GetXMLTypeAttribute();

		object[] WriteXML(IComponentInputData inputData);

		object[] WriteXML(IDriverModelData inputData);

		object[] WriteXML(IEngineeringInputDataProvider inputData);

		object[] WriteXML(ITransmissionInputData inputData, int gearNumber);
		
		object[] WriteXML(IAxleEngineeringInputData axle, int idx, Meter dynamicTyreRadius);
		object[] WriteXML(IAuxiliaryEngineeringInputData inputData);

		object[] WriteXML(IAdvancedDriverAssistantSystemsEngineering inputData);
	}


	public interface IXMLEngineeringDriverDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLLookaheadDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLOverspeedDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLAccelerationDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLGearshiftDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLVehicleDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringGearWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringTorqueconverterWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAxlegearWriter :IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringRetarderWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAirdragWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAxlesWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAxleWriter : IXMLEngineeringComponentWriter { }

	public interface IXmlEngineeringTyreWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAngledriveWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLAuxiliariesWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLAuxiliaryWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringADASWriter : IXMLEngineeringComponentWriter { }



}