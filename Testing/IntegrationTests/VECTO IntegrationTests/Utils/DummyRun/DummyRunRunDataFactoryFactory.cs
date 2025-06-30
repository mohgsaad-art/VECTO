using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Generic;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunRunDataFactoryFactory : IVectoRunDataFactoryFactory
{
    #region Implementation of IVectoRunDataFactoryFactory

    private IDeclarationCycleFactory _cycleFactory;

    protected IPowertrainBuilder PowertrainBuilder;

    public DummyRunRunDataFactoryFactory(IDeclarationCycleFactory cycleFactory, IPowertrainBuilder ptBuilder)
    {
        _cycleFactory = cycleFactory;
        PowertrainBuilder = ptBuilder;
    }

    public IVectoRunDataFactory CreateDeclarationRunDataFactory(IInputDataProvider inputDataProvider,
        IDeclarationReport report,
        IVTPReport vtpReport)
    {
        if (inputDataProvider == null)
            throw new ArgumentNullException(nameof(inputDataProvider));


        switch (inputDataProvider)
        {
            case IVTPDeclarationInputDataProvider vtpProvider:
                return CreateRunDataReader(vtpProvider, vtpReport);
            case ISingleBusInputDataProvider singleBusProvider:
                return CreateRunDataReader(singleBusProvider, report);
            case IDeclarationInputDataProvider declDataProvider:
                return CreateRunDataReader(declDataProvider, report);
            case IMultistageVIFInputData multistageVifInputData:
                return CreateRunDataReader(multistageVifInputData, report);
            default:
                break;
        }
        throw new VectoException("Unknown InputData for Declaration Mode!");
    }

    private IVectoRunDataFactory CreateRunDataReader(IMultistageVIFInputData multiStepVifInputData, IDeclarationReport report)
    {
        if (multiStepVifInputData.VehicleInputData == null)
        {
            return new DummyRunMultistageCompletedBusRunDataFactory(
                multiStepVifInputData,
                report, new DeclarationDataAdapterSpecificCompletedBus.Conventional(),
                new DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.Conventional(),
                _cycleFactory, null, PowertrainBuilder);
        }
        else
        {
            return new DeclarationModeMultistageBusVectoRunDataFactory(multiStepVifInputData, report);
        }
    }

    private IVectoRunDataFactory CreateRunDataReader(IDeclarationInputDataProvider declDataProvider,
        IDeclarationReport report)
    {
        var vehicleCategory = declDataProvider.JobInputData.Vehicle.VehicleCategory;
        if (vehicleCategory.IsLorry())
        {
            return new DummyRunLorryVectoRunDataFactory(declDataProvider, report, new DeclarationDataAdapterHeavyLorry.Conventional(),
                _cycleFactory, null, PowertrainBuilder);
        }

        if (vehicleCategory.IsBus())

            switch (declDataProvider.JobInputData.Vehicle.VehicleCategory)
            {
                case VehicleCategory.HeavyBusCompletedVehicle:
                    throw new NotImplementedException();
                //return new DeclarationModeMultistageBusVectoRunDataFactory(declDataProvider, report);
                case VehicleCategory.HeavyBusPrimaryVehicle:
                    return new DummyRunPrimaryBusRunDataFactory(declDataProvider, report, new DeclarationDataAdapterPrimaryBus.Conventional(),
                        _cycleFactory, null, PowertrainBuilder);
                default:
                    break;
            }

        throw new Exception(
            $"Could not create RunDataFactory for Vehicle Category{vehicleCategory}");
    }

    private IVectoRunDataFactory CreateRunDataReader(ISingleBusInputDataProvider singleBusProvider, IDeclarationReport report)
    {
        throw new NotImplementedException();
    }

    private IVectoRunDataFactory CreateRunDataReader(IVTPDeclarationInputDataProvider vtpProvider, IVTPReport vtpReport)
    {
        throw new NotImplementedException();
    }

    public IVectoRunDataFactory CreateEngineeringRunDataFactory(IInputDataProvider inputDataProvider)
    {
        throw new NotImplementedException();
    }

    #endregion
}