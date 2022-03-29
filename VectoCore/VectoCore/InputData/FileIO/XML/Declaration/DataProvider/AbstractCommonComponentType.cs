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

using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider {
	public abstract class AbstractCommonComponentType : AbstractXMLResource
	{
		protected AbstractCommonComponentType(XmlNode node, string source) : base(node, source) { }

		public bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public virtual string Manufacturer
		{
			get { return GetString(XMLNames.Component_Manufacturer); }
		}

		public virtual string Model
		{
			get { return GetString(XMLNames.Component_Model); }
		}

		public virtual string Date
		{
			get { return GetString(XMLNames.Component_Date); }
		}

		public virtual CertificationMethod CertificationMethod
		{
			get {
				var certMethod = GetString(XMLNames.Component_Gearbox_CertificationMethod, required:false) ?? GetString(XMLNames.Component_CertificationMethod, required:false);
				return certMethod != null ? EnumHelper.ParseEnum<CertificationMethod>(certMethod) : CertificationMethod.Measured;
			}
		}

		public virtual string CertificationNumber
		{
			get { return GetString(XMLNames.Component_CertificationNumber); }
		}

		public virtual DigestData DigestValue
		{
			get { return new DigestData(GetNode(XMLNames.DI_Signature, required:false)); }
		}
	}
}