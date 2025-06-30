using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
{
    public class SimulatorFactoryEngineering : SimulatorFactory
    {
		public SimulatorFactoryEngineering(IInputDataProvider dataProvider, IOutputDataWriter writer, bool validate,
			IVectoRunDataFactoryFactory runDataFactoryFactory, IPowertrainBuilder ptBuilder, IModalDataFactory modDataFactory)
			: base(ExecutionMode.Engineering, writer, validate, ptBuilder, modDataFactory)
		{
			RunDataFactory = runDataFactoryFactory.CreateEngineeringRunDataFactory(dataProvider);
		}

	}
}
