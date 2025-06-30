using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Models.Declaration.BusAux;

[TestFixture, Parallelizable(ParallelScope.All)]
public class SSMTestHeatingCooling
{
	private const BusHVACSystemConfiguration CFG1 = BusHVACSystemConfiguration.Configuration1;
	private const BusHVACSystemConfiguration CFG2 = BusHVACSystemConfiguration.Configuration2;
	private const BusHVACSystemConfiguration CFG3 = BusHVACSystemConfiguration.Configuration3;
	private const BusHVACSystemConfiguration CFG4 = BusHVACSystemConfiguration.Configuration4;
	private const BusHVACSystemConfiguration CFG5 = BusHVACSystemConfiguration.Configuration5;
	private const BusHVACSystemConfiguration CFG6 = BusHVACSystemConfiguration.Configuration6;
	private const BusHVACSystemConfiguration CFG7 = BusHVACSystemConfiguration.Configuration7;
	private const BusHVACSystemConfiguration CFG8 = BusHVACSystemConfiguration.Configuration8;
	private const BusHVACSystemConfiguration CFG9 = BusHVACSystemConfiguration.Configuration9;
	private const BusHVACSystemConfiguration CFG10 = BusHVACSystemConfiguration.Configuration10;

	private const HeatingDistributionCase HDC1 = HeatingDistributionCase.HeatingDistribution1;
	private const HeatingDistributionCase HDC2 = HeatingDistributionCase.HeatingDistribution2;
	private const HeatingDistributionCase HDC3 = HeatingDistributionCase.HeatingDistribution3;
	private const HeatingDistributionCase HDC4 = HeatingDistributionCase.HeatingDistribution4;
	private const HeatingDistributionCase HDC5 = HeatingDistributionCase.HeatingDistribution5;
	private const HeatingDistributionCase HDC6 = HeatingDistributionCase.HeatingDistribution6;
	private const HeatingDistributionCase HDC7 = HeatingDistributionCase.HeatingDistribution7;
	private const HeatingDistributionCase HDC8 = HeatingDistributionCase.HeatingDistribution8;
	private const HeatingDistributionCase HDC9 = HeatingDistributionCase.HeatingDistribution9;
	private const HeatingDistributionCase HDC10 = HeatingDistributionCase.HeatingDistribution10;
	private const HeatingDistributionCase HDC11 = HeatingDistributionCase.HeatingDistribution11;
	private const HeatingDistributionCase HDC12 = HeatingDistributionCase.HeatingDistribution12;

	private const HeatPumpType HeatPumpNone = HeatPumpType.none;
	private const HeatPumpType HeatPumpR744 = HeatPumpType.R_744;
	private const HeatPumpType HeatPump2Stage = HeatPumpType.non_R_744_2_stage;
	private const HeatPumpType HeatPump3Stage = HeatPumpType.non_R_744_3_stage;
	private const HeatPumpType HeatPumpCont = HeatPumpType.non_R_744_continuous;
	private const HeaterType NoElHtr = HeaterType.None;
	private const HeaterType AirElHtr = HeaterType.AirElectricHeater;
	private const HeaterType WaterElHtr = HeaterType.WaterElectricHeater;
	private const HeaterType OthrElHtr = HeaterType.OtherElectricHeating;
	private const double AuxHtrPwr0 = 0.0;
	private const double AuxHtrPwr30 = 30.0e3;


	[
	// only mechanical heatpumps
	TestCase(CFG1  , 1  , HeatPumpNone      , HeatPumpNone                 , 299.88    , 0.0)       ,
	TestCase(CFG1  , 6  , HeatPumpNone      , HeatPumpNone                 , 299.88    , 0.0)       ,
	TestCase(CFG1  , 8  , HeatPumpNone      , HeatPumpNone                 , 287.8848  , 0.0)       ,
	TestCase(CFG1  , 11 , HeatPumpNone      , HeatPumpNone                 , 287.8848  , 0.0)       ,

	TestCase(CFG2  , 1  , HeatPump2Stage    , HeatPumpNone                 , 299.88    , 0.0)       ,
	TestCase(CFG2  , 6  , HeatPump2Stage    , HeatPumpNone                 , 299.88    , 0.0)       ,
	TestCase(CFG2  , 8  , HeatPump2Stage    , HeatPumpNone                 , 287.8848  , 1.0156)    ,
	TestCase(CFG2  , 11 , HeatPump2Stage    , HeatPumpNone                 , 287.8848  , 1351.1654) ,

	TestCase(CFG3  , 1  , HeatPumpNone       , HeatPumpNone                 , 428.400   , 0.0)       ,
	TestCase(CFG3  , 6  , HeatPumpNone       , HeatPumpNone                 , 856.800   , 0.0)       ,
	TestCase(CFG3  , 8  , HeatPumpNone       , HeatPumpNone                 , 822.5280  , 0.0)       ,
	TestCase(CFG3  , 11 , HeatPumpNone       , HeatPumpNone                 , 822.5280  , 0.0)       ,

	TestCase(CFG4  , 1  , HeatPump2Stage     , HeatPumpNone                 , 428.400   , 0.0)       ,
	TestCase(CFG4  , 6  , HeatPump2Stage     , HeatPumpNone                 , 428.4     , 0.0)       ,
	TestCase(CFG4  , 8  , HeatPump2Stage     , HeatPumpNone                 , 822.5280  , 1.0157)  ,
	TestCase(CFG4  , 11 , HeatPump2Stage     , HeatPumpNone                 , 822.5280  , 1351.1654)    ,

	TestCase(CFG5  , 1  , HeatPumpNone       , HeatPump3Stage    , 428.400   , 0.0)       ,
	TestCase(CFG5  , 6  , HeatPumpNone       , HeatPump3Stage    , 856.80    , 0.0)       ,
	TestCase(CFG5  , 8  , HeatPumpNone       , HeatPump3Stage    , 822.528   , 612.7957)  ,
	TestCase(CFG5  , 11 , HeatPumpNone       , HeatPump3Stage    , 822.528   , 2965.1077) ,

	TestCase(CFG6  , 1  , HeatPumpNone       , HeatPump3Stage    , 428.400   , 0.0)       ,
	TestCase(CFG6  , 6  , HeatPumpNone       , HeatPump3Stage    , 856.80    , 0.0)       ,
	TestCase(CFG6  , 8  , HeatPumpNone       , HeatPump3Stage    , 822.528   , 612.7957)  ,
	TestCase(CFG6  , 11 , HeatPumpNone       , HeatPump3Stage    , 822.528   , 2965.1077) ,

	TestCase(CFG7  , 1  , HeatPump2Stage     , HeatPump3Stage    , 428.400   , 0.0)       ,
	TestCase(CFG7  , 6  , HeatPump2Stage     , HeatPump3Stage    , 856.80    , 0.0)       ,
	TestCase(CFG7  , 8  , HeatPump2Stage     , HeatPump3Stage    , 822.528   , 616.1931)  ,
	TestCase(CFG7  , 11 , HeatPump2Stage     , HeatPump3Stage    , 822.528   , 2984.8347) ,

	TestCase(CFG8  , 1  , HeatPumpNone       , HeatPump3Stage    , 428.400   , 0.0)       ,
	TestCase(CFG8  , 6  , HeatPumpNone       , HeatPump3Stage    , 856.80    , 0.0)       ,
	TestCase(CFG8  , 8  , HeatPumpNone       , HeatPump3Stage    , 822.528   , 612.7957)  ,
	TestCase(CFG8  , 11 , HeatPumpNone       , HeatPump3Stage    , 822.528   , 2965.1077) ,

	TestCase(CFG9  , 1  , HeatPump2Stage     , HeatPump3Stage    , 428.400   , 0.0)       ,
	TestCase(CFG9  , 6  , HeatPump2Stage     , HeatPump3Stage    , 856.80    , 0.0)       ,
	TestCase(CFG9  , 8  , HeatPump2Stage     , HeatPump3Stage    , 822.528   , 616.1931)  ,
	TestCase(CFG9  , 11 , HeatPump2Stage     , HeatPump3Stage    , 822.528   , 2984.8347) ,

	TestCase(CFG10 , 1  , HeatPumpNone       , HeatPump3Stage    , 428.400   , 0.0)       ,
	TestCase(CFG10 , 6  , HeatPumpNone       , HeatPump3Stage    , 856.80    , 0.0)       ,
	TestCase(CFG10 , 8  , HeatPumpNone       , HeatPump3Stage    , 822.528   , 612.7957)  ,
	TestCase(CFG10 , 11 , HeatPumpNone       , HeatPump3Stage    , 822.528   , 2965.1077) ,

	// only electrical heatpumps
	TestCase(CFG2  , 1  , HeatPumpCont       , HeatPumpNone   , 299.88    , 0.0)       ,
	TestCase(CFG2  , 6  , HeatPumpCont       , HeatPumpNone   , 299.88    , 0.0)       ,
	TestCase(CFG2  , 8  , HeatPumpCont       , HeatPumpNone   , 288.8324  , 0)         ,
	TestCase(CFG2  , 11 , HeatPumpCont       , HeatPumpNone   , 1520.7    , 0)         ,

	TestCase(CFG2  , 1  , HeatPumpR744       , HeatPumpNone   , 299.88    , 0.0)       ,
	TestCase(CFG2  , 6  , HeatPumpR744       , HeatPumpNone   , 299.88    , 0.0)       ,
	TestCase(CFG2  , 8  , HeatPumpR744       , HeatPumpNone   , 288.8524  , 0)         ,
	TestCase(CFG2  , 11 , HeatPumpR744       , HeatPumpNone   , 1866.3491 , 0)         ,

	TestCase(CFG4  , 1  , HeatPumpCont       , HeatPumpNone  , 428.400   , 0.0)       ,
	TestCase(CFG4  , 6  , HeatPumpCont       , HeatPumpNone  , 428.4     , 0.0)       ,
	TestCase(CFG4  , 8  , HeatPumpCont       , HeatPumpNone  , 823.4756, 0)         ,
	TestCase(CFG4  , 11 , HeatPumpCont       , HeatPumpNone  , 2055.3432, 0)         ,

	TestCase(CFG4  , 1  , HeatPumpR744       , HeatPumpNone  , 428.400   , 0.0)       ,
	TestCase(CFG4  , 6  , HeatPumpR744       , HeatPumpNone  , 428.4     , 0.0)       ,
	TestCase(CFG4  , 8  , HeatPumpR744       , HeatPumpNone  , 823.4956  , 0)         ,
	TestCase(CFG4  , 11 , HeatPumpR744       , HeatPumpNone  , 2400.9923 , 0)         ,

	TestCase(CFG5  , 1  , HeatPumpNone       , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG5  , 6  , HeatPumpNone       , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG5  , 8  , HeatPumpNone       , HeatPumpCont , 1413.2125 , 0)         ,
	TestCase(CFG5  , 11 , HeatPumpNone       , HeatPumpCont , 3636.1338 , 0)         ,

	TestCase(CFG5  , 1  , HeatPumpNone       , HeatPumpR744 , 428.400   , 0.0)       ,
	TestCase(CFG5  , 6  , HeatPumpNone       , HeatPumpR744 , 856.80    , 0.0)       ,
	TestCase(CFG5  , 8  , HeatPumpNone       , HeatPumpR744 , 1425.648, 0)         ,
	TestCase(CFG5  , 11 , HeatPumpNone       , HeatPumpR744 , 4424.9953, 0)         ,

	TestCase(CFG6  , 1  , HeatPumpNone       , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG6  , 6  , HeatPumpNone       , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG6  , 8  , HeatPumpNone       , HeatPumpCont , 1413.2125 , 0)         ,
	TestCase(CFG6  , 11 , HeatPumpNone       , HeatPumpCont , 3636.1338 , 0)         ,
	
	TestCase(CFG6  , 1  , HeatPumpNone       , HeatPumpR744 , 428.400   , 0.0)       ,
	TestCase(CFG6  , 6  , HeatPumpNone       , HeatPumpR744 , 856.80    , 0.0)       ,
	TestCase(CFG6  , 8  , HeatPumpNone       , HeatPumpR744 , 1425.648, 0)         ,
	TestCase(CFG6  , 11 , HeatPumpNone       , HeatPumpR744 , 4424.9953, 0)         ,

	TestCase(CFG7  , 1  , HeatPumpCont       , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG7  , 6  , HeatPumpCont       , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG7  , 8  , HeatPumpCont       , HeatPumpCont , 1413.2125 , 0)         ,
	TestCase(CFG7  , 11 , HeatPumpCont       , HeatPumpCont , 3636.1338 , 0)         ,

	TestCase(CFG7  , 1  , HeatPumpR744       , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG7  , 6  , HeatPumpR744       , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG7  , 8  , HeatPumpR744       , HeatPumpCont , 1415.3128, 0)         ,
	TestCase(CFG7  , 11 , HeatPumpR744       , HeatPumpCont , 3746.1447, 0)         ,

	TestCase(CFG7  , 1  , HeatPumpCont       , HeatPumpR744 , 428.400   , 0.0)       ,
	TestCase(CFG7  , 6  , HeatPumpCont       , HeatPumpR744 , 856.80    , 0.0)       ,
	TestCase(CFG7  , 8  , HeatPumpCont       , HeatPumpR744 , 1423.474, 0)         ,
	TestCase(CFG7  , 11 , HeatPumpCont       , HeatPumpR744 , 4259.4119, 0)         ,

	TestCase(CFG8  , 1  , HeatPumpNone       , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG8  , 6  , HeatPumpNone       , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG8  , 8  , HeatPumpNone       , HeatPumpCont , 1413.2125 , 0)         ,
	TestCase(CFG8  , 11 , HeatPumpNone       , HeatPumpCont , 3636.1338 , 0)         ,

	TestCase(CFG8  , 1  , HeatPumpNone       , HeatPumpR744 , 428.400   , 0.0)       ,
	TestCase(CFG8  , 6  , HeatPumpNone       , HeatPumpR744 , 856.80    , 0.0)       ,
	TestCase(CFG8  , 8  , HeatPumpNone       , HeatPumpR744 , 1425.648, 0)         ,
	TestCase(CFG8  , 11 , HeatPumpNone       , HeatPumpR744 , 4424.9953, 0)         ,

	TestCase(CFG9  , 1  , HeatPumpCont       , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG9  , 6  , HeatPumpCont       , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG9  , 8  , HeatPumpCont       , HeatPumpCont , 1413.2125 , 0)         ,
	TestCase(CFG9  , 11 , HeatPumpCont       , HeatPumpCont , 3636.1338 , 0)         ,

	TestCase(CFG9  , 1  , HeatPumpR744       , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG9  , 6  , HeatPumpR744       , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG9  , 8  , HeatPumpR744       , HeatPumpCont , 1415.3128, 0)         ,
	TestCase(CFG9  , 11 , HeatPumpR744       , HeatPumpCont , 3746.1447, 0)         ,

	TestCase(CFG9  , 1  , HeatPumpCont       , HeatPumpR744 , 428.400   , 0.0)       ,
	TestCase(CFG9  , 6  , HeatPumpCont       , HeatPumpR744 , 856.80    , 0.0)       ,
	TestCase(CFG9  , 8  , HeatPumpCont       , HeatPumpR744 , 1423.474, 0)         ,
	TestCase(CFG9  , 11 , HeatPumpCont       , HeatPumpR744 , 4259.4119, 0)         ,

	TestCase(CFG10 , 1  , HeatPumpNone       , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG10 , 6  , HeatPumpNone       , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG10 , 8  , HeatPumpNone       , HeatPumpCont , 1413.2125 , 0)         ,
	TestCase(CFG10 , 11 , HeatPumpNone       , HeatPumpCont , 3636.1338 , 0)         ,

	// mixed electrical and mechanical heatpumps
	TestCase(CFG7  , 1  , HeatPumpCont       , HeatPump3Stage    , 428.400   , 0.0)       ,
	TestCase(CFG7  , 6  , HeatPumpCont       , HeatPump3Stage    , 856.80    , 0.0)       ,
	TestCase(CFG7  , 8  , HeatPumpCont       , HeatPump3Stage    , 883.4159  , 547.9913)  ,
	TestCase(CFG7  , 11 , HeatPumpCont       , HeatPump3Stage    , 1116.3204 , 2644.1315) ,

	TestCase(CFG7  , 1  , HeatPump2Stage    , HeatPumpCont       , 428.400   , 0.0)       ,
	TestCase(CFG7  , 6  , HeatPump2Stage    , HeatPumpCont       , 856.80    , 0.0)       ,
	TestCase(CFG7  , 8  , HeatPump2Stage    , HeatPumpCont       , 1360.3368 , 59.7565)   ,
	TestCase(CFG7  , 11 , HeatPump2Stage    , HeatPumpCont       , 3393.4694 , 285.6602)  ,

	TestCase(CFG9  , 1  , HeatPumpCont      , HeatPump3Stage    , 428.400   , 0.0)       ,
	TestCase(CFG9  , 6  , HeatPumpCont      , HeatPump3Stage    , 856.80    , 0.0)       ,
	TestCase(CFG9  , 8  , HeatPumpCont      , HeatPump3Stage    , 883.4159  , 547.9913)  ,
	TestCase(CFG9  , 11 , HeatPumpCont      , HeatPump3Stage    , 1116.3204 , 2644.1315) ,

	TestCase(CFG9  , 1  , HeatPump2Stage    , HeatPumpCont , 428.400   , 0.0)       ,
	TestCase(CFG9  , 6  , HeatPump2Stage    , HeatPumpCont , 856.80    , 0.0)       ,
	TestCase(CFG9  , 8  , HeatPump2Stage    , HeatPumpCont , 1360.3368 , 59.7565)   ,
	TestCase(CFG9  , 11 , HeatPump2Stage    , HeatPumpCont , 3393.4694 , 285.6602)  ,
		Category(Definitions.TESTCASE_MIGRATED)
    ]
	public void SSMTest_Cooling_SingleEnvironment(BusHVACSystemConfiguration cfg, int envId, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump,  double expectedElPwrW, double expectedMechPwrW)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatpumpCooling: driverHeatpump, passengerHeatpumpCooling: passengerHeatpump);

		// set list of environmental conditions to a single entry
		var ssmInputs = auxData.SSMInputsCooling as SSMInputs;
		Assert.NotNull(ssmInputs);
		var envAll = ssmInputs.EnvironmentalConditions;
		var selectedEnv = envAll.EnvironmentalConditionsMap.GetEnvironmentalConditions().Where(x => x.ID == envId).FirstOrDefault();
		Assert.NotNull(selectedEnv);
		var newEnv = new EnvironmentalConditionMapEntry(selectedEnv.ID, selectedEnv.Temperature, selectedEnv.Solar, 1.0,
			selectedEnv.HeatPumpCoP.ToDictionary(x => x.Key, x => x.Value), 
			selectedEnv.HeaterEfficiency.ToDictionary(x => x.Key, x => x.Value));
		ssmInputs.EnvironmentalConditionsMap = new EnvironmentalConditionsMap(new[] { newEnv });
		// ---

		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsCooling);

		Console.WriteLine($"{ssm.ElectricalWAdjusted.Value().ToGUIFormat(4)}, {ssm.MechanicalWBaseAdjusted.Value().ToGUIFormat(4)}");
		
		Assert.AreEqual(expectedElPwrW, ssm.ElectricalWAdjusted.Value(), 1e-3, "expected Electric Power");
		Assert.AreEqual(expectedMechPwrW, ssm.MechanicalWBaseAdjusted.Value(), 1e-3, "expected Mechanical Power");

	}


	[
		// only mechanical heatpumps
		// electric: only ventilation, mechanic: heatpump
		TestCase(CFG1  , HeatPumpNone     , HeatPumpNone   , 295.3949 , 0.0)       ,
		TestCase(CFG2  , HeatPump2Stage   , HeatPumpNone   , 295.3949 , 109.29748) ,
		TestCase(CFG3  , HeatPumpNone     , HeatPumpNone   , 664.4004 , 0.0)       ,
		TestCase(CFG4  , HeatPump2Stage   , HeatPumpNone   , 575.7645 , 109.2975)  ,
		TestCase(CFG5  , HeatPumpNone     , HeatPump3Stage , 664.4004 , 429.8231)  ,
		TestCase(CFG6  , HeatPumpNone     , HeatPump3Stage , 664.4004 , 429.8231)  ,
		TestCase(CFG7  , HeatPump2Stage   , HeatPump3Stage , 664.4004 , 432.3237)  ,
		TestCase(CFG8  , HeatPumpNone     , HeatPump3Stage , 664.4004 , 429.8231)  ,
		TestCase(CFG9  , HeatPump2Stage   , HeatPump3Stage , 664.4004 , 432.3237)  ,
		TestCase(CFG10 , HeatPumpNone     , HeatPump3Stage , 664.4004 , 429.8231)  ,

		// only electrical heatpumps
		// electric: ventilation + heatpump
		TestCase(CFG2, HeatPumpCont, HeatPumpNone, 395.9945, 0.0),
		TestCase(CFG4, HeatPumpCont, HeatPumpNone, 676.364, 0),
		TestCase(CFG2, HeatPumpR744, HeatPumpNone, 418.4411, 0),
		TestCase(CFG4, HeatPumpR744, HeatPumpNone, 698.8106, 0),

		TestCase(CFG5  , HeatPumpNone  , HeatPumpCont , 1076.1036 , 0) ,
		TestCase(CFG5  , HeatPumpNone  , HeatPumpR744 , 1136.6973, 0) ,
		TestCase(CFG6  , HeatPumpNone  , HeatPumpCont , 1076.1036 , 0) ,
		TestCase(CFG6  , HeatPumpNone  , HeatPumpR744 , 1136.6973, 0) ,
		TestCase(CFG7  , HeatPumpCont  , HeatPumpCont , 1076.1036 , 0) ,
		TestCase(CFG7  , HeatPumpR744  , HeatPumpCont , 1084.8656, 0) ,
		TestCase(CFG7  , HeatPumpCont  , HeatPumpR744 , 1124.3766, 0) ,
		TestCase(CFG7  , HeatPumpR744  , HeatPumpR744 , 1136.6973, 0) ,
		TestCase(CFG8  , HeatPumpNone  , HeatPumpCont , 1076.1036 , 0) ,
		TestCase(CFG8  , HeatPumpNone  , HeatPumpR744 , 1136.6973, 0) ,
		TestCase(CFG9  , HeatPumpCont  , HeatPumpCont, 1076.1036 , 0) ,
		TestCase(CFG9  , HeatPumpR744  , HeatPumpCont, 1084.8656, 0) ,
		TestCase(CFG9  , HeatPumpCont  , HeatPumpR744, 1124.3766, 0) ,
		TestCase(CFG9  , HeatPumpR744  , HeatPumpR744, 1136.6973, 0) ,
		TestCase(CFG10 , HeatPumpNone  , HeatPumpCont, 1076.1036 , 0) ,
		TestCase(CFG10 , HeatPumpNone  , HeatPumpR744, 1136.6973 , 0) ,

		// mixed electrical and mechanical heatpumps
		TestCase(CFG7 , HeatPumpCont   , HeatPump3Stage , 707.0598  , 383.9348) ,
		TestCase(CFG7 , HeatPump2Stage , HeatPumpCont   , 1039.7276 , 41.703)   ,
		TestCase(CFG7 , HeatPumpR744   , HeatPump3Stage , 708.0087, 392.4745) ,
		TestCase(CFG7 , HeatPump2Stage , HeatPumpR744   , 1084.5117, 46.679)   ,
		TestCase(CFG9 , HeatPumpCont   , HeatPump3Stage , 707.0598  , 383.9348) ,
		TestCase(CFG9 , HeatPump2Stage , HeatPumpCont   , 1039.7276 , 41.703)   ,
		TestCase(CFG9 , HeatPumpR744   , HeatPump3Stage , 708.0087, 392.4745) ,
		TestCase(CFG9 , HeatPump2Stage , HeatPumpR744   , 1084.5117, 46.679)   ,
		Category(Definitions.TESTCASE_MIGRATED)
	]
	public void SSMTest_Cooling_AvgAllEnvironments(BusHVACSystemConfiguration cfg, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, double expectedElPwrW, double expectedMechPwrW)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatpumpCooling: driverHeatpump, passengerHeatpumpCooling: passengerHeatpump);

		var ssmInputs = auxData.SSMInputsCooling as SSMInputs;
		Assert.NotNull(ssmInputs);
		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsCooling);

		Console.WriteLine($"{ssm.ElectricalWAdjusted.Value().ToGUIFormat(4)}, {ssm.MechanicalWBaseAdjusted.Value().ToGUIFormat(4)}");

		Assert.AreEqual(expectedElPwrW, ssm.ElectricalWAdjusted.Value(), 1e-3, "expected electrical power");
		Assert.AreEqual(expectedMechPwrW, ssm.MechanicalWBaseAdjusted.Value(), 1e-3, "expected mechanical power");

	}

	

	[
		// check that the heating demand is equal for all configurations
		TestCase(CFG1, 1,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG1, 2,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG1, 3,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG1, 4,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG1, 5,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG1, 6,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),
		TestCase(CFG1, 7,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),
		TestCase(CFG1, 8,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),
		TestCase(CFG1, 9,  HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),
		TestCase(CFG1, 10, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),
		TestCase(CFG1, 11, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG2, 1, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG2, 2, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG2, 3, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG2, 4, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG2, 5, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG2, 6, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG3, 1, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG3, 2, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG3, 3, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG3, 4, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG3, 5, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG3, 6, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG4, 1, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG4, 2, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG4, 3, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG4, 4, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG4, 5, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG4, 6, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG5, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG5, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG5, 3, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG5, 4, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG5, 5, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG5, 6, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG6, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG6, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG6, 3, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG6, 4, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG6, 5, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG6, 6, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG7, 1, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG7, 2, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG7, 3, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG7, 4, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG7, 5, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG7, 6, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG8, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG8, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG8, 3, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG8, 4, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG8, 5, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG8, 6, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG9, 1, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG9, 2, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG9, 3, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG9, 4, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG9, 5, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG9, 6, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 0),

		TestCase(CFG10, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 13475.4938),
		TestCase(CFG10, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 7261.9812),
		TestCase(CFG10, 3, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 4395.4812),
		TestCase(CFG10, 4, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1973.9875),
		TestCase(CFG10, 5, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 1494.6531),
		TestCase(CFG10, 6, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, 0),
		Category(Definitions.TESTCASE_MIGRATED)
	]
	public void SSMTest_HeatingDemand_SingleEnvironment(BusHVACSystemConfiguration cfg, int envId, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr, double expectedHeatingPower)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatPumpHeating: driverHeatpump, passengerHeatPumpHeating: passengerHeatpump,
			electricHeater: electricHeater, auxHeaterPower: auxHeaterPwr.SI<Watt>());

		// set list of environmental conditions to a single entry
		var ssmInputs = auxData.SSMInputsHeating as SSMInputs;
		Assert.NotNull(ssmInputs);
		var envAll = ssmInputs.EnvironmentalConditions;
		var selectedEnv = envAll.EnvironmentalConditionsMap.GetEnvironmentalConditions().Where(x => x.ID == envId).FirstOrDefault();
		Assert.NotNull(selectedEnv);
		var newEnv = new EnvironmentalConditionMapEntry(selectedEnv.ID, selectedEnv.Temperature, selectedEnv.Solar, 1.0,
			selectedEnv.HeatPumpCoP.ToDictionary(x => x.Key, x => x.Value),
			selectedEnv.HeaterEfficiency.ToDictionary(x => x.Key, x => x.Value));
		ssmInputs.EnvironmentalConditionsMap = new EnvironmentalConditionsMap(new[] { newEnv });
		// ---

		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsHeating);

		var heatingPower = ssm.AverageHeaterPower(0.SI<Watt>());

		Console.WriteLine($"{heatingPower.RequiredHeatingPower.Value().ToGUIFormat(4)}, {heatingPower.HeatPumpPowerEl.Value().ToGUIFormat(4)}, {heatingPower.HeatPumpPowerMech.Value().ToGUIFormat(4)}, {heatingPower.ElectricHeaterPowerEl.Value().ToGUIFormat(4)}, {heatingPower.AuxHeaterPower.Value().ToGUIFormat(4)}");

		Assert.AreEqual(expectedHeatingPower, heatingPower.RequiredHeatingPower.Value(), 1e-3, "expected Heating Demand");
	}

	[
		TestCase("1a", CFG1, 1, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC12, HDC12, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG1, 1, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr0, HDC9, HDC9, 13475.4938, 0, 0, 14489.7782, 0),
		TestCase("1c", CFG1, 1, HeatPumpNone, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC9, HDC9, 13475.4938, 0, 0, 14489.7782, 0),
		TestCase("1d", CFG1, 1, HeatPumpNone, HeatPumpNone, OthrElHtr | AirElHtr, AuxHtrPwr0, HDC9, HDC9, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1e", CFG1, 1, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC11, HDC11, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1f", CFG1, 1, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC10, HDC10, 13475.4938, 0, 0, 2897.9556, 13475.4938),
		TestCase("1g", CFG1, 1, HeatPumpNone, HeatPumpNone, WaterElHtr, AuxHtrPwr30, HDC10, HDC10, 13475.4938, 0, 0, 2897.9556, 13475.4938),
		TestCase("1h", CFG1, 1, HeatPumpNone, HeatPumpNone, OthrElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC10, 13475.4938, 0, 0, 2897.9556, 13475.4938),
				// ---
		TestCase("2a", CFG1, 2, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC12, HDC12, 7261.9812, 0, 0, 0, 0),
		TestCase("2b", CFG1, 2, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr0, HDC9, HDC9, 7261.9812, 0, 0, 7808.582, 0),
		TestCase("2c", CFG1, 2, HeatPumpNone, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC9, HDC9, 7261.9812, 0, 0, 7808.582, 0),
		TestCase("2d", CFG1, 2, HeatPumpNone, HeatPumpNone, OthrElHtr | AirElHtr, AuxHtrPwr0, HDC9, HDC9, 7261.9812, 0, 0, 7808.582, 0),

		TestCase("2e", CFG1, 2, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC11, HDC11, 7261.9812, 0, 0, 0, 9077.4766),
		TestCase("2f", CFG1, 2, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC10, HDC10, 7261.9812, 0, 0, 0, 9077.4766),
		TestCase("2g", CFG1, 2, HeatPumpNone, HeatPumpNone, WaterElHtr, AuxHtrPwr30, HDC10, HDC10, 7261.9812, 0, 0, 0, 9077.4766),
		TestCase("2h", CFG1, 2, HeatPumpNone, HeatPumpNone, OthrElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC10, 7261.9812, 0, 0, 0, 9077.4766),
		Category(Definitions.TESTCASE_MIGRATED)

    ]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG1(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	// =========================

	[
		TestCase("1a", CFG2, 1, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG2, 1, HeatPump2Stage, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC6, HDC9, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG2, 1, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1d", CFG2, 1, HeatPump2Stage, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 13475.4938, 0, 0, 2897.9556, 13475.4938),

        TestCase("1e", CFG2, 1, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 13475.4938, 0, 0, 0, 0),
        TestCase("1f", CFG2, 1, HeatPumpCont, HeatPumpNone, OthrElHtr, AuxHtrPwr0, HDC6, HDC9, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1g", CFG2, 1, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 13475.4938, 0, 0, 0, 16844.3672),
        TestCase("1h", CFG2, 1, HeatPumpCont, HeatPumpNone, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC7, HDC10, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1i", CFG2, 1, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC1, HDC12, 13475.4938, 748.6385, 0, 0, 0),
		TestCase("1j", CFG2, 1, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC2, HDC9, 13475.4938, 374.3193, 0, 13765.2893, 0),

		TestCase("1k", CFG2, 1, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC4, HDC11, 13475.4938, 299.4554, 0, 0, 16170.5925),
		TestCase("1l", CFG2, 1, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr30, HDC3, HDC10, 13475.4938, 299.4554, 0, 2897.9556, 12801.7191),
		// ---
		TestCase("2a", CFG2, 2, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 7261.9812, 0, 471.5572, 0, 0),
		TestCase("2b", CFG2, 2, HeatPump2Stage, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC6, HDC9, 7261.9812, 0, 471.5572, 7027.7238, 0),

		TestCase("2c", CFG2, 2, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 7261.9812, 0, 330.0901, 0, 8442.0532),
		TestCase("2d", CFG2, 2, HeatPump2Stage, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 7261.9812, 0, 330.0901, 0, 8442.0532),

		TestCase("2e", CFG2, 2, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 7261.9812, 407.9765, 0, 0, 0),
		TestCase("2f", CFG2, 2, HeatPumpCont, HeatPumpNone, OthrElHtr, AuxHtrPwr0, HDC6, HDC9, 7261.9812, 407.9765, 0, 7027.7238, 0),

		TestCase("2g", CFG2, 2, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 7261.9812, 285.5835, 0, 0, 8442.0532),
		TestCase("2h", CFG2, 2, HeatPumpCont, HeatPumpNone, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC7, HDC10, 7261.9812, 285.5835, 0, 0, 8442.0532),

		TestCase("2i", CFG2, 2, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC1, HDC12, 7261.9812, 355.9795, 0, 0, 0),
		TestCase("2j", CFG2, 2, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC2, HDC9, 7261.9812, 355.9795, 0, 7027.7238, 0),

		TestCase("2k", CFG2, 2, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC4, HDC11, 7261.9812, 249.1856, 0, 0, 8442.0532),
		TestCase("2l", CFG2, 2, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr30, HDC3, HDC10, 7261.9812, 249.1856, 0, 0, 8442.0532),
				//---
		TestCase("3a", CFG2, 3, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 4395.4812, 0, 219.7741, 0, 0),
		TestCase("3b", CFG2, 3, HeatPump2Stage, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC6, HDC9, 4395.4812, 0, 219.7741, 4253.6915, 0),

		TestCase("3c", CFG2, 3, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 4395.4812, 0, 175.8192, 0, 5054.8034),
		TestCase("3d", CFG2, 3, HeatPump2Stage, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 4395.4812, 0, 175.8192, 0, 5054.8034),

		TestCase("3e", CFG2, 3, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 4395.4812, 197.9947, 0, 0, 0),
		TestCase("3f", CFG2, 3, HeatPumpCont, HeatPumpNone, OthrElHtr, AuxHtrPwr0, HDC6, HDC9, 4395.4812, 197.9947, 0, 4253.6915, 0),

		TestCase("3g", CFG2, 3, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 4395.4812, 158.3957, 0, 0, 5054.8034),
		TestCase("3h", CFG2, 3, HeatPumpCont, HeatPumpNone, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC7, HDC10, 4395.4812, 158.3957, 0, 0, 5054.8034),

		TestCase("3i", CFG2, 3, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC1, HDC12, 4395.4812, 175.8192, 0, 0, 0),
		TestCase("3j", CFG2, 3, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC2, HDC9, 4395.4812, 175.8192, 0, 4253.6915, 0),

		TestCase("3k", CFG2, 3, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC4, HDC11, 4395.4812, 140.6554, 0, 0, 5054.8034),
		TestCase("3l", CFG2, 3, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr30, HDC3, HDC10, 4395.4812, 140.6554, 0, 0, 5054.8034),
		Category(Definitions.TESTCASE_MIGRATED)

    ]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG2(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

    // =========================

    [
		TestCase("1a", CFG3, 1, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC12, HDC12, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG3, 1, HeatPumpNone, HeatPumpNone, WaterElHtr | OthrElHtr, AuxHtrPwr0, HDC9, HDC9, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG3, 1, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC11, HDC11, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1d", CFG3, 1, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC10, HDC10, 13475.4938, 0, 0, 2897.9556, 13475.4938),
				// ---
		TestCase("2a", CFG3, 2, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC12, HDC12, 7261.9812, 0, 0, 0, 0),
		TestCase("2b", CFG3, 2, HeatPumpNone, HeatPumpNone, WaterElHtr | OthrElHtr, AuxHtrPwr0, HDC9, HDC9, 7261.9812, 0, 0, 7808.582, 0),

		TestCase("2c", CFG3, 2, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC11, HDC11, 7261.9812, 0, 0, 0, 9077.4766),
		TestCase("2d", CFG3, 2, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC10, HDC10, 7261.9812, 0, 0, 0, 9077.4766),
		Category(Definitions.TESTCASE_MIGRATED)

    ]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG3(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	// =========================

	[
		TestCase("1a", CFG4, 1, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG4, 1, HeatPump2Stage, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC6, HDC9, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG4, 1, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1d", CFG4, 1, HeatPump2Stage, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1e", CFG4, 1, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 13475.4938, 0, 0, 0, 0),
		TestCase("1f", CFG4, 1, HeatPumpCont, HeatPumpNone, AirElHtr | OthrElHtr, AuxHtrPwr0, HDC6, HDC9, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1g", CFG4, 1, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1h", CFG4, 1, HeatPumpCont, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1i", CFG4, 1, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC1, HDC12, 13475.4938, 748.6385, 0, 0, 0),
		TestCase("1j", CFG4, 1, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC2, HDC9, 13475.4938, 374.3193, 0, 13765.2893, 0),

		TestCase("1k", CFG4, 1, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC4, HDC11, 13475.4938, 299.4554, 0, 0, 16170.5925),
		TestCase("1l", CFG4, 1, HeatPumpR744, HeatPumpNone, OthrElHtr, AuxHtrPwr30, HDC3, HDC10, 13475.4938, 299.4554, 0, 2897.9556, 12801.7191),
		// ---
		TestCase("2a", CFG4, 2, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 7261.9812, 0, 471.5572, 0, 0),
		TestCase("2b", CFG4, 2, HeatPump2Stage, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC6, HDC9, 7261.9812, 0, 471.5572, 7027.7238, 0),

		TestCase("2c", CFG4, 2, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 7261.9812, 0, 330.0901, 0, 8442.0532),
		TestCase("2d", CFG4, 2, HeatPump2Stage, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 7261.9812, 0, 330.0901, 0, 8442.0532),

		TestCase("2e", CFG4, 2, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 7261.9812, 407.9765, 0, 0, 0),
		TestCase("2f", CFG4, 2, HeatPumpCont, HeatPumpNone, AirElHtr | OthrElHtr, AuxHtrPwr0, HDC6, HDC9, 7261.9812, 407.9765, 0, 7027.7238, 0),

		TestCase("2g", CFG4, 2, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 7261.9812, 285.5835, 0, 0, 8442.0532),
		TestCase("2h", CFG4, 2, HeatPumpCont, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 7261.9812, 285.5835, 0, 0, 8442.0532),

		TestCase("2i", CFG4, 2, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC1, HDC12, 7261.9812, 355.9795, 0, 0, 0),
		TestCase("2j", CFG4, 2, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC2, HDC9, 7261.9812, 355.9795, 0, 7027.7238, 0),

		TestCase("2k", CFG4, 2, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC4, HDC11, 7261.9812, 249.1856, 0, 0, 8442.0532),
		TestCase("2l", CFG4, 2, HeatPumpR744, HeatPumpNone, OthrElHtr, AuxHtrPwr30, HDC3, HDC10, 7261.9812, 249.1856, 0, 0, 8442.0532),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG4(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr, 
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	// ==================================================
	[
		TestCase("1a", CFG5, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG5, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG5, 1, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1d", CFG5, 1, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1e", CFG5, 1, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 13475.4938, 7486.3854, 0, 0, 0),
		TestCase("1f", CFG5, 1, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 13475.4938, 3743.1927, 0, 7244.8891, 0),

		TestCase("1g", CFG5, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1h", CFG5, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1i", CFG5, 1, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1j", CFG5, 1, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1k", CFG5, 1, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 13475.4938, 2994.5542, 0, 0, 10106.6203),
		TestCase("1l", CFG5, 1, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 13475.4938, 2994.5542, 0, 2897.9556, 6737.7469),
		// ---
		TestCase("2a", CFG5, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 7261.9812, 0, 4428.0373, 0, 0),
		TestCase("2b", CFG5, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 7261.9812, 0, 4428.0373, 0, 0),

		TestCase("2c", CFG5, 2, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 7261.9812, 4079.7647, 0, 0, 0),
		TestCase("2d", CFG5, 2, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 7261.9812, 4079.7647, 0, 0, 0),

		TestCase("2e", CFG5, 2, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 7261.9812, 3559.7947, 0, 0, 0),
		TestCase("2f", CFG5, 2, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 7261.9812, 3559.7947, 0, 0, 0),

		TestCase("2g", CFG5, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 7261.9812, 0, 3099.6261, 0, 2723.243),
		TestCase("2h", CFG5, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 7261.9812, 0, 3099.6261, 0, 2723.243),

		TestCase("2i", CFG5, 2, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 7261.9812, 2855.8353, 0, 0, 2723.243),
		TestCase("2j", CFG5, 2, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 7261.9812, 2855.8353, 0, 0, 2723.243),

		TestCase("2k", CFG5, 2, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 7261.9812, 2491.8563, 0, 0, 2723.243),
		TestCase("2l", CFG5, 2, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 7261.9812, 2491.8563, 0, 0, 2723.243),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG5(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	// =========================

	[
		TestCase("1a", CFG6, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG6, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG6, 1, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1d", CFG6, 1, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1e", CFG6, 1, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 13475.4938, 7486.3854, 0, 0, 0),
		TestCase("1f", CFG6, 1, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 13475.4938, 3743.1927, 0, 7244.8891, 0),

		TestCase("1g", CFG6, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1h", CFG6, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1i", CFG6, 1, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1j", CFG6, 1, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1k", CFG6, 1, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 13475.4938, 2994.5542, 0, 0, 10106.6203),
		TestCase("1l", CFG6, 1, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 13475.4938, 2994.5542, 0, 2897.9556, 6737.7469),
		// ---
		TestCase("2a", CFG6, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 7261.9812, 0, 4428.0373, 0, 0),
		TestCase("2b", CFG6, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 7261.9812, 0, 4428.0373, 0, 0),

		TestCase("2c", CFG6, 2, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 7261.9812, 4079.7647, 0, 0, 0),
		TestCase("2d", CFG6, 2, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 7261.9812, 4079.7647, 0, 0, 0),

		TestCase("2e", CFG6, 2, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 7261.9812, 3559.7947, 0, 0, 0),
		TestCase("2f", CFG6, 2, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 7261.9812, 3559.7947, 0, 0, 0),

		TestCase("2g", CFG6, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 7261.9812, 0, 3099.6261, 0, 2723.243),
		TestCase("2h", CFG6, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 7261.9812, 0, 3099.6261, 0, 2723.243),

		TestCase("2i", CFG6, 2, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 7261.9812, 2855.8353, 0, 0, 2723.243),
		TestCase("2j", CFG6, 2, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 7261.9812, 2855.8353, 0, 0, 2723.243),

		TestCase("2k", CFG6, 2, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 7261.9812, 2491.8563, 0, 0, 2723.243),
		TestCase("2l", CFG6, 2, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 7261.9812, 2491.8563, 0, 0, 2723.243),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG6(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	// =========================

	[
		TestCase("1a", CFG7, 1, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG7, 1, HeatPump2Stage, HeatPump3Stage, AirElHtr, AuxHtrPwr0, HDC6, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG7, 1, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1d", CFG7, 1, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC6, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1e", CFG7, 1, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1f", CFG7, 1, HeatPump2Stage, HeatPumpCont, OthrElHtr, AuxHtrPwr0, HDC6, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1g", CFG7, 1, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC1, HDC5, 13475.4938, 748.6385, 0, 0, 0),
		TestCase("1h", CFG7, 1, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC2, HDC6, 13475.4938, 374.3193, 0, 13765.2893, 0),

		TestCase("1i", CFG7, 1, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC5, HDC1, 13475.4938, 6737.7469, 0, 0, 0),
		TestCase("1j", CFG7, 1, HeatPump2Stage, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC6, HDC2, 13475.4938, 3368.8734, 0, 7969.378, 0),

		TestCase("1k", CFG7, 1, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC1, HDC1, 13475.4938, 7486.3854, 0, 0, 0),
		TestCase("1l", CFG7, 1, HeatPumpR744, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC2, HDC2, 13475.4938, 3743.1927, 0, 7244.8891, 0),

		TestCase("1m", CFG7, 1, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1n", CFG7, 1, HeatPump2Stage, HeatPump3Stage, AirElHtr, AuxHtrPwr30, HDC7, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1o", CFG7, 1, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1p", CFG7, 1, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC7, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1q", CFG7, 1, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1r", CFG7, 1, HeatPump2Stage, HeatPumpCont, OthrElHtr, AuxHtrPwr30, HDC7, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1s", CFG7, 1, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC4, HDC8, 13475.4938, 299.4554, 0, 0, 16170.5925),
		TestCase("1t", CFG7, 1, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC3, HDC7, 13475.4938, 299.4554, 0, 2897.9556, 12801.7191),

		TestCase("1u", CFG7, 1, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC8, HDC4, 13475.4938, 2695.0987, 0, 0, 10780.395),
		TestCase("1v", CFG7, 1, HeatPump2Stage, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC7, HDC3, 13475.4938, 2695.0988, 0, 2897.9556, 7411.5216),

		TestCase("1w", CFG7, 1, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC4, HDC4, 13475.4938, 2994.5542, 0, 0, 10106.6203),
		TestCase("1x", CFG7, 1, HeatPumpR744, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC3, HDC3, 13475.4938, 2994.5542, 0, 2897.9556, 6737.7469),
		// ---
		TestCase("2a", CFG7, 2, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 7261.9812, 0, 4456.7908, 0, 0),
		TestCase("2b", CFG7, 2, HeatPump2Stage, HeatPump3Stage, AirElHtr, AuxHtrPwr0, HDC6, HDC6, 7261.9812, 0, 4456.7908, 0, 0),

		TestCase("2c", CFG7, 2, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 7261.9812, 407.9765, 3985.2336, 0, 0),
		TestCase("2d", CFG7, 2, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC6, HDC6, 7261.9812, 407.9765, 3985.2336, 0, 0),

		TestCase("2e", CFG7, 2, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 7261.9812, 3671.7883, 471.5572, 0, 0),
		TestCase("2f", CFG7, 2, HeatPump2Stage, HeatPumpCont, OthrElHtr, AuxHtrPwr0, HDC6, HDC6, 7261.9812, 3671.7883, 471.5572, 0, 0),

		TestCase("2g", CFG7, 2, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC1, HDC5, 7261.9812, 355.9795, 3985.2336, 0, 0),
		TestCase("2h", CFG7, 2, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC2, HDC6, 7261.9812, 355.9795, 3985.2336, 0, 0),

		TestCase("2i", CFG7, 2, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC5, HDC1, 7261.9812, 3203.8153, 471.5572, 0, 0),
		TestCase("2j", CFG7, 2, HeatPump2Stage, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC6, HDC2, 7261.9812, 3203.8153, 471.5572, 0, 0),

		TestCase("2k", CFG7, 2, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC1, HDC1, 7261.9812, 3559.7947, 0, 0, 0),
		TestCase("2l", CFG7, 2, HeatPumpR744, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC2, HDC2, 7261.9812, 3559.7947, 0, 0, 0),

		TestCase("2m", CFG7, 2, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 7261.9812, 0, 3119.7536, 0, 2723.243),
		TestCase("2n", CFG7, 2, HeatPump2Stage, HeatPump3Stage, AirElHtr, AuxHtrPwr30, HDC7, HDC7, 7261.9812, 0, 3119.7536, 0, 2723.243),

		TestCase("2o", CFG7, 2, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 7261.9812, 285.5835, 2789.6635, 0, 2723.243),
		TestCase("2p", CFG7, 2, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC7, HDC7, 7261.9812, 285.5835, 2789.6635, 0, 2723.243),

		TestCase("2q", CFG7, 2, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 7261.9812, 2570.2518, 330.0901, 0, 2723.243),
		TestCase("2r", CFG7, 2, HeatPump2Stage, HeatPumpCont, OthrElHtr, AuxHtrPwr30, HDC7, HDC7, 7261.9812, 2570.2518, 330.0901, 0, 2723.243),

		TestCase("2s", CFG7, 2, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC4, HDC8, 7261.9812, 249.1856, 2789.6635, 0, 2723.243),
		TestCase("2t", CFG7, 2, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC3, HDC7, 7261.9812, 249.1856, 2789.6635, 0, 2723.243),

		TestCase("2u", CFG7, 2, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC8, HDC4, 7261.9812, 2242.6707, 330.0901, 0, 2723.243),
		TestCase("2v", CFG7, 2, HeatPump2Stage, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC7, HDC3, 7261.9812, 2242.6707, 330.0901, 0, 2723.243),

		TestCase("2w", CFG7, 2, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC4, HDC4, 7261.9812, 2491.8563, 0, 0, 2723.243),
		TestCase("2x", CFG7, 2, HeatPumpR744, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC3, HDC3, 7261.9812, 2491.8563, 0, 0, 2723.243),

		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG7(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	// =========================

	[
		TestCase("1a", CFG8, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG8, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG8, 1, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1d", CFG8, 1, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1e", CFG8, 1, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 13475.4938, 7486.3854, 0, 0, 0),
		TestCase("1f", CFG8, 1, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 13475.4938, 3743.1927, 0, 7244.8891, 0),

		TestCase("1g", CFG8, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1h", CFG8, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1i", CFG8, 1, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1j", CFG8, 1, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1k", CFG8, 1, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 13475.4938, 2994.5542, 0, 0, 10106.6203),
		TestCase("1l", CFG8, 1, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 13475.4938, 2994.5542, 0, 2897.9556, 6737.7469),
		// ---
		TestCase("2a", CFG8, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 7261.9812, 0, 4428.0373, 0, 0),
		TestCase("2b", CFG8, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 7261.9812, 0, 4428.0373, 0, 0),

		TestCase("2c", CFG8, 2, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 7261.9812, 4079.7647, 0, 0, 0),
		TestCase("2d", CFG8, 2, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 7261.9812, 4079.7647, 0, 0, 0),

		TestCase("2e", CFG8, 2, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 7261.9812, 3559.7947, 0, 0, 0),
		TestCase("2f", CFG8, 2, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 7261.9812, 3559.7947, 0, 0, 0),

		TestCase("2g", CFG8, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 7261.9812, 0, 3099.6261, 0, 2723.243),
		TestCase("2h", CFG8, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 7261.9812, 0, 3099.6261, 0, 2723.243),

		TestCase("2i", CFG8, 2, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 7261.9812, 2855.8353, 0, 0, 2723.243),
		TestCase("2j", CFG8, 2, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 7261.9812, 2855.8353, 0, 0, 2723.243),

		TestCase("2k", CFG8, 2, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 7261.9812, 2491.8563, 0, 0, 2723.243),
		TestCase("2l", CFG8, 2, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 7261.9812, 2491.8563, 0, 0, 2723.243),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG8(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}
	// =========================

	[
		TestCase("1a", CFG9, 1, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG9, 1, HeatPump2Stage, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC6, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG9, 1, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1d", CFG9, 1, HeatPump2Stage, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC6, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1e", CFG9, 1, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1f", CFG9, 1, HeatPumpCont, HeatPump3Stage, OthrElHtr, AuxHtrPwr0, HDC6, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1g", CFG9, 1, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC1, HDC5, 13475.4938, 748.6385, 0, 0, 0),
		TestCase("1h", CFG9, 1, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC2, HDC6, 13475.4938, 374.3193, 0, 13765.2893, 0),

		TestCase("1i", CFG9, 1, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC5, HDC1, 13475.4938, 6737.7469, 0, 0, 0),
		TestCase("1j", CFG9, 1, HeatPump2Stage, HeatPumpR744, WaterElHtr | OthrElHtr, AuxHtrPwr0, HDC6, HDC2, 13475.4938, 3368.8734, 0, 7969.378, 0),

		TestCase("1k", CFG9, 1, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC1, HDC1, 13475.4938, 7486.3854, 0, 0, 0),
		TestCase("1l", CFG9, 1, HeatPumpR744, HeatPumpR744, WaterElHtr, AuxHtrPwr0, HDC2, HDC2, 13475.4938, 3743.1927, 0, 7244.8891, 0),

		TestCase("1m", CFG9, 1, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1n", CFG9, 1, HeatPump2Stage, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC7, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1o", CFG9, 1, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1p", CFG9, 1, HeatPump2Stage, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC7, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1q", CFG9, 1, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1r", CFG9, 1, HeatPumpCont, HeatPump3Stage, OthrElHtr, AuxHtrPwr30, HDC7, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1s", CFG9, 1, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC4, HDC8, 13475.4938, 299.4554, 0, 0, 16170.5925),
		TestCase("1t", CFG9, 1, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC3, HDC7, 13475.4938, 299.4554, 0, 2897.9556, 12801.7191),

		TestCase("1u", CFG9, 1, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC8, HDC4, 13475.4938, 2695.0987, 0, 0, 10780.395),
		TestCase("1v", CFG9, 1, HeatPump2Stage, HeatPumpR744, WaterElHtr | OthrElHtr, AuxHtrPwr30, HDC7, HDC3, 13475.4938, 2695.0988, 0, 2897.9556, 7411.5216),

		TestCase("1w", CFG9, 1, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC4, HDC4, 13475.4938, 2994.5542, 0, 0, 10106.6203),
		TestCase("1x", CFG9, 1, HeatPumpR744, HeatPumpR744, WaterElHtr, AuxHtrPwr30, HDC3, HDC3, 13475.4938, 2994.5542, 0, 2897.9556, 6737.7469),
		// ---
		TestCase("2a", CFG9, 2, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 7261.9812, 0, 4456.7908, 0, 0),
		TestCase("2b", CFG9, 2, HeatPump2Stage, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC6, HDC6, 7261.9812, 0, 4456.7908, 0, 0),

		TestCase("2c", CFG9, 2, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 7261.9812, 3671.7883, 471.5572, 0, 0),
		TestCase("2d", CFG9, 2, HeatPump2Stage, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC6, HDC6, 7261.9812, 3671.7883, 471.5572, 0, 0),

		TestCase("2e", CFG9, 2, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 7261.9812, 407.9765, 3985.2336, 0, 0),
		TestCase("2f", CFG9, 2, HeatPumpCont, HeatPump3Stage, OthrElHtr, AuxHtrPwr0, HDC6, HDC6, 7261.9812, 407.9765, 3985.2336, 0, 0),

		TestCase("2g", CFG9, 2, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC1, HDC5, 7261.9812, 355.9795, 3985.2336, 0, 0),
		TestCase("2h", CFG9, 2, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC2, HDC6, 7261.9812, 355.9795, 3985.2336, 0, 0),

		TestCase("2i", CFG9, 2, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC5, HDC1, 7261.9812, 3203.8153, 471.5572, 0, 0),
		TestCase("2j", CFG9, 2, HeatPump2Stage, HeatPumpR744, WaterElHtr | OthrElHtr, AuxHtrPwr0, HDC6, HDC2, 7261.9812, 3203.8153, 471.5572, 0, 0),

		TestCase("2k", CFG9, 2, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC1, HDC1, 7261.9812, 3559.7947, 0, 0, 0),
		TestCase("2l", CFG9, 2, HeatPumpR744, HeatPumpR744, WaterElHtr, AuxHtrPwr0, HDC2, HDC2, 7261.9812, 3559.7947, 0, 0, 0),

		TestCase("2m", CFG9, 2, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 7261.9812, 0, 3119.7536, 0, 2723.243),
		TestCase("2n", CFG9, 2, HeatPump2Stage, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC7, HDC7, 7261.9812, 0, 3119.7536, 0, 2723.243),

		TestCase("2o", CFG9, 2, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 7261.9812, 2570.2518, 330.0901, 0, 2723.243),
		TestCase("2p", CFG9, 2, HeatPump2Stage, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC7, HDC7, 7261.9812, 2570.2518, 330.0901, 0, 2723.243),

		TestCase("2q", CFG9, 2, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 7261.9812, 285.5835, 2789.6635, 0, 2723.243),
		TestCase("2r", CFG9, 2, HeatPumpCont, HeatPump3Stage, OthrElHtr, AuxHtrPwr30, HDC7, HDC7, 7261.9812, 285.5835, 2789.6635, 0, 2723.243),

		TestCase("2s", CFG9, 2, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC4, HDC8, 7261.9812, 249.1856, 2789.6635, 0, 2723.243),
		TestCase("2t", CFG9, 2, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC3, HDC7, 7261.9812, 249.1856, 2789.6635, 0, 2723.243),

		TestCase("2u", CFG9, 2, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC8, HDC4, 7261.9812, 2242.6707, 330.0901, 0, 2723.243),
		TestCase("2v", CFG9, 2, HeatPump2Stage, HeatPumpR744, WaterElHtr | OthrElHtr, AuxHtrPwr30, HDC7, HDC3, 7261.9812, 2242.6707, 330.0901, 0, 2723.243),

		TestCase("2w", CFG9, 2, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC4, HDC4, 7261.9812, 2491.8563, 0, 0, 2723.243),
		TestCase("2x", CFG9, 2, HeatPumpR744, HeatPumpR744, WaterElHtr, AuxHtrPwr30, HDC3, HDC3, 7261.9812, 2491.8563, 0, 0, 2723.243),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG9(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	// =========================

	[
		TestCase("1a", CFG10, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1b", CFG10, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1c", CFG10, 1, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 13475.4938, 0, 0, 0, 0),
		TestCase("1d", CFG10, 1, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 13475.4938, 0, 0, 14489.7782, 0),

		TestCase("1e", CFG10, 1, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 13475.4938, 7486.3854, 0, 0, 0),
		TestCase("1f", CFG10, 1, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 13475.4938, 3743.1927, 0, 7244.8891, 0),

		TestCase("1g", CFG10, 1, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1h", CFG10, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1i", CFG10, 1, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 13475.4938, 0, 0, 0, 16844.3672),
		TestCase("1j", CFG10, 1, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 13475.4938, 0, 0, 2897.9556, 13475.4938),

		TestCase("1k", CFG10, 1, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 13475.4938, 2994.5542, 0, 0, 10106.6203),
		TestCase("1l", CFG10, 1, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 13475.4938, 2994.5542, 0, 2897.9556, 6737.7469),
		// ---
		TestCase("2a", CFG10, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 7261.9812, 0, 4428.0373, 0, 0),
		TestCase("2b", CFG10, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 7261.9812, 0, 4428.0373, 0, 0),

		TestCase("2c", CFG10, 2, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 7261.9812, 4079.7647, 0, 0, 0),
		TestCase("2d", CFG10, 2, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 7261.9812, 4079.7647, 0, 0, 0),

		TestCase("2e", CFG10, 2, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 7261.9812, 3559.7947, 0, 0, 0),
		TestCase("2f", CFG10, 2, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 7261.9812, 3559.7947, 0, 0, 0),

		TestCase("2g", CFG10, 2, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 7261.9812, 0, 3099.6261, 0, 2723.243),
		TestCase("2h", CFG10, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 7261.9812, 0, 3099.6261, 0, 2723.243),

		TestCase("2i", CFG10, 2, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 7261.9812, 2855.8353, 0, 0, 2723.243),
		TestCase("2j", CFG10, 2, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 7261.9812, 2855.8353, 0, 0, 2723.243),

		TestCase("2k", CFG10, 2, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 7261.9812, 2491.8563, 0, 0, 2723.243),
		TestCase("2l", CFG10, 2, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 7261.9812, 2491.8563, 0, 0, 2723.243),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_SingleEnvironment_CFG10(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater,
			auxHeaterPwr, expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	// ==================================================
	// ==================================================

	[
		TestCase("a", CFG1, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC12, HDC12, 1485.6642, 0, 0, 0, 0),
		TestCase("b", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr0, HDC9, HDC9, 1485.6642, 0, 0, 1597.4884, 0),
		TestCase("c", CFG1, HeatPumpNone, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC9, HDC9, 1485.6642, 0, 0, 1597.4884, 0),
		TestCase("d", CFG1, HeatPumpNone, HeatPumpNone, OthrElHtr | AirElHtr, AuxHtrPwr0, HDC9, HDC9, 1485.6642, 0, 0, 1597.4884, 0),

		TestCase("e", CFG1, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC11, HDC11, 1485.6642, 0, 0, 0, 1857.0802),
		TestCase("f", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC10, HDC10, 1485.6642, 0, 0, 15.3592, 1839.2252),
		TestCase("g", CFG1, HeatPumpNone, HeatPumpNone, WaterElHtr, AuxHtrPwr30, HDC10, HDC10, 1485.6642, 0, 0, 15.3592, 1839.2252),
		TestCase("h", CFG1, HeatPumpNone, HeatPumpNone, OthrElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC10, 1485.6642, 0, 0, 15.3592, 1839.2252),
		Category(Definitions.TESTCASE_MIGRATED)
	]
	public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG1(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	[
		TestCase("a", CFG2, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 1485.6642, 0, 73.8202, 0, 0),
		TestCase("b", CFG2, HeatPump2Stage, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC6, HDC9, 1485.6642, 0, 73.8202, 1445.4191, 0),
		TestCase("c", CFG2, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 1485.6642, 0, 58.5043, 0, 1711.8704),
		TestCase("d", CFG2, HeatPump2Stage, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 1485.6642, 0, 58.5043, 15.3592, 1694.0154),

		TestCase("e", CFG2, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 1485.6642, 65.4048, 0, 0, 0),
		TestCase("f", CFG2, HeatPumpCont, HeatPumpNone, OthrElHtr, AuxHtrPwr0, HDC6, HDC9, 1485.6642, 65.4048, 0, 1445.4191, 0),
		TestCase("g", CFG2, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 1485.6642, 52.0243, 0, 0, 1711.8704),
		TestCase("h", CFG2, HeatPumpCont, HeatPumpNone, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC7, HDC10, 1485.6642, 52.0243, 0, 15.3592, 1694.0154),

		TestCase("i", CFG2, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC1, HDC12, 1485.6642, 63.0399, 0, 0, 0),
		TestCase("j", CFG2, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC2, HDC9, 1485.6642, 61.056, 0, 1441.5793, 0),
		TestCase("k", CFG2, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC4, HDC11, 1485.6642, 48.9335, 0, 0, 1708.2994),
		TestCase("l", CFG2, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr30, HDC3, HDC10, 1485.6642, 48.9335, 0, 15.3592, 1690.4444),
		Category(Definitions.TESTCASE_MIGRATED)

    ]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG2(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	[
		TestCase("a", CFG3, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC12, HDC12, 1485.6642, 0, 0, 0, 0),
		TestCase("b", CFG3, HeatPumpNone, HeatPumpNone, WaterElHtr | OthrElHtr, AuxHtrPwr0, HDC9, HDC9, 1485.6642, 0, 0, 1597.4884, 0),

		TestCase("c", CFG3, HeatPumpNone, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC11, HDC11, 1485.6642, 0, 0, 0, 1857.0802),
		TestCase("d", CFG3, HeatPumpNone, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC10, HDC10, 1485.6642, 0, 0, 15.3592, 1839.2252),
		Category(Definitions.TESTCASE_MIGRATED)

    ]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG3(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

    [
		TestCase("a", CFG4, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 1485.6642, 0, 73.8202, 0, 0),
		TestCase("b", CFG4, HeatPump2Stage, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC6, HDC9, 1485.6642, 0, 73.8202, 1445.4191, 0),

		TestCase("c", CFG4, HeatPump2Stage, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 1485.6642, 0, 58.5043, 0, 1711.8704),
		TestCase("d", CFG4, HeatPump2Stage, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 1485.6642, 0, 58.5043, 15.3592, 1694.0154),

		TestCase("e", CFG4, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC5, HDC12, 1485.6642, 65.4048, 0, 0, 0),
		TestCase("f", CFG4, HeatPumpCont, HeatPumpNone, AirElHtr | OthrElHtr, AuxHtrPwr0, HDC6, HDC9, 1485.6642, 65.4048, 0, 1445.4191, 0),

		TestCase("g", CFG4, HeatPumpCont, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC8, HDC11, 1485.6642, 52.0243, 0, 0, 1711.8704),
		TestCase("h", CFG4, HeatPumpCont, HeatPumpNone, AirElHtr, AuxHtrPwr30, HDC7, HDC10, 1485.6642, 52.0243, 0, 15.3592, 1694.0154),

		TestCase("i", CFG4, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr0, HDC1, HDC12, 1485.6642, 63.0399, 0, 0, 0),
		TestCase("j", CFG4, HeatPumpR744, HeatPumpNone, WaterElHtr, AuxHtrPwr0, HDC2, HDC9, 1485.6642, 61.056, 0, 1441.5793, 0),

		TestCase("k", CFG4, HeatPumpR744, HeatPumpNone, NoElHtr, AuxHtrPwr30, HDC4, HDC11, 1485.6642, 48.9335, 0, 0, 1708.2994),
		TestCase("l", CFG4, HeatPumpR744, HeatPumpNone, OthrElHtr, AuxHtrPwr30, HDC3, HDC10, 1485.6642, 48.9335, 0, 15.3592, 1690.4444),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG4(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

    [
		TestCase("a", CFG5, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 1485.6642, 0, 699.8368, 0, 0),
		TestCase("b", CFG5, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 1485.6642, 0, 699.8368, 76.7958, 0),

		TestCase("c", CFG5, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 1485.6642, 654.0484, 0, 0, 0),
		TestCase("d", CFG5, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 1485.6642, 654.0484, 0, 76.7958, 0),

		TestCase("e", CFG5, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 1485.6642, 630.3992, 0, 0, 0),
		TestCase("f", CFG5, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 1485.6642, 610.5603, 0, 38.3979, 0),

		TestCase("g", CFG5, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 1485.6642, 0, 555.5323, 0, 404.9817),
		TestCase("h", CFG5, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 1485.6642, 0, 555.5323, 15.3592, 387.1267),

		TestCase("i", CFG5, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 1485.6642, 520.2431, 0, 0, 404.9817),
		TestCase("j", CFG5, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 1485.6642, 520.2431, 0, 15.3592, 387.1267),

		TestCase("k", CFG5, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 1485.6642, 489.3354, 0, 0, 369.2716),
		TestCase("l", CFG5, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 1485.6642, 489.3354, 0, 15.3592, 351.4166),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG5(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

    [
		TestCase("a", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 1485.6642, 0, 699.8368, 0, 0),
		TestCase("b", CFG6, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 1485.6642, 0, 699.8368, 76.7958, 0),

		TestCase("c", CFG6, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 1485.6642, 654.0484, 0, 0, 0),
		TestCase("d", CFG6, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 1485.6642, 654.0484, 0, 76.7958, 0),

		TestCase("e", CFG6, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 1485.6642, 630.3992, 0, 0, 0),
		TestCase("f", CFG6, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 1485.6642, 610.5603, 0, 38.3979, 0),

		TestCase("g", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 1485.6642, 0, 555.5323, 0, 404.9817),
		TestCase("h", CFG6, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 1485.6642, 0, 555.5323, 15.3592, 387.1267),

		TestCase("i", CFG6, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 1485.6642, 520.2431, 0, 0, 404.9817),
		TestCase("j", CFG6, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 1485.6642, 520.2431, 0, 15.3592, 387.1267),

		TestCase("k", CFG6, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 1485.6642, 489.3354, 0, 0, 369.2716),
		TestCase("l", CFG6, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 1485.6642, 489.3354, 0, 15.3592, 351.4166),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG6(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

    [
		TestCase("a", CFG7, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 1485.6642, 0, 703.6733, 0, 0),
		TestCase("b", CFG7, HeatPump2Stage, HeatPump3Stage, AirElHtr, AuxHtrPwr0, HDC6, HDC6, 1485.6642, 0, 703.6733, 76.7958, 0),

		TestCase("c", CFG7, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 1485.6642, 65.4048, 629.8532, 0, 0),
		TestCase("d", CFG7, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC6, HDC6, 1485.6642, 65.4048, 629.8532, 76.7958, 0),

		TestCase("e", CFG7, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 1485.6642, 588.6435, 73.8202, 0, 0),
		TestCase("f", CFG7, HeatPump2Stage, HeatPumpCont, OthrElHtr, AuxHtrPwr0, HDC6, HDC6, 1485.6642, 588.6435, 73.8202, 76.7958, 0),

		TestCase("g", CFG7, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC1, HDC5, 1485.6642, 63.0399, 629.8532, 0, 0),
		TestCase("h", CFG7, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC2, HDC6, 1485.6642, 61.056, 629.8532, 72.956, 0),

		TestCase("i", CFG7,HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC5, HDC1, 1485.6642, 567.3593, 73.8202, 0, 0),
		TestCase("j", CFG7,HeatPump2Stage, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC6, HDC2, 1485.6642, 549.5042, 73.8202, 42.2377, 0),

		TestCase("k", CFG7, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC1, HDC1, 1485.6642, 630.3992, 0, 0, 0),
		TestCase("l", CFG7, HeatPumpR744, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC2, HDC2, 1485.6642, 610.5603, 0, 38.3979, 0),

		TestCase("m", CFG7, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 1485.6642, 0, 558.4834, 0, 404.9817),
		TestCase("n", CFG7, HeatPump2Stage, HeatPump3Stage, AirElHtr, AuxHtrPwr30, HDC7, HDC7, 1485.6642, 0, 558.4834, 15.3592, 387.1267),

		TestCase("o", CFG7, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 1485.6642, 52.0243, 499.9791, 0, 404.9817),
		TestCase("p", CFG7, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC7, HDC7, 1485.6642, 52.0243, 499.9791, 15.3592, 387.1267),

		TestCase("q", CFG7, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 1485.6642, 468.2188, 58.5043, 0, 404.9817),
		TestCase("r", CFG7, HeatPump2Stage, HeatPumpCont, OthrElHtr, AuxHtrPwr30, HDC7, HDC7, 1485.6642, 468.2188, 58.5043, 15.3592, 387.1267),

		TestCase("s", CFG7, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC4, HDC8, 1485.6642, 48.9335, 499.9791, 0, 401.4107),
		TestCase("t", CFG7, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC3, HDC7, 1485.6642, 48.9335, 499.9791, 15.3592, 383.5557),

		TestCase("u", CFG7, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC8, HDC4, 1485.6642, 440.4019, 58.5043, 0, 372.8427),
		TestCase("v", CFG7, HeatPump2Stage, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC7, HDC3, 1485.6642, 440.4019, 58.5043, 15.3592, 354.9876),

		TestCase("w", CFG7, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC4, HDC4, 1485.6642, 489.3354, 0, 0, 369.2716),
		TestCase("x", CFG7, HeatPumpR744, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC3, HDC3, 1485.6642, 489.3354, 0, 15.3592, 351.4166),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG7(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

	[
		TestCase("a", CFG8, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 1485.6642, 0, 699.8368, 0, 0),
		TestCase("b", CFG8, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 1485.6642, 0, 699.8368, 76.7958, 0),

		TestCase("c", CFG8, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 1485.6642, 654.0484, 0, 0, 0),
		TestCase("d", CFG8, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 1485.6642, 654.0484, 0, 76.7958, 0),

		TestCase("e", CFG8, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 1485.6642, 630.3992, 0, 0, 0),
		TestCase("f", CFG8, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 1485.6642, 610.5603, 0, 38.3979, 0),

		TestCase("g", CFG8, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 1485.6642, 0, 555.5323, 0, 404.9817),
		TestCase("h", CFG8, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 1485.6642, 0, 555.5323, 15.3592, 387.1267),

		TestCase("i", CFG8, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 1485.6642, 520.2431, 0, 0, 404.9817),
		TestCase("j", CFG8, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 1485.6642, 520.2431, 0, 15.3592, 387.1267),

		TestCase("k", CFG8, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 1485.6642, 489.3354, 0, 0, 369.2716),
		TestCase("l", CFG8, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 1485.6642, 489.3354, 0, 15.3592, 351.4166),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG8(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

    [
		TestCase("a", CFG9, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 1485.6642, 0, 703.6733, 0, 0),
		TestCase("b", CFG9, HeatPump2Stage, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC6, HDC6, 1485.6642, 0, 703.6733, 76.7958, 0),

		TestCase("c", CFG9, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 1485.6642, 588.6435, 73.8202, 0, 0),
		TestCase("d", CFG9, HeatPump2Stage, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC6, HDC6, 1485.6642, 588.6435, 73.8202, 76.7958, 0),

		TestCase("e", CFG9, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC5, HDC5, 1485.6642, 65.4048, 629.8532, 0, 0),
		TestCase("f", CFG9, HeatPumpCont, HeatPump3Stage, OthrElHtr, AuxHtrPwr0, HDC6, HDC6, 1485.6642, 65.4048, 629.8532, 76.7958, 0),

		TestCase("g", CFG9, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC1, HDC5, 1485.6642, 63.0399, 629.8532, 0, 0),
		TestCase("h", CFG9, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC2, HDC6, 1485.6642, 61.056, 629.8532, 72.956, 0),

		TestCase("i", CFG9, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC5, HDC1, 1485.6642, 567.3593, 73.8202, 0, 0),
		TestCase("j", CFG9, HeatPump2Stage, HeatPumpR744, WaterElHtr | OthrElHtr, AuxHtrPwr0, HDC6, HDC2, 1485.6642, 549.5042, 73.8202, 42.2377, 0),

		TestCase("k", CFG9, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC1, HDC1, 1485.6642, 630.3992, 0, 0, 0),
		TestCase("l", CFG9, HeatPumpR744, HeatPumpR744, WaterElHtr, AuxHtrPwr0, HDC2, HDC2, 1485.6642, 610.5603, 0, 38.3979, 0),

		TestCase("m", CFG9, HeatPump2Stage, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 1485.6642, 0, 558.4834, 0, 404.9817),
		TestCase("n", CFG9, HeatPump2Stage, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC7, HDC7, 1485.6642, 0, 558.4834, 15.3592, 387.1267),

		TestCase("o", CFG9, HeatPump2Stage, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 1485.6642, 468.2188, 58.5043, 0, 404.9817),
		TestCase("p", CFG9, HeatPump2Stage, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC7, HDC7, 1485.6642, 468.2188, 58.5043, 15.3592, 387.1267),

		TestCase("q", CFG9, HeatPumpCont, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC8, HDC8, 1485.6642, 52.0243, 499.9791, 0, 404.9817),
		TestCase("r", CFG9, HeatPumpCont, HeatPump3Stage, OthrElHtr, AuxHtrPwr30, HDC7, HDC7, 1485.6642, 52.0243, 499.9791, 15.3592, 387.1267),

		TestCase("s", CFG9, HeatPumpR744, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC4, HDC8, 1485.6642, 48.9335, 499.9791, 0, 401.4107),
		TestCase("t", CFG9, HeatPumpR744, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, HDC3, HDC7, 1485.6642, 48.9335, 499.9791, 15.3592, 383.5557),

		TestCase("u", CFG9, HeatPump2Stage, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC8, HDC4, 1485.6642, 440.4019, 58.5043, 0, 372.8427),
		TestCase("v", CFG9, HeatPump2Stage, HeatPumpR744, WaterElHtr | OthrElHtr, AuxHtrPwr30, HDC7, HDC3, 1485.6642, 440.4019, 58.5043, 15.3592, 354.9876),

		TestCase("w", CFG9, HeatPumpR744, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC4, HDC4, 1485.6642, 489.3354, 0, 0, 369.2716),
		TestCase("x", CFG9, HeatPumpR744, HeatPumpR744, WaterElHtr, AuxHtrPwr30, HDC3, HDC3, 1485.6642, 489.3354, 0, 15.3592, 351.4166),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG9(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}

    [
		TestCase("a", CFG10, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 1485.6642, 0, 699.8368, 0, 0),
		TestCase("b", CFG10, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr0, HDC9, HDC6, 1485.6642, 0, 699.8368, 76.7958, 0),

		TestCase("c", CFG10, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr0, HDC12, HDC5, 1485.6642, 654.0484, 0, 0, 0),
		TestCase("d", CFG10, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr0, HDC9, HDC6, 1485.6642, 654.0484, 0, 76.7958, 0),

		TestCase("e", CFG10, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr0, HDC12, HDC1, 1485.6642, 630.3992, 0, 0, 0),
		TestCase("f", CFG10, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr0, HDC9, HDC2, 1485.6642, 610.5603, 0, 38.3979, 0),

		TestCase("g", CFG10, HeatPumpNone, HeatPump3Stage, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 1485.6642, 0, 555.5323, 0, 404.9817),
		TestCase("h", CFG10, HeatPumpNone, HeatPump3Stage, WaterElHtr | AirElHtr, AuxHtrPwr30, HDC10, HDC7, 1485.6642, 0, 555.5323, 15.3592, 387.1267),

		TestCase("i", CFG10, HeatPumpNone, HeatPumpCont, NoElHtr, AuxHtrPwr30, HDC11, HDC8, 1485.6642, 520.2431, 0, 0, 404.9817),
		TestCase("j", CFG10, HeatPumpNone, HeatPumpCont, AirElHtr, AuxHtrPwr30, HDC10, HDC7, 1485.6642, 520.2431, 0, 15.3592, 387.1267),

		TestCase("k", CFG10, HeatPumpNone, HeatPumpR744, NoElHtr, AuxHtrPwr30, HDC11, HDC4, 1485.6642, 489.3354, 0, 0, 369.2716),
		TestCase("l", CFG10, HeatPumpNone, HeatPumpR744, OthrElHtr, AuxHtrPwr30, HDC10, HDC3, 1485.6642, 489.3354, 0, 15.3592, 351.4166),
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void SSMTest_HeatingDistribution_AvgAllEnvironment_CFG10(string dummySort, BusHVACSystemConfiguration cfg,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_AvgAllEnvironment(cfg, driverHeatpump, passengerHeatpump, electricHeater,
			auxHeaterPwr, expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater);
	}


	// ==================================================

	[
		TestCase(CFG7  , 11  , HeatPump2Stage    , HeatPump3Stage, 5000, 9639.0, 329.0112, 5705.3337),
		TestCase(CFG7, 11, HeatPumpCont, HeatPumpCont, 5000, 9639.0, 5671.7119, 0),
		Category(Definitions.TESTCASE_MIGRATED)
		]
	public void SSMTest_CoolingLimited_SingleEnvironment(BusHVACSystemConfiguration cfg, int envId, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, double expectedMaxCoolingPwrDriver, double expectedMaxCoolingPwrPassenger
		, double expectedElPwrW, double expectedMechPwrW)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatpumpCooling: driverHeatpump, 
			passengerHeatpumpCooling: passengerHeatpump, length: 12, height: 1.5);

		// set list of environmental conditions to a single entry
		var ssmInputs = auxData.SSMInputsCooling as SSMInputs;
		Assert.NotNull(ssmInputs);
		var envAll = ssmInputs.EnvironmentalConditions;
		var selectedEnv = envAll.EnvironmentalConditionsMap.GetEnvironmentalConditions().Where(x => x.ID == envId).FirstOrDefault();
		Assert.NotNull(selectedEnv);
		var newEnv = new EnvironmentalConditionMapEntry(selectedEnv.ID, selectedEnv.Temperature, selectedEnv.Solar, 1.0,
			selectedEnv.HeatPumpCoP.ToDictionary(x => x.Key, x => x.Value),
			selectedEnv.HeaterEfficiency.ToDictionary(x => x.Key, x => x.Value));
		ssmInputs.EnvironmentalConditionsMap = new EnvironmentalConditionsMap(new[] { newEnv });
		// override passenger count
		ssmInputs.NumberOfPassengers = 150;
		// ---

		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsCooling);

		Console.WriteLine($"{ssm.ElectricalWAdjusted.Value().ToGUIFormat(4)}, {ssm.MechanicalWBaseAdjusted.Value().ToGUIFormat(4)}");

		Assert.AreEqual(expectedMaxCoolingPwrDriver, ssmInputs.HVACMaxCoolingPowerDriver.Value(), "max cooling power driver");
		Assert.AreEqual(expectedMaxCoolingPwrPassenger, ssmInputs.HVACMaxCoolingPowerPassenger.Value(), "max cooling power passenger");
		Assert.AreEqual(expectedElPwrW, ssm.ElectricalWAdjusted.Value(), 1e-3, "expected Electric Power");
		Assert.AreEqual(expectedMechPwrW, ssm.MechanicalWBaseAdjusted.Value(), 1e-3, "expected Mechanical Power");

	}

    // ==================================================
    [
		TestCase(CFG1, 1, 5e3, 8475.49374),
		TestCase(CFG1, 2, 5e3, 2261.98124),
		TestCase(CFG1, 3, 5e3, 0), // no negative heating demand!
		TestCase(CFG1, 4, 5e3, 0), // no negative heating demand!
		TestCase(CFG1, 5, 5e3, 0), // no negative heating demand!
		TestCase(CFG1, 6, 5e3, 0),
		TestCase(CFG1, 7, 5e3, 0),
		TestCase(CFG1, 8, 5e3, 0),
		TestCase(CFG1, 9, 5e3, 0),
		TestCase(CFG1, 10, 5e3, 0),
		TestCase(CFG1, 11, 5e3, 0),
		Category(Definitions.TESTCASE_MIGRATED)
	]
	public void SSMTest_HeatingDemand_EngineWasteHeat_SingleEnvironment(BusHVACSystemConfiguration cfg, int envId,
		double engineWasteHeat, double expectedHeatingPower)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatPumpHeating: HeatPumpNone, passengerHeatPumpHeating: HeatPumpNone,
			electricHeater: NoElHtr, auxHeaterPower: 0.SI<Watt>());

		// set list of environmental conditions to a single entry
		var ssmInputs = auxData.SSMInputsHeating as SSMInputs;
		Assert.NotNull(ssmInputs);
		var envAll = ssmInputs.EnvironmentalConditions;
		var selectedEnv = envAll.EnvironmentalConditionsMap.GetEnvironmentalConditions().Where(x => x.ID == envId).FirstOrDefault();
		Assert.NotNull(selectedEnv);
		var newEnv = new EnvironmentalConditionMapEntry(selectedEnv.ID, selectedEnv.Temperature, selectedEnv.Solar, 1.0,
			selectedEnv.HeatPumpCoP.ToDictionary(x => x.Key, x => x.Value),
			selectedEnv.HeaterEfficiency.ToDictionary(x => x.Key, x => x.Value));
		ssmInputs.EnvironmentalConditionsMap = new EnvironmentalConditionsMap(new[] { newEnv });
		// ---

		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsHeating);

		var heatingPower = ssm.AverageHeaterPower(engineWasteHeat.SI<Watt>());

		Console.WriteLine($"{heatingPower.RequiredHeatingPower.Value().ToGUIFormat(4)}");

		Assert.AreEqual(expectedHeatingPower, heatingPower.RequiredHeatingPower.Value(), 1e-3, "expected Heating Demand");
	}

    [
		TestCase("a", CFG7, 1, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, 8e3, HDC7, HDC7, 5475.4938, 0, 0, 1177.5255, 5475.4938),
		TestCase("b", CFG7, 2, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, 8e3, HDC7, HDC7, 0, 0, 0, 0, 0),
		TestCase("c", CFG7, 3, HeatPumpCont, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, 8e3, HDC7, HDC7, 0, 0, 0, 0, 0),

		TestCase("d", CFG6, 1, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, 8e3, HDC10, HDC7, 5475.4938, 0, 0, 1177.5255, 5475.4937),
		TestCase("e", CFG6, 2, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, 8e3, HDC10, HDC7, 0, 0, 0, 0, 0),
		TestCase("f", CFG6, 3, HeatPumpNone, HeatPump3Stage, WaterElHtr, AuxHtrPwr30, 8e3, HDC10, HDC7, 0, 0, 0, 0, 0),

	]
	public void SSMTest_HeatingDistribution_EngineWasteHeat_SingleEnvironment(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr, double engineWasteHeat,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		SSMTest_HeatingDistribution_SingleEnvironment(cfg, envId, driverHeatpump, passengerHeatpump, electricHeater, auxHeaterPwr,
			expectedHeatingDistributionCase, expectedHeatingDistributionCasePassenger, expectedHeatingDemand,
			expectedHPElPwrW, expectedHPMechPwrW, expecetdElecricPowerElHeater, expectedAuxHeater, engineWasteHeat);
	}

	// ==================================================

	[
		TestCase("a1", CFG7, 1, HeatPumpR744, HeatPump3Stage, 0, HDC1, HDC5, 7513.7338, 331.3278, 0, 0, 0), // exceeding heating limit
		TestCase("a2", CFG7, 1, HeatPumpR744, HeatPump3Stage, 5e3, HDC1, HDC5, 2513.7337, 139.6519, 0, 0, 0), // not exceeding heating limit
		TestCase("b1", CFG7, 1, HeatPump2Stage, HeatPumpR744, 0, HDC5, HDC1, 7513.7338, 2981.95, 0, 0, 0),
		TestCase("b2", CFG7, 1, HeatPump2Stage, HeatPumpR744, 5e3, HDC5, HDC1, 2513.7337, 1256.8669, 0, 0, 0),
		TestCase("c1", CFG7, 1, HeatPumpR744, HeatPumpR744, 0, HDC1, HDC1, 7513.7338, 3313.2778, 0, 0, 0),
		TestCase("c2", CFG7, 1, HeatPumpR744, HeatPumpR744, 5e3, HDC1, HDC1, 2513.7337, 1396.5188, 0, 0, 0),
	]
	public void SSMTest_HeatingDistribution_SingleEnvironment_Limited(string dummySort, BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, double engineWasteHeat,
		HeatingDistributionCase expectedHeatingDistributionCase, HeatingDistributionCase expectedHeatingDistributionCasePassenger,
		double expectedHeatingDemand, double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatPumpHeating: driverHeatpump, passengerHeatPumpHeating: passengerHeatpump,
			electricHeater: NoElHtr, auxHeaterPower: 0.SI<Watt>(), height: 0.6);

		// set list of environmental conditions to a single entry
		var ssmInputs = auxData.SSMInputsHeating as SSMInputs;
		Assert.NotNull(ssmInputs);
		var envAll = ssmInputs.EnvironmentalConditions;
		var selectedEnv = envAll.EnvironmentalConditionsMap.GetEnvironmentalConditions().Where(x => x.ID == envId).FirstOrDefault();
		Assert.NotNull(selectedEnv);
		var newEnv = new EnvironmentalConditionMapEntry(selectedEnv.ID, selectedEnv.Temperature, selectedEnv.Solar, 1.0,
			selectedEnv.HeatPumpCoP.ToDictionary(x => x.Key, x => x.Value),
			selectedEnv.HeaterEfficiency.ToDictionary(x => x.Key, x => x.Value));
		ssmInputs.EnvironmentalConditionsMap = new EnvironmentalConditionsMap(new[] { newEnv });
		

		ssmInputs.NumberOfPassengers = 1;
		// ---

		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsHeating);

		var heatingPower = ssm.AverageHeaterPower(engineWasteHeat.SI<Watt>());

		Console.WriteLine($"HDC{ssmInputs.HeatingDistributionCaseDriver.GetID()}, HDC{ssmInputs.HeatingDistributionCasePassenger.GetID()}, {heatingPower.RequiredHeatingPower.Value().ToGUIFormat(4)}, {heatingPower.HeatPumpPowerEl.Value().ToGUIFormat(4)}, {heatingPower.HeatPumpPowerMech.Value().ToGUIFormat(4)}, {heatingPower.ElectricHeaterPowerEl.Value().ToGUIFormat(4)}, {heatingPower.AuxHeaterPower.Value().ToGUIFormat(4)}");

		Assert.AreEqual(expectedHeatingDistributionCase, ssmInputs.HeatingDistributionCaseDriver, "Driver");
		Assert.AreEqual(expectedHeatingDistributionCasePassenger, ssmInputs.HeatingDistributionCasePassenger, "Passenger");

		Assert.AreEqual(engineWasteHeat.IsEqual(0), heatingPower.RequiredHeatingPower.Value() > ssmInputs.MaxHeatingPower.Value());

		Assert.AreEqual(expectedHeatingDemand, heatingPower.RequiredHeatingPower.Value(), 1e-3, "expected Heating Demand");
		Assert.AreEqual(expectedHPElPwrW, heatingPower.HeatPumpPowerEl.Value(), 1e-3, "expected HeatPump Electric Power");
		Assert.AreEqual(expectedHPMechPwrW, heatingPower.HeatPumpPowerMech.Value(), 1e-3, "expected HeatPump Mechanical Power");
		Assert.AreEqual(expecetdElecricPowerElHeater, heatingPower.ElectricHeaterPowerEl.Value(), 1e-3, "expected ElectricHeater Power");
		Assert.AreEqual(expectedAuxHeater, heatingPower.AuxHeaterPower.Value(), 1e-3, "expected AuxHeater Power");

		// re-calculate electrical and mechanical heating demand, ignoring electrical heater and aux heater
		// in case no engine waste heat is available, the re-calculated values have to be higher than the values from the SSM module where certain 
		// limits are applied (only in case the heat-pump is used!)
		var copDriver = newEnv.HeatPumpCoP.ContainsKey(driverHeatpump) ? newEnv.HeatPumpCoP[driverHeatpump] : double.NaN;
		var copPassenger = newEnv.HeatPumpCoP.ContainsKey(passengerHeatpump) ? newEnv.HeatPumpCoP[passengerHeatpump] : double.NaN;
		var heatingDriver = expectedHeatingDemand.SI<Watt>() * ssmInputs.DriverHVACContribution;
		var heatingPassenger = expectedHeatingDemand.SI<Watt>() * ssmInputs.PassengerHVACContribution;

		var hpElHeating = (driverHeatpump.IsElectrical() && !double.IsNaN(copDriver) ? heatingDriver / copDriver : 0.SI<Watt>()) + 
						(passengerHeatpump.IsElectrical() && !double.IsNaN(copPassenger) ? heatingPassenger / copPassenger : 0.SI<Watt>());
		var hpMechHeating = (driverHeatpump.IsMechanical() && !double.IsNaN(copDriver) ? heatingDriver / copDriver : 0.SI<Watt>()) +
						(passengerHeatpump.IsMechanical() && !double.IsNaN(copPassenger) ? heatingPassenger / copPassenger : 0.SI<Watt>());
		if (engineWasteHeat.IsEqual(0)) {
			Assert.IsTrue(hpElHeating.IsEqual(0) || hpElHeating > heatingPower.HeatPumpPowerEl);
			Assert.IsTrue(hpMechHeating.IsEqual(0) || hpMechHeating > heatingPower.HeatPumpPowerMech);
		} else {
			Assert.AreEqual(hpElHeating.Value(), heatingPower.HeatPumpPowerEl.Value(), 1e-3);
			Assert.AreEqual(hpMechHeating.Value(), heatingPower.HeatPumpPowerMech.Value(), 1e-3);
		}
	}

	// ==================================================
	// ==================================================

	public void SSMTest_HeatingDistribution_AvgAllEnvironment(BusHVACSystemConfiguration cfg, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr, HeatingDistributionCase expectedHeatingDistributionCaseDriver,
		HeatingDistributionCase expectedHeatingDistributionCasePassenger, double expectedHeatingPower,
		double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatPumpHeating: driverHeatpump, passengerHeatPumpHeating: passengerHeatpump,
			electricHeater: electricHeater, auxHeaterPower: auxHeaterPwr.SI<Watt>());

		// set list of environmental conditions to a single entry
		var ssmInputs = auxData.SSMInputsHeating as SSMInputs;
		Assert.NotNull(ssmInputs);
		// ---

		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsHeating);

		var heatingPower = ssm.AverageHeaterPower(0.SI<Watt>());

		Console.WriteLine($"HDC{ssmInputs.HeatingDistributionCaseDriver.GetID()}, HDC{ssmInputs.HeatingDistributionCasePassenger.GetID()}, {heatingPower.RequiredHeatingPower.Value().ToGUIFormat(4)}, {heatingPower.HeatPumpPowerEl.Value().ToGUIFormat(4)}, {heatingPower.HeatPumpPowerMech.Value().ToGUIFormat(4)}, {heatingPower.ElectricHeaterPowerEl.Value().ToGUIFormat(4)}, {heatingPower.AuxHeaterPower.Value().ToGUIFormat(4)}");

		Assert.AreEqual(expectedHeatingDistributionCaseDriver, ssmInputs.HeatingDistributionCaseDriver, "Driver");
		Assert.AreEqual(expectedHeatingDistributionCasePassenger, ssmInputs.HeatingDistributionCasePassenger, "Passenger");

		Assert.AreEqual(expectedHeatingPower, heatingPower.RequiredHeatingPower.Value(), 1e-3, "expected Heating Demand");
		Assert.AreEqual(expectedHPElPwrW, heatingPower.HeatPumpPowerEl.Value(), 1e-3, "expected HeatPump Electric Power");
		Assert.AreEqual(expectedHPMechPwrW, heatingPower.HeatPumpPowerMech.Value(), 1e-3, "expected HeatPump Mechanical Power");
		Assert.AreEqual(expecetdElecricPowerElHeater, heatingPower.ElectricHeaterPowerEl.Value(), 1e-3, "expected ElectricHeater Power");
		Assert.AreEqual(expectedAuxHeater, heatingPower.AuxHeaterPower.Value(), 1e-3, "expected AuxHeater Power");

		//Assert.AreEqual(expecetdElecricPowerElHeater, ssm.);

	}


	protected void SSMTest_HeatingDistribution_SingleEnvironment(BusHVACSystemConfiguration cfg, int envId,
		HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, HeaterType electricHeater, double auxHeaterPwr,
		HeatingDistributionCase expectedHeatingDistributionCaseDriver,
		HeatingDistributionCase expectedHeatingDistributionCasePassenger, double expectedHeatingPower,
		double expectedHPElPwrW, double expectedHPMechPwrW, double expecetdElecricPowerElHeater,
		double expectedAuxHeater, double engineWasteHeat = 0)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatPumpHeating: driverHeatpump, passengerHeatPumpHeating: passengerHeatpump,
			electricHeater: electricHeater, auxHeaterPower: auxHeaterPwr.SI<Watt>());

		// set list of environmental conditions to a single entry
		var ssmInputs = auxData.SSMInputsHeating as SSMInputs;
		Assert.NotNull(ssmInputs);
		var envAll = ssmInputs.EnvironmentalConditions;
		var selectedEnv = envAll.EnvironmentalConditionsMap.GetEnvironmentalConditions().Where(x => x.ID == envId).FirstOrDefault();
		Assert.NotNull(selectedEnv);
		var newEnv = new EnvironmentalConditionMapEntry(selectedEnv.ID, selectedEnv.Temperature, selectedEnv.Solar, 1.0,
			selectedEnv.HeatPumpCoP.ToDictionary(x => x.Key, x => x.Value),
			selectedEnv.HeaterEfficiency.ToDictionary(x => x.Key, x => x.Value));
		ssmInputs.EnvironmentalConditionsMap = new EnvironmentalConditionsMap(new[] { newEnv });
		// ---

		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsHeating);

		var heatingPower = ssm.AverageHeaterPower(engineWasteHeat.SI<Watt>());

		Console.WriteLine($"HDC{ssmInputs.HeatingDistributionCaseDriver.GetID()}, HDC{ssmInputs.HeatingDistributionCasePassenger.GetID()}, {heatingPower.RequiredHeatingPower.Value().ToGUIFormat(4)}, {heatingPower.HeatPumpPowerEl.Value().ToGUIFormat(4)}, {heatingPower.HeatPumpPowerMech.Value().ToGUIFormat(4)}, {heatingPower.ElectricHeaterPowerEl.Value().ToGUIFormat(4)}, {heatingPower.AuxHeaterPower.Value().ToGUIFormat(4)}");

		Assert.AreEqual(expectedHeatingDistributionCaseDriver, ssmInputs.HeatingDistributionCaseDriver, "Driver");
		Assert.AreEqual(expectedHeatingDistributionCasePassenger, ssmInputs.HeatingDistributionCasePassenger, "Passenger");

        Assert.AreEqual(expectedHeatingPower, heatingPower.RequiredHeatingPower.Value(), 1e-3, "expected Heating Demand");
        Assert.AreEqual(expectedHPElPwrW, heatingPower.HeatPumpPowerEl.Value(), 1e-3, "expected HeatPump Electric Power");
        Assert.AreEqual(expectedHPMechPwrW, heatingPower.HeatPumpPowerMech.Value(), 1e-3, "expected HeatPump Mechanical Power");
        Assert.AreEqual(expecetdElecricPowerElHeater, heatingPower.ElectricHeaterPowerEl.Value(), 1e-3, "expected ElectricHeater Power");
        Assert.AreEqual(expectedAuxHeater, heatingPower.AuxHeaterPower.Value(), 1e-3, "expected AuxHeater Power");

        //Assert.AreEqual(expecetdElecricPowerElHeater, ssm.);

    }

	protected IAuxiliaryConfig GetAuxParametersConventionalSingleDeckForTest(BusHVACSystemConfiguration hvacConfig, HeatPumpType? driverHeatpumpCooling = null,
		HeatPumpType? passengerHeatpumpCooling = null, HeatPumpType? driverHeatPumpHeating = null, HeatPumpType? passengerHeatPumpHeating = null, 
		Watt auxHeaterPower = null, HeaterType? electricHeater = null, int passengerCount = 40, double length = 12, double height = 3)
	{
		return SSMBusAuxModelParameters.CreateBusAuxInputParameters(MissionType.Urban, 
			VehicleClass.Class31a, 
			VectoSimulationJobType.ConventionalVehicle, 
			VehicleCode.CA, 
			RegistrationClass.II, 
			AxleConfiguration.AxleConfig_4x2, 
			articulated: false, 
			lowEntry: false, 
			length: length.SI<Meter>(), 
			height: height.SI<Meter>(), 
			width: 2.55.SI<Meter>(), 
			numPassengersLowerdeck: passengerCount, 
			numPassengersUpperdeck: 0, 
			hpHeatingDriver: driverHeatPumpHeating ?? HeatPumpType.none, 
			hpCoolingDriver: driverHeatpumpCooling ?? HeatPumpType.none, 
			hpHeatingPassenger: passengerHeatPumpHeating ?? HeatPumpType.none, 
			hpCoolingPassenger: passengerHeatpumpCooling ?? HeatPumpType.none, 
			auxHeaterPower: auxHeaterPower ?? 0.SI<Watt>(), 
			airElectricHeater: electricHeater != null && (electricHeater & HeaterType.AirElectricHeater) != 0,
			waterElectricHeater: electricHeater != null && (electricHeater & HeaterType.WaterElectricHeater) != 0,
			otherElectricHeater: electricHeater != null && (electricHeater & HeaterType.OtherElectricHeating) != 0,
			hvacConfig: hvacConfig, 
			doubleGlazing: false, 
			adjustableAuxHeater: false,
			separateAirdistributionDicts: false, 
			adjustableCoolantThermostat: false, 
			engineWasteGasHeatExchanger: false,
            steeringpumps: new[] { "Dual displacement" }, 
			fanTech: "Crankshaft mounted - Discrete step clutch", 
			alternatorTech: AlternatorType.Conventional, 
			entranceHeight: 0.3.SI<Meter>(), 
			loading: LoadingType.ReferenceLoad);

	}

}