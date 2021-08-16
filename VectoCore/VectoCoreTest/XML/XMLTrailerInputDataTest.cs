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

		private const string SampleTrailer = @"TestData\Trailer\TEST_CASE.xml";

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
			var declarationSegmentTrailerTest = new DeclarationSegmentTrailerTest();
			declarationSegmentTrailerTest.TrailerLookupTest(inputDataProvider);

			//Aerofeature lookup
			declarationSegmentTrailerTest.TrailerAeroFeaturesTest();

			//Corrections
			TrailerCorrectionsTest();

			//Missions Test
			TrailerMissionsTest();

			//Report XML schema
			var xmlReportTrailerTest = new XMLReportTrailerTest();
			xmlReportTrailerTest.RunBeforeAnyTests();
			xmlReportTrailerTest.RunTrailerDeclarationJob();
		}

		[TestCase]
		public void TestXMLInputDeclTrailer()
		{
			//general info
			Assert.AreEqual("Manufacturer", inputDataProvider.JobInputData.Trailer.Manufacturer);
			Assert.AreEqual("ManufacturerAddress", inputDataProvider.JobInputData.Trailer.ManufacturerAddress);
			//Assert.AreEqual("Model", inputDataProvider.JobInputData.Trailer.TrailerModel);
			Assert.AreEqual("123-456-789", inputDataProvider.JobInputData.Trailer.VIN);
			Assert.AreEqual("O4", inputDataProvider.JobInputData.Trailer.LegislativeCategory);
			Assert.AreEqual(NumberOfTrailerAxles.Two, inputDataProvider.JobInputData.Trailer.NumberOfAxles);
			Assert.AreEqual(TypeTrailer.DB, inputDataProvider.JobInputData.Trailer.TrailerType);
			Assert.AreEqual(BodyWorkCode.Drybody, inputDataProvider.JobInputData.Trailer.BodyworkCode);
			Assert.AreEqual(TrailerCouplingPoint.Unknown, inputDataProvider.JobInputData.Trailer.TrailerCouplingPoint);

			//weights
			Assert.AreEqual(Kilogram.Create(7500), inputDataProvider.JobInputData.Trailer.MassInRunningOrder);
			Assert.AreEqual(Kilogram.Create(39000), inputDataProvider.JobInputData.Trailer.TPMLMTotalTrailer);
			Assert.AreEqual(Kilogram.Create(0), inputDataProvider.JobInputData.Trailer.TPMLMAxleAssembly);

			//lengths
			Assert.AreEqual(Meter.Create(7.400), inputDataProvider.JobInputData.Trailer.ExternalBodyLength);
			Assert.AreEqual(Meter.Create(2.550), inputDataProvider.JobInputData.Trailer.ExternalBodyWidth);
			Assert.AreEqual(Meter.Create(2.730), inputDataProvider.JobInputData.Trailer.ExternalBodyHeight);
			Assert.AreEqual(Meter.Create(4.000), inputDataProvider.JobInputData.Trailer.TotalTrailerHeight);
			Assert.AreEqual(Meter.Create(1.450), inputDataProvider.JobInputData.Trailer.LengthFromFrontToFirstAxle);
			Assert.AreEqual(Meter.Create(5.300), inputDataProvider.JobInputData.Trailer.LengthBetweenCentersOfAxles);
			Assert.AreEqual(Meter.Create(2.700), inputDataProvider.JobInputData.Trailer.InternalBodyHeight);

			Assert.AreEqual(CubicMeter.Create(91.000), inputDataProvider.JobInputData.Trailer.CargoVolume);

			Assert.AreEqual(0.00, inputDataProvider.JobInputData.Trailer.YawAngle0);
			Assert.AreEqual(0.00, inputDataProvider.JobInputData.Trailer.YawAngle3);
			Assert.AreEqual(0.00, inputDataProvider.JobInputData.Trailer.YawAngle6);
			Assert.AreEqual(0.00, inputDataProvider.JobInputData.Trailer.YawAngle9);
		}

		[TestCase]
		public void TrailerCorrectionsTest()
		{
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(inputDataProvider.JobInputData.Trailer.NumberOfAxles, inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode, false, null);

			var trailerDataAdapter = new DeclarationTrailerDataAdapter();

			var genericInputDataProvider = DeclarationData.TrailerSegments.GetGenericVehicleData(trailerSegment.GenericCode);

			var specificTrailerData = trailerDataAdapter.GetTrailerData(trailerSegment, inputDataProvider.JobInputData.Trailer, genericInputDataProvider.JobInputData.Vehicle);

			Assert.AreEqual(SquareMeter.Create(0.002002076364697263), specificTrailerData.TotalCorrection.A1);
			Assert.AreEqual(SquareMeter.Create(0.00435041028151623), specificTrailerData.TotalCorrection.A2);
			Assert.AreEqual(SquareMeter.Create(-0.00014910803563933207), specificTrailerData.TotalCorrection.A3);
			Assert.AreEqual(SquareMeter.Create(5.2740394728833708), specificTrailerData.TotalCorrection.CdxA0);
			Assert.AreEqual(SquareMeter.Create(5.3151734775489068), specificTrailerData.TotalCorrection.CdxA3);
			Assert.AreEqual(SquareMeter.Create(5.4104593655159938), specificTrailerData.TotalCorrection.CdxA6);
			Assert.AreEqual(SquareMeter.Create(5.5357416349869046), specificTrailerData.TotalCorrection.CdxA9);
			Assert.AreEqual(SquareMeter.Create(0.00), specificTrailerData.TotalCorrection.CdxA00);
			Assert.AreEqual(SquareMeter.Create(0.041134004665535961), specificTrailerData.TotalCorrection.CdxA03);
			Assert.AreEqual(SquareMeter.Create(0.136419892632623), specificTrailerData.TotalCorrection.CdxA06);
			Assert.AreEqual(SquareMeter.Create(0.26170216210353381), specificTrailerData.TotalCorrection.CdxA09);

		}

		[TestCase]
		public void TrailerMissionsTest()
		{
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(
				inputDataProvider.JobInputData.Trailer.NumberOfAxles,
				inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode,
				false, null);

			Assert.AreEqual(3, trailerSegment.Missions.Count());
			Assert.AreEqual(1, trailerSegment.Missions.Count(x=> x.MissionType == MissionType.LongHaul));
			Assert.AreEqual(1, trailerSegment.Missions.Count(x => x.MissionType == MissionType.RegionalDelivery));
			Assert.AreEqual(1, trailerSegment.Missions.Count(x => x.MissionType == MissionType.UrbanDelivery));

			Assert.AreEqual(Kilogram.Create(2600), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.LongHaul).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(19300), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.LongHaul).SpecificTrailerRefLoad);
			
			Assert.AreEqual(Kilogram.Create(2600), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.RegionalDelivery).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(12900), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.RegionalDelivery).SpecificTrailerRefLoad);
			
			Assert.AreEqual(Kilogram.Create(2600), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.UrbanDelivery).SpecificTrailerLowLoad);
			Assert.AreEqual(Kilogram.Create(12900), trailerSegment.Missions.FirstOrDefault(x => x.MissionType == MissionType.UrbanDelivery).SpecificTrailerRefLoad);

		}
	}
}
