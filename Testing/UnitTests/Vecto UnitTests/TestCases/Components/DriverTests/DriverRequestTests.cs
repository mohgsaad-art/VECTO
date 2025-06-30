using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.DriverTests;

public class DriverRequestTests
{
	public const double Tolerance = 0.001;

    [TestCase]
    public void DriverAccelerationTest()
    {
		var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
		var vehicleContainer = GetMockVehicleContainer(cycleData);

		var vehicleMock = Mock.Get(vehicleContainer.VehicleInfo);
		var driver = new Driver(vehicleContainer, vehicleContainer.RunData.DriverData, new DefaultDriverStrategy(vehicleContainer));


		MeterPerSquareSecond reqAcc = null;
		Radian reqGradient = null;
		Second reqSimInterval = null;

        var mockPort = new Mock<IDriverDemandOutPort>();
		mockPort.Setup(p =>
				p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<MeterPerSquareSecond>(), It.IsAny<Radian>(), It.IsAny<bool>()))
			.Returns((Second t, Second dt, MeterPerSquareSecond acc, Radian grad, bool dryryb) => {
				reqAcc = acc;
				reqGradient = grad;
				reqSimInterval = dt;
				return new ResponseSuccess(this) {
					SimulationInterval = dt
				};
			});
		
		mockPort.Setup(p => p.Initialize(It.IsAny<MeterPerSecond>(), It.IsAny<Radian>(), It.IsAny<MeterPerSquareSecond>())).Returns(
			(MeterPerSecond velocity, Radian gradient, MeterPerSquareSecond acc) => new ResponseSuccess(this));
		driver.Connect(mockPort.Object);

		var vehicleSpeed = 0.SI<MeterPerSecond>();
        vehicleMock.Setup(v => v.VehicleSpeed).Returns(() => vehicleSpeed);
        var absTime = 0.SI<Second>();
        var ds = 1.SI<Meter>();
        var gradient = 0.SI<Radian>();

        var targetVelocity = 5.SI<MeterPerSecond>();

        //			var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

        var accelerations = new[] {
                1.01570922, 1.384540943, 1.364944972, 1.350793466, 1.331848649, 1.314995215, 1.2999934,
                1.281996392, 1.255462262
            };
        var simulationIntervals = new[] {
                1.403234648, 0.553054094, 0.405255346, 0.33653593, 0.294559444, 0.26555781, 0.243971311, 0.22711761,
                0.213554656
            };

        // accelerate from 0 to just below the target velocity and test derived simulation intervals & accelerations
        for (var i = 0; i < accelerations.Length; i++) {
            var tmpResponse = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

            Assert.IsInstanceOf<ResponseSuccess>(tmpResponse);
            Assert.AreEqual(accelerations[i], reqAcc.Value(), Tolerance);
            Assert.AreEqual(simulationIntervals[i], tmpResponse.SimulationInterval.Value(), Tolerance);

            vehicleContainer.CommitSimulationStep(absTime, tmpResponse.SimulationInterval);
            absTime += tmpResponse.SimulationInterval;
			vehicleSpeed +=
                (tmpResponse.SimulationInterval * reqAcc).Cast<MeterPerSecond>();
        }

        // full acceleration would exceed target velocity, driver should limit acceleration such that target velocity is reached...
        var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

        Assert.IsInstanceOf<ResponseSuccess>(response);
        Assert.AreEqual(0.899715479, reqAcc.Value(), Tolerance);
        Assert.AreEqual(0.203734517, response.SimulationInterval.Value(), Tolerance);

        vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
        absTime += response.SimulationInterval;
		vehicleSpeed +=
            (response.SimulationInterval * reqAcc).Cast<MeterPerSecond>();

        Assert.AreEqual(targetVelocity.Value(), vehicleSpeed.Value(), Tolerance);

        // vehicle has reached target velocity, no further acceleration necessary...

        response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

        Assert.IsInstanceOf<ResponseSuccess>(response);
        Assert.AreEqual(0, reqAcc.Value(), Tolerance);
        Assert.AreEqual(0.2, response.SimulationInterval.Value(), Tolerance);
    }

    [TestCase]
    public void DriverDecelerationTest()
    {
		var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0\n110,20,0,0\n120,21,0,0\n130,22,0,0\n140,23,0,0\n150,24,0,0\n160,25,0,0\n170,26,0,0\n180,27,0,0\n190,28,0,0\n200,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
		var vehicleContainer = GetMockVehicleContainer(cycleData);

		var vehicleMock = Mock.Get(vehicleContainer.VehicleInfo);
		var driver = new Driver(vehicleContainer, vehicleContainer.RunData.DriverData, new DefaultDriverStrategy(vehicleContainer));


		MeterPerSquareSecond reqAcc = null;
		Radian reqGradient = null;
		Second reqSimInterval = null;

		var mockPort = new Mock<IDriverDemandOutPort>();
		mockPort.Setup(p =>
				p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<MeterPerSquareSecond>(), It.IsAny<Radian>(), It.IsAny<bool>()))
			.Returns((Second t, Second dt, MeterPerSquareSecond acc, Radian grad, bool dryryb) => {
				reqAcc = acc;
				reqGradient = grad;
				reqSimInterval = dt;
				return new ResponseSuccess(this) {
					SimulationInterval = dt
				};
			});

		mockPort.Setup(p => p.Initialize(It.IsAny<MeterPerSecond>(), It.IsAny<Radian>(), It.IsAny<MeterPerSquareSecond>())).Returns(
			(MeterPerSecond velocity, Radian gradient, MeterPerSquareSecond acc) => new ResponseSuccess(this));
		driver.Connect(mockPort.Object);

		var vehicleSpeed = 5.SI<MeterPerSecond>();
		vehicleMock.Setup(v => v.VehicleSpeed).Returns(() => vehicleSpeed);

        var absTime = 0.SI<Second>();
        var ds = 1.SI<Meter>();
        var gradient = 0.SI<Radian>();

        var targetVelocity = 0.SI<MeterPerSecond>();

        //			var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

        var accelerations = new[] {
                -0.68799597, -0.690581291, -0.693253225, -0.696020324, -0.698892653, -0.701882183, -0.695020765,
                -0.677731071,
                -0.660095846, -0.642072941, -0.623611107, -0.604646998, -0.58510078, -0.56497051, -0.547893288,
                -0.529859078,
                -0.510598641, -0.489688151, -0.466386685, -0.425121905
            };
        var simulationIntervals = new[] {
                0.202830428, 0.20884052, 0.215445127, 0.222749141, 0.230885341, 0.240024719, 0.250311822, 0.26182762,
                0.274732249,
                0.289322578, 0.305992262, 0.325276486, 0.34792491, 0.37502941, 0.408389927, 0.451003215, 0.5081108,
                0.590388012,
                0.724477573, 1.00152602
            };

        // accelerate from 0 to just below the target velocity and test derived simulation intervals & accelerations
        for (var i = 0; i < accelerations.Length; i++) {
            var tmpResponse = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

            Assert.IsInstanceOf<ResponseSuccess>(tmpResponse);
            Assert.AreEqual(accelerations[i], reqAcc.Value(), Tolerance);
            Assert.AreEqual(simulationIntervals[i], tmpResponse.SimulationInterval.Value(), Tolerance);

            vehicleContainer.CommitSimulationStep(absTime, tmpResponse.SimulationInterval);
            absTime += tmpResponse.SimulationInterval;
			vehicleSpeed +=
                (tmpResponse.SimulationInterval * reqAcc).Cast<MeterPerSecond>();
        }

        var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

        Assert.IsInstanceOf<ResponseSuccess>(response);
        Assert.AreEqual(-0.308576594, reqAcc.Value(), Tolerance);
        Assert.AreEqual(2.545854078, response.SimulationInterval.Value(), Tolerance);

        vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
        //absTime += response.SimulationInterval;
        vehicleSpeed +=
            (response.SimulationInterval * reqAcc).Cast<MeterPerSecond>();

        Assert.AreEqual(targetVelocity.Value(), vehicleSpeed.Value(), Tolerance);
    }

    [TestCase]
    public void DriverOverloadTest()
    {
        var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,5,0,0\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
		var vehicleContainer = GetMockVehicleContainer(cycleData);
        
		var driver = new Driver(vehicleContainer, vehicleContainer.RunData.DriverData, new DefaultDriverStrategy(vehicleContainer));

		var vehicleSpeed = 5.KMPHtoMeterPerSecond();

		// take into account the axle ratio and 1st-gear ratio
		var dynamicTyreRadius = 0.52.SI<Meter>() / (3.24 * 6.38);
		var vehicleMass = 48700.SI<Kilogram>();
		var wheelsInertia = 108.SI<KilogramSquareMeter>();
		var engineFld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(IceFldHdr, IceFldData));

        var vehicleMock = Mock.Get(vehicleContainer.VehicleInfo);
		vehicleMock.Setup(v => v.VehicleSpeed).Returns(() => vehicleSpeed);

        var mockPort = new Mock<IDriverDemandOutPort>();
		mockPort.Setup(p =>
				p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<MeterPerSquareSecond>(), It.IsAny<Radian>(), It.IsAny<bool>()))
			.Returns((Second t, Second dt, MeterPerSquareSecond acc, Radian grad, bool dryRun) => {
				var speed = vehicleSpeed + acc * dt;
				var rpm = speed / dynamicTyreRadius;
				var inertiaTq = Formulas.InertiaPower(rpm, vehicleSpeed /dynamicTyreRadius, wheelsInertia, dt) / ((speed + vehicleSpeed)/2.0/dynamicTyreRadius);

				var tq = (vehicleMass * acc + vehicleMass * Physics.GravityAccelleration * Math.Sin(grad.Value())) * dynamicTyreRadius + inertiaTq;
				var maxTq = engineFld.FullLoadStationaryTorque(rpm);
				var minTq = engineFld.DragLoadStationaryTorque(rpm);
                var deltaFull = tq - maxTq;
                var deltaDrag = tq - minTq;
				if (dryRun) {
					return new ResponseDryRun(this) {
						DeltaFullLoad = deltaFull * rpm,
						DeltaDragLoad = deltaDrag * rpm,
						DeltaFullLoadTorque = deltaFull,
						DeltaDragLoadTorque = deltaDrag,
					};
				}
				if (deltaFull.IsGreater(0)) {
					return new ResponseOverload(this) {
						AbsTime = t,
						Delta = deltaFull * rpm,
					};
				}

				if (deltaDrag.IsSmaller(0)) {
					return new ResponseUnderload(this) {
						AbsTime = t,
						Delta = deltaDrag * rpm,
					};
				}
				return new ResponseSuccess(this) {
					SimulationInterval = dt
				};
			});

		mockPort.Setup(p => p.Initialize(It.IsAny<MeterPerSecond>(), It.IsAny<Radian>(), It.IsAny<MeterPerSquareSecond>())).Returns(
			(MeterPerSecond velocity, Radian gradient, MeterPerSquareSecond acc) => new ResponseSuccess(this));
		driver.Connect(mockPort.Object);

        var driverPort = driver.OutPort();

        driverPort.Initialize(vehicleSpeed, 0.SI<Radian>());

        var absTime = 0.SI<Second>();

        var response = driverPort.Request(absTime, 1.SI<Meter>(), 20.SI<MeterPerSecond>(), 0.SI<Radian>());

        Assert.IsInstanceOf<ResponseSuccess>(response);

        vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
        absTime += response.SimulationInterval;

        Assert.AreEqual(0.29066, response.Driver.Acceleration.Value(), Tolerance);

        response = driverPort.Request(absTime, 1.SI<Meter>(), 20.SI<MeterPerSecond>(), 0.SI<Radian>());

        Assert.IsInstanceOf<ResponseSuccess>(response);

        vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
        absTime += response.SimulationInterval;

        Assert.AreEqual(0.2900, response.Driver.Acceleration.Value(), Tolerance);
    }

    [TestCase]
    public void DriverCoastingTest()
    {
        var vehicleContainer = GetMockVehicleContainer(null);

        var driver = new Driver(vehicleContainer, vehicleContainer.RunData.DriverData, new DefaultDriverStrategy(vehicleContainer));

        var vehicleSpeed = 5.SI<MeterPerSecond>();

        // take into account the axle ratio and 1st-gear ratio
        var dynamicTyreRadius = 0.52.SI<Meter>() / (3.24 * 6.38);
        var vehicleMass = 48700.SI<Kilogram>();
        var wheelsInertia = 108.SI<KilogramSquareMeter>();
        var engineFld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(IceFldHdr, IceFldData));

        var vehicleMock = Mock.Get(vehicleContainer.VehicleInfo);
        vehicleMock.Setup(v => v.VehicleSpeed).Returns(() => vehicleSpeed);

		NewtonMeter tqRequest = null;
		NewtonMeter tqDrag = null;
        var mockPort = new Mock<IDriverDemandOutPort>();
        mockPort.Setup(p =>
                p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<MeterPerSquareSecond>(), It.IsAny<Radian>(), It.IsAny<bool>()))
            .Returns((Second t, Second dt, MeterPerSquareSecond acc, Radian grad, bool dryRun) => {
                var speed = vehicleSpeed + acc * dt;
                var rpm = speed / dynamicTyreRadius;
                var inertiaTq = Formulas.InertiaPower(rpm, vehicleSpeed / dynamicTyreRadius, wheelsInertia, dt) / ((speed + vehicleSpeed) / 2.0 / dynamicTyreRadius);

                var tq = (vehicleMass * acc + vehicleMass * Physics.GravityAccelleration * Math.Sin(grad.Value())) * dynamicTyreRadius + inertiaTq;
                var maxTq = engineFld.FullLoadStationaryTorque(rpm);
                var minTq = engineFld.DragLoadStationaryTorque(rpm);
                var deltaFull = tq - maxTq;
                var deltaDrag = tq - minTq;
				tqRequest = tq;
				tqDrag = minTq;
                if (dryRun) {
                    return new ResponseDryRun(this) {
                        DeltaFullLoad = deltaFull * rpm,
                        DeltaDragLoad = deltaDrag * rpm,
                        DeltaFullLoadTorque = deltaFull,
                        DeltaDragLoadTorque = deltaDrag,
                    };
                }
                if (deltaFull.IsGreater(0)) {
                    return new ResponseOverload(this) {
                        AbsTime = t,
                        Delta = deltaFull * rpm,
                    };
                }

                if (deltaDrag.IsSmaller(0)) {
                    return new ResponseUnderload(this) {
                        AbsTime = t,
                        Delta = deltaDrag * rpm,
                    };
                }
                return new ResponseSuccess(this) {
                    SimulationInterval = dt
                };
            });

        mockPort.Setup(p => p.Initialize(It.IsAny<MeterPerSecond>(), It.IsAny<Radian>(), It.IsAny<MeterPerSquareSecond>())).Returns(
            (MeterPerSecond velocity, Radian gradient, MeterPerSquareSecond acc) => new ResponseSuccess(this));
        driver.Connect(mockPort.Object);


        //var velocity = 5.SI<MeterPerSecond>();
        driver.OutPort().Initialize(vehicleSpeed, 0.SI<Radian>());

        var absTime = 0.SI<Second>();

        var response = driver.DrivingActionCoast(absTime, 1.SI<Meter>(), vehicleSpeed, 0.SI<Radian>());
		vehicleSpeed += response.Driver.Acceleration * response.SimulationInterval;
        Assert.IsInstanceOf<ResponseSuccess>(response);

        vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
        absTime += response.SimulationInterval;

        Assert.AreEqual(4.989790, vehicleContainer.VehicleInfo.VehicleSpeed.Value(), Tolerance);
        Assert.AreEqual(0.2004, response.SimulationInterval.Value(), Tolerance);
        Assert.AreEqual(tqDrag.Value(), tqRequest.Value(),
            Constants.SimulationSettings.LineSearchTolerance);

        while (vehicleContainer.VehicleInfo.VehicleSpeed > 1.7) {
            response = driver.DrivingActionCoast(absTime, 1.SI<Meter>(), vehicleSpeed, 0.SI<Radian>());
			vehicleSpeed += response.Driver.Acceleration * response.SimulationInterval;

            Assert.IsInstanceOf<ResponseSuccess>(response);
            Assert.IsTrue(response.Driver.Acceleration.IsSmaller(0));

            vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
            absTime += response.SimulationInterval;
            //modData.Finish(VectoRun.Status.Success);
        }
        //modData.Finish(VectoRun.Status.Success);
    }

    [TestCase]
    public void DriverCoastingTest2()
    {
        var vehicleContainer = GetMockVehicleContainer(null);

        var driver = new Driver(vehicleContainer, vehicleContainer.RunData.DriverData, new DefaultDriverStrategy(vehicleContainer));

        var vehicleSpeed = 5.SI<MeterPerSecond>();

        // take into account the axle ratio and 1st-gear ratio
        var dynamicTyreRadius = 0.52.SI<Meter>() / (3.24 * 6.38);
        var vehicleMass = 48700.SI<Kilogram>();
        var wheelsInertia = 108.SI<KilogramSquareMeter>();
        var engineFld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(IceFldHdr, IceFldData));

        var vehicleMock = Mock.Get(vehicleContainer.VehicleInfo);
        vehicleMock.Setup(v => v.VehicleSpeed).Returns(() => vehicleSpeed);

        NewtonMeter tqRequest = null;
        NewtonMeter tqDrag = null;
        var mockPort = new Mock<IDriverDemandOutPort>();
        mockPort.Setup(p =>
                p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<MeterPerSquareSecond>(), It.IsAny<Radian>(), It.IsAny<bool>()))
            .Returns((Second t, Second dt, MeterPerSquareSecond acc, Radian grad, bool dryRun) => {
                var speed = vehicleSpeed + acc * dt;
                var rpm = speed / dynamicTyreRadius;
                var inertiaTq = Formulas.InertiaPower(rpm, vehicleSpeed / dynamicTyreRadius, wheelsInertia, dt) / ((speed + vehicleSpeed) / 2.0 / dynamicTyreRadius);

                var tq = (vehicleMass * acc + vehicleMass * Physics.GravityAccelleration * Math.Sin(grad.Value())) * dynamicTyreRadius + inertiaTq;
                var maxTq = engineFld.FullLoadStationaryTorque(rpm);
                var minTq = engineFld.DragLoadStationaryTorque(rpm);
                var deltaFull = tq - maxTq;
                var deltaDrag = tq - minTq;
                tqRequest = tq;
                tqDrag = minTq;
                if (dryRun) {
                    return new ResponseDryRun(this) {
                        DeltaFullLoad = deltaFull * rpm,
                        DeltaDragLoad = deltaDrag * rpm,
                        DeltaFullLoadTorque = deltaFull,
                        DeltaDragLoadTorque = deltaDrag,
                    };
                }
                if (deltaFull.IsGreater(0)) {
                    return new ResponseOverload(this) {
                        AbsTime = t,
                        Delta = deltaFull * rpm,
                    };
                }

                if (deltaDrag.IsSmaller(0)) {
                    return new ResponseUnderload(this) {
                        AbsTime = t,
                        Delta = deltaDrag * rpm,
                    };
                }
                return new ResponseSuccess(this) {
                    SimulationInterval = dt
                };
            });

        mockPort.Setup(p => p.Initialize(It.IsAny<MeterPerSecond>(), It.IsAny<Radian>(), It.IsAny<MeterPerSquareSecond>())).Returns(
            (MeterPerSecond velocity, Radian gradient, MeterPerSquareSecond acc) => new ResponseSuccess(this));
        driver.Connect(mockPort.Object);


        driver.OutPort().Initialize(vehicleSpeed, 0.SI<Radian>());

        var gradient = VectoMath.InclinationToAngle(-0.020237973 / 100.0);
        driver.OutPort().Initialize(vehicleSpeed, gradient);

        var absTime = 0.SI<Second>();

        var response = driver.DrivingActionCoast(absTime, 1.SI<Meter>(), vehicleSpeed, gradient);
		vehicleSpeed += response.Driver.Acceleration * response.SimulationInterval;

        Assert.IsInstanceOf<ResponseSuccess>(response);

        vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
        absTime += response.SimulationInterval;

        Assert.AreEqual(4.98987, vehicleContainer.VehicleInfo.VehicleSpeed.Value(), Tolerance);
        Assert.AreEqual(0.2004, response.SimulationInterval.Value(), Tolerance);
        Assert.AreEqual(tqDrag.Value(), tqRequest.Value(),
            Constants.SimulationSettings.LineSearchTolerance);

        while (vehicleContainer.VehicleInfo.VehicleSpeed > 1.7) {
            response = driver.DrivingActionCoast(absTime, 1.SI<Meter>(), vehicleSpeed, gradient);
			vehicleSpeed += response.Driver.Acceleration * response.SimulationInterval;

            Assert.IsInstanceOf<ResponseSuccess>(response);
            Assert.IsTrue(response.Driver.Acceleration.IsSmaller(0));
            vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
            absTime += response.SimulationInterval;
            //modData.Finish(VectoRun.Status.Success);
        }
        //modData.Finish(VectoRun.Status.Success);
    }


    // --------
    private static DriverData GetDriverData()
    {
        var lookAheadData = new DriverData.LACData {
            Enabled = DeclarationData.Driver.LookAhead.Enabled,

            //Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
            MinSpeed = DeclarationData.Driver.LookAhead.MinimumSpeed,
            LookAheadDecisionFactor = new LACDecisionFactor(),
            LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
        };
        var overspeedData = new DriverData.OverSpeedData {
            Enabled = true,
            MinSpeed = DeclarationData.Driver.OverSpeed.MinSpeed,
            OverSpeed = DeclarationData.Driver.OverSpeed.AllowedOverSpeed,
        };
        return new DriverData {
            AccelerationCurve = AccelerationCurveReader.Create(InputDataHelper.InputDataAsTableData(AccHdr, AccData)),
            LookAheadCoasting = lookAheadData,
            OverSpeed = overspeedData,
            EcoRoll = new DriverData.EcoRollData() {
                UnderspeedThreshold = DeclarationData.Driver.EcoRoll.UnderspeedThreshold,
                MinSpeed = DeclarationData.Driver.EcoRoll.MinSpeed,
                ActivationPhaseDuration = DeclarationData.Driver.EcoRoll.ActivationDelay,
                AccelerationLowerLimit = DeclarationData.Driver.EcoRoll.AccelerationLowerLimit,
                AccelerationUpperLimit = DeclarationData.Driver.EcoRoll.AccelerationUpperLimit,
            },
            PCC = new DriverData.PCCData() {
                PCCEnableSpeed = DeclarationData.Driver.PCC.PCCEnableSpeed,
                MinSpeed = DeclarationData.Driver.PCC.MinSpeed,
                PreviewDistanceUseCase1 = DeclarationData.Driver.PCC.PreviewDistanceUseCase1,
                PreviewDistanceUseCase2 = DeclarationData.Driver.PCC.PreviewDistanceUseCase2,
                UnderSpeed = DeclarationData.Driver.PCC.Underspeed,
                OverspeedUseCase3 = DeclarationData.Driver.PCC.OverspeedUseCase3
            }
        };
    }

	private static IVehicleContainer GetMockVehicleContainer(DrivingCycleData drivingCycleData)
	{
		var runData = new VectoRunData() {
			GearshiftParameters = new ShiftStrategyParameters() {
				StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
				StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
			},
			DriverData = GetDriverData(),
			Cycle = drivingCycleData,
			VehicleData = new VehicleData() {
				VehicleCategory = VehicleCategory.RigidTruck
			}
		};
		var container = new Mock<IVehicleContainer>();
		var vehicle = new Mock<IVehicleInfo>();
		vehicle.Setup(v => v.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());

		

		var cycle = new Mock<IDrivingCycleInfo>();
        cycle.Setup(c => c.CycleStartDistance).Returns(0.SI<Meter>());
        cycle.Setup(c => c.LookAhead(It.IsAny<Meter>())).Returns(new List<DrivingCycleData.DrivingCycleEntry>());
		if (drivingCycleData != null) {
			var left = drivingCycleData.Entries.GetEnumerator();
			var right = drivingCycleData.Entries.GetEnumerator();
			left.MoveNext();
			right.MoveNext();
			right.MoveNext();

			cycle.Setup(c => c.CycleData).Returns(() => new CycleData() {
				AbsTime = 0.SI<Second>(),
				AbsDistance = 0.SI<Meter>(),
				LeftSample = left.Current,
				RightSample = right.Current
			});
			container.Setup(c => c.CommitSimulationStep(It.IsAny<Second>(), It.IsAny<Second>())).Callback(
				(Second absTime, Second dt) => {
					left.MoveNext();
					right.MoveNext();
				});
        }

		var milage = new Mock<IMileageCounter>();
        milage.Setup(m => m.Distance).Returns(0.SI<Meter>());

        var pt = new Mock<IPowertainInfo>();
        pt.Setup(p => p.HasCombustionEngine).Returns(true);
        pt.Setup(p => p.HasElectricMotor).Returns(false);

        var iceCtl = new Mock<IEngineControl>();

        var gi = new Mock<IGearboxInfo>();
        gi.Setup(g => g.GearboxType).Returns(GearboxType.AMT);
        gi.Setup(g => g.GearEngaged(It.IsAny<Second>())).Returns(true);

        var ci = new Mock<IClutchInfo>();
        ci.Setup(c => c.ClutchClosed(It.IsAny<Second>())).Returns(true);

        var br = new Mock<IBrakes>();
        br.Setup(b => b.BrakePower).Returns(0.SI<Watt>());

        container.Setup(c => c.RunData).Returns(runData);
        container.Setup(c => c.VehicleInfo).Returns(vehicle.Object);
        container.Setup(c => c.DrivingCycleInfo).Returns(cycle.Object);
        container.Setup(c => c.MileageCounter).Returns(milage.Object);
        container.Setup(c => c.PowertrainInfo).Returns(pt.Object);
        container.Setup(c => c.EngineCtl).Returns(iceCtl.Object);
        container.Setup(c => c.GearboxInfo).Returns(gi.Object);
        container.Setup(c => c.ClutchInfo).Returns(ci.Object);
        container.Setup(c => c.Brakes).Returns(br.Object);
        
        return container.Object;
    }

    const string AccHdr = "v [km/h],acc [m/s²],dec [m/s²]";

	static readonly string[] AccData = new[] {
		"0,1.01570922360353,-0.231742702878269",
		"5,1.38546581120225,-0.45346198022574",
		"10,1.34993329755465,-0.565404125020508",
		"15,1.29026714002479,-0.703434814668512",
		"20,1.16369598822194,-0.677703399378421",
		"25,1.04024417156355,-0.63631961226452",
		"30,0.910278494884728,-0.548894523516266",
		"35,0.785875078338323,-0.453995336940216",
		"40,0.69560012996407,-0.385460695652016",
		"45,0.648984223443223,-0.349181329186105",
		"50,0.594249623931624,-0.309125096967231",
		"55,0.559156929181929,-0.296716093796643",
		"60,0.541508805860806,-0.270229542673924",
		"65,0.539582904761905,-0.256408113084341",
		"70,0.539103523809524,-0.217808535739946",
		"75,0.529581598997494,-0.18609307386602",
		"80,0.496418462064251,-0.142683384645006",
		"85,0.453932619248656,-0.117950211164234",
		"90,0.397824554210839,-0.102997621205622",
		"95,0.33969661577071,-0.102997621205622",
		"100,0.289428370365158,-0.102997621205622",
		"105,0.256471472751248,-0.102997621205622",
		"110,0.24,-0.102997621205622",
		"115,0.22,-0.102997621205622",
		"120,0.2,-0.102997621205622",
	};

    const string IceFldHdr = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

	private static readonly string[] IceFldData = new[] {
		"560,1500,-149,0.6",
		"600,1600,-148,0.6",
		"799.9999999,2150,-149,0.6",
		"1000,2300,-160,0.6",
		"1200,2300,-179,0.6",
		"1400,2300,-203,0.6",
		"1599.999999,2079,-235,0.49",
		"1800,1857,-264,0.25",
		"2000.000001,1352,-301,0.25",
		"2100,1100,-320,0.25",
	};
}