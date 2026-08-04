using System.Diagnostics;
using System.IO;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML.XMLComponentInputTest
{
    [TestFixture]
    public class XMLAirdragComponentTest
    {
        private StandardKernel _kernel;

        private string BASEDIRComponent = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.6";

        private IDeclarationInjectFactory _declarationFactory;

        [SetUp]
        public void Setup()
        {
            _kernel = new StandardKernel(new VectoNinjectModule());
            _declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
        }

        [TestCase("Airdrag_normal.xml", 4.06, TestName = "v26_XMLAirDragData_normal")]
        [TestCase("Airdrag_bad_delta.xml", 4.03, TestName = "v26_XMLAirDragData_bad_delta")]
        [TestCase("Airdrag_empty_delta.xml", 4.03, TestName = "v26_XMLAirDragData_empty_delta")]
        [TestCase("Airdrag_missing_CFD.xml", 4.05, TestName = "v26_XMLAirDragData_missing_CFD")]
        public void BatterySystemInternalResistanceTest(string fileName, double airdragArea)
        {
            var path = GetFullPath(BASEDIRComponent, fileName);
            var document = LoadAndValidate(path);
            Assert.IsNotNull(document);
            TestContext.WriteLine(document);

            var reader = CreateAirdragReader(document, path);

            Assert.AreEqual(true, reader.AirDragArea.IsEqual(airdragArea));
        }

        [DebuggerStepThrough]
        public string GetFullPath(string baseDir, string fileName)
        {
            return Path.GetFullPath((Path.Combine(baseDir, fileName)));
        }

        public XmlDocument LoadAndValidate(string filePath)
        {
            Assert.IsTrue(File.Exists(filePath), $"{filePath} missing");
            var document = new XmlDocument();
            using (var reader = XmlReader.Create(filePath))
            {
                document.Load(reader);
            }
            var xmlValidator = new XMLValidator(document, null, XMLValidator.CallBackExceptionOnError);
            Assert.IsTrue(xmlValidator.ValidateXML(XmlDocumentType.DeclarationComponentData));
            return document;
        }

        IXMLAirdragDeclarationInputData CreateAirdragReader(XmlDocument document, string source)
        {
            var componentNode = document.FirstChild.NextSibling.SelectSingleNode($"./*[local-name()='AirDrag']");
            Assert.NotNull(componentNode);
            var dataNode = componentNode.SelectSingleNode($"./*[local-name()='Data']");
            Assert.NotNull(dataNode);
            var version = XMLHelper.GetXsdType(dataNode.SchemaInfo.SchemaType);
            var input = _declarationFactory.CreateAirdragData(version, null, componentNode, source);
            Assert.NotNull(input);
            return input;
        }
    }
}
