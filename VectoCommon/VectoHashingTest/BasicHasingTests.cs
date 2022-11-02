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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using TUGraz.VectoHashing.Impl;
using TUGraz.VectoHashing.Util;
using NUnit.Framework;

namespace VectoHashingTest
{
	[TestFixture]
	//[Parallelizable(ParallelScope.All)]
	public class BasicHasingTests
	{
		public const string SimpleXML = @"Testdata\XML\simple_document.xml";
		public const string ReferenceXMLEngine = @"Testdata\XML\Reference\vecto_engine-sample.xml";
		public const string ReferenceXMLVehicle = @"Testdata\XML\Reference\vecto_vehicle-sample_FULL.xml";

		public const string UnorderedXMLVehicle = @"Testdata\XML\Variations\vecto_vehicle-sample_FULL_Entry_Order.xml";

		public const string HashSimpleXML = "U2zic7KOnKw60rzh+KKQ1lwZL6NmXju+DXG7cYYmlxo=";

		public const string HashEngineXML = "cfPKB2LkHIbznFA9aQwCNfNLSj9V7qNnSskyOxaXB+o=";
		public const string HashVehicleXML = "k029AO90zxKbTybDrvUlCFszdynJot8S1Y+U5lVUG18=";

		public string[] Canonicalization;
		public string DigestAlgorithm;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			Canonicalization = new[] { XMLHashProvider.VectoDsigTransform, XMLHashProvider.DsigExcC14NTransform };
			DigestAlgorithm = XMLHashProvider.DigestMethodSha256;
		}

		[TestCase]
		public void HashSimpleXml()
		{
			var elementToHash = "elemID";
			var doc = new XmlDocument();
			doc.Load(SimpleXML);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "simple_document_hashed.xml");


			Assert.AreEqual(HashSimpleXML, hash);
		}

		[TestCase]
		public void HashReferenceEngineXML()
		{
			var elementToHash = "ENG-gooZah3D";
			var doc = new XmlDocument();
			doc.Load(ReferenceXMLEngine);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_engine_hashed.xml");


			Assert.AreEqual(HashEngineXML, hash);
		}

		[TestCase]
		public void HashReferenceVehicleXML()
		{
			var elementToHash = "VEH-1234567890";
			var doc = new XmlDocument();
			doc.Load(ReferenceXMLVehicle);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_vehicle_hashed.xml");


			Assert.AreEqual(HashVehicleXML, hash);
		}

		[TestCase]
		public void HashUnorderedVehicleXML()
		{
			var elementToHash = "VEH-1234567890";
			var doc = new XmlDocument();
			doc.Load(UnorderedXMLVehicle);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_vehicle_hashed.xml");


			Assert.AreEqual(HashVehicleXML, hash);
		}

		private static string GetHashValue(XmlDocument hashed, string elementToHash)
		{
			var xdoc = hashed.ToXDocument();
			var hash = xdoc.XPathSelectElement("//*[@URI='#" + elementToHash + "']/*[local-name() = 'DigestValue']").Value;
			return hash;
		}

		private static void WriteSignedXML(XmlDocument doc, string filename)
		{
			var xmltw = new XmlTextWriter(filename, new UTF8Encoding(false));
			doc.WriteTo(xmltw);
			xmltw.Close();
		}
	}
}
