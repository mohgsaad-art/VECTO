using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Tests.Models.Declaration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;

namespace TUGraz.VectoCore.Tests.XML
{

	public class XMLTrailerInputDataTest
	{
		protected IXMLInputDataReader XMLInputReader;
		private IKernel _kernel;

		private const string SampleTrailer = @"TestData\Trailer\test4.xml";

		private IDeclarationTrailerInputDataProvider inputDataProvider;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();

			var reader = XmlReader.Create(SampleTrailer);
			inputDataProvider = XMLInputReader.CreateDeclarationTrailer(reader);
		}

		[TestCase]
		public void TrailerFullTest()
		{
			//InputData
			TestXMLInputDeclTrailer();

			//Trailer Segment lookup
			DeclarationSegmentTrailerTest declarationSegmentTrailerTest = new DeclarationSegmentTrailerTest();
			declarationSegmentTrailerTest.TrailerSegmentLookupTest(inputDataProvider);

			//Aerofeature lookup
			declarationSegmentTrailerTest.TrailerAeroFeaturesTest();

			//Specific Trailer Data
			SpecificTrailerDataTest();

			//Missions Test
			TrailerMissionsTest();

			//Report XML schema
			XMLReportTrailerTest xmlReportTrailerTest = new XMLReportTrailerTest();
			xmlReportTrailerTest.RunBeforeAnyTests();
			xmlReportTrailerTest.RunTrailerDeclarationJob();
		}

		[TestCase]
		public void TestXMLInputDeclTrailer()
		{
			//general info
			Assert.AreEqual("testing123", inputDataProvider.JobInputData.Trailer.Manufacturer);
			Assert.AreEqual("testing123", inputDataProvider.JobInputData.Trailer.ManufacturerAddress);
			Assert.AreEqual("Generic Tyre Model", inputDataProvider.JobInputData.Trailer.Model);
			Assert.AreEqual("123-123-123", inputDataProvider.JobInputData.Trailer.VIN);
			Assert.AreEqual("O4", inputDataProvider.JobInputData.Trailer.LegislativeCategory);
			Assert.AreEqual(NumberOfTrailerAxles.Two, inputDataProvider.JobInputData.Trailer.NumberOfAxles);
			Assert.AreEqual(TypeTrailer.DA, inputDataProvider.JobInputData.Trailer.TrailerType);
			Assert.AreEqual(BodyWorkCode.Drybody, inputDataProvider.JobInputData.Trailer.BodyworkCode);

			//weights
			Assert.AreEqual(Kilogram.Create(7000), inputDataProvider.JobInputData.Trailer.MassInRunningOrder);
			Assert.AreEqual(Kilogram.Create(18000), inputDataProvider.JobInputData.Trailer.TPMLMTotalTrailer);
			Assert.AreEqual(Kilogram.Create(17000), inputDataProvider.JobInputData.Trailer.TPMLMAxleAssembly);

			//lengths
			Assert.AreEqual(Meter.Create(8.740), inputDataProvider.JobInputData.Trailer.ExternalBodyLength);
			Assert.AreEqual(Meter.Create(2.550), inputDataProvider.JobInputData.Trailer.ExternalBodyWidth);
			Assert.AreEqual(Meter.Create(4.250), inputDataProvider.JobInputData.Trailer.ExternalBodyHeight);
			Assert.AreEqual(Meter.Create(5.250), inputDataProvider.JobInputData.Trailer.TotalTrailerHeight);
			Assert.AreEqual(Meter.Create(5.333), inputDataProvider.JobInputData.Trailer.LengthFromFrontToFirstAxle);
			Assert.AreEqual(Meter.Create(7.550), inputDataProvider.JobInputData.Trailer.LengthBetweenCentersOfAxles);
			Assert.AreEqual(Meter.Create(4.000), inputDataProvider.JobInputData.Trailer.InternalBodyHeight);

			Assert.AreEqual(CubicMeter.Create(86.700), inputDataProvider.JobInputData.Trailer.CargoVolume);
		}

		[TestCase]
		public void SpecificTrailerDataTest()
		{
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(inputDataProvider.JobInputData.Trailer.NumberOfAxles, inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode, false);

			var trailerDataAdapter = new DeclarationTrailerDataAdapter();

			var genericInputDataProvider = DeclarationData.TrailerSegments.GetGenericVehicleData(trailerSegment.GenericCode);

			var specificTrailerData = trailerDataAdapter.GetSpecificTrailerData(trailerSegment, inputDataProvider.JobInputData.Trailer, genericInputDataProvider.JobInputData.Vehicle);

			Assert.AreEqual(Kilogram.Create(7000), specificTrailerData.CurbMassTrailer);
			Assert.AreEqual(35000, specificTrailerData.MaxGcm);
			Assert.AreEqual(KilogramSquareMeter.Create(14.9), specificTrailerData.Inertia);
			Assert.AreEqual(6.84625, specificTrailerData.TotalCorrection);
		}

		[TestCase]
		public void TrailerMissionsTest()
		{
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(
				inputDataProvider.JobInputData.Trailer.NumberOfAxles,
				inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode,
				false);

			Assert.AreEqual(3, trailerSegment.Missions.Count());
			Assert.AreEqual(1, trailerSegment.Missions.Count(x=> x.MissionType == MissionType.LongHaul));
			Assert.AreEqual(1, trailerSegment.Missions.Count(x => x.MissionType == MissionType.RegionalDelivery));
			Assert.AreEqual(1, trailerSegment.Missions.Count(x => x.MissionType == MissionType.UrbanDelivery));

			Assert.AreEqual(Kilogram.Create(2300), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.LongHaul).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(17400), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.LongHaul).SpecificTrailerRefLoad);
			
			Assert.AreEqual(Kilogram.Create(2300), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.RegionalDelivery).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(11600), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.RegionalDelivery).SpecificTrailerRefLoad);
			
			Assert.AreEqual(Kilogram.Create(2300), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.UrbanDelivery).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(11600), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.UrbanDelivery).SpecificTrailerRefLoad);

		}
	}
}
