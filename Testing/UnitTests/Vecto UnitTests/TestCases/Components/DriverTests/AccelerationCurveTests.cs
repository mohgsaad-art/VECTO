using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.DriverTests;

public class AccelerationCurveTests
{
	public const double Tolerance = 0.0001;

	
    [
		// FIXED POINTS
        TestCase(0, 1.01570922360353, -0.231742702878269),
		TestCase(5, 1.38546581120225, -0.45346198022574),
		TestCase(10, 1.34993329755465, -0.565404125020508),
		TestCase(15, 1.29026714002479, -0.703434814668512),
		TestCase(20, 1.16369598822194, -0.677703399378421),
		TestCase(25, 1.04024417156355, -0.63631961226452),
		TestCase(30, 0.910278494884728, -0.548894523516266),
		TestCase(35, 0.785875078338323, -0.453995336940216),
		TestCase(40, 0.69560012996407, -0.385460695652016),
		TestCase(45, 0.648984223443223, -0.349181329186105),
		TestCase(50, 0.594249623931624, -0.309125096967231),
		TestCase(55, 0.559156929181929, -0.296716093796643),
		TestCase(60, 0.541508805860806, -0.270229542673924),
		TestCase(65, 0.539582904761905, -0.256408113084341),
		TestCase(70, 0.539103523809524, -0.217808535739946),
		TestCase(75, 0.529581598997494, -0.18609307386602),
		TestCase(80, 0.496418462064251, -0.142683384645006),
		TestCase(85, 0.453932619248656, -0.117950211164234),
		TestCase(90, 0.397824554210839, -0.102997621205622),
		TestCase(95, 0.33969661577071, -0.102997621205622),
		TestCase(100, 0.289428370365158, -0.102997621205622),
		TestCase(105, 0.256471472751248, -0.102997621205622),
		TestCase(110, 0.24, -0.102997621205622),
		TestCase(115, 0.22, -0.102997621205622),
		TestCase(120, 0.2, -0.102997621205622),

		// INTERPOLATED POINTS
        TestCase(0, 1.015709224, -0.231742703),
		TestCase(2.5, 1.200587517, -0.342602342),
		TestCase(7.5, 1.367699554, -0.509433053),
		TestCase(12.5, 1.320100219, -0.63441947),
		TestCase(17.5, 1.226981564, -0.690569107),
		TestCase(22.5, 1.10197008, -0.657011506),
		TestCase(27.5, 0.975261333, -0.592607068),
		TestCase(32.5, 0.848076787, -0.50144493),
		TestCase(37.5, 0.740737604, -0.419728016),
		TestCase(42.5, 0.672292177, -0.367321012),
		TestCase(47.5, 0.621616924, -0.329153213),
		TestCase(52.5, 0.576703277, -0.302920595),
		TestCase(57.5, 0.550332868, -0.283472818),
		TestCase(62.5, 0.540545855, -0.263318828),
		TestCase(67.5, 0.539343214, -0.237108324),
		TestCase(72.5, 0.534342561, -0.201950805),
		TestCase(77.5, 0.513000031, -0.164388229),
		TestCase(82.5, 0.475175541, -0.130316798),
		TestCase(87.5, 0.425878587, -0.110473916),
		TestCase(92.5, 0.368760585, -0.102997621),
		TestCase(97.5, 0.314562493, -0.102997621),
		TestCase(102.5, 0.272949922, -0.102997621),
		TestCase(107.5, 0.248235736, -0.102997621),
		TestCase(112.5, 0.23, -0.102997621),
		TestCase(117.5, 0.21, -0.102997621),

		// EXTRAPOLATE
        TestCase(130, 0.16, -0.103)
    ]
    public void AccelerationTest(double velocity, double expectedAcceleration, double expectedDeceleration)
    {
        var accCurve = AccelerationCurveReader.ReadFromStream(InputDataHelper.InputDataAsStream(Header, CoachData));

		var entry = accCurve.Lookup(velocity.KMPHtoMeterPerSecond());
		Assert.AreEqual(expectedAcceleration, entry.Acceleration.Value(), Tolerance);
		Assert.AreEqual(expectedDeceleration, entry.Deceleration.Value(), Tolerance);

    }

    [
     // in this part the deceleration is constant
     TestCase(25, 0, 24.11265432099),
     TestCase(25, 15, 15.43209876543),
     TestCase(50, 0, 96.45061728395),
     TestCase(50, 15, 87.77006172840),
     TestCase(100, 60, 493.82716049383),
     // decelerate in the non-constant part only
     TestCase(60, 50, 59.44491148),
     TestCase(59, 55, 27.33155090),
     // decelerate across multiple areas of acceleration curve
     TestCase(60, 0, 59.44491148 + 96.45061728395),
     TestCase(100, 0, 59.44491148 + 96.45061728395 + 493.82716049383)
        ]
    public void ComputeAccelerationDistanceTest(double v1, double v2, double expectedDistance)
    {
        var accData = AccelerationCurveReader.ReadFromStream(InputDataHelper.InputDataAsStream(Header, TruckData));

        var result = accData.ComputeDecelerationDistance(v1.KMPHtoMeterPerSecond(), v2.KMPHtoMeterPerSecond());
        Assert.AreEqual(expectedDistance, result.Value(), Tolerance);

    }

    [TestCase(20, 1, 23.6),
     TestCase(20, 3, 30.56621),
     TestCase(20, 7, 42.54667),
     TestCase(20, 10, 50.0461),
     TestCase(20, 13, 56.4733),
     TestCase(20, 18, 65.6398),
     TestCase(30, 3, 39.2932),
     TestCase(30, 15, 65.3336),
     TestCase(50, 5, 60.2040),
     TestCase(50, 15, 78.2040),
     TestCase(55, 15, 82.3264),
     TestCase(65, 15, 92),
     TestCase(20, 300, 120)
     ]
    public void ComputeEndVelocity(double startSpeed, double accTime, double expectedVelocity)
    {
		var accData = AccelerationCurveReader.ReadFromStream(InputDataHelper.InputDataAsStream(Header, TruckData));

        var result = accData.ComputeEndVelocityAccelerate(startSpeed.KMPHtoMeterPerSecond(), accTime.SI<Second>());
        Assert.AreEqual(expectedVelocity, result.AsKmph, 1e-3);
    }

	public const string Header = "v [km/h],acc [m/s²],dec [m/s²]";

	public readonly string[] CoachData = new[] {
		"0,1.01570922360353,-0.231742702878269",
		"5,1.38546581120225,-0.45346198022574",
		"10,1.34993329755465,-0.565404125020508",
		"15,1.29026714002479,-0.703434814668512",
		"20,1.16369598822194,-0.677703399378421",
		"25,1.04024417156355,-0.63631961226452",
		"30,0.910278494884728,-0.548894523516266",
		"35,0.785875078338323,-0.453995336940216",
		"40,0.69560012996407,-0.385460695652016",
		"45,0.648984223443223,-0.349181329186105",
		"50,0.594249623931624,-0.309125096967231",
		"55,0.559156929181929,-0.296716093796643",
		"60,0.541508805860806,-0.270229542673924",
		"65,0.539582904761905,-0.256408113084341",
		"70,0.539103523809524,-0.217808535739946",
		"75,0.529581598997494,-0.18609307386602",
		"80,0.496418462064251,-0.142683384645006",
		"85,0.453932619248656,-0.117950211164234",
		"90,0.397824554210839,-0.102997621205622",
		"95,0.33969661577071,-0.102997621205622",
		"100,0.289428370365158,-0.102997621205622",
		"105,0.256471472751248,-0.102997621205622",
		"110,0.24,-0.102997621205622",
		"115,0.22,-0.102997621205622",
		"120,0.2,-0.102997621205622",
	};

	public readonly string[] TruckData = new[] { 
		"0,1,-1",
		"25,1,-1",
		"50,0.642857143,-1",
		"60,0.5,-0.5",
		"120,0.5,-0.5", };
}