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
using System.IO;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace TUGraz.VectoHashing
{
	public class XmlDsigVectoTransform : Transform
	{
		//private static readonly Type[] _inputTypes = { typeof(Stream), typeof(XmlDocument), typeof(XmlNodeList) };
		//private static readonly Type[] _outputTypes = { typeof(Stream) };
		//private XmlDocument _doc = new XmlDocument();

		private XmlDsigXsltTransform _transform;

		public XmlDsigVectoTransform()
		{
			Algorithm = "urn:vecto:xml:2017:canonicalization";
			_transform = new XmlDsigXsltTransform();

			XmlDocument doc = new XmlDocument();
			doc.Load(ReadStream("TUGraz.VectoHashing.Resources.XSLT.SortInputData.xslt"));

			_transform.LoadInnerXml(doc.ChildNodes);
		}

		public override void LoadInnerXml(XmlNodeList nodeList) {}

		protected override XmlNodeList GetInnerXml()
		{
			return null;
		}

		public override void LoadInput(object obj)
		{
			_transform.LoadInput(obj);
		}

		public override object GetOutput()
		{
			return _transform.GetOutput();
		}

		public override object GetOutput(Type type)
		{
			return _transform.GetOutput(type);
		}

		public override Type[] InputTypes => _transform.InputTypes;

		public override Type[] OutputTypes => _transform.OutputTypes;

		private static Stream ReadStream(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resource = assembly.GetManifestResourceStream(resourceName);
			if (resource == null) {
				throw new Exception("Resource file not found: " + resourceName);
			}
			return resource;
		}
	}
}
