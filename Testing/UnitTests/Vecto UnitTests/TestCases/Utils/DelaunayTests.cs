using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto_UnitTests.TestCases.Utils;

public class DelaunayTests
{
    [TestCase]
    public void Test_Simple_DelaunayMap()
    {
        var map = new DelaunayMap("TEST");
        map.AddPoint(0, 0, 0);
        map.AddPoint(1, 0, 0);
        map.AddPoint(0, 1, 0);

        map.Triangulate();

        var result = map.Interpolate(0.25, 0.25);

        AssertHelper.AreRelativeEqual(0, result);
    }

    [TestCase]
    public void Test_DelaunayMapTriangle()
    {
        var map = new DelaunayMap("TEST");
        map.AddPoint(0, 0, 0);
        map.AddPoint(1, 0, 1);
        map.AddPoint(0, 1, 2);

        map.Triangulate();

        // fixed points
        AssertHelper.AreRelativeEqual(0, map.Interpolate(0, 0));
        AssertHelper.AreRelativeEqual(1, map.Interpolate(1, 0));
        AssertHelper.AreRelativeEqual(2, map.Interpolate(0, 1));

        // interpolations
        AssertHelper.AreRelativeEqual(0.5, map.Interpolate(0.5, 0));
        AssertHelper.AreRelativeEqual(1, map.Interpolate(0, 0.5));
        AssertHelper.AreRelativeEqual(1.5, map.Interpolate(0.5, 0.5));

        AssertHelper.AreRelativeEqual(0.25, map.Interpolate(0.25, 0));
        AssertHelper.AreRelativeEqual(0.5, map.Interpolate(0, 0.25));
        AssertHelper.AreRelativeEqual(0.75, map.Interpolate(0.25, 0.25));

        AssertHelper.AreRelativeEqual(0.75, map.Interpolate(0.75, 0));
        AssertHelper.AreRelativeEqual(1.5, map.Interpolate(0, 0.75));

        // extrapolation (should fail)
        Assert.IsNaN(map.Interpolate(1, 1));
        Assert.IsNaN(map.Interpolate(-1, -1));
        Assert.IsNaN(map.Interpolate(1, -1));
        Assert.IsNaN(map.Interpolate(-1, 1));
    }

    [
		// fixed points
		TestCase(0, 0, 0),
	    TestCase(1, 0, 1),
		TestCase(0, 1, 2),
		TestCase(1, 1, 3),
        // interpolations
		TestCase(0.5, 0, 0.5),
		TestCase(0, 0.5, 1),
		TestCase(1, 0.5, 2),
		TestCase(0.5, 1, 2.5),
		TestCase(0.5, 0.5, 1.5),
		TestCase(0.25, 0.25, 0.75),
		TestCase(0.75, 0.75, 2.25),
		TestCase(0.25, 0.75, 1.75),
		TestCase(0.75, 0.25, 1.25),
        // extrapolation
		TestCase(1.5, 0.5, double.NaN),
		TestCase(1.5, 1.5, double.NaN),
		TestCase(0.5, 1.5, double.NaN),
		TestCase(-0.5, 1.5, double.NaN),
		TestCase(-0.5, 0.5, double.NaN),
		TestCase(-1.5, -1.5, double.NaN),
		TestCase(0.5, -0.5, double.NaN),
		TestCase(-1.5, -0.5, double.NaN),
    ]
    public void Test_DelaunayMapPlane(double x, double y, double expected)
    {
        var map = new DelaunayMap("TEST");
        map.AddPoint(0, 0, 0);
        map.AddPoint(1, 0, 1);
        map.AddPoint(0, 1, 2);
        map.AddPoint(1, 1, 3);

        map.Triangulate();

		Assert.AreEqual(expected, map.Interpolate(x, y));
	}

    [TestCase]
    public void Test_Delaunay_LessThan3Points()
    {
        AssertHelper.Exception<ArgumentException>(() => new DelaunayMap("TEST").Triangulate(),
            "TEST: Triangulation needs at least 3 Points. Got 0 Points.");

        AssertHelper.Exception<ArgumentException>(() => {
            var map1 = new DelaunayMap("TEST");
            map1.AddPoint(1, 0, 0);
            map1.Triangulate();
        }, "TEST: Triangulation needs at least 3 Points. Got 1 Points.");

        AssertHelper.Exception<ArgumentException>(() => {
            var map2 = new DelaunayMap("TEST");
            map2.AddPoint(1, 0, 0);
            map2.AddPoint(0, 1, 0);
            map2.Triangulate();
        }, "TEST: Triangulation needs at least 3 Points. Got 2 Points.");

        var map = new DelaunayMap("TEST");
        map.AddPoint(1, 0, 0);
        map.AddPoint(0, 1, 0);
        map.AddPoint(0, 0, 1);
        map.Triangulate();
    }

    [TestCase]
    public void Test_Delaunay_DuplicatePoints()
    {
        var map = new DelaunayMap("TEST");
        map.AddPoint(0, 0, 0);
        map.AddPoint(1, 0, 1);
        map.AddPoint(1, 1, 3);
        map.AddPoint(0, 1, 2);
        map.AddPoint(1, 1, 5);

        AssertHelper.Exception<VectoException>(() => { map.Triangulate(); },
            "TEST: Input Data for Delaunay map contains duplicates! \n1 / 1");
    }

    [TestCase]
    public void Test_Delaunay_NormalOperation()
    {
        foreach (var factors in new[] {
                Tuple.Create(1.0, 1.0),
                Tuple.Create(1.0, 0.04),
                Tuple.Create(1.0, 0.1),
                Tuple.Create(1.0, 0.01),
                Tuple.Create(1.0, 0.0001)
            }) {
            var xfactor = factors.Item1;
            var yfactor = factors.Item2;

            var map = new DelaunayMap("TEST");
            var points = Data
                .Skip(1)
                .Select(s => {
                    var p = s.Split(',').ToDouble().ToList();
                    return new Point(p[0] * xfactor, p[1] * yfactor, p[2]);
                })
                .ToList();

            points.ForEach(p => map.AddPoint(p.X, p.Y, p.Z));
            map.Triangulate();

            // test fixed points
            foreach (var p in points) {
                AssertHelper.AreRelativeEqual(p.Z, map.Interpolate(p.X, p.Y));
            }
            map.DrawGraph();

            // test one arbitrary point in the middle
            AssertHelper.AreRelativeEqual(37681, map.Interpolate(1500 * xfactor, 1300 * yfactor),
                $"{xfactor}, {yfactor}");
        }
    }

	private const string Header = "engine speed [1/min],torque [Nm],fuel consumption [g/h]";

	private readonly string[] Data = new[] {
		"560  , -149 , 0",
		"560  , 0    , 1256",
		"560  , 200  , 3197",
		"560  , 400  , 5295",
		"560  , 600  , 7615",
		"560  , 800  , 9375",
		"560  , 1000 , 11239",
		"560  , 1180 , 12869",
		"600  , -148 , 0",
		"600  , 0    , 1459",
		"600  , 200  , 3358",
		"600  , 400  , 5498",
		"600  , 600  , 8101",
		"600  , 800  , 10014",
		"600  , 1000 , 12071",
		"600  , 1200 , 14201",
		"600  , 1282 , 15304",
		"800  , -149 , 0",
		"800  , 0    , 1879",
		"800  , 200  , 4286",
		"800  , 400  , 7021",
		"800  , 600  , 10059",
		"800  , 800  , 13086",
		"800  , 1000 , 16015",
		"800  , 1200 , 19239",
		"800  , 1400 , 22426",
		"800  , 1600 , 25483",
		"800  , 1791 , 28905",
		"1000 , -160 , 0",
		"1000 , 0    , 2865",
		"1000 , 200  , 5963",
		"1000 , 400  , 9198",
		"1000 , 600  , 12354",
		"1000 , 800  , 15965",
		"1000 , 1000 , 19864",
		"1000 , 1200 , 23530",
		"1000 , 1400 , 27202",
		"1000 , 1600 , 31165",
		"1000 , 1800 , 35103",
		"1000 , 2000 , 39360",
		"1000 , 2200 , 44120",
		"1000 , 2300 , 46836",
		"1200 , -179 , 0",
		"1200 , 0    , 3307",
		"1200 , 200  , 6897",
		"1200 , 400  , 10651",
		"1200 , 600  , 14645",
		"1200 , 800  , 19115",
		"1200 , 1000 , 23677",
		"1200 , 1200 , 28180",
		"1200 , 1400 , 32431",
		"1200 , 1600 , 36698",
		"1200 , 1800 , 41691",
		"1200 , 2000 , 46915",
		"1200 , 2200 , 51783",
		"1200 , 2300 , 54932",
		"1400 , -203 , 0",
		"1400 , 0    , 4306",
		"1400 , 200  , 8143",
		"1400 , 400  , 12723",
		"1400 , 600  , 17523",
		"1400 , 800  , 22288",
		"1400 , 1000 , 27093",
		"1400 , 1200 , 32536",
		"1400 , 1400 , 37746",
		"1400 , 1600 , 43194",
		"1400 , 1800 , 49453",
		"1400 , 2000 , 55830",
		"1400 , 2200 , 61072",
		"1400 , 2300 , 64377",
		"1600 , -235 , 0",
		"1600 , 0    , 5209",
		"1600 , 200  , 9669",
		"1600 , 400  , 14838",
		"1600 , 600  , 20127",
		"1600 , 800  , 25894",
		"1600 , 1000 , 31631",
		"1600 , 1200 , 37248",
		"1600 , 1400 , 42826",
		"1600 , 1600 , 49752",
		"1600 , 1800 , 57020",
		"1600 , 2000 , 63914",
		"1600 , 2079 , 66520",
		"1800 , -264 , 0",
		"1800 , 0    , 6409",
		"1800 , 200  , 11777",
		"1800 , 400  , 17320",
		"1800 , 600  , 23394",
		"1800 , 800  , 30501",
		"1800 , 1000 , 36378",
		"1800 , 1200 , 43079",
		"1800 , 1400 , 49796",
		"1800 , 1600 , 57436",
		"1800 , 1800 , 65157",
		"1800 , 1857 , 67574",
		"2000 , -301 , 0",
		"2000 , 0    , 9127",
		"2000 , 200  , 14822",
		"2000 , 400  , 20655",
		"2000 , 600  , 27076",
		"2000 , 800  , 34188",
		"2000 , 1000 , 42837",
		"2000 , 1200 , 51018",
		"2000 , 1352 , 56618",
		"2100 , -320 , 0",
		"2100 , 0    , 10470",
		"2100 , 200  , 16332",
		"2100 , 400  , 22396",
		"2100 , 600  , 28914",
		"2100 , 800  , 35717",
		"2100 , 1000 , 45643",
		"2100 , 1100 , 50653",
		"2300 , -320 , 0",
		"2300 , 0    , 10470",
		"2300 , 200  , 16332",
		"2300 , 400  , 22396",
		"2300 , 600  , 28914",
		"2300 , 800  , 35717",
		"2300 , 1000 , 45643",
		"2300 , 1100 , 50653",
	};
	//"560,-149,0\n560,0,1256\n560,200,3197\n560,400,5295\n560,600,7615\n560,800,9375\n560,1000,11239\n560,1180,12869\n600,-148,0\n600,0,1459\n600,200,3358\n600,400,5498\n600,600,8101\n600,800,10014\n600,1000,12071\n600,1200,14201\n600,1282,15304\n800,-149,0\n800,0,1879\n800,200,4286\n800,400,7021\n800,600,10059\n800,800,13086\n800,1000,16015\n800,1200,19239\n800,1400,22426\n800,1600,25483\n800,1791,28905\n1000,-160,0\n1000,0,2865\n1000,200,5963\n1000,400,9198\n1000,600,12354\n1000,800,15965\n1000,1000,19864\n1000,1200,23530\n1000,1400,27202\n1000,1600,31165\n1000,1800,35103\n1000,2000,39360\n1000,2200,44120\n1000,2300,46836\n1200,-179,0\n1200,0,3307\n1200,200,6897\n1200,400,10651\n1200,600,14645\n1200,800,19115\n1200,1000,23677\n1200,1200,28180\n1200,1400,32431\n1200,1600,36698\n1200,1800,41691\n1200,2000,46915\n1200,2200,51783\n1200,2300,54932\n1400,-203,0\n1400,0,4306\n1400,200,8143\n1400,400,12723\n1400,600,17523\n1400,800,22288\n1400,1000,27093\n1400,1200,32536\n1400,1400,37746\n1400,1600,43194\n1400,1800,49453\n1400,2000,55830\n1400,2200,61072\n1400,2300,64377\n1600,-235,0\n1600,0,5209\n1600,200,9669\n1600,400,14838\n1600,600,20127\n1600,800,25894\n1600,1000,31631\n1600,1200,37248\n1600,1400,42826\n1600,1600,49752\n1600,1800,57020\n1600,2000,63914\n1600,2079,66520\n1800,-264,0\n1800,0,6409\n1800,200,11777\n1800,400,17320\n1800,600,23394\n1800,800,30501\n1800,1000,36378\n1800,1200,43079\n1800,1400,49796\n1800,1600,57436\n1800,1800,65157\n1800,1857,67574\n2000,-301,0\n2000,0,9127\n2000,200,14822\n2000,400,20655\n2000,600,27076\n2000,800,34188\n2000,1000,42837\n2000,1200,51018\n2000,1352,56618\n2100,-320,0\n2100,0,10470\n2100,200,16332\n2100,400,22396\n2100,600,28914\n2100,800,35717\n2100,1000,45643\n2100,1100,50653\n2300,-320,0\n2300,0,10470\n2300,200,16332\n2300,400,22396\n2300,600,28914\n2300,800,35717\n2300,1000,45643\n2300,1100,50653";
}