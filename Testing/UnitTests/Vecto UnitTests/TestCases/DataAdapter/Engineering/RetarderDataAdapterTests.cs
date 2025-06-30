using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Engineering;

public class RetarderDataAdapterTests
{
	// TODO: MQ 20240510: mismatch of allowed retarder positions in declaration mode and engineering mode!
    [TestCase(RetarderType.AxlegearInputRetarder, PowertrainPosition.BatteryElectricE2, true),
	TestCase(RetarderType.AxlegearInputRetarder, PowertrainPosition.BatteryElectricE3, false),
	TestCase(RetarderType.AxlegearInputRetarder, PowertrainPosition.BatteryElectricE4, true),
	TestCase(RetarderType.AxlegearInputRetarder, PowertrainPosition.HybridP2, true),
	TestCase(RetarderType.AxlegearInputRetarder, PowertrainPosition.HybridP3, true),
	TestCase(RetarderType.AxlegearInputRetarder, PowertrainPosition.HybridPositionNotSet, true),

	TestCase(RetarderType.EngineRetarder, PowertrainPosition.BatteryElectricE2, false),
	TestCase(RetarderType.EngineRetarder, PowertrainPosition.BatteryElectricE3, false),
	TestCase(RetarderType.EngineRetarder, PowertrainPosition.BatteryElectricE4, false),
	TestCase(RetarderType.EngineRetarder, PowertrainPosition.HybridP2, false),
	TestCase(RetarderType.EngineRetarder, PowertrainPosition.HybridP3, false),
	TestCase(RetarderType.EngineRetarder, PowertrainPosition.HybridPositionNotSet, false),

	TestCase(RetarderType.LossesIncludedInTransmission, PowertrainPosition.BatteryElectricE2, false),
	TestCase(RetarderType.LossesIncludedInTransmission, PowertrainPosition.BatteryElectricE3, false),
	TestCase(RetarderType.LossesIncludedInTransmission, PowertrainPosition.BatteryElectricE4, false),
	TestCase(RetarderType.LossesIncludedInTransmission, PowertrainPosition.HybridP2, false),
	TestCase(RetarderType.LossesIncludedInTransmission, PowertrainPosition.HybridP3, false),
	TestCase(RetarderType.LossesIncludedInTransmission, PowertrainPosition.HybridPositionNotSet, false),

	TestCase(RetarderType.None, PowertrainPosition.BatteryElectricE2, false),
	TestCase(RetarderType.None, PowertrainPosition.BatteryElectricE3, false),
	TestCase(RetarderType.None, PowertrainPosition.BatteryElectricE4, false),
	TestCase(RetarderType.None, PowertrainPosition.HybridP2, false),
	TestCase(RetarderType.None, PowertrainPosition.HybridP3, false),
	TestCase(RetarderType.None, PowertrainPosition.HybridPositionNotSet, false),

	TestCase(RetarderType.TransmissionInputRetarder, PowertrainPosition.BatteryElectricE2, false),
	TestCase(RetarderType.TransmissionInputRetarder, PowertrainPosition.BatteryElectricE3, true),
	TestCase(RetarderType.TransmissionInputRetarder, PowertrainPosition.BatteryElectricE4, true),
	TestCase(RetarderType.TransmissionInputRetarder, PowertrainPosition.HybridP2, false),
	TestCase(RetarderType.TransmissionInputRetarder, PowertrainPosition.HybridP3, false),
	TestCase(RetarderType.TransmissionInputRetarder, PowertrainPosition.HybridPositionNotSet, false),

	TestCase(RetarderType.TransmissionOutputRetarder, PowertrainPosition.BatteryElectricE2, false),
	TestCase(RetarderType.TransmissionOutputRetarder, PowertrainPosition.BatteryElectricE3, true),
	TestCase(RetarderType.TransmissionOutputRetarder, PowertrainPosition.BatteryElectricE4, true),
	TestCase(RetarderType.TransmissionOutputRetarder, PowertrainPosition.HybridP2, false),
	TestCase(RetarderType.TransmissionOutputRetarder, PowertrainPosition.HybridP3, false),
	TestCase(RetarderType.TransmissionOutputRetarder, PowertrainPosition.HybridPositionNotSet, false),
	]
    public void CreateRetarderDataTest(RetarderType type, PowertrainPosition arch, bool throwsExeption)
    {
        var adapter = new EngineeringDataAdapter();
        var lossMap = InputDataHelper.InputDataAsTableData("Retarder Speed [rpm],Loss Torque [Nm]",
            new[] { "0, 10", "1000, 12", "2000, 18", "2300, 20.58" });

        var inputData = new Mock<IRetarderInputData>();
        inputData.Setup(r => r.Type).Returns(type);
        if (type.IsDedicatedComponent()) {
            inputData.Setup(r => r.LossMap).Returns(lossMap);
        }

        if (throwsExeption) {
            NUnit.Framework.Assert.Throws<VectoException>(() => adapter.CreateRetarderData(inputData.Object, arch));
        } else {
            var retarderData = adapter.CreateRetarderData(inputData.Object, arch);
            NUnit.Framework.Assert.NotNull(retarderData);
        }

    }
}