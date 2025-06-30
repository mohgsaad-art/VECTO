using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.BatteryTests;

public class BatterySystemTests
{
	public const double REESS_Capacity = 4.5;
	public const double REESS_MinSoC = 0.2;
	public const double REESS_MaxSoC = 0.8;

    [TestCase(0.5, 0.5, 5000),
   TestCase(0.5, 0.5, -5000)]
    public void BatterySystemTimeDependentInternalResistanceTest_LoadChanges(double initialSoC, double dt, double powerDemand)
    {
        var r1 = 0.02;
        var r2 = 0.04;
        var r3 = 0.1;

        var batteryData = new BatterySystemData() {
            Batteries = new List<Tuple<int, BatteryData>>() {
                    Tuple.Create(0, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(0, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(1, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(1, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    })
                }
        };

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var es = new ElectricSystem(container, batteryData);
        es.Connect(bat);
        es.Connect(new MockElectricConsumer(0.SI<Watt>()));
        bat.Initialize(initialSoC);

        var modData = new MockModalDataContainer();

        var absTime = 0.SI<Second>();

        var i = 0;

        for (; i < 11; i++) { // constant for the first 2 sec
            var response = es.Request(absTime, dt.SI<Second>(), -Math.Sign(powerDemand) * Math.Abs(powerDemand).SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            absTime += dt.SI<Second>();
        }

        for (; i < 11 + 5; i++) { // constant for the first 2 sec
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            Assert.AreEqual(r1, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 11 + 21; i++) { // linear increase for the next 8s to 0.04
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            var slope = (r2 - r1) / (10 - 2);
            var r = slope * (absTime.Value() - 5.5) + r1 - slope * 2;
            Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 11 + 41; i++) { // linear increase for the next 10s to 0.1
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            var slope = (r3 - r2) / (20 - 10);
            var r = slope * (absTime.Value() - 5.5) + r2 - slope * 10;
            Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 11 + 100; i++) { // constant after 20 sec
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            Assert.AreEqual(r3, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }
    }

    [TestCase(0.5, 0.5, 5000),
   TestCase(0.5, 0.5, -5000)]
    public void BatterySystemTimeDependentInternalResistanceTest_ConstantLoad(double initialSoC, double dt, double powerDemand)
    {
        var r1 = 0.02;
        var r2 = 0.04;
        var r3 = 0.1;

        var batteryData = new BatterySystemData() {
            Batteries = new List<Tuple<int, BatteryData>>() {
                    Tuple.Create(0, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(0, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(1, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    }),
                    Tuple.Create(1, new BatteryData() {
                        Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        MinSOC = REESS_MinSoC,
                        MaxSOC = REESS_MaxSoC,
                        SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
                        InternalResistance =
                            BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
                        MaxCurrent = BatteryMaxCurrentReader.Create(
                            "SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
                    })
                }
        };

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var es = new ElectricSystem(container, batteryData);
        es.Connect(bat);
        es.Connect(new MockElectricConsumer(0.SI<Watt>()));
        bat.Initialize(initialSoC);

        var modData = new MockModalDataContainer();

        var absTime = 0.SI<Second>();

        var i = 0;
        for (; i < 5; i++) { // constant for the first 2 sec
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            Assert.AreEqual(r1, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 21; i++) { // linear increase for the next 8s to 0.04
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            var slope = (r2 - r1) / (10 - 2);
            var r = slope * absTime.Value() + r1 - slope * 2;
            Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 41; i++) { // linear increase for the next 10s to 0.1
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            var slope = (r3 - r2) / (20 - 10);
            var r = slope * absTime.Value() + r2 - slope * 10;
            Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }

        for (; i < 100; i++) { // constant after 20 sec
            var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
            Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
            bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

            var current = (Ampere)modData[ModalResultField.I_reess];
            var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
            Assert.AreEqual(r3, rREESS.Value(), 1e-9, $"{i} / {absTime}");

            absTime += dt.SI<Second>();
        }
    }


    [TestCase(0.5, 1, 7000, new[] { 0.50020169, 0.50020169, 0.50020169 }),
        TestCase(0.35, 1, 14000, new[] { 0.350404181979, 0.350404181979, 0.350404181979 }),
        TestCase(0.75, 1, -14500, new[] { 0.7495793463, 0.7495793463, 0.7495793463 })

        ]
    public void TestBatterySystemRequest_2Serial(double initialSoC, double simInterval, double powerDemand, double[] expectedSoC)
    {
        var inputData = GetMockBatteryInputData();
        Assert.NotNull(inputData);

        var dao = new EngineeringDataAdapter();
        var tmp = new MockBatteryInputData() {
            _electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = inputData,
                        StringId = 0,
                        Count = 2
                    }
                }
        };
        var batteryData = dao.CreateBatteryData(tmp, 0.8);

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var modData = new MockModalDataContainer();
        bat.Initialize(initialSoC);

        var absTime = 0.SI<Second>();
        var dt = simInterval.SI<Second>();
        var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
        Assert.IsInstanceOf<RESSResponseSuccess>(response);
        bat.CommitSimulationStep(absTime, dt, modData);

        var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
        for (var i = 0; i < socs.Length; i++) {
            Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
        }

        Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
    }


	[TestCase(0.5, 1, 7000, new[] { 0.50020169, 0.50020169, 0.50020169 }),
	TestCase(0.35, 1, 14000, new[] { 0.350404181979, 0.350404181979, 0.350404181979 }),
	TestCase(0.75, 1, -14500, new[] { 0.7495793463, 0.7495793463, 0.7495793463 })

    ]
    public void TestBatterySystemRequest_2Parallel(double initialSoC, double simInterval, double powerDemand, double[] expectedSoC)
    {
        var inputData = GetMockBatteryInputData();
        Assert.NotNull(inputData);

        var dao = new EngineeringDataAdapter();
        var tmp = new MockBatteryInputData() {
            _electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = inputData,
                        StringId = 0,
                        Count = 1
                    },
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = inputData,
                        StringId = 1,
                        Count = 1
                    }
                }
        };
        var batteryData = dao.CreateBatteryData(tmp, 0.8);

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var modData = new MockModalDataContainer();
        bat.Initialize(initialSoC);

        var absTime = 0.SI<Second>();
        var dt = simInterval.SI<Second>();
        var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
        Assert.IsInstanceOf<RESSResponseSuccess>(response);
        bat.CommitSimulationStep(absTime, dt, modData);

        var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
        for (var i = 0; i < socs.Length; i++) {
            Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
        }

        Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
    }

    [TestCase(0.5, 1, 60000, new[] { 10.0, 20, 30 }, new[] { 0.500693147221155, 0.500692716960012, 0.500693147221155, 0.5006930038 }),
    TestCase(0.35, 1, 14000, new[] { 2.33333333, 4.666666666, 7 }, new[] { 0.350161966207795, 0.350161942625547, 0.350161966207795, 0.3501619583470 }),
    TestCase(0.75, 1, -14500, new[] { -2.4166666666, -4.833333333, -7.25 }, new[] { 0.749832099811648, 0.749832074409512, 0.749832099811648, 0.74983209134426 })
    ]
    public void TestBatterySystemRequest_3Strings_1_Balanced(double initialSoC, double simInterval, double powerDemand, double[] expectedPowerDemand, double[] expectedSoC)
    {
        var inputData = GetMockBatteryInputData();
        Assert.NotNull(inputData);

        var dao = new EngineeringDataAdapter();
        var tmp = new MockBatteryInputData() {
            _electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(10.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            400.SI<Volt>(), 500.SI<Ampere>(), 0.03.SI<Ohm>()),
                        StringId = 0,
                        Count = 1
                    },
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(20.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            400.SI<Volt>(), 500.SI<Ampere>(), 0.02.SI<Ohm>()),
                        StringId = 1,
                        Count = 1
                    },
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(30.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            400.SI<Volt>(), 500.SI<Ampere>(), 0.01.SI<Ohm>()),
                        StringId = 2,
                        Count = 1
                    }

                }
        };
        var batteryData = dao.CreateBatteryData(tmp, 0.8);

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var modData = new MockModalDataContainer();
        bat.Initialize(initialSoC);

        var absTime = 0.SI<Second>();
        var dt = simInterval.SI<Second>();
        var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
        Assert.IsInstanceOf<RESSResponseSuccess>(response);
        bat.CommitSimulationStep(absTime, dt, modData);

        for (var i = 0; i < 3; i++) {
            Assert.AreEqual(expectedPowerDemand[i] * 1e3, ((SI)modData[ModalResultField.P_reess_terminal, i]).Value(), 1e-3);
        }

        var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
        Console.WriteLine(socs.Join());
        for (var i = 0; i < socs.Length; i++) {
            Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
        }

        Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
    }


	[
        TestCase(new[] { 0.48, 0.52, 0.5 }, 1, 60000, new[] { 10.463576, 19.337748, 30.198675 }, new[] { 0.480725217195313, 0.520669834354267, 0.500697728984787, 0.5040263454 }),
        TestCase(new[] { 0.48, 0.52, 0.5 }, 1, -60000, new[] { -9.536424, -20.662252, -29.801325 }, new[] { 0.479336559932017, 0.519280698083512, 0.499308864822235, 0.502637425094291 }),
        TestCase(new[] { 0.48, 0.52, 0.5 }, 10, 60000, new[] { 10.463576, 19.337748, 30.198675 }, new[] { 0.487252171953127, 0.526698343542669, 0.506977289847874, 0.510263454763681 }),
        TestCase(new[] { 0.48, 0.52, 0.5 }, 10, -60000, new[] { -9.536424, -20.662252, -29.801325 }, new[] { 0.473365599320166, 0.512806980835123, 0.493088648222347, 0.496374250942909 }),
    //TestCase(0.35, 1, 14000, new[] { 2.33333333, 4.666666666, 7 }, new[] { 0.350161966207795, 0.350161942625547, 0.350161966207795, 0.3501619583470 }),
    //TestCase(0.75, 1, -14500, new[] { -2.4166666666, -4.833333333, -7.25 }, new[] { 0.749832099811648, 0.749832074409512, 0.749832099811648, 0.74983209134426 })
    ]
    public void TestBatterySystemRequest_3Strings_1_Unbalanced(double[] initialSoC, double simInterval, double powerDemand, double[] expectedPowerDemand, double[] expectedSoC)
    {
        var dao = new EngineeringDataAdapter();
        var tmp = new MockBatteryInputData() {
            _electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack =GetSimpleMockBatteryInputData(10.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            400.SI<Volt>(), 500.SI<Ampere>(), 0.03.SI<Ohm>()),
                        StringId = 0,
                        Count = 1
                    },
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(20.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            400.SI<Volt>(), 500.SI<Ampere>(), 0.02.SI<Ohm>()),
                        StringId = 1,
                        Count = 1
                    },
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(30.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            400.SI<Volt>(), 500.SI<Ampere>(), 0.01.SI<Ohm>()),
                        StringId = 2,
                        Count = 1
                    }

                }
        };
        var batteryData = dao.CreateBatteryData(tmp, 0.8);

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var modData = new MockModalDataContainer();
        for (var i = 0; i < bat.Batteries.Count; i++) {
            foreach (var b in bat.Batteries[i].Batteries) {
                b.Initialize(initialSoC[i]);
            }
        }
        //bat.Initialize(initialSoC);

        var absTime = 0.SI<Second>();
        var dt = simInterval.SI<Second>();
        var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
        Assert.IsInstanceOf<RESSResponseSuccess>(response);
        bat.CommitSimulationStep(absTime, dt, modData);

        for (var i = 0; i < 3; i++) {
            Assert.AreEqual(expectedPowerDemand[i] * 1e3, ((SI)modData[ModalResultField.P_reess_terminal, i]).Value(), 1e-3);
        }

        var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
        Console.WriteLine(socs.Join());
        Console.WriteLine(bat.StateOfCharge);
        for (var i = 0; i < socs.Length; i++) {
            Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
        }

        Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
    }


    [
        TestCase(0.5, 1, 60000, new[] { 3.74922167, 6.25077833, 20, 30 }, new[] { 0.500693147221155, 0.500462098147437, 0.500692716960012, 0.500693147221155, 0.5006930038 }),
        TestCase(0.5, 1, -60000, new[] { -3.75078419, -6.24921581, -20, -30 }, new[] { 0.499304248566401, 0.499536165710934, 0.499303810709253, 0.499304248566401, 0.499304102614018 }),
    //TestCase(0.35, 1, 14000, new[] { 2.33333333, 4.666666666, 7 }, new[] { 0.350161966207795, 0.350161942625547, 0.350161966207795, 0.3501619583470 }),
    //TestCase(0.75, 1, -14500, new[] { -2.4166666666, -4.833333333, -7.25 }, new[] { 0.749832099811648, 0.749832074409512, 0.749832099811648, 0.74983209134426 })
    ]
    public void TestBatterySystemRequest_3Strings_2_Balanced(double initialSoC, double simInterval, double powerDemand, double[] expectedPowerDemand, double[] expectedSoC)
    {
        var dao = new EngineeringDataAdapter();
        var tmp = new MockBatteryInputData() {
            _electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(10.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            150.SI<Volt>(), 200.SI<Ampere>(), 0.01.SI<Ohm>()),
                        StringId = 0,
                        Count = 1
                    },
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(15.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            250.SI<Volt>(), 300.SI<Ampere>(), 0.02.SI<Ohm>()),
                        StringId = 0,
                        Count = 1
                    },
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(20.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            400.SI<Volt>(), 500.SI<Ampere>(), 0.02.SI<Ohm>()),
                        StringId = 1,
                        Count = 1
                    },
                    new MockBatteryInputData.MockElectricStorageInputWrapper() {
                        REESSPack = GetSimpleMockBatteryInputData(30.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            400.SI<Volt>(), 500.SI<Ampere>(), 0.01.SI<Ohm>()),
                        StringId = 2,
                        Count = 1
                    }

                }
        };
        var batteryData = dao.CreateBatteryData(tmp, 0.8);

        var container = new MockVehicleContainer();
        var bat = new BatterySystem(container, batteryData);
        var modData = new MockModalDataContainer();
        bat.Initialize(initialSoC);

        var absTime = 0.SI<Second>();
        var dt = simInterval.SI<Second>();
        var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
        Assert.IsInstanceOf<RESSResponseSuccess>(response);
        bat.CommitSimulationStep(absTime, dt, modData);

        for (var i = 0; i < 3; i++) {
            Assert.AreEqual(expectedPowerDemand[i] * 1e3, ((SI)modData[ModalResultField.P_reess_terminal, i]).Value(), 1e-3);
        }

        var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
        Console.WriteLine(socs.Join());
        Console.WriteLine(bat.StateOfCharge);
        for (var i = 0; i < socs.Length; i++) {
            Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
        }

        Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
    }

	private IBatteryPackDeclarationInputData GetMockBatteryInputData()
	{
		var bat = new Mock<IBatteryPackDeclarationInputData>();
		bat.Setup(b => b.MinSOC).Returns(0.2);
		bat.Setup(b => b.MaxSOC).Returns(0.8);
		bat.Setup(b => b.Capacity).Returns(7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>());
		bat.Setup(b => b.JunctionboxIncluded).Returns(true);
		bat.Setup(b => b.ConnectorsSubsystemsIncluded).Returns(true);
		bat.Setup(b => b.InternalResistanceCurve)
			.Returns(InputDataHelper.InputDataAsTableData("SoC, Ri", new[] { "0,  0.4986666", "100,  0.4986666" }));
		bat.Setup(b => b.VoltageCurve).Returns(InputDataHelper.InputDataAsTableData("SoC, V",
			new[] {
				"0, 590",
				"10, 614",
				"20, 626",
				"30, 634",
				"40, 638",
				"50, 640",
				"60, 640",
				"70, 642",
				"80, 646",
				"90, 650",
				"100, 658",
			}));
		bat.Setup(b => b.MaxCurrentMap).Returns(InputDataHelper.InputDataAsTableData("SOC, I_charge, I_discharge",
			new[] {
				"0, 375, 375",
				"50, 375, 375",
				"100, 375, 375",
			}));
		return bat.Object;
	}

    private IBatteryPackDeclarationInputData GetSimpleMockBatteryInputData(AmpereSecond capacity, Volt ocv, Ampere maxCur, Ohm intRes)
	{
		var bat = new Mock<IBatteryPackDeclarationInputData>();
		bat.Setup(b => b.MinSOC).Returns(0.2);
		bat.Setup(b => b.MaxSOC).Returns(0.8);
		bat.Setup(b => b.Capacity).Returns(capacity);
		bat.Setup(b => b.JunctionboxIncluded).Returns(true);
		bat.Setup(b => b.ConnectorsSubsystemsIncluded).Returns(true);
		bat.Setup(b => b.InternalResistanceCurve)
			.Returns(InputDataHelper.InputDataAsTableData("SoC, Ri",
				new[] {
					$"0,  {intRes.Value()}",
					$"100,   {intRes.Value()}"
				}));
		bat.Setup(b => b.VoltageCurve).Returns(InputDataHelper.InputDataAsTableData("SoC, V",
			new[] {
				$"0, {ocv.Value()}",
				$"100, {ocv.Value()}",
			}));
		bat.Setup(b => b.MaxCurrentMap).Returns(InputDataHelper.InputDataAsTableData("SOC, I_charge, I_discharge",
			new[] {
				$"0, {maxCur.Value()}, {maxCur.Value()}",
				$"100, {maxCur.Value()}, {maxCur.Value()}",
			}));
		return bat.Object;
	}
}