using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class FactorMethodGenericElectricMotorTests
{
    [TestCase()]
    public void TestGenericBusElectricMotorDataMeasured()
    {
        var em = GetMockElectricMotorInputData(CertificationMethod.Measured);
		var averageVoltage = 350.SI<Volt>();

        var genericElectricMotor = new GenericBusElectricMotorData();
        var electricMotorData = genericElectricMotor.CreateGenericElectricMotorData(em, null,
            averageVoltage);

        Assert.AreEqual(2, electricMotorData.EfficiencyData.VoltageLevels.Count);
		// Todo: More assertions...
    }

	[TestCase()]
	public void TestGenericBusElectricMotorDataStdVal()
	{
		var em = GetMockElectricMotorInputData(CertificationMethod.StandardValues);
		var averageVoltage = 350.SI<Volt>();

        var genericElectricMotor = new GenericBusElectricMotorData();
		var electricMotorData = genericElectricMotor.CreateGenericElectricMotorData(em, null,
			averageVoltage);

		Assert.AreEqual(2, electricMotorData.EfficiencyData.VoltageLevels.Count);
		// Todo: More assertions...
	}

	[TestCase()]
	public void TestGenericIEPCElectricMotorDataMeasured()
	{
		var iepcData = GetMockIEPCInputData(CertificationMethod.StandardValues);

		var genericIEPCData = new GenericBusIEPCData();
		var iepcMotorData = genericIEPCData.CreateIEPCElectricMotorData(iepcData);

		Assert.AreEqual(1, iepcMotorData.EfficiencyData.VoltageLevels.Count);
		// Todo: More assertions...
	}

    [TestCase()]
    public void TestGenericIEPCElectricMotorDataStdVal()
    {
        var iepcData = GetMockIEPCInputData(CertificationMethod.StandardValues);
        
        var genericIEPCData = new GenericBusIEPCData();
        var iepcMotorData = genericIEPCData.CreateIEPCElectricMotorData(iepcData);

        Assert.AreEqual(1, iepcMotorData.EfficiencyData.VoltageLevels.Count);
		// Todo: More assertions...
    }

	

	[TestCase(@"TestData/XML/XMLVIFBusReport/IHPC_HEV_completedBus_2.VIF_Report_1.xml")]
    public void TestGenericIHPCElectricMotorData(string ihpcFilePath)
    {
        var electricMachineEntry = GetMockIHPCEMInputData(CertificationMethod.Measured);
        var transmission = GetMockIHPCGbxInputData();
        var machineType = electricMachineEntry.ElectricMachine.ElectricMachineType;

        var genericBusIHPCData = new GenericBusIHPCData();
        var ihpcData = genericBusIHPCData.CreateGenericBusIHPCData(electricMachineEntry, machineType, transmission);

        Assert.AreEqual(2, ihpcData.EfficiencyData.VoltageLevels.Count);
		// Todo: More assertions...
    }

	
	[TestCase()]
	public void TestFullLoadCurveRatedPointSearch()
	{
		var fullLoadCurve = InputDataHelper.InputDataAsTableData(EMFldHdr, EMFldData);

		var emResult = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtEM(fullLoadCurve);
		Assert.IsNotNull(emResult);
		Assert.AreEqual(755.11, emResult.NRated.AsRPM, 1e-2, "Wrong speed");
		Assert.AreEqual(4027.8000, emResult.TRated.Value(), 1e-4, "Wrong torque");
		Assert.AreEqual(318498.02032, emResult.PRated.Value(), 1e-4, "Wrong power");

		var iepcResult = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtIEPC(fullLoadCurve, 1, 1, 0.95, 1);
		Assert.IsNotNull(iepcResult);
		Assert.AreEqual(755.11, iepcResult.NRated.AsRPM, 1e-2);
		Assert.AreEqual(4239.7894, iepcResult.TRated.Value(), 1e-4);
		Assert.AreEqual(335261.0740, iepcResult.PRated.Value(), 1e-2);
	}

	private ElectricMachineEntry<IElectricMotorDeclarationInputData> GetMockElectricMotorInputData(
		CertificationMethod certificationMethod)
	{
		var retVal = new ElectricMachineEntry<IElectricMotorDeclarationInputData>();
		var em = new Mock<IElectricMotorDeclarationInputData>();
		retVal.ElectricMachine = em.Object;
		retVal.Count = 1;
		retVal.Position = PowertrainPosition.HybridP2;

		em.Setup(e => e.Inertia).Returns(0.1.SI<KilogramSquareMeter>());
		em.Setup(e => e.CertificationMethod).Returns(certificationMethod);
		var vLow = new Mock<IElectricMotorVoltageLevel>();
		var vHi = new Mock<IElectricMotorVoltageLevel>();
		if (certificationMethod == CertificationMethod.StandardValues) {
			em.Setup(e => e.VoltageLevels).Returns(new List<IElectricMotorVoltageLevel>() { vLow.Object });
		} else {
			em.Setup(e => e.VoltageLevels).Returns(new List<IElectricMotorVoltageLevel>() { vLow.Object, vHi.Object });
			vLow.Setup(v => v.VoltageLevel).Returns(400.SI<Volt>());
		}

        vLow.Setup(v => v.ContinuousTorque).Returns(200.SI<NewtonMeter>());
		vLow.Setup(v => v.ContinuousTorqueSpeed).Returns(2000.RPMtoRad());
		vLow.Setup(v => v.OverloadTorque).Returns(400.SI<NewtonMeter>());
		vLow.Setup(v => v.OverloadTestSpeed).Returns(2000.RPMtoRad());
		vLow.Setup(v => v.OverloadTime).Returns(30.SI<Second>());
		// explicitly return a new instance on every call because the column names in GenericBusElectricMotorData are overwritten and this leads to an error, see also ToDo's in ElectricMotorInputData
		vLow.Setup(v => v.FullLoadCurve).Returns(() => new List<IElectricMotorLoadCurve>() {
			new ElectricMotorLoadCurve() {
				Gear = 0,
				LoadCurve = InputDataHelper.InputDataAsTableData(EMFldHdr, EMFldData)
			}
		});

		vHi.Setup(v => v.VoltageLevel).Returns(400.SI<Volt>());
		vHi.Setup(v => v.ContinuousTorque).Returns(200.SI<NewtonMeter>());
		vHi.Setup(v => v.ContinuousTorqueSpeed).Returns(2000.RPMtoRad());
		vHi.Setup(v => v.OverloadTorque).Returns(400.SI<NewtonMeter>());
		vHi.Setup(v => v.OverloadTestSpeed).Returns(2000.RPMtoRad());
		vHi.Setup(v => v.OverloadTime).Returns(30.SI<Second>());
		// explicitly return a new instance on every call because the column names in GenericBusElectricMotorData are overwritten and this leads to an error, see also ToDo's in ElectricMotorInputData
		vHi.Setup(v => v.FullLoadCurve).Returns(() => new List<IElectricMotorLoadCurve>() {
			new ElectricMotorLoadCurve() {
				Gear = 0,
				LoadCurve = InputDataHelper.InputDataAsTableData(EMFldHdr, EMFldData)
			}
		});

		em.Setup(e => e.DragCurve).Returns(InputDataHelper.InputDataAsTableData(EmDragHdr, EmDragData));
        return retVal;
	}

	private IIEPCDeclarationInputData GetMockIEPCInputData(CertificationMethod certificationMethod)
	{
		var retVal = new Mock<IIEPCDeclarationInputData>();

		retVal.Setup(r => r.CertificationMethod).Returns(CertificationMethod.StandardValues);
		retVal.Setup(r => r.DifferentialIncluded).Returns(false);
		retVal.Setup(r => r.DesignTypeWheelMotor).Returns(false);
		retVal.Setup(r => r.Inertia).Returns(0.1.SI<KilogramSquareMeter>());
		retVal.Setup(r => r.Gears).Returns(new[] { 3.0, 1.0 }.Select((i, idx) => {
			var g = new Mock<IGearEntry>();
			g.Setup(x => x.Ratio).Returns(i);
			g.Setup(x => x.GearNumber).Returns(idx + 1);
			return g.Object;
		}).ToList());

		var vLow = new Mock<IElectricMotorVoltageLevel>();
		var vHi = new Mock<IElectricMotorVoltageLevel>();
		if (certificationMethod == CertificationMethod.StandardValues) {
			retVal.Setup(e => e.VoltageLevels).Returns(new List<IElectricMotorVoltageLevel>() { vLow.Object });
		} else {
			retVal.Setup(e => e.VoltageLevels).Returns(new List<IElectricMotorVoltageLevel>() { vLow.Object, vHi.Object });
			vLow.Setup(v => v.VoltageLevel).Returns(400.SI<Volt>());
		}

		vLow.Setup(v => v.ContinuousTorque).Returns(200.SI<NewtonMeter>());
		vLow.Setup(v => v.ContinuousTorqueSpeed).Returns(2000.RPMtoRad());
		vLow.Setup(v => v.OverloadTorque).Returns(400.SI<NewtonMeter>());
		vLow.Setup(v => v.OverloadTestSpeed).Returns(2000.RPMtoRad());
		vLow.Setup(v => v.OverloadTime).Returns(30.SI<Second>());
		// explicitly return a new instance on every call because the column names in GenericBusElectricMotorData are overwritten and this leads to an error, see also ToDo's in ElectricMotorInputData
		vLow.Setup(v => v.FullLoadCurve).Returns(() => new List<IElectricMotorLoadCurve>() {
			new ElectricMotorLoadCurve() {
				Gear = 0,
				LoadCurve = InputDataHelper.InputDataAsTableData(EMFldHdr, EMFldData)
			}
		});

		vHi.Setup(v => v.VoltageLevel).Returns(400.SI<Volt>());
		vHi.Setup(v => v.ContinuousTorque).Returns(200.SI<NewtonMeter>());
		vHi.Setup(v => v.ContinuousTorqueSpeed).Returns(2000.RPMtoRad());
		vHi.Setup(v => v.OverloadTorque).Returns(400.SI<NewtonMeter>());
		vHi.Setup(v => v.OverloadTestSpeed).Returns(2000.RPMtoRad());
		vHi.Setup(v => v.OverloadTime).Returns(30.SI<Second>());
		// explicitly return a new instance on every call because the column names in GenericBusElectricMotorData are overwritten and this leads to an error, see also ToDo's in ElectricMotorInputData
		vHi.Setup(v => v.FullLoadCurve).Returns(() => new List<IElectricMotorLoadCurve>() {
			new ElectricMotorLoadCurve() {
				Gear = 0,
				LoadCurve = InputDataHelper.InputDataAsTableData(EMFldHdr, EMFldData)
			}
		});

		var drag = new Mock<IDragCurve>();
		drag.Setup(d => d.DragCurve).Returns(() => InputDataHelper.InputDataAsTableData(EmDragHdr, EmDragData));
		drag.Setup(d => d.Gear).Returns(1);
        retVal.Setup(r => r.DragCurves).Returns(new List<IDragCurve>() { drag.Object });

        return retVal.Object;
	}

	private IGearboxDeclarationInputData GetMockIHPCGbxInputData()
	{
		var gbx = new Mock<IGearboxDeclarationInputData>();
		gbx.Setup(g => g.Gears).Returns(new[] { 14.93, 11.64 }.Select((i, idx) => {
			var g = new Mock<ITransmissionInputData>();
			g.Setup(x => x.Ratio).Returns(i);
			g.Setup(x => x.Gear).Returns(idx + 1);
			return g.Object;
		}).ToList());
		return gbx.Object;
	}

	private ElectricMachineEntry<IElectricMotorDeclarationInputData> GetMockIHPCEMInputData(CertificationMethod certificationMethod)
	{
		var em = GetMockElectricMotorInputData(certificationMethod);
		var mock = Mock.Get(em.ElectricMachine);
		mock.Setup(m => m.IHPCType).Returns("IHPC Type 1");
		return em;
	}


    private const string EMFldHdr = "outShaftSpeed, maxTorque, minTorque";

	private static readonly string[] EMFldData = new[] {
		"0.00, 4027.80, -4027.80",
		"14.96, 4010.00, -4010.00",
		"151.09, 3980.00, -3980.00",
		"302.19, 4010.00, -4010.00",
		"452.92, 3950.00, -3950.00",
		"604.01, 3900.00, -3900.00",
		"755.11, 3950.00, -3950.00",
		"906.20, 3356.50, -3356.50",
		"1057.30, 2876.98, -2876.98",
		"1208.03, 2517.38, -2517.38",
		"1359.12, 2237.68, -2237.68",
		"1510.22, 2013.90, -2013.90",
		"1661.31, 1830.82, -1830.82",
		"1812.41, 1678.25, -1678.25",
		"1963.14, 1549.15, -1549.15",
		"2114.23, 1438.52, -1438.52",
		"2265.33, 1342.60, -1342.60",
		"2416.42, 1258.71, -1258.71",
		"2567.52, 1184.66, -1184.66",
		"2718.25, 1118.82, -1118.82",
		"2869.34, 1059.96, -1059.96",
		"3020.44, 1006.95, -1006.95",
    };

	private const string EmDragHdr = "outShaftSpeed, dragTorque";

	private static readonly string[] EmDragData = new[] {
		"0, -10",
		"4000, -30",
	};
}