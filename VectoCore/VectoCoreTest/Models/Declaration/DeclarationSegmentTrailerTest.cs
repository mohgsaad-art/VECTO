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
			var trailerSegment = DeclarationData.TrailerSegments.Lookup(inputDataProvider.JobInputData.Trailer.NumberOfAxles, inputDataProvider.JobInputData.Trailer.TrailerType, inputDataProvider.JobInputData.Trailer.BodyworkCode, false, inputDataProvider.JobInputData.Trailer.TPMLMAxleAssembly);
			Assert.AreEqual(25421, trailerSegment.VehicleGroup);
			Assert.AreEqual("40000", trailerSegment.MaxGcm);
			Assert.AreEqual(VehicleClass.Class9, trailerSegment.Class);
			Assert.AreEqual("R2", trailerSegment.GenericCode);
			Assert.AreEqual(5.23, trailerSegment.GenericCadVehicle.CdxA0);
			Assert.AreEqual(0.02105344, trailerSegment.GenericCadVehicle.DCdf0);
			Assert.AreEqual(0.01805329, trailerSegment.GenericCadVehicle.DCda0);
			Assert.AreEqual(0.109850923, trailerSegment.GenericCadVehicle.DCdr0);
			Assert.AreEqual(SquareMeter.Create(-0.00167), trailerSegment.GenericCadVehicle.A1);
			Assert.AreEqual(SquareMeter.Create(0.00222), trailerSegment.GenericCadVehicle.A2);
			Assert.AreEqual(SquareMeter.Create(0), trailerSegment.GenericCadVehicle.A3);
			Assert.AreEqual(Meter.Create(2.895), trailerSegment.GenericCadVehicle.Lbf);
			Assert.AreEqual(Meter.Create(2.43), trailerSegment.GenericCadVehicle.Lba);
			Assert.AreEqual(Meter.Create(2.495), trailerSegment.GenericCadVehicle.Lbr);
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
