using System;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Components.DictionaryAdapter;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter;


[TestFixture]
public class HybridStrategyDataAdapterTest
{

    [TestCase("SerialHybridStrategyParamsTest A", 0.7375, 0.2625, 11273.9176, 778.61, 247500,OvcHevMode.ChargeDepleting, false, 0.49, 0.307641364)]
	[TestCase("SerialHybridStrategyParamsTest B", 0.6, 0.2, 11000, 778.61, 80000,  OvcHevMode.ChargeDepleting, false, 0.39, 0.234065732)]
	[TestCase("SerialHybridStrategyParamsTest C", 0.6, 0.2, 11000, 778.61, 2000,  OvcHevMode.ChargeDepleting, true, 0.39, 0.634065732)]
	[TestCase("SerialHybridStrategyParamsTest D", 0.7375, 0.2625, 11273.9176, 778.61, 247500,  OvcHevMode.ChargeSustaining, false, 0.624646591, 0.307641364)]
	[TestCase("SerialHybridStrategyParamsTest E", 0.6, 0.2, 11000, 778.61, 80000,  OvcHevMode.ChargeSustaining, false, 0.582967134, 0.234065732)]
	[TestCase("SerialHybridStrategyParamsTest F", 0.6, 0.2, 11000, 778.61, 2000,  OvcHevMode.ChargeSustaining, true, 0.268131464, 0.634065732),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void SerialHybridStrategyTest(string testName, double bat_soc_max, double bat_soc_min, double vehicle_mass,
		double nominalVoltage, double nominalCapacity, OvcHevMode mode,
		bool exception, double expected_target_soc, double expected_min_soc)
	{
		var mass = vehicle_mass.SI<Kilogram>();
		var v_nom = nominalVoltage.SI<Volt>();
		var cap_nom = nominalCapacity.SI<AmpereSecond>();


		var dataAdapter = new SerialHybridStrategyParameterDataAdapter();
		var batterySystemData = new BatterySystemData() {
			Batteries = new EditableList<Tuple<int, BatteryData>>() {
				new Tuple<int, BatteryData>(1, new BatteryData() {
					Capacity = cap_nom,
					ChargeDepletingBattery = false,
					BatteryId = -42,
					MaxSOC = bat_soc_max,
					MinSOC = bat_soc_min,
					SOCMap = new SOCMap(new SOCMap.SOCMapEntry[] {
						new SOCMap.SOCMapEntry() {
							BatteryVolts = Math.Max((v_nom + (v_nom * 0.1)).Value(), 0).SI<Volt>(),
							SOC = 0.1
						},

						new SOCMap.SOCMapEntry() {
							BatteryVolts = v_nom,
							SOC = 0.5
						},

						new SOCMap.SOCMapEntry() {
							BatteryVolts = v_nom - (v_nom * 0.1),
							SOC = 0.9
						}
					})
				})
			}
		};

		if (exception) {
			AssertHelper.Exception<Exception>(() => dataAdapter.CreateHybridStrategyParameters(batterySystemData, null, mass, mode), messageContains: "Min SOC higher than Target SOC");
		} else {
			var parameters = dataAdapter.CreateHybridStrategyParameters(batterySystemData, null, mass,
				mode);
			Assert.AreEqual(expected_target_soc, parameters!.TargetSoC, 1e-6, "target soc mismatch");
			Assert.AreEqual(expected_min_soc, parameters.MinSoC, 1e-6, "min soc mismatch");
		}

		

	}

	[TestCase(1000.00000000000000 	,10.00000000000000	,1000.00000000000000	,1.00000000000000	,0.61496863314570	,0.78861497592102	,0.70714213564177	,1.00000000000000	,false)]
	//[TestCase(40000.00000000000000	,10.00000000000000	,1000.00000000000000	,25.00000000000000,	0.77784206083558,	0.62853936105471,	0.70714213564177,	1.00000000000000,	true)] //fallback 30 kmh
	[TestCase(1000.00000000000000 	,10.00000000000000	,1000.00000000000000	,25.00000000000000,	0.12338337323207,	0.99240946348263,	0.70714213564177,	1.00000000000000,	false)]
	[TestCase(40000.00000000000000	,10.00000000000000	,1000.00000000000000	,25.00000000000000,	0.33348329959851,	0.94280904158206,	0.70714213564177,	1.00000000000000,	false)]
	[TestCase(40000.00000000000000	,10.00000000000000	,1000.00000000000000	,3.00000000000000	,0.96230240877072	,0.27216552697591	,0.70714213564177	,1.00000000000000	,true),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void SerialHybridSuperCapTest(double mass_kg, double U_min_V, double U_max_V, double C_F, double exp_soc_min, double exp_soc_target, double exp_soc_initial, double exp_soc_max, bool exception = false)
	{
		var dataAdapter = new SerialHybridStrategyParameterDataAdapter();

		var mass = mass_kg.SI<Kilogram>();
		var Umin_sc = U_min_V.SI<Volt>();
		var Umax_sc = U_max_V.SI<Volt>();
		var Cap_sc = C_F.SI<Farad>();


		var scData = new SuperCapData() {
			Capacity = Cap_sc,
			MaxVoltage = Umax_sc,
			MinVoltage = Umin_sc,
		};
		if (exception) {
			Assert.Throws<VectoException>(() =>
				dataAdapter.CreateHybridStrategyParameters(null, scData, mass,
					OvcHevMode.ChargeSustaining));
			Assert.Pass();
		}
		var parameter = dataAdapter.CreateHybridStrategyParameters(null, scData, mass, OvcHevMode.ChargeSustaining);

		Assert.AreEqual(exp_soc_target, parameter.TargetSoC, 1E-3);
		Assert.AreEqual(exp_soc_min, parameter.MinSoC, 1E-3);
		Assert.AreEqual(exp_soc_max, parameter.MaxSoC, 1E-3);


		Assert.AreEqual(exp_soc_initial, parameter.InitialSoc, 1E-3);
	}

}