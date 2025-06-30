using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.GearShiftStrategy;

public class PEV_ShiftLineTests
{
    [TestCase]
    public void ComputePEVShiftLinesADC()
    {
		var axlegearRatio = 2.64;
		var r_dyn = 0.421.SI<Meter>();

		var expectedDownshiftNoADC = new[] {
			// Gear 1
            new Point[] {

			},
            // Gear 2
            new[] {
				new Point(1969.345479169557, -1067),
				new Point(1969.345479169557, -950.6),
				new Point(253.98783622789142, -950.6),
				new Point(253.98783622789142, 950.6),
				new Point(2452.135493, 950.6),
				new Point(2461.1589140000006, 950.6),
				new Point(2466.8630340000004, 948.41362),
				new Point(2481.590574, 942.781315),
				new Point(2496.3181150000005, 937.220305),
				new Point(2503.681885, 934.463565),
				new Point(2539.878362278914, 921.3333994166501),
				new Point(2539.878362278914, 1067),
            },
			// Gear 3
			new[] {
				new Point(1969.345479169557, -1067),
				new Point(1969.345479169557, -950.6),
				new Point(253.98783622789142, -950.6),
				new Point(253.98783622789142, 950.6),
				new Point(2452.135493, 950.6),
				new Point(2461.1589140000006, 950.6),
				new Point(2466.8630340000004, 948.41362),
				new Point(2481.590574, 942.781315),
				new Point(2496.3181150000005, 937.220305),
				new Point(2503.681885, 934.463565),
				new Point(2539.878362278914, 921.3333994166501),
				new Point(2539.878362278914, 1067),
			},
        };
		var expectedUpshiftNoADC = new[] {
			// Gear 1
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
			// Gear 2
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
			// Gear 3
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
        };
		var countEm = 2;
		var emRatio = 2.0;

		var expectedUpshift =
			expectedUpshiftNoADC.Select(ps => ps.Select(p => new Point(p.X / emRatio, p.Y * emRatio)).ToArray()).ToArray();
		var expectedDownshift =
			expectedDownshiftNoADC.Select(ps => ps.Select(p => new Point(p.X / emRatio, p.Y * emRatio)).ToArray()).ToArray();



        var emFld = new ElectricMotorFullLoadCurve(EM_FullLoad_125kW_485Nm.Select(x =>
			new ElectricMotorFullLoadCurve.FullLoadEntry() {
				// invert motor full load curve here as would be done in respective reader class
				MotorSpeed = x[0].RPMtoRad(),
				FullDriveTorque = -x[1].SI<NewtonMeter>() * countEm,
				FullGenerationTorque = -x[2].SI<NewtonMeter>() * countEm,
			}).ToList());
		
		var emEffMap = GetEffMap(emFld, countEm);


		

		var emData = new ElectricMotorData() {
			EfficiencyData = new VoltageLevelData() {
				VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
					new ElectricMotorVoltageLevelData() {
						FullLoadCurve = emFld,
						EfficiencyMap = emEffMap,
					}
				}
			},
			TransmissionLossMap = TransmissionLossMapReader.CreateEmADCLossMap(1, emRatio, "ADC"),
			RatioADC = emRatio
		};
		var gearboxData = new Mock<IGearboxEngineeringInputData>().Object;
		var gear = new Mock<ITransmissionInputData>().Object;
		Mock.Get(gearboxData).Setup(g => g.Gears).Returns(new List<ITransmissionInputData>() {
			gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear,
        });

		var shiftPolygons = new List<ShiftPolygon>();

		var shiftStrategyParams = new ShiftStrategyParameters(); // use default (declaration) shift strategy params
		// var downshiftMaxSpeed = emFld.RatedSpeed * shiftStrategyParams.PEV_DownshiftSpeedFactor.LimitTo(0, 1);
		// var downshiftMinSpeed = emFld.RatedSpeed * shiftStrategyParams.PEV_DownshiftMinSpeedFactor;

		
		
        for (var i = 0; i < gearboxData.Gears.Count; i++) {
			var shiftPolygon = DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(
				i,
				emData,
				gearboxData.Gears,
				shiftStrategyParams.PEV_DownshiftSpeedFactor.LimitTo(0,1),
				shiftStrategyParams.PEV_DownshiftMinSpeedFactor
			);
			
			// var shiftpolygon = DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(i, 
			// 	emFld, emRatio, 
			// 	gearboxData.Gears, 
			// 	axlegearRatio,
			// 	r_dyn, 
			// 	downshiftMaxSpeed, 
			// 	downshiftMinSpeed);
            shiftPolygons.Add(shiftPolygon);
        }


		CommonSanityChecks(shiftPolygons, emData.EfficiencyData.MaxSpeed / emRatio);
		

		for (var i = 0; i < Math.Min(gearboxData.Gears.Count, Math.Min(expectedDownshift.Length, expectedUpshift.Length)); i++) {
			foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}

			foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}
		}

		Assert.AreEqual(0, shiftPolygons.First().Downshift.Count);
		Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);

        //var suffix = factorDownshiftSpeed.HasValue ? $"_{factorDownshiftSpeed.Value}" : "";

    }

	[TestCase]
    public void ComputePEVShiftLines()
    {
		var axlegearRatio = 2.64;
		var r_dyn = 0.421.SI<Meter>();

		var expectedDownshift = new[] {
			// Gear 1
            new Point[] {

			},
            // Gear 2
            new[] {
				new Point(1969.345479169557, -1067),
				new Point(1969.345479169557, -950.6),
				new Point(253.98783622789142, -950.6),
				new Point(253.98783622789142, 950.6),
				new Point(2452.135493, 950.6),
				new Point(2461.1589140000006, 950.6),
				new Point(2466.8630340000004, 948.41362),
				new Point(2481.590574, 942.781315),
				new Point(2496.3181150000005, 937.220305),
				new Point(2503.681885, 934.463565),
				new Point(2539.878362278914, 921.3333994166501),
				new Point(2539.878362278914, 1067),
            },
			// Gear 3
			new[] {
				new Point(1969.345479169557, -1067),
				new Point(1969.345479169557, -950.6),
				new Point(253.98783622789142, -950.6),
				new Point(253.98783622789142, 950.6),
				new Point(2452.135493, 950.6),
				new Point(2461.1589140000006, 950.6),
				new Point(2466.8630340000004, 948.41362),
				new Point(2481.590574, 942.781315),
				new Point(2496.3181150000005, 937.220305),
				new Point(2503.681885, 934.463565),
				new Point(2539.878362278914, 921.3333994166501),
				new Point(2539.878362278914, 1067),
			},
        };
		var expectedUpshift = new[] {
			// Gear 1
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
			// Gear 2
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
			// Gear 3
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
        };

		var countEm = 2;
		var emRatio = 1.0;

        var emFld = new ElectricMotorFullLoadCurve(EM_FullLoad_125kW_485Nm.Select(x =>
			new ElectricMotorFullLoadCurve.FullLoadEntry() {
				// invert motor full load curve here as would be done in respective reader class
				MotorSpeed = x[0].RPMtoRad(),
				FullDriveTorque = -x[1].SI<NewtonMeter>() * countEm,
				FullGenerationTorque = -x[2].SI<NewtonMeter>() * countEm,
			}).ToList());
		
		var emEffMap = GetEffMap(emFld, countEm);


		

		var emData = new ElectricMotorData() {
			EfficiencyData = new VoltageLevelData() {
				VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
					new ElectricMotorVoltageLevelData() {
						FullLoadCurve = emFld,
						EfficiencyMap = emEffMap,
					}
				}
			},
			TransmissionLossMap = TransmissionLossMapReader.CreateEmADCLossMap(1, emRatio, "ADC"),
			RatioADC = emRatio
		};
		var gearboxData = new Mock<IGearboxEngineeringInputData>().Object;
		var gear = new Mock<ITransmissionInputData>().Object;
		Mock.Get(gearboxData).Setup(g => g.Gears).Returns(new List<ITransmissionInputData>() {
			gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear,
        });

		var shiftPolygons = new List<ShiftPolygon>();

		var shiftStrategyParams = new ShiftStrategyParameters(); // use default (declaration) shift strategy params
		// var downshiftMaxSpeed = emFld.RatedSpeed * shiftStrategyParams.PEV_DownshiftSpeedFactor.LimitTo(0, 1);
		// var downshiftMinSpeed = emFld.RatedSpeed * shiftStrategyParams.PEV_DownshiftMinSpeedFactor;

		
		
        for (var i = 0; i < gearboxData.Gears.Count; i++) {
			var shiftPolygon = DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(
				i,
				emData,
				gearboxData.Gears,
				shiftStrategyParams.PEV_DownshiftSpeedFactor.LimitTo(0,1),
				shiftStrategyParams.PEV_DownshiftMinSpeedFactor
			);
			
			// var shiftpolygon = DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(i, 
			// 	emFld, emRatio, 
			// 	gearboxData.Gears, 
			// 	axlegearRatio,
			// 	r_dyn, 
			// 	downshiftMaxSpeed, 
			// 	downshiftMinSpeed);
            shiftPolygons.Add(shiftPolygon);
        }


		CommonSanityChecks(shiftPolygons, emData.EfficiencyData.MaxSpeed / emRatio);

		for (var i = 0; i < Math.Min(gearboxData.Gears.Count, Math.Min(expectedDownshift.Length, expectedUpshift.Length)); i++) {
			foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}

			foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}
		}

		Assert.AreEqual(0, shiftPolygons.First().Downshift.Count);
		Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);

        //var suffix = factorDownshiftSpeed.HasValue ? $"_{factorDownshiftSpeed.Value}" : "";

    }

	
	
	[TestCase]
    public void ComputePEVShiftLinesDeratedSanity()
    {
		var axlegearRatio = 2.64;
		var r_dyn = 0.421.SI<Meter>();

		var expectedDownshift = new[] {
			// Gear 1
            new Point[] {

			},
            // Gear 2
            new[] {
				new Point(1969.345479169557, -1067),
				new Point(1969.345479169557, -950.6),
				new Point(253.98783622789142, -950.6),
				new Point(253.98783622789142, 950.6),
				new Point(2452.135493, 950.6),
				new Point(2461.1589140000006, 950.6),
				new Point(2466.8630340000004, 948.41362),
				new Point(2481.590574, 942.781315),
				new Point(2496.3181150000005, 937.220305),
				new Point(2503.681885, 934.463565),
				new Point(2539.878362278914, 921.3333994166501),
				new Point(2539.878362278914, 1067),
            },
			// Gear 3
			new[] {
				new Point(1969.345479169557, -1067),
				new Point(1969.345479169557, -950.6),
				new Point(253.98783622789142, -950.6),
				new Point(253.98783622789142, 950.6),
				new Point(2452.135493, 950.6),
				new Point(2461.1589140000006, 950.6),
				new Point(2466.8630340000004, 948.41362),
				new Point(2481.590574, 942.781315),
				new Point(2496.3181150000005, 937.220305),
				new Point(2503.681885, 934.463565),
				new Point(2539.878362278914, 921.3333994166501),
				new Point(2539.878362278914, 1067),
			},
        };
		var expectedUpshift = new[] {
			// Gear 1
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
			// Gear 2
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
			// Gear 3
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
        };

		var countEm = 2;
		var emRatio = 1.0;

        var emFld = new ElectricMotorFullLoadCurve(EM_FullLoad_125kW_485Nm.Select(x =>
			new ElectricMotorFullLoadCurve.FullLoadEntry() {
				// invert motor full load curve here as would be done in respective reader class
				MotorSpeed = x[0].RPMtoRad(),
				FullDriveTorque = -x[1].SI<NewtonMeter>() * countEm,
				FullGenerationTorque = -x[2].SI<NewtonMeter>() * countEm,
			}).ToList());
		
		var emEffMap = GetEffMap(emFld, countEm);



		var continuousTorque = emFld.MaxGenerationTorque;
		var emData = new ElectricMotorData() {
			Overload = new OverloadData() {
				ContinuousTorque = continuousTorque,
			},
			EfficiencyData = new VoltageLevelData() {
				VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
					new ElectricMotorVoltageLevelData() {
						FullLoadCurve = emFld,
						EfficiencyMap = emEffMap,
					}
				}
			},
			TransmissionLossMap = TransmissionLossMapReader.CreateEmADCLossMap(1, emRatio, "ADC"),
			RatioADC = emRatio
		};
		var gearboxData = new Mock<IGearboxEngineeringInputData>().Object;
		var gear = new Mock<ITransmissionInputData>().Object;
		Mock.Get(gearboxData).Setup(g => g.Gears).Returns(new List<ITransmissionInputData>() {
			gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear,
        });

		var shiftPolygons = new List<ShiftPolygon>();
		var runData = new VectoRunData() { GearshiftParameters = new ShiftStrategyParameters() };
		var container = new Mock<IVehicleContainer>();
		container.Setup(c => c.RunData).Returns(runData);
		// var shiftStrategy = new PEVAMTShiftStrategy(container.Object);
		// var deRatedShiftLines = shiftStrategy.CalculateDeratedShiftLines(emData, gearboxData.Gears,
		// 	r_dyn, axlegearRatio, GearboxType.AMT);
		
		var shiftStrategy = new PEVAMTShiftStrategyPolygonCreator(runData.GearshiftParameters);
		//var deRatedShiftLines = shiftStrategy.CalculateDeratedShiftLines(emData, gearboxData.Gears,
		//    r_dyn, axlegearRatio, GearboxType.AMT);

		for (var i = 0; i < gearboxData.Gears.Count; i++) {
			//var emFld = emData.EfficiencyData.VoltageLevels.First().FullLoadCurve;
			var contTq = emData.Overload.ContinuousTorque;
			var limitedFld = DeclarationData.Gearbox.LimitElectricMotorFullLoadCurve(emFld, contTq);
			var limitedEm = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() {
					VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
						new DeratedVoltageLevelData(emFld.MaxSpeed) {
							FullLoadCurve = limitedFld
						}
					}
				},
				RatioADC = emData.RatioADC,
			};

			var deratedShiftLine = shiftStrategy.ComputeElectricMotorDeclarationShiftPolygon(GearboxType.APTN, i,
				gearboxData.Gears, axlegearRatio, r_dyn, emData, limitedEm);
			shiftPolygons.Add(deratedShiftLine);
		}


		CommonSanityChecks(shiftPolygons, emData.EfficiencyData.MaxSpeed / emRatio);

		for (var i = 0; i < Math.Min(gearboxData.Gears.Count, Math.Min(expectedDownshift.Length, expectedUpshift.Length)); i++) {
			foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}

			foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}
		}

		Assert.AreEqual(0, shiftPolygons.First().Downshift.Count);
		Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);

        //var suffix = factorDownshiftSpeed.HasValue ? $"_{factorDownshiftSpeed.Value}" : "";

    }

	
	
	[TestCase]
    public void ComputePEVShiftLinesDeratedSanityADC()
    {
		var axlegearRatio = 2.64;
		var r_dyn = 0.421.SI<Meter>();

		var expectedDownshiftNoADC = new[] {
			// Gear 1
            new Point[] {

			},
            // Gear 2
            new[] {
				new Point(1969.345479169557, -1067),
				new Point(1969.345479169557, -950.6),
				new Point(253.98783622789142, -950.6),
				new Point(253.98783622789142, 950.6),
				new Point(2452.135493, 950.6),
				new Point(2461.1589140000006, 950.6),
				new Point(2466.8630340000004, 948.41362),
				new Point(2481.590574, 942.781315),
				new Point(2496.3181150000005, 937.220305),
				new Point(2503.681885, 934.463565),
				new Point(2539.878362278914, 921.3333994166501),
				new Point(2539.878362278914, 1067),
            },
			// Gear 3
			new[] {
				new Point(1969.345479169557, -1067),
				new Point(1969.345479169557, -950.6),
				new Point(253.98783622789142, -950.6),
				new Point(253.98783622789142, 950.6),
				new Point(2452.135493, 950.6),
				new Point(2461.1589140000006, 950.6),
				new Point(2466.8630340000004, 948.41362),
				new Point(2481.590574, 942.781315),
				new Point(2496.3181150000005, 937.220305),
				new Point(2503.681885, 934.463565),
				new Point(2539.878362278914, 921.3333994166501),
				new Point(2539.878362278914, 1067),
			},
        };
		var expectedUpshiftNoADC = new[] {
			// Gear 1
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
			// Gear 2
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
			// Gear 3
			new[] {
				new Point(6627.393225000001, 1067),
				new Point(6627.393225000001, -1067),
			},
        };

		var countEm = 2;
		var emRatio = 2.0;

		var expectedUpshift =
			expectedUpshiftNoADC.Select(ps => ps.Select(p => new Point(p.X / emRatio, p.Y * emRatio)).ToArray()).ToArray();
		var expectedDownshift =
			expectedDownshiftNoADC.Select(ps => ps.Select(p => new Point(p.X / emRatio, p.Y * emRatio)).ToArray()).ToArray();

		
        var emFld = new ElectricMotorFullLoadCurve(EM_FullLoad_125kW_485Nm.Select(x =>
			new ElectricMotorFullLoadCurve.FullLoadEntry() {
				// invert motor full load curve here as would be done in respective reader class
				MotorSpeed = x[0].RPMtoRad(),
				FullDriveTorque = -x[1].SI<NewtonMeter>() * countEm,
				FullGenerationTorque = -x[2].SI<NewtonMeter>() * countEm,
			}).ToList());
		
		var emEffMap = GetEffMap(emFld, countEm);



		var continuousTorque = emFld.MaxGenerationTorque;
		var emData = new ElectricMotorData() {
			Overload = new OverloadData() {
				ContinuousTorque = continuousTorque,
			},
			EfficiencyData = new VoltageLevelData() {
				VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
					new ElectricMotorVoltageLevelData() {
						FullLoadCurve = emFld,
						EfficiencyMap = emEffMap,
					}
				}
			},
			TransmissionLossMap = TransmissionLossMapReader.CreateEmADCLossMap(1, emRatio, "ADC"),
			RatioADC = emRatio
		};
		var gearboxData = new Mock<IGearboxEngineeringInputData>().Object;
		var gear = new Mock<ITransmissionInputData>().Object;
		Mock.Get(gearboxData).Setup(g => g.Gears).Returns(new List<ITransmissionInputData>() {
			gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear,
        });

		var shiftPolygons = new List<ShiftPolygon>();
		var runData = new VectoRunData() { GearshiftParameters = new ShiftStrategyParameters() };
		var container = new Mock<IVehicleContainer>();
		container.Setup(c => c.RunData).Returns(runData);

		var polygonCreator = new PEVAMTShiftStrategyPolygonCreator(runData.GearshiftParameters);
		//var deRatedShiftLines = shiftStrategy.CalculateDeratedShiftLines(emData, gearboxData.Gears,
		//    r_dyn, axlegearRatio, GearboxType.AMT);

		for (var i = 0; i < gearboxData.Gears.Count; i++) {
			//var emFld = emData.EfficiencyData.VoltageLevels.First().FullLoadCurve;
			var contTq = emData.Overload.ContinuousTorque;
			var limitedFld = DeclarationData.Gearbox.LimitElectricMotorFullLoadCurve(emFld, contTq);
			var limitedEm = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() {
					VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
						new DeratedVoltageLevelData(emFld.MaxSpeed) {
							FullLoadCurve = limitedFld
						}
					}
				},
				RatioADC = emData.RatioADC,
			};

			var deratedShiftLine = polygonCreator.ComputeElectricMotorDeclarationShiftPolygon(GearboxType.APTN, i,
				gearboxData.Gears, axlegearRatio, r_dyn, emData, limitedEm);
			shiftPolygons.Add(deratedShiftLine);
		}


		CommonSanityChecks(shiftPolygons, emData.EfficiencyData.MaxSpeed / emRatio);
		
		for (var i = 0; i < Math.Min(gearboxData.Gears.Count, Math.Min(expectedDownshift.Length, expectedUpshift.Length)); i++) {
			foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}

			foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}
		}

		Assert.AreEqual(0, shiftPolygons.First().Downshift.Count);
		Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);

        //var suffix = factorDownshiftSpeed.HasValue ? $"_{factorDownshiftSpeed.Value}" : "";

    }

	
	
	private static EfficiencyMap GetEffMap(ElectricMotorFullLoadCurve emFld, int countEm)
	{
		var emEffMap = ElectricMotorMapReader.Create(
			InputDataHelper.InputDataAsTableData(
				"n [rpm] , T [Nm] , P_el [kW]",
				$"0, {emFld.MaxDriveTorque.Value()}, 0",
				$"0, {emFld.MaxGenerationTorque.Value()}, 0",
				$"0, 0, 0",
				$"{emFld.MaxSpeed.AsRPM / 3}, {emFld.MaxDriveTorque.Value()}, -300",
				$"{emFld.MaxSpeed.AsRPM / 3}, {emFld.MaxGenerationTorque.Value()}, 750",
				$"{emFld.MaxSpeed.AsRPM / 3}, 0, 100",
				$"{emFld.MaxSpeed.AsRPM / 2}, {emFld.MaxDriveTorque.Value()}, -300",
				$"{emFld.MaxSpeed.AsRPM / 2}, {emFld.MaxGenerationTorque.Value()}, 750",
				$"{emFld.MaxSpeed.AsRPM / 2}, 0, 100",
				$"{emFld.MaxSpeed.AsRPM}, {emFld.MaxDriveTorque.Value()}, -300",
				$"{emFld.MaxSpeed.AsRPM}, {emFld.MaxGenerationTorque.Value()}, 750",
				$"{emFld.MaxSpeed.AsRPM}, 0, 100"
			).ApplyFactor(ElectricMotorMapReader.Fields.PowerElectrical, 1E3), countEm, ExecutionMode.Declaration);
		return emEffMap;
	}

	[TestCase]
    public void ComputePEVShiftLinesDeRated()
    {

        var axlegearRatio = 2.64;
		var r_dyn = 0.421.SI<Meter>();


		var expectedDownshift = new Point[][] {
			new Point[] { },
			new[] {
				new Point(6480.117820000001, -319),
				new Point(6480.117820000001, -284.2),
				new Point(6480.11782, -284.2),
				new Point(6406.480118, -284.2),
				new Point(6332.842415, -284.2),
				new Point(6259.204713, -284.2),
				new Point(6185.56701, -284.2),
				new Point(6111.929308000001, -284.2),
				new Point(6038.291605, -284.2),
				new Point(5964.653903, -284.2),
				new Point(5891.016200000001, -284.2),
				new Point(5817.378498, -284.2),
				new Point(5743.740795, -284.2),
				new Point(5670.103092999999, -284.2),
				new Point(5596.46539, -284.2),
				new Point(5522.827688, -284.2),
				new Point(5449.189984999999, -284.2),
				new Point(5375.552283000001, -284.2),
				new Point(5301.91458, -284.2),
				new Point(5228.276878, -284.2),
				new Point(5154.639175000001, -284.2),
				new Point(5081.001473, -284.2),
				new Point(5007.36377, -284.2),
				new Point(4933.726068000001, -284.2),
				new Point(4860.088365, -284.2),
				new Point(4786.450663, -284.2),
				new Point(4712.81296, -284.2),
				new Point(4639.175258, -284.2),
				new Point(4565.537555, -284.2),
				new Point(4491.899853, -284.2),
				new Point(4418.26215, -284.2),
				new Point(4344.624448, -284.2),
				new Point(4270.986745, -284.2),
				new Point(4197.349043, -284.2),
				new Point(4123.71134, -284.2),
				new Point(4050.073638, -284.2),
				new Point(3976.435935, -284.2),
				new Point(3902.7982329999995, -284.2),
				new Point(3829.16053, -284.2),
				new Point(3755.5228280000006, -284.2),
				new Point(3681.8851250000002, -284.2),
				new Point(3608.247423, -284.2),
				new Point(3534.60972, -284.2),
				new Point(3460.972018, -284.2),
				new Point(3387.334315, -284.2),
				new Point(3313.696613, -284.2),
				new Point(3240.05891, -284.2),
				new Point(3166.4212079999998, -284.2),
				new Point(3092.783505, -284.2),
				new Point(3019.145803, -284.2),
				new Point(2945.5081000000005, -284.2),
				new Point(2871.870398, -284.2),
				new Point(2798.232695, -284.2),
				new Point(2724.594993, -284.2),
				new Point(2650.95729, -284.2),
				new Point(2577.319588, -284.2),
				new Point(2503.681885, -284.2),
				new Point(2496.3181150000005, -284.2),
				new Point(2481.590574, -284.2),
				new Point(2466.8630340000004, -284.2),
				new Point(2461.1589140000006, -284.2),
				new Point(2452.135493, -284.2),
				new Point(253.98783622789142, -284.2),
				new Point(253.98783622789142, 284.2),
				new Point(2452.135493, 284.2),
				new Point(2461.1589140000006, 284.2),
				new Point(2466.8630340000004, 284.2),
				new Point(2481.590574, 284.2),
				new Point(2496.3181150000005, 284.2),
				new Point(2503.681885, 284.2),
				new Point(2539.878362278914, 284.2),
				new Point(2539.878362278914, 319),
			},
			new[] {
				new Point(6480.117820000001, -319),
				new Point(6480.117820000001, -284.2),
				new Point(6480.11782, -284.2),
				new Point(6406.480118, -284.2),
				new Point(6332.842415, -284.2),
				new Point(6259.204713, -284.2),
				new Point(6185.56701, -284.2),
				new Point(6111.929308000001, -284.2),
				new Point(6038.291605, -284.2),
				new Point(5964.653903, -284.2),
				new Point(5891.016200000001, -284.2),
				new Point(5817.378498, -284.2),
				new Point(5743.740795, -284.2),
				new Point(5670.103092999999, -284.2),
				new Point(5596.46539, -284.2),
				new Point(5522.827688, -284.2),
				new Point(5449.189984999999, -284.2),
				new Point(5375.552283000001, -284.2),
				new Point(5301.91458, -284.2),
				new Point(5228.276878, -284.2),
				new Point(5154.639175000001, -284.2),
				new Point(5081.001473, -284.2),
				new Point(5007.36377, -284.2),
				new Point(4933.726068000001, -284.2),
				new Point(4860.088365, -284.2),
				new Point(4786.450663, -284.2),
				new Point(4712.81296, -284.2),
				new Point(4639.175258, -284.2),
				new Point(4565.537555, -284.2),
				new Point(4491.899853, -284.2),
				new Point(4418.26215, -284.2),
				new Point(4344.624448, -284.2),
				new Point(4270.986745, -284.2),
				new Point(4197.349043, -284.2),
				new Point(4123.71134, -284.2),
				new Point(4050.073638, -284.2),
				new Point(3976.435935, -284.2),
				new Point(3902.7982329999995, -284.2),
				new Point(3829.16053, -284.2),
				new Point(3755.5228280000006, -284.2),
				new Point(3681.8851250000002, -284.2),
				new Point(3608.247423, -284.2),
				new Point(3534.60972, -284.2),
				new Point(3460.972018, -284.2),
				new Point(3387.334315, -284.2),
				new Point(3313.696613, -284.2),
				new Point(3240.05891, -284.2),
				new Point(3166.4212079999998, -284.2),
				new Point(3092.783505, -284.2),
				new Point(3019.145803, -284.2),
				new Point(2945.5081000000005, -284.2),
				new Point(2871.870398, -284.2),
				new Point(2798.232695, -284.2),
				new Point(2724.594993, -284.2),
				new Point(2650.95729, -284.2),
				new Point(2577.319588, -284.2),
				new Point(2503.681885, -284.2),
				new Point(2496.3181150000005, -284.2),
				new Point(2481.590574, -284.2),
				new Point(2466.8630340000004, -284.2),
				new Point(2461.1589140000006, -284.2),
				new Point(2452.135493, -284.2),
				new Point(253.98783622789142, -284.2),
				new Point(253.98783622789142, 284.2),
				new Point(2452.135493, 284.2),
				new Point(2461.1589140000006, 284.2),
				new Point(2466.8630340000004, 284.2),
				new Point(2481.590574, 284.2),
				new Point(2496.3181150000005, 284.2),
				new Point(2503.681885, 284.2),
				new Point(2539.878362278914, 284.2),
				new Point(2539.878362278914, 319),
			}
		};
		var expectedUpshift = new Point[][] {
			new [] {
				new Point(7290.132547500001, 319),
				new Point(7290.132547500001, -319),
			},
			new [] {
				new Point(7290.132547500001, 319),
				new Point(7290.132547500001, -319),
			},
			new [] {
				new Point(7290.132547500001, 319),
				new Point(7290.132547500001, -319),
			}
		};
        //emData.ContinuousTorque = 500.SI<NewtonMeter>();
        //var contTqFld = new ElectricMotorFullLoadCurve(new List<ElectricMotorFullLoadCurve.FullLoadEntry>() {
        //        new ElectricMotorFullLoadCurve.FullLoadEntry() {
        //            MotorSpeed = 0.RPMtoRad(),
        //            FullDriveTorque = -emData.Overload.ContinuousTorque,
        //            FullGenerationTorque = emData.Overload.ContinuousTorque
        //        },
        //        new ElectricMotorFullLoadCurve.FullLoadEntry() {
        //            MotorSpeed = 1.1 * emData.EfficiencyData.VoltageLevels.First().FullLoadCurve.MaxSpeed,
        //            FullDriveTorque = -emData.Overload.ContinuousTorque,
        //            FullGenerationTorque = emData.Overload.ContinuousTorque
        //        }
        //    });

		var countEm = 2;
		var emRatio = 1.0;
		var continuousTorque = 145.SI<NewtonMeter>() * countEm;
        var emFld = new ElectricMotorFullLoadCurve(EM_FullLoad_125kW_485Nm.Select(x =>
			new ElectricMotorFullLoadCurve.FullLoadEntry() {
				// invert motor full load curve here as would be done in respective reader class
				MotorSpeed = x[0].RPMtoRad(),
				FullDriveTorque = -x[1].SI<NewtonMeter>() * countEm,
				FullGenerationTorque = -x[2].SI<NewtonMeter>() * countEm,
			}).ToList());

		var emEffMap = GetEffMap(emFld: emFld, countEm: countEm);
		

		var emData = new ElectricMotorData() {
			RatioADC = emRatio,
			Overload = new OverloadData() {
				ContinuousTorque = continuousTorque,
			},
			EfficiencyData = new VoltageLevelData() {
				VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
					new ElectricMotorVoltageLevelData() {
						FullLoadCurve = emFld,
						EfficiencyMap = emEffMap
					}
				}
			},
			TransmissionLossMap = TransmissionLossMapReader.CreateEmADCLossMap(1, emRatio, "ADC")
		};
		var gearboxData = new Mock<IGearboxEngineeringInputData>().Object;
		var gear = new Mock<ITransmissionInputData>().Object;
		Mock.Get(gearboxData).Setup(g => g.Gears).Returns(new List<ITransmissionInputData>() {
			gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear, gear,
		});
        //var limitedFld = AbstractSimulationDataAdapter.IntersectEMFullLoadCurves(emData.EfficiencyData.VoltageLevels.First().FullLoadCurve, contTqFld);

        //var fullLoadCurve = limitedFld.FullLoadEntries.Select(x =>
        //    new EngineFullLoadCurve.FullLoadCurveEntry() {
        //        EngineSpeed = x.MotorSpeed,
        //        TorqueFullLoad = -x.FullDriveTorque,
        //        TorqueDrag = -x.FullGenerationTorque
        //    }).ToList();
        //var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
        //var engineData = new CombustionEngineData() {
        //    IdleSpeed = 600.RPMtoRad()
        //};
        //fullLoadCurves[(uint)(0)] = new EngineFullLoadCurve(fullLoadCurve, null) { EngineData = engineData };

        var shiftPolygons = new List<ShiftPolygon>();
		var runData = new VectoRunData() { GearshiftParameters = new ShiftStrategyParameters() };
		var container = new Mock<IVehicleContainer>();
		container.Setup(c => c.RunData).Returns(runData);

		var polygonCreator = new PEVAMTShiftStrategyPolygonCreator(runData.GearshiftParameters);
		//var deRatedShiftLines = shiftStrategy.CalculateDeratedShiftLines(emData, gearboxData.Gears,
		//    r_dyn, axlegearRatio, GearboxType.AMT);

		for (var i = 0; i < gearboxData.Gears.Count; i++) {
			//var emFld = emData.EfficiencyData.VoltageLevels.First().FullLoadCurve;
			var contTq = emData.Overload.ContinuousTorque;
			var limitedFld = DeclarationData.Gearbox.LimitElectricMotorFullLoadCurve(emFld, contTq);
			var limitedEm = new ElectricMotorData() {
				EfficiencyData = new VoltageLevelData() {
					VoltageLevels = new List<ElectricMotorVoltageLevelData>() {
						new DeratedVoltageLevelData(emFld.MaxSpeed) {
							FullLoadCurve = limitedFld
						}
					}
				},
				RatioADC = emData.RatioADC,
			};

			var deratedShiftLine = polygonCreator.ComputeElectricMotorDeclarationShiftPolygon(GearboxType.APTN, i,
				gearboxData.Gears, axlegearRatio, r_dyn, emData, limitedEm);
			shiftPolygons.Add(deratedShiftLine);
		}

		
		CommonSanityChecks(shiftPolygons, emData.EfficiencyData.MaxSpeed / emRatio);
		
		for (var i = 0; i < Math.Min(gearboxData.Gears.Count, Math.Min(expectedDownshift.Length, expectedUpshift.Length)); i++) {
			foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}

			foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.AsRPM, 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
				Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
			}
		}

        //var suffix = factorDownshiftSpeed.HasValue ? $"_{factorDownshiftSpeed.Value}" : "";
        //var imageFile = Path.Combine(Path.GetDirectoryName(pevE2Job), Path.GetFileNameWithoutExtension(pevE2Job) + $"_shiftlines_DeRated{suffix}.png");

        //ShiftPolygonDrawer.DrawShiftPolygons(Path.GetDirectoryName(pevE2Job), fullLoadCurves, shiftPolygons,
        //    imageFile,
        //    DeclarationData.Gearbox.TruckMaxAllowedSpeed / r_dyn * axlegearRatio * gearboxData.Gears.Last().Ratio);
    }

	protected double[][] EM_FullLoad_125kW_485Nm = new[] {
        new[] { 0.0,485,-485 },
		new[] { 2452.135493,485,-485 },
		new[] { 2461.158914,485,-485 },
		new[] { 2466.863034,483.8845,-483.8845 },
		new[] { 2481.590574,481.010875,-481.010875 },
		new[] { 2496.318115,478.173625,-478.173625 },
		new[] { 2503.681885,476.767125,-476.767125 },
		new[] { 2577.319588,463.138625,-463.138625 },
		new[] { 2650.95729,450.274,-450.274 },
		new[] { 2724.594993,438.1005,-438.1005 },
		new[] { 2798.232695,426.58175,-426.58175 },
		new[] { 2871.870398,415.645,-415.645 },
		new[] { 2945.5081,405.253875,-405.253875 },
		new[] { 3019.145803,395.359875,-395.359875 },
		new[] { 3092.783505,385.950875,-385.950875 },
		new[] { 3166.421208,376.978375,-376.978375 },
		new[] { 3240.05891,368.406,-368.406 },
		new[] { 3313.696613,360.221625,-360.221625 },
		new[] { 3387.334315,352.388875,-352.388875 },
		new[] { 3460.972018,344.895625,-344.895625 },
		new[] { 3534.60972,337.7055,-337.7055 },
		new[] { 3608.247423,330.8185,-330.8185 },
		new[] { 3681.885125,324.19825,-324.19825 },
		new[] { 3755.522828,317.84475,-317.84475 },
		new[] { 3829.16053,311.73375,-311.73375 },
		new[] { 3902.798233,305.853125,-305.853125 },
		new[] { 3976.435935,300.178625,-300.178625 },
		new[] { 4050.073638,294.722375,-294.722375 },
		new[] { 4123.71134,289.460125,-289.460125 },
		new[] { 4197.349043,284.37975,-284.37975 },
		new[] { 4270.986745,279.48125,-279.48125 },
		new[] { 4344.624448,274.740375,-274.740375 },
		new[] { 4418.26215,270.16925,-270.16925 },
		new[] { 4491.899853,265.7315,-265.7315 },
		new[] { 4565.537555,261.451375,-261.451375 },
		new[] { 4639.175258,257.304625,-257.304625 },
		new[] { 4712.81296,253.279125,-253.279125 },
		new[] { 4786.450663,249.387,-249.387 },
		new[] { 4860.088365,245.604,-245.604 },
		new[] { 4933.726068,241.94225,-241.94225 },
		new[] { 5007.36377,238.3775,-238.3775 },
		new[] { 5081.001473,234.921875,-234.921875 },
		new[] { 5154.639175,231.575375,-231.575375 },
		new[] { 5228.276878,228.31375,-228.31375 },
		new[] { 5301.91458,225.137,-225.137 },
		new[] { 5375.552283,222.05725,-222.05725 },
		new[] { 5449.189985,219.05025,-219.05025 },
		new[] { 5522.827688,216.128125,-216.128125 },
		new[] { 5596.46539,213.290875,-213.290875 },
		new[] { 5670.103093,210.51425,-210.51425 },
		new[] { 5743.740795,207.8225,-207.8225 },
		new[] { 5817.378498,205.191375,-205.191375 },
		new[] { 5891.0162,202.620875,-202.620875 },
		new[] { 5964.653903,200.123125,-200.123125 },
		new[] { 6038.291605,197.686,-197.686 },
		new[] { 6111.929308,195.297375,-195.297375 },
		new[] { 6185.56701,192.969375,-192.969375 },
		new[] { 6259.204713,190.702,-190.702 },
		new[] { 6332.842415,188.483125,-188.483125 },
		new[] { 6406.480118,186.324875,-186.324875 },
		new[] { 6480.11782,184.203,-184.203 },
		new[] { 6553.755523,182.129625,-182.129625 },
		new[] { 6627.393225,180.10475,-180.10475 },
		new[] { 6701.030928,178.128375,-178.128375 },
		new[] { 6774.66863,176.2005,-176.2005 },
		new[] { 6848.306333,174.296875,-174.296875 },
		new[] { 6921.944035,172.44175,-172.44175 },
		new[] { 6995.581738,170.635125,-170.635125 },
		new[] { 7069.21944,168.85275,-168.85275 },
		new[] { 7142.857143,167.10675,-167.10675 },
		new[] { 7216.494845,165.40925,-165.40925 },
		new[] { 7290.132548,163.736,-163.736 },
		new[] { 7363.77025,162.099125,-162.099125 },
    };




	private void CommonSanityChecks(IEnumerable<ShiftPolygon> shiftPolygons, PerSecond emMaxSpeed)
	{
		var polygonList = shiftPolygons.ToList();
		foreach (var upShiftPolygon in polygonList.Select(p => p.Upshift)) {
			foreach (var entry in upShiftPolygon) {
				Assert.Less(entry.AngularSpeed, emMaxSpeed );
			}
		}

		foreach (var shiftPolygon in polygonList) {
			var upshift = shiftPolygon.Upshift;
			var downshift = shiftPolygon.Downshift;
			if (upshift.Count > 0 && downshift.Count > 0) {
				Assert.True(downshift.All(d => d.AngularSpeed.IsSmallerOrEqual(upshift.First().AngularSpeed)));
			}
		}





	}
}
