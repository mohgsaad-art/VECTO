using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ScottPlot;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Utils;
using Edge = ScottPlot.Renderable.Edge;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData;

[TestFixture]
public class FuelCellDataTest
{

	public const string BASEOUTPUTPATH = "FuelCellTestOutput/";
	[Test]
	public void FuelCellMassFlowMap_MinMaxPowerTest() {
		var massFlowMap = GetFuelCellMassFlowMap(1);


		Assert.That(massFlowMap.MinPowerMap.Value(), Is.EqualTo(30*1e3));
		Assert.That(massFlowMap.MaxPowerMap.Value(), Is.EqualTo(300*1e3));
		Assert.That(massFlowMap.MinEffPower.Value(), Is.EqualTo(60*1e3));
	}
	//[TestCase(30.0, 2002.0)] <- without timeslicing
	[TestCase(30.0, 1501.5)]
	[TestCase(60.0, 3003.0)]
	[TestCase(255.0, 17017.0)]
	[TestCase(300.0, 21450.0)]
	//interpolate
	[TestCase(70, 3543.778)]
	public void FuelCellMassFlowMap_LookupTest(double power_kW, double exp_H2_g_p_h) {
		var massFlowMap = GetFuelCellMassFlowMap(1);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var expFuelConsumption = exp_H2_g_p_h.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>();
		TestContext.WriteLine($"MinEffPower {massFlowMap.MinEffPower.ConvertToKiloWatt()}");
		var fc = massFlowMap.Lookup(power);

		Assert.That(fc.IsEqual(expFuelConsumption), "exp {0} got {1}", expFuelConsumption.Value(), fc.Value());
	}

	[TestCase(600)]
	//[TestCase(10)] <- supported now with timeslicing 
	[TestCase(-100)]
	public void FuelCellMassFlowMap_LookupTestFail(double power_kW) {
		var massFlowMap = GetFuelCellMassFlowMap(1);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();

		Assert.That(() => massFlowMap.Lookup(power), Throws.TypeOf(typeof(VectoException)));
	}


    [Test]
	public void InvalidFuelCellMassFlowMapTest() {
		Assert.Fail("Add checks if entry is present twice" +
					"Add checks there are enough entries present" +
					"..." +
					"" +
					"");
	}





	[TestCase(30, 1)]
	[TestCase(60, 1)]
	[TestCase(90, 1)]
	[TestCase(110, 2)]

	[TestCase(120, 2)]


	[TestCase(180, 3)]

	[TestCase(900, 3)]
    public void FuelCellString_FuelCellCountTest(double power_kW, int activeFc)
	{
		int variant = 1;
		var map = GetFuelCellMassFlowMap(variant);
		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var fcString = new FuelCellStringMassFlowMap(map, 3);


		var limits = GetLimits(variant);
		var dir = BASEOUTPUTPATH + $"fuel_cell_count_test_{power_kW}";

        PrintFcMap(limits.min, limits.max, variant, dir );
		PrintString(limits.min, limits.max, variant, 3, dir);


		Assert.That(fcString.GetActiveFuelCellCount(power), Is.EqualTo(activeFc));

    }

	[Test]
	public void FuelCellString_MinMaxPowerTest() {
		var map = GetFuelCellMassFlowMap(1);
		var fcString = new FuelCellStringMassFlowMap(map, 3);
		Assert.That(fcString.MinPower.Value(), Is.EqualTo(30*1e3));
		Assert.That(fcString.MaxPower.Value(), Is.EqualTo(900*1e3));
		Assert.That(fcString.MinPowerEff.Value(), Is.EqualTo(60 * 1e3));
	}

	[TestCase(30, 1501.5)]
	[TestCase(60, 3003)]
    [TestCase(120, 6006)]
	[TestCase(180, 9009)]

    public void FuelCellString_LookupTest(double power_kW, double exp_H2_g_p_h) {
		var map = GetFuelCellMassFlowMap(1);
		var fcString = new FuelCellStringMassFlowMap(map, 3);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var expFuelConsumption = exp_H2_g_p_h.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>();


        Assert.That(fcString.Lookup(power), Is.EqualTo(expFuelConsumption));
	}

	[Test]
	public void FuelCellString_MeasuredPointsTest()
	{
		var fcMap = GetFuelCellMassFlowMap(1);
		var fcString = new FuelCellStringMassFlowMap(fcMap, 3);
		var measuredPoints = fcString.MeasuredPoints;


		var expected = new List<double>() {
			30,60,105,120,180,315, 450, 585, 765, 900
		}.Select(d => (d * 1e3).SI<Watt>()).ToList();




		Assert.That(expected.Count, Is.EqualTo(measuredPoints.Count));
		for (int i = 0; i < measuredPoints.Count; i++) {
			Assert.That(measuredPoints[i], Is.EqualTo(expected[i]));
		}
	}


	//[TestCase(1, 1, 3, 3)]
	//[TestCase(1, 1, 1, 0)]
	//[TestCase(1,2, 3, 3)]
	[Combinatorial]
	public void FuelCellSystem_GetShareSmokeTest([Range(1,3)] int string1Variant, [Range(1,3)]int string2Variant, [Range(1,3)]int string1Count,  [Range(0,3)]int string2Count)
	{
		var fuelCellSystemMassFlowMap = new FuelCellSystemMassFlowMap(GetFuelCellStringMassFlowMap(
				count: string1Count, 
				variant:string2Variant),
			string2Count != 0 
				? GetFuelCellStringMassFlowMap(
				count: string2Count, 
				variant:string2Variant) : null);


		//var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();



		var fuelCellSystemShare = new FuelCellSystemShareMap(fuelCellSystemMassFlowMap);

		var step = (fuelCellSystemMassFlowMap.MaxPower - fuelCellSystemMassFlowMap.MinPower) / 10000;
		var power = fuelCellSystemMassFlowMap.MinPower;
		while (power.IsSmallerOrEqual(fuelCellSystemMassFlowMap.MaxPower)) {
			var share = fuelCellSystemShare.Lookup(power);
			power += step;
		}
	}
	

	[TestCase(47700, new double[]{0.0, 1, 0.37, 0.63})]
	[TestCase(1, new double[] { 0.0, 1 })]
	[TestCase(150e3, new double[] { 0.2, 0.8 })]
	public void FuelCellSystem_GetShareTestEqualFuelCell(double power_W, double[] expected_a_s)
	{
		TestShares(power_W, expected_a_s, 1, 1);
	}

	[TestCase(200e3, new double[] { 0 })]
	[TestCase(1, new double[] { 0 })]
	[TestCase(550e3, new double[] { 0.24 })]
	[TestCase(150e3,  new double[] { 0.07 })]
    public void FuelCellSystem_GetShareTestDifferentFuelCellSameRange(double power_W, double[] expected_a_s)
	{
		Assert.That(expected_a_s.Count() > 0);

		var fc1Limits = GetLimits(1, true);
		var fc2Limits = GetLimits(2, true);

		PrintPowerDistribution(1, 3, 2, 3, power_W, BASEOUTPUTPATH + $"ShareTestDifferentFuelCellSameRange_{power_W / 1000} kW/", fc1Limits.min, fc1Limits.max, fc2Limits.min, fc2Limits.max);
        TestShares(power_W, expected_a_s, 1, 2);
	}

	


	[TestCase(116667.4412, new double[]{0}) ]
	public void FuelCellSystem_GetShare_GenericFCHV(double power_W, double[] expected_a_s)
	{

		Assert.That(expected_a_s.Count() > 0);

		var var1 = 1;
		var var2 = 4;

		(double min, double max) fc1Limits_W = (30 * 1000, 100 * 1000);
		(double min, double max) fc2Limits_W = (30 * 1000, 100 * 1000);

        PrintPowerDistribution(var1, 3, var2, 3, power_W, BASEOUTPUTPATH + $"GenericFCHV{power_W / 1000} kW/", fc1Limits_W.min, fc1Limits_W.max, fc2Limits_W.min, fc2Limits_W.max);
		TestShares(power_W, expected_a_s, var1, var2);
	}

	[TestCase(493e3, new double[]{0.60}, 1, 1)]
	[TestCase(495e3, new double[]{0.60}, 1, 1)]
	[TestCase(494e3, new double[]{0.15}, 3, 3)]
    public void FuelCellSystem_GetShareTestDifferentFuelCellDifferentRange(double power_W, double[] expected_a_s, int countfc1, int countfc2)
	{
		TestShares(power_W, expected_a_s, 1,3, countfc1:countfc1, countfc2:countfc2);



	}
    /// <summary>
    /// Tests if the fuel cell power distribution between the fuel cell strings.
    /// Visualizations for the power distribution can be generated by adding a parametrized test case to 'test_multiple_fuelcells' in "\Documentation\Additional Material\TestCases FuelCell\TestCases.py"
    /// For existing test cases, parametrized python tests are already provided. When the tests are executed all the relevant output files can be found in these folders:
    /// <list type="bullet">
    /// <item><see cref="FuelCellSystem_GetShareTestEqualFuelCell"/>: "testoutput/same_fc_same_range/"</item>
    /// <item><see cref="FuelCellSystem_GetShareTestDifferentFuelCellDifferentRange"/>: "different_fc_different_range/", "different_fc_different_range_#3/" </item>
	/// <item><see cref="FuelCellSystem_GetShareTestDifferentFuelCellSameRange"/>: "testoutput/different_fc_same_range/" </item>
    /// </list>
    /// </summary>
    /// <param name="power_W"></param>
    /// <param name="expected_a_s"></param>
    /// <param name="variantFc1"></param>
    /// <param name="variantFc2"></param>
    /// <param name="countfc1"></param>
    /// <param name="countfc2"></param>
    private void TestShares(double power_W, double[] expected_a_s, int variantFc1, int variantFc2, int countfc1 = 3, int countfc2 = 3)
	{
		var fuelCellSystemMassFlowMap = new FuelCellSystemMassFlowMap(GetFuelCellStringMassFlowMap(variant: variantFc1, count: countfc1),
			GetFuelCellStringMassFlowMap(variant: variantFc2, count: countfc2));

		var power = power_W.SI(Unit.SI.Watt).Cast<Watt>();
		

		TestContext.Progress.WriteLine(fuelCellSystemMassFlowMap.FuelCell1.MinPowerEff);

		var fuelCellSystemShare = new FuelCellSystemShareMap(fuelCellSystemMassFlowMap);
		var shares = fuelCellSystemShare.GetSharesWithLowestFuelConsumption(power).ToList();
		var fc = shares.First().FuelConsumption;
		Assert.That(shares.All(s => s.FuelConsumption.IsEqual(fc, 1E-12.SI<KilogramPerSecond>())));

		TestContext.Progress.WriteLine(shares.Select(s => s.FuelConsumption.ConvertToGrammPerHour()).Join("\n"));


		var share = fuelCellSystemShare.Lookup(power);

		var orderedShares = shares.OrderBy(a => a.Share.ShareA).ToArray();
		foreach (var fuelCellShareEntry in orderedShares)
		{
			TestContext.Progress.WriteLine(
				$"Share a:{fuelCellShareEntry.Share.ShareA} b: {fuelCellShareEntry.Share.ShareB}");
		}

		Assert.AreEqual(expected_a_s.Length, shares.Count());
		var orderedExpected = expected_a_s.OrderBy(a => a).ToArray();
		for (int i = 0; i < shares.Count(); i++)
		{
			Assert.That(orderedShares[i].Share.ShareA.IsEqual(orderedExpected[i], 1E-02),
				$"Expected {orderedExpected[i]} but was {orderedShares[i].Share.ShareA}");
		}
	}



    [TestCase(214000, 3,3, 2, Description="The maximum power of the fuel cell is smaller than 2x minEffPower")]
	public void GetActiveFuelCellCount(double power_W, int fcVariant, int fcCount, int expectedFcCount)
	{
		var fc = GetFuelCellStringMassFlowMap(fcVariant, fcCount);
		var p = power_W.SI<Watt>();
		Assert.That(fc.GetActiveFuelCellCount(p), Is.EqualTo(expectedFcCount));

	}

	public IFuelCellComponentEngineeringInputData GetFuelCellComponent((double, double)[] valueTuples, Watt minPower, Watt maxPower, string model = "mock")
	{
		var mock = new Mock<IFuelCellComponentEngineeringInputData>();
			mock.SetupGet(fc => fc.MassFlowMap).Returns(new TableData() { });

		mock.Object.MassFlowMap.Columns.AddRange(new [] {
			new DataColumn("power"),
			new DataColumn("fuel consumption")
		});
		foreach (var valueTuple in valueTuples) {
			var row = mock.Object.MassFlowMap.NewRow();

			row[0] = valueTuple.Item1;
			row[1] = valueTuple.Item2;
			mock.Object.MassFlowMap.Rows.Add(row);
		}

		mock.SetupGet(fcc => fcc.MaxElectricPower).Returns(maxPower);
		mock.SetupGet(fcc => fcc.MinElectricPower).Returns(minPower);
		mock.SetupGet(fcc => fcc.Manufacturer).Returns("");
		mock.SetupGet(fcc => fcc.Model).Returns(model);


        return mock.Object;
	}

	public IFuelCellComponentEngineeringInputData GetFuelCellComponent(int variant, Watt minPower_perFc, Watt maxPower_perFc)
	{
		return GetFuelCellComponent(GetTable(variant), minPower_perFc, maxPower_perFc, $"Variant {variant}");

	}


	[TestCase(30, 200,1,2, 400)]
	public void TestMaxPowerPerString(double minPower_perFc_kW, double maxPower_perFc_kW, int fcVariant, int fcCount, int expMaxPower_kW)
	{


		var maxPower = maxPower_perFc_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var minPower = minPower_perFc_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();


		///CHECK if max limits are respected in simulation components

		var fuelCellSystem = GetFuelCellSystemInputData(fcVariant, minPower_perFc_kW * 1000, maxPower_perFc_kW * 1000, fcCount);

		var dao = new EngineeringDataAdapter();
		var data = dao.CreateFuelCellSystemData(fuelCellSystem);

		Assert.IsTrue(data.MaxElectricPower.IsEqual(expMaxPower_kW * 1000));

	}

	public IFuelCellSystemEngineeringInputData GetFuelCellSystemInputData(int fc1Variant, double fc1MinPower_w, double fc1MaxPower_w, int fc1Count, 
		int? fc2Variant = null, double? fc2MinPower_w = null, double? fc2MaxPower_w = null, int? fc2Count = null)
	{
		if (fc2Variant == null && (fc2Count != null || fc2MinPower_w != null || fc2MaxPower_w != null)) {
			throw new ArgumentException("Either all or none parameter for second fuel cell string must be provided");
		}

		var fuelCellSystem = new Mock<IFuelCellSystemEngineeringInputData>();

		var fcList = new List<FuelCellStringEntry<IFuelCellComponentEngineeringInputData>>() {
			new FuelCellStringEntry<IFuelCellComponentEngineeringInputData>() {
				Count = fc1Count,
				FuelCellComponent = GetFuelCellComponent(fc1Variant, fc1MinPower_w.SI<Watt>(), fc1MaxPower_w.SI<Watt>())
			}
		};
		if (fc2Count != null) {
			fcList.Add(new FuelCellStringEntry<IFuelCellComponentEngineeringInputData>() {
				Count = fc2Count.Value,
				FuelCellComponent = GetFuelCellComponent(fc2Variant.Value, fc2MinPower_w.Value.SI<Watt>(), fc2MaxPower_w.Value.SI<Watt>())
			});
		}

        fuelCellSystem.SetupGet(fcs => fcs.FuelCellStrings).Returns(fcList);
		return fuelCellSystem.Object;
	}



	[TestCase( 30, 200, 1, BASEOUTPUTPATH + "Variant1")]
	[TestCase(30, 200, 1, BASEOUTPUTPATH + "Variant2")]
    public void PrintFcMap(double fcMin_kW, double fcMax_kW, int variant, string outputFile)
	{
		var fcComponent = GetFuelCellComponent(1, fcMin_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>(), fcMax_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>());
		PrintFcMap(fcComponent, outputFile);
	}




	public void PrintFcMap(IFuelCellComponentEngineeringInputData component, string outputDirectory)
	{
		var plt = new ScottPlot.Plot();
		var dao = new EngineeringDataAdapter();

		var fcData = dao.CreateFuelCellData(component);



		var maxEntry = fcData.MassFlowMap.Entries.MaxBy(e => e.P_el_out).P_el_out;
		var minEntry = fcData.MassFlowMap.Entries.MinBy(e => e.P_el_out).P_el_out;


		var x = DataGen.Range(minEntry.Value(), maxEntry.Value(), 500);
		var x_from_zero = DataGen.Range(0, maxEntry.Value(), 500);

		var x_measured = fcData.MassFlowMap.MeasuredPoints;



		var fc = x.Select(p => fcData.MassFlowMap.Lookup(p.SI<Watt>(), false).ConvertToGrammPerHour().Value).ToArray();

		var fcMeasured = x_measured.Select(p => fcData.MassFlowMap.Lookup(p, false).ConvertToGrammPerHour().Value).ToArray();
		var fcTimeSliceEnabled = x_from_zero.Select(p => fcData.MassFlowMap.Lookup(p.SI<Watt>(), true).ConvertToGrammPerHour().Value).ToArray();


		var fcPairs = x.Zip(fc, (x, fc) => (x, fc));
		var eff = fcPairs.Select(pair => pair.fc / pair.x);



		var measuredfcScatter = plt.AddScatter(xs: x_measured.Select(x => x.Value()).ToArray(),
			ys: fcMeasured, color: Color.BlueViolet, lineStyle: LineStyle.None,
			markerShape: MarkerShape.cross, label: "Measured FC");
		var fcScatter = plt.AddScatter(xs: x, ys: fc, color: Color.Green, markerShape: MarkerShape.none, label:"Fuel consumption");
		var timesliceFcScatter = plt.AddScatter(xs: x_from_zero, ys: fcTimeSliceEnabled, color: Color.Pink, markerShape: MarkerShape.none, lineStyle:LineStyle.Dash, label: "Fuel consumption (time slicing)");

		plt.AddScatter(new[] { fcData.MinEffPower.Value() },
			new[] { fcData.MassFlowMap.Lookup(fcData.MinEffPower).ConvertToGrammPerHour().Value },
			lineStyle: LineStyle.None, color: Color.Red, markerShape: MarkerShape.openCircle, markerSize: 8F);


		plt.AddVerticalLine(fcData.MinElectricPower.Value(), label:"Min electric Power");
		plt.AddVerticalLine(fcData.MaxElectricPower.Value(), label: "Max electric Power");
        var effScatter = plt.AddScatter(x, eff.ToArray(), color: Color.Coral, markerShape: MarkerShape.none,
			label: "Efficiency");

		var yaxis3 = plt.AddAxis(Edge.Right, title: "efficiency in g/Wh");
		effScatter.YAxisIndex = yaxis3.AxisIndex;


        plt.Title($"Massflow Map {component.Manufacturer} {component.Model}");
		plt.Legend(true, Alignment.LowerRight);

		plt.XLabel("power in W");
		plt.YLabel("Fuel consumption in g/h");



		var outputFile = $"{outputDirectory}/massflowmap_fc_{component.Model}.png";
		if (!File.Exists(outputFile)) {
			Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
		}
        plt.SaveFig(outputFile);

	}

	[TestCase(30, 300, 1, 3, BASEOUTPUTPATH + "Fuel Cell String Var 1")]
	[TestCase(30, 300, 2, 3, BASEOUTPUTPATH + "Fuel Cell String Var 2")]
	[TestCase(30, 195, 3, 3, BASEOUTPUTPATH + "Fuel Cell String Var 3")]
    public void PrintString(double fcMin_kW, double fcMax_kW, int variant, int fcCount, string outputDirectory)
	{
		var fcSystem = GetFuelCellSystemInputData(variant, fcMin_kW*1000, fcMax_kW*1000, fcCount);
		var fuelCellString = fcSystem.FuelCellStrings.First();

		PrintFcString(fuelCellString, outputDirectory, $"variant{variant}_count{fcCount}");



	}

	private void PrintFcString(FuelCellStringEntry<IFuelCellComponentEngineeringInputData> fuelCellString, string outputDirectory, string suffix)
	{
		var engineeringDataAdapter = new EngineeringDataAdapter();
		var fcStringData = engineeringDataAdapter.CreateFuelCellStringData(fuelCellString.FuelCellComponent, fuelCellString.Count);


		var p = DataGen.Range(0, fcStringData.MaxPower.Value(), step: 50, true);
		var pMeasured = fcStringData.MassFlowMap.SupportingPoints.Where(p => p.IsSmallerOrEqual(fcStringData.MaxPower)).Select(p => p.Value()).ToArray();


        var fc = p.Select(p => fcStringData.MassFlowMap.Lookup(p.SI<Watt>()).ConvertToGrammPerHour().Value).ToArray();
		var fcMeasured = pMeasured.Select(p => fcStringData.MassFlowMap.Lookup(p.SI<Watt>()).ConvertToGrammPerHour().Value).ToArray();
		var activeFc = p.Select(p => (double)fcStringData.MassFlowMap.GetActiveFuelCellCount(p.SI<Watt>())).ToArray();







		var plt = new Plot();



		plt.AddScatterLines(p, fc, label:"Fuel Consumption");
		plt.AddScatterPoints(pMeasured, fcMeasured, label: "Fuel Consumption at measured points");
		var yaxis3 = plt.AddAxis(Edge.Right, title: "nr of active fcs");
		var active_fc_line = plt.AddScatterLines(p, activeFc, label: "Active FCS");
		active_fc_line.YAxisIndex = yaxis3.AxisIndex;
		var limits = plt.GetAxisLimits();

        for (int i = 1; i <= fcStringData.FcCount; i++) {
			var fcAllActive = p.Select(p => {
				var fc = LookupMaxActiveFc(fcStringData, p.SI<Watt>(), out var fcCount, i);
				return (fc, fcCount);
			}).ToArray();
			var fuelConsumptionAllActive = plt.AddScatterLines(p, fcAllActive.Select(tuple => tuple.fc).ToArray(), label: $"Fuel consumption (max fcs = {i})", lineStyle: LineStyle.Dot);
			var activeFcAllActive = plt.AddScatterLines(p, fcAllActive.Select(tuple => (double)tuple.fcCount).ToArray(),
				label: $"Nr of active fcs (max fcs = {i})", lineStyle: LineStyle.Dot);
			//var measuredMax = pMeasured.Select(p => p * i);

			//var measured = plt.AddScatterPoints(measuredMax.ToArray(), measuredMax.Select(m =>
			//	LookupMaxActiveFc(fcStringData, power: m.SI<Watt>(), out _, i)).ToArray());
			activeFcAllActive.YAxisIndex = yaxis3.AxisIndex;
		}




        plt.Legend(true, Alignment.LowerRight);

		plt.XLabel("Power in W");
		plt.YLabel("Fuel consumption in g/h");
		plt.SetAxisLimits(xMax: fcStringData.MaxPower.Value());

		var outputFile = $"{outputDirectory}/massflowmap_fc_{suffix}.png";
		if (!File.Exists(outputFile))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
		}
		plt.SaveFig(outputFile, 800, 600, scale:2);
	}

	[TestCase(1, 1, 1, 1, 120e3, BASEOUTPUTPATH + "PowerDistribution_v1_c1_v1_c1_120kW", 30e3, 300e3, 30e3, 300e3 )]
	[TestCase(1, 1, 4, 1, 120e3, BASEOUTPUTPATH + "PowerDistribution_v1_c1_v4_c1_120kW", 30e3, 300e3, 30e3, 300e3)]
    public void PrintPowerDistribution(int var1, int count1, int var2, int count2, double power_W,
		string outputDirectory, double minPower1_W, double maxPower1_W, double minPower2_W, double maxPower2_W)
	{
		var fcs = GetFuelCellSystemInputData(var1, minPower1_W, maxPower1_W, count1, var2, minPower2_W, maxPower2_W, count2);
		PrintFcSystemPowerDistributions(fcs, outputDirectory, (power_W / 1000).ToString(CultureInfo.InvariantCulture), power_W.SI<Watt>());




    }


	private void PrintFcSystemPowerDistributions(IFuelCellSystemEngineeringInputData fuelCellSystem,
		string outputDirectory, string suffix, Watt power)
	{
		(double, double, double) Lookup(double a, Watt power, FuelCellStringData fcString1, FuelCellStringData fcString2)
		{
			try
			{
				var fc1 = fcString1.MassFlowMap.Lookup(a * power).ConvertToGrammPerHour().Value;
				var fc2 = fcString2.MassFlowMap.Lookup((1 - a) * power).ConvertToGrammPerHour().Value;
				var total = fc1 + fc2;
				return (total, fc1, fc2);
			}
			catch (Exception)
			{
				return (double.NaN, double.NaN, double.NaN);
			}
        }


		if (fuelCellSystem.FuelCellStrings.Count < 2) {
			Assert.Fail();
		}
		var dao = new EngineeringDataAdapter();
		var fuelCellSystemData = dao.CreateFuelCellSystemData(fuelCellSystem);
		fuelCellSystemData.FuelCellShareMap = dao.CreateFuelCellShareMap(fuelCellSystemData);

		var plt = new Plot(1920, 1080);

		var a = DataGen.Range(0, 1, 0.001);
		var shares = fuelCellSystemData.FuelCellShareMap.GetValidShares(power).ToArray();
        //var relevantPoints = fuelCellSystemData.FuelCellPowerMap.
        //var b = a.Select(a => 1 - a).ToArray();

        var fcString1 = fuelCellSystemData.FuelCellStrings[0];
		var fcString2 = fuelCellSystemData.FuelCellStrings[1];

		var fuelConsumption = a.Select(a => {
			return Lookup(a, power, fcString1, fcString2);
		});

		var total = plt.AddScatterLines(a, fuelConsumption.Select(fc => fc.Item1).ToArray(), label:"FC total in g/h");
		var fc1 = plt.AddScatterLines(a, fuelConsumption.Select(fc => fc.Item2).ToArray(), label:"FC String 1 in g/h");
		var fc2 = plt.AddScatterLines(a, fuelConsumption.Select(fc => fc.Item3).ToArray(), label:"FC String 2 in g/h");
		var share = plt.AddScatterPoints(shares.Select(s => s.ShareA).ToArray(), shares.Select(s => Lookup(s.ShareA, power, fcString1, fcString2).Item1).ToArray());

		plt.XAxis.Label("Share of String 1");
		plt.YAxis.Label("Fuel consumption in g/h");
		plt.Title($"Power distribution {power.ConvertToKiloWatt().ToOutputFormat(showUnit:true)}");
		plt.Legend(true, Alignment.MiddleRight);
		var outputFile = $"{outputDirectory}/powerDistribution_{power.ToOutputFormat()}.png";
		if (!File.Exists(outputFile))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
		}
		plt.SaveFig(outputFile, 1000, 400);

    }



	/// <summary>
	/// Looks up the fuel consumption in a string when all strings are switched on
	/// </summary>
	/// <param name="fcStringData"></param>
	/// <param name="power"></param>
	/// <returns>Fuel consumption in grams per hour</returns>
	private double LookupMaxActiveFc(FuelCellStringData fcStringData, Watt power, out int active, int max)
	{
		var fcData = fcStringData.FcData;
		var p_min = fcData.MinElectricPower;

		if (power.IsSmaller(2*p_min)) {
			active = 1;
			return fcData.MassFlowMap.Lookup(power, true).ConvertToGrammPerHour().Value;
		}

		active = VectoMath.Min((int)Math.Floor((power / p_min).Value()), max);

		if (power.IsGreater(active * fcStringData.FcData.MaxElectricPower)) {
			return double.NaN;
		}

		var fc = active * fcData.MassFlowMap.Lookup(power / active, active == 1).ConvertToGrammPerHour().Value;
		return fc;
	}




	public FuelCellStringMassFlowMap GetFuelCellStringMassFlowMap(int variant = 1, int count = 1)
	{
		return new FuelCellStringMassFlowMap(GetFuelCellMassFlowMap(variant), count);
	}

	public (double, double)[] GetTable(int variant)
	{

		var variant1 = new (double, double)[] {
			//P_el_out[kW],   m_H2[g / h]
			(30.0, 2002.0),
			(60.0, 3003.0),
			(105.0, 5436.5),
			(150.0, 8662.5),
			(195.0, 12199.7),
			(255.0, 17017.0),
			(300.0, 21450.0),
		};

		var variant2 = new (double, double)[] {
			//P_el_out [kW],	m_H2 [g/h]
			(30.0, 2002.0),
			(70.0, 3003.0),
			(140.0, 4436.5),
			(150.0, 7662.5),
			(195.0, 14199.7),
			(255.0, 19017.0),
			(300.0, 28450.0),
		};

		var variant3 = new (double, double)[] {
			//P_el_out [kW],	m_H2 [g/h]
			(30.0, 2002.0),
			(70.0, 3003.0),
			(140.0, 4436.5),
			(150.0, 7662.5),
			(195.0, 14199.7),
		};

		var variant4 = new (double, double)[] {

			//P_el_out [kW],  m_H2 [g/h]
			(30.0,           2002.0),
			(60.0,           2503.0),
			(105.0,          4436.5),
			(150.0,          7662.5),
			(195.0,          10199.7),
			(255.0,          14017.0),
			(300.0,          18450.0),

        };



		switch (variant) {
			case 1:
				return variant1;
			case 2:
				return variant2;
			case 3:
				return variant3;
			case 4:
				return variant4;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public (double min, double max) GetLimits(int variant, bool inWatt = false)
	{
		var table = GetTable(variant);

		var factor = inWatt ? 1000 : 1;
		return (table.First().Item1 * factor, table.Last().Item1 * factor);
	}



	public FuelCellMassFlowMap GetFuelCellMassFlowMap(int variant = 1)
	{
		FuelCellMassFlowMap CreateFuelCellMassFlowMap((double, double)[] valueTuples)
		{
			return new FuelCellMassFlowMap(valueTuples.Select(e => new FuelCellMassFlowMap.MassFlowMapEntry() {
				P_el_out = e.Item1.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
				H2 = e.Item2.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>()
			}).ToArray());
		}

		return CreateFuelCellMassFlowMap(GetTable(variant));
	}
}