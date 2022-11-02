using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ShiftstrategyParameterTests
	{

		[TestCase(0.3, 1.0),
		TestCase(0.8, 1.0),
		TestCase(0.9, 0.5),
		TestCase(1.5, 0.5),
		TestCase(0.85, 0.75)
		]
		public void TestPredictionIntervalLookup(double speedRatio, double expected)
		{
			var lookup = PredictionDurationLookupReader.ReadFromStream(
				RessourceHelper.ReadStream(
					DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.PredictionTimeLookup.csv"));
			var val = lookup.Lookup(speedRatio);

			Assert.AreEqual(expected, val, 1e-6);
		}


		[TestCase(10, 0.06),
		 TestCase(30, 0.15),
		 TestCase(15, 0.1),
		 TestCase(25, 0.145),
		 TestCase(120, 0.15),]
		public void TestShareIdleLowLookup(double velocity, double expected)
		{
			var lookup = ShareIdleLowReader.ReadFromStream(
				RessourceHelper.ReadStream(
					DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareIdleLow.csv"));
			var val = lookup.Lookup(velocity.KMPHtoMeterPerSecond());

			Assert.AreEqual(expected, val, 1e-6);
		}


		[TestCase(0, 0.0),
		 TestCase(0.4, 0.0),
		 TestCase(0.55, 0.3),
		 TestCase(1.1, 1.0),
			]
		public void TestShareEngineSpeedHighLookup(double torqueRatio, double expected)
		{
			var lookup = EngineSpeedHighLookupReader.ReadFromStream(
				RessourceHelper.ReadStream(
					DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareEngineSpeedHigh.csv"));
			var val = lookup.Lookup(torqueRatio);

			Assert.AreEqual(expected, val, 1e-6);
		}

		[TestCase(0, 0.0),
		TestCase(30, -0.05),
		TestCase(50, -0.1),
		TestCase(120, -0.1),
		TestCase(25, -0.025),
		TestCase(45, -0.075)]
		public void TestAccelerationReserveLow(double velocity, double expected)
		{
			var lookup = AccelerationReserveLookupReader.ReadFromStream(
				RessourceHelper.ReadStream(
					DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.AccelerationReserveLookup.csv"));
			var val = lookup.LookupLow(velocity.KMPHtoMeterPerSecond());

			Assert.AreEqual(expected, val.Value(), 1e-6);
		}

		[TestCase(0, 0.15),
		TestCase(30, 0.1),
		TestCase(70, 0.05),
		TestCase(120, 0.05),
		TestCase(25, 0.125),
		TestCase(65, 0.075)]
		public void TestAccelerationReserveHigh(double velocity, double expected)
		{
			var lookup = AccelerationReserveLookupReader.ReadFromStream(
				RessourceHelper.ReadStream(
					DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.AccelerationReserveLookup.csv"));
			var val = lookup.LookupHigh(velocity.KMPHtoMeterPerSecond());

			Assert.AreEqual(expected, val.Value(), 1e-6);
		}

		[TestCase(0, 0.6),
			TestCase(20, 0.8),
			TestCase(30, 1.0),
			TestCase(110, 1.0),
			TestCase(15, 0.7),
			TestCase(25, 0.9),]
		public void TestShareTq99l(double velcoity, double expected)
		{
			var lookup = ShareTorque99lLookupReader.ReadFromStream(
				RessourceHelper.ReadStream(
					DeclarationData.DeclarationDataResourcePrefix + ".GearshiftParameters.ShareTq99L.csv"));
			var val = lookup.Lookup(velcoity.KMPHtoMeterPerSecond());

			Assert.AreEqual(expected, val, 1e-6);
		}
	}
}
