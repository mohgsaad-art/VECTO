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
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	
	public abstract class AbstractCommonComponentType : AbstractXMLResource
	{
		public AbstractCommonComponentType(XmlNode node, string source) : base(node, source) { }

		
		public bool SavedInDeclarationMode => false;

		public string Manufacturer => GetString(XMLNames.Component_Manufacturer);

		public string Model => GetString(XMLNames.Component_Model);

		public DateTime Date => XmlConvert.ToDateTime(GetString(XMLNames.Component_Date), XmlDateTimeSerializationMode.Utc);

		public virtual string AppVersion => GetString(XMLNames.Component_AppVersion);

		public virtual CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public virtual string CertificationNumber => Constants.NOT_AVAILABLE;

		public virtual DigestData DigestValue => null;

		public virtual XmlNode XMLSource => BaseNode;
	}
}
