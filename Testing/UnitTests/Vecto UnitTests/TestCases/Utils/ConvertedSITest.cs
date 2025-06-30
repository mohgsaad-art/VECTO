using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto_UnitTests.TestCases.Utils
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ConvertedSITest
    {

        [Test]
        public void ConvertedSITest_ConvertToLiterPer100KiloMeter()
        {
            var fcLiter = 5.0;
            var distanceKm = 100.0;
            CubicMeter fc = fcLiter.SI(Unit.SI.Liter).Cast<CubicMeter>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();

            var converted = (fc / distance).ConvertToLiterPer100KiloMeter();

            Assert.AreEqual(fcLiter / distanceKm * 100, converted.Value, 1e-6);
            Assert.AreEqual("l/100km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_ConvertToLiterPerTonKiloMeter()
        {
            var fcLiter = 5.0;
            var distanceKm = 100.0;
            var payloadTon = 2;
            CubicMeter fc = fcLiter.SI(Unit.SI.Liter).Cast<CubicMeter>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
            Kilogram payload = payloadTon.SI(Unit.SI.Ton).Cast<Kilogram>();

            var converted = (fc / distance / payload).ConvertToLiterPerTonKiloMeter();

            Assert.AreEqual(fcLiter / distanceKm / payloadTon, converted.Value, 1e-6);
            Assert.AreEqual("l/t-km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_ConvertToLiterPerCubicMeterKiloMeter()
        {
            var fcLiter = 5.0;
            var distanceKm = 100.0;
            var cargo = 10;
            CubicMeter fc = fcLiter.SI(Unit.SI.Liter).Cast<CubicMeter>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
            CubicMeter cargoVolume = cargo.SI(Unit.SI.Cubic.Meter).Cast<CubicMeter>();

            var converted = (fc / distance / cargoVolume).ConvertToLiterPerCubicMeterKiloMeter();

            Assert.AreEqual(fcLiter / distanceKm / cargo, converted.Value, 1e-6);
            Assert.AreEqual("l/m³-km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_J_ConvertToMegaJoulePerCubicMeterKiloMeter()
        {
            var ecMj = 200.0;
            var distanceKm = 100.0;
            var cargo = 10;
            Joule ec = ecMj.SI(Unit.SI.Mega.Joule).Cast<Joule>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
            CubicMeter cargoVolume = cargo.SI(Unit.SI.Cubic.Meter).Cast<CubicMeter>();

            var converted = (ec / distance / cargoVolume).ConvertToMegaJoulePerCubicMeterKiloMeter();

            Assert.AreEqual(ecMj / distanceKm / cargo, converted.Value, 1e-6);
            Assert.AreEqual("MJ/m³-km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_ConvertToMegaJoulePerTonKiloMeter()
        {
            var ecMj = 200.0;
            var distanceKm = 100.0;
            var payloadTon = 2.5;
            Joule ec = ecMj.SI(Unit.SI.Mega.Joule).Cast<Joule>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
            Kilogram payload = payloadTon.SI(Unit.SI.Ton).Cast<Kilogram>();

            var converted = (ec / distance / payload).ConvertToMegaJoulePerTonKiloMeter();

            Assert.AreEqual(ecMj / distanceKm / payloadTon, converted.Value, 1e-6);
            Assert.AreEqual("MJ/t-km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_ConvertToMegaJoulePerKilometer()
        {
            var ecMj = 200.0;
            var distanceKm = 100.0;
            Joule ec = ecMj.SI(Unit.SI.Mega.Joule).Cast<Joule>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();

            var converted = (ec / distance).ConvertToMegaJoulePerKilometer();

            Assert.AreEqual(ecMj / distanceKm, converted.Value, 1e-6);
            Assert.AreEqual("MJ/km", converted.Units);
        }


        [Test]
        public void ConvertedSITest_ConvertToKiloWattHourPerKiloMeter()
        {
            var ec_kWh = 200.0;
            var distanceKm = 100.0;
            WattSecond ec = ec_kWh.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();

            var converted = (ec / distance).ConvertToKiloWattHourPerKiloMeter();

            Assert.AreEqual(ec_kWh / distanceKm, converted.Value, 1e-6);
            Assert.AreEqual("kWh/km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_ConvertToKiloWattHourPerTonKiloMeter()
        {
            var ec_kWh = 200.0;
            var distanceKm = 100.0;
            var payloadTon = 2.5;
            WattSecond ec = ec_kWh.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
            Kilogram payload = payloadTon.SI(Unit.SI.Ton).Cast<Kilogram>();

            var converted = (ec / distance / payload).ConvertToKiloWattHourPerTonKiloMeter();

            Assert.AreEqual(ec_kWh / distanceKm / payloadTon, converted.Value, 1e-6);
            Assert.AreEqual("kWh/t-km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_ConvertToKiloWattHourPerCubicMeterKiloMeter()
        {
            var ec_kWh = 200.0;
            var distanceKm = 100.0;
            var cargo = 10;
            WattSecond ec = ec_kWh.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
            CubicMeter cargoVolume = cargo.SI(Unit.SI.Cubic.Meter).Cast<CubicMeter>();

            var converted = (ec / distance / cargoVolume).ConvertToKiloWattHourPerCubicMeterKiloMeter();

            Assert.AreEqual(ec_kWh / distanceKm / cargo, converted.Value, 1e-6);
            Assert.AreEqual("kWh/m³-km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_Ws_ConvertToMegaJoulePerKiloMeter()
        {
            var ec_MJ = 200.0;
            var distanceKm = 100.0;
            WattSecond ec = ec_MJ.SI(Unit.SI.Mega.Joule).Cast<WattSecond>();
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();

            var converted = (ec / distance).ConvertToMegaJoulePerKiloMeter();

            Assert.AreEqual(ec_MJ / distanceKm, converted.Value, 1e-6);
            Assert.AreEqual("MJ/km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_Ws_ConvertToMegaJoulePerTonKiloMeter()
        {
            var ec_MJ = 200.0;
            var distanceKm = 100.0;
            WattSecond ec = ec_MJ.SI(Unit.SI.Mega.Joule).Cast<WattSecond>();
            var payloadTon = 2.5;
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
            Kilogram payload = payloadTon.SI(Unit.SI.Ton).Cast<Kilogram>();

            var converted = (ec / distance / payload).ConvertToMegaJoulePerTonKiloMeter();

            Assert.AreEqual(ec_MJ / distanceKm / payloadTon, converted.Value, 1e-6);
            Assert.AreEqual("MJ/t-km", converted.Units);
        }

        [Test]
        public void ConvertedSITest_Ws_ConvertToMegaJoulePerCubicMeterKiloMeter()
        {
            var ec_MJ = 200.0;
            var distanceKm = 100.0;
            WattSecond ec = ec_MJ.SI(Unit.SI.Mega.Joule).Cast<WattSecond>();
            var cargo = 10;
            Meter distance = distanceKm.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
            CubicMeter cargoVolume = cargo.SI(Unit.SI.Cubic.Meter).Cast<CubicMeter>();

            var converted = (ec / distance / cargoVolume).ConvertToMegaJoulePerCubicMeterKiloMeter();

            Assert.AreEqual(ec_MJ / distanceKm / cargo, converted.Value, 1e-6);
            Assert.AreEqual("MJ/m³-km", converted.Units);
        }
    }

}