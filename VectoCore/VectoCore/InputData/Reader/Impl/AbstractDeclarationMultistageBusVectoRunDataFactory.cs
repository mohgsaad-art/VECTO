using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public abstract class AbstractDeclarationMultistageBusVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{ 
		protected readonly IMultistageVIFInputData InputDataProvider;

		protected IDeclarationReport Report;
		

		protected AbstractDeclarationMultistageBusVectoRunDataFactory(IMultistageVIFInputData dataProvider, IDeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;
		}
		
		public IEnumerable<VectoRunData> NextRun()
		{
			if(Report != null)
				InitializeReport();

			return GetNextRun();
		}

		protected abstract IEnumerable<VectoRunData> GetNextRun();

		protected abstract VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading);

		protected virtual void InitializeReport()
		{
			var vehicle = InputDataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle;
			var powertrainConfig = CreateVectoRunData(vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
			var fuels = new List<List<FuelData.Entry>>();
			Report.InitializeReport(powertrainConfig, fuels);
		}
	}
}
