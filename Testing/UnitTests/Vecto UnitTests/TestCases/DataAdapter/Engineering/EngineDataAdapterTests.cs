using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Engineering;

public class EngineDataAdapterTests
{
	[TestCase]
	public void Test_EngineData()
	{
		var inputData = GetMockEngineInputData();
		var dao = new EngineeringDataAdapter();
		var engineData = dao.CreateEngineData(inputData, inputData.EngineModes.First());
		var motorway = engineData.Fuels.First().WHTCMotorway;
		Assert.AreEqual(motorway, 1);

		var rural = engineData.Fuels.First().WHTCRural;
		Assert.AreEqual(1, rural);

		var urban = engineData.Fuels.First().WHTCUrban;
		Assert.AreEqual(1, urban);

		var fcCorr = engineData.Fuels.First().FuelConsumptionCorrectionFactor;
        Assert.AreEqual(1, fcCorr);

		var displace = engineData.Displacement;
		Assert.AreEqual(0.01273, displace.Value());
		Assert.IsTrue(displace.HasEqualUnit(new SI(Unit.SI.Cubic.Meter)));

		var inert = engineData.Inertia;
		Assert.AreEqual(3.8, inert.Value(), 0.00001);
		Assert.IsTrue(inert.HasEqualUnit(new SI(Unit.SI.Kilo.Gramm.Square.Meter)));

		var idle = engineData.IdleSpeed;
		Assert.AreEqual(58.6430628670095, idle.Value(), 0.000001);
		Assert.IsTrue(idle.HasEqualUnit(0.SI<PerSecond>()));
	}

	private IEngineEngineeringInputData GetMockEngineInputData()
	{
		var ice = new Mock<IEngineEngineeringInputData>();
		var mode = new Mock<IEngineModeEngineeringInputData>();
		var fuel = new Mock<IEngineFuelEngineeringInputData>();

		ice.Setup(e => e.Displacement).Returns(12730.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>());
		ice.Setup(e => e.Inertia).Returns(3.8.SI<KilogramSquareMeter>());
		ice.Setup(e => e.EngineModes).Returns(new List<IEngineModeEngineeringInputData>() { mode.Object });

		var decl = ice.As<IEngineDeclarationInputData>();
		decl.Setup(e => e.EngineModes).Returns(new List<IEngineModeDeclarationInputData>() { });

		mode.Setup(m => m.IdleSpeed).Returns(560.RPMtoRad());
		mode.Setup(m => m.Fuels).Returns(new List<IEngineFuelEngineeringInputData>() { fuel.Object });
		mode.Setup(m => m.FullLoadCurve).Returns(InputDataHelper.InputDataAsTableData(IceFldHdr, IceFldData));

		fuel.Setup(f => f.WHTCEngineering).Returns(1.0);
		fuel.Setup(f => f.FuelType).Returns(FuelType.DieselCI);
		fuel.Setup(f => f.FuelConsumptionMap).Returns(InputDataHelper.InputDataAsTableData(IceMapHdr, IceMapData));
		fuel.Setup(f => f.WHTCRural).Returns(1);
		fuel.Setup(f => f.WHTCUrban).Returns(1);
		return ice.Object;
	}

    const string IceFldHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

    private static readonly string[] IceFldData = new[] {
        "560,1180,-149,0.6",
        "600,1282,-148,0.6",
        "799.9999999,1791,-149,0.6",
        "1000,2300,-160,0.6",
        "1200,2300,-179,0.6",
        "1400,2300,-203,0.6",
        "1599.999999,2079,-235,0.49",
        "1800,1857,-264,0.25",
        "2000.000001,1352,-301,0.25",
        "2100,1100,-320,0.25",
    };

    const string IceMapHdr = "engine speed [1/min],torque [Nm],fuel consumption [g/h]";

    private static readonly string[] IceMapData = new[] {
        "560,-149,0",
        "560,0,1256",
        "560,200,3197",
        "560,400,5295",
        "560,600,7615",
        "560,800,9375",
        "560,1000,11239",
        "560,1180,12869",
        "600,-148,0",
        "600,0,1459",
        "600,200,3358",
        "600,400,5498",
        "600,600,8101",
        "600,800,10014",
        "600,1000,12071",
        "600,1200,14201",
        "600,1282,15304",
        "800,-149,0",
        "800,0,1879",
        "800,200,4286",
        "800,400,7021",
        "800,600,10059",
        "800,800,13086",
        "800,1000,16015",
        "800,1200,19239",
        "800,1400,22426",
        "800,1600,25483",
        "800,1791,28905",
        "1000,-160,0",
        "1000,0,2865",
        "1000,200,5963",
        "1000,400,9198",
        "1000,600,12354",
        "1000,800,15965",
        "1000,1000,19864",
        "1000,1200,23530",
        "1000,1400,27202",
        "1000,1600,31165",
        "1000,1800,35103",
        "1000,2000,39360",
        "1000,2200,44120",
        "1000,2300,46836",
        "1200,-179,0",
        "1200,0,3307",
        "1200,200,6897",
        "1200,400,10651",
        "1200,600,14645",
        "1200,800,19115",
        "1200,1000,23677",
        "1200,1200,28180",
        "1200,1400,32431",
        "1200,1600,36698",
        "1200,1800,41691",
        "1200,2000,46915",
        "1200,2200,51783",
        "1200,2300,54932",
        "1400,-203,0",
        "1400,0,4306",
        "1400,200,8143",
        "1400,400,12723",
        "1400,600,17523",
        "1400,800,22288",
        "1400,1000,27093",
        "1400,1200,32536",
        "1400,1400,37746",
        "1400,1600,43194",
        "1400,1800,49453",
        "1400,2000,55830",
        "1400,2200,61072",
        "1400,2300,64377",
        "1600,-235,0",
        "1600,0,5209",
        "1600,200,9669",
        "1600,400,14838",
        "1600,600,20127",
        "1600,800,25894",
        "1600,1000,31631",
        "1600,1200,37248",
        "1600,1400,42826",
        "1600,1600,49752",
        "1600,1800,57020",
        "1600,2000,63914",
        "1600,2079,66520",
        "1800,-264,0",
        "1800,0,6409",
        "1800,200,11777",
        "1800,400,17320",
        "1800,600,23394",
        "1800,800,30501",
        "1800,1000,36378",
        "1800,1200,43079",
        "1800,1400,49796",
        "1800,1600,57436",
        "1800,1800,65157",
        "1800,1857,67574",
        "2000,-301,0",
        "2000,0,9127",
        "2000,200,14822",
        "2000,400,20655",
        "2000,600,27076",
        "2000,800,34188",
        "2000,1000,42837",
        "2000,1200,51018",
        "2000,1352,56618",
        "2100,-320,0",
        "2100,0,10470",
        "2100,200,16332",
        "2100,400,22396",
        "2100,600,28914",
        "2100,800,35717",
        "2100,1000,45643",
        "2100,1100,50653",
        "500,-500,0",
        "3000,1500,0",
        "3000,-500,0",
    };

}