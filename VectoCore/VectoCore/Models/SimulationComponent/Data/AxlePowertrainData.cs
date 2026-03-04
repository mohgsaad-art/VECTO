using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class AxlePowertrainData
	{
		public AxlePowertrainData()
		{
			
		}

		[Required, Range(-1, 4)]
		private int AxleNumberAsInt => AxleNumber;
		
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
		public Tuple<PowertrainPosition, ElectricMotorData> ElectricMachineData
		{
			get
			{
				return ElectricMachinesData?.SingleOrDefault(em => em.Item1 != PowertrainPosition.GEN);
			}
		}

		public string ShiftStrategy { get; set; }
	}
}
