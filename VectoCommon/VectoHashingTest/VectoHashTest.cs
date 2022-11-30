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
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using NUnit.Framework;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using VectoHashingTest.Utils;
using Assert = NUnit.Framework.Assert;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace VectoHashingTest
{
	

	[TestFixture]
	//[Parallelizable(ParallelScope.All)]
	public class VectoHashTest
	{
		public const string ReferenceXMLEngine = @"Testdata\XML\Reference\vecto_engine-sample.xml";
		public const string ReferenceXMLVehicle = @"Testdata\XML\Reference\vecto_vehicle-sample_FULL.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void TestComponentsEngineFile()
		{
			var h = VectoHash.Load(ReferenceXMLEngine);
			var components = h.GetContainigComponents().ToList();

			Assert.AreEqual(1, components.Count);
			Assert.AreEqual(VectoComponents.Engine, components[0]);
		}

		[TestCase]
		public void TestComponentsVehicleFile()
		{
			var h = VectoHash.Load(ReferenceXMLVehicle);
			var components = h.GetContainigComponents().ToList();

			Assert.AreEqual(10, components.Count);
		}

		[TestCase(ReferenceXMLEngine, VectoComponents.Engine, BasicHasingTests.HashEngineXML)]
		public void TestHashComputationSelected(string file, VectoComponents component, string expectedHash)
		{
			var h = VectoHash.Load(file);
			var hash = h.ComputeHash(component);

			Assert.AreEqual(expectedHash, hash);
		}

		[TestCase(ReferenceXMLVehicle, BasicHasingTests.HashVehicleXML),
		TestCase(ReferenceXMLEngine, BasicHasingTests.HashEngineXML)]
		public void TestHashComputation(string file, string expectedHash)
		{
			var h = VectoHash.Load(file);
			var hash = h.ComputeHash();

			Assert.AreEqual(expectedHash, hash);
		}

		[TestCase(ReferenceXMLEngine)]
		public void TestHashComputationInvalidComponent(string file)
		{
			var h = VectoHash.Load(file);
			AssertHelper.Exception<Exception>(() => h.ComputeHash(VectoComponents.Gearbox), "Component Gearbox not found");
		}

		[TestCase(ReferenceXMLEngine)]
		public void TestReadHashInvalidComponent(string file)
		{
			var h = VectoHash.Load(file);
			AssertHelper.Exception<Exception>(() => h.ReadHash(VectoComponents.Gearbox), "Component Gearbox not found");
		}

		[TestCase(ReferenceXMLVehicle, VectoComponents.Engine, "e0c253b643f7f8f09b963aca4a264d06fbfa599f"),
		TestCase(ReferenceXMLVehicle, VectoComponents.Gearbox, "d14189366134120e08fa3f2c6e3328dd13c08a23")]
		public void TestReadHash(string file, VectoComponents component, string expectedHash)
		{
			var h = VectoHash.Load(file);
			var existingHash = h.ReadHash(component);

			Assert.AreEqual(expectedHash, existingHash);
		}

		[TestCase(ReferenceXMLVehicle, VectoComponents.Tyre, 0, "5074334bb2c090c5e258e9a664f5d19689a3f13d"),
		TestCase(ReferenceXMLVehicle, VectoComponents.Tyre, 1, "6074334bb2c090c5e258e9a664f5d19689a3f13d")]
		public void TestReadHashIdx(string file, VectoComponents component, int index, string expectedHash)
		{
			var h = VectoHash.Load(file);
			var existingHash = h.ReadHash(component, index);

			Assert.AreEqual(expectedHash, existingHash);
		}

		[TestCase(@"Testdata\XML\ToHash\vecto_vehicle-sample_3axle1.xml"),
		TestCase(@"Testdata\XML\ToHash\vecto_vehicle-sample_3axle1_unsortedAxle.xml")]
		public void TestReadTyres1Index(string file)
		{
			var h = VectoHash.Load(file);
			var expectedHash = new[] {
				"5074334bb2c090c5e258e9a664f5d19689a3f13d",
				"6074334bb2c090c5e258e9a664f5d19689a3f13d",
				"6074334bb2c090c5e258e9a664f5d19689a3f13d"
			};

			for (int i = 0; i < expectedHash.Length; i++) {
				var existingHash = h.ReadHash(VectoComponents.Tyre, i);

				Assert.AreEqual(expectedHash[i], existingHash, $"component index {i}");
			}
		}

		[TestCase]
		public void TestReadTyres2Index()
		{
			var file = @"Testdata\XML\ToHash\vecto_vehicle-sample_3axle2.xml";
			var h = VectoHash.Load(file);
			var expectedHash = new[] {
				"5074334bb2c090c5e258e9a664f5d19689a3f13d",
				"5074334bb2c090c5e258e9a664f5d19689a3f13d",
				"6074334bb2c090c5e258e9a664f5d19689a3f13d"
			};

			for (int i = 0; i < expectedHash.Length; i++) {
				var existingHash = h.ReadHash(VectoComponents.Tyre, i);

				Assert.AreEqual(expectedHash[i], existingHash);
			}

			AssertHelper.Exception<Exception>(() => h.ReadHash(VectoComponents.Tyre, 3),
				"index exceeds number of components found! index: 3, #components: 3");
		}

		[TestCase(@"Testdata\XML\ToHash\vecto_vehicle-sample_3axle1.xml"),
		TestCase(@"Testdata\XML\ToHash\vecto_vehicle-sample_3axle1_unsortedAxle.xml")]
		public void TestComputeTyres1Index(string file)
		{
			var h = VectoHash.Load(file);

			var hash1 = h.ComputeHash(VectoComponents.Tyre, 1);
			var hash2 = h.ComputeHash(VectoComponents.Tyre, 2);

			Assert.AreEqual(hash1, hash2);

			AssertHelper.Exception<Exception>(() => h.ComputeHash(VectoComponents.Tyre, 3),
				"index exceeds number of components found! index: 3, #components: 3");
		}

		[TestCase]
		public void TestComputeTyres2Index()
		{
			var file = @"Testdata\XML\ToHash\vecto_vehicle-sample_3axle2.xml";
			var h = VectoHash.Load(file);

			var hash1 = h.ComputeHash(VectoComponents.Tyre, 0);
			var hash2 = h.ComputeHash(VectoComponents.Tyre, 1);

			Assert.AreEqual(hash1, hash2);

			AssertHelper.Exception<Exception>(() => h.ComputeHash(VectoComponents.Tyre, 3),
				"index exceeds number of components found! index: 3, #components: 3");
		}

		[TestCase("vecto_vehicle-sample_FULL_Comments.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_vehicle-sample_FULL_Entry_Order.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_vehicle-sample_FULL_Newlines_Linux_LF.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_vehicle-sample_FULL_Newlines_Mac_CR.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_vehicle-sample_FULL_Newlines_Windows_CRLF.xml", BasicHasingTests.HashVehicleXML),
		TestCase("vecto_engine-sample Encoding ISO 8859-15.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding UTF-8 BOM.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding UTF-8.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding UTF-16 BE BOM.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding UTF-16 LE.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample Encoding windows-1292.xml", BasicHasingTests.HashEngineXML),
		TestCase("vecto_engine-sample_Whitespaces.xml", BasicHasingTests.HashEngineXML),
		]
		public void TestHashComputationVariations(string file, string expectedHash)
		{
			var h = VectoHash.Load(@"Testdata\XML\Variations\" + file);
			var hash = h.ComputeHash();

			Assert.AreEqual(expectedHash, hash);
		}


		[TestCase(@"Testdata\XML\Validation\vecto_engine_valid.xml"),
		TestCase(@"Testdata\XML\Validation\vecto_gearbox_valid.xml")]
		public void TestValidation(string file)
		{
			var h = VectoHash.Load(file);
			Assert.IsTrue(h.ValidateHash());
		}

		[TestCase(@"Testdata\XML\Validation\vecto_engine_invalid.xml"),
		TestCase(@"Testdata\XML\Validation\vecto_gearbox_invalid.xml")]
		public void TestValidationInvalid(string file)
		{
			var h = VectoHash.Load(file);
			Assert.IsFalse(h.ValidateHash());
		}

		[TestCase(VectoComponents.Engine),
		TestCase(VectoComponents.Gearbox),
		]
		public void TestValidationComponentValid(VectoComponents component)
		{
			var file = @"Testdata\XML\Validation\vecto_vehicle_components_valid-engine_gbx.xml";
			var h = VectoHash.Load(file);

			Assert.IsTrue(h.ValidateHash(component));
		}

		[TestCase(VectoComponents.Engine),
		TestCase(VectoComponents.Gearbox),
		TestCase(VectoComponents.Axlegear),
		TestCase(VectoComponents.Angledrive),
		TestCase(VectoComponents.Retarder),
		TestCase(VectoComponents.TorqueConverter),
		TestCase(VectoComponents.Tyre),
		TestCase(VectoComponents.Airdrag),
		]
		public void TestValidationComponentInvalid(VectoComponents component)
		{
			var file = @"Testdata\XML\Validation\vecto_vehicle_components_invalid.xml";
			var h = VectoHash.Load(file);

			Assert.IsFalse(h.ValidateHash(component));
		}

		[TestCase(@"Testdata\XML\ToHash\vecto_engine-input.xml"),
		TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml"),
		TestCase(@"Testdata\XML\ToHash\vecto_gearbox-input.xml")]
		public void TestAddHash(string file)
		{
			var destination = Path.GetFileNameWithoutExtension(file) + "_hashed.xml";

			var h = VectoHash.Load(file);
			var r = h.AddHash();

			var writer = new XmlTextWriter(destination, Encoding.UTF8);
			r.WriteTo(writer);
			writer.Flush();
			writer.Close();

			var h2 = VectoHash.Load(destination);
			Assert.IsTrue(h2.ValidateHash());
		}

		[TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml", 5),
		TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml", 10),
		TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml", 15),
		TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml", 20),
		]
		public void TestAddHashoDoNotOverwriteID(string file, int idLength)
		{
			var newid = "x" + Guid.NewGuid().ToString("n").Substring(0, idLength - 1);
			var input = new XmlDocument();
			input.Load(file);
			var data = input.SelectSingleNode("//*[local-name()='Data']");
			data.Attributes["id"].Value = newid;

			var h = VectoHash.Load(input);
			var r = h.AddHash();

			var id = r.XPathSelectElement("//*[local-name()='Data']");
			Assert.IsNotNull(id.Attribute("id"));
			Assert.AreEqual(newid, id.Attribute("id").Value);
		}

		[TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml", 2),
		TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml", 3),
		TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml", 4)]
		public void TestAddHashoDoOverwriteID(string file, int idLength)
		{
			var newid = "x" + Guid.NewGuid().ToString("n").Substring(0, idLength - 1);
			var input = new XmlDocument();
			input.Load(file);
			var data = input.SelectSingleNode("//*[local-name()='Data']");
			data.Attributes["id"].Value = newid;

			var h = VectoHash.Load(input);
			var r = h.AddHash();

			var id = r.XPathSelectElement("//*[local-name()='Data']");
			Assert.IsNotNull(id.Attribute("id"));
			Assert.AreNotEqual(newid, id.Attribute("id").Value);
		}

		[TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml")]
		public void TestReplaceDate(string file)
		{
			var input = new XmlDocument();
			input.Load(file);
			var dateNode = input.SelectSingleNode("//*[local-name()='Date']");
			var date = XmlConvert.ToDateTime(dateNode.FirstChild.Value, XmlDateTimeSerializationMode.Utc);

			var h = VectoHash.Load(input);
			var r = h.AddHash();

			var newDateNode = r.XPathSelectElement("//*[local-name()='Date']");
			var newDate = XmlConvert.ToDateTime(newDateNode.Value, XmlDateTimeSerializationMode.Utc);

			var now = DateTime.Now;

			Assert.AreNotEqual(date.ToString(), newDate.ToString());
			Assert.IsTrue(now.ToUniversalTime() - date > new TimeSpan(0, 0, 0, 1));
			Assert.IsTrue(now.ToUniversalTime() - newDate < new TimeSpan(0, 0, 0, 1));
		}

		[TestCase(@"Testdata\XML\ToHash\vecto_engine_withhash-input.xml", "input data already contains a signature element"),
		TestCase(@"Testdata\XML\ToHash\vecto_vehicle-sample.xml", "adding hash for Vehicle is not supported"),
		TestCase(@"Testdata\XML\ToHash\vecto_gearbox-input_nodata.xml", "'Data' element for component 'Gearbox' not found!"),
		TestCase(@"Testdata\XML\ToHash\multiple_components.xml", "input must not contain multiple components!"),
		]
		public void TestAddHashException(string file, string expectedExceptionMsg)
		{
			var destination = Path.GetFileNameWithoutExtension(file) + "_hashed.xml";

			var h = VectoHash.Load(file);
			AssertHelper.Exception<Exception>(() => { var r = h.AddHash(); }, expectedExceptionMsg);
		}

		[TestCase]
		public void TestDuplicateSigElement()
		{
			var filename = @"Testdata\XML\Invalid\duplicate-sig.xml";
			var h = VectoHash.Load(filename);

			AssertHelper.Exception<Exception>(() => { var r = h.ReadHash(); }, "Multiple DigestValue elements found!");
		}


		[TestCase()]
		public void TestLoadFromStream()
		{
			var fs = new FileStream(BasicHasingTests.ReferenceXMLVehicle, FileMode.Open);
			var h = VectoHash.Load(fs);

			var hash = h.ComputeHash();
			Assert.AreEqual(BasicHasingTests.HashVehicleXML, hash);
			fs.Close();
		}

		[TestCase(WhitespaceHandling.All),
		TestCase(WhitespaceHandling.None),
		TestCase(WhitespaceHandling.Significant)]
		public void TestLoadXmlDocument(WhitespaceHandling whitespace)
		{
			var xml = new XmlDocument();
			var reader = new XmlTextReader(BasicHasingTests.ReferenceXMLVehicle);
			reader.WhitespaceHandling = whitespace;
			xml.Load(reader);
			var h = VectoHash.Load(xml);

			var hash = h.ComputeHash();
			Assert.AreEqual(BasicHasingTests.HashVehicleXML, hash);
		}

		[TestCase(@"Testdata\XML\ToHash\vecto_engine-input.xml"),
		TestCase(@"Testdata\XML\ToHash\vecto_engine-input_emptyDate.xml"),
		TestCase(@"Testdata\XML\ToHash\vecto_engine_withid-input.xml"),
		TestCase(@"Testdata\XML\ToHash\vecto_gearbox-input.xml")]
		public void TestHashedComponentIsValid(string file)
		{
			var destination = Path.GetFileNameWithoutExtension(file) + "_hashed.xml";

			var h = VectoHash.Load(file);
			var r = h.AddHash();

			var writer = new XmlTextWriter(destination, Encoding.UTF8);
			r.WriteTo(writer);
			writer.Flush();
			writer.Close();

			var h2 = VectoHash.Load(destination);
			Assert.IsTrue(h2.ValidateHash());

			// re-load generated XML and perform XSD validation
			var validator = new XMLValidator(XmlReader.Create(destination));
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.DeclarationComponentData));
		}


		[TestCase("vecto_vehicle-namespace_prefix.xml", BasicHasingTests.HashVehicleXML)]
		public void TestNamespacePrefixVariations(string file, string expectedHash)
		{
			var h = VectoHash.Load(@"Testdata\XML\Variations\" + file);
			var hash = h.ComputeHash();

			Assert.AreEqual(expectedHash, hash);
		}

		[TestCase()]
		public void TestInvalidXMLAsFile()
		{
			var file = @"Testdata\XML\Invalid\invalid-comp.xml";

			AssertHelper.Exception<Exception>(() => VectoHash.Load(file), "failed to read XML document");
		}

		[TestCase()]
		public void TestInvalidXMLAsStream()
		{
			var file = @"Testdata\XML\Invalid\invalid-comp.xml";
			var stream = File.Open(file, FileMode.Open);
			AssertHelper.Exception<Exception>(() => VectoHash.Load(stream), "failed to read XML document");
		}

		[TestCase()]
		public void TestComputeHashNoComponentInXML()
		{
			var xml = @"<VectoInputDeclaration/>";
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(xml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			var h = VectoHash.Load(stream);
			AssertHelper.Exception<Exception>(() => h.ComputeHash(), "No component found");
		}

		[TestCase()]
		public void TestReadHashNoComponentInXML()
		{
			var xml = @"<VectoInputDeclaration/>";
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(xml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			var h = VectoHash.Load(stream);
			AssertHelper.Exception<Exception>(() => h.ReadHash(), "No component found");
		}

		[TestCase(VectoComponents.Engine, "ENG-"),
		TestCase(VectoComponents.Gearbox, "GBX-"),
		TestCase(VectoComponents.Axlegear, "AXL-"),
		TestCase(VectoComponents.Retarder, "RET-"),
		TestCase(VectoComponents.TorqueConverter, "TC-"),
		TestCase(VectoComponents.Angledrive, "ANGL-"),
		TestCase(VectoComponents.Airdrag, "AD-"),
		TestCase(VectoComponents.Tyre, "TYRE-"),
		]
		public void TestIdPrefix(VectoComponents component, string expectedPrefix)
		{
			Assert.AreEqual(expectedPrefix, component.HashIdPrefix());
		}

		[TestCase()]
		public void TestInvalidComponentXMLName()
		{
			AssertHelper.Exception<ArgumentOutOfRangeException>(() => ((VectoComponents)9999).XMLElementName());
		}

		[TestCase()]
		public void TestInvalidComponentPrefix()
		{
			AssertHelper.Exception<ArgumentOutOfRangeException>(() => ((VectoComponents)9999).HashIdPrefix());
		}



		[TestCase(@"Testdata\XML\Validation\vecto_engine_valid.xml"),
		TestCase(@"Testdata\XML\Validation\vecto_gearbox_valid.xml")]
		public void TestXMLComponentValidation(string file)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(XmlReader.Create(file));
			var validator = new XMLValidator(xmlDoc);
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.DeclarationComponentData | XmlDocumentType.DeclarationJobData | XmlDocumentType.CustomerReport | XmlDocumentType.ManufacturerReport));

			var version = XMLHelper.GetVersionFromNamespaceUri(xmlDoc.DocumentElement.NamespaceURI);
			Assert.AreEqual("1.0", version);

		}

		[TestCase(@"Testdata\XML\Validation\vecto_engine_valid.xml"),
		TestCase(@"Testdata\XML\Validation\vecto_engine_valid-typeAttr.xml")]
		public void TestIgnoreTypeAttribute(string file)
		{
			var h = VectoHash.Load(file);
			Assert.IsTrue(h.ValidateHash());
		}

		private const string WheelsFileToHash25 = @"Testdata\XML\ToHash\Tyre_v25.xml";

        [TestCase(WheelsFileToHash25)]
		public void TestTyreValidDimension(string file)
		{
			var validDimensions = DeclarationData.Wheels.GetWheelsDimensions();
			foreach (var dimension in validDimensions) {
				var modified = GetModifiedXML(file, dimension);
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(modified);
				var h = VectoHash.Load(xmlDoc);
				var hashed = h.AddHash();

				var validator = new XMLValidator(XmlReader.Create(new StringReader(hashed.ToString())));
				Assert.IsTrue(validator.ValidateXML(XmlDocumentType.DeclarationComponentData));
			}
		}

		
		[
			TestCase(WheelsFileToHash25, "255/70 R22.5",  "255/70 R22.5"),
			TestCase(WheelsFileToHash25, "255/70 R22.5",  "  255/70 R22.5"),
			TestCase(WheelsFileToHash25, "255/70 R22.5",  "255/70 R22.5  "),
			TestCase(WheelsFileToHash25, "255/70 R22.5", "255/70 R22.5"),
			TestCase(WheelsFileToHash25, "  255/70 R22.5", "255/70 R22.5"),
			TestCase(WheelsFileToHash25, "255/70 R22.5  ", "255/70 R22.5"),
		]
		public void TestTyreValidDimensionVariationHash(string file, string dimension, string dimensionVaried)
		{
			// create XML with dimension string and hash it
			var reference = GetModifiedXML(file, dimension);
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(reference);
			var h = VectoHash.Load(xmlDoc);
			var hashed = h.AddHash();

			// created XML needs to be valid
			var validator = new XMLValidator(XmlReader.Create(new StringReader(hashed.ToString())));
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.DeclarationComponentData));

			// change the dimension string to something else and load the modified XML
			var variation = GetModifiedXML(hashed, dimensionVaried);
			var xmlDocV = new XmlDocument();
			xmlDocV.Load(variation);
			var h2 = VectoHash.Load(xmlDocV);

			// the hash has to be valid
			var variationValid = h2.ValidateHash();
			Assert.IsTrue(variationValid);

			var validatorV = new XMLValidator(xmlDocV);
			Assert.IsTrue(validatorV.ValidateXML(XmlDocumentType.DeclarationComponentData));
		}

		[
			TestCase(WheelsFileToHash25, "255/70 R22.5  ", "255/70  R22.5"), // the hash is still the same but the tyre dimension is invalid
			//TestCase(WheelsFileToHash25, "255/70 R22.5  ", "255/70  R 22.5"),
			//TestCase(WheelsFileToHash25, "255/70 R22.5  ", "255 / 70  R 22.5"),
		]
		public void TestTyreInvalidDimensionVariationHash(string file, string dimension, string dimensionVaried)
		{
			// create XML with dimension string and hash it
			var reference = GetModifiedXML(file, dimension);
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(reference);
			var h = VectoHash.Load(xmlDoc);
			var hashed = h.AddHash();

			// created XML needs to be valid
			var validator = new XMLValidator(XmlReader.Create(new StringReader(hashed.ToString())));
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.DeclarationComponentData));

			// change the dimension string to something else and load the modified XML
			var variation = GetModifiedXML(hashed, dimensionVaried);
			var xmlDocV = new XmlDocument();
			xmlDocV.Load(variation);
			var h2 = VectoHash.Load(xmlDocV);

			// the hash has to be valid
			var variationValid = h2.ValidateHash();
			Assert.IsTrue(variationValid);

			var message = "";
			var validatorV = new XMLValidator(xmlDocV, validationErrorAction: (s, ve) => message = ve.Exception.Message);
			var result = validatorV.ValidateXML(XmlDocumentType.DeclarationComponentData);

			Assert.IsFalse(result);
			Assert.IsTrue(message.Contains("Invalid tyre dimension"));
		}

		[
            TestCase(WheelsFileToHash25, "255/70 R22.5", "255/70  R 22.5"),
            TestCase(WheelsFileToHash25, "255/70 R22.5", "255 / 70  R 22.5"),
        ]
		public void TestTyreIDimensionVariationInvalidHash(string file, string dimension, string dimensionVaried)
		{
			// create XML with dimension string and hash it
			var reference = GetModifiedXML(file, dimension);
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(reference);
			var h = VectoHash.Load(xmlDoc);
			var hashed = h.AddHash();

			// created XML needs to be valid
			var validator = new XMLValidator(XmlReader.Create(new StringReader(hashed.ToString())));
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.DeclarationComponentData));

			// change the dimension string to something else and load the modified XML
			var variation = GetModifiedXML(hashed, dimensionVaried);
			var xmlDocV = new XmlDocument();
			xmlDocV.Load(variation);
			var h2 = VectoHash.Load(xmlDocV);

			// the hash is no longer valid
			var variationValid = h2.ValidateHash();
			Assert.IsFalse(variationValid);

		}

		[
			TestCase(WheelsFileToHash25, "asdf"),
			TestCase(WheelsFileToHash25, "250/70R14XX"),
			TestCase(WheelsFileToHash25, "V425/55 R19.5"),
			TestCase(WheelsFileToHash25, "425 / 55 R 19.5"),
	        TestCase(WheelsFileToHash25, "255 / 70 R22.5"),
	        TestCase(WheelsFileToHash25, "255/70   R22.5"),
	        TestCase(WheelsFileToHash25, "  255/70    R22.5		   "),
	        TestCase(WheelsFileToHash25, "  255/70	R22.5		   "),
	        TestCase(WheelsFileToHash25, "255/70   R22.5"),
		]
		public void TestTyreInValidDimension(string file, string dimension)
		{
			var modified = GetModifiedXML(file, dimension);
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(modified);
			var h = VectoHash.Load(xmlDoc);
			var hashed = h.AddHash();
			string message = "";
			var validator = new XMLValidator(XmlReader.Create(new StringReader(hashed.ToString())),
				validationErrorAction: (s, ve) => message = ve.Exception.Message);
			var result = validator.ValidateXML(XmlDocumentType.DeclarationComponentData);
			
			Assert.IsFalse(result);
			Assert.IsTrue(message.Contains("Invalid tyre dimension"));
		}

		private XmlReader GetModifiedXML(XDocument doc, string dimension)
		{
			var tyreDimension = doc.XPathSelectElement("//*[local-name()='Dimension']");
			Assert.NotNull(tyreDimension);
			tyreDimension.Value = dimension;

			var xmlString = "";
			using (MemoryStream ms = new MemoryStream()) {
				using (XmlWriter xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true })) {
					doc.WriteTo(xw);
					xw.Flush();
				}
				ms.Flush();
				ms.Seek(0, SeekOrigin.Begin);
				using (var reader = new StreamReader(ms)) {
					xmlString = reader.ReadToEnd();
				}
			}

			var modified = XmlReader.Create(new StringReader(xmlString));
			return modified;
		}

		private XmlReader GetModifiedXML(string file, string dimension)
		{
			var inputXml = new XmlDocument();
			inputXml.Load(file);

			var tyreDimension = inputXml.SelectSingleNode("//*[local-name()='Dimension']");
			Assert.NotNull(tyreDimension);
			tyreDimension.InnerText = dimension;
			
			var modified = XmlReader.Create(new StringReader(inputXml.OuterXml));
			return modified;
		}

  //      [TestCase()]
		//public void TestXSLTransform()
		//{
		//	var dimension = "  425 / 55 R 19.5  ";

		//	var modified = GetModifiedXML(WheelsFileToHash25, dimension);
		//	var xmlDoc = new XmlDocument();
		//	xmlDoc.Load(modified);


		//	var transform = new XmlDsigExcC14NTransform();
		//	transform.LoadInput(xmlDoc);

		//	var output = transform.GetOutput() as Stream;
		//	var sr = new StreamReader(output);
			
		//	var xml = sr.ReadToEnd();

		//}
	}
}
