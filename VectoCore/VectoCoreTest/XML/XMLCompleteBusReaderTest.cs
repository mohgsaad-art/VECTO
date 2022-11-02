using System;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;

namespace TUGraz.VectoCore.Tests.XML
{

	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLCompleteBusReaderTest
	{
		private const string CompleteBusExample =
			"TestData/XML/XMLReaderDeclaration/SchemaVersion2.4/vecto_vehicle-completed_heavyBus-sample.xml";



		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}


		[TestCase,
		Ignore("CompleteBus no longer used - new multistep approach")]
		public void TestCompleteBusVehicleData()
		{
			var reader = XmlReader.Create(CompleteBusExample);

			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			Assert.IsNotNull(vehicle);
			Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
			Assert.AreEqual("Infinite Loop 1", vehicle.ManufacturerAddress);
			Assert.AreEqual("Sample Bus Model", vehicle.Model);
			Assert.AreEqual("VEH-1234567890", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2020-01-09T11:00:00Z").ToUniversalTime(), vehicle.Date);
			Assert.IsTrue(vehicle.LegislativeClass == LegislativeClass.N2);
			Assert.AreEqual("II+III", vehicle.RegisteredClass.GetLabel());
			Assert.IsTrue(vehicle.VehicleCode == VehicleCode.CD);
			Assert.AreEqual(8300, vehicle.CurbMassChassis.Value());
			Assert.AreEqual(15400, vehicle.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass ?!?
			//Assert.That(() => vehicle.TankSystem, Throws.InstanceOf<VectoException>());
			Assert.IsNull(vehicle.TankSystem);
			Assert.AreEqual(50, vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(0, vehicle.NumberPassengerSeatsUpperDeck);
			Assert.IsTrue(vehicle.LowEntry);
			Assert.AreEqual(2.700, vehicle.Height.Value());
			Assert.AreEqual(11.830, vehicle.Length.Value());
			Assert.AreEqual(2.550, vehicle.Width.Value());
			Assert.AreEqual(0.120, vehicle.EntranceHeight.Value());
			Assert.AreEqual(ConsumerTechnology.Pneumatically, vehicle.DoorDriveTechnology);
	
			var components = inputDataProvider.JobInputData.Vehicle.Components;
			Assert.IsNotNull(components);
			
			var airDrag = components.AirdragInputData;

			Assert.AreEqual("Generic Manufacturer", airDrag.Manufacturer);
			Assert.AreEqual("Generic Model", airDrag.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", airDrag.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-03-24T15:00:00Z").ToUniversalTime(), airDrag.Date.ToUniversalTime());
			Assert.AreEqual("Vecto AirDrag x.y", airDrag.AppVersion);
			//CdxA ?!?
			//TransferredCdxA ?!? 
			Assert.AreEqual(6.34, airDrag.AirDragArea.Value());
			
			Assert.AreEqual("#CabinX23h", airDrag.DigestValue.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", airDrag.DigestValue.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", airDrag.DigestValue.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", airDrag.DigestValue.DigestMethod);
			Assert.AreEqual("b9SHCfOoVrBxFQ8wwDK32OO+9bd85DuaUdgs6j/29N8=", airDrag.DigestValue.DigestValue);

			var auxiliaries = components.BusAuxiliaries;
			Assert.IsFalse(auxiliaries.ElectricConsumers.DayrunninglightsLED);
			Assert.IsFalse(auxiliaries.ElectricConsumers.HeadlightsLED);
			Assert.IsFalse(auxiliaries.ElectricConsumers.PositionlightsLED);
			Assert.IsFalse(auxiliaries.ElectricConsumers.BrakelightsLED);
			Assert.IsFalse(auxiliaries.ElectricConsumers.InteriorLightsLED);
			
			var electricSupl = components.BusAuxiliaries.ElectricSupply;
			Assert.IsNotNull(electricSupl.Alternators);
			Assert.AreEqual(1, electricSupl.Alternators.Count);
			Assert.AreEqual(AlternatorType.Conventional, electricSupl.AlternatorTechnology);

			//Assert.AreEqual(ConsumerTechnology.Pneumatically, components.BusAuxiliaries.PneumaticConsumers.DoorDriveTechnology);

			var havacAux = components.BusAuxiliaries.HVACAux;
			Assert.IsNotNull(havacAux);
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration7, havacAux.SystemConfiguration);
			Assert.AreEqual(0.SI<Watt>(), havacAux.AuxHeaterPower);
			Assert.IsTrue(havacAux.DoubleGlazing);
			Assert.IsTrue(havacAux.AdjustableAuxiliaryHeater);
			Assert.IsTrue(havacAux.SeparateAirDistributionDucts);
		}
	}
}
