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
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
    internal class DeclarationVTPModeVectoRunDataFactoryLorries : AbstractVTPModeVectoRunDataFactory
    {
		protected readonly IInputDataProvider InputDataProvider;

        public DeclarationVTPModeVectoRunDataFactoryLorries(IVTPDeclarationInputDataProvider ivtpProvider, IVTPReport report,
			ILorryDeclarationDataAdapter declarationDataAdapter) : base(
            ivtpProvider.JobInputData, report)
		{
			DataAdapter = declarationDataAdapter;
            InputDataProvider = ivtpProvider;
        }

        protected DeclarationVTPModeVectoRunDataFactoryLorries(IInputDataProvider inputProvider, IVTPReport report,
			ILorryDeclarationDataAdapter declarationDataAdapter) :
            base((inputProvider as IVTPEngineeringInputDataProvider).JobInputData, report)
		{
			DataAdapter = declarationDataAdapter;
            InputDataProvider = inputProvider;
        }

        public override IInputDataProvider DataProvider
		{
			get { return InputDataProvider; }
		}

		protected override IDeclarationDataAdapter Dao => DataAdapter;

		private ILorryDeclarationDataAdapter DataAdapter { get; } //return _dao ?? (_dao = new DeclarationDataAdapterHeavyLorry.Conventional()); }


		protected override void Initialize()
        {
            var vehicle = JobInputData.Vehicle;
            try
            {
                Segment = DeclarationData.TruckSegments.Lookup(
                    vehicle.VehicleCategory,
                    vehicle.AxleConfiguration,
                    vehicle.GrossVehicleMassRating,
                    vehicle.CurbMassChassis,
                    vehicle.VocationalVehicle);
            }
            catch (VectoException)
            {
                _allowVocational = false;
                Segment = DeclarationData.TruckSegments.Lookup(
                    vehicle.VehicleCategory,
                    vehicle.AxleConfiguration,
                    vehicle.GrossVehicleMassRating,
                    vehicle.CurbMassChassis,
                    false);
            }
            Driverdata = DataAdapter.CreateDriverData(Segment);
            var tempVehicle = Dao.CreateVehicleData(
                vehicle, Segment, Segment.Missions.First(),
                Segment.Missions.First().Loadings.First(), _allowVocational);

            var vtpMission = Segment.VehicleClass.IsMediumLorry()
                ? DeclarationData.VTPMode.SelectedMissionMediumLorry
				: DeclarationData.VTPMode.GetSelectedMissionHeavyLorry(Segment.VehicleClass);

            AirdragData = DataAdapter.CreateAirdragData(vehicle,
                Segment.Missions.First(), Segment, OvcHevMode.NotApplicable);
            EngineData = DataAdapter.CreateEngineData(
                vehicle, vehicle.Components.EngineInputData.EngineModes.First(),
                new Mission() { MissionType = vtpMission });
            AxlegearData = JobInputData.Vehicle.Components.GearboxInputData.DifferentialIncluded
                ? DataAdapter.CreateDummyAxleGearData(JobInputData.Vehicle.Components.GearboxInputData)
                : DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
            AngledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);

            GearboxData = DataAdapter.CreateGearboxData(
                vehicle, new VectoRunData() { EngineData = EngineData, AxleGearSinglePwt = AxlegearData, VehicleData = tempVehicle,
                Cycle = VTPCycle },
                null);
            RetarderData = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, vehicle.ArchitectureID, vehicle.Components.IEPC);

            PTOTransmissionData =
				DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData, vehicle.Components.GearboxInputData);

            GearshiftData = DataAdapter.CreateGearshiftData(
                 AxlegearData.AxleGear.Ratio * (AngledriveData?.Angledrive.Ratio ?? 1.0), EngineData.IdleSpeed, GearboxData.Type, GearboxData.Gears.Count);

            AuxVTP = CreateVTPAuxData(vehicle);
        }

        protected override IEnumerable<VectoRunData.AuxData> GetAuxiliaryData(MissionType missionType)
        {
            return DataAdapter.CreateAuxiliaryData(
                JobInputData.Vehicle.Components.AuxiliaryInputData,
                JobInputData.Vehicle.Components.BusAuxiliaries,
                missionType,
                Segment.VehicleClass, JobInputData.Vehicle.Length,
                JobInputData.Vehicle.Components.AxleWheels.NumSteeredAxles, JobInputData.Vehicle.VehicleType, false);
        }

        protected virtual List<VectoRunData.AuxData> CreateVTPAuxData(IVehicleDeclarationInputData vehicle)
        {
            var numSteered = vehicle.Components.AxleWheels.NumSteeredAxles;
            var auxRD = DataAdapter.CreateAuxiliaryData(
                                vehicle.Components.AuxiliaryInputData, vehicle.Components.BusAuxiliaries, MissionType.RegionalDelivery, Segment.VehicleClass, vehicle.Length, numSteered, vehicle.VehicleType, false)
                            .ToList();
            foreach (var entry in auxRD)
            {
                entry.MissionType = MissionType.RegionalDelivery;
            }

            var auxLH = DataAdapter.CreateAuxiliaryData(
                                vehicle.Components.AuxiliaryInputData, vehicle.Components.BusAuxiliaries, MissionType.LongHaul, Segment.VehicleClass, vehicle.Length, numSteered, vehicle.VehicleType, false)
                            .ToList();
            foreach (var entry in auxLH)
            {
                entry.MissionType = MissionType.LongHaul;
            }

            var auxUD = DataAdapter.CreateAuxiliaryData(
                                vehicle.Components.AuxiliaryInputData, vehicle.Components.BusAuxiliaries, MissionType.UrbanDelivery, Segment.VehicleClass, vehicle.Length, numSteered, vehicle.VehicleType, false)
                            .ToList();
            foreach (var entry in auxUD)
            {
                entry.MissionType = MissionType.UrbanDelivery;
            }

            var aux = new List<VectoRunData.AuxData>();
            aux.AddRange(auxRD);
            aux.AddRange(auxLH);
            aux.AddRange(auxUD);

            aux.RemoveAll(x => x.ID == Constants.Auxiliaries.IDs.Fan);
            aux.Add(
                new VectoRunData.AuxData
                {
                    DemandType = AuxiliaryDemandType.Direct,
                    ID = DrivingCycleDataReader.Fields.AdditionalAuxPowerDemand
                });
            return aux;
        }

        protected override IEnumerable<VectoRunData> GetNextRun()
        {
			// Loading is not relevant as we use P_wheel
            var vtpRunData = CreateVectoRunData(Segment, Segment.Missions.First(), Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null));
            vtpRunData.Cycle = VTPCycle;
            vtpRunData.Aux = AuxVTP;
            vtpRunData.FanDataVTP = GetFanData();
            vtpRunData.ExecutionMode = ExecutionMode.Declaration;
            vtpRunData.SimulationType = SimulationType.VerificationTest;
            vtpRunData.Mission = new Mission()
            {
                MissionType = MissionType.VerificationTest
            };
            vtpRunData.VehicleData.VehicleClass = Segment.VehicleClass;
            vtpRunData.VehicleData.LegislativeClass = JobInputData.Vehicle.LegislativeClass;
            vtpRunData.DriverData = Driverdata;

            //var ncvStd = DeclarationData.FuelData.Lookup(JobInputData.Vehicle.Components.EngineInputData.FuelType).LowerHeatingValueVecto;
            //var ncvCorrection = ncvStd / JobInputData.NetCalorificValueTestFuel;
            var mileageCorrection = GetMileagecorrectionFactor(JobInputData.Mileage);
            var correctionFactors = JobInputData.FuelNCVs.ToDictionary(
				keySelector: f => f.Type,
				elementSelector: f => (f.NCV / DeclarationData.FuelData.Lookup(
					f.Type,
					JobInputData.Vehicle.TankSystem).LowerHeatingValueVecto).Value() * mileageCorrection);

            vtpRunData.VTPData = new VTPData() {
				CorrectionFactors = correctionFactors,
				FuelNCVs = JobInputData.FuelNCVs
            };

			vtpRunData.TorqueDriftLeftWheel = JobInputData.TorqueDriftLeftWheel;
			vtpRunData.TorqueDriftRightWheel = JobInputData.TorqueDriftRightWheel;

            yield return vtpRunData;
        }
    }
}
