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

using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.FileIO;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockSimulationDataFactory
	{
		/// <summary>
		/// Create gearboxdata instance directly from a file
		/// </summary>
		/// <param name="gearBoxFile"></param>
		/// <param name="engineFile"></param>
		/// <param name="declarationMode"></param>
		/// <returns>GearboxData instance</returns>
		public static GearboxData CreateGearboxDataFromFile(string gearBoxFile, string engineFile, bool declarationMode = true)
		{
			var gearboxInput = JSONInputDataFactory.ReadGearbox(gearBoxFile);
			var engineInput = JSONInputDataFactory.ReadEngine(engineFile);
			if (declarationMode) {
				var dao = new DeclarationDataAdapter();
				var vehicleInput = new MockDeclarationVehicleInputData() {
					EngineInputData = engineInput,
					GearboxInputData = gearboxInput
				};
				var mission = new Mission() {
					MissionType = MissionType.LongHaul
				};
				var engineData = dao.CreateEngineData(
					vehicleInput, engineInput.EngineModes.First(),
					mission);
				return dao.CreateGearboxData(
					new MockVehicleTestInputData() {
						Components = new MockComponentsTest() {
							GearboxInputData = gearboxInput,
							TorqueConverterInputData = (ITorqueConverterDeclarationInputData)gearboxInput
						}
					}, new VectoRunData() {
						EngineData = engineData,
						AxleGearData = new AxleGearData() {
							AxleGear = new TransmissionData() { Ratio = ((IAxleGearInputData)gearboxInput).Ratio },
						},
						VehicleData =
							new VehicleData() { VehicleCategory = VehicleCategory.RigidTruck, DynamicTyreRadius = 0.5.SI<Meter>() }
					}, null);
			} else {
				var dao = new EngineeringDataAdapter();
				var runData = new MockEngineeringVehicleInputData() {
					EngineInputData = engineInput,
					GearboxInputData = gearboxInput,
					TorqueConverterInputData = (ITorqueConverterEngineeringInputData)gearboxInput
				};
				var engineData = dao.CreateEngineData(runData, engineInput.EngineModes.First());
				return dao.CreateGearboxData(
					new MockEngineeringInputProvider() {
						DriverInputData = new MockDriverTestInputData() {
							GearshiftInputData = (IGearshiftEngineeringInputData)gearboxInput
						},
						JobInputData = new MockJobTestInputData() {
							Vehicle = new MockEngineeringVehicleInputData() {
								GearboxInputData = gearboxInput,
								TorqueConverterInputData = (ITorqueConverterEngineeringInputData)gearboxInput,
							}
						}
					}, new VectoRunData() {
						EngineData = engineData,
						VehicleData = new VehicleData() {
							VehicleCategory = VehicleCategory.RigidTruck,
							DynamicTyreRadius = 0.5.SI<Meter>()
						},
						AxleGearData = new AxleGearData() { AxleGear = new TransmissionData() { Ratio = 2.1 } }
					}, null);
			}
		}

		public static AxleGearData CreateAxleGearDataFromFile(string axleGearFile, bool declarationMode = true)
		{
			if (declarationMode) {
				var dao = new DeclarationDataAdapter();
				var axleGearInput = JSONInputDataFactory.ReadGearbox(axleGearFile);
				return dao.CreateAxleGearData((IAxleGearInputData)axleGearInput);
			} else {
				var dao = new EngineeringDataAdapter();
				var axleGearInput = JSONInputDataFactory.ReadGearbox(axleGearFile);
				return dao.CreateAxleGearData((IAxleGearInputData)axleGearInput);
			}
		}

		public static CombustionEngineData CreateEngineDataFromFile(string engineFile, int numGears)
		{
			var dao = new EngineeringDataAdapter();
			var engineInput = JSONInputDataFactory.ReadEngine(engineFile);
			var vehicleInput = new MockEngineeringVehicleInputData() {
				EngineInputData = engineInput,
			};
			var engineData = dao.CreateEngineData(vehicleInput, engineInput.EngineModes.First());
			for (uint i = 1; i <= numGears; i++) {
				engineData.FullLoadCurves[i] = engineData.FullLoadCurves[0];
			}
			return engineData;
		}

		public static VehicleData CreateVehicleDataFromFile(string vehicleDataFile)
		{
			var dao = new EngineeringDataAdapter();
			var vehicleInput = JSONInputDataFactory.ReadJsonVehicle(vehicleDataFile, null);
			var airdragData = vehicleInput as IAirdragEngineeringInputData;
			return dao.CreateVehicleData(vehicleInput);
		}

		public static AirdragData CreateAirdragDataFromFile(string vehicleDataFile)
		{
			var dao = new EngineeringDataAdapter();
			var vehicleInput = JSONInputDataFactory.ReadJsonVehicle(vehicleDataFile, null);
			var airdragData = vehicleInput as IAirdragEngineeringInputData;
			return dao.CreateAirdragData(airdragData, vehicleInput);
		}

		public static DriverData CreateDriverDataFromFile(string driverDataFile)
		{
			var jobInput = JSONInputDataFactory.ReadJsonJob(driverDataFile);
			var engineeringJob = jobInput as IEngineeringInputDataProvider;
			if (engineeringJob == null) {
				throw new VectoException("Failed to cas to Engineering InputDataProvider");
			}
			var dao = new EngineeringDataAdapter();
			return dao.CreateDriverData(engineeringJob.DriverInputData);
		}
	}

	public class MockComponentsTest : IVehicleComponentsDeclaration
	{
		public IAirdragDeclarationInputData AirdragInputData { get; }
		public IGearboxDeclarationInputData GearboxInputData { get; set; }
		public ITorqueConverterDeclarationInputData TorqueConverterInputData { get; set; }
		public IAxleGearInputData AxleGearInputData { get; }
		public IAngledriveInputData AngledriveInputData { get; }
		public IEngineDeclarationInputData EngineInputData { get; }
		public IAuxiliariesDeclarationInputData AuxiliaryInputData { get; }
		public IRetarderInputData RetarderInputData { get; }
		public IPTOTransmissionInputData PTOTransmissionInputData { get; }
		public IAxlesDeclarationInputData AxleWheels { get; }
	}

	public class MockVehicleTestInputData : IVehicleDeclarationInputData
	{
		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public string Date { get; }
		public CertificationMethod CertificationMethod { get; }
		public string CertificationNumber { get; }
		public DigestData DigestValue { get; }
		public string Identifier { get; }
		public bool ExemptedVehicle { get; }
		public string VIN { get; }
		public LegislativeClass LegislativeClass { get; }
		public VehicleCategory VehicleCategory { get; }
		public AxleConfiguration AxleConfiguration { get; }
		public Kilogram CurbMassChassis { get; }
		public Kilogram GrossVehicleMassRating { get; }
		public IList<ITorqueLimitInputData> TorqueLimits { get; }
		public string ManufacturerAddress { get; }
		public PerSecond EngineIdleSpeed { get; }
		public bool VocationalVehicle { get; }
		public bool? SleeperCab { get; }
		public TankSystem? TankSystem { get; }
		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get; }
		public bool ZeroEmissionVehicle { get; }
		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public Watt MaxNetPower1 { get; }
		public Watt MaxNetPower2 { get; }
		public IVehicleComponentsDeclaration Components { get; set; }
	}
}
