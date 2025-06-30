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

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DeclarationDataTest
	{
		private const double Tolerance = 0.0001;

		private readonly MissionType[] _missions = {
			MissionType.LongHaul,
			MissionType.RegionalDelivery,
			MissionType.UrbanDelivery,
			MissionType.MunicipalUtility,
			MissionType.Construction,
		};

		private StandardKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			_kernel = new StandardKernel(new VectoNinjectModule());
        }

        [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 0.002, null, 0.8),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 0.0032, null, 1.28),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 0.0033, GearboxType.ATPowerSplit, 1.3719),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 0.0033, GearboxType.MT, 1.3787),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 0.005, GearboxType.MT, 3.06),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 0.006, GearboxType.MT, 3.33),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 0.006, GearboxType.ATPowerSplit, 3.23),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, 0.006, null, 2.03),
		Category(Definitions.TESTCASE_MIGRATED)
        ]
        public void EngineInertiaTest(VectoSimulationJobType jobType, double displacement, GearboxType? gbxType,
            double inertia)
        { 
            KilogramSquareMeter result = DeclarationData.Engine.EngineInertia(
                jobType, 
                displacement.SI<CubicMeter>(), 
                gbxType);
            
            Assert.AreEqual(inertia, result.Value(), 1e-4);
        }


        [TestCase("285/60 R22.5", 10.6, 0.914, 3.03, 0.440766),
		TestCase("285/70 R19.5", 7.9, 0.895, 3.05, 0.434453),
		TestCase("395/85 R20", 27.9, 1.18, 3.05, 0.572798),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void WheelDataTest(string wheels, double inertia, double wheelsDiameter, double circumferenceFactor,
			double expectedDynamicRadius)
		{
			var tmp = DeclarationData.Wheels.Lookup(wheels);

			AssertHelper.AreRelativeEqual(inertia, tmp.Inertia);
			AssertHelper.AreRelativeEqual(wheelsDiameter, tmp.WheelsDiameter);
			AssertHelper.AreRelativeEqual(circumferenceFactor, tmp.CircumferenceFactor);
			Assert.AreEqual(expectedDynamicRadius, tmp.DynamicTyreRadius.Value(), 1e-6);
		}

		[
			// fixed points
			TestCase(400, 0),
			TestCase(800, 0.47),
			TestCase(1000, 0.58),
			TestCase(1200, 0.53),
			TestCase(1400, 0.46),
			TestCase(1500, 0.43),
			TestCase(1750, 0.22),
			TestCase(1800, 0.2),
			TestCase(2000, 0.11),
			TestCase(2500, 0.11),
			// interpolate
			TestCase(600, 0.235),
			TestCase(900, 0.525),
			TestCase(1100, 0.555),
			TestCase(1300, 0.495),
			TestCase(1450, 0.445),
			TestCase(1625, 0.325),
			TestCase(1775, 0.21),
			TestCase(1900, 0.155),
			TestCase(2250, 0.11),
            Category(Definitions.TESTCASE_MIGRATED)
		]
		public void PT1Test(double rpm, double expectedPt1)
		{
			var pt1 = DeclarationData.PT1.Lookup(rpm.RPMtoRad());
			Assert.AreEqual(expectedPt1, pt1.Value.Value(), Tolerance);
			Assert.IsFalse(pt1.Extrapolated);
		}

		[TestCase(200),
		TestCase(0),
		TestCase(13000),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void PT1ExceptionsTest(double rpm)
		{
			// EXTRAPOLATE 
			var tmp = DeclarationData.PT1.Lookup(rpm.RPMtoRad());
			Assert.IsTrue(tmp.Extrapolated);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void WHTCTest()
		{
			var whtc = DeclarationData.WHTCCorrection;

			var factors = new {
				urban = new[] { 0.0, 0.17, 0.69, 0.98, 0.62, 1.0, 1.0, 1.0, 0.45, 0.0 },
				rural = new[] { 0.0, 0.3, 0.27, 0.0, 0.32, 0.0, 0.0, 0.0, 0.36, 0.22 },
				motorway = new[] { 1.0, 0.53, 0.04, 0.02, 0.06, 0.0, 0.0, 0.0, 0.19, 0.78 }
			};

			var r = new Random();
			for (var i = 0; i < _missions.Length; i++) {
				var urban = r.NextDouble() * 2;
				var rural = r.NextDouble() * 2;
				var motorway = r.NextDouble() * 2;
				var whtcValue = whtc.Lookup(_missions[i], rural: rural, urban: urban, motorway: motorway);
				Assert.AreEqual(urban * factors.urban[i] + rural * factors.rural[i] + motorway * factors.motorway[i],
					whtcValue);
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void WHTCLookupTestLongHaul()
		{
			var expected = 1.0057;

			var rural = 1.0265;
			var urban = 1.0948;
			var motorway = 1.0057;

			var lookup = DeclarationData.WHTCCorrection.Lookup(MissionType.LongHaul, rural: rural, urban: urban,
				motorway: motorway);
			Assert.AreEqual(expected, lookup, 1e-8);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void WHTCLookupTestRegionalDelivery()
		{
			var expected = 1.02708700;

			var rural = 1.0265;
			var urban = 1.0948;
			var motorway = 1.0057;

			var lookup = DeclarationData.WHTCCorrection.Lookup(MissionType.RegionalDelivery, rural: rural, urban: urban,
				motorway: motorway);
			Assert.AreEqual(expected, lookup, 1e-8);
		}

		[
			//TestCase("MediumLorryVan",),   
			//TestCase("MediumLorryRigid"),
			TestCase("RigidSolo", 0.013526, 0.017746, -0.000666),
			TestCase("RigidTrailer", 0.017125, 0.072275, -0.004148),
			TestCase("TractorSemitrailer", 0.030042, 0.040817, -0.00213),
			TestCase("CoachBus", -0.000794, 0.02109, -0.00109),
			TestCase("MediumLorriesRigid", -0.0015, 0.0086, -0.00029),
			TestCase("MediumLorriesVan", 0.0032, 0.00532, -0.00028),
			Category(Definitions.TESTCASE_MIGRATED)]

		public void AirDrag_WithStringKey(string key, double a1, double a2, double a3)
		{
			var value = DeclarationData.AirDrag.Lookup(key);
			AssertHelper.AreRelativeEqual(a1, value.A1);
			AssertHelper.AreRelativeEqual(a2, value.A2);
			AssertHelper.AreRelativeEqual(a3, value.A3);
		}

		[
			//TestCase("MediumLorryVan",),   
			//TestCase("MediumLorryRigid"),
			TestCase("RigidSolo", 0.013526, 0.017746, -0.000666),
			TestCase("TractorSemitrailer", 0.030042, 0.040817, -0.00213),
			TestCase("RigidTrailer", 0.017125, 0.072275, -0.004148),
			TestCase("CoachBus", -0.000794, 0.02109, -0.00109),
			TestCase("MediumLorriesRigid", -0.0015, 0.0086, -0.00029),
			TestCase("MediumLorriesVan", 0.0032, 0.00532, -0.00028),
			Category(Definitions.DUPLICATE)]
		public void AirDrag_WithVehicleCategory(string parameterSet, double a1, double a2, double a3)
		{
			var value = DeclarationData.AirDrag.Lookup(parameterSet);
			AssertHelper.AreRelativeEqual(a1, value.A1);
			AssertHelper.AreRelativeEqual(a2, value.A2);
			AssertHelper.AreRelativeEqual(a3, value.A3);
		}

		[TestCase("TractorSemitrailer", 6.46, 0, 4.0, 7.71712257),
		TestCase("TractorSemitrailer", 6.46, 60, 4.0, 7.71712257),
		TestCase("TractorSemitrailer", 6.46, 75, 3.75, 7.35129203),
		TestCase("TractorSemitrailer", 6.46, 100, 4.0, 7.03986404),
		TestCase("TractorSemitrailer", 6.46, 62.1234, 4.0, 7.65751048),
		TestCase("TractorSemitrailer", 6.46, 73.5432, 3.75, 7.37814098),
		TestCase("TractorSemitrailer", 6.46, 92.8765, 4.0, 7.11234364),
		TestCase("TractorSemitrailer", 6.46, 100.449, 4.0, 7.03571556),
		TestCase("TractorSemitrailer", 6.46, 103, 3.6, 6.99454230),
		TestCase("TractorSemitrailer", 6.46, 105, 3.9, 6.99177143),
		TestCase("TractorSemitrailer", 6.46, 115, 4.0, 6.92267778),
		TestCase("TractorSemitrailer", 6.46, 130, 4.0, 6.83867361),
		Category(Definitions.TESTCASE_MIGRATED),]
		public void CrossWindCorrectionTest(string parameterSet, double crossSectionArea, double kmph, double height,
			double expected)
		{
			var crossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(crossSectionArea.SI<SquareMeter>(), 0.SI<SquareMeter>(),
                new AirdragDataAdapter().GetDeclarationAirResistanceCurve(parameterSet,
					crossSectionArea.SI<SquareMeter>(),
					height.SI<Meter>()),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			var tmp = crossWindCorrectionCurve.EffectiveAirDragArea(kmph.KMPHtoMeterPerSecond());
			AssertHelper.AreRelativeEqual(expected, tmp.Value(), toleranceFactor: 1e-3);
		}

		[TestCase("TractorSemitrailer", 5.8, 4.0),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void CrossWindGetDeclarationAirResistance(string parameterSet, double cdxa0, double height)
		{
			var curve =
				new AirdragDataAdapter().GetDeclarationAirResistanceCurve(parameterSet, cdxa0.SI<SquareMeter>(),
					height.SI<Meter>());

			AssertHelper.AreRelativeEqual(60.KMPHtoMeterPerSecond(), curve[1].Velocity);
			AssertHelper.AreRelativeEqual(7.0418009.SI<SquareMeter>(), curve[1].EffectiveCrossSectionArea);

			AssertHelper.AreRelativeEqual(65.KMPHtoMeterPerSecond(), curve[2].Velocity);
			AssertHelper.AreRelativeEqual(6.90971991.SI<SquareMeter>(), curve[2].EffectiveCrossSectionArea);

			AssertHelper.AreRelativeEqual(85.KMPHtoMeterPerSecond(), curve[6].Velocity);
			AssertHelper.AreRelativeEqual(6.54224222.SI<SquareMeter>(), curve[6].EffectiveCrossSectionArea);

			AssertHelper.AreRelativeEqual(100.KMPHtoMeterPerSecond(), curve[9].Velocity);
			AssertHelper.AreRelativeEqual(6.37434824.SI<SquareMeter>(), curve[9].EffectiveCrossSectionArea);

			AssertHelper.AreRelativeEqual(105.KMPHtoMeterPerSecond(), curve[10].Velocity);
			AssertHelper.AreRelativeEqual(6.33112792.SI<SquareMeter>(), curve[10].EffectiveCrossSectionArea);


			Assert.AreEqual(16, curve.Count);
		}

		[
			TestCase("TractorSemitrailer", 6.46, -0.1, 3.0),
			TestCase("TractorSemitrailer", 6.46, 200.1, 3.0),
			Category(Definitions.TESTCASE_MIGRATED),
		]
		public void CrossWindCorrectionExceptionTest(string parameterSet, double crossSectionArea, double kmph,
			double height)
		{
			var crossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(crossSectionArea.SI<SquareMeter>(), 0.SI<SquareMeter>(),
                new AirdragDataAdapter().GetDeclarationAirResistanceCurve(parameterSet,
					crossSectionArea.SI<SquareMeter>(),
					height.SI<Meter>()),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			AssertHelper.Exception<VectoException>(() =>
				crossWindCorrectionCurve.EffectiveAirDragArea(kmph.KMPHtoMeterPerSecond()));
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void CrossWindAreaCdxANotSet_DeclarationMode()
		{
			var airDrag = new AirdragData() {
				CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection,
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(null, null, null, CrossWindCorrectionMode.DeclarationModeCorrection)
			};

			Assert.IsTrue(airDrag.IsValid(),
				"In Speed Dependent (Declaration Mode) Crosswind Correction the CdxA Value can be empty.");
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void CrossWindAreaCdxANotSet_Other()
		{
			foreach (var correctionMode in EnumHelper.GetValues<CrossWindCorrectionMode>()) {
				if (correctionMode == CrossWindCorrectionMode.DeclarationModeCorrection) {
					continue;
				}

				var airDrag = new AirdragData {
					CrossWindCorrectionMode = correctionMode,
					CrossWindCorrectionCurve =
						new CrosswindCorrectionCdxALookup(null, null, null, correctionMode)
				};

				Assert.IsFalse(airDrag.IsValid(),
					"Only in Speed Dependent (Declaration Mode) Crosswind Correction the CdxA Value can be empty.");
			}
		}

		//Heavy Lorry
		[TestCase(VehicleClass.Class6, MissionType.LongHaul, "Standard technology", 1200, 0.7),
		TestCase(VehicleClass.Class6, MissionType.RegionalDelivery, "Standard technology", 1000, 0.7),
		TestCase(VehicleClass.Class6, MissionType.UrbanDelivery, "Standard technology", 1000, 0.7),
		TestCase(VehicleClass.Class6, MissionType.MunicipalUtility, "Standard technology", 1000, 0.7),
		TestCase(VehicleClass.Class6, MissionType.Construction, "Standard technology", 1000, 0.7),
		TestCase(VehicleClass.Class6, MissionType.LongHaul, "Standard technology - LED headlights, all", 1150, 0.7),
		TestCase(VehicleClass.Class6, MissionType.RegionalDelivery, "Standard technology - LED headlights, all", 950,
			0.7),
		TestCase(VehicleClass.Class6, MissionType.UrbanDelivery, "Standard technology - LED headlights, all", 950, 0.7),
		TestCase(VehicleClass.Class6, MissionType.MunicipalUtility, "Standard technology - LED headlights, all", 950,
			0.7),
		TestCase(VehicleClass.Class6, MissionType.Construction, "Standard technology - LED headlights, all", 950, 0.7),]

		//Medium Lorry
		[TestCase(VehicleClass.Class51, MissionType.RegionalDelivery, "Standard technology", 600, 0.7),
		TestCase(VehicleClass.Class52, MissionType.UrbanDelivery, "Standard technology", 600, 0.7),
		TestCase(VehicleClass.Class53, MissionType.RegionalDelivery, "Standard technology - LED headlights, all", 550,
			0.7),
		TestCase(VehicleClass.Class55, MissionType.UrbanDelivery, "Standard technology - LED headlights, all", 550,
			0.7)]
        [TestCase(VehicleClass.Class55, MissionType.LongHaul, "Standard technology", 720, 0.7),
        TestCase(VehicleClass.Class55, MissionType.LongHaul, "Standard technology - LED headlights, all", 660, 0.7),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void AuxElectricSystemTest(VehicleClass hdvClass, MissionType mission, string technology, double value,
			double efficiency)
		{
			AssertHelper.AreRelativeEqual(value / efficiency,
				DeclarationData.ElectricSystem.Lookup(hdvClass, mission, technology).PowerDemand.Value());
		}

		//Heavy Lorry
		[TestCase(VehicleClass.Class6, MissionType.Interurban, "Standard technology"),
		TestCase(VehicleClass.Class6, MissionType.LongHaul, "Standard technology - Flux-Compensator")]
		//Medium Lorry
		//[TestCase(VehicleClass.Class55, MissionType.LongHaul, "Standard technology"),
		[TestCase(VehicleClass.Class55, MissionType.UrbanDelivery, "Standard technology - Flux-Compensator"),
		Category(Definitions.TESTCASE_MIGRATED)]
		//TestCase(VehicleClass.Class55, MissionType.LongHaul, "Standard technology - LED headlights, all")]

		public void AuxElectricSystem_NotExistingError(VehicleClass hdvClass, MissionType mission, string technology)
        {
            AssertHelper.Exception<VectoException>(() => { DeclarationData.ElectricSystem.Lookup(hdvClass, mission, technology); });
        }

        [TestCase("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", 50),
        TestCase("only the drive shaft of the PTO - multi-disc clutch", 350),
        TestCase("only the drive shaft of the PTO - multi-disc clutch, oil pump", 3000),
        TestCase("drive shaft and/or up to 2 gear wheels - shift claw, synchronizer, sliding gearwheel", 150),
        TestCase("drive shaft and/or up to 2 gear wheels - multi-disc clutch", 400),
        TestCase("drive shaft and/or up to 2 gear wheels - multi-disc clutch, oil pump", 3050),
        TestCase("drive shaft and/or more than 2 gear wheels - shift claw, synchronizer, sliding gearwheel", 200),
        TestCase("drive shaft and/or more than 2 gear wheels - multi-disc clutch", 450),
        TestCase("drive shaft and/or more than 2 gear wheels - multi-disc clutch, oil pump", 3100),
        TestCase("PTO which includes 1 or more additional gearmesh(es), without disconnect clutch", 1500),
        TestCase("only one engaged gearwheel above oil level", 0),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void AuxPTOTransmissionTest(string technology, double value)
        {
            AssertHelper.AreRelativeEqual(value, DeclarationData.PTOTransmission.Lookup(technology).PowerDemand.Value());
        }

        [TestCase("Superfluid"),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void AuxPTOTransmission_NotExistingError(string technology)
        {
            AssertHelper.Exception<VectoException>(() => { DeclarationData.PTOTransmission.Lookup(technology); });
        }

        [TestCase(VehicleClass.Class1, "", new[] { 618, 671, 516, 566, 1037 }),
        TestCase(VehicleClass.Class1, "Crankshaft mounted - Electronically controlled visco clutch", new[] { 618, 671, 516, 566, 1037 }),
        TestCase(VehicleClass.Class1, "Crankshaft mounted - Bimetallic controlled visco clutch", new[] { 818, 871, 676, 766, 1277 }),
        TestCase(VehicleClass.Class1, "Crankshaft mounted - Discrete step clutch", new[] { 668, 721, 616, 616, 1157 }),
        TestCase(VehicleClass.Class1, "Crankshaft mounted - On/off clutch", new[] { 718, 771, 666, 666, 1237 }),
        TestCase(VehicleClass.Class1, "Belt driven or driven via transm. - Electronically controlled visco clutch",
            new[] { 989, 1044, 833, 933, 1478 }),
        TestCase(VehicleClass.Class1, "Belt driven or driven via transm. - Bimetallic controlled visco clutch",
            new[] { 1189, 1244, 993, 1133, 1718 }),
        TestCase(VehicleClass.Class1, "Belt driven or driven via transm. - Discrete step clutch", new[] { 1039, 1094, 983, 983, 1598 }),
        TestCase(VehicleClass.Class1, "Belt driven or driven via transm. - On/off clutch", new[] { 1089, 1144, 1033, 1033, 1678 }),
        TestCase(VehicleClass.Class1, "Hydraulic driven - Variable displacement pump", new[] { 938, 1155, 832, 917, 1872 }),
        TestCase(VehicleClass.Class1, "Hydraulic driven - Constant displacement pump", new[] { 1200, 1400, 1000, 1100, 2300 }),
        TestCase(VehicleClass.Class1, "Electrically driven - Electronically controlled", new[] { 700, 800, 600, 600, 1400 }),

        //medium lorries
        TestCase(VehicleClass.Class51, "", new[] { 0, 258, 198, 0, 0 }),
        TestCase(VehicleClass.Class51, "Crankshaft mounted - Electronically controlled visco clutch", new[] { 0, 258, 198, 0, 0 }),
        TestCase(VehicleClass.Class51, "Crankshaft mounted - Bimetallic controlled visco clutch", new[] { 0, 335, 260, 0, 0 }),
        TestCase(VehicleClass.Class51, "Crankshaft mounted - Discrete step clutch", new[] { 0, 277, 237, 0, 0 }),
        TestCase(VehicleClass.Class51, "Crankshaft mounted - On/off clutch", new[] { 0, 297, 256, 0, 0 }),
        TestCase(VehicleClass.Class51, "Belt driven or driven via transm. - Electronically controlled visco clutch",
            new[] { 0, 402, 320, 0, 0 }),
        TestCase(VehicleClass.Class51, "Belt driven or driven via transm. - Bimetallic controlled visco clutch",
            new[] { 0, 478, 382, 0, 0 }),
        TestCase(VehicleClass.Class51, "Belt driven or driven via transm. - Discrete step clutch", new[] { 0, 421, 378, 0, 0 }),
        TestCase(VehicleClass.Class51, "Belt driven or driven via transm. - On/off clutch", new[] { 0, 440, 397, 0, 0 }),
        TestCase(VehicleClass.Class51, "Hydraulic driven - Variable displacement pump", new[] { 0, 444, 320, 0, 0 }),
        TestCase(VehicleClass.Class51, "Hydraulic driven - Constant displacement pump", new[] { 0, 538, 385, 0, 0 }),
        TestCase(VehicleClass.Class51, "Electrically driven - Electronically controlled", new[] { 0, 308, 231, 0, 0 }),
		Category(Definitions.TESTCASE_MIGRATED)
        ]
		public void AuxFanTechTest(VehicleClass vehicleClass, string technology, int[] expected)
        {
            for (var i = 0; i < _missions.Length; i++)
            {
                if (expected[i] == 0)
                    continue;
                var lookup = DeclarationData.Fan.LookupPowerDemand(vehicleClass, _missions[i], technology);
                Assert.AreEqual(expected[i], lookup.Value(), Tolerance);
            }
        }

        [TestCase(VehicleClass.Class1, "Superfluid Hydraulic", MissionType.LongHaul, TestName = "AuxFanTechError( wrong tech )"),
        TestCase(VehicleClass.Class1, "Hydraulic driven - Electronically controlled", MissionType.Coach,
            TestName = "AuxFanTechError( wrong mission )"),
		Category(Definitions.TESTCASE_MIGRATED)
        ]
        public void AuxFanTechError(VehicleClass vehicleClass, string technology, MissionType missionType)
        {
            AssertHelper.Exception<VectoException>(() => DeclarationData.Fan.LookupPowerDemand(vehicleClass, missionType, technology));
        }

        [TestCase(VehicleClass.Class51, new[] { 200, 150, 150, -1, -1 }),
		TestCase(VehicleClass.Class52, new[] { 200, 150, 150, -1, -1 }),
		TestCase(VehicleClass.Class53, new[] { 200, 150, 150, -1, -1 }),
		TestCase(VehicleClass.Class54, new[] { 200, 150, 150, -1, -1 }),
		TestCase(VehicleClass.Class55, new[] { 200, 150, 150, -1, -1 }),
		TestCase(VehicleClass.Class56, new[] { 200, 150, 150, -1, -1 }),
		TestCase(VehicleClass.Class1s, new[] { 200, 150, 150, -1, -1 }),
		TestCase(VehicleClass.Class1, new[] { 200, 150, 150, -1, -1 }),
		TestCase(VehicleClass.Class2, new[] { 200, 200, 150, -1, -1 }),
		TestCase(VehicleClass.Class3, new[] { 200, 200, 150, -1, -1 }),
		TestCase(VehicleClass.Class4, new[] { 350, 200, 150, 300, 200 }),
		TestCase(VehicleClass.Class5, new[] { 350, 200, 150, -1, 200 }),
		TestCase(VehicleClass.Class9, new[] { 350, 200, 150, 300, 200 }),
		TestCase(VehicleClass.Class10, new[] { 350, 200, 150, -1, 200 }),
		TestCase(VehicleClass.Class11, new[] { 350, 200, 150, 300, 200 }),
		TestCase(VehicleClass.Class12, new[] { 350, 200, 150, -1, 200 }),
		TestCase(VehicleClass.Class16, new[] { 350, 200, 150, -1, 200 }),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void AuxHeatingVentilationAirConditionTest_Default(VehicleClass vehicleClass, int[] expected)
        {
            for (var i = 0; i < expected.Length; i++)
            {
                if (expected[i] >= 0)
                {
                    AssertHelper.AreRelativeEqual(expected[i],
                        DeclarationData.HeatingVentilationAirConditioning.Lookup(_missions[i], "Default", vehicleClass)
                            .PowerDemand.Value());
                }
                else
                {
                    var i1 = i;
                    AssertHelper.Exception<VectoException>(
                        () => DeclarationData.HeatingVentilationAirConditioning.Lookup(_missions[i1], "Default", vehicleClass));
                }
            }
        }

        [
        TestCase(VehicleClass.Class51, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class52, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class53, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class54, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class55, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class56, new[] { 0, 0, 0, 0, 0 }),
		TestCase(VehicleClass.Class1s, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class1, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class2, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class3, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class4, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class5, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class9, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class10, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class11, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class12, new[] { 0, 0, 0, 0, 0 }),
        TestCase(VehicleClass.Class16, new[] { 0, 0, 0, 0, 0 }),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void AuxHeatingVentilationAirConditionTest_None(VehicleClass vehicleClass, int[] expected)
        {
            for (var i = 0; i < expected.Length; i++)
            {
                AssertHelper.AreRelativeEqual(expected[i],
                    DeclarationData.HeatingVentilationAirConditioning.Lookup(_missions[i], "None", vehicleClass).PowerDemand.Value());
            }
        }

        [TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void AuxHeatingVentilationAirConditionTechnologyTest()
        {
            var tech = DeclarationData.HeatingVentilationAirConditioning.GetTechnologies();
            Assert.AreEqual(2, tech.Length);
            Assert.IsTrue(tech.Contains("Default"));
            Assert.IsTrue(tech.Contains("None"));
        }

        [Test,
        TestCase("Small", new[] { 1400, 1300, 1200, 1200, 1300 }),
        TestCase("Small + ESS", new[] { 900, 800, 800, 800, 800 }),
        TestCase("Small + visco clutch", new[] { 800, 700, 700, 700, 700 }),
        TestCase("Small + mech. clutch", new[] { 600, 600, 650, 650, 600 }),
        TestCase("Small + ESS + AMS", new[] { 500, 400, 500, 500, 400 }),
        TestCase("Small + visco clutch + AMS", new[] { 400, 300, 400, 400, 300 }),
        TestCase("Small + mech. clutch + AMS", new[] { 200, 200, 350, 350, 200 }),
        TestCase("Medium Supply 1-stage", new[] { 1600, 1400, 1350, 1350, 1500 }),
        TestCase("Medium Supply 1-stage + ESS", new[] { 1000, 900, 900, 900, 900 }),
        TestCase("Medium Supply 1-stage + visco clutch", new[] { 850, 800, 800, 800, 750 }),
        TestCase("Medium Supply 1-stage + mech. clutch", new[] { 600, 550, 550, 550, 600 }),
        TestCase("Medium Supply 1-stage + ESS + AMS", new[] { 600, 700, 700, 700, 500 }),
        TestCase("Medium Supply 1-stage + visco clutch + AMS", new[] { 450, 600, 600, 600, 350 }),
        TestCase("Medium Supply 1-stage + mech. clutch + AMS", new[] { 200, 350, 350, 350, 200 }),
        TestCase("Medium Supply 2-stage", new[] { 2100, 1750, 1700, 1700, 2100 }),
        TestCase("Medium Supply 2-stage + ESS", new[] { 1100, 1050, 1000, 1000, 1000 }),
        TestCase("Medium Supply 2-stage + visco clutch", new[] { 1000, 850, 800, 800, 900 }),
        TestCase("Medium Supply 2-stage + mech. clutch", new[] { 700, 650, 600, 600, 800 }),
        TestCase("Medium Supply 2-stage + ESS + AMS", new[] { 700, 850, 800, 800, 500 }),
        TestCase("Medium Supply 2-stage + visco clutch + AMS", new[] { 600, 650, 600, 600, 400 }),
        TestCase("Medium Supply 2-stage + mech. clutch + AMS", new[] { 300, 450, 400, 400, 300 }),
        TestCase("Large Supply", new[] { 4300, 3600, 3500, 3500, 4100 }),
        TestCase("Large Supply + ESS", new[] { 1600, 1300, 1200, 1200, 1500 }),
        TestCase("Large Supply + visco clutch", new[] { 1300, 1100, 1000, 1000, 1200 }),
        TestCase("Large Supply + mech. clutch", new[] { 800, 800, 700, 700, 900 }),
        TestCase("Large Supply + ESS + AMS", new[] { 1100, 1000, 1000, 1000, 1000 }),
        TestCase("Large Supply + visco clutch + AMS", new[] { 800, 800, 800, 800, 700 }),
        TestCase("Large Supply + mech. clutch + AMS", new[] { 300, 500, 500, 500, 400 }),
        TestCase("Vacuum pump", new[] { 190, 160, 130, 130, 130 }),
		Category(Definitions.TESTCASE_MIGRATED),
        ]
        public void AuxPneumaticSystemTest(string technology, int[] expected)
        {
            for (var i = 0; i < _missions.Length; i++)
            {
                var lookup = DeclarationData.PneumaticSystem.Lookup(_missions[i], technology);
                AssertHelper.AreRelativeEqual(expected[i], lookup.PowerDemand.Value());
            }
        }

        [
            TestCase(MissionType.LongHaul, VehicleClass.Class2, 370, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.LongHaul, VehicleClass.Class4, 610, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.LongHaul, VehicleClass.Class5, 720, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.LongHaul, VehicleClass.Class9, 720, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.LongHaul, VehicleClass.Class10, 570, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.LongHaul, VehicleClass.Class11, 720, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.LongHaul, VehicleClass.Class12, 570, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class1s, 280, 0, "Fixed displacement", null, null, null),
			TestCase(MissionType.RegionalDelivery, VehicleClass.Class1, 280, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 340, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class3, 370, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class4, 570, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class5, 670, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class9, 590, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class10, 570, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class11, 590, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class12, 570, 0, "Fixed displacement", null, null, null),
			TestCase(MissionType.UrbanDelivery, VehicleClass.Class1s, 270, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.UrbanDelivery, VehicleClass.Class1, 270, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.UrbanDelivery, VehicleClass.Class2, 310, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.UrbanDelivery, VehicleClass.Class3, 350, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.UrbanDelivery, VehicleClass.Class5, 620, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.MunicipalUtility, VehicleClass.Class4, 510, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.MunicipalUtility, VehicleClass.Class9, 510, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.MunicipalUtility, VehicleClass.Class11, 510, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.Construction, VehicleClass.Class11, 770, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.Construction, VehicleClass.Class12, 770, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.Construction, VehicleClass.Class16, 770, 0, "Fixed displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 325.5, 0, "Fixed displacement with elec. control", null,
                null,
                null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 289, 0, "Dual displacement", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 255, 0, "Variable displacement mech. controlled", null,
                null,
                null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 204, 0, "Variable displacement elec. controlled", null,
                null,
                null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 92.87, 0, "Electric", null, null, null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 665, 0, "Fixed displacement", "Fixed displacement", null,
                null),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 1295, 0, "Fixed displacement", "Fixed displacement",
                "Fixed displacement", "Fixed displacement"),
            TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 1021.5, 0, 
				"Dual displacement",
                "Variable displacement mech. controlled", 
				"Fixed displacement with elec. control",
                "Variable displacement elec. controlled"),

            //Electric and Mechanic
			TestCase(MissionType.UrbanDelivery, 
				VehicleClass.Class5, 
				852.048, 
				768.72,
                "Fixed displacement",
				"Dual displacement",
                "Electric driven pump",
                "Full electric steering gear"),
			TestCase(MissionType.UrbanDelivery, 
				VehicleClass.Class5, 
				0, 
				262.386,
                "Full electric steering gear",
				"Full electric steering gear",
				"Full electric steering gear",
				"Full electric steering gear"),
			Category(Definitions.TESTCASE_MIGRATED),
        ]
        public void Aux_SteeringPumpLookupValues(MissionType mission, VehicleClass hdvClass, double expectedMech, double expectedElectric, string axle1,
            string axle2, string axle3, string axle4)
		{
			var result = DeclarationData.SteeringPump.Lookup(mission, hdvClass,
				new[] { axle1, axle2, axle3, axle4 }.TakeWhile(a => a != null).ToArray());
            AssertHelper.AreRelativeEqual(expectedMech, result.mechanicalPumps);
            AssertHelper.AreRelativeEqual(expectedElectric, result.electricPumps);
		}

        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)] 
        public void Aux_SteeringpumpMultipleLookups()
        {
            // testcase to illustrate modification of lookup-data for steering pump
            const string axle1 = "Electric";
            const MissionType mission = MissionType.LongHaul;
            const VehicleClass hdvClass = VehicleClass.Class5;
            var first = DeclarationData.SteeringPump.Lookup(mission, hdvClass,
                new[] { axle1 }.TakeWhile(a => a != null).ToArray()).mechanicalPumps;

            for (var i = 0; i < 10; i++)
            {
                DeclarationData.SteeringPump.Lookup(mission, hdvClass,
                    new[] { axle1 }.TakeWhile(a => a != null).ToArray());
            }

            var last = DeclarationData.SteeringPump.Lookup(mission, hdvClass,
                new[] { axle1 }.TakeWhile(a => a != null).ToArray()).mechanicalPumps;

            Assert.AreEqual(first.Value(), last.Value(), 1e-3);
        }

        [TestCase(MissionType.LongHaul, VehicleClass.Class1s, 
			TestName = "Aux_SteeringPumpLookupFail class 1s ( No Value )"),
		TestCase(MissionType.LongHaul, VehicleClass.Class1, 
            TestName = "Aux_SteeringPumpLookupFail class 1 ( No Value )"),
        TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, "Super displacement",
            TestName = "Aux_SteeringPumpLookupFail class 2 ( Wrong Tech )"),
        TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, "Dual displacement", "Dual displacement",
            "Dual displacement", "Dual displacement", "Dual displacement", TestName = "Aux_SteeringPumpLookupFail class 2( >4 Techs )"),
        TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, TestName = "Aux_SteeringPumpLookupFail class 2( Null Techs )"),
        TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, new string[0],
            TestName = "Aux_SteeringPumpLookupFail class 2 ( 0 Techs )"),
		Category(Definitions.TESTCASE_MIGRATED),
        ]
        public void Aux_SteeringPumpLookupFail(MissionType mission, VehicleClass hdvClass, params string[] tech)
        {
            AssertHelper.Exception<VectoException>(() => DeclarationData.SteeringPump.Lookup(mission, hdvClass, tech));
        }

        [
            TestCase(0),
            TestCase(1000),
            TestCase(3500),
			Category(Definitions.TESTCASE_MIGRATED),
            //TestCase(7500)
        ]
        public void SegmentWeightOutOfRange4X2(double weight)
        {
            AssertHelper.Exception<VectoException>(() =>
				DeclarationData.TruckSegments.Lookup(
                    VehicleCategory.RigidTruck,
                    AxleConfiguration.AxleConfig_4x2,
                    weight.SI<Kilogram>(),
                    0.SI<Kilogram>(),
                    false),
                $"ERROR: Could not find the declaration segment for vehicle. " +
                $"Category: {VehicleCategory.RigidTruck}, " +
                $"AxleConfiguration: {AxleConfiguration.AxleConfig_4x2.GetName()}, " +
                $"GrossVehicleWeight: {weight.SI<Kilogram>()}");
        }

        [
            TestCase(0),
            TestCase(1000),
            TestCase(3500),
            TestCase(7500),
			Category(Definitions.TESTCASE_MIGRATED)
        ]
        public void SegmentWeightOutOfRange4X4(double weight)
        {
            AssertHelper.Exception<VectoException>(() =>
				DeclarationData.TruckSegments.Lookup(
                    VehicleCategory.RigidTruck,
                    AxleConfiguration.AxleConfig_4x4,
                    weight.SI<Kilogram>(),
                    0.SI<Kilogram>(),
                    false),
                $"ERROR: Could not find the declaration segment for vehicle. " +
                $"Category: {VehicleCategory.RigidTruck}, " +
                $"AxleConfiguration: {AxleConfiguration.AxleConfig_4x4.GetName()}, " +
                $"GrossVehicleWeight: {weight.SI<Kilogram>()}");
        }
        
        [Test,
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2F, 5000.01, 0, false, VehicleClass.Class51),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2F, 5000.01, 0, false, VehicleClass.Class52),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 5000.01, 0, false, VehicleClass.Class53),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 5000.01, 0, false, VehicleClass.Class53),
        TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 5000.01, 0, false, VehicleClass.Class54),
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 5000.01, 0, false, VehicleClass.Class55),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 5000.01, 0, false, VehicleClass.Class55),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x4, 5000.01, 0, false, VehicleClass.Class56),

		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7400.01, 0, false, VehicleClass.Class1s),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7400.01, 0, false, VehicleClass.Class1s),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500.01, 0, false, VehicleClass.Class1),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500.01, 0, false, VehicleClass.Class1),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1),

        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10001, 0, false, VehicleClass.Class2),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10001, 0, false, VehicleClass.Class2),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2),

        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12001, 0, false, VehicleClass.Class3),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12001, 0, false, VehicleClass.Class3),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3),

        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, false, VehicleClass.Class4),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 99000, 0, false, VehicleClass.Class4),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, true, VehicleClass.Class4),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 99000, 0, true, VehicleClass.Class4),

        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, false, VehicleClass.Class5),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 99000, 0, false, VehicleClass.Class5),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, true, VehicleClass.Class5),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 99000, 0, true, VehicleClass.Class5),

        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 7500, 0, VehicleClass.Class6),
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 16000, 0, VehicleClass.Class6),
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 16001, 0, VehicleClass.Class7),
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 99000, 0, VehicleClass.Class7),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 16000, 0, VehicleClass.Class8),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 99000, 0, VehicleClass.Class8),

        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7500, 0, false, VehicleClass.Class9),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class9),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class9),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 99000, 0, false, VehicleClass.Class9),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7500, 0, true, VehicleClass.Class9),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 99000, 0, true, VehicleClass.Class9),

        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, false, VehicleClass.Class10),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class10),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class10),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 99000, 0, false, VehicleClass.Class10),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, true, VehicleClass.Class10),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 99000, 0, true, VehicleClass.Class10),

        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 7500, 0, false, VehicleClass.Class11),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 40000, 0, false, VehicleClass.Class11),

        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 7500, 0, false, VehicleClass.Class12),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 99000, 0, false, VehicleClass.Class12),

        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 7500, 0, false, VehicleClass.Class16),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 99000, 0, false, VehicleClass.Class16),
            Category(Definitions.TESTCASE_MIGRATED),
        ]
        public void SegmentLookupTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
            double curbWeight, bool vocational, VehicleClass expectedClass)
        {
			var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);
            Assert.AreEqual(expectedClass, segment.VehicleClass);
        }

        [
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2F, 5001, 0, false, VehicleClass.Class51, 85),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2F, 5001, 0, false, VehicleClass.Class52, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 5001, 0, false, VehicleClass.Class53, 85),
        TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 5001, 0, false, VehicleClass.Class54, 85),
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 5001, 0, false, VehicleClass.Class55, 85),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x4, 5001, 0, false, VehicleClass.Class56, 85),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7401, 0, false, VehicleClass.Class1s, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7501, 0, false, VehicleClass.Class1, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10001, 0, false, VehicleClass.Class2, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12001, 0, false, VehicleClass.Class3, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, false, VehicleClass.Class4, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, true, VehicleClass.Class4, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, false, VehicleClass.Class5, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, true, VehicleClass.Class5, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7501, 0, false, VehicleClass.Class9, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7501, 0, true, VehicleClass.Class9, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, false, VehicleClass.Class10, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, true, VehicleClass.Class10, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 7500, 0, false, VehicleClass.Class11, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 7500, 0, false, VehicleClass.Class12, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 7500, 0, false, VehicleClass.Class16, 85),
		Category(Definitions.TESTCASE_MIGRATED),]
        public void SegmentDesignSpeedTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
            double curbWeight, bool vocational, VehicleClass expectedClass, double speed)
        {
			var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);

            Assert.AreEqual(speed.KMPHtoMeterPerSecond(), segment.DesignSpeed);
        }

        [Test,
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class51, 800, null,
        //    TestName = "SegmentLookupBodyWeight Class51 Rigid"),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class51, 800, null,
        //    TestName = "SegmentLookupBodyWeight Class51 Tractor"),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class52, 0, null,
        //    TestName = "SegmentLookupBodyWeight Class52 Van"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class53, 800, null,
            TestName = "SegmentLookupBodyWeight Class53 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class53, 800, null,
            TestName = "SegmentLookupBodyWeight Class53 Tractor"),
        TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class54, 0, null,
            TestName = "SegmentLookupBodyWeight Class54 Van"),
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class55, 800, null,
        //    TestName = "SegmentLookupBodyWeight Class55 Rigid"),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class55, 800, null,
        //    TestName = "SegmentLookupBodyWeight Class55 Tractor"),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class56, 0, null,
        //    TestName = "SegmentLookupBodyWeight ClassML4rvan Van"),
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s, 1600, null,
			TestName = "SegmentLookupBodyWeight Class1s Rigid"),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s, 1600, null,
			TestName = "SegmentLookupBodyWeight Class1s Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1, 1600, null,
            TestName = "SegmentLookupBodyWeight Class1 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1, 1600, null,
            TestName = "SegmentLookupBodyWeight Class1 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2, 1900, 3400,
            TestName = "SegmentLookupBodyWeight Class2 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2, 1900, 3400,
            TestName = "SegmentLookupBodyWeight Class2 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3, 2000, null,
            TestName = "SegmentLookupBodyWeight Class3 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3, 2000, null,
            TestName = "SegmentLookupBodyWeight Class3 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class4, 2100, 5400,
            TestName = "SegmentLookupBodyWeight Class4"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class5, null, 7500,
            TestName = "SegmentLookupBodyWeight Class5"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class9, 2200, 5400,
            TestName = "SegmentLookupBodyWeight Class9"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class10, null, 7500,
            TestName = "SegmentLookupBodyWeight Class10"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 12000, 0, false, VehicleClass.Class11, 2200, 5400,
            TestName = "SegmentLookupBodyWeight Class11"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 12000, 0, false, VehicleClass.Class12, null, 7500,
            TestName = "SegmentLookupBodyWeight Class12"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 12000, 0, false, VehicleClass.Class16, null, null,
            TestName = "SegmentLookupBodyWeight Class16"),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void SegmentLookupBodyTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
            double curbWeight, bool vocational, VehicleClass expectedClass, int? expectedBodyWeight, int? expectedTrailerWeight)
        {
			var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);
            Assert.AreEqual(expectedClass, segment.VehicleClass);

            if (expectedBodyWeight.HasValue)
            {
                Assert.AreEqual(expectedBodyWeight, segment.Missions[0].BodyCurbWeight.Value());
            }
            if (expectedTrailerWeight.HasValue)
            {
                var trailerMission = segment.Missions.Where(m => m.Trailer.Count > 0).ToList();
                if (trailerMission.Count > 0)
                {
                    Assert.AreEqual(expectedTrailerWeight, trailerMission.First().Trailer.First().TrailerCurbWeight.Value());
                }
            }
        }

        [Test,
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class51, 3.5,
        //    TestName = "SegmentLookupHeight Class51 Rigid"),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class51, 3.5,
        //    TestName = "SegmentLookupHeight Class51 Tractor"),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class52, 2.9,
        //    TestName = "SegmentLookupHeight Class52 Van"),

        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class53, 3.5,
            TestName = "SegmentLookupHeight Class53 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class53, 3.5,
            TestName = "SegmentLookupHeight Class53 Tractor"),
        TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class54, 2.9,
            TestName = "SegmentLookupHeight Class54 Van"),

        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class55, 3.5,
        //    TestName = "SegmentLookupHeight Class55 Rigid"),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class55, 3.5,
        //    TestName = "SegmentLookupHeight Class55 Tractor"),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class56, 2.9,
        //    TestName = "SegmentLookupHeight Class56 Van"),


		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s, 3.6,
			TestName = "SegmentLookupHeight Class1s Rigid"),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s, 3.6,
			TestName = "SegmentLookupHeight Class1s Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1, 3.6,
            TestName = "SegmentLookupHeight Class1 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1, 3.6,
            TestName = "SegmentLookupHeight Class1 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2, 3.75,
            TestName = "SegmentLookupHeight Class2 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2, 3.75,
            TestName = "SegmentLookupHeight Class2 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3, 3.9,
            TestName = "SegmentLookupHeight Class3 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3, 3.9,
            TestName = "SegmentLookupHeight Class3 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class4, 4.0,
            TestName = "SegmentLookupHeight Class4"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, true, VehicleClass.Class4, 4.0,
            TestName = "SegmentLookupHeight Class4v"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class5, 4.0,
            TestName = "SegmentLookupHeight Class5"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, true, VehicleClass.Class5, 4.0,
            TestName = "SegmentLookupHeight Class5v"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 10000, 0, false, VehicleClass.Class9, 3.6,
            TestName = "SegmentLookupHeight Class9 - 1"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 12000, 0, false, VehicleClass.Class9, 3.75,
            TestName = "SegmentLookupHeight Class9 - 2"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class9, 3.9,
            TestName = "SegmentLookupHeight Class9 - 3"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 18000, 0, false, VehicleClass.Class9, 4.0,
            TestName = "SegmentLookupHeight Class9 - 4"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class9, 4.0,
            TestName = "SegmentLookupHeight Class9 - other"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, true, VehicleClass.Class9, 4.0,
            TestName = "SegmentLookupHeight Class9v - other"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class10, 4.0,
            TestName = "SegmentLookupHeight Class10"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, true, VehicleClass.Class10, 4.0,
            TestName = "SegmentLookupHeight Class10v"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 12000, 0, false, VehicleClass.Class11, 4.0,
            TestName = "SegmentLookupHeight Class11"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 12000, 0, false, VehicleClass.Class12, 4.0,
            TestName = "SegmentLookupHeight Class12"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 12000, 0, false, VehicleClass.Class16, 3.6,
            TestName = "SegmentLookupHeight Class16"),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void SegmentLookupHeightTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
            double curbWeight, bool vocational, VehicleClass expectedClass, double expectedHeight)
        {
			var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);
            Assert.AreEqual(expectedClass, segment.VehicleClass);
			AssertHelper.AreRelativeEqual(expectedHeight, segment.Missions.First().VehicleHeight);
        }

        [Test,
		TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7401, 0, false, VehicleClass.Class1s,
			new[] { 36.5, 36.5 }),
		TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7401, 0, false, VehicleClass.Class1s,
			new[] { 36.5, 36.5 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7501, 0, false, VehicleClass.Class1,
            new[] { 36.5, 36.5 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7501, 0, false, VehicleClass.Class1,
            new[] { 36.5, 36.5 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2,
            new[] { 85.0, 45.2, 45.2 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2,
            new[] { 85.0, 45.2, 45.2 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3,
            new[] { 47.7, 47.7 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3,
            new[] { 47.7, 47.7 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class4,
            new[] { 98.9, 49.4, 49.4, 0.0, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, true, VehicleClass.Class4,
            new[] { 98.9, 49.4, 49.4, 0.0, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class5,
            new[] { 91.0, 140.5, 91.0, 140.5, 91.0, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, true, VehicleClass.Class5,
            new[] { 91.0, 140.5, 91.0, 140.5, 91.0, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class9,
            new[] { 101.4, 142.9, 51.9, 142.9, 0.0, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, true, VehicleClass.Class9,
            new[] { 101.4, 142.9, 51.9, 142.9, 0.0, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class10,
            new[] { 91.0, 140.5, 91.0, 140.5, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000, 0, true, VehicleClass.Class10,
            new[] { 91.0, 140.5, 91.0, 140.5, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 40000, 0, false, VehicleClass.Class11,
            new[] { 101.4, 142.9, 51.9, 142.9, 0.0, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 99000, 0, false, VehicleClass.Class12,
            new[] { 91.0, 140.5, 91.0, 140.5, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 99000, 0, false, VehicleClass.Class16,
            new[] { 101.4, 142.9, 51.9, 142.9, 0.0, }),
		Category(Definitions.TESTCASE_MIGRATED)
        ]
        public void SegmentLookupCargoVolumeTest(VehicleCategory category, AxleConfiguration axleConfiguration,
            double grossWeight,
            double curbWeight, bool vocational, VehicleClass expectedClass, double[] expectedCargoVolume)
        {
			var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);
            Assert.AreEqual(expectedClass, segment.VehicleClass);
            Assert.AreEqual(expectedCargoVolume.Length, segment.Missions.Length);
            for (var i = 0; i < expectedCargoVolume.Length; i++)
            {
                Assert.AreEqual(expectedCargoVolume[i], segment.Missions[i].TotalCargoVolume.Value());
            }
        }

        /// <summary>
        /// trailer in longhaul, always pc formula
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment2Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
                GrossVehicleMassRating = 11900.SI<Kilogram>(),
                CurbWeight = 5850.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class2, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(3, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaul,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.225, 0.325 },
                trailerAxleWeightDistribution: new[] { 0.45 },
                trailerAxleCount: new[] { 2 },
                bodyCurbWeight: 1900,
                trailerCurbWeight: new[] { 3400.0 },
                trailerType: new[] { TrailerType.T1 },
                lowLoad: 1296.8235,
                refLoad: 9450,
                trailerGrossVehicleWeight: new[] { 10500.0 },
                deltaCdA: 1.3,
                maxLoad: 11250);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.45, 0.55 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 1900,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 596.8235,
                refLoad: 2984.1176,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 4150);

            AssertMission(segment.Missions[2],
                vehicleData: vehicleData,
                missionType: MissionType.UrbanDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.45, 0.55 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 1900,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 596.8235,
                refLoad: 2984.1176,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 4150);
        }


        /// <summary>
        /// trailer in longhaul, always pc formula
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment2TestHeavy()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
                GrossVehicleMassRating = 11990.SI<Kilogram>(),
                CurbWeight = 9500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class2, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(3, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaul,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.225, 0.325 },
                trailerAxleWeightDistribution: new[] { 0.45 },
                trailerAxleCount: new[] { 2 },
                bodyCurbWeight: 1900,
                trailerCurbWeight: new[] { 3400.0 },
                trailerType: new[] { TrailerType.T1 },
                lowLoad: 1290,
                refLoad: 5890,
                trailerGrossVehicleWeight: new[] { 10500.0 },
                deltaCdA: 1.3,
                maxLoad: 7690);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.45, 0.55 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 1900,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 590,
                refLoad: 590,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 590);

            AssertMission(segment.Missions[2],
                vehicleData: vehicleData,
                missionType: MissionType.UrbanDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.45, 0.55 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 1900,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 590,
                refLoad: 590,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 590);
        }


        /// <summary>
        /// normal pc formula, no trailer
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment3Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
                GrossVehicleMassRating = 14000.SI<Kilogram>(),
                CurbWeight = 5850.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class3, segment.VehicleClass);

            Assert.AreEqual(2, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.4, 0.6 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 2000,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 762.3529,
                refLoad: 3811.7647,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 6150);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.UrbanDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.4, 0.6 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 2000,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 762.3529,
                refLoad: 3811.7647,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 6150);
        }

        /// <summary>
        /// fixed reference weight, trailer only in longhaul
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment4Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
                GrossVehicleMassRating = 18000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class4, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(4, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaul,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.2, 0.3 },
                trailerAxleWeightDistribution: new[] { 0.5 },
                trailerAxleCount: new[] { 2 },
                bodyCurbWeight: 2100,
                trailerCurbWeight: new[] { 5400.0 },
                trailerType: new[] { TrailerType.T2 },
                lowLoad: 1900,
                refLoad: 14000,
                trailerGrossVehicleWeight: new[] { 18000.0 },
                deltaCdA: 1.5,
                maxLoad: 21000);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.45, 0.55 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 2100,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 900,
                refLoad: 4400,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 8400);

            AssertMission(segment.Missions[2],
                        vehicleData: vehicleData,
                        missionType: MissionType.UrbanDelivery,
                        cosswindCorrection: "RigidSolo",
                        axleWeightDistribution: new[] { 0.45, 0.55 },
                        trailerAxleWeightDistribution: new double[] { },
                        trailerAxleCount: new int[] { },
                        bodyCurbWeight: 2100,
                        trailerCurbWeight: new double[] { },
                        trailerType: new TrailerType[] { },
                        lowLoad: 900,
                        refLoad: 4400,
                        trailerGrossVehicleWeight: new double[] { },
                        deltaCdA: 0,
                        maxLoad: 8400);

            AssertMission(segment.Missions[3],
                vehicleData: vehicleData,
                missionType: MissionType.MunicipalUtility,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.45, 0.55 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 6000,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 600,
                refLoad: 3000,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 4500);
        }

        /// <summary>
        /// fixed reference weight, trailer only in longhaul
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment4VocationalTest()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
                GrossVehicleMassRating = 18000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, true);

            Assert.AreEqual(VehicleClass.Class4, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(2, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.MunicipalUtility,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.45, 0.55 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 6000,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 600,
                refLoad: 3000,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 4500);

            AssertMission(segment.Missions[1],
                        vehicleData: vehicleData,
                        missionType: MissionType.Construction,
                        cosswindCorrection: "RigidSolo",
                        axleWeightDistribution: new[] { 0.45, 0.55 },
                        trailerAxleWeightDistribution: new double[] { },
                        trailerAxleCount: new int[] { },
                        bodyCurbWeight: 2000,
                        trailerCurbWeight: new double[] { },
                        trailerType: new TrailerType[] { },
                        lowLoad: 900,
                        refLoad: 4400,
                        trailerGrossVehicleWeight: new double[] { },
                        deltaCdA: 0,
                        maxLoad: 8500);
        }

        /// <summary>
        /// Segment 5: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment5Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.Tractor,
                AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
                GrossVehicleMassRating = 18000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class5, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(5, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaul,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.2, 0.25 },
                trailerAxleWeightDistribution: new[] { 0.55 },
                trailerAxleCount: new[] { 3 }, bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0 },
                trailerType: new[] { TrailerType.ST1 },
                lowLoad: 2600,
                refLoad: 19300,
                trailerGrossVehicleWeight: new[] { 24000.0 },
                deltaCdA: 0,
                maxLoad: 25000);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaulEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.15, 0.2 },
                trailerAxleWeightDistribution: new[] { 0.40, 0.25 },
                trailerAxleCount: new[] { 3, 2 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0, 5400 },
                trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
                lowLoad: 3500,
                refLoad: 26500,
                trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
                deltaCdA: 1.5,
                maxLoad: 39600,
                ems: true);

            AssertMission(segment.Missions[2],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.25, 0.25 },
                trailerAxleWeightDistribution: new[] { 0.5 },
                trailerAxleCount: new[] { 3 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0 },
                trailerType: new[] { TrailerType.ST1 },
                lowLoad: 2600,
                refLoad: 12900,
                trailerGrossVehicleWeight: new[] { 24000.0 },
                deltaCdA: 0, maxLoad: 25000);

            AssertMission(segment.Missions[3],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDeliveryEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.175, 0.25 },
                trailerAxleWeightDistribution: new[] { 0.35, 0.225 },
                trailerAxleCount: new[] { 3, 2 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0, 5400 },
                trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
                lowLoad: 3500,
                refLoad: 17500,
                trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
                deltaCdA: 1.5,
                maxLoad: 39600,
                ems: true);

			AssertMission(segment.Missions[4],
				vehicleData: vehicleData,
				missionType: MissionType.UrbanDelivery,
				cosswindCorrection: "TractorSemitrailer",
				axleWeightDistribution: new[] { 0.25, 0.25 },
				trailerAxleWeightDistribution: new[] { 0.5 },
				trailerAxleCount: new[] { 3 },
				bodyCurbWeight: 0,
				trailerCurbWeight: new[] { 7500.0 },
				trailerType: new[] { TrailerType.ST1 },
				lowLoad: 2600,
				refLoad: 12900,
				trailerGrossVehicleWeight: new[] { 24000.0 },
				deltaCdA: 0, maxLoad: 25000);
        }

        /// <summary>
        /// Segment 5: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment5VocationalTest()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.Tractor,
                AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
                GrossVehicleMassRating = 18000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, true);

            Assert.AreEqual(VehicleClass.Class5, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(1, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.Construction,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.25, 0.25 },
                trailerAxleWeightDistribution: new[] { 0.5 },
                trailerAxleCount: new[] { 3 }, bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 6100.0 },
                trailerType: new[] { TrailerType.STT1 },
                lowLoad: 2600,
                refLoad: 12900,
                trailerGrossVehicleWeight: new[] { 24000.0 },
                deltaCdA: 0,
                maxLoad: 26400);

        }

        /// <summary>
        /// Segment 9: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment9Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
                GrossVehicleMassRating = 24000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class9, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(5, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaul,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.2, 0.3, 0.15 },
                trailerAxleWeightDistribution: new[] { 0.35 },
                trailerAxleCount: new[] { 2 },
                bodyCurbWeight: 2200,
                trailerCurbWeight: new[] { 5400.0 },
                trailerType: new[] { TrailerType.T2 },
                lowLoad: 2600,
                refLoad: 19300,
                trailerGrossVehicleWeight: new[] { 18000.0 },
                deltaCdA: 1.5,
                maxLoad: 24900);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaulEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.15, 0.2, 0.1 },
                trailerAxleWeightDistribution: new[] { 0.225, 0.325 },
                trailerAxleCount: new[] { 2, 3 },
                bodyCurbWeight: 2200,
                trailerCurbWeight: new[] { 2500, 7500.0 },
                trailerType: new[] { TrailerType.Dolly, TrailerType.ST1 },
                lowLoad: 3500,
                refLoad: 26500,
                trailerGrossVehicleWeight: new[] { 12000.0, 24000 },
                deltaCdA: 2.1,
                maxLoad: 40300,
                ems: true);

            AssertMission(segment.Missions[2],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.35, 0.4, 0.25 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 2200,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 1400,
                refLoad: 7100,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 14300);

            AssertMission(segment.Missions[3],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDeliveryEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.175, 0.2, 0.1 },
                trailerAxleWeightDistribution: new[] { 0.225, 0.3 },
                trailerAxleCount: new[] { 2, 3 },
                bodyCurbWeight: 2200,
                trailerCurbWeight: new[] { 2500, 7500.0 },
                trailerType: new[] { TrailerType.Dolly, TrailerType.ST1 },
                lowLoad: 3500,
                refLoad: 17500,
                trailerGrossVehicleWeight: new[] { 12000.0, 24000 },
                deltaCdA: 2.1,
                maxLoad: 40300,
                ems: true);

            AssertMission(segment.Missions[4],
                vehicleData: vehicleData,
                missionType: MissionType.MunicipalUtility,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.35, 0.4, 0.25 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 6750,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 1200,
                refLoad: 6000,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 9750);
        }

        /// <summary>
        /// Segment 9: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment9VocationalTest()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
                GrossVehicleMassRating = 24000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, true);

            Assert.AreEqual(VehicleClass.Class9, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(2, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.MunicipalUtility,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.35, 0.4, 0.25 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 6750,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 1200,
                refLoad: 6000,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 9750);

            AssertMission(segment.Missions[1],
                        vehicleData: vehicleData,
                        missionType: MissionType.Construction,
                        cosswindCorrection: "RigidSolo",
                        axleWeightDistribution: new[] { 0.35, 0.4, 0.25 },
                        trailerAxleWeightDistribution: new double[] { },
                        trailerAxleCount: new int[] { },
                        bodyCurbWeight: 3230,
                        trailerCurbWeight: new double[] { },
                        trailerType: new TrailerType[] { },
                        lowLoad: 1400,
                        refLoad: 7100,
                        trailerGrossVehicleWeight: new double[] { },
                        deltaCdA: 0,
                        maxLoad: 13270);

        }

        /// <summary>
        /// Segment 10: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment10Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.Tractor,
                AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
                GrossVehicleMassRating = 24000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class10, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(4, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaul,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.15, 0.1, 0.2 },
                trailerAxleWeightDistribution: new[] { 0.55 },
                trailerAxleCount: new[] { 3 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0 },
                trailerType: new[] { TrailerType.ST1 },
                lowLoad: 2600,
                refLoad: 19300,
                trailerGrossVehicleWeight: new[] { 24000.0 },
                deltaCdA: 0,
                maxLoad: 25000);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaulEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.125, 0.15, 0.1 },
                trailerAxleWeightDistribution: new[] { 0.375, 0.25 },
                trailerAxleCount: new[] { 3, 2 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0, 5400 },
                trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
                lowLoad: 3500,
                refLoad: 26500,
                trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
                deltaCdA: 1.5,
                maxLoad: 39600,
                ems: true);

            AssertMission(segment.Missions[2],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.2, 0.1, 0.2 },
                trailerAxleWeightDistribution: new[] { 0.5 },
                trailerAxleCount: new[] { 3 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0 },
                trailerType: new[] { TrailerType.ST1 },
                lowLoad: 2600,
                refLoad: 12900,
                trailerGrossVehicleWeight: new[] { 24000.0 },
                deltaCdA: 0,
                maxLoad: 25000);

            AssertMission(segment.Missions[3],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDeliveryEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.15, 0.15, 0.1 },
                trailerAxleWeightDistribution: new[] { 0.35, 0.25 },
                trailerAxleCount: new[] { 3, 2 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0, 5400 },
                trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
                lowLoad: 3500,
                refLoad: 17500,
                trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
                deltaCdA: 1.5,
                maxLoad: 39600,
                ems: true);
        }

        /// <summary>
        /// Segment 10: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment10VocationalTest()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.Tractor,
                AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
                GrossVehicleMassRating = 24000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, true);

            Assert.AreEqual(VehicleClass.Class10, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(1, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.Construction,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.2, 0.1, 0.2 },
                trailerAxleWeightDistribution: new[] { 0.5 },
                trailerAxleCount: new[] { 2 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 5600.0 },
                trailerType: new[] { TrailerType.STT2 },
                lowLoad: 2600,
                refLoad: 12900,
                trailerGrossVehicleWeight: new[] { 18000.0 },
                deltaCdA: 0,
                maxLoad: 26900);

        }

        /// <summary>
        /// Segment 11: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment11Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_6x4,
                GrossVehicleMassRating = 24000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class11, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(6, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaul,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.2, 0.225, 0.225 },
                trailerAxleWeightDistribution: new[] { 0.35 },
                trailerAxleCount: new[] { 2 },
                bodyCurbWeight: 2200,
                trailerCurbWeight: new[] { 5400.0 },
                trailerType: new[] { TrailerType.T2 },
                lowLoad: 2600,
                refLoad: 19300,
                trailerGrossVehicleWeight: new[] { 18000.0 },
                deltaCdA: 1.5,
                maxLoad: 24900);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaulEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.15, 0.2, 0.1 },
                trailerAxleWeightDistribution: new[] { 0.225, 0.325 },
                trailerAxleCount: new[] { 2, 3 },
                bodyCurbWeight: 2200,
                trailerCurbWeight: new[] { 2500, 7500.0 },
                trailerType: new[] { TrailerType.Dolly, TrailerType.ST1 },
                lowLoad: 3500,
                refLoad: 26500,
                trailerGrossVehicleWeight: new[] { 12000.0, 24000 },
                deltaCdA: 2.1,
                maxLoad: 40300,
                ems: true);

            AssertMission(segment.Missions[2],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.35, 0.35, 0.3 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { }, bodyCurbWeight: 2200,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 1400,
                refLoad: 7100,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 14300);

            AssertMission(segment.Missions[3],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDeliveryEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.175, 0.2, 0.1 },
                trailerAxleWeightDistribution: new[] { 0.225, 0.3 },
                trailerAxleCount: new[] { 2, 3 },
                bodyCurbWeight: 2200,
                trailerCurbWeight: new[] { 2500, 7500.0 },
                trailerType: new[] { TrailerType.Dolly, TrailerType.ST1 },
                lowLoad: 3500,
                refLoad: 17500,
                trailerGrossVehicleWeight: new[] { 12000.0, 24000 },
                deltaCdA: 2.1,
                maxLoad: 40300,
                ems: true);

            AssertMission(segment.Missions[4],
                vehicleData: vehicleData,
                missionType: MissionType.MunicipalUtility,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.35, 0.35, 0.3 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 6750,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 1200,
                refLoad: 6000,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 9750);

            AssertMission(segment.Missions[5],
                vehicleData: vehicleData,
                missionType: MissionType.Construction,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.35, 0.35, 0.3 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 3230,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 1400,
                refLoad: 7100,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 13270);
        }

        /// <summary>
        /// Segment 10: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment12Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.Tractor,
                AxleConfiguration = AxleConfiguration.AxleConfig_6x4,
                GrossVehicleMassRating = 24000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class12, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(5, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaul,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.15, 0.15, 0.15 },
                trailerAxleWeightDistribution: new[] { 0.55 },
                trailerAxleCount: new[] { 3 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0 },
                trailerType: new[] { TrailerType.ST1 },
                lowLoad: 2600,
                refLoad: 19300,
                trailerGrossVehicleWeight: new[] { 24000.0 },
                deltaCdA: 0,
                maxLoad: 25000);

            AssertMission(segment.Missions[1],
                vehicleData: vehicleData,
                missionType: MissionType.LongHaulEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.125, 0.15, 0.1 },
                trailerAxleWeightDistribution: new[] { 0.375, 0.25 },
                trailerAxleCount: new[] { 3, 2 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0, 5400 },
                trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
                lowLoad: 3500,
                refLoad: 26500,
                trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
                deltaCdA: 1.5,
                maxLoad: 39600,
                ems: true);

            AssertMission(segment.Missions[2],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDelivery,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.2, 0.15, 0.15 },
                trailerAxleWeightDistribution: new[] { 0.5 },
                trailerAxleCount: new[] { 3 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0 },
                trailerType: new[] { TrailerType.ST1 },
                lowLoad: 2600,
                refLoad: 12900,
                trailerGrossVehicleWeight: new[] { 24000.0 },
                deltaCdA: 0,
                maxLoad: 25000);

            AssertMission(segment.Missions[3],
                vehicleData: vehicleData,
                missionType: MissionType.RegionalDeliveryEMS,
                cosswindCorrection: "RigidTrailer",
                axleWeightDistribution: new[] { 0.15, 0.15, 0.1 },
                trailerAxleWeightDistribution: new[] { 0.35, 0.25 },
                trailerAxleCount: new[] { 3, 2 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 7500.0, 5400 },
                trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
                lowLoad: 3500,
                refLoad: 17500,
                trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
                deltaCdA: 1.5,
                maxLoad: 39600,
                ems: true);

            AssertMission(segment.Missions[4],
                vehicleData: vehicleData,
                missionType: MissionType.Construction,
                cosswindCorrection: "TractorSemitrailer",
                axleWeightDistribution: new[] { 0.2, 0.15, 0.15 },
                trailerAxleWeightDistribution: new[] { 0.5 },
                trailerAxleCount: new[] { 2 },
                bodyCurbWeight: 0,
                trailerCurbWeight: new[] { 5600.0 },
                trailerType: new[] { TrailerType.STT2 },
                lowLoad: 2600,
                refLoad: 12900,
                trailerGrossVehicleWeight: new[] { 18000.0 },
                deltaCdA: 0,
                maxLoad: 26900,
                ems: false);
        }

        /// <summary>
        /// Segment 9: fixed reference weight, trailer always used
        /// </summary>
        [TestCase, Category(Definitions.TESTCASE_MIGRATED)]
        public void Segment16Test()
        {
            var vehicleData = new
            {
                VehicleCategory = VehicleCategory.RigidTruck,
                AxleConfiguration = AxleConfiguration.AxleConfig_8x4,
                GrossVehicleMassRating = 36000.SI<Kilogram>(),
                CurbWeight = 7500.SI<Kilogram>()
            };

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
                vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

            Assert.AreEqual(VehicleClass.Class16, segment.VehicleClass);

            var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            TestAcceleration(data);

            Assert.AreEqual(1, segment.Missions.Length);

            AssertMission(segment.Missions[0],
                vehicleData: vehicleData,
                missionType: MissionType.Construction,
                cosswindCorrection: "RigidSolo",
                axleWeightDistribution: new[] { 0.25, 0.25, 0.25, 0.25 },
                trailerAxleWeightDistribution: new double[] { },
                trailerAxleCount: new int[] { },
                bodyCurbWeight: 4355,
                trailerCurbWeight: new double[] { },
                trailerType: new TrailerType[] { },
                lowLoad: 2600,
                refLoad: 12900,
                trailerGrossVehicleWeight: new double[] { },
                deltaCdA: 0,
                maxLoad: 24145);
        }

		/// <summary>
		/// Segment 53: medium lorry
		/// </summary>
		[TestCase, Category(Definitions.TESTCASE_MIGRATED)]
		public void Segment53Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.RigidTruck,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				GrossVehicleMassRating = 5200.SI<Kilogram>(),
				CurbWeight = 1300.SI<Kilogram>()
			};

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
				vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

			Assert.AreEqual(VehicleClass.Class53, segment.VehicleClass);

			var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
			TestAcceleration(data);

			Assert.AreEqual(2, segment.Missions.Length);

			AssertMission(segment.Missions[0],
				vehicleData: vehicleData,
				missionType: MissionType.RegionalDelivery,
				cosswindCorrection: "MediumLorriesRigid",
				axleWeightDistribution: new[] { 0.45, 0.55 },
				trailerAxleWeightDistribution: new double[] { },
				trailerAxleCount: new int[] { },
				bodyCurbWeight: 800,
				trailerCurbWeight: new double[] { },
				trailerType: new TrailerType[] { },
				lowLoad: 149.743589,
				refLoad: 764.3589743,
				trailerGrossVehicleWeight: new double[] { },
				deltaCdA: 0,
				maxLoad: 3100);
			AssertMission(segment.Missions[1],
				vehicleData: vehicleData,
				missionType: MissionType.UrbanDelivery,
				cosswindCorrection: "MediumLorriesRigid",
				axleWeightDistribution: new[] { 0.45, 0.55 },
				trailerAxleWeightDistribution: new double[] { },
				trailerAxleCount: new int[] { },
				bodyCurbWeight: 800,
				trailerCurbWeight: new double[] { },
				trailerType: new TrailerType[] { },
				lowLoad: 149.743589,
				refLoad: 764.3589743,
				trailerGrossVehicleWeight: new double[] { },
				deltaCdA: 0,
				maxLoad: 3100);
        }

		/// <summary>
		/// Segment 54: medium lorry
		/// </summary>
		[TestCase, Category(Definitions.TESTCASE_MIGRATED)]
		public void Segment54Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.Van,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				GrossVehicleMassRating = 5200.SI<Kilogram>(),
				CurbWeight = 1300.SI<Kilogram>()
			};

			var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
				vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

			Assert.AreEqual(VehicleClass.Class54, segment.VehicleClass);

			var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
			TestAcceleration(data);

			Assert.AreEqual(2, segment.Missions.Length);

			AssertMission(segment.Missions[0],
				vehicleData: vehicleData,
				missionType: MissionType.RegionalDelivery,
				cosswindCorrection: "MediumLorriesVan",
				axleWeightDistribution: new[] { 0.45, 0.55 },
				trailerAxleWeightDistribution: new double[] { },
				trailerAxleCount: new int[] { },
				bodyCurbWeight: 0,
				trailerCurbWeight: new double[] { },
				trailerType: new TrailerType[] { },
				lowLoad: 149.743589,
				refLoad: 764.3589743,
				trailerGrossVehicleWeight: new double[] { },
				deltaCdA: 0,
				maxLoad: 3900);
			AssertMission(segment.Missions[1],
				vehicleData: vehicleData,
				missionType: MissionType.UrbanDelivery,
				cosswindCorrection: "MediumLorriesVan",
				axleWeightDistribution: new[] { 0.45, 0.55 },
				trailerAxleWeightDistribution: new double[] { },
				trailerAxleCount: new int[] { },
				bodyCurbWeight: 0,
				trailerCurbWeight: new double[] { },
				trailerType: new TrailerType[] { },
				lowLoad: 149.743589,
				refLoad: 764.3589743,
				trailerGrossVehicleWeight: new double[] { },
				deltaCdA: 0,
				maxLoad: 3900);
        }

        public static void AssertMission(Mission m, dynamic vehicleData, MissionType missionType, string cosswindCorrection,
            double[] axleWeightDistribution, double[] trailerAxleWeightDistribution, int[] trailerAxleCount,
            double bodyCurbWeight, double[] trailerCurbWeight, TrailerType[] trailerType, double lowLoad, double refLoad,
            double maxLoad, double[] trailerGrossVehicleWeight, double deltaCdA, bool ems = false)
        {
            Assert.AreEqual(missionType, m.MissionType);
            Assert.AreEqual(cosswindCorrection, m.CrossWindCorrectionParameters);
            CollectionAssert.AreEqual(axleWeightDistribution, m.AxleWeightDistribution,
                "Axle distribution not equal.\nexpected: {0}\nactual: {1}", axleWeightDistribution.Join(),
                m.AxleWeightDistribution.Join());
            CollectionAssert.AreEqual(trailerAxleWeightDistribution, m.Trailer.Select(t => t.TrailerAxleWeightShare),
                "Trailer axle distribution not equal.\nexpected: {0}\nactual: {1}", trailerAxleWeightDistribution.Join(),
                m.Trailer.Select(t => t.TrailerAxleWeightShare).Join());
            Assert.AreEqual(bodyCurbWeight.SI<Kilogram>(), m.BodyCurbWeight);
            CollectionAssert.AreEqual(trailerCurbWeight, m.Trailer.Select(t => t.TrailerCurbWeight.Value()));
            CollectionAssert.AreEqual(trailerType, m.Trailer.Select(t => t.TrailerType));
            CollectionAssert.AreEqual(trailerAxleCount, m.Trailer.Select(t => t.TrailerWheels.Count));
            //Assert.IsNotNull(m.CycleFile);
            //Assert.IsTrue(!string.IsNullOrEmpty(new StreamReader(m.CycleFile).ReadLine()));
            Assert.AreEqual(null, m.MinLoad);
            AssertHelper.AreRelativeEqual(lowLoad, m.LowLoad);
            AssertHelper.AreRelativeEqual(refLoad, m.RefLoad);
            Assert.AreEqual(maxLoad.SI<Kilogram>(), m.MaxPayload);
            CollectionAssert.AreEqual(trailerGrossVehicleWeight, m.Trailer.Select(t => t.TrailerGrossVehicleWeight.Value()));
            Assert.AreEqual(
                VectoMath.Min(
                    vehicleData.GrossVehicleMassRating +
                    m.Trailer.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0),
                    ems ? 60000.SI<Kilogram>() : 40000.SI<Kilogram>())
                - m.BodyCurbWeight - m.Trailer.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0) -
                vehicleData.CurbWeight,
                m.MaxPayload);
            Assert.AreEqual(deltaCdA.SI<SquareMeter>(),
                m.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0));
        }

        private static void EqualAcceleration(AccelerationCurveData data, double velocity, double acceleration,
            double deceleration)
        {
            var entry = data.Lookup(velocity.KMPHtoMeterPerSecond());
            Assert.AreEqual(entry.Acceleration.Value(), acceleration, Tolerance);
            Assert.AreEqual(entry.Deceleration.Value(), deceleration, Tolerance);
        }

        private static void TestAcceleration(AccelerationCurveData data)
        {
            // FIXED POINTS
            EqualAcceleration(data, 0, 1, -1);
            EqualAcceleration(data, 25, 1, -1);
            EqualAcceleration(data, 50, 0.642857143, -1);
            EqualAcceleration(data, 60, 0.5, -0.5);
            EqualAcceleration(data, 120, 0.5, -0.5);

            // INTERPOLATED POINTS
            EqualAcceleration(data, 20, 1, -1);
            EqualAcceleration(data, 40, 0.785714286, -1);
            EqualAcceleration(data, 55, 0.571428572, -0.75);
            EqualAcceleration(data, 80, 0.5, -0.5);
            EqualAcceleration(data, 100, 0.5, -0.5);

            // EXTRAPOLATE 
            EqualAcceleration(data, -20, 1, -1);
            EqualAcceleration(data, 140, 0.5, -0.5);
        }

        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void Declaration_WheelsForT1_Class2()
        {
            var dataProvider =
                JSONInputDataFactory.ReadJsonJob(@"TestData/Jobs/12t Delivery Truck.vecto") as IDeclarationInputDataProvider;
			var dataReader = new DeclarationModeHeavyLorryRunDataFactory.Conventional(dataProvider, null, new DeclarationDataAdapterHeavyLorry.Conventional(), _kernel.Get<IDeclarationCycleFactory>(), _kernel.Get<IMissionFilter>(), _kernel.Get<IPowertrainBuilder>());

            var runs = dataReader.NextRun().ToList();
            Assert.AreEqual(6, runs.Count);
            var withT1 = new[] { 6.0, 6.0, 4.5, 4.5 };

            CollectionAssert.AreEqual(withT1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(withT1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            var bodyOnly = new[] { 6.0, 6.0 };

            CollectionAssert.AreEqual(bodyOnly, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(bodyOnly, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            CollectionAssert.AreEqual(bodyOnly, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(bodyOnly, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        }

        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void Declaration_WheelsForT2_Class4()
        {
            var dataProvider =
                JSONInputDataFactory.ReadJsonJob(
                    @"TestData/Jobs/Class4_40t_Long_Haul_Truck.vecto") as IDeclarationInputDataProvider;
			var dataReader = new DeclarationModeHeavyLorryRunDataFactory.Conventional(dataProvider, null, new DeclarationDataAdapterHeavyLorry.Conventional(), _kernel.Get<IDeclarationCycleFactory>(), _kernel.Get<IMissionFilter>(), _kernel.Get<IPowertrainBuilder>());

            var runs = dataReader.NextRun().ToList();
            Assert.AreEqual(8, runs.Count);
            var withT1 = new[] { 14.9, 14.9, 19.2, 19.2 };
            CollectionAssert.AreEqual(withT1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(withT1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            var bodyOnly = new[] { 14.9, 14.9 };
            CollectionAssert.AreEqual(bodyOnly, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(bodyOnly, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            CollectionAssert.AreEqual(bodyOnly, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(bodyOnly, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            CollectionAssert.AreEqual(bodyOnly, runs[6].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(bodyOnly, runs[7].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        }

        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void Declaration_WheelsForDefault_Class5()
        {
            var dataProvider =
                JSONInputDataFactory.ReadJsonJob(@"TestData/Jobs/40t_Long_Haul_Truck.vecto") as IDeclarationInputDataProvider;
			var dataReader = new DeclarationModeHeavyLorryRunDataFactory.Conventional(dataProvider, null, new DeclarationDataAdapterHeavyLorry.Conventional(), _kernel.Get<IDeclarationCycleFactory>(), _kernel.Get<IMissionFilter>(), _kernel.Get<IPowertrainBuilder>());

            var runs = dataReader.NextRun().ToList();

            Assert.AreEqual(VehicleClass.Class5, runs[0].VehicleData.VehicleClass);
            Assert.AreEqual(10, runs.Count);

            //var bodyOnly = new[] { 14.9, 14.9 };
            var withST1 = new[] { 14.9, 14.9, 19.2, 19.2, 19.2 };
            var withSTT1 = new[] { 14.9, 14.9, 19.2, 19.2, 19.2 };
            var withST1andT2 = new[] { 14.9, 14.9, 19.2, 19.2, 19.2, 19.2, 19.2 };

            CollectionAssert.AreEqual(withST1, runs[0].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(withST1, runs[1].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            CollectionAssert.AreEqual(withST1andT2, runs[2].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(withST1andT2, runs[3].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            CollectionAssert.AreEqual(withST1, runs[4].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(withST1, runs[5].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            CollectionAssert.AreEqual(withST1andT2, runs[6].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(withST1andT2, runs[7].VehicleData.AxleData.Select(a => a.Inertia.Value()));

            CollectionAssert.AreEqual(withSTT1, runs[8].VehicleData.AxleData.Select(a => a.Inertia.Value()));
            CollectionAssert.AreEqual(withSTT1, runs[9].VehicleData.AxleData.Select(a => a.Inertia.Value()));
        }


		[TestCase(false, false, false, PredictiveCruiseControlType.None, "0"),
		TestCase(true, false, false, PredictiveCruiseControlType.None, "1"),
		TestCase(true, false, false, PredictiveCruiseControlType.Option_1_2_3, "7/2"),
		TestCase(true, true, false, PredictiveCruiseControlType.Option_1_2, "10/1"), 
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestADASCombinationLookup(bool engineStopStart, bool ecoRollWOEngineStop, bool ecoRollWEngineStop, PredictiveCruiseControlType pcc,
	string expectedADASGroup)
		{
			var adas = DeclarationData.ADASCombinations.Lookup(engineStopStart, EcorollTypeHelper.Get(ecoRollWOEngineStop, ecoRollWEngineStop), pcc);
			Assert.AreEqual(adas.ID, expectedADASGroup);
		}

		[TestCase(true, true, true, PredictiveCruiseControlType.Option_1_2),
		TestCase(true, true, true, PredictiveCruiseControlType.None), 
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestInvalidADASCombinationLookup(
			bool engineStopStart, bool ecoRollWOEngineStop, bool ecoRollWEngineStop, PredictiveCruiseControlType pcc)
		{
			AssertHelper.Exception<VectoException>(() => {
				DeclarationData.ADASCombinations.Lookup(engineStopStart, EcorollTypeHelper.Get(ecoRollWOEngineStop, ecoRollWEngineStop), pcc);
			});
		}

		[TestCase(false, false, true, PredictiveCruiseControlType.None ),
		TestCase(true, false, true, PredictiveCruiseControlType.None),
		TestCase(false, false, true, PredictiveCruiseControlType.Option_1_2),
		TestCase(false, false, true, PredictiveCruiseControlType.Option_1_2_3),
		TestCase(true, false, true, PredictiveCruiseControlType.Option_1_2),
		TestCase(true, false, true, PredictiveCruiseControlType.Option_1_2_3), 
		Category(Definitions.TESTCASE_MIGRATED),
			]
		public void TestInvalidATADASCombinationLookup(bool engineStopStart, bool ecoRollWOEngineStop, bool ecoRollWEngineStop, PredictiveCruiseControlType pcc)
		{
			var adas = new ADASMock() {
				EngineStopStart =  engineStopStart,
				EcoRoll = EcorollTypeHelper.Get(ecoRollWOEngineStop, ecoRollWEngineStop),
				PredictiveCruiseControl = pcc
			};
			AssertHelper.Exception<VectoException>(() => {
				DeclarationData.ADASCombinations.Lookup(adas, GearboxType.ATSerial);
			});
		}

		class ADASMock : IAdvancedDriverAssistantSystemDeclarationInputData
		{
			#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

			public bool EngineStopStart { get; set; }
			public EcoRollType EcoRoll { get; set; }
			public PredictiveCruiseControlType PredictiveCruiseControl { get; set; }
			public bool? ATEcoRollReleaseLockupClutch { get; set; }
			public XmlNode XMLSource { get; }

			#endregion
		}


        [
            TestCase("Diesel CI", null, 1.0),
            TestCase("Ethanol CI", null, 1.011811),
            TestCase("Petrol PI", null, 1.0),
            TestCase("Ethanol PI", null, 0.993174),
            TestCase("NG PI", TankSystem.Liquefied, 0.918533),
            TestCase("NG PI", TankSystem.Compressed, 0.939583),
			Category(Definitions.TESTCASE_MIGRATED)
            ]
        public void TestNCVCorrection(string fuelTypeStr, TankSystem? tankSystem, double expectedCorrectionFactor)
        {
            var fuelType = fuelTypeStr.ParseEnum<FuelType>();
            var cf = DeclarationData.FuelData.Lookup(fuelType, tankSystem).HeatingValueCorrection;

            Assert.AreEqual(expectedCorrectionFactor, cf, 1e-6);
        }

        [
		Category(Definitions.TESTCASE_MIGRATED),

            TestCase(WeightingGroup.Group51, 0, 0, 0.25, 0.25, 0.25, 0.25),
            TestCase(WeightingGroup.Group52, 0, 0, 0.25, 0.25, 0.25, 0.25),
            TestCase(WeightingGroup.Group53, 0, 0, 0.25, 0.25, 0.25, 0.25),
            TestCase(WeightingGroup.Group54, 0, 0, 0.25, 0.25, 0.25, 0.25),
            TestCase(WeightingGroup.Group55, 0, 0, 0.25, 0.25, 0.25, 0.25),
            TestCase(WeightingGroup.Group56, 0, 0, 0.25, 0.25, 0.25, 0.25),

			TestCase(WeightingGroup.Group1, 0, 0, 0.1, 0.3, 0.18, 0.42, TestName = "TestMissionProfileWeights Grp 1"),
			TestCase(WeightingGroup.Group2, 0, 0, 0.125, 0.375, 0.15, 0.35, TestName = "TestMissionProfileWeights Grp 2"),
			TestCase(WeightingGroup.Group3, 0, 0, 0.125, 0.375, 0.15, 0.35, TestName = "TestMissionProfileWeights Grp 3"),


			TestCase(WeightingGroup.Group4UD, 0, 0, 0, 0, 0.5, 0.5, TestName = "TestMissionProfileWeights Grp 4UD"),
			TestCase(WeightingGroup.Group4RD, 0.05, 0.05, 0.45, 0.45, 0, 0, TestName = "TestMissionProfileWeights Grp 4RD"),
			TestCase(WeightingGroup.Group4LH, 0.45, 0.45, 0.05, 0.05, 0, 0, TestName = "TestMissionProfileWeights Grp 4LH"),

			TestCase(WeightingGroup.Group5RD, 0.03, 0.07, 0.27, 0.63, 0, 0, TestName = "TestMissionProfileWeights Grp 5RD"),
			TestCase(WeightingGroup.Group5LH, 0.27, 0.63, 0.03, 0.07, 0, 0, TestName = "TestMissionProfileWeights Grp 5LH"),

			TestCase(WeightingGroup.Group9RD, 0.03, 0.07, 0.27, 0.63, 0, 0, TestName = "TestMissionProfileWeights Grp 9RD"),
			TestCase(WeightingGroup.Group9LH, 0.27, 0.63, 0.03, 0.07, 0, 0, TestName = "TestMissionProfileWeights Grp 9LH"),

			TestCase(WeightingGroup.Group10RD, 0.03, 0.07, 0.27, 0.63, 0, 0, TestName = "TestMissionProfileWeights Grp 10RD"),
			TestCase(WeightingGroup.Group10LH, 0.27, 0.63, 0.03, 0.07, 0, 0, TestName = "TestMissionProfileWeights Grp 10LH"),

			TestCase(WeightingGroup.Group11, 0, 0, 0.15, 0.35, 0, 0, 0, 0, 0.15, 0.35, TestName = "TestMissionProfileWeights Grp 11"),
			TestCase(WeightingGroup.Group12, 0, 0, 0.21, 0.49, 0, 0, 0, 0, 0.09, 0.21, TestName = "TestMissionProfileWeights Grp 12"),
			TestCase(WeightingGroup.Group16, 0, 0, 0, 0, 0, 0, 0, 0, 0.3, 0.7, TestName = "TestMissionProfileWeights Grp 16"),
			Category(Definitions.TESTCASE_MIGRATED),
		]
		public void TestMissionProfileWeights(WeightingGroup group, double eLhLow, double eLhRef, double eRdLow, double eRdRef, double eUdLow, double eUdRef, double eMuLow = 0, double eMuRef = 0, double eCoLow = 0, double eCoRef = 0, double elhEmsLow = 0, double eLhEmsRef = 0, double eRdEmsLow = 0, double eRdEmsRef = 0)
		{
            var factors = DeclarationData.WeightingFactors.Lookup(group);

            Assert.AreEqual(1, factors.Values.Sum(x => x), 1e-9);

            Assert.AreEqual(eLhLow, factors[Tuple.Create(MissionType.LongHaul, LoadingType.LowLoading)], 1e-9);
            Assert.AreEqual(eLhRef, factors[Tuple.Create(MissionType.LongHaul, LoadingType.ReferenceLoad)], 1e-9);
            Assert.AreEqual(eRdLow, factors[Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading)], 1e-9);
            Assert.AreEqual(eRdRef, factors[Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad)], 1e-9);
            Assert.AreEqual(eUdLow, factors[Tuple.Create(MissionType.UrbanDelivery, LoadingType.LowLoading)], 1e-9);
            Assert.AreEqual(eUdRef, factors[Tuple.Create(MissionType.UrbanDelivery, LoadingType.ReferenceLoad)], 1e-9);

			/*Assert.AreEqual(0, factors[Tuple.Create(MissionType.Construction, LoadingType.LowLoading)], 1e-9);
			Assert.AreEqual(0, factors[Tuple.Create(MissionType.Construction, LoadingType.ReferenceLoad)], 1e-9);

            //Assert.AreEqual(0, factors[Tuple.Create(MissionType.MunicipalUtility, LoadingType.ReferenceLoad)], 1e-9);
            //Assert.AreEqual(0, factors[Tuple.Create(MissionType.LongHaulEMS, LoadingType.LowLoading)], 1e-9);
			
            //Assert.AreEqual(0, factors[Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.LowLoading)], 1e-9);
            //Assert.AreEqual(0, factors[Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad)], 1e-9);
			Assert.AreEqual(0, factors[Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad)], 1e-9);*/
        }

        [
            TestCase(0.0030, "A"),
            TestCase(0.0040, "A"),
            TestCase(0.0050, "B"),
            TestCase(0.0060, "C"),
            TestCase(0.0070, "D"),
            TestCase(0.0080, "E"),

            TestCase(0.0041, "B"),
            TestCase(0.0051, "C"),
            TestCase(0.0061, "D"),
            TestCase(0.0071, "E"),
            TestCase(0.0081, "F"),

            TestCase(0.00402, "A"),
            TestCase(0.004049, "A"),
            TestCase(0.00405, "B"),
            TestCase(0.00407, "B"),

            TestCase(0.00502, "B"),
            TestCase(0.005049, "B"),
            TestCase(0.00505, "C"),
            TestCase(0.00507, "C"),
			Category(Definitions.TESTCASE_MIGRATED),
            Category(Definitions.DEPRECATED)
		]
        public void TestTyreLabelLookup(double rrc, string expectedClass)
        {
            var tyreClass = DeclarationData.Wheels.TyreClass.Lookup(rrc);
            //Assert.IsTrue(expectedClass.Equals(tyreClass, StringComparison.InvariantCultureIgnoreCase));
            Assert.AreEqual(expectedClass, tyreClass);
        }


        [TestCase(VehicleClass.Class1s, MissionType.RegionalDelivery, 2)]
		[TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 2)]
		[TestCase(VehicleClass.Class16, MissionType.Construction, 2)]
		[TestCase(VehicleClass.Class53, MissionType.UrbanDelivery, 4),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void VehicleOperationLookupChargingEventsLorry(VehicleClass hdvClass, MissionType mission, double expected)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
			Assert.AreEqual(expected, val.StationaryChargingDuringMission_NbrEvents);
		}

		[TestCase(VehicleClass.Class1s, MissionType.RegionalDelivery, 0.5)]
		[TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 0.5)]
		[TestCase(VehicleClass.Class16, MissionType.Construction, 0.5)]
		[TestCase(VehicleClass.Class53, MissionType.UrbanDelivery, 0.5),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void VehicleOperationLookupChargingDurationLorry(VehicleClass hdvClass, MissionType mission, double expected)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
			Assert.AreEqual(expected * 3600, val.StationaryChargingDuringMission_AvgDurationPerEvent.Value()); //stored in seconds
		}

		[TestCase(VehicleClass.Class1s, MissionType.RegionalDelivery, 250)]
		[TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 250)]
		[TestCase(VehicleClass.Class16, MissionType.Construction, 100)]
		[TestCase(VehicleClass.Class53, MissionType.UrbanDelivery, 250),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void VehicleOperationLookupMaxChargingPowerLorry(VehicleClass hdvClass, MissionType mission, double expected)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
			Assert.AreEqual(expected * 1000, val.StationaryChargingMaxPwrInfrastructure.Value()); //stored in watt
		}

		[TestCase(VehicleClass.Class1s, MissionType.RegionalDelivery, 80000, 320)]
		[TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 80000, 320)]
		[TestCase(VehicleClass.Class16, MissionType.Construction, 60000, 240)]
		[TestCase(VehicleClass.Class53, MissionType.UrbanDelivery, 60000, 240),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void VehicleOperationLookupMileageLorry(VehicleClass hdvClass, MissionType mission, double expectedAnnual, double expectedDaily)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
			Assert.AreEqual(expectedAnnual * 1000, val.Mileage.AnnualMileage.Value()); //stored in meter
			Assert.AreEqual(expectedDaily * 1000, val.Mileage.DailyMileage.Value()); //stored in meter
		}

        [TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 0.34)]
		[TestCase(VehicleClass.Class2, MissionType.LongHaul, 0.5)]
		[TestCase(VehicleClass.Class3, MissionType.RegionalDelivery, 0.34)]
		[TestCase(VehicleClass.Class4, MissionType.UrbanDelivery, 0)]
		[TestCase(VehicleClass.Class12, MissionType.RegionalDelivery, 0.34)]
		[TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 0.30)]
		public void VehicleOperationLookupShareInMotionInfrastructureCatenary(VehicleClass hdvClass, MissionType mission,
			double expectedShare)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
            Assert.AreEqual(expectedShare, val.ShareInMotionCharging.ShareCatenary);
		}

		[TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.Class2, MissionType.LongHaul, 0.0)]
		[TestCase(VehicleClass.Class3, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.Class4, MissionType.UrbanDelivery, 0.0)]
		[TestCase(VehicleClass.Class12, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 0.80)]
		[TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 0.20)]
		[TestCase(VehicleClass.ClassP33SD, MissionType.Interurban, 0.20)]
		[TestCase(VehicleClass.ClassP40DD, MissionType.Coach, 0.00)]
        public void VehicleOperationLookupShareInMotionInfrastructurTrolley(VehicleClass hdvClass, MissionType mission,
			double expectedShare)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
			Assert.AreEqual(expectedShare, val.ShareInMotionCharging.ShareTrolley);
		}

		[TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.Class2, MissionType.LongHaul, 0.0)]
		[TestCase(VehicleClass.Class3, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.Class4, MissionType.UrbanDelivery, 0.5)]
		[TestCase(VehicleClass.Class12, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 0.50)]
		[TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 0.00)]
		[TestCase(VehicleClass.ClassP33SD, MissionType.Interurban, 0.00)]
		[TestCase(VehicleClass.ClassP40DD, MissionType.Coach, 0.00)]
        public void VehicleOperationLookupShareInMotionInfrastructureGroundRail(VehicleClass hdvClass, MissionType mission,
			double expectedShare)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
			Assert.AreEqual(expectedShare, val.ShareInMotionCharging.ShareGroundRail);
		}

		[TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.Class2, MissionType.LongHaul, 0.0)]
		[TestCase(VehicleClass.Class3, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.Class4, MissionType.UrbanDelivery, 0.5)]
		[TestCase(VehicleClass.Class12, MissionType.RegionalDelivery, 0.0)]
		[TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 0.50)]
		[TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 0.00)]
		[TestCase(VehicleClass.ClassP33SD, MissionType.Interurban, 0.00)]
		[TestCase(VehicleClass.ClassP40DD, MissionType.Coach, 0.00)]
        public void VehicleOperationLookupShareInMotionInfrastructureWireless(VehicleClass hdvClass, MissionType mission,
			double expectedShare)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
			Assert.AreEqual(expectedShare, val.ShareInMotionCharging.ShareWireless);
		}

        [TestCaseSource(nameof(VehicleOperationTestSourceLorry)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void VehicleOperationLookupMileage(VehicleClass hdvClass, MissionType mission)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        }


		public static IEnumerable<object[]> VehicleOperationTestSourceLorry()
		{
			var missions = EnumHelper.GetValues<MissionType>();

			var segmentTable = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".SegmentTable.csv"));
			var hdvMissionDict = new Dictionary<VehicleClass, HashSet<MissionType>>();

			foreach (DataRow row in segmentTable.Rows) {
				if (!row["valid"].ToString().ToBoolean()) {
					continue;
				}
				var hdvGroup = VehicleClassHelper.Parse(row["HDV group"].ToString());
				hdvMissionDict.TryAdd(hdvGroup, new HashSet<MissionType>());
				foreach (var missionType in missions.Where(
							m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
								row.Field<string>(m.GetLabel()) != "-")) {
					hdvMissionDict[hdvGroup].Add(missionType);
				}
			}

			foreach (var hdvCl in 
					hdvMissionDict
						.Where(kv => kv.Value.Count > 0)
						.OrderBy(kv => kv.Key)) {
				foreach (var mission in hdvCl.Value) {
					yield return new object[]{hdvCl.Key, mission};
				}
			}
		}


        [TestCase(VehicleClass.Class53, MissionType.LongHaul)]
		[TestCase(VehicleClass.Class16, MissionType.UrbanDelivery)]
		[TestCase(VehicleClass.Class1s, MissionType.Coach),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void VehicleOperationHeavyLorryFail(VehicleClass hdvClass, MissionType mission)
		{
			Assert.Throws<VectoException>(() => {
				DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
			});

        }

        // =======================================================

        [TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 10)]
        [TestCase(VehicleClass.ClassP31DD, MissionType.HeavyUrban, 10)]
        [TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 5)]
        [TestCase(VehicleClass.ClassP32DD, MissionType.Interurban, 5)]
		[TestCase(VehicleClass.Class31a, MissionType.Urban, 10)]
		[TestCase(VehicleClass.Class31e, MissionType.Suburban, 10)]
		[TestCase(VehicleClass.Class32a, MissionType.Coach, 2)]
		[TestCase(VehicleClass.Class32b, MissionType.Interurban, 5),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void VehicleOperationLookupChargingEventsBus(VehicleClass hdvClass, MissionType mission, double expected)
        {
            var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
            Assert.AreEqual(expected, val.StationaryChargingDuringMission_NbrEvents);
        }

		[TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 0.17)]
		[TestCase(VehicleClass.ClassP31DD, MissionType.HeavyUrban, 0.17)]
		[TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 0.17)]
		[TestCase(VehicleClass.ClassP32DD, MissionType.Interurban, 0.17)]
		[TestCase(VehicleClass.Class31a, MissionType.Urban, 0.17)]
		[TestCase(VehicleClass.Class31e, MissionType.Suburban, 0.17)]
		[TestCase(VehicleClass.Class32a, MissionType.Coach, 0.75)]
		[TestCase(VehicleClass.Class32b, MissionType.Interurban, 0.17),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void VehicleOperationLookupChargingDurationBus(VehicleClass hdvClass, MissionType mission, double expected)
        {
            var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
            Assert.AreEqual(expected * 3600, val.StationaryChargingDuringMission_AvgDurationPerEvent.Value()); //stored in seconds
        }

		[TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 450)]
		[TestCase(VehicleClass.ClassP31DD, MissionType.HeavyUrban, 450)]
		[TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 300)]
		[TestCase(VehicleClass.ClassP32DD, MissionType.Interurban, 300)]
		[TestCase(VehicleClass.Class31a, MissionType.Urban, 450)]
		[TestCase(VehicleClass.Class31e, MissionType.Suburban, 450)]
		[TestCase(VehicleClass.Class32a, MissionType.Coach, 300)]
		[TestCase(VehicleClass.Class32b, MissionType.Interurban, 300),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void VehicleOperationLookupMaxChargingPowerBus(VehicleClass hdvClass, MissionType mission, double expected)
        {
            var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
            Assert.AreEqual(expected * 1000, val.StationaryChargingMaxPwrInfrastructure.Value()); //stored in watt
        }

		[TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 60000, 240)]
		[TestCase(VehicleClass.ClassP31DD, MissionType.HeavyUrban, 60000, 240)]
		[TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 80000, 320)]
		[TestCase(VehicleClass.ClassP32DD, MissionType.Interurban, 80000, 320)]
		[TestCase(VehicleClass.Class31a, MissionType.Urban, 60000, 240)]
		[TestCase(VehicleClass.Class31e, MissionType.Suburban, 60000, 240)]
		[TestCase(VehicleClass.Class32a, MissionType.Coach, 100000, 400)]
		[TestCase(VehicleClass.Class32b, MissionType.Interurban, 80000, 320),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void VehicleOperationLookupMileageBus(VehicleClass hdvClass, MissionType mission, double expectedAnnual, double expectedDaily)
        {
            var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
            Assert.AreEqual(expectedAnnual * 1000, val.Mileage.AnnualMileage.Value()); //stored in meter
            Assert.AreEqual(expectedDaily * 1000, val.Mileage.DailyMileage.Value()); //stored in meter
        }

		[TestCaseSource(nameof(VehicleOperationTestSourcePrimaryBus)),
		Category(Definitions.TESTCASE_MIGRATED)]
        [TestCaseSource(nameof(VehicleOperationTestSourceCompletedBus)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void VehicleOperationLookupMileageBus(VehicleClass hdvClass, MissionType mission)
		{
			var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
		}


		public static IEnumerable<object[]> VehicleOperationTestSourcePrimaryBus()
		{
			var missions = EnumHelper.GetValues<MissionType>();

			var segmentTable = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".PrimaryBusSegmentationTable.csv"));
			var hdvMissionDict = new Dictionary<VehicleClass, HashSet<MissionType>>();

			foreach (DataRow row in segmentTable.Rows) {
				var hdvGroup = VehicleClassHelper.Parse(row["HDV group"].ToString());
				hdvMissionDict.TryAdd(hdvGroup, new HashSet<MissionType>());
				foreach (var missionType in missions.Where(
							m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
                                segmentTable.Columns.Contains(m.GetLabel()) &&
								!row.Field<string>(m.GetLabel()).IsNullOrWhiteSpace())) {
					hdvMissionDict[hdvGroup].Add(missionType);
				}
			}

			foreach (var hdvCl in
					hdvMissionDict
						.Where(kv => kv.Value.Count > 0)
						.OrderBy(kv => kv.Key)) {
				foreach (var mission in hdvCl.Value) {
					yield return new object[] { hdvCl.Key, mission };
				}
			}
		}

		public static IEnumerable<object[]> VehicleOperationTestSourceCompletedBus()
		{
			var missions = EnumHelper.GetValues<MissionType>();

			var segmentTable = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".CompletedBusSegmentationTable.csv"));
			var hdvMissionDict = new Dictionary<VehicleClass, HashSet<MissionType>>();

			foreach (DataRow row in segmentTable.Rows) {
				var hdvGroup = VehicleClassHelper.Parse(row["HDV group"].ToString());
				hdvMissionDict.TryAdd(hdvGroup, new HashSet<MissionType>());
				foreach (var missionType in missions.Where(
							m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
								segmentTable.Columns.Contains(m.GetLabel()) &&
                                !row.Field<string>(m.GetLabel()).IsNullOrWhiteSpace())) {
					hdvMissionDict[hdvGroup].Add(missionType);
				}
			}

			foreach (var hdvCl in
					hdvMissionDict
						.Where(kv => kv.Value.Count > 0)
						.OrderBy(kv => kv.Key)) {
				foreach (var mission in hdvCl.Value) {
					yield return new object[] { hdvCl.Key, mission };
				}
			}
		}

    }
}