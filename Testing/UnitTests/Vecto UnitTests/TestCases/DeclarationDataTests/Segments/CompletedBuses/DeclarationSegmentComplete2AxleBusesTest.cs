using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.Segments.CompletedBuses
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DeclarationSegmentComplete2AxleBusesTest
    {
        private MissionType[] _missionsTypes;

        [OneTimeSetUp]
        public void RunBeforeAnyTest()
        {
            _missionsTypes = new[]
                { MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };
        }

        [
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.I, 0, 0, false, VehicleClass.Class31a, 3),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.II, 0, 0, true, VehicleClass.Class31b2, 4),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CF, RegistrationClass.A, 0, 0, false, VehicleClass.Class31c, 3),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CI, RegistrationClass.B, 0, 0, false, VehicleClass.Class31d, 3),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CJ, RegistrationClass.A, 0, 0, false, VehicleClass.Class31e, 3),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II, 0, 0, false, VehicleClass.Class32a, 2),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II_III, 0, 3.1, false, VehicleClass.Class32b, 2),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II_III, 0, 3.1001, false, VehicleClass.Class32c, 2),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.III, 0, 0, false, VehicleClass.Class32d, 2),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CB, RegistrationClass.B, 6, 0, false, VehicleClass.Class32e, 2),
            TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CB, RegistrationClass.II, 7, 0, false, VehicleClass.Class32f, 2),
        ]
        public void SegmentLookupTest(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck,
            double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>(), lowEntry);
            Assert.AreEqual(numberOfMissions, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);
            Assert.IsNotNull(segment.AccelerationFile);
        }

        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.I, 0, 0, false, VehicleClass.Class31a, 3)]
        public void TestComplete2AxlesCompleteBus31A(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>(), lowEntry);
            Assert.AreEqual(3, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i], mission.MissionType);
                Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 3.0 },
                    airDragAllowed: false,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.I, 0, 0, true, VehicleClass.Class31b1, 3),
        TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.II, 0, 0, true, VehicleClass.Class31b2, 4)]
        public void TestComplete2AxlesCompleteBus31B(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
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
                Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { mission.MissionType == MissionType.Interurban ? 2.2 : 3 },
                    airDragAllowed: mission.MissionType == MissionType.Interurban ? true : false,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CF, RegistrationClass.I, 0, 0, false, VehicleClass.Class31c, 3)]
        public void TestComplete2AxlesCompleteBus31F(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);
            Assert.AreEqual(3, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i], mission.MissionType);
                Assert.AreEqual(6.2, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 3.7 },
                    airDragAllowed: false,
                    doubleDecker: true,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CI, RegistrationClass.B, 0, 0, false, VehicleClass.Class31d, 3)]
        public void TestComplete2AxlesCompleteBus31D(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);

            Assert.AreEqual(3, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i], mission.MissionType);
                Assert.AreEqual(5.7, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 3.0 },
                    airDragAllowed: false,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 1, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CJ, RegistrationClass.A, 0, 0, false, VehicleClass.Class31e, 3)]
        public void TestComplete2AxlesCompleteBus31E(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);

            Assert.AreEqual(3, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i], mission.MissionType);
                Assert.AreEqual(7, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 3.7 },
                    airDragAllowed: false,
                    doubleDecker: true,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 2, internalDisplays: 1, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II, 0, 0, false, VehicleClass.Class32a, 2)]
        public void TestComplete2AxlesCompleteBus32A(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);

            Assert.AreEqual(2, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
                Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 2.2, 1.4 },
                    airDragAllowed: true,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II_III, 0, 3.1, false, VehicleClass.Class32b, 2),]
        public void TestComplete2AxlesCompleteBus32B(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);

            Assert.AreEqual(2, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
                Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 2.2, 1.4 },
                    airDragAllowed: true,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }



        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II_III, 0, 3.1001, false, VehicleClass.Class32c, 2)]
        public void TestComplete2AxlesCompleteBus32C(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);

            Assert.AreEqual(2, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
                Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 2.2, 1.4 },
                    airDragAllowed: true,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }


        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.III, 0, 0, false, VehicleClass.Class32d, 2)]
        public void TestComplete2AxlesCompleteBus32D(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);

            Assert.AreEqual(2, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
                Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 2.2, 1.4 },
                    airDragAllowed: true,
                    doubleDecker: false,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }

        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CB, RegistrationClass.B, 6, 0, false, VehicleClass.Class32e, 2)]
        public void TestComplete2AxlesCompleteBus32E(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);

            Assert.AreEqual(2, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
                Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 2.2, 1.4 },
                    airDragAllowed: true,
                    doubleDecker: true,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1,
                    vehicleEquipment: mission.BusParameter.ElectricalConsumers);
            }
        }

        [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CB, RegistrationClass.II, 7, 0, false, VehicleClass.Class32f, 2)]
        public void TestComplete2AxlesCompleteBus32F(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass,
            int passengersLowerDeck, double bodyHeight, bool lowEntry, VehicleClass vehicleParameterGroup, int numberOfMissions)
        {
            var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck,
                bodyHeight.SI<Meter>(), lowEntry);

            Assert.AreEqual(2, segment.Missions.Length);
            Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);

            for (int i = 0; i < segment.Missions.Length; i++)
            {
                var mission = segment.Missions[i];

                Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
                Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
                Assert.AreEqual("CoachBus", mission.CrossWindCorrectionParameters);
                AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);

                AssertBusParameters(
                    missionType: mission.MissionType,
                    passengerDensity: new[] { 3.0, 2.0 },
                    airDragAllowed: true,
                    doubleDecker: true,
                    busParameters: mission.BusParameter
                );

                AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1,
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
