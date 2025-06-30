using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Driver;

public class ADASTests
{
    [TestCase(false, false, false, PredictiveCruiseControlType.None, "0"),
        TestCase(true, false, false, PredictiveCruiseControlType.None, "1"),
        TestCase(true, false, false, PredictiveCruiseControlType.Option_1_2_3, "7/2"),
        TestCase(true, true, false, PredictiveCruiseControlType.Option_1_2, "10/1")
	]
    public void TestADASCombinationLookup(bool engineStopStart, bool ecoRollWOEngineStop, bool ecoRollWEngineStop, PredictiveCruiseControlType pcc,
    string expectedADASGroup)
    {
        var adas = DeclarationData.ADASCombinations.Lookup(engineStopStart, EcorollTypeHelper.Get(ecoRollWOEngineStop, ecoRollWEngineStop), pcc);
        Assert.AreEqual(adas.ID, expectedADASGroup);
    }

    [TestCase(true, true, true, PredictiveCruiseControlType.Option_1_2),
    TestCase(true, true, true, PredictiveCruiseControlType.None)]
    public void TestInvalidADASCombinationLookup(
        bool engineStopStart, bool ecoRollWOEngineStop, bool ecoRollWEngineStop, PredictiveCruiseControlType pcc)
    {
        AssertHelper.Exception<VectoException>(() => {
            DeclarationData.ADASCombinations.Lookup(engineStopStart, EcorollTypeHelper.Get(ecoRollWOEngineStop, ecoRollWEngineStop), pcc);
        });
    }

    [TestCase(false, false, true, PredictiveCruiseControlType.None),
    TestCase(true, false, true, PredictiveCruiseControlType.None),
    TestCase(false, false, true, PredictiveCruiseControlType.Option_1_2),
    TestCase(false, false, true, PredictiveCruiseControlType.Option_1_2_3),
    TestCase(true, false, true, PredictiveCruiseControlType.Option_1_2),
    TestCase(true, false, true, PredictiveCruiseControlType.Option_1_2_3),
        ]
    public void TestInvalidATADASCombinationLookup(bool engineStopStart, bool ecoRollWOEngineStop, bool ecoRollWEngineStop, PredictiveCruiseControlType pcc)
    {
        var adas = new Mock<IAdvancedDriverAssistantSystemDeclarationInputData>();
		adas.SetupGet(x => x.EngineStopStart).Returns(engineStopStart);
		adas.SetupGet(x => x.EcoRoll).Returns(EcorollTypeHelper.Get(ecoRollWOEngineStop, ecoRollWEngineStop));
		adas.SetupGet(x => x.PredictiveCruiseControl).Returns(pcc);

        AssertHelper.Exception<VectoException>(() => {
            DeclarationData.ADASCombinations.Lookup(adas.Object, GearboxType.ATSerial);
        });
    }

    
}