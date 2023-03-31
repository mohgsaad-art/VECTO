using NUnit.Framework;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{

	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ConvertedSITest
	{
		[Test]
		public void ConvertedSITest_ConvertToGram()
		{
			var value = 1234.5678;
			var si = value.SI(Unit.SI.Gramm).Cast<Kilogram>();

			var converted = si.ConvertToGramm();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("g", converted.Units);
		}

		[Test]
		public void ConvertedSITest_ConvertToTon()
		{
			var value = 1234.5678;
			var si = value.SI(Unit.SI.Ton).Cast<Kilogram>();

			var converted = si.ConvertToTon();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("t", converted.Units);
		}

		[Test]
		public void ConvertedSITest_ConvertToKiloMeterPerHour()
		{
			var value = 1234.5678;
			var si = value.SI(Unit.SI.Kilo.Meter.Per.Hour).Cast<MeterPerSecond>();

			var converted = si.ConvertToKiloMeterPerHour();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("km/h", converted.Units);
		}

		[Test]
		public void ConvertedSITest_ConvertToGrammPerKiloMeter()
		{
			var value = 1234.5678;
			var si = value.SI(Unit.SI.Gramm.Per.Kilo.Meter).Cast<KilogramPerMeter>();

			var converted = si.ConvertToGrammPerKiloMeter();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("g/km", converted.Units);
		}

		[Test]
		public void ConvertedSITest_ConvertToGramPerKiloWattHour()
		{
			var value = 1234.5678;
			var si = value.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<SpecificFuelConsumption>();

			var converted = si.ConvertToGramPerKiloWattHour();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("g/kWh", converted.Units);
		}


		[Test]
		public void ConvertedSITest_ConvertToMilliGramPerKiloWattHour()
		{
			var value = 1234.5678;
			var mgpkwh = value.SI(Unit.SI.Milli.Gramm.Per.Kilo.Watt.Hour).Cast<KilogramPerWattSecond>();

			var converted = mgpkwh.ConvertToMilliGramPerKiloWattHour();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("mg/kWh", converted.Units);
		}

		[Test]
		public void ConvertedSITest_ConvertToGramPerKiloWattHour_2()
		{
			var value = 1234.5678;
			var gpkwh = value.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<KilogramPerWattSecond>();

			var converted = gpkwh.ConvertToGramPerKiloWattHour();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("g/kWh", converted.Units);
		}


		[Test]
		public void ConvertedSITest_ConvertToPerKiloWattHour()
		{
			var value = 1234.5678;
			var pkwh = value.SI(Unit.SI.Per.Kilo.Watt.Hour).Cast<PerWattSecond>();

			var converted = pkwh.ConvertToPerKiloWattHour();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("#/kWh", converted.Units);
		}

		[Test]
		public void ConvertedSITest_ConvertToKiloWattHour()
		{
			var value = 1234.5678;
			var pkwh = value.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();

			var converted = pkwh.ConvertToKiloWattHour();

			Assert.AreEqual(value, converted.Value, 1e-6);
			Assert.AreEqual("kWh", converted.Units);
		}
	}

}