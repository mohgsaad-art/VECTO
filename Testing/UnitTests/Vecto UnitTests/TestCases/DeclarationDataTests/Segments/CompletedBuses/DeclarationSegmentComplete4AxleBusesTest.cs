using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.Segments.CompletedBuses
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DeclarationSegmentComplete4AxleBusesTest
    {
        private MissionType[] _missionsTypes;

        [OneTimeSetUp]
        public void RunBeforeAnyTest()
        {
            _missionsTypes = new[]
                { MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };
        }

        [
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CE, RegistrationClass.I, 0, 0, false, VehicleClass.Class37a, 3),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CE, RegistrationClass.I, 0, 0, true, VehicleClass.Class37b1, 3),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CE, RegistrationClass.II, 0, 0, true, VehicleClass.Class37b2, 4),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CF, RegistrationClass.A, 0, 0, false, VehicleClass.Class37c, 3),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CI, RegistrationClass.B, 0, 0, false, VehicleClass.Class37d, 3),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CJ, RegistrationClass.A, 0, 0, false, VehicleClass.Class37e, 3),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CA, RegistrationClass.II, 0, 0, false, VehicleClass.Class38a, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CA, RegistrationClass.II_III, 0, 3.1, false, VehicleClass.Class38b, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CA, RegistrationClass.II_III, 0, 3.1001, false, VehicleClass.Class38c, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CA, RegistrationClass.III, 0, 0, false, VehicleClass.Class38d, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CB, RegistrationClass.B, 5, 0, false, VehicleClass.Class38e, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CB, RegistrationClass.II, 8, 0, false, VehicleClass.Class38f, 2),

            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CG, RegistrationClass.I, 0, 0, false, VehicleClass.Class39a, 3),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CG, RegistrationClass.I_II, 0, 0, true, VehicleClass.Class39b1, 3),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CG, RegistrationClass.II, 0, 0, true, VehicleClass.Class39b2, 4),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CH, RegistrationClass.A, 0, 0, false, VehicleClass.Class39c, 3),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CC, RegistrationClass.II, 0, 0, false, VehicleClass.Class40a, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CC, RegistrationClass.II_III, 0, 3.1, true, VehicleClass.Class40b, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CC, RegistrationClass.II_III, 0, 3.1001, false, VehicleClass.Class40c, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CC, RegistrationClass.B, 0, 0, false, VehicleClass.Class40d, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CD, RegistrationClass.III, 5, 0, false, VehicleClass.Class40e, 2),
            TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CD, RegistrationClass.II, 10, 0, false, VehicleClass.Class40f, 2)
        ]
        public void SegmentLookupTest(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck,
            double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>(), lowEntry);
            Assert.AreEqual(numberOfMissions, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);
            Assert.IsNotNull(segment.AccelerationFile);
        }


        [TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CG, RegistrationClass.I, 0, 0, false, VehicleClass.Class39a, 3)]
        public void TestComplete4AxlesCompleteBus39A(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>(), lowEntry);
            Assert.AreEqual(3, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i], mission.MissionType);
                Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 20, axle2: 28.20, axle3: 32.40, axle4: 19.40, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 3.0 },
                    airDragAllowed: false,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        [TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CG, RegistrationClass.A, 0, 0, true, VehicleClass.Class39b1, 3),
        TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CG, RegistrationClass.II, 0, 0, true, VehicleClass.Class39b2, 4)]
        public void TestComplete4AxlesCompleteBus39B(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);
            Assert.AreEqual(numberOfMissions, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i], mission.MissionType);
                Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 20, axle2: 28.2, axle3: 32.4, axle4: 19.4, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { mission.MissionType == MissionType.Interurban ? 2.2 : 3 },
                    airDragAllowed: mission.MissionType == MissionType.Interurban ? true : false,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        #region Assert Methods

        private void AssertBusParameters(MissionType missionType, double[] passengerDensity, bool airDragAllowed,
            bool doubleDecker, BusParameters busParameters)
        {
            switch (missionType)
            {
                case MissionType.HeavyUrban:
                case MissionType.Urban:
                case MissionType.Suburban:
                case MissionType.Interurban:
                    var currentValue = passengerDensity[0];
                    Assert.AreEqual(currentValue, busParameters.PassengerDensityRef.Value());
                    Assert.AreEqual(currentValue, busParameters.PassengerDensityLow.Value());
                    break;
                case MissionType.Coach:
                    var coachValue = passengerDensity[1];
                    Assert.AreEqual(coachValue, busParameters.PassengerDensityRef.Value());
                    Assert.AreEqual(coachValue, busParameters.PassengerDensityLow.Value());
                    break;

            }
            Assert.AreEqual(airDragAllowed, busParameters.AirDragMeasurementAllowed);
            Assert.AreEqual(doubleDecker, busParameters.DoubleDecker);
            Assert.AreEqual(0.30.SI<Meter>(), busParameters.DeltaHeight);
        }


        private void AssertAxleDistribution(double axle1, double axle2, double axle3, double axle4,
            double[] axleLoadDistribution)
        {
            Assert.AreEqual(axle1, axleLoadDistribution[0] * 100, 1e-0);
            Assert.AreEqual(axle2, axleLoadDistribution[1] * 100, 1e-0);
            if (axle3 > 0)
            {
                Assert.AreEqual(axle3, axleLoadDistribution[2] * 100, 1e-0);
            }
            if (axle4 > 0)
            {
                Assert.AreEqual(axle4, axleLoadDistribution[3] * 100, 1e-0);
            }
        }


        private void AssertVehicleEquipment(double externalDisplays, double internalDisplays, double fridge,
            double kitchenStandard, Dictionary<string, double> vehicleEquipment)
        {
            Assert.AreEqual(externalDisplays, vehicleEquipment["External displays"]);
            Assert.AreEqual(internalDisplays, vehicleEquipment["Internal displays"]);
            Assert.AreEqual(fridge, vehicleEquipment["Fridge"]);
            Assert.AreEqual(kitchenStandard, vehicleEquipment["Kitchen Standard"]);
        }

        #endregion
    }
}
