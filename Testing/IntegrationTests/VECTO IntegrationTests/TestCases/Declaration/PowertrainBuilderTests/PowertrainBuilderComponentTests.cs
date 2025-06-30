/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using Ninject;
using Ninject.Activation;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
    [TestFixture]
    //[Parallelizable(ParallelScope.All)]
    public class PowerTrainBuilderComponentsTest
    {
        private StandardKernel _kernel;
		private ISimplePowertrainBuilder _simplePowertrainBuilder;
		
        protected IPowertrainBuilder PowertrainBuilder;
       
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            _kernel = new StandardKernel(new VectoNinjectModule());
			_simplePowertrainBuilder = _kernel.Get<ISimplePowertrainBuilder>();
            _kernel.Rebind<ISimplePowertrainBuilder>().ToMethod(CreateInterceptSimplePowertrainBuilder).InThreadScope(); //.InSingletonScope();

            PowertrainBuilder = _kernel.Get<IPowertrainBuilder>();
		}

        [SetUp]
        public void RunBeforeEveryTest()
        {
            Mock.Get(_kernel.Get<ISimplePowertrainBuilder>()).Invocations.Clear();
        }

        private ISimplePowertrainBuilder GetSimplePowertrainBuilder()
        {
            return _kernel.Get<ISimplePowertrainBuilder>();
        }

        private ISimplePowertrainBuilder CreateInterceptSimplePowertrainBuilder(IContext arg)
        {
            var retVal = new Mock<ISimplePowertrainBuilder>();
            //var ptBuilder = new SimplePowertrainBuilder(_kernel.Get<IPowertrainComponentFactory>(), _kernel.Get<IShiftStrategyFactory>());
			retVal.Setup(b => b.CreateTestPowertrain(It.IsAny<IVehicleContainer>(), It.IsAny<bool>(),
					It.IsAny<VectoSimulationJobType>()))
				.Returns((IVehicleContainer container, bool createDriver, VectoSimulationJobType jobType) =>
					_simplePowertrainBuilder.CreateTestPowertrain(container, createDriver, jobType));
            retVal.Setup(b => b.CreateTestPowertrain(It.IsAny<IVehicleContainer>(), It.IsAny<bool>()))
                .Returns((IVehicleContainer container, bool createDriver) =>
                    _simplePowertrainBuilder.CreateTestPowertrain(container, createDriver));
            return retVal.Object;
        }

        // ================================

        protected enum TestPowertrainSource
        {
            Unknown,
            Driver,
            ShiftStrategySimplePowertrain,
            ShiftStrategyTestPowertrain,
            HybridStrategyTestPowertrain,
        }

        private ISimpleVehicleContainer FindSimpleVehicleContainerInShiftStrategy(IShiftStrategy strategy)
        {
            if (strategy == null) {
                return null;
            }
            var fields = strategy.GetType().GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            foreach (var field in fields) {
                if (field.GetValue(strategy) is ISimpleVehicleContainer testPt) {
                    return testPt;
                }
            }

            return null;
        }

		private ISimpleVehicleContainer FindSimpleVehicleContainerInDriver(IDriverInfo driver)
		{
			if (driver == null) {
				return null;
			}
			var fields = driver.GetType().GetFields(
				BindingFlags.NonPublic |
				BindingFlags.Instance);
			foreach (var field in fields) {
				if (field.GetValue(driver) is ITestPowertrain testPt) {
					return testPt.Container;
				}
			}

			return null;
		}

        private ISimpleVehicleContainer FindTestpowertrainSimpleVehicleContainerInShiftStrategy(IShiftStrategy strategy)
        {
            if (strategy == null) {
                return null;
            }
            var fields = strategy.GetType().GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            foreach (var field in fields) {
                var value = field.GetValue(strategy);
                if (value is ITestPowertrain testpt1) {
                    return testpt1.Container;
                }
			}

            return null;
        }

        private ISimpleVehicleContainer FindTestpowertrainSimpleVehicleContainerHybridStrategy(IHybridControlStrategy ctl)
        {
            if (ctl == null) {
                return null;
            }
            var fields = ctl.GetType().GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            foreach (var field in fields) {
                var value = field.GetValue(ctl);
                if (value is ITestPowertrain testpt1) {
                    return testpt1.Container;
                }
			}

            return null;
        }


        private IHybridControlStrategy GetHybridStrategy(IHybridControllerInfo ctl)
        {
            if (ctl == null) {
                return null;
            }
            FieldInfo[] fields = ctl.GetType().GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            foreach (var field in fields) {
                if (field.GetValue(ctl) is IHybridControlStrategy strategy) {
                    return strategy;
                }
            }

            return null;
        }

        private IShiftStrategy GetShiftStrategy(IGearboxInfo gbx)
        {
            if (gbx == null) {
                return null;
            }

            return gbx switch {
				AbstractAMTGearbox gearbox => gearbox._strategy,
                APTGearbox atgearbox => atgearbox._strategy,
                CycleGearbox cycleGbx => cycleGbx.Strategy,
                _ => throw new ArgumentException($"unhandled gearbox type {gbx.GetType()}")
            };
        }

        private Dictionary<TestPowertrainSource, ISimpleVehicleContainer> GetSimpleVehicleContainer(IVehicleContainer vehicleContainer)
        {
            var retVal = new Dictionary<TestPowertrainSource, ISimpleVehicleContainer> {
				{ TestPowertrainSource.Driver , FindSimpleVehicleContainerInDriver(vehicleContainer.DriverInfo)},
                {TestPowertrainSource.ShiftStrategySimplePowertrain, FindSimpleVehicleContainerInShiftStrategy(GetShiftStrategy(vehicleContainer.GearboxInfo))},
                {TestPowertrainSource.ShiftStrategyTestPowertrain, FindTestpowertrainSimpleVehicleContainerInShiftStrategy(GetShiftStrategy(vehicleContainer.GearboxInfo))},
                {TestPowertrainSource.HybridStrategyTestPowertrain, FindTestpowertrainSimpleVehicleContainerHybridStrategy(GetHybridStrategy(vehicleContainer.HybridControllerInfo))}
            };
            return retVal.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
        }


		private IModalDataContainer GetMockModalDataContainer()
		{
			return new Mock<IModalDataContainer>().Object;
		}

		private ISumData GetMockSumWriter()
		{
			return new Mock<ISumData>().Object;
		}

        protected void AssertPowertrainComponents(IVehicleContainer container, Type vehicleContainer, Type drivingCycle,
            Type driver, Type vehicle, Type wheels, Type brakes, Type axlegear, Type angledrive, Type retarder,
            Type gearbox,
            Type shiftStrategyT, Type clutch, Type engine, Type hybridControllerT, Type hybridStrategyT,
            Type torqueConverter)
        {
            Assert.Multiple(() => {
                if (vehicleContainer != null) {
                    Assert.IsInstanceOf(vehicleContainer, container, "VehicleContainer");
                } else {
                    Assert.IsNull(container, "VehicleContainer");
                    return;
                }

                if (drivingCycle != null) {
                    Assert.IsInstanceOf(drivingCycle, container.DrivingCycleInfo, "DrivingCycle");
                } else {
                    Assert.IsNull(container.DrivingCycleInfo, "DrivingCycle");
                }

                if (driver != null) {
                    Assert.IsInstanceOf(driver, container.DriverInfo, "Driver");
                } else {
                    Assert.IsNull(container.DriverInfo, "Driver");
                }

                if (vehicle != null) {
                    Assert.IsInstanceOf(vehicle, container.VehicleInfo, "Vehicle");
                } else {
                    Assert.IsNull(container.VehicleInfo, "Vehicle");
                }

                if (wheels != null) {
                    Assert.IsInstanceOf(wheels, container.WheelsInfo, "Wheels");
                } else {
                    Assert.IsNull(container.WheelsInfo, "Wheels");
                }

                if (brakes != null) {
                    Assert.IsInstanceOf(brakes, container.Brakes, "Brakes");
                } else {
                    Assert.IsNull(container.Brakes, "Brakes");
                }

                if (axlegear != null) {
                    Assert.IsInstanceOf(axlegear, container.AxlegearInfo, "Axlegear");
                } else {
                    Assert.IsNull(container.AxlegearInfo, "Axlegear");
                }

                if (angledrive != null) {
                    //Assert.IsInstanceOf(angledrive, container.);
                } else {
                    //Assert.IsNull(drivingCycle, "DrivingCycle");
                }

                if (retarder != null) {
                    //Assert.IsInstanceOf(driver, container.DriverInfo);
                } else {
                    //Assert.IsNull(drivingCycle, "DrivingCycle");
                }

                if (gearbox != null) {
                    Assert.IsInstanceOf(gearbox, container.GearboxInfo, "Gearbox");
                    var strategy = GetShiftStrategy(container.GearboxInfo);
                    if (shiftStrategyT != null) {
                        Assert.IsInstanceOf(shiftStrategyT, strategy, "ShiftStrategy");
                    } else {
                        Assert.IsNull(strategy, "ShiftStrategy");
                    }
                } else {
                    Assert.IsNull(container.GearboxInfo, "Gearbox");
                }

                if (torqueConverter != null) {
                    Assert.IsInstanceOf(torqueConverter, container.TorqueConverterInfo);
                } else {
                    Assert.IsNull(container.TorqueConverterInfo);
                }
                if (clutch != null) {
                    Assert.IsInstanceOf(clutch, container.ClutchInfo, "Clutch");
                } else {
                    Assert.IsNull(container.ClutchInfo, "Clutch");
                }

                if (engine != null) {
                    Assert.IsInstanceOf(engine, container.EngineInfo, "Engine");
                } else {
                    Assert.IsNull(container.EngineInfo, "Engine");
                }

                if (hybridControllerT != null) {
                    Assert.IsInstanceOf(hybridControllerT, container.HybridControllerInfo, "HybridController");
                    var strategy = GetHybridStrategy(container.HybridControllerInfo);
                    if (hybridStrategyT != null) {
                        Assert.IsInstanceOf(hybridStrategyT, strategy, "HybridStrategy");
                    } else {
                        Assert.IsNull(strategy, "HybridStrategy");
                    }
                } else {
                    Assert.IsNull(container.HybridControllerInfo, "HybridController");
                }
            });
        }

        protected void AssertTestpowertrains(bool expectTestPT,
            Dictionary<TestPowertrainSource, ISimpleVehicleContainer> simpleVehicleContainers)
        {
            Assert.IsFalse(simpleVehicleContainers.ContainsKey(TestPowertrainSource.ShiftStrategySimplePowertrain));
            if (expectTestPT) {
                if (simpleVehicleContainers.Count == 0) {
                    Assert.Fail("Testpowertrain expected, but didn't find one");
                } else {
                    var sptb = Mock.Get(GetSimplePowertrainBuilder());
                    Assert.IsTrue(sptb.Invocations.Count > 0);
                    foreach (var invocation in sptb.Invocations) {
                        TestContext.WriteLine(invocation.ToString());
                    }
                }
            } else {
                var sptb = Mock.Get(GetSimplePowertrainBuilder());
                sptb.VerifyNoOtherCalls();
            }
        }

        private static Type VehicleContainerT = typeof(VehicleContainer);
        private static Type DistanceBasedDrivingCycleT = typeof(DistanceBasedDrivingCycle);
        private static Type MeasuredspeedDrivingCycleT = typeof(MeasuredSpeedDrivingCycle);
        private static Type VTPCycleT = typeof(VTPCycle);
        private static Type PWheelCycleT = typeof(PWheelCycle);
        private static Type DriverT = typeof(Driver);
        private static Type VehicleT = typeof(Vehicle);
        private static Type WheelsT = typeof(Wheels);
        private static Type BrakesT = typeof(Brakes);
        private static Type AxleGearT = typeof(AxleGear);
        private static Type MTGearboxT = typeof(MTGearbox);
        private static Type AMTGearboxT = typeof(AMTGearbox);
        private static Type APTSGearboxT = typeof(APTGearbox);
        private static Type APTPGearboxT = typeof(APTGearbox);
        private static Type APTNGearboxT = typeof(APTNGearbox);
        private static Type IEPCGearboxT = typeof(IEPCGearboxMultipleGears);
        private static Type PEVGearboxT = typeof(PEVGearbox);
        private static Type CycleGearboxT = typeof(CycleGearbox);
        private static Type VTPGearboxT = typeof(VTPGearbox);
        private static Type TorqueConverterT = typeof(TorqueConverter);
        private static Type ClutchT = typeof(Clutch);
        private static Type ATClutchInfoT = typeof(ATClutchInfo);
        private static Type EngineT = typeof(StopStartCombustionEngine);
        private static Type DummyEngineT = typeof(DummyEngineInfo);

        private static Type MTShiftStrategyT = typeof(MTShiftStrategy);
        private static Type AMTShiftStrategyT = typeof(AMTShiftStrategyOptimized);
        private static Type ATShiftStrategyT = typeof(ATShiftStrategyOptimized);
        private static Type PEVShiftStrategyT = typeof(PEVAMTShiftStrategy);
        private static Type HybridShiftStrategyT = typeof(HybridCtlShiftStrategy);
        private static Type HybridATShiftStrategyT = typeof(HybridCtlATShiftStrategy);
        private static Type HybridIHPCShiftStrategyT = typeof(HybridCtlIHPCShiftStrategy);
		private static Type MeasuredSpeedHybridsGearboxT = typeof(MeasuredSpeedHybridsGearbox); 
		private static Type MeasuredSpeedHybridCycleGearboxT = typeof(MeasuredSpeedHybridsCycleGearbox);

        private static Type HybridCtlT = typeof(HybridController);
        private static Type SerialHybridCtl = typeof(SerialHybridController);

        private static Type HybridStrategyT = typeof(HybridStrategy);
        private static Type HybridStrategyATT = typeof(HybridStrategyAT);
        private static Type MeasuredSpeedGearHybridStrategyT = typeof(MeasuredSpeedGearHybridStrategy);
        private static Type MeasuredSpeedGearATHybridStrategyT = typeof(MeasuredSpeedGearATHybridStrategy);


        private static Type SerialHybridStrategyT = typeof(SerialHybridStrategy);
        private static Type SerialHybridATStrategyT = typeof(SerialHybridStrategyAT);

        private static Type TestVehicleContainerT = typeof(SimplePowertrainContainer);
        private static Type TestDistanceBasedDrivingCycleT = typeof(DistanceBasedDrivingCycle);
        private static Type TestMeasuredspeedDrivingCycleT = typeof(MeasuredSpeedDrivingCycle);
        private static Type TestPowertrainCycleT = typeof(MockDrivingCycle);
        private static Type TestDriverSimpleContT = typeof(SimplePowertrainContainer);
        private static Type TestDriverTestPTT = typeof(MockDriver);
        private static Type TestDriverT = typeof(Driver);
        private static Type TestVehicleT = typeof(Vehicle);
        private static Type TestWheelsT = typeof(Wheels);
        private static Type TestBrakesT = typeof(Brakes);
        private static Type TestAxleGearT = typeof(AxleGear);
        private static Type TestMTGearboxT = typeof(TestPowertrainGearbox);
        private static Type TestAMTGearboxT = typeof(TestPowertrainGearbox);
        private static Type TestAPTSGearboxT = typeof(TestPowertrainAPTGearbox);
        private static Type TestAPTPGearboxT = typeof(TestPowertrainAPTGearbox);
        private static Type TestAPTNGearboxT = typeof(TestPowertrainAPTNGearbox);
		private static Type TestIEPCGearboxMultipleGearsT = typeof(TestpowertrainIEPCGearboxMultipleGears);
		private static Type TestMeasuredSpeedHybridsGearboxT = typeof(MeasuredSpeedHybridsGearbox);
        private static Type TestMeasuredSpeedHybridsCycleGearboxT = typeof(MeasuredSpeedHybridsCycleGearbox);

        private static Type TestEngineT = typeof(StopStartCombustionEngine);
        private static Type TestDummyEngineT = typeof(DummyEngineInfo);

        private static Type TestHybridCtl = typeof(SimpleHybridController);

        // - - - - - - - - - - - - - - - - - - - - - - - - -

        #region Conventional PT, distance based
        // ############################
        //   Conventional PT, distance based

        public static object[] DistanceBased_Conventional_Source =
        {
            new object[] {"Dist Conv MT", CycleType.DistanceBased, GearboxType.MT, MTGearboxT, null, MTShiftStrategyT, ClutchT},
            new object[] {"Dist Conv AMT", CycleType.DistanceBased, GearboxType.AMT, AMTGearboxT, null, AMTShiftStrategyT, ClutchT},
            new object[] {"Dist Conv APT-S", CycleType.DistanceBased, GearboxType.ATSerial, APTSGearboxT, TorqueConverterT, ATShiftStrategyT, ATClutchInfoT},
            new object[] {"Dist Conv APT-P", CycleType.DistanceBased, GearboxType.ATPowerSplit, APTPGearboxT, TorqueConverterT, ATShiftStrategyT, ATClutchInfoT},

        };

        [TestCaseSource(nameof(DistanceBased_Conventional_Source))]
        public void TestPowertrainBuilder_Components_Distance_Conventional(string nuame, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT, BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, null, null, expectedTorqueConverterT);

        }


		public static object[] DistanceBased_Conventional_TestPT_Source =
        {
            new object[] {"Dist Conv MT", true, CycleType.DistanceBased, GearboxType.MT, TestMTGearboxT, null, null, ClutchT},
            new object[] {"Dist Conv AMT", true, CycleType.DistanceBased, GearboxType.AMT, TestAMTGearboxT, null, null, ClutchT},
            new object[] {"Dist Conv APT-S", true, CycleType.DistanceBased, GearboxType.ATSerial, TestAPTSGearboxT, TorqueConverterT, null, ATClutchInfoT},
            new object[] {"Dist Conv APT-P", true, CycleType.DistanceBased, GearboxType.ATPowerSplit, TestAPTPGearboxT, TorqueConverterT, null, ATClutchInfoT},

        };

        [TestCaseSource(nameof(DistanceBased_Conventional_TestPT_Source))]
        public void TestPowertrainBuilder_Components_Distance_Conventional_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
			AssertTestpowertrains(expectTestPT, testpowertrains);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, TestVehicleContainerT, TestDistanceBasedDrivingCycleT, TestDriverSimpleContT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverSimpleContT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
		}

        #endregion

        #region Conventional PT, measured speed
        // ############################
        //   Conventional PT, distance based

        public static object[] MeasuredSpeed_Conventional_Source =
        {
            new object[] { "MeasuredSpeed Conv MT", CycleType.MeasuredSpeed, GearboxType.MT, MTGearboxT, null, MTShiftStrategyT, ClutchT},
            new object[] { "MeasuredSpeed Conv AMT", CycleType.MeasuredSpeed, GearboxType.AMT, AMTGearboxT, null, AMTShiftStrategyT, ClutchT},
            new object[] { "MeasuredSpeed Conv APT-S", CycleType.MeasuredSpeed, GearboxType.ATSerial, APTSGearboxT, TorqueConverterT, ATShiftStrategyT, ATClutchInfoT},
            new object[] { "MeasuredSpeed Conv APT-P", CycleType.MeasuredSpeed, GearboxType.ATPowerSplit, APTPGearboxT, TorqueConverterT, ATShiftStrategyT, ATClutchInfoT},

        };

        [TestCaseSource(nameof(MeasuredSpeed_Conventional_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_Conventional(string nuame, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, MeasuredspeedDrivingCycleT, MeasuredspeedDrivingCycleT, VehicleT, WheelsT, BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, null, null, expectedTorqueConverterT);

        }

        public static object[] MeasuredSpeed_Conventional_TestPT_Source =
        {
            new object[] { "MeasuredSpeed Conv MT", true, CycleType.MeasuredSpeed, GearboxType.MT, TestMTGearboxT, null, null, ClutchT},
            new object[] { "MeasuredSpeed Conv AMT", true, CycleType.MeasuredSpeed, GearboxType.AMT, TestAMTGearboxT, null, null, ClutchT},
            new object[] { "MeasuredSpeed Conv APT-S", true, CycleType.MeasuredSpeed, GearboxType.ATSerial, TestAPTSGearboxT, TorqueConverterT, null, ATClutchInfoT},
            new object[] { "MeasuredSpeed Conv APT-P", true, CycleType.MeasuredSpeed, GearboxType.ATPowerSplit, TestAPTPGearboxT, TorqueConverterT, null, ATClutchInfoT},

        };

        [TestCaseSource(nameof(MeasuredSpeed_Conventional_TestPT_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_Conventional_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
			AssertTestpowertrains(expectTestPT, testpowertrains);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, TestVehicleContainerT, MeasuredspeedDrivingCycleT, MeasuredspeedDrivingCycleT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, MeasuredspeedDrivingCycleT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
		}

        #endregion

        #region Conventional PT, measured speed gear
        // ############################
        //   Conventional PT, distance based

        public static object[] MeasuredSpeedGear_Conventional_Source =
        {
            new object[] { "MeasuredSpeedGear Conv MT", CycleType.MeasuredSpeedGear, GearboxType.MT, CycleGearboxT, null, null, ATClutchInfoT},
            new object[] { "MeasuredSpeedGear Conv AMT", CycleType.MeasuredSpeedGear, GearboxType.AMT, CycleGearboxT, null, null, ATClutchInfoT},
            new object[] { "MeasuredSpeedGear Conv APT-S", CycleType.MeasuredSpeedGear, GearboxType.ATSerial, CycleGearboxT, TorqueConverterT, null, ATClutchInfoT},
            new object[] { "MeasuredSpeedGear Conv APT-P", CycleType.MeasuredSpeedGear, GearboxType.ATPowerSplit, CycleGearboxT, TorqueConverterT, null, ATClutchInfoT},

        };

        [TestCaseSource(nameof(MeasuredSpeedGear_Conventional_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeedGear_Conventional(string nuame, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, MeasuredspeedDrivingCycleT, MeasuredspeedDrivingCycleT, VehicleT, WheelsT, BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, null, null, expectedTorqueConverterT);

        }

        public static object[] MeasuredSpeedGear_Conventional_TestPT_Source =
        {
            new object[] { "MeasuredSpeedGear Conv MT", false, CycleType.MeasuredSpeedGear, GearboxType.MT, null, null, null},
            new object[] { "MeasuredSpeedGear Conv AMT", false, CycleType.MeasuredSpeedGear, GearboxType.AMT, null, null, null},
            new object[] { "MeasuredSpeedGear Conv APT-S", false, CycleType.MeasuredSpeedGear, GearboxType.ATSerial, null, null, null},
            new object[] { "MeasuredSpeedGear Conv APT-P", false, CycleType.MeasuredSpeedGear, GearboxType.ATPowerSplit, null, null, null},

        };


        [TestCaseSource(nameof(MeasuredSpeedGear_Conventional_TestPT_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeedGear_Conventional_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }

        #endregion

        #region Conventional PT, PWheel
        // ############################
        //   Conventional PT, distance based

        public static object[] PWheel_Conventional_Source =
        {
            new object[] { "PWheel Conv MT", CycleType.PWheel, GearboxType.MT, CycleGearboxT, null, null, ClutchT},
            new object[] { "PWheel Conv AMT", CycleType.PWheel, GearboxType.AMT, CycleGearboxT, null, null, ClutchT},
            new object[] { "PWheel Conv APT-S", CycleType.PWheel, GearboxType.ATSerial, CycleGearboxT, TorqueConverterT, null, ClutchT},
            new object[] { "PWheel Conv APT-P", CycleType.PWheel, GearboxType.ATPowerSplit, CycleGearboxT, TorqueConverterT, null, ClutchT},

        };

        [TestCaseSource(nameof(PWheel_Conventional_Source))]
        public void TestPowertrainBuilder_Components_PWheel_Conventional(string nuame, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer(), GetMockSumWriter());

            AssertPowertrainComponents(pt, VehicleContainerT, PWheelCycleT, PWheelCycleT, PWheelCycleT, null, null, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, null, null, expectedTorqueConverterT);

        }

		public static object[] PWheel_Conventional_TestPT_Source = {
            new object[] { "PWheel Conv MT", false, CycleType.PWheel, GearboxType.MT, null, null, null, null },
            new object[] { "PWheel Conv AMT", false, CycleType.PWheel, GearboxType.AMT, null, null, null, null },
            new object[] { "PWheel Conv APT-S", false, CycleType.PWheel, GearboxType.ATSerial, null, null, null, null },
            new object[] { "PWheel Conv APT-P", false, CycleType.PWheel, GearboxType.ATPowerSplit, null, null, null, null },
        };


        [TestCaseSource(nameof(PWheel_Conventional_TestPT_Source))]
        public void TestPowertrainBuilder_Components_PWheel_Conventional_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer(), GetMockSumWriter());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }

        #endregion

        #region Conventional PT, VTP
        // ############################
        //   Conventional PT, distance based

        public static object[] VTP_Conventional_Source =
        {
            new object[] { "VTP Conv MT", CycleType.VTP, GearboxType.MT, VTPGearboxT, null, null, ClutchT},
            new object[] { "VTP Conv AMT", CycleType.VTP, GearboxType.AMT, VTPGearboxT, null, null, ClutchT},
            new object[] { "VTP Conv APT-S", CycleType.VTP, GearboxType.ATSerial, VTPGearboxT, TorqueConverterT, null, ClutchT},
            new object[] { "VTP Conv APT-P", CycleType.VTP, GearboxType.ATPowerSplit, VTPGearboxT, TorqueConverterT, null, ClutchT},

        };

        [TestCaseSource(nameof(VTP_Conventional_Source))]
        public void TestPowertrainBuilder_Components_VTP_Conventional(string nuame, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, VTPCycleT, VTPCycleT, VTPCycleT, null, null, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, null, null, expectedTorqueConverterT);

        }

        public static object[] VTP_Conventional_TestPT_Source =
        {
            new object[] { "VTP Conv MT", false, CycleType.VTP, GearboxType.MT, null, null, null,null},
            new object[] { "VTP Conv AMT", false, CycleType.VTP, GearboxType.AMT, null, null, null, null},
            new object[] { "VTP Conv APT-S", false, CycleType.VTP, GearboxType.ATSerial, TorqueConverterT, null, null, null },
            new object[] { "VTP Conv APT-P", false, CycleType.VTP, GearboxType.ATPowerSplit, TorqueConverterT, null, null, null },

        };


        [TestCaseSource(nameof(VTP_Conventional_TestPT_Source))]
        public void TestPowertrainBuilder_Components_VTP_Conventional_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ConventionalVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - -

        #region Parallel Hybrid PT, distance based
        // ############################
        //   Conventional PT, distance based

        public static object[] DistanceBased_PHEV_Source =
        {
            //new object[] {"Dist PHEV MT", CycleType.DistanceBased, GearboxType.MT, MTGearboxT, ClutchT},
            new object[] {"Dist PHEV AMT", CycleType.DistanceBased, GearboxType.AMT, AMTGearboxT, null, HybridShiftStrategyT, ClutchT, HybridCtlT, HybridStrategyT},
            new object[] {"Dist PHEV APT-S", CycleType.DistanceBased, GearboxType.ATSerial, APTSGearboxT, TorqueConverterT, HybridATShiftStrategyT, ATClutchInfoT, HybridCtlT, HybridStrategyATT},
            new object[] { "Dist PHEV APT-P", CycleType.DistanceBased, GearboxType.ATPowerSplit, APTPGearboxT, TorqueConverterT, HybridATShiftStrategyT, ATClutchInfoT, HybridCtlT, HybridStrategyATT},
        };

        [TestCaseSource(nameof(DistanceBased_PHEV_Source))]
        public void TestPowertrainBuilder_Components_Distance_PHEV(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ParallelHybridVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] DistanceBased_PHEV_TestPT_Source =
        {
            //new object[] {"Dist PHEV MT", true, CycleType.DistanceBased, GearboxType.MT, MTGearboxT, ClutchT},
            new object[] {"Dist PHEV AMT", true, CycleType.DistanceBased, GearboxType.AMT, TestAMTGearboxT, null, null, ClutchT, TestHybridCtl, null},
            new object[] {"Dist PHEV APT-S", true, CycleType.DistanceBased, GearboxType.ATSerial, TestAPTSGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
            new object[] { "Dist PHEV APT-P", true, CycleType.DistanceBased, GearboxType.ATPowerSplit, TestAPTPGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},

        };

        [TestCaseSource(nameof(DistanceBased_PHEV_TestPT_Source))]
        public void TestPowertrainBuilder_Components_Distance_PHEV_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ParallelHybridVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }



        #endregion

        #region Parallel Hybrid PT, measured speed
        // ############################
        //   Conventional PT, distance based

        public static object[] MeasuredSpeed_PHEV_Source =
        {
            //new object[] {"Dist PHEV MT", CycleType.DistanceBased, GearboxType.MT, MTGearboxT, ClutchT},
            new object[] { "MeasuredSpeed PHEV AMT", CycleType.MeasuredSpeed, GearboxType.AMT, MeasuredSpeedHybridsGearboxT, null, HybridShiftStrategyT, ClutchT, HybridCtlT, HybridStrategyT},
            new object[] { "MeasuredSpeed PHEV APT-S", CycleType.MeasuredSpeed, GearboxType.ATSerial, APTSGearboxT, TorqueConverterT, HybridATShiftStrategyT, ATClutchInfoT, HybridCtlT, HybridStrategyATT},
            new object[] { "MeasuredSpeed PHEV APT-P", CycleType.MeasuredSpeed, GearboxType.ATPowerSplit, APTPGearboxT, TorqueConverterT, HybridATShiftStrategyT, ATClutchInfoT, HybridCtlT, HybridStrategyATT},
        };

        [TestCaseSource(nameof(MeasuredSpeed_PHEV_Source))]
        public void TestPowertrainBuilder_ComponentsMeasuredSpeed_PHEV(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ParallelHybridVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, MeasuredspeedDrivingCycleT, MeasuredspeedDrivingCycleT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] MeasuredSpeed_PHEV_TestPT_Source =
        {
            //new object[] {"Dist PHEV MT", true, CycleType.DistanceBased, GearboxType.MT, MTGearboxT, ClutchT},
            new object[] { "MeasuredSpeed PHEV AMT", true, CycleType.MeasuredSpeed, GearboxType.AMT, TestMeasuredSpeedHybridsGearboxT, null, null, ClutchT, TestHybridCtl, null},
            new object[] { "MeasuredSpeed PHEV APT-S", true, CycleType.MeasuredSpeed, GearboxType.ATSerial, TestAPTSGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
            new object[] { "MeasuredSpeed PHEV APT-P", true, CycleType.MeasuredSpeed, GearboxType.ATPowerSplit, TestAPTPGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},

        };

        [TestCaseSource(nameof(MeasuredSpeed_PHEV_TestPT_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_PHEV_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ParallelHybridVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }

        #endregion

        #region Parallel Hybrid PT, measured speed gear
        // ############################
        //   Conventional PT, distance based

        public static object[] MeasuredSpeedGear_PHEV_Source =
        {
            //new object[] {"Dist PHEV MT", CycleType.DistanceBased, GearboxType.MT, MTGearboxT, ClutchT},
            new object[] { "MeasuredSpeedGear PHEV AMT", CycleType.MeasuredSpeedGear, GearboxType.AMT, MeasuredSpeedHybridCycleGearboxT, null, null, ClutchT, HybridCtlT, MeasuredSpeedGearHybridStrategyT},
            new object[] { "MeasuredSpeedGear PHEV APT-S", CycleType.MeasuredSpeedGear, GearboxType.ATSerial, MeasuredSpeedHybridCycleGearboxT, TorqueConverterT, null, ATClutchInfoT, HybridCtlT, MeasuredSpeedGearATHybridStrategyT},
            new object[] { "MeasuredSpeedGear PHEV APT-P", CycleType.MeasuredSpeedGear, GearboxType.ATPowerSplit, MeasuredSpeedHybridCycleGearboxT, TorqueConverterT, null, ATClutchInfoT, HybridCtlT, MeasuredSpeedGearATHybridStrategyT},
        };

        [TestCaseSource(nameof(MeasuredSpeedGear_PHEV_Source))]
        public void TestPowertrainBuilder_ComponentsMeasuredSpeedGear_PHEV(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ParallelHybridVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, MeasuredspeedDrivingCycleT, MeasuredspeedDrivingCycleT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] MeasuredSpeedGear_PHEV_TestPT_Source =
        {
            //new object[] {"Dist PHEV MT", true, CycleType.DistanceBased, GearboxType.MT, MTGearboxT, ClutchT},
            new object[] { "MeasuredSpeedGear PHEV AMT", true, CycleType.MeasuredSpeedGear, GearboxType.AMT, TestMeasuredSpeedHybridsCycleGearboxT, null, null, ClutchT, TestHybridCtl, null},
            new object[] { "MeasuredSpeedGear PHEV APT-S", true, CycleType.MeasuredSpeedGear, GearboxType.ATSerial, TestMeasuredSpeedHybridsCycleGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
            new object[] { "MeasuredSpeedGear PHEV APT-P", true, CycleType.MeasuredSpeedGear, GearboxType.ATPowerSplit, TestMeasuredSpeedHybridsCycleGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},

        };

        [TestCaseSource(nameof(MeasuredSpeedGear_PHEV_TestPT_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeedGear_PHEV_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.ParallelHybridVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }



        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - -

        #region IHPC Hybrid PT, distance based
        // ############################
        //   Conventional PT, distance based

        public static object[] DistanceBased_IHPC_Source =
        {
            new object[] {"Dist PHEV AMT", CycleType.DistanceBased, GearboxType.IHPC, APTNGearboxT, null, HybridIHPCShiftStrategyT, ClutchT, HybridCtlT, HybridStrategyT},
        };

        [TestCaseSource(nameof(DistanceBased_IHPC_Source))]
        public void TestPowertrainBuilder_Components_Distance_IHPC(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IHPC, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] DistanceBased_IHPC_TestPT_Source =
        {
            new object[] {"Dist PHEV AMT", true, CycleType.DistanceBased, GearboxType.IHPC, TestAMTGearboxT, null, null, ClutchT, TestHybridCtl, null},
        };

        [TestCaseSource(nameof(DistanceBased_IHPC_TestPT_Source))]
        public void TestPowertrainBuilder_Components_Distance_IHPC_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IHPC, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
			AssertTestpowertrains(expectTestPT, testpowertrains);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }

            
        }



        #endregion

        #region IHPC Hybrid PT, measrued speed
        // ############################
        //   Conventional PT, distance based

        public static object[] MeasuredSpeed_IHPC_Source =
        {
            new object[] { "MeasuredSpeed PHEV AMT", CycleType.MeasuredSpeed, GearboxType.IHPC, APTNGearboxT, null, HybridIHPCShiftStrategyT, ClutchT, HybridCtlT, HybridStrategyT},
        };

        [TestCaseSource(nameof(MeasuredSpeed_IHPC_Source)),
        Ignore("currently not supported")]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_IHPC(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IHPC, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] MeasuredSpeed_IHPC_TestPT_Source =
        {
            new object[] { "MeasuredSpeed PHEV AMT", true, CycleType.MeasuredSpeed, GearboxType.IHPC, TestAPTNGearboxT, null, null, ClutchT, TestHybridCtl, null},
        };

        [TestCaseSource(nameof(MeasuredSpeed_IHPC_TestPT_Source)),
        Ignore("currently not supported")]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_IHPC_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IHPC, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }



        #endregion

        #region IHPC Hybrid PT, measrued speed
        // ############################
        //   Conventional PT, distance based

        public static object[] MeasuredSpeedGear_IHPC_Source =
        {
            new object[] { "MeasuredSpeed PHEV AMT", CycleType.MeasuredSpeed, GearboxType.IHPC, APTNGearboxT, null, HybridIHPCShiftStrategyT, ClutchT, HybridCtlT, HybridStrategyT},
        };

        [TestCaseSource(nameof(MeasuredSpeedGear_IHPC_Source)),]
        [Ignore("currently not supported")]
        public void TestPowertrainBuilder_Components_MeasuredSpeedGear_IHPC(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IHPC, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] MeasuredSpeedGear_IHPC_TestPT_Source =
        {
            new object[] { "MeasuredSpeed PHEV AMT", true, CycleType.MeasuredSpeed, GearboxType.IHPC, TestAPTNGearboxT, null, null, ClutchT, TestHybridCtl, null},
        };

        [TestCaseSource(nameof(MeasuredSpeedGear_IHPC_TestPT_Source)),]
        [Ignore("currently not supported")]
        public void TestPowertrainBuilder_Components_MeasuredSpeedGear_IHPC_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IHPC, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }



        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - -

        #region Serial Hybrid PT, distance based
        // ############################
        //   Conventional PT, distance based

        public static object[] DistanceBased_SHEV_Source =
        {
            new object[] { "Dist SHEV AMT", CycleType.DistanceBased, GearboxType.AMT, PEVGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridStrategyT},
			// APT-S and APT-P are mapped to APT-N in DataAdapter!
			//new object[] { "Dist SHEV APT-S", CycleType.DistanceBased, GearboxType.ATSerial, APTSGearboxT, TorqueConverterT, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridATStrategyT},
			//new object[] { "Dist SHEV APT-P", CycleType.DistanceBased, GearboxType.ATPowerSplit, APTPGearboxT, TorqueConverterT, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridATStrategyT},
			new object[] { "Dist SHEV APT-N", CycleType.DistanceBased, GearboxType.APTN, APTNGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridATStrategyT},
        };

        [TestCaseSource(nameof(DistanceBased_SHEV_Source))]
        public void TestPowertrainBuilder_Components_Distance_SHEV(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.SerialHybridVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] DistanceBased_SHEV_TestPT_Source =
        {
            new object[] { "Dist SHEV AMT", true, CycleType.DistanceBased, GearboxType.AMT, TestAMTGearboxT, null, null, null, TestHybridCtl, null},
			// APT-S and APT-P are mapped to APT-N in DataAdapter!
			//new object[] { "Dist SHEV APT-S", true, CycleType.DistanceBased, GearboxType.ATSerial, TestAPTSGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
			//new object[] { "Dist SHEV APT-P", true, CycleType.DistanceBased, GearboxType.ATPowerSplit, TestAPTPGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
			new object[] { "Dist SHEV APT-N", true, CycleType.DistanceBased, GearboxType.APTN, TestAPTNGearboxT, null, null, null, TestHybridCtl, null},
        };

        [TestCaseSource(nameof(DistanceBased_SHEV_TestPT_Source))]
        public void TestPowertrainBuilder_Components_Distance_SHEV_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.SerialHybridVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, TestVehicleContainerT, TestDistanceBasedDrivingCycleT, TestDriverSimpleContT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, TestDummyEngineT, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }

        #endregion


        // - - - - - - - - - - - - - - - - - - - - - - - - -

        #region IEPC-S Hybrid PT, distance based
        // ############################
        //   Conventional PT, distance based

        public static object[] DistanceBased_IEPCS_Source =
        {
            new object[] { "Dist IEPC-S", CycleType.DistanceBased, GearboxType.APTN, IEPCGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridStrategyT},
        };

        [TestCaseSource(nameof(DistanceBased_IEPCS_Source))]
        public void TestPowertrainBuilder_Components_Distance_IEPCS(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IEPC_S, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, EngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] DistanceBased_IEPCS_TestPT_Source =
        {
            new object[] { "Dist IEPC-S", true, CycleType.DistanceBased, GearboxType.APTN, TestIEPCGearboxMultipleGearsT, null, null, null, TestHybridCtl, null},
        };

        [TestCaseSource(nameof(DistanceBased_IEPCS_TestPT_Source))]
        public void TestPowertrainBuilder_Components_Distance_IEPCS_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IEPC_S, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, TestVehicleContainerT, TestDistanceBasedDrivingCycleT, TestDriverSimpleContT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, expectedHybridController, expectedHybridStrategy, expectedTorqueConverterT);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - -

        #region Battery Electric PT, distance based
        // ############################
        //   Conventional PT, distance based

        public static object[] DistanceBased_PEV_Source =
        {
            new object[] { "Dist PEV AMT", CycleType.DistanceBased, GearboxType.AMT, PEVGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, null, null},
			// APT-S and APT-P are mapped to APT-N in DataAdapter!
			//new object[] { "Dist SHEV APT-S", CycleType.DistanceBased, GearboxType.ATSerial, APTSGearboxT, TorqueConverterT, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridATStrategyT},
			//new object[] { "Dist SHEV APT-P", CycleType.DistanceBased, GearboxType.ATPowerSplit, APTPGearboxT, TorqueConverterT, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridATStrategyT},
			new object[] { "Dist PEV APT-N", CycleType.DistanceBased, GearboxType.APTN, APTNGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, null, null},
        };

        [TestCaseSource(nameof(DistanceBased_PEV_Source))]
        public void TestPowertrainBuilder_Components_Distance_PEV(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.BatteryElectricVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, DummyEngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] DistanceBased_PEV_TestPT_Source =
        {
            new object[] { "Dist PEV AMT", true, CycleType.DistanceBased, GearboxType.AMT, TestAMTGearboxT, null, null, null, TestHybridCtl, null},
			// APT-S and APT-P are mapped to APT-N in DataAdapter!
			//new object[] { "Dist SHEV APT-S", true, CycleType.DistanceBased, GearboxType.ATSerial, TestAPTSGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
			//new object[] { "Dist SHEV APT-P", true, CycleType.DistanceBased, GearboxType.ATPowerSplit, TestAPTPGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
			new object[] { "Dist PEV APT-N", true, CycleType.DistanceBased, GearboxType.APTN, TestAPTNGearboxT, null, null, null, TestHybridCtl, null},
        };

        [TestCaseSource(nameof(DistanceBased_PEV_TestPT_Source))]
        public void TestPowertrainBuilder_Components_Distance_PEV_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.BatteryElectricVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
			
			AssertTestpowertrains(expectTestPT, testpowertrains);

			if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, TestVehicleContainerT, TestDistanceBasedDrivingCycleT, TestDriverSimpleContT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

        }

        #endregion

        #region Battery Electric PT, measured speed
        // ############################
        //   Conventional PT, distance based

        public static object[] MeasuredSpeed_PEV_Source =
        {
            new object[] { "Dist PEV AMT", CycleType.MeasuredSpeed, GearboxType.AMT, PEVGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, null, null},
			// APT-S and APT-P are mapped to APT-N in DataAdapter!
			//new object[] { "Dist SHEV APT-S", CycleType.DistanceBased, GearboxType.ATSerial, APTSGearboxT, TorqueConverterT, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridATStrategyT},
			//new object[] { "Dist SHEV APT-P", CycleType.DistanceBased, GearboxType.ATPowerSplit, APTPGearboxT, TorqueConverterT, PEVShiftStrategyT, ATClutchInfoT, SerialHybridCtl, SerialHybridATStrategyT},
			new object[] { "Dist PEV APT-N", CycleType.MeasuredSpeed, GearboxType.APTN, APTNGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, null, null},
        };

        [TestCaseSource(nameof(MeasuredSpeed_PEV_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_PEV(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.BatteryElectricVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, MeasuredspeedDrivingCycleT, MeasuredspeedDrivingCycleT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] MeasuredSpeed_PEV_TestPT_Source =
        {
            new object[] { "Dist PEV AMT", true, CycleType.MeasuredSpeed, GearboxType.AMT, TestAMTGearboxT, null, null, null, TestHybridCtl, null},
			// APT-S and APT-P are mapped to APT-N in DataAdapter!
			//new object[] { "Dist SHEV APT-S", true, CycleType.DistanceBased, GearboxType.ATSerial, TestAPTSGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
			//new object[] { "Dist SHEV APT-P", true, CycleType.DistanceBased, GearboxType.ATPowerSplit, TestAPTPGearboxT, TorqueConverterT, null, ATClutchInfoT, TestHybridCtl, null},
			new object[] { "Dist PEV APT-N", true, CycleType.MeasuredSpeed, GearboxType.APTN, TestAPTNGearboxT, null, null, null, TestHybridCtl, null},
        };

        [TestCaseSource(nameof(MeasuredSpeed_PEV_TestPT_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_PEV_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.BatteryElectricVehicle, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, TestVehicleContainerT, TestMeasuredspeedDrivingCycleT, TestMeasuredspeedDrivingCycleT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - -

        #region IEPC-E Hybrid PT, distance based
        // ############################
        //   Conventional PT, distance based

        public static object[] DistanceBased_IEPCE_Source =
        {
            new object[] { "Dist IEPC-E", CycleType.DistanceBased, GearboxType.APTN, IEPCGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, null, null},
        };

        [TestCaseSource(nameof(DistanceBased_IEPCE_Source))]
        public void TestPowertrainBuilder_Components_Distance_IEPCE(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IEPC_E, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, DistanceBasedDrivingCycleT, DriverT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, DummyEngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] DistanceBased_IEPCE_TestPT_Source =
        {
            new object[] { "Dist IEPC-E", true, CycleType.DistanceBased, GearboxType.APTN, TestIEPCGearboxMultipleGearsT, null, null, null, null, null},
        };

        [TestCaseSource(nameof(DistanceBased_IEPCE_TestPT_Source))]
        public void TestPowertrainBuilder_Components_Distance_IEPCE_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IEPC_E, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, TestVehicleContainerT, TestDistanceBasedDrivingCycleT, TestDriverSimpleContT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }



        #endregion

        #region IEPC-E Hybrid PT, measured speed
        // ############################
        //   Conventional PT, distance based

        public static object[] MeasuredSpeed_IEPCE_Source =
        {
            new object[] { "Dist IEPC-E", CycleType.MeasuredSpeed, GearboxType.APTN, IEPCGearboxT, null, PEVShiftStrategyT, ATClutchInfoT, null, null},
        };

        [TestCaseSource(nameof(MeasuredSpeed_IEPCE_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_IEPCE(string nuame, CycleType cycleType,
            GearboxType gbxType, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridControllerT,
            Type expectedHybridStrategyT)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IEPC_E, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            AssertPowertrainComponents(pt, VehicleContainerT, MeasuredspeedDrivingCycleT, MeasuredspeedDrivingCycleT, VehicleT, WheelsT,
                BrakesT, AxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, DummyEngineT, expectedHybridControllerT, expectedHybridStrategyT, expectedTorqueConverterT);
        }

        public static object[] MeasuredSpeed_IEPCE_TestPT_Source =
        {
            new object[] { "Dist IEPC-E", true, CycleType.MeasuredSpeed, GearboxType.APTN, IEPCGearboxT, TestAPTNGearboxT, null, null, null, null, null},
        };

		[TestCaseSource(nameof(MeasuredSpeed_IEPCE_TestPT_Source))]
        public void TestPowertrainBuilder_Components_MeasuredSpeed_IEPCE_TestPT(string nuame, bool expectTestPT, CycleType cycleType, GearboxType gbxType, Type gbxT, Type expectedGearboxT, Type expectedTorqueConverterT, Type expectedShiftStrategyT, Type expectedClutchT, Type expectedHybridController, Type expectedHybridStrategy)
        {
            var runData = CreateRunData(cycleType, VectoSimulationJobType.IEPC_E, gbxType);
            var pt = PowertrainBuilder.Build(runData, GetMockModalDataContainer());

            var testpowertrains = GetSimpleVehicleContainer(pt);
            if (testpowertrains.TryGetValue(TestPowertrainSource.ShiftStrategySimplePowertrain, out var c1)) {
                AssertPowertrainComponents(c1, TestVehicleContainerT, TestMeasuredspeedDrivingCycleT, TestMeasuredspeedDrivingCycleT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c2)) {
                AssertPowertrainComponents(c2, TestVehicleContainerT, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            }
            if (testpowertrains.TryGetValue(TestPowertrainSource.HybridStrategyTestPowertrain, out var c3)) {
                AssertPowertrainComponents(c3, TestVehicleContainerT, TestPowertrainCycleT, TestDriverTestPTT, TestVehicleT, TestWheelsT, TestBrakesT, TestAxleGearT, null, null, expectedGearboxT, expectedShiftStrategyT, expectedClutchT, null, null, null, expectedTorqueConverterT);
            }

            AssertTestpowertrains(expectTestPT, testpowertrains);
        }



        #endregion


        // ############################
        // ############################
        // ############################

        private VectoRunData CreateRunData(CycleType cycleType, VectoSimulationJobType jobType, GearboxType gbxType)
        {
            var engineIdlingSpeed = 600.RPMtoRad();
            var retVal = new VectoRunData() {
                JobType = jobType,
				Cycle = new DrivingCycleData() {
                    CycleType = cycleType,
                    Entries = new List<DrivingCycleData.DrivingCycleEntry>() {
                        new DrivingCycleData.DrivingCycleEntry() {
                            Distance = 0.SI<Meter>(),
                            RoadGradient = 0.SI<Radian>()
                        }
                    },
                },
                DriverData = new DriverData() {

                },
                VehicleData = new VehicleData() {
                    DynamicTyreRadius = 0.5.SI<Meter>(),
                    CurbMass = 7000.SI<Kilogram>(),
                    AxleData = new List<Axle>() {
                        new Axle() {
                            AxleType = AxleType.VehicleNonDriven,
                            RollResistanceCoefficient = 0.005,
                            AxleWeightShare = 0.5,
                            TyreTestLoad = 33500.SI<Newton>(),
                            Inertia = 5.SI<KilogramSquareMeter>()
                        },
                        new Axle() {
                            AxleType = AxleType.VehicleDriven,
                            RollResistanceCoefficient = 0.005,
                            AxleWeightShare = 0.5,
                            TyreTestLoad = 33500.SI<Newton>(),
                            Inertia = 5.SI<KilogramSquareMeter>()
                        }
                    },
                    AirDensity = DeclarationData.AirDensity,
                },
                AirdragData = new AirdragData() {
                    CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection,
                    CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(4.SI<SquareMeter>(), 0.SI<SquareMeter>(), new[] {
                        new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry() {
                            Velocity = 0.KMPHtoMeterPerSecond(),
                            EffectiveCrossSectionArea = 4.SI<SquareMeter>()
                        },
                        new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry() {
                            Velocity = 100.KMPHtoMeterPerSecond(),
                            EffectiveCrossSectionArea = 4.SI<SquareMeter>()
                        },
					}.ToList(), CrossWindCorrectionMode.DeclarationModeCorrection)
                },
                AxleGearData = new AxleGearData() {
                    AxleGear = new TransmissionData() {
                        Ratio = 1.0,
                        LossMap = TransmissionLossMapReader.Create(1.0, 1.0, "axlegear"),
                    }
                },
                GearboxData = new GearboxData() {
                    Type = gbxType,
                    Gears = new Dictionary<uint, GearData>() {
                        {1, new GearData() {
                            Ratio = 1.0,
                            LossMap = TransmissionLossMapReader.Create(1.0, 1.0, "Gear 1")
                        }},
                        {2, new GearData() {
                                Ratio = 0.9,
                                LossMap = TransmissionLossMapReader.Create(1.0, 0.9, "Gear 2")
                        } }
                    },
                    ShiftStrategy = _kernel.Get<IShiftStrategyFactory>().GetShiftStrategyName(gbxType, jobType),
                },
                GearshiftParameters = new ShiftStrategyParameters() {
                    StartSpeed = 8.KMPHtoMeterPerSecond(),
                    LoadStageThresoldsDown = DeclarationData.GearboxTCU.LoadStageThresoldsDown,
                    LoadStageThresoldsUp = DeclarationData.GearboxTCU.LoadStageThresholdsUp,
                    ShiftSpeedsTCToLocked = DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
                        .Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
                },
                HybridStrategyParameters = new HybridStrategyParameters() {

                },
                Retarder = new RetarderData() {
                    Type = RetarderType.None,
                },
                ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),

                Aux = new List<VectoRunData.AuxData>(),
            };

            if (jobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
                    VectoSimulationJobType.SerialHybridVehicle, VectoSimulationJobType.IEPC_S,
                    VectoSimulationJobType.IEPC_E)) {
                var gbxMock = new Mock<IGearboxDeclarationInputData>();
                gbxMock.Setup(g => g.Gears).Returns(new List<ITransmissionInputData>());
                retVal.GearboxData.InputData = gbxMock.Object;
            }

            if (!jobType.IsBatteryElectric()) {
                retVal.EngineData = new CombustionEngineData() {
                    IdleSpeed = engineIdlingSpeed,
                    Inertia = 2.SI<KilogramSquareMeter>(),
                    EngineStartTime = 1.SI<Second>(),
                    Fuels = new List<CombustionEngineFuelData>(),
                    FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
                        {
                            0,
                            new EngineFullLoadCurve(new[] {
                                new EngineFullLoadCurve.FullLoadCurveEntry() {
                                    EngineSpeed = 550.RPMtoRad(), TorqueDrag = -10.SI<NewtonMeter>(),
                                    TorqueFullLoad = 200.SI<NewtonMeter>(),
                                },
                                new EngineFullLoadCurve.FullLoadCurveEntry() {
                                    EngineSpeed = 650.RPMtoRad(), TorqueDrag = -10.SI<NewtonMeter>(),
                                    TorqueFullLoad = 400.SI<NewtonMeter>()
                                },
                                new EngineFullLoadCurve.FullLoadCurveEntry() {
                                    EngineSpeed = 1500.RPMtoRad(), TorqueDrag = -10.SI<NewtonMeter>(),
                                    TorqueFullLoad = 400.SI<NewtonMeter>()
                                },
                                new EngineFullLoadCurve.FullLoadCurveEntry() {
                                    EngineSpeed = 2500.RPMtoRad(), TorqueDrag = -10.SI<NewtonMeter>(),
                                    TorqueFullLoad = 200.SI<NewtonMeter>()
                                }

                            }.ToList(), null)
                        }
                    }
                };
                retVal.EngineData.FullLoadCurves[1] = retVal.EngineData.FullLoadCurves[0];
                retVal.EngineData.FullLoadCurves[2] = retVal.EngineData.FullLoadCurves[0];
            }

            if (jobType.IsOneOf(VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.IHPC)) {
                retVal.ElectricMachinesData.Add(Tuple.Create(PowertrainPosition.HybridP3, new ElectricMotorData() {
                    Overload = new OverloadData() { ContinuousPowerLoss = 0.SI<Watt>(), ContinuousTorque = 0.SI<NewtonMeter>(), OverloadBuffer = 0.SI<Joule>() },
                    OverloadRecoveryFactor = 0.9,
                }));

                retVal.BatteryData = new BatterySystemData() {
                    Batteries = new List<Tuple<int, BatteryData>>() {
                        Tuple.Create(0, new BatteryData() {
                            BatteryId = 0,
                            Capacity = 50.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            MinSOC = 0.3,
                            MaxSOC = 0.8
                        })
                    },
                    ConnectionSystemResistance = 0.SI<Ohm>(),
                    InitialSoC = 0.5
                };
            }

            if (jobType.IsOneOf(VectoSimulationJobType.SerialHybridVehicle, VectoSimulationJobType.IEPC_S)) {
                retVal.ElectricMachinesData.Add(Tuple.Create(PowertrainPosition.GEN, new ElectricMotorData() {
                    Overload = new OverloadData() {
                        ContinuousPowerLoss = 0.SI<Watt>(),
                        ContinuousTorque = 0.SI<NewtonMeter>(),
                        OverloadBuffer = 0.SI<Joule>(),
                    },
                    OverloadRecoveryFactor = 0.9,
                    EfficiencyData = new VoltageLevelData() {
                        VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
                            new ElectricMotorVoltageLevelData() {
                                FullLoadCurve = new ElectricMotorFullLoadCurve(new[] {new ElectricMotorFullLoadCurve.FullLoadEntry() {
                                    FullDriveTorque = 400.SI<NewtonMeter>(),
                                    FullGenerationTorque = 400.SI<NewtonMeter>(),
                                    MotorSpeed = 0.RPMtoRad()
                                }}.ToList())
                            }
                        }
                    }
                }));
            }

            if (jobType.IsOneOf(VectoSimulationJobType.SerialHybridVehicle, VectoSimulationJobType.IEPC_S,
                    VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E)) {
                var pos = jobType == VectoSimulationJobType.IEPC_S || jobType == VectoSimulationJobType.IEPC_E
                    ? PowertrainPosition.IEPC
                    : PowertrainPosition.BatteryElectricE2;
                retVal.ElectricMachinesData.Add(Tuple.Create(pos, new ElectricMotorData() {
                    Overload = new OverloadData() {
                        ContinuousPowerLoss = 0.SI<Watt>(),
                        ContinuousTorque = 0.SI<NewtonMeter>(),
                        OverloadBuffer = 0.SI<Joule>()
                    },
                    OverloadRecoveryFactor = 0.9,
                    EfficiencyData = new VoltageLevelData() {
                        VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
                            new ElectricMotorVoltageLevelData() {
                                FullLoadCurve = new ElectricMotorFullLoadCurve(new[] {
                                    new ElectricMotorFullLoadCurve.FullLoadEntry() {
                                        FullDriveTorque = 400.SI<NewtonMeter>(),
                                        FullGenerationTorque = 400.SI<NewtonMeter>(),
                                        MotorSpeed = 0.RPMtoRad()
                                    }
                                }.ToList())
                            }
                        }
                    }
                }));
            }

            if (jobType != VectoSimulationJobType.ConventionalVehicle) {
                retVal.BatteryData = new BatterySystemData() {
                    Batteries = new List<Tuple<int, BatteryData>>() {
                        Tuple.Create(0, new BatteryData() {
                            BatteryId = 0,
                            Capacity = 50.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                            MinSOC = 0.3,
                            MaxSOC = 0.8
                        })
                    },
                    ConnectionSystemResistance = 0.SI<Ohm>(),
                    InitialSoC = 0.5
                };
            }

            return retVal;
        }
    }
}
