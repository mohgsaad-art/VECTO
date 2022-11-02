using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModeMultistageBusVectoRunDataFactory : AbstractDeclarationMultistageBusVectoRunDataFactory
	{
		public DeclarationModeMultistageBusVectoRunDataFactory(IMultistageVIFInputData dataProvider, IDeclarationReport report) 
			: base(dataProvider, report) { }

		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			yield return CreateVectoRunData(null, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
		}

		protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			return new VectoRunData
			{
				Exempted = InputDataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle,
				MultistageRun = true,
				ExecutionMode = ExecutionMode.Declaration,
				Report = Report,
				Mission = new Mission { MissionType = MissionType.ExemptedMission },
				VehicleData = CreateVehicleData(InputDataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle),
				MultistageVIFInputData = InputDataProvider
			};
		}
		
		private VehicleData CreateVehicleData(IVehicleDeclarationInputData data)
		{
			var exempted = SetCommonVehicleData(data);
			exempted.VIN = data.VIN;
			exempted.ManufacturerAddress = data.ManufacturerAddress;
			exempted.LegislativeClass = data.LegislativeClass;
			exempted.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			exempted.HybridElectricHDV = data.HybridElectricHDV;
			exempted.DualFuelVehicle = data.DualFuelVehicle;// true;
			exempted.MaxNetPower1 = data.MaxNetPower1;
			exempted.MaxNetPower2 = data.MaxNetPower2;

			return exempted;
		}
		
		private VehicleData SetCommonVehicleData(IVehicleDeclarationInputData data)
		{
			var retVal = new VehicleData
			{
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
	}
}
