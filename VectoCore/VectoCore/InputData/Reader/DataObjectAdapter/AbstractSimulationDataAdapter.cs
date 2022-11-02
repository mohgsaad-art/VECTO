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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public abstract class AbstractSimulationDataAdapter : LoggingObject
	{
		// =========================

		internal VehicleData SetCommonVehicleData(IVehicleDeclarationInputData data)
		{
			var retVal = new VehicleData {
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				//CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				VehicleCategory = data.VehicleCategory,
				CurbMass = data.CurbMassChassis,
				GrossVehicleMass = data.GrossVehicleMassRating,
				AirDensity = Physics.AirDensity,
			};

			return retVal;
		}

		protected VehicleData.ADASData CreateADAS(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return new VehicleData.ADASData {
				EngineStopStart = adas.EngineStopStart,
				EcoRoll = adas.EcoRoll,
				PredictiveCruiseControl = adas.PredictiveCruiseControl,
				InputData = adas
			};
		}

		internal AirdragData SetCommonAirdragData(IAirdragDeclarationInputData data)
		{
			var retVal = new AirdragData() {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				CertificationMethod = data.CertificationMethod,
				CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
			};
			return retVal;
		}

		internal RetarderData SetCommonRetarderData(IRetarderInputData retarderInputData, 
			PowertrainPosition position = PowertrainPosition.HybridPositionNotSet)
		{
			try {
				var retarder = new RetarderData { Type = retarderInputData.Type };

				switch (retarder.Type) {
					case RetarderType.TransmissionInputRetarder:
					case RetarderType.TransmissionOutputRetarder:
						if (!(position.IsParallelHybrid() || position.IsOneOf(PowertrainPosition.HybridPositionNotSet, PowertrainPosition.BatteryElectricE2))) {
							throw new ArgumentException("Transmission retarder is only allowed in powertrains that " +
														"contain a gearbox: Conventional, HEV-P, and PEV-E2.", nameof(retarder));
						}

						retarder.LossMap = RetarderLossMapReader.Create(retarderInputData.LossMap);
						retarder.Ratio = retarderInputData.Ratio;
						break;
					
					case RetarderType.AxlegearInputRetarder:
						if (position != PowertrainPosition.BatteryElectricE3)
							throw new ArgumentException("AxlegearInputRetarder is only allowed for PEV-E3, HEV-S3, S-IEPC, E-IEPC. ", nameof(retarder));
						retarder.LossMap = RetarderLossMapReader.Create(retarderInputData.LossMap);
						retarder.Ratio = retarderInputData.Ratio;
						break;
					
					case RetarderType.None:
					case RetarderType.LossesIncludedInTransmission:
					case RetarderType.EngineRetarder:
						retarder.Ratio = 1;
						break;
					
					default:
						throw new ArgumentOutOfRangeException(nameof(retarder), retarder.Type, "RetarderType unknown");
				}

				if (retarder.Type.IsDedicatedComponent()) {
					retarder.SavedInDeclarationMode = retarderInputData.SavedInDeclarationMode;
					retarder.Manufacturer = retarderInputData.Manufacturer;
					retarder.ModelName = retarderInputData.Model;
					retarder.Date = retarderInputData.Date;
					retarder.CertificationMethod = retarderInputData.CertificationMethod;
					retarder.CertificationNumber = retarderInputData.CertificationNumber;
					retarder.DigestValueInput = retarderInputData.DigestValue != null ? retarderInputData.DigestValue.DigestValue : "";
				}

				return retarder;
			} catch (Exception e) {
				throw new VectoException("Error while Reading Retarder Data: {0}", e.Message);
			}
		}

		internal CombustionEngineData SetCommonCombustionEngineData(IEngineDeclarationInputData data, TankSystem? tankSystem)
		{
			var retVal = new CombustionEngineData {
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				CertificationNumber = data.CertificationNumber,
				CertificationMethod = CertificationMethod.Measured,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				Displacement = data.Displacement,
				//IdleSpeed = data.IdleSpeed,
				//ConsumptionMap = FuelConsumptionMapReader.Create(data.FuelConsumptionMap),
				RatedPowerDeclared = data.RatedPowerDeclared,
				RatedSpeedDeclared = data.RatedSpeedDeclared,
				MaxTorqueDeclared = data.MaxTorqueDeclared,
				//FuelData = DeclarationData.FuelData.Lookup(data.FuelType, tankSystem)
				MultipleEngineFuelModes = data.EngineModes.Count > 1,
				WHRType = data.WHRType,
			};
			return retVal;
		}

		internal static GearboxData SetCommonGearboxData(IGearboxDeclarationInputData data)
		{
			return new GearboxData {
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				CertificationMethod = data.CertificationMethod,
				CertificationNumber = data.CertificationMethod != CertificationMethod.StandardValues ? 
					data.CertificationNumber : "",
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				Type = data.Type
			};
		}

		protected virtual TransmissionLossMap CreateGearLossMap(ITransmissionInputData gear, uint i, 
			bool useEfficiencyFallback, VehicleCategory vehicleCategory, GearboxType gearboxType)
		{
			if (gear.LossMap != null) {
				return TransmissionLossMapReader.Create(gear.LossMap, gear.Ratio, $"Gear {i + 1}", true);
			}
			if (useEfficiencyFallback) {
				return TransmissionLossMapReader.Create(gear.Efficiency, gear.Ratio, $"Gear {i + 1}");
			}
			throw new InvalidFileFormatException("Gear {0} LossMap missing.", i + 1);
		}

		protected static void CreateTCSecondGearATSerial(GearData gearData,
			ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = gearData.Ratio;
			gearData.TorqueConverterGearLossMap = gearData.LossMap;
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		protected static void CreateTCFirstGearATSerial(GearData gearData,
			ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = gearData.Ratio;
			gearData.TorqueConverterGearLossMap = gearData.LossMap;
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		protected virtual void CretateTCFirstGearATPowerSplit(GearData gearData, uint i, ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = 1;
			gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(1, 1, $"TCGear {i + 1}");
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		
		internal TransmissionLossMap ReadAxleLossMap(IAxleGearInputData data, bool useEfficiencyFallback)
		{
			TransmissionLossMap axleLossMap;
			if (data.LossMap == null && useEfficiencyFallback) {
				axleLossMap = TransmissionLossMapReader.Create(data.Efficiency, data.Ratio, "Axlegear");
			} else {
				if (data.LossMap == null) {
					throw new InvalidFileFormatException("LossMap for Axlegear is missing.");
				}
				axleLossMap = TransmissionLossMapReader.Create(data.LossMap, data.Ratio, "Axlegear", true);
			}
			if (axleLossMap == null) {
				throw new InvalidFileFormatException("LossMap for Axlegear is missing.");
			}
			return axleLossMap;
		}

		internal AxleGearData SetCommonAxleGearData(IAxleGearInputData data)
		{
			return new AxleGearData {
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				LineType = data.LineType,
				Date = data.Date,
				CertificationMethod = data.CertificationMethod,
				CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				AxleGear = new GearData { Ratio = data.Ratio }
			};
		}

		/// <summary>
		/// Creates an AngledriveData or returns null if there is no anglegear.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="useEfficiencyFallback">if true, the Efficiency value is used if no LossMap is found.</param>
		/// <returns></returns>
		internal AngledriveData DoCreateAngledriveData(IAngledriveInputData data, bool useEfficiencyFallback)
		{
			try {
				var type = data?.Type ?? AngledriveType.None;

				switch (type) {
					case AngledriveType.LossesIncludedInGearbox:
					case AngledriveType.None:
						return null;
					case AngledriveType.SeparateAngledrive:
						var angledriveData = new AngledriveData {
							InputData = data,
							SavedInDeclarationMode = data.SavedInDeclarationMode,
							Manufacturer = data.Manufacturer,
							ModelName = data.Model,
							Date = data.Date,
							CertificationMethod = data.CertificationMethod,
							CertificationNumber = data.CertificationNumber,
							DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
							Type = type,
							Angledrive = new TransmissionData { Ratio = data.Ratio }
						};
						try {
							angledriveData.Angledrive.LossMap = TransmissionLossMapReader.Create(data.LossMap,
								data.Ratio, "Angledrive", true);
						} catch (VectoException ex) {
							Log.Info("Angledrive Loss Map not found.");
							if (useEfficiencyFallback) {
								Log.Info("Angledrive Trying with Efficiency instead of Loss Map.");
								angledriveData.Angledrive.LossMap = TransmissionLossMapReader.Create(data.Efficiency,
									data.Ratio, "Angledrive");
							} else {
								throw new VectoException("Angledrive: LossMap not found.", ex);
							}
						}
						return angledriveData;
					default:
						throw new ArgumentOutOfRangeException(nameof(data), "Unknown Angledrive Type.");
				}
			} catch (Exception e) {
				throw new VectoException("Error while reading Angledrive data: {0}", e.Message, e);
			}
		}

		internal static ElectricMotorFullLoadCurve IntersectEMFullLoadCurves(ElectricMotorFullLoadCurve fullLoadCurve,
			ElectricMotorFullLoadCurve maxTorqueCurve)
		{
			if (maxTorqueCurve == null)
			{
				return fullLoadCurve;
			}

			if (maxTorqueCurve.MaxSpeed.IsSmaller(fullLoadCurve.MaxSpeed)) {
				throw new VectoException("EM Torque limitation has to cover the whole speed range");
			}

			var motorSpeeds = fullLoadCurve.FullLoadEntries.Select(x => x.MotorSpeed)
				.Concat(maxTorqueCurve.FullLoadEntries.Select(x => x.MotorSpeed)).ToList();
			// iterate over all segments in the full-load curve
			foreach (var fldTuple in fullLoadCurve.FullLoadEntries.Pairwise()) {
				// find all grid points of max torque curve within the current segment of fld
				var maxPtsWithinSegment = maxTorqueCurve.FullLoadEntries.Where(x =>
					x.MotorSpeed.IsGreaterOrEqual(fldTuple.Item1.MotorSpeed) &&
					x.MotorSpeed.IsSmallerOrEqual(fldTuple.Item2.MotorSpeed)).OrderBy(x => x.MotorSpeed).ToList();
				if (maxPtsWithinSegment.Count == 0) {
					// if grid pint is within, take the 'surrounding' segment
					var segment =
						maxTorqueCurve.FullLoadEntries.GetSection(x => x.MotorSpeed < fldTuple.Item1.MotorSpeed);
					maxPtsWithinSegment = new[] { segment.Item1, segment.Item2 }.ToList();
				} else {
					// add the point just before and just after the current list of points 
					if (maxPtsWithinSegment.Min(x => x.MotorSpeed).IsGreater(fldTuple.Item1.MotorSpeed)) {
						maxPtsWithinSegment.Add(maxTorqueCurve.FullLoadEntries.Last(x =>
							x.MotorSpeed.IsSmaller(fldTuple.Item1.MotorSpeed)));
					}

					if (maxPtsWithinSegment.Max(x => x.MotorSpeed).IsSmaller(fldTuple.Item2.MotorSpeed)) {
						maxPtsWithinSegment.Add(maxTorqueCurve.FullLoadEntries.First(x => x.MotorSpeed.IsGreater(fldTuple.Item2.MotorSpeed)));
					}

					maxPtsWithinSegment = maxPtsWithinSegment.OrderBy(x => x.MotorSpeed).ToList();
				}

				var fldEdgeDrive =
					Edge.Create(new Point(fldTuple.Item1.MotorSpeed.Value(), fldTuple.Item1.FullDriveTorque.Value()),
						new Point(fldTuple.Item2.MotorSpeed.Value(), fldTuple.Item2.FullDriveTorque.Value()));
				var fldEdgeGenerate =
					Edge.Create(new Point(fldTuple.Item1.MotorSpeed.Value(), fldTuple.Item1.FullGenerationTorque.Value()),
						new Point(fldTuple.Item2.MotorSpeed.Value(), fldTuple.Item2.FullGenerationTorque.Value()));
				foreach (var maxTuple in maxPtsWithinSegment.Pairwise()) {
					var maxEdgeDrive =
						Edge.Create(new Point(maxTuple.Item1.MotorSpeed.Value(), maxTuple.Item1.FullDriveTorque.Value()),
							new Point(maxTuple.Item2.MotorSpeed.Value(), maxTuple.Item2.FullDriveTorque.Value()));
					if (!(maxEdgeDrive.SlopeXY - fldEdgeDrive.SlopeXY).IsEqual(0, 1e-12)) {
						// lines are not parallel
						var nIntersectDrive =
							((fldEdgeDrive.OffsetXY - maxEdgeDrive.OffsetXY) /
							(maxEdgeDrive.SlopeXY - fldEdgeDrive.SlopeXY)).SI<PerSecond>();
						if (nIntersectDrive.IsBetween(fldTuple.Item1.MotorSpeed, fldTuple.Item2.MotorSpeed)) {
							motorSpeeds.Add(nIntersectDrive);
						}
					}

					var maxEdgeGenerate =
						Edge.Create(new Point(maxTuple.Item1.MotorSpeed.Value(), maxTuple.Item1.FullGenerationTorque.Value()),
							new Point(maxTuple.Item2.MotorSpeed.Value(), maxTuple.Item2.FullGenerationTorque.Value()));
					if (!((maxEdgeGenerate.SlopeXY - fldEdgeGenerate.SlopeXY).IsEqual(0, 1e-12))) {
						// lines are not parallel
						var nIntersectGenerate =
							((fldEdgeGenerate.OffsetXY - maxEdgeGenerate.OffsetXY) /
							(maxEdgeGenerate.SlopeXY - fldEdgeGenerate.SlopeXY)).SI<PerSecond>();


						if (nIntersectGenerate.IsBetween(fldTuple.Item1.MotorSpeed, fldTuple.Item2.MotorSpeed)) {
							motorSpeeds.Add(nIntersectGenerate);
						}
					}
				}
			}

			// create new full-load curve with values closest to zero.
			return new ElectricMotorFullLoadCurve(motorSpeeds.OrderBy(x => x.Value()).Distinct().Select(x =>
				new ElectricMotorFullLoadCurve.FullLoadEntry() {
					MotorSpeed = x,
					FullDriveTorque = VectoMath.Max(fullLoadCurve.FullLoadDriveTorque(x),
						maxTorqueCurve.FullLoadDriveTorque(x)),
					FullGenerationTorque = VectoMath.Min(fullLoadCurve.FullGenerationTorque(x),
						maxTorqueCurve.FullGenerationTorque(x)),
				}).ToList());
		}

		



		/// <summary>
		/// Intersects ICE full load curves.
		/// </summary>
		/// <param name="engineCurve"></param>
		/// <param name="maxTorque"></param>
		/// <returns>A combined EngineFullLoadCurve with the minimum full load torque over all inputs curves.</returns>
		internal static EngineFullLoadCurve IntersectFullLoadCurves(EngineFullLoadCurve engineCurve, NewtonMeter maxTorque)
		{
			if (maxTorque == null) {
				return engineCurve;
			}

			var entries = new List<EngineFullLoadCurve.FullLoadCurveEntry>();
			var firstEntry = engineCurve.FullLoadEntries.First();
			if (firstEntry.TorqueFullLoad < maxTorque) {
				entries.Add(engineCurve.FullLoadEntries.First());
			} else {
				entries.Add(new EngineFullLoadCurve.FullLoadCurveEntry {
					EngineSpeed = firstEntry.EngineSpeed,
					TorqueFullLoad = maxTorque,
					TorqueDrag = firstEntry.TorqueDrag
				});
			}
			foreach (var entry in engineCurve.FullLoadEntries.Pairwise(Tuple.Create)) {
				if (entry.Item1.TorqueFullLoad <= maxTorque && entry.Item2.TorqueFullLoad <= maxTorque) {
					// segment is below maxTorque line -> use directly
					entries.Add(entry.Item2);
				} else if (entry.Item1.TorqueFullLoad > maxTorque && entry.Item2.TorqueFullLoad > maxTorque) {
					// segment is above maxTorque line -> add limited entry
					entries.Add(new EngineFullLoadCurve.FullLoadCurveEntry {
						EngineSpeed = entry.Item2.EngineSpeed,
						TorqueFullLoad = maxTorque,
						TorqueDrag = entry.Item2.TorqueDrag
					});
				} else {
					// segment intersects maxTorque line -> add new entry at intersection
					var edgeFull = Edge.Create(
						new Point(entry.Item1.EngineSpeed.Value(), entry.Item1.TorqueFullLoad.Value()),
						new Point(entry.Item2.EngineSpeed.Value(), entry.Item2.TorqueFullLoad.Value()));
					var edgeDrag = Edge.Create(
						new Point(entry.Item1.EngineSpeed.Value(), entry.Item1.TorqueDrag.Value()),
						new Point(entry.Item2.EngineSpeed.Value(), entry.Item2.TorqueDrag.Value()));
					var intersectionX = (maxTorque.Value() - edgeFull.OffsetXY) / edgeFull.SlopeXY;
                    if (!entries.Any(x => x.EngineSpeed.IsEqual(intersectionX)) && !intersectionX.IsEqual(entry.Item2.EngineSpeed.Value())) {
                        entries.Add(new EngineFullLoadCurve.FullLoadCurveEntry {
							EngineSpeed = intersectionX.SI<PerSecond>(),
							TorqueFullLoad = maxTorque,
							TorqueDrag = VectoMath.Interpolate(edgeDrag.P1, edgeDrag.P2, intersectionX).SI<NewtonMeter>()
						});
                    }

                    entries.Add(new EngineFullLoadCurve.FullLoadCurveEntry {
						EngineSpeed = entry.Item2.EngineSpeed,
						TorqueFullLoad = entry.Item2.TorqueFullLoad > maxTorque ? maxTorque : entry.Item2.TorqueFullLoad,
						TorqueDrag = entry.Item2.TorqueDrag
					});
				}
			}

			var flc = new EngineFullLoadCurve(entries.ToList(), engineCurve.PT1Data) {
				EngineData = engineCurve.EngineData,
			};
			return flc;
		}


		/// <summary>
		/// Intersects max torque curve.
		/// </summary>
		/// <param name="maxTorqueEntries"></param>
		/// <param name="maxTorque"></param>
		/// <returns>A combined EngineFullLoadCurve with the minimum full load torque over all inputs curves.</returns>
		internal static IList<VehicleMaxPropulsionTorque.FullLoadEntry> IntersectMaxPropulsionTorqueCurve(IList<VehicleMaxPropulsionTorque.FullLoadEntry> maxTorqueEntries, NewtonMeter maxTorque)
		{
			if (maxTorque == null) {
				return maxTorqueEntries;
			}

			var entries = new List<VehicleMaxPropulsionTorque.FullLoadEntry>();
			var firstEntry = maxTorqueEntries.First();
			if (firstEntry.FullDriveTorque < maxTorque) {
				entries.Add(maxTorqueEntries.First());
			} else {
				entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry {
					MotorSpeed = firstEntry.MotorSpeed,
					FullDriveTorque = maxTorque,
				});
			}
			foreach (var entry in maxTorqueEntries.Pairwise(Tuple.Create)) {
				if (entry.Item1.FullDriveTorque <= maxTorque && entry.Item2.FullDriveTorque <= maxTorque) {
					// segment is below maxTorque line -> use directly
					entries.Add(entry.Item2);
				} else if (entry.Item1.FullDriveTorque > maxTorque && entry.Item2.FullDriveTorque > maxTorque) {
					// segment is above maxTorque line -> add limited entry
					entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry {
						MotorSpeed = entry.Item2.MotorSpeed,
						FullDriveTorque = maxTorque,
					});
				} else {
					// segment intersects maxTorque line -> add new entry at intersection
					var edgeFull = Edge.Create(
						new Point(entry.Item1.MotorSpeed.Value(), entry.Item1.FullDriveTorque.Value()),
						new Point(entry.Item2.MotorSpeed.Value(), entry.Item2.FullDriveTorque.Value()));
					
					var intersectionX = (maxTorque.Value() - edgeFull.OffsetXY) / edgeFull.SlopeXY;
					if (!entries.Any(x => x.MotorSpeed.IsEqual(intersectionX)) && !intersectionX.IsEqual(entry.Item2.MotorSpeed.Value())) {
						entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry {
							MotorSpeed = intersectionX.SI<PerSecond>(),
							FullDriveTorque = maxTorque,
						});
					}

					entries.Add(new VehicleMaxPropulsionTorque.FullLoadEntry {
						MotorSpeed = entry.Item2.MotorSpeed,
						FullDriveTorque = entry.Item2.FullDriveTorque > maxTorque ? maxTorque : entry.Item2.FullDriveTorque,
						
					});
				}
			}

			
			return entries;
		}
	}
}


