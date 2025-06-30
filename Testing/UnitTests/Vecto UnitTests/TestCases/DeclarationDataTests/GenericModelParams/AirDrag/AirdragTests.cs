using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.AirDrag;

public class AirdragTests
{
	[
		TestCase("RigidSolo", 0.013526, 0.017746, -0.000666),
		TestCase("RigidTrailer", 0.017125, 0.072275, -0.004148),
		TestCase("TractorSemitrailer", 0.030042, 0.040817, -0.00213),
		TestCase("CoachBus", -0.000794, 0.02109, -0.00109),
		TestCase("MediumLorriesRigid", -0.0015, 0.0086, -0.00029),
		TestCase("MediumLorriesVan", 0.0032, 0.00532, -0.00028)]

	public void AirDrag_WithStringKey(string key, double a1, double a2, double a3)
	{
		var value = DeclarationData.AirDrag.Lookup(key);
		AssertHelper.AreRelativeEqual(a1, value.A1);
		AssertHelper.AreRelativeEqual(a2, value.A2);
		AssertHelper.AreRelativeEqual(a3, value.A3);
	}

    [TestCase("TractorSemitrailer", 6.46, 0, 4.0, 7.71712257),
        TestCase("TractorSemitrailer", 6.46, 60, 4.0, 7.71712257),
        TestCase("TractorSemitrailer", 6.46, 75, 3.75, 7.35129203),
        TestCase("TractorSemitrailer", 6.46, 100, 4.0, 7.03986404),
        TestCase("TractorSemitrailer", 6.46, 62.1234, 4.0, 7.65751048),
        TestCase("TractorSemitrailer", 6.46, 73.5432, 3.75, 7.37814098),
        TestCase("TractorSemitrailer", 6.46, 92.8765, 4.0, 7.11234364),
        TestCase("TractorSemitrailer", 6.46, 100.449, 4.0, 7.03571556),
        TestCase("TractorSemitrailer", 6.46, 103, 3.6, 6.99454230),
        TestCase("TractorSemitrailer", 6.46, 105, 3.9, 6.99177143),
        TestCase("TractorSemitrailer", 6.46, 115, 4.0, 6.92267778),
        TestCase("TractorSemitrailer", 6.46, 130, 4.0, 6.83867361),]
    public void CrossWindCorrectionTest(string parameterSet, double crossSectionArea, double kmph, double height,
            double expected)
    {
        var crossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(crossSectionArea.SI<SquareMeter>(),
            0.SI<SquareMeter>(),
            new AirdragDataAdapter().GetDeclarationAirResistanceCurve(parameterSet,
                crossSectionArea.SI<SquareMeter>(),
                height.SI<Meter>()),
            CrossWindCorrectionMode.DeclarationModeCorrection);

        var tmp = crossWindCorrectionCurve.EffectiveAirDragArea(kmph.KMPHtoMeterPerSecond());
        AssertHelper.AreRelativeEqual(expected, tmp.Value(), toleranceFactor: 1e-3);
    }

    [TestCase("TractorSemitrailer", 5.8, 4.0)]
    public void CrossWindGetDeclarationAirResistance(string parameterSet, double cdxa0, double height)
    {
        var curve =
            new AirdragDataAdapter().GetDeclarationAirResistanceCurve(parameterSet, cdxa0.SI<SquareMeter>(),
                height.SI<Meter>());

        AssertHelper.AreRelativeEqual(60.KMPHtoMeterPerSecond(), curve[1].Velocity);
        AssertHelper.AreRelativeEqual(7.0418009.SI<SquareMeter>(), curve[1].EffectiveCrossSectionArea);

        AssertHelper.AreRelativeEqual(65.KMPHtoMeterPerSecond(), curve[2].Velocity);
        AssertHelper.AreRelativeEqual(6.90971991.SI<SquareMeter>(), curve[2].EffectiveCrossSectionArea);

        AssertHelper.AreRelativeEqual(85.KMPHtoMeterPerSecond(), curve[6].Velocity);
        AssertHelper.AreRelativeEqual(6.54224222.SI<SquareMeter>(), curve[6].EffectiveCrossSectionArea);

        AssertHelper.AreRelativeEqual(100.KMPHtoMeterPerSecond(), curve[9].Velocity);
        AssertHelper.AreRelativeEqual(6.37434824.SI<SquareMeter>(), curve[9].EffectiveCrossSectionArea);

        AssertHelper.AreRelativeEqual(105.KMPHtoMeterPerSecond(), curve[10].Velocity);
        AssertHelper.AreRelativeEqual(6.33112792.SI<SquareMeter>(), curve[10].EffectiveCrossSectionArea);


        Assert.AreEqual(16, curve.Count);
    }

    [
        TestCase("TractorSemitrailer", 6.46, -0.1, 3.0),
        TestCase("TractorSemitrailer", 6.46, 200.1, 3.0),
    ]
    public void CrossWindCorrectionExceptionTest(string parameterSet, double crossSectionArea, double kmph,
        double height)
    {
        var crossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(crossSectionArea.SI<SquareMeter>(),
			0.SI<SquareMeter>(),
            new AirdragDataAdapter().GetDeclarationAirResistanceCurve(parameterSet,
                crossSectionArea.SI<SquareMeter>(),
                height.SI<Meter>()),
            CrossWindCorrectionMode.DeclarationModeCorrection);

        AssertHelper.Exception<VectoException>(() =>
            crossWindCorrectionCurve.EffectiveAirDragArea(kmph.KMPHtoMeterPerSecond()));
    }

    [TestCase]
    public void CrossWindAreaCdxANotSet_DeclarationMode()
    {
        var airDrag = new AirdragData() {
            CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection,
            CrossWindCorrectionCurve =
                new CrosswindCorrectionCdxALookup(null, null, null, CrossWindCorrectionMode.DeclarationModeCorrection)
        };

        Assert.IsTrue(airDrag.IsValid(),
            "In Speed Dependent (Declaration Mode) Crosswind Correction the CdxA Value can be empty.");
    }

    [TestCase]
    public void CrossWindAreaCdxANotSet_Other()
    {
        foreach (var correctionMode in EnumHelper.GetValues<CrossWindCorrectionMode>()) {
            if (correctionMode == CrossWindCorrectionMode.DeclarationModeCorrection) {
                continue;
            }

            var airDrag = new AirdragData {
                CrossWindCorrectionMode = correctionMode,
                CrossWindCorrectionCurve =
                    new CrosswindCorrectionCdxALookup(null, null, null, correctionMode)
            };

            Assert.IsFalse(airDrag.IsValid(),
                "Only in Speed Dependent (Declaration Mode) Crosswind Correction the CdxA Value can be empty.");
        }
    }
}