using System;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Ninject;

namespace TUGraz.VectoCore.Tests.XML
{
    [TestFixture]
    public class XMLDeclarationInputv27
    {
        protected IXMLInputDataReader xmlInputReader;
        private IKernel _kernel;

        private const string BASE_DIR = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.7";
        private const string LORRIES = $"{BASE_DIR}/Lorries/";
        private const string PRIMARYBUSES = $"{BASE_DIR}/PrimaryBuses/";
        private const string COMPLETEDBUSES = $"{BASE_DIR}/CompletedBuses/";

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

            _kernel = new StandardKernel(new VectoNinjectModule());
            xmlInputReader = _kernel.Get<IXMLInputDataReader>();
        }

        [
            TestCase(@"Conventional_HeavyLorry.xml", LORRIES, TestName = "v27_Conventional_HeavyLorry"),
            TestCase(@"Conventional_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Conventional_HeavyLorry_requiredOnly"),
            TestCase(@"Exempted_HeavyLorry.xml", LORRIES, TestName = "v27_Exempted_HeavyLorry"),
            TestCase(@"Exempted_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Exempted_HeavyLorry_requiredOnly"),
            TestCase(@"SHEV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_IEPC_HeavyLorry"),
            TestCase(@"SHEV_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_SHEV_IEPC_HeavyLorry_requiredOnly"),
            TestCase(@"SHEV_S2_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_S2_HeavyLorry"),
            TestCase(@"SHEV_S2_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_SHEV_S2_HeavyLorry_requiredOnly"),
            TestCase(@"SHEV_S3_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_S3_HeavyLorry"),
            TestCase(@"SHEV_S3_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_SHEV_S3_HeavyLorry_requiredOnly"),
            TestCase(@"SHEV_S4_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_S4_HeavyLorry"),
            TestCase(@"SHEV_S4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_SHEV_S4_HeavyLorry_requiredOnly"),
            TestCase(@"SHEV_H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_H2_ICE_HeavyLorry"),
            TestCase(@"PEV_E2_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_E2_HeavyLorry"),
            TestCase(@"PEV_E2_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_PEV_E2_HeavyLorry_requiredOnly"),
            TestCase(@"PEV_E3_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_E3_HeavyLorry"),
            TestCase(@"PEV_E3_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_PEV_E3_HeavyLorry_requiredOnly"),
            TestCase(@"PEV_E4_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_E4_HeavyLorry"),
            TestCase(@"PEV_E4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_PEV_E4_HeavyLorry_requiredOnly"),
            TestCase(@"PEV_E4_stdEM_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_E4_stdEM_HeavyLorry"),
            TestCase(@"PEV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_IEPC_HeavyLorry"),
            TestCase(@"PEV_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_PEV_IEPC_HeavyLorry_requiredOnly"),
            TestCase(@"PEV_IEPC_multiCurve_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_IEPC_multiCurve_HeavyLorry"),
            TestCase(@"PEV_IEPC_stdValues_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_IEPC_stdValues_HeavyLorry"),
            TestCase(@"H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_H2_ICE_HeavyLorry"),
            TestCase(@"HEV_P2_HeavyLorry.xml", LORRIES, TestName = "v27_HEV_P2_HeavyLorry"),
            TestCase(@"HEV_P2_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_HEV_P2_HeavyLorry_requiredOnly"),
            TestCase(@"HEV_P2_supercap_HeavyLorry.xml", LORRIES, TestName = "v27_HEV_P2_supercap_HeavyLorry"),
            TestCase(@"HEV_IHPC_HeavyLorry.xml", LORRIES, TestName = "v27_HEV_IHPC_HeavyLorry"),
            TestCase(@"HEV_H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_HEV_H2_ICE_HeavyLorry"),
            TestCase(@"FCHV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_IEPC_HeavyLorry"),
            TestCase(@"FCHV_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_FCHV_IEPC_HeavyLorry_requiredOnly"),
            TestCase(@"FCHV_IEPC_2xFC_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_IEPC_2xFC_HeavyLorry"),
            TestCase(@"FCHV_F2_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_F2_HeavyLorry"),
            TestCase(@"FCHV_F2_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_FCHV_F2_HeavyLorry_requiredOnly"),
            TestCase(@"FCHV_F3_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_F3_HeavyLorry"),
            TestCase(@"FCHV_F3_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_FCHV_F3_HeavyLorry_requiredOnly"),
            TestCase(@"FCHV_F4_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_F4_HeavyLorry"),
            TestCase(@"FCHV_F4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_FCHV_F4_HeavyLorry_requiredOnly"),
            TestCase(@"Multiple_PEV_E2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_PEV_E2_IEPC_HeavyLorry"),
            TestCase(@"Multiple_PEV_E2_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Multiple_PEV_E2_IEPC_HeavyLorry_requiredOnly"),
            TestCase(@"Multiple_PEV_E3_E4_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_PEV_E3_E4_HeavyLorry"),
            TestCase(@"Multiple_PEV_E3_E4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Multiple_PEV_E3_E4_HeavyLorry_requiredOnly"),
            TestCase(@"Multiple_FCHV_F2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_FCHV_F2_IEPC_HeavyLorry"),
            TestCase(@"Multiple_FCHV_F2_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Multiple_FCHV_F2_IEPC_HeavyLorry_requiredOnly"),
            TestCase(@"Multiple_FCHV_F3_F4_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_FCHV_F3_F4_HeavyLorry"),
            TestCase(@"Multiple_FCHV_F3_F4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Multiple_FCHV_F3_F4_HeavyLorry_requiredOnly"),
            TestCase(@"Multiple_SHEV_S2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_SHEV_S2_IEPC_HeavyLorry"),
            TestCase(@"Multiple_SHEV_S2_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Multiple_SHEV_S2_IEPC_HeavyLorry_requiredOnly"),
            TestCase(@"Multiple_SHEV_S3_S4_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_SHEV_S3_S4_HeavyLorry"),
            TestCase(@"Multiple_SHEV_S3_S4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Multiple_SHEV_S3_S4_HeavyLorry_requiredOnly"),
            TestCase(@"Multiple_SHEV_H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_SHEV_H2_ICE_HeavyLorry"),

            TestCase(@"Conventional_MediumLorry.xml", LORRIES, TestName = "v27_Conventional_MediumLorry"),
            TestCase(@"Conventional_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Conventional_MediumLorry_requiredOnly"),
            TestCase(@"Exempted_MediumLorry.xml", LORRIES, TestName = "v27_Exempted_MediumLorry"),
            TestCase(@"Exempted_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Exempted_MediumLorry_requiredOnly"),
            TestCase(@"H2_ICE_MediumLorry.xml", LORRIES, TestName = "v27_H2_ICE_MediumLorry"),
            TestCase(@"HEV_P2_MediumLorry.xml", LORRIES, TestName = "v27_HEV_P2_MediumLorry"),
            TestCase(@"HEV_P2_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_HEV_P2_MediumLorry_requiredOnly"),
            TestCase(@"SHEV_S2_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_S2_MediumLorry"),
            TestCase(@"SHEV_S2_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_SHEV_S2_MediumLorry_requiredOnly"),
            TestCase(@"SHEV_S3_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_S3_MediumLorry"),
            TestCase(@"SHEV_S3_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_SHEV_S3_MediumLorry_requiredOnly"),
            TestCase(@"SHEV_S4_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_S4_MediumLorry"),
            TestCase(@"SHEV_S4_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_SHEV_S4_MediumLorry_requiredOnly"),
            TestCase(@"SHEV_H2_ICE_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_H2_ICE_MediumLorry"),
            TestCase(@"HEV_IHPC_MediumLorry.xml", LORRIES, TestName = "v27_HEV_IHPC_MediumLorry"),
            TestCase(@"HEV_H2_ICE_MediumLorry.xml", LORRIES, TestName = "v27_HEV_H2_ICE_MediumLorry"),
            TestCase(@"SHEV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_IEPC_MediumLorry"),
            TestCase(@"SHEV_IEPC_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_SHEV_IEPC_MediumLorry_requiredOnly"),
            TestCase(@"PEV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_PEV_IEPC_MediumLorry"),
            TestCase(@"PEV_IEPC_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_PEV_IEPC_MediumLorry_requiredOnly"),
            TestCase(@"PEV_E2_MediumLorry.xml", LORRIES, TestName = "v27_PEV_E2_MediumLorry"),
            TestCase(@"PEV_E2_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_PEV_E2_MediumLorry_requiredOnly"),
            TestCase(@"PEV_E3_MediumLorry.xml", LORRIES, TestName = "v27_PEV_E3_MediumLorry"),
            TestCase(@"PEV_E3_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_PEV_E3_MediumLorry_requiredOnly"),
            TestCase(@"PEV_E4_MediumLorry.xml", LORRIES, TestName = "v27_PEV_E4_MediumLorry"),
            TestCase(@"PEV_E4_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_PEV_E4_MediumLorry_requiredOnly"),
            TestCase(@"FCHV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_FCHV_IEPC_MediumLorry"),
            TestCase(@"FCHV_IEPC_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_FCHV_IEPC_MediumLorry_requiredOnly"),
            TestCase(@"FCHV_F2_MediumLorry.xml", LORRIES, TestName = "v27_FCHV_F2_MediumLorry"),
            TestCase(@"FCHV_F2_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_FCHV_F2_MediumLorry_requiredOnly"),
            TestCase(@"FCHV_F3_MediumLorry.xml", LORRIES, TestName = "v27_FCHV_F3_MediumLorry"),
            TestCase(@"FCHV_F3_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_FCHV_F3_MediumLorry_requiredOnly"),
            TestCase(@"FCHV_F4_MediumLorry.xml", LORRIES, TestName = "v27_FCHV_F4_MediumLorry"),
            TestCase(@"FCHV_F4_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_FCHV_F4_MediumLorry_requiredOnly"),

            TestCase(@"Conventional_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Conventional_PrimaryBus"),
            TestCase(@"Conventional_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Conventional_PrimaryBus_requiredOnly"),
            TestCase(@"H2_ICE_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_H2_ICE_PrimaryBus"),
            TestCase(@"Exempted_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Exempted_PrimaryBus"),
            TestCase(@"Exempted_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Exempted_PrimaryBus_requiredOnly"),
            TestCase(@"PEV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_PEV_IEPC_PrimaryBus"),
            TestCase(@"PEV_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_PEV_IEPC_PrimaryBus_requiredOnly"),
            TestCase(@"PEV_E2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_PEV_E2_PrimaryBus"),
            TestCase(@"PEV_E2_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_PEV_E2_PrimaryBus_requiredOnly"),
            TestCase(@"PEV_E3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_PEV_E3_PrimaryBus"),
            TestCase(@"PEV_E3_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_PEV_E3_PrimaryBus_requiredOnly"),
            TestCase(@"PEV_E4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_PEV_E4_PrimaryBus"),
            TestCase(@"HEV_IHPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_HEV_IHPC_PrimaryBus"),
            TestCase(@"HEV_P2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_HEV_P2_PrimaryBus"),
            TestCase(@"HEV_P2_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_HEV_P2_PrimaryBus_requiredOnly"),
            TestCase(@"SHEV_S2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_SHEV_S2_PrimaryBus"),
            TestCase(@"SHEV_S2_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_SHEV_S2_PrimaryBus_requiredOnly"),
            TestCase(@"SHEV_S3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_SHEV_S3_PrimaryBus"),
            TestCase(@"SHEV_S3_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_SHEV_S3_PrimaryBus_requiredOnly"),
            TestCase(@"SHEV_S4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_SHEV_S4_PrimaryBus"),
            TestCase(@"SHEV_S4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_SHEV_S4_PrimaryBus_requiredOnly"),
            TestCase(@"SHEV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_SHEV_IEPC_PrimaryBus"),
            TestCase(@"SHEV_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_SHEV_IEPC_PrimaryBus_requiredOnly"),
            TestCase(@"FCHV_F2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_FCHV_F2_PrimaryBus"),
            TestCase(@"FCHV_F2_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_FCHV_F2_PrimaryBus_requiredOnly"),
            TestCase(@"FCHV_F3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_FCHV_F3_PrimaryBus"),
            TestCase(@"FCHV_F3_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_FCHV_F3_PrimaryBus_requiredOnly"), 
            TestCase(@"FCHV_F4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_FCHV_F4_PrimaryBus"),
            TestCase(@"FCHV_F4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_FCHV_F4_PrimaryBus_requiredOnly"),
            TestCase(@"FCHV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_FCHV_IEPC_PrimaryBus"),
            TestCase(@"FCHV_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_FCHV_IEPC_PrimaryBus_requiredOnly"),
            TestCase(@"Multiple_PEV_E2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_PEV_E2_IEPC_PrimaryBus"),
            TestCase(@"Multiple_PEV_E2_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Multiple_PEV_E2_IEPC_PrimaryBus_requiredOnly"),
            TestCase(@"Multiple_PEV_E3_E4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_PEV_E3_E4_PrimaryBus"),
            TestCase(@"Multiple_FCHV_F2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_FCHV_F2_IEPC_PrimaryBus"),
            TestCase(@"Multiple_FCHV_F2_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Multiple_FCHV_F2_IEPC_PrimaryBus_requiredOnly"),
            TestCase(@"Multiple_FCHV_F3_F4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_FCHV_F3_F4_PrimaryBus"),
            TestCase(@"Multiple_FCHV_F3_F4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Multiple_FCHV_F3_F4_PrimaryBus_requiredOnly"),
            TestCase(@"Multiple_SHEV_S2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_SHEV_S2_IEPC_PrimaryBus"),
            TestCase(@"Multiple_SHEV_S2_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Multiple_SHEV_S2_IEPC_PrimaryBus_requiredOnly"),
            TestCase(@"Multiple_SHEV_S3_S4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_SHEV_S3_S4_PrimaryBus"),
            TestCase(@"Multiple_SHEV_S3_S4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Multiple_SHEV_S3_S4_PrimaryBus_requiredOnly"),

            TestCase(@"Conventional_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Conventional_CompletedBus"),
            TestCase(@"Conventional_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_Conventional_CompletedBus_requiredOnly"),
            TestCase(@"Exempted_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Exempted_CompletedBus"),
            TestCase(@"Exempted_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_Exempted_CompletedBus_requiredOnly"),
            TestCase(@"HEV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_HEV_CompletedBus"),
            TestCase(@"HEV_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_HEV_CompletedBus_requiredOnly"),
            TestCase(@"PEV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_PEV_CompletedBus"),
            TestCase(@"PEV_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_PEV_CompletedBus_requiredOnly"),
            TestCase(@"FCHV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_FCHV_CompletedBus"),
            TestCase(@"FCHV_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_FCHV_CompletedBus_requiredOnly"),
        ]
        public void TestVehicleInput(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);

            var inputData = XmlReader.Create(filename);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(inputData);

            if (xmlDoc.DocumentElement == null)
            {
                throw new VectoException("empty xml document!");
            }

            var documentType = XMLHelper.GetDocumentTypeFromRootElement(xmlDoc.DocumentElement.LocalName);
            if (documentType == null)
            {
                throw new VectoException("unknown xml file! {0}", xmlDoc.DocumentElement.LocalName);
            }

            var isValid = new XMLValidator(xmlDoc, null, XMLValidator.CallBackExceptionOnError).ValidateXML(documentType.Value);
            Assert.IsTrue(isValid);
        }

        [TestCase(@"Conventional_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Conventional_HeavyLorry")]
        public void TestReaderConventionalHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(6995.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - multi-disc clutch", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Crankshaft mounted - Electronically controlled visco clutch")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement mech. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 1-stage + mech. clutch + AMS")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Conventional_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Conventional_HeavyLorry_requiredOnly")]
        public void TestReaderConventionalHeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(6995.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - multi-disc clutch", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(0, vehicle.TorqueLimits.Count);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Crankshaft mounted - Electronically controlled visco clutch")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement mech. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 1-stage + mech. clutch + AMS")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Conventional_MediumLorry.xml", LORRIES, TestName = "v27_Reader_Conventional_MediumLorry")]
        public void TestReaderConventionalMediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Conventional_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Conventional_MediumLorry_requiredOnly")]
        public void TestReaderConventionalMediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(0, vehicle.TorqueLimits.Count);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_H2_ICE_HeavyLorry")]
        public void TestReaderH2ICEHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(6995.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - multi-disc clutch", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(80.0.SI<Kilogram>(), vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(0, vehicle.TorqueLimits.Count);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Crankshaft mounted - Electronically controlled visco clutch")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement mech. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 1-stage + mech. clutch + AMS")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"H2_ICE_MediumLorry.xml", LORRIES, TestName = "v27_Reader_H2_ICE_MediumLorry")]
        public void TestReaderH2ICEMediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(6995.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(false, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(1.0.SI<Kilogram>(), vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(0, vehicle.TorqueLimits.Count);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Crankshaft mounted - Electronically controlled visco clutch")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement mech. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 1-stage + mech. clutch + AMS")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Exempted_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Exempted_HeavyLorry")]
        public void TestReaderExemptedHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(6000.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(12000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(400000, vehicle.MaxNetPower1.Value());
            Assert.AreEqual("In-motion charging Article 9 exempted", vehicle.ExemptedTechnology);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Exempted_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Exempted_HeavyLorry_requiredOnly")]
        public void TestReaderExemptedHeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(6000.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(12000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(400000, vehicle.MaxNetPower1.Value());
            Assert.AreEqual("In-motion charging Article 9 exempted", vehicle.ExemptedTechnology);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Exempted_MediumLorry.xml", LORRIES, TestName = "v27_Reader_Exempted_MediumLorry")]
        public void TestReaderExemptedMediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("ML Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("ML Street 1", vehicle.ManufacturerAddress);
            Assert.AreEqual("Sample ML Model", vehicle.Model);
            Assert.AreEqual("VEH-ML34567890", vehicle.VIN);
            Assert.AreEqual("2020-01-09T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3600.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7300.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(350000, vehicle.MaxNetPower1.Value());
            Assert.AreEqual("FCV Article 9 exempted", vehicle.ExemptedTechnology);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Exempted_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Exempted_MediumLorry_requiredOnly")]
        public void TestReaderExemptedMediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("ML Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("ML Street 1", vehicle.ManufacturerAddress);
            Assert.AreEqual("Sample ML Model", vehicle.Model);
            Assert.AreEqual("VEH-ML34567890", vehicle.VIN);
            Assert.AreEqual("2020-01-09T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3600.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7300.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(350000, vehicle.MaxNetPower1.Value());
            Assert.AreEqual("FCV Article 9 exempted", vehicle.ExemptedTechnology);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_P2_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_HEV_P2_HeavyLorry")]
        public void TestReaderHEVP2HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(7204.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.OverheadPantograph, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.HybridP2));
            Assert.AreEqual(2, vehicle.BoostingLimitations.Rows.Count);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_P2_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_HEV_P2_HeavyLorry_requiredOnly")]
        public void TestReaderHEVP2HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(7204.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.OverheadPantograph, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(0, vehicle.TorqueLimits.Count);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_P2_MediumLorry.xml", LORRIES, TestName = "v27_Reader_HEV_P2_MediumLorry")]
        public void TestReaderHEVP2MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.HybridP2));
            Assert.AreEqual(2, vehicle.BoostingLimitations.Rows.Count);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_P2_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_HEV_P2_MediumLorry_requiredOnly")]
        public void TestReaderHEVP2MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(0, vehicle.TorqueLimits.Count);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_IHPC_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_HEV_IHPC_HeavyLorry")]
        public void TestReaderIHPCHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(7500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.HybridP2));
            Assert.AreEqual(2, vehicle.BoostingLimitations.Rows.Count);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Belt driven or driven via transm. - Bimetallic controlled visco clutch")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 1-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("None")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_IHPC_MediumLorry.xml", LORRIES, TestName = "v27_Reader_HEV_IHPC_MediumLorry")]
        public void TestReaderIHPCMediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(7500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.HybridP2));
            Assert.AreEqual(2, vehicle.BoostingLimitations.Rows.Count);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Belt driven or driven via transm. - Bimetallic controlled visco clutch")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 1-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("None")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_HEV_H2_ICE_HeavyLorry")]
        public void TestReaderHEV_H2_ICE_HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(7204.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.OverheadPantograph, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.HybridP2));
            Assert.AreEqual(2, vehicle.BoostingLimitations.Rows.Count);
            Assert.AreEqual(80, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_H2_ICE_MediumLorry.xml", LORRIES, TestName = "v27_Reader_HEV_H2_ICE_MediumLorry")]
        public void TestReaderHEV_H2_ICE_MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.HybridP2));
            Assert.AreEqual(2, vehicle.BoostingLimitations.Rows.Count);
            Assert.AreEqual(80, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Variable displacement elec. controlled")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_IEPC_HeavyLorry")]
        public void TestReaderSHEVIEPCHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.GroundRail, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.None, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Vacuum pump + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);
            
            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_SHEV_IEPC_HeavyLorry_requiredOnly")]
        public void TestReaderSHEVIEPCHeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.GroundRail, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.None, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Vacuum pump + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_IEPC_MediumLorry")]
        public void TestReaderSHEVIEPCMediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_IEPC_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_SHEV_IEPC_MediumLorry_requiredOnly")]
        public void TestReaderSHEVIEPCMediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S2_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_S2_HeavyLorry")]
        public void TestReaderSHEVS2HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_H2_ICE_HeavyLorry")]
        public void TestReaderSHEV_H2_ICE_HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));
            Assert.AreEqual(80, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S2_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_SHEV_S2_HeavyLorry_requiredOnly")]
        public void TestReaderSHEVS2HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S2_MediumLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_S2_MediumLorry")]
        public void TestReaderSHEVS2MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.3, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_H2_ICE_MediumLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_H2_ICE_MediumLorry")]
        public void TestReaderSHEV_H2_ICE_MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.3, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));
            Assert.AreEqual(80, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S2_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_SHEV_S2_MediumLorry_requiredOnly")]
        public void TestReaderSHEVS2MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S3_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_S3_HeavyLorry")]
        public void TestReaderSHEVS3HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S3_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_SHEV_S3_HeavyLorry_requiredOnly")]
        public void TestReaderSHEVS3HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S3_MediumLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_S3_MediumLorry")]
        public void TestReaderSHEVS3MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.3, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1.0, vehicle.Components.RetarderInputData.Ratio);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S3_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_SHEV_S3_MediumLorry_requiredOnly")]
        public void TestReaderSHEVS3MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S4_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_S4_HeavyLorry")]
        public void TestReaderSHEVS4HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.GroundRail, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_SHEV_S4_HeavyLorry_requiredOnly")]
        public void TestReaderSHEVS4HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(600, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual("only one engaged gearwheel above oil level", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.GroundRail, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S4_MediumLorry.xml", LORRIES, TestName = "v27_Reader_SHEV_S4_MediumLorry")]
        public void TestReaderSHEVS4MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(20.3, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));

            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S4_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_SHEV_S4_MediumLorry_requiredOnly")]
        public void TestReaderSHEVS4MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(650, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E2_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_PEV_E2_HeavyLorry")]
        public void TestReaderPEVE2HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E2_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_PEV_E2_HeavyLorry_requiredOnly")]
        public void TestReaderPEVE2HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E2_MediumLorry.xml", LORRIES, TestName = "v27_Reader_PEV_E2_MediumLorry")]
        public void TestReaderPEVE2MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E2_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_PEV_E2_MediumLorry_requiredOnly")]
        public void TestReaderPEVE2MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E3_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_PEV_E3_HeavyLorry")]
        public void TestReaderPEVE3HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E3_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_PEV_E3_HeavyLorry_requiredOnly")]
        public void TestReaderPEVE3HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E3_MediumLorry.xml", LORRIES, TestName = "v27_Reader_PEV_E3_MediumLorry")]
        public void TestReaderPEVE3MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E3_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_PEV_E3_MediumLorry_requiredOnly")]
        public void TestReaderPEVE3MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E4_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_PEV_E4_HeavyLorry")]
        public void TestReaderPEVE4HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_PEV_E4_HeavyLorry_requiredOnly")]
        public void TestReaderPEVE4HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E4_MediumLorry.xml", LORRIES, TestName = "v27_Reader_PEV_E4_MediumLorry")]
        public void TestReaderPEVE4MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E4_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_PEV_E4_MediumLorry_requiredOnly")]
        public void TestReaderPEVE4MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_PEV_IEPC_HeavyLorry")]
        public void TestReaderPEVIEPCHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry IEPC", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_PEV_IEPC_HeavyLorry_requiredOnly")]
        public void TestReaderPEVIEPCHeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry IEPC", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_Reader_PEV_IEPC_MediumLorry")]
        public void TestReaderPEVIEPCMediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_IEPC_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_PEV_IEPC_MediumLorry_requiredOnly")]
        public void TestReaderPEVIEPCMediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F2_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_FCHV_F2_HeavyLorry")]
        public void TestReaderFCHVF2HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F2_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_FCHV_F2_HeavyLorry_requiredOnly")]
        public void TestReaderFCHVF2HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F2_MediumLorry.xml", LORRIES, TestName = "v27_Reader_FCHV_F2_MediumLorry")]
        public void TestReaderFCHVF2MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F2_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_FCHV_F2_MediumLorry_requiredOnly")]
        public void TestReaderFCHVF2MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F3_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_FCHV_F3_HeavyLorry")]
        public void TestReaderFCHVF3HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F3_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_FCHV_F3_HeavyLorry_requiredOnly")]
        public void TestReaderFCHVF3HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F3_MediumLorry.xml", LORRIES, TestName = "v27_Reader_FCHV_F3_MediumLorry")]
        public void TestReaderFCHVF3MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F3_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_FCHV_F3_MediumLorry_requiredOnly")]
        public void TestReaderFCHVF3MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F4_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_FCHV_F4_HeavyLorry")]
        public void TestReaderFCHVF4HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_FCHV_F4_HeavyLorry_requiredOnly")]
        public void TestReaderFCHVF4HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual("None", vehicle.Components.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F4_MediumLorry.xml", LORRIES, TestName = "v27_Reader_FCHV_F4_MediumLorry")]
        public void TestReaderFCHVF4MediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap.Rows.Count; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F4_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_FCHV_F4_MediumLorry_requiredOnly")]
        public void TestReaderFCHVF4MediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_FCHV_IEPC_HeavyLorry")]
        public void TestReaderFCHVIEPCHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("FCHV Heavy Lorry IEPC", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1.0, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Vacuum pump + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_FCHV_IEPC_HeavyLorry_requiredOnly")]
        public void TestReaderFCHVIEPCHeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("FCHV Heavy Lorry IEPC", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1.0, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Vacuum pump + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_Reader_FCHV_IEPC_MediumLorry")]
        public void TestReaderFCHVIEPCMediumLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("FCHV Medium Lorry IEPC", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(20.300, vehicle.CargoVolume.Value());
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1.0, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Vacuum pump + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_IEPC_MediumLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_FCHV_IEPC_MediumLorry_requiredOnly")]
        public void TestReaderFCHVIEPCMediumLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("FCHV Medium Lorry IEPC", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1.0, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Vacuum pump + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_FCHV_F2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Multiple_FCHV_F2_IEPC_HeavyLorry")]
        public void TestReaderMultipleFCHVF2IEPCHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(true, axlePt2.RetarderInputData.Ratio.IsEqual(2));
            Assert.AreEqual(AngledriveType.SeparateAngledrive, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("drive shaft and/or up to 2 gear wheels - multi-disc clutch", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE2) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt1.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNotNull(axlePt1.TorqueConverterInputData);
            Assert.AreEqual(6, axlePt1.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt2.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_FCHV_F2_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Multiple_FCHV_F2_IEPC_HeavyLorry_requiredOnly")]
        public void TestReaderMultipleFCHVF2IEPCHeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("None", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("None", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt1.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_FCHV_F3_F4_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Multiple_FCHV_F3_F4_HeavyLorry")]
        public void TestReaderMultipleFCHVF3F4HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("drive shaft and/or up to 2 gear wheels - multi-disc clutch", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE3) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE4) && (x.Key.AxleNumber == 2)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt1.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt2.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_FCHV_F3_F4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Multiple_FCHV_F3_F4_HeavyLorry_requiredOnly")]
        public void TestReaderMultipleFCHVF3F4HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("None", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("None", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt1.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt2.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_PEV_E2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Multiple_PEV_E2_IEPC_HeavyLorry")]
        public void TestReaderMultiplePEVE2IEPCHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(true, axlePt2.RetarderInputData.Ratio.IsEqual(2));
            Assert.AreEqual(AngledriveType.SeparateAngledrive, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("drive shaft and/or up to 2 gear wheels - multi-disc clutch", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE2) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt1.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNotNull(axlePt1.TorqueConverterInputData);
            Assert.AreEqual(6, axlePt1.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt2.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_PEV_E2_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Multiple_PEV_E2_IEPC_HeavyLorry_requiredOnly")]
        public void TestReaderMultiplePEVE2IEPCHeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("None", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("None", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt1.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_PEV_E3_E4_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Multiple_PEv_E3_E4_HeavyLorry")]
        public void TestReaderMultiplePEVE3E4HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("drive shaft and/or up to 2 gear wheels - multi-disc clutch", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE3) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE4) && (x.Key.AxleNumber == 2)));
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt1.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt2.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_PEV_E3_E4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Multiple_PEV_E3_E4_HeavyLorry_requiredOnly")]
        public void TestReaderMultiplePEVE3E4HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(10143.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(18000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("None", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("None", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt1.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(true, axlePt2.ElectricMotor.ADC.Ratio.IsEqual(12.720));
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Electric driven pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage + elec. driven")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Multiple_SHEV_H2_ICE_HeavyLorry")]
        public void TestReaderMultipleSHEVH2ICEHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("drive shaft and/or up to 2 gear wheels - multi-disc clutch", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(3, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE3) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE4) && (x.Key.AxleNumber == 2)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.GEN) && (x.Key.AxleNumber == Constants.NOT_IN_AXLE_POWERTRAIN)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(80, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(1, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt1.ElectricMotor.ADC);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(1, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt2.ElectricMotor.ADC);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_S2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Multiple_SHEV_S2_IEPC_HeavyLorry")]
        public void TestReaderMultipleSHEVS2IEPCHeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(true, axlePt2.RetarderInputData.Ratio.IsEqual(2));
            Assert.AreEqual(AngledriveType.SeparateAngledrive, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("drive shaft and/or up to 2 gear wheels - multi-disc clutch", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.GEN) && (x.Key.AxleNumber == Constants.NOT_IN_AXLE_POWERTRAIN)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE2) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(true, vehicle.H2StorageUsableCapacity.IsEqual(80));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(1, axlePt1.ElectricMotor.Count);
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNotNull(axlePt1.TorqueConverterInputData);
            Assert.AreEqual(6, axlePt1.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt2.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_S2_IEPC_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Multiple_SHEV_S2_IEPC_HeavyLorry_requiredOnly")]
        public void TestReaderMultipleSHEVS2IEPCHeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("None", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("None", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(1, axlePt1.ElectricMotor.Count);
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_S3_S4_HeavyLorry.xml", LORRIES, TestName = "v27_Reader_Multiple_SHEV_S3_S4_HeavyLorry")]
        public void TestReaderMultipleSHEVS3S4HeavyLorry(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("drive shaft and/or up to 2 gear wheels - multi-disc clutch", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(3, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE3) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE4) && (x.Key.AxleNumber == 2)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.GEN) && (x.Key.AxleNumber == Constants.NOT_IN_AXLE_POWERTRAIN)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(80, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(1, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt1.ElectricMotor.ADC);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(1, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt2.ElectricMotor.ADC);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNotNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_S3_S4_HeavyLorry_requiredOnly.xml", LORRIES, TestName = "v27_Reader_Multiple_SHEV_S3_S4_HeavyLorry_requiredOnly")]
        public void TestReaderMultipleSHEVS3S4HeavyLorryRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("HEV Heavy Lorry Px", vehicle.Model);
            Assert.AreEqual("1234", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(4670.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(11990.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual("None", axlePt1.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual("None", axlePt2.PTOTransmissionInputData.PTOTransmissionType);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.VocationalVehicle);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(true, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(1, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt1.ElectricMotor.ADC);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(1, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt2.ElectricMotor.ADC);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.AuxiliaryInputData.Auxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.Fan) && x.Technology.Contains("Hydraulic driven - Constant displacement pump")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.SteeringPump) && x.Technology.Contains("Full electric steering gear")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.ElectricSystem) && x.Technology.Contains("Standard technology - LED headlights, all")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.PneumaticSystem) && x.Technology.Contains("Medium Supply 2-stage")));
            Assert.AreEqual(true, aux.Any(x => (x.Type == AuxiliaryType.HVAC) && x.Technology.Contains("Default")));
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Conventional_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_Conventional_PrimaryBus")]
        public void TestReaderConventionalPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890_nonSmart", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(700));
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.UNKNOWN, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.TorqueLimits.Count);
            Assert.AreEqual(6, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(1800, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.TorqueLimits[1].Gear);
            Assert.AreEqual(2500, vehicle.TorqueLimits[1].MaxTorque.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNull(vehicle.Components.ElectricMachines);
            Assert.IsNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(25, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Variable displacement elec. controlled"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.Alternators.Any(x => x.RatedVoltage.IsEqual(12) && x.RatedCurrent.IsEqual(1)));
            Assert.AreEqual(true, aux.ElectricSupply.ElectricStorage.Any(x => x.Technology == "lead-acid battery - AGM"));
            Assert.AreEqual(true, aux.ElectricSupply.ElectricStorage.Any(x => x.Technology == "with DCDC converter"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "Large Supply 2-stage");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.mechanically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartAirCompression);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Conventional_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_Conventional_PrimaryBus_requiredOnly")]
        public void TestReaderConventionalPrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890_nonSmart", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(700));
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.UNKNOWN, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(0, vehicle.TorqueLimits.Count);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNull(vehicle.Components.ElectricMachines);
            Assert.IsNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Variable displacement elec. controlled"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.Alternators.Count() == 0);
            Assert.AreEqual(true, aux.ElectricSupply.ElectricStorage.Count() == 0);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "Large Supply 2-stage");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.mechanically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartAirCompression);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Exempted_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_Exempted_PrimaryBus")]
        public void TestReaderExemptedPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Infinite Loop 1", vehicle.ManufacturerAddress);
            Assert.AreEqual("Sample Bus Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2020-01-09T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_6x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(15400.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.MaxNetPower1.IsEqual(350000));
            Assert.AreEqual(true, vehicle.ExemptedTechnology == "FCV Article 9 exempted");
            
            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Exempted_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_Exempted_PrimaryBus_requiredOnly")]
        public void TestReaderExemptedPrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Infinite Loop 1", vehicle.ManufacturerAddress);
            Assert.AreEqual("Sample Bus Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2020-01-09T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_6x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(15400.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(true, vehicle.MaxNetPower1.IsEqual(350000));
            Assert.AreEqual(true, vehicle.ExemptedTechnology == "FCV Article 9 exempted");

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_FCHV_F2_PrimaryBus")]
        public void TestReaderFCHVF2PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F2_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_FCHV_F2_PrimaryBus_requiredOnly")]
        public void TestReaderFCHVF2PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_FCHV_F3_PrimaryBus")]
        public void TestReaderFCHVF3PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F3_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_FCHV_F3_PrimaryBus_requiredOnly")]
        public void TestReaderFCHVF3PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_FCHV_F4_PrimaryBus")]
        public void TestReaderFCHVF4PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_F4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_FCHV_F4_PrimaryBus_requiredOnly")]
        public void TestReaderFCHVF4PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_FCHV_IEPC_PrimaryBus")]
        public void TestReaderFCHVIEPCPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_FCHV_IEPC_PrimaryBus_requiredOnly")]
        public void TestReaderFCHVIEPCPrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.Ratio; });
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            Assert.IsNull(vehicle.Components.AuxiliaryInputData);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"H2_ICE_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_H2_ICE_PrimaryBus")]
        public void TestReaderH2ICEPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890_nonSmart", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(700));
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.UNKNOWN, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.TorqueLimits.Count);
            Assert.AreEqual(6, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(1800, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.TorqueLimits[1].Gear);
            Assert.AreEqual(2500, vehicle.TorqueLimits[1].MaxTorque.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(80, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNull(vehicle.Components.ElectricMachines);
            Assert.IsNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(25, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Variable displacement elec. controlled"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "Large Supply 2-stage");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.mechanically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartAirCompression);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_IHPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_HEV_IHPC_PrimaryBus")]
        public void TestReaderHEVIHPSPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(600));
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.None, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            
            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Variable displacement elec. controlled"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "Large Supply 2-stage");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.mechanically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartAirCompression);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_P2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_HEV_P2_PrimaryBus")]
        public void TestReaderHEVP2PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(600));
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.None, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.TorqueLimits.Count);
            Assert.AreEqual(9, vehicle.TorqueLimits[0].Gear);
            Assert.AreEqual(2000, vehicle.TorqueLimits[0].MaxTorque.Value());
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.HybridP2));
            Assert.AreEqual(3, vehicle.BoostingLimitations.Rows.Count);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Variable displacement elec. controlled"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.Alternators.Any(x => x.RatedVoltage.IsEqual(12) && x.RatedCurrent.IsEqual(1)));
            Assert.AreEqual(true, aux.ElectricSupply.ElectricStorage.Any(x => x.Technology == "lead-acid battery - AGM"));
            Assert.AreEqual(true, aux.ElectricSupply.ElectricStorage.Any(x => x.Technology == "with DCDC converter"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "Large Supply 2-stage");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.mechanically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartAirCompression);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_P2_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_HEV_P2_PrimaryBus_requiredOnly")]
        public void TestReaderHEVP2PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(600));
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.None, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(0, vehicle.TorqueLimits.Count);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Variable displacement elec. controlled"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "Large Supply 2-stage");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.mechanically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartAirCompression);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_PEV_E2_PrimaryBus")]
        public void TestReaderPEVE2PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.None, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E2_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_PEV_E2_PrimaryBus_requiredOnly")]
        public void TestReaderPEVE2PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, vehicle.Components.AngledriveInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.None, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_PEV_E3_PrimaryBus")]
        public void TestReaderPEVE3PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E3_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_PEV_E3_PrimaryBus_requiredOnly")]
        public void TestReaderPEVE3PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_PEV_E4_PrimaryBus")]
        public void TestReaderPEVE4PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_E4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_PEV_E4_PrimaryBus_requiredOnly")]
        public void TestReaderPEVE4PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_PEV_IEPC_PrimaryBus")]
        public void TestReaderPEVIEPCPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_PEV_IEPC_PrimaryBus_requiredOnly")]
        public void TestReaderPEVIEPCPrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(false, vehicle.VocationalVehicle);
            Assert.AreEqual(false, vehicle.SleeperCab);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_SHEV_IEPC_PrimaryBus")]
        public void TestReaderSHEVIEPCPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(25000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(true, vehicle.H2StorageUsableCapacity.IsEqual(1));
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.GEN));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.AreEqual(2, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "Large Supply 2-stage");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.mechanically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_SHEV_IEPC_PrimaryBus_requiredOnly")]
        public void TestReaderSHEVIEPCPrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(25000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.IsNotNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "Large Supply 2-stage");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.mechanically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_SHEV_S2_PrimaryBus")]
        public void TestReaderSHEVS2PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(1));
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(true, vehicle.Components.AngledriveInputData.Type == AngledriveType.SeparateAngledrive);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(true, vehicle.H2StorageUsableCapacity.IsEqual(1));
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE2));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count());
            Assert.IsNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNotNull(vehicle.Components.TorqueConverterInputData);
            Assert.AreEqual(6, vehicle.Components.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S2_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_SHEV_S2_PrimaryBus_requiredOnly")]
        public void TestReaderSHEVS2PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(1));
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(true, vehicle.Components.AngledriveInputData.Type == AngledriveType.None);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count());
            Assert.IsNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_SHEV_S3_PrimaryBus")]
        public void TestReaderSHEVS3PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(1));
            Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(1, vehicle.Components.RetarderInputData.Ratio);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(true, vehicle.H2StorageUsableCapacity.IsEqual(1));
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE3));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count());
            Assert.IsNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.AreEqual(31, vehicle.Components.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S3_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_SHEV_S3_PrimaryBus_requiredOnly")]
        public void TestReaderSHEVS3PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(1));
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count());
            Assert.IsNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNotNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_SHEV_S4_PrimaryBus")]
        public void TestReaderSHEVS4PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(1));
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(true, vehicle.H2StorageUsableCapacity.IsEqual(1));
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => x.Key.Position == PowertrainPosition.BatteryElectricE4));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count());
            Assert.IsNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"SHEV_S4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_SHEV_S4_PrimaryBus_requiredOnly")]
        public void TestReaderSHEVS4PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.EngineIdleSpeed.AsRPM.IsEqual(1));
            Assert.AreEqual(RetarderType.None, vehicle.Components.RetarderInputData.Type);
            Assert.AreEqual(null, vehicle.Components.AngledriveInputData);
            Assert.AreEqual(null, vehicle.Components.PTOTransmissionInputData);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureID);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.TorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricMachines);
            Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count());
            Assert.IsNull(vehicle.Components.IEPC);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNull(vehicle.Components.GearboxInputData);
            Assert.IsNull(vehicle.Components.TorqueConverterInputData);
            Assert.IsNull(vehicle.Components.AngledriveInputData);
            Assert.Throws<VectoException>(() => { var x = vehicle.Components.RetarderInputData.LossMap; });
            Assert.IsNull(vehicle.Components.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual("Crankshaft mounted - Electronically controlled visco clutch", aux.FanTechnology);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.PneumaticConsumers.AdBlueDosing == ConsumerTechnology.Pneumatically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_FCHV_F2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_FCHV_F2_IEPC_PrimaryBus")]
        public void TestReaderMultipleFCHVF2IEPCPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(true, axlePt2.RetarderInputData.Ratio.IsEqual(2));
            Assert.AreEqual(AngledriveType.SeparateAngledrive, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE2) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNotNull(axlePt1.TorqueConverterInputData);
            Assert.AreEqual(6, axlePt1.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt2.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_FCHV_F2_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_FCHV_F2_IEPC_PrimaryBus_requiredOnly")]
        public void TestReaderMultipleFCHVF2IEPCPrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.F2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.F_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_FCHV_F3_F4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_FCHV_F3_F4_PrimaryBus")]
        public void TestReaderMultipleFCHVF3F4PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);
            
            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE3) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE4) && (x.Key.AxleNumber == 2)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_FCHV_F3_F4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_FCHV_F3_F4_PrimaryBus_requiredOnly")]
        public void TestReaderMultipleFCHVF3F4PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.F3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.F4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNotNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_PEV_E2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_PEV_E2_IEPC_PrimaryBus")]
        public void TestReaderMultiplePEVE2IEPCPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(true, axlePt2.RetarderInputData.Ratio.IsEqual(2));
            Assert.AreEqual(AngledriveType.SeparateAngledrive, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE2) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNotNull(axlePt1.TorqueConverterInputData);
            Assert.AreEqual(6, axlePt1.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt2.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_PEV_E2_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_PEV_E2_IEPC_PrimaryBus_requiredOnly")]
        public void TestReaderMultiplePEVE2IEPCPrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.CargoVolume);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.E2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.E_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_PEV_E3_E4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_PEV_E3_E4_PrimaryBus")]
        public void TestReaderMultiplePEVE3E4PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE3) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE4) && (x.Key.AxleNumber == 2)));
            
            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_PEV_E3_E4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_PEV_E3_E4_PrimaryBus_requiredOnly")]
        public void TestReaderMultiplePEVE3E4PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("TU Graz", vehicle.Manufacturer);
            Assert.AreEqual("Inffeldgasse 19", vehicle.ManufacturerAddress);
            Assert.AreEqual("PEV Heavy Lorry E2", vehicle.Model);
            Assert.AreEqual("123467890", vehicle.VIN);
            Assert.AreEqual("2017-01-01T00:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(28000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.E3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.E4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            
            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Electric driven pump"));
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_S2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_SHEV_S2_IEPC_PrimaryBus")]
        public void TestReaderMultipleSHEVS2IEPCPrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(1, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(true, axlePt2.RetarderInputData.Ratio.IsEqual(2));
            Assert.AreEqual(AngledriveType.SeparateAngledrive, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(2, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.GEN) && (x.Key.AxleNumber == Constants.NOT_IN_AXLE_POWERTRAIN)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE2) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(true, vehicle.H2StorageUsableCapacity.IsEqual(1));

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNotNull(axlePt1.TorqueConverterInputData);
            Assert.AreEqual(6, axlePt1.AngledriveInputData.LossMap.Rows.Count);
            Assert.AreEqual(31, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.AreEqual(2, axlePt2.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.FanTechnology.Contains("Crankshaft mounted - Electronically controlled visco clutch"));
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_S2_IEPC_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_SHEV_S2_IEPC_PrimaryBus_requiredOnly")]
        public void TestReaderMultipleSHEVS2IEPCPrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(1, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.S2, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S_IEPC, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE2, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.IsNotNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.IEPCInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.FanTechnology.Contains("Crankshaft mounted - Electronically controlled visco clutch"));
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_S3_S4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_SHEV_S3_S4_PrimaryBus")]
        public void TestReaderMultipleSHEVS3S4PrimaryBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(1, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.TransmissionInputRetarder, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(true, axlePt1.RetarderInputData.Ratio.IsEqual(1));
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(3, vehicle.ElectricMotorTorqueLimits.Count);
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE3) && (x.Key.AxleNumber == 1)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.BatteryElectricE4) && (x.Key.AxleNumber == 2)));
            Assert.AreEqual(true, vehicle.ElectricMotorTorqueLimits.Any(x => (x.Key.Position == PowertrainPosition.GEN) && (x.Key.AxleNumber == Constants.NOT_IN_AXLE_POWERTRAIN)));
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt1.ElectricMotor.ADC);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.AreEqual(31, axlePt1.RetarderInputData.LossMap.Rows.Count);
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt2.ElectricMotor.ADC);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.FanTechnology.Contains("Crankshaft mounted - Electronically controlled visco clutch"));
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Multiple_SHEV_S3_S4_PrimaryBus_requiredOnly.xml", PRIMARYBUSES, TestName = "v27_Reader_Multiple_SHEV_S3_S4_PrimaryBus_requiredOnly")]
        public void TestReaderMultipleSHEVS3S4PrimaryBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            var axlePt1 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 1);
            var axlePt2 = vehicle.Components.AxlePowertrainInputData.First(x => x.AxleNumber == 2);

            Assert.AreEqual(vehicle.ArchitectureID, axlePt1.Architecture);
            Assert.AreEqual(vehicle.ArchitectureIDPwt2, axlePt2.Architecture);

            Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
            Assert.AreEqual("Generic Model", vehicle.Model);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2017-02-15T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
            Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
            Assert.AreEqual(true, vehicle.Articulated);
            Assert.AreEqual(20000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(1, vehicle.EngineIdleSpeed.AsRPM);
            Assert.AreEqual(RetarderType.None, axlePt1.RetarderInputData.Type);
            Assert.AreEqual(RetarderType.None, axlePt2.RetarderInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt1.AngledriveInputData.Type);
            Assert.AreEqual(AngledriveType.None, axlePt2.AngledriveInputData.Type);
            Assert.AreEqual(true, vehicle.ZeroEmissionVehicle);
            Assert.AreEqual(ArchitectureID.S3, vehicle.ArchitectureID);
            Assert.AreEqual(ArchitectureID.S4, vehicle.ArchitectureIDPwt2);
            Assert.AreEqual(true, vehicle.OVC);
            Assert.AreEqual(true, vehicle.BatteryOnlyMode);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(null, vehicle.ElectricMotorTorqueLimits);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);

            Assert.IsNull(vehicle.Components.FuelCellSystem);
            Assert.IsNotNull(vehicle.Components.EngineInputData);
            Assert.IsNotNull(vehicle.Components.Generator);
            Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.Generator.Position);
            Assert.AreEqual(1, vehicle.Components.Generator.Count);
            Assert.IsNotNull(vehicle.Components.ElectricStorage);
            Assert.IsNotNull(axlePt1.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE3, axlePt1.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt1.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt1.ElectricMotor.ADC);
            Assert.IsNull(axlePt1.GearboxInputData);
            Assert.IsNull(axlePt1.TorqueConverterInputData);
            Assert.Throws<VectoException>(() => { var x = axlePt1.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt1.RetarderInputData.LossMap; });
            Assert.IsNotNull(axlePt1.AxleGearInputData);
            Assert.IsNotNull(axlePt2.ElectricMotor);
            Assert.AreEqual(PowertrainPosition.BatteryElectricE4, axlePt2.ElectricMotor.Position);
            Assert.AreEqual(2, axlePt2.ElectricMotor.Count);
            Assert.AreEqual(null, axlePt2.ElectricMotor.ADC);
            Assert.Throws<VectoException>(() => { var x = axlePt2.AngledriveInputData.LossMap; });
            Assert.Throws<VectoException>(() => { var x = axlePt2.RetarderInputData.LossMap; });
            Assert.IsNull(axlePt2.AxleGearInputData);
            Assert.IsNotNull(vehicle.Components.AxleWheels);
            var aux = vehicle.Components.BusAuxiliaries;
            Assert.IsNotNull(aux);
            Assert.AreEqual(true, aux.FanTechnology.Contains("Crankshaft mounted - Electronically controlled visco clutch"));
            Assert.AreEqual(true, aux.SteeringPumpTechnology.Contains("Full electric steering gear"));
            Assert.AreEqual(true, aux.ElectricSupply.AlternatorTechnology == AlternatorType.Conventional);
            Assert.AreEqual(true, aux.ElectricSupply.ESSupplyFromHEVREESS);
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorSize == "not applicable");
            Assert.AreEqual(true, aux.PneumaticSupply.CompressorDrive == CompressorDrive.electrically);
            Assert.AreEqual(true, aux.PneumaticSupply.Clutch == "none");
            Assert.AreEqual(true, aux.PneumaticSupply.Ratio.IsEqual(1));
            Assert.AreEqual(true, aux.PneumaticSupply.SmartRegeneration);
            Assert.AreEqual(true, aux.PneumaticConsumers.AirsuspensionControl == ConsumerTechnology.Electrically);
            Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
            Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
            Assert.IsNull(vehicle.Components.AirdragInputData.AirDragArea);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Conventional_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Reader_Conventional_CompletedBus")]
        public void TestReaderConventionalCompletedBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2021-06-30T22:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual("Sample Bus Model", vehicle.Model);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.AirdragModifiedMultistep);
            Assert.AreEqual(RegistrationClass.II_III, vehicle.RegisteredClass);
            Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);
            Assert.AreEqual(1, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(10, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(11, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(2, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(VehicleCode.CB, vehicle.VehicleCode);
            Assert.AreEqual(true, vehicle.LowEntry);
            Assert.AreEqual(2.5, vehicle.Height.Value());
            Assert.AreEqual(9.5, vehicle.Length.Value());
            Assert.AreEqual(2.5, vehicle.Width.Value());
            Assert.AreEqual(2.0, vehicle.EntranceHeight.Value());
            Assert.AreEqual(ConsumerTechnology.Electrically, vehicle.DoorDriveTechnology);
            Assert.AreEqual(VehicleDeclarationType.final, vehicle.VehicleDeclarationType);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.None, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);

            Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.InteriorLightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.DayrunninglightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.PositionlightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.BrakelightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.HeadlightsLED);
            Assert.AreEqual(BusHVACSystemConfiguration.Configuration6, vehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeHeatingDriverCompartment);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeCoolingDriverCompartment);
            Assert.AreEqual(HeatPumpType.non_R_744_3_stage, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeCoolingPassengerCompartment);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeHeatingPassengerCompartment);
            Assert.AreEqual(50000, vehicle.Components.BusAuxiliaries.HVACAux.AuxHeaterPower.Value());
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.DoubleGlazing);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.AdjustableAuxiliaryHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.SeparateAirDistributionDucts);
            Assert.IsNotNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Conventional_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_Reader_Conventional_CompletedBus_requiredOnly")]
        public void TestReaderConventionalCompletedBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2021-06-30T22:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual(null, vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(null, vehicle.Model);
            Assert.AreEqual(null, vehicle.LegislativeClass);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(null, vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.AirdragModifiedMultistep);
            Assert.AreEqual(null, vehicle.RegisteredClass);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(null, vehicle.VehicleCode);
            Assert.AreEqual(null, vehicle.LowEntry);
            Assert.AreEqual(null, vehicle.Height);
            Assert.AreEqual(null, vehicle.Length);
            Assert.AreEqual(null, vehicle.Width);
            Assert.AreEqual(null, vehicle.EntranceHeight);
            Assert.AreEqual(null, vehicle.DoorDriveTechnology);
            Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(null, vehicle.ADAS);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);

            Assert.IsNull(vehicle.Components);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Exempted_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Reader_Exempted_CompletedBus")]
        public void TestReaderExemptedCompletedBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Infinite Loop", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567891", vehicle.VIN);
            Assert.AreEqual("2021-01-09T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual("Sample Bus Model 2", vehicle.Model);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(7000.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(10000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(RegistrationClass.A, vehicle.RegisteredClass);
            Assert.AreEqual(10, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(42, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(20, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(13, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(VehicleCode.CE, vehicle.VehicleCode);
            Assert.AreEqual(true, vehicle.LowEntry);
            Assert.AreEqual(2.5, vehicle.Height.Value());
            
            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"Exempted_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_Reader_Exempted_CompletedBus_requiredOnly")]
        public void TestReaderExemptedCompletedBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Infinite Loop", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567891", vehicle.VIN);
            Assert.AreEqual("2021-01-09T11:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual(null, vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(null, vehicle.Model);
            Assert.AreEqual(null, vehicle.LegislativeClass);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(null, vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.RegisteredClass);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(null, vehicle.VehicleCode);
            Assert.AreEqual(null, vehicle.LowEntry);
            Assert.AreEqual(null, vehicle.Height);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Reader_HEV_CompletedBus")]
        public void TestReaderHEVCompletedBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2021-06-30T22:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual("Sample Bus Model", vehicle.Model);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.AirdragModifiedMultistep);
            Assert.AreEqual(RegistrationClass.II_III, vehicle.RegisteredClass);
            Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);
            Assert.AreEqual(1, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(10, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(11, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(2, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(VehicleCode.CB, vehicle.VehicleCode);
            Assert.AreEqual(true, vehicle.LowEntry);
            Assert.AreEqual(2.5, vehicle.Height.Value());
            Assert.AreEqual(9.5, vehicle.Length.Value());
            Assert.AreEqual(2.5, vehicle.Width.Value());
            Assert.AreEqual(2.0, vehicle.EntranceHeight.Value());
            Assert.AreEqual(ConsumerTechnology.Electrically, vehicle.DoorDriveTechnology);
            Assert.AreEqual(VehicleDeclarationType.final, vehicle.VehicleDeclarationType);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);

            Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.InteriorLightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.DayrunninglightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.PositionlightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.BrakelightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.HeadlightsLED);
            Assert.AreEqual(BusHVACSystemConfiguration.Configuration6, vehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeHeatingDriverCompartment);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeCoolingDriverCompartment);
            Assert.AreEqual(HeatPumpType.R_744, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeCoolingPassengerCompartment);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeHeatingPassengerCompartment);
            Assert.AreEqual(50000, vehicle.Components.BusAuxiliaries.HVACAux.AuxHeaterPower.Value());
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.DoubleGlazing);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.AdjustableAuxiliaryHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.SeparateAirDistributionDucts);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.WaterElectricHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.AirElectricHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.OtherHeatingTechnology);
            Assert.IsNotNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"HEV_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_Reader_HEV_CompletedBus_requiredOnly")]
        public void TestReaderHEVCompletedBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2021-06-30T22:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual(null, vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(null, vehicle.Model);
            Assert.AreEqual(null, vehicle.LegislativeClass);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(null, vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.AirdragModifiedMultistep);
            Assert.AreEqual(null, vehicle.RegisteredClass);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(null, vehicle.VehicleCode);
            Assert.AreEqual(null, vehicle.LowEntry);
            Assert.AreEqual(null, vehicle.Height);
            Assert.AreEqual(null, vehicle.Length);
            Assert.AreEqual(null, vehicle.Width);
            Assert.AreEqual(null, vehicle.EntranceHeight);
            Assert.AreEqual(null, vehicle.DoorDriveTechnology);
            Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(DynamicChargingTechnology.None, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(null, vehicle.ADAS);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);

            Assert.IsNull(vehicle.Components);
            
            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Reader_PEV_CompletedBus")]
        public void TestReaderPEVCompletedBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2021-06-30T22:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual("Sample Bus Model", vehicle.Model);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.AirdragModifiedMultistep);
            Assert.AreEqual(RegistrationClass.II_III, vehicle.RegisteredClass);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(1, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(10, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(11, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(2, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(VehicleCode.CB, vehicle.VehicleCode);
            Assert.AreEqual(true, vehicle.LowEntry);
            Assert.AreEqual(2.5, vehicle.Height.Value());
            Assert.AreEqual(9.5, vehicle.Length.Value());
            Assert.AreEqual(2.5, vehicle.Width.Value());
            Assert.AreEqual(2.0, vehicle.EntranceHeight.Value());
            Assert.AreEqual(ConsumerTechnology.Electrically, vehicle.DoorDriveTechnology);
            Assert.AreEqual(VehicleDeclarationType.final, vehicle.VehicleDeclarationType);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);

            Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.InteriorLightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.DayrunninglightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.PositionlightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.BrakelightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.HeadlightsLED);
            Assert.AreEqual(BusHVACSystemConfiguration.Configuration6, vehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeHeatingDriverCompartment);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeCoolingDriverCompartment);
            Assert.AreEqual(HeatPumpType.R_744, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeCoolingPassengerCompartment);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeHeatingPassengerCompartment);
            Assert.AreEqual(50000, vehicle.Components.BusAuxiliaries.HVACAux.AuxHeaterPower.Value());
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.DoubleGlazing);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.AdjustableAuxiliaryHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.SeparateAirDistributionDucts);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.WaterElectricHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.AirElectricHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.OtherHeatingTechnology);
            Assert.IsNotNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"PEV_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_Reader_PEV_CompletedBus_requiredOnly")]
        public void TestReaderPEVCompletedBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2021-06-30T22:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual(null, vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(null, vehicle.Model);
            Assert.AreEqual(null, vehicle.LegislativeClass);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(null, vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.AirdragModifiedMultistep);
            Assert.AreEqual(null, vehicle.RegisteredClass);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(null, vehicle.VehicleCode);
            Assert.AreEqual(null, vehicle.LowEntry);
            Assert.AreEqual(null, vehicle.Height);
            Assert.AreEqual(null, vehicle.Length);
            Assert.AreEqual(null, vehicle.Width);
            Assert.AreEqual(null, vehicle.EntranceHeight);
            Assert.AreEqual(null, vehicle.DoorDriveTechnology);
            Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(DynamicChargingTechnology.None, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(null, vehicle.ADAS);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);

            Assert.IsNull(vehicle.Components);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Reader_FCHV_CompletedBus")]
        public void TestReaderFCHVCompletedBus(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2021-06-30T22:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual("x", vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual("Sample Bus Model", vehicle.Model);
            Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
            Assert.AreEqual(500.SI<Kilogram>(), vehicle.CurbMassChassis);
            Assert.AreEqual(3500.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
            Assert.AreEqual(true, vehicle.AirdragModifiedMultistep);
            Assert.AreEqual(RegistrationClass.II_III, vehicle.RegisteredClass);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(1, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(10, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(11, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(2, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(VehicleCode.CB, vehicle.VehicleCode);
            Assert.AreEqual(true, vehicle.LowEntry);
            Assert.AreEqual(2.5, vehicle.Height.Value());
            Assert.AreEqual(9.5, vehicle.Length.Value());
            Assert.AreEqual(2.5, vehicle.Width.Value());
            Assert.AreEqual(2.0, vehicle.EntranceHeight.Value());
            Assert.AreEqual(ConsumerTechnology.Electrically, vehicle.DoorDriveTechnology);
            Assert.AreEqual(VehicleDeclarationType.final, vehicle.VehicleDeclarationType);
            Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(1, vehicle.H2StorageUsableCapacity.Value());
            Assert.AreEqual(HydrogenStorageTechnology.Compressed, vehicle.HydrogenStorageTechnology);
            Assert.AreEqual(DynamicChargingTechnology.Wireless, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(false, vehicle.ADAS.EngineStopStart);
            Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
            Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);

            Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.InteriorLightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.DayrunninglightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.PositionlightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.BrakelightsLED);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.ElectricConsumers.HeadlightsLED);
            Assert.AreEqual(BusHVACSystemConfiguration.Configuration6, vehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeHeatingDriverCompartment);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeCoolingDriverCompartment);
            Assert.AreEqual(HeatPumpType.non_R_744_3_stage, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeCoolingPassengerCompartment);
            Assert.AreEqual(HeatPumpType.none, vehicle.Components.BusAuxiliaries.HVACAux.HeatPumpTypeHeatingPassengerCompartment);
            Assert.AreEqual(50000, vehicle.Components.BusAuxiliaries.HVACAux.AuxHeaterPower.Value());
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.DoubleGlazing);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.AdjustableAuxiliaryHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.SeparateAirDistributionDucts);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.WaterElectricHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.AirElectricHeater);
            Assert.AreEqual(true, vehicle.Components.BusAuxiliaries.HVACAux.OtherHeatingTechnology);
            Assert.IsNotNull(vehicle.Components.AirdragInputData);

            Assert.IsNotNull(vehicle.VehicleMonitoringData);
        }

        [TestCase(@"FCHV_CompletedBus_requiredOnly.xml", COMPLETEDBUSES, TestName = "v27_Reader_FCHV_CompletedBus_requiredOnly")]
        public void TestReaderFCHVCompletedBusRequiredOnly(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);
            var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

            Assert.NotNull(dataProvider);
            Assert.NotNull(dataProvider.JobInputData);

            var vehicle = dataProvider.JobInputData.Vehicle;
            Assert.NotNull(vehicle);

            Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
            Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
            Assert.AreEqual("VEH-1234567890", vehicle.VIN);
            Assert.AreEqual("2021-06-30T22:00:00Z", vehicle.Date.ToXmlFormat());
            Assert.AreEqual(null, vehicle.SimulationToolLicenseNumber);
            Assert.AreEqual(null, vehicle.Model);
            Assert.AreEqual(null, vehicle.LegislativeClass);
            Assert.AreEqual(null, vehicle.CurbMassChassis);
            Assert.AreEqual(null, vehicle.GrossVehicleMassRating);
            Assert.AreEqual(null, vehicle.AirdragModifiedMultistep);
            Assert.AreEqual(null, vehicle.RegisteredClass);
            Assert.AreEqual(null, vehicle.TankSystem);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingLowerDeck);
            Assert.AreEqual(null, vehicle.NumberPassengerSeatsUpperDeck);
            Assert.AreEqual(null, vehicle.NumberPassengersStandingUpperDeck);
            Assert.AreEqual(null, vehicle.VehicleCode);
            Assert.AreEqual(null, vehicle.LowEntry);
            Assert.AreEqual(null, vehicle.Height);
            Assert.AreEqual(null, vehicle.Length);
            Assert.AreEqual(null, vehicle.Width);
            Assert.AreEqual(null, vehicle.EntranceHeight);
            Assert.AreEqual(null, vehicle.DoorDriveTechnology);
            Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);
            Assert.AreEqual(null, vehicle.VehicleTypeApprovalNumber);
            Assert.AreEqual(DynamicChargingTechnology.None, vehicle.DynamicChargingTechnology);
            Assert.AreEqual(null, vehicle.ADAS);
            Assert.AreEqual(null, vehicle.H2StorageUsableCapacity);
            Assert.AreEqual(null, vehicle.HydrogenStorageTechnology);

            Assert.IsNull(vehicle.Components);

            Assert.IsNull(vehicle.VehicleMonitoringData);
        }

    }
}
