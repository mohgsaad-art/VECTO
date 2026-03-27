using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
    public interface ISimplePowertrainContainer : IPowertrainContainer
    {
        ITestPowertrainTransmission Gearbox { get; }
    }


    public interface IPowertrainContainer : IPowertrainDataBus
    {
        AxlePowertrainData AxlePowertrainData { get; }
    }

    public class SimplePowertrainContainer : PowertrainContainer, ISimplePowertrainContainer
    {
        private ISimpleVehicleContainer _simpleVehicleContainer;
        public SimplePowertrainContainer(ISimpleVehicleContainer simpleVehicleContainer) : this(simpleVehicleContainer, AxleNumber.NOT_IN_AXLE_POWERTRAIN) { }

        public SimplePowertrainContainer(ISimpleVehicleContainer simpleVehicleContainer, AxleNumber axleNumber) : base(simpleVehicleContainer, axleNumber)
        {
            _simpleVehicleContainer = simpleVehicleContainer;
        }

        // public ITnOutPort GearboxOutPort => (GearboxInfo as IGearbox).OutPort();
        public ITestPowertrainTransmission Gearbox => GearboxInfo as ITestPowertrainTransmission;
        public Dictionary<PowertrainPosition, IElectricMotorInfo> ElectricMotors => _vehicleContainer.ElectricMotorsInfo
            .Where(e => e.AxleNumber == AxleNumber).ToDictionary(e => e.Position, e => e);

    }

    [DebuggerDisplay("Powertrain: {AxleNumber}")]
    public class PowertrainContainer : IPowertrainContainer
    {
        protected readonly IVehicleContainer _vehicleContainer;

        public T GetFromAxleNumber<T>(IEnumerable<T> components) where T : class, IHasAxleNumber
        {
            return components?.SingleOrDefault(c => c.AxleNumber == AxleNumber);
        }

        // ReSharper disable once IntroduceOptionalParameters.Global
        /// Created with <see cref="IPowertrainComponentFactory"/>
        public PowertrainContainer(IVehicleContainer vehicleContainer) : this(vehicleContainer, AxleNumber.NOT_IN_AXLE_POWERTRAIN)
        {
        }

        // ReSharper disable once ConvertToPrimaryConstructor
        /// Created with <see cref="IPowertrainComponentFactory"/>
        public PowertrainContainer(IVehicleContainer vehicleContainer, AxleNumber axleNumber)
        {
            AxleNumber = axleNumber;
            _vehicleContainer = vehicleContainer;
            vehicleContainer.AddPowertrainContainer(this);
        }

        public AxleNumber AxleNumber { get; }

        public IGearboxInfo GearboxInfo => GetFromAxleNumber(_vehicleContainer.GearboxesInfo);
        public IGearboxControl GearboxControl => GetFromAxleNumber(_vehicleContainer.GearboxesCtl);
        public IClutchInfo ClutchInfo => GetFromAxleNumber(_vehicleContainer.ClutchesInfo);
        public IAxlegearInfo AxlegearInfo => GetFromAxleNumber(_vehicleContainer.AxlegearsInfo);
        public IRetarder Retarder => GetFromAxleNumber(_vehicleContainer.RetardersInfo);
        public ITorqueConverterControl TorqueConverterControl => GetFromAxleNumber(_vehicleContainer.TorqueConvertersCtl);
        public ITorqueConverterInfo TorqueConverterInfo => GetFromAxleNumber(_vehicleContainer.TorqueConvertersInfo);
        public IElectricMotorInfo ElectricMotorInfo(PowertrainPosition pos) => _vehicleContainer.ElectricMotorsInfo.FirstOrDefault(x => (x.Position == pos) && (x.AxleNumber == AxleNumber));
        public AxlePowertrainData AxlePowertrainData => _vehicleContainer.RunData.AxlePowertrainsDict[AxleNumber];
    }



}