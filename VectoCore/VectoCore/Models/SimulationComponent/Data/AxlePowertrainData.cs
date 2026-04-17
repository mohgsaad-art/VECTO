using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class AxlePowertrainData : IEquatable<AxlePowertrainData>
	{
		public AxlePowertrainData()
		{
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
        }

		[Required]
		public AxleNumber AxleNumber { get; internal set; }

		[Required]
		public ArchitectureID Architecture { get; internal set; }

		[ValidateObject]
		public GearboxData GearboxData { get; internal set; }

		[ValidateObject]
		public AxleGearData AxleGearData { get; internal set; }

		[ValidateObject]
		public AngledriveData AngledriveData { get; internal set; }

		[ValidateObject]
		public RetarderData Retarder { get; internal set; }

		[ValidateObject]
		public PTOData PTO { get; internal set; }

        public IDrivingCycleData PTOCycleWhileDrive { get; internal set; }

        public ShiftStrategyParameters GearshiftParameters { get; internal set; }

        public IList<Tuple<PowertrainPosition, ElectricMotorData>> ElectricMachinesData { get; internal set; }

        /// <summary>
        /// Gets the data of the generator
        /// </summary>
        public Tuple<PowertrainPosition, ElectricMotorData> ElectricMachineDataGenerator =>
	        ElectricMachinesData.SingleOrDefault(em => em.Item1 == PowertrainPosition.GEN);
        
        /// <summary>
        /// Gets the data of the (propelling) electric machine
        /// </summary>
		public Tuple<PowertrainPosition, ElectricMotorData> ElectricMachineData =>
		    ElectricMachinesData.SingleOrDefault(em => em.Item1 != PowertrainPosition.GEN);

		public string ShiftStrategy { get; set; }

        public override bool Equals(object obj) => Equals(obj as AxlePowertrainData);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(AxlePowertrainData other)
		{
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return (Architecture == other.Architecture)
                && (GearboxData == other.GearboxData)
                && (AxleGearData == other.AxleGearData)
                && (AngledriveData == other.AngledriveData)
                && (Retarder == other.Retarder)
                && (PTO == other.PTO)
                && (GearshiftParameters == other.GearshiftParameters)
                && (ShiftStrategy == other.ShiftStrategy)
                && ElectricMachinesData.SequenceEqual(other.ElectricMachinesData);
        }

        public static bool operator ==(AxlePowertrainData axpt1, AxlePowertrainData axpt2)
		{
			if (ReferenceEquals(axpt1, axpt2))
			{
				return true;
			}

			if (ReferenceEquals(axpt1, null) || ReferenceEquals(axpt2, null))
			{
				return false;
			}

            return axpt1.Equals(axpt2);
        }

        public static bool operator !=(AxlePowertrainData axpt1, AxlePowertrainData axpt2) => !(axpt1 == axpt2);
    }
}
