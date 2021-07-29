using System;
using System.Collections.Generic;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	public class DeclarationSegmentTrailerTest
	{
		[TestCase]
		public void TrailerLookupTest(IDeclarationTrailerInputDataProvider inputDataProvider)
		{
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(inputDataProvider.JobInputData.Trailer.NumberOfAxles, inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode, false, null);
			Assert.AreEqual(23101, trailerSegment.VehicleGroup);
			Assert.AreEqual("40000", trailerSegment.MaxGcm);
			Assert.AreEqual(VehicleClass.Class9, trailerSegment.Class);
			Assert.AreEqual("R2", trailerSegment.GenericCode);
			Assert.AreEqual(5.23, trailerSegment.GenericCadVehicle.CdxA0);
			Assert.AreEqual(0.012906274, trailerSegment.GenericCadVehicle.DCdf0);
			Assert.AreEqual(0.013634074, trailerSegment.GenericCadVehicle.DCda0);
			Assert.AreEqual(0.013794079, trailerSegment.GenericCadVehicle.DCdr0);
			Assert.AreEqual(SquareMeter.Create(0.015111), trailerSegment.GenericCadVehicle.A1);
			Assert.AreEqual(SquareMeter.Create(-0.001167), trailerSegment.GenericCadVehicle.A2);
			Assert.AreEqual(SquareMeter.Create(0.000228), trailerSegment.GenericCadVehicle.A3);
			Assert.AreEqual(Meter.Create(1.962), trailerSegment.GenericCadVehicle.Lbf);
			Assert.AreEqual(Meter.Create(4.228), trailerSegment.GenericCadVehicle.Lba);
			Assert.AreEqual(Meter.Create(0.890336957), trailerSegment.GenericCadVehicle.Lbr);
		}		


		[TestCase]
		public void TrailerAeroFeaturesTest()
		{
			var aeroFeatures = new List<AeroFeatureTechnology> { AeroFeatureTechnology.SideSkirtsShort };

			var combination = DeclarationData.AeroFeatures.Lookup(aeroFeatures);
			Assert.AreEqual(1, combination);
		}

	}
}
