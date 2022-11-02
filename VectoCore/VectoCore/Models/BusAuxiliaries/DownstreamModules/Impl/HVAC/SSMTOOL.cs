using System.Linq;
using System.Text;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used by frmHVACTool
	// Replaces Spreadsheet model which does the same calculation
	// Version of which appears on the form title.
	public class SimpleSSMTool : ISSMPowerDemand
	{
		private ISSMEngineeringInputs _ssmInput;

		public SimpleSSMTool(ISSMInputs ssmInput)
		{
			if (!(ssmInput is ISSMEngineeringInputs)) {
				throw new VectoException("SSM Inputs are not in engineering mode!");
			}

			_ssmInput = ssmInput as ISSMEngineeringInputs;
		}

		#region Implementation of ISSMPowerDemand

		public Watt ElectricalWAdjusted => _ssmInput.ElectricPower;

		public Watt MechanicalWBaseAdjusted => _ssmInput.MechanicalPower;

		public Watt AverageAuxHeaterPower(Watt averageUseableEngineWasteHeat)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}


	public class SSMTOOL : ISSMTOOL
	{
		

		// Constructors
		public SSMTOOL(ISSMInputs ssmInput)
		{
			SSMInputs = ssmInput as ISSMDeclarationInputs;
			if (SSMInputs == null) {
				throw new VectoException("SSM Inputs are not in declaration mode!");
			}

			TechList = SSMInputs.Technologies;

			Calculate = new SSMCalculate(this);
			EngineWasteHeat = 0.SI<Watt>();
		}

		public ISSMDeclarationInputs SSMInputs { get;  }

		public ISSMBoundaryConditions BoundaryConditions { get; set; }

		public ISSMTechnologyBenefits TechList { get; set; }
		public ISSMCalculate Calculate { get; set; }
		public IHVACConstants HVACConstants { get; set; }

		// Repeat Warning Flags
		private bool CompressorCapacityInsufficientWarned;

		// Base Values
		public Watt ElectricalWBase => Calculate.ElectricalWBase; // .SI(Of Watt)()

		public Watt MechanicalWBase => Calculate.MechanicalWBase; // .SI(Of Watt)()

		//public KilogramPerSecond FuelPerHBase
		//{
		//	get {
		//		return Calculate.FuelPerHBase; // .SI(Of LiterPerHour)()
		//	}
		//}

		// Adjusted Values
		public Watt ElectricalWAdjusted => Calculate.ElectricalWAdjusted; // .SI(Of Watt)()

		public Watt MechanicalWBaseAdjusted
		{
			get {
				var mechAdjusted = Calculate.MechanicalWBaseAdjusted;

				if (CompressorCapacityInsufficientWarned == false && (mechAdjusted) / (1000 * SSMInputs.ACSystem.COP) > SSMInputs.ACSystem.HVACMaxCoolingPower) {
					LoggingObject.Logger<SSMTOOL>().Warn("HVAC SSM :AC-Compressor Capacity unable to service cooling, run continues as if capacity was sufficient.");
					CompressorCapacityInsufficientWarned = true;
				}


				return mechAdjusted; // .SI(Of Watt)()
			}
		}

		//public KilogramPerSecond FuelPerHBaseAdjusted
		//{
		//	get {
		//		return Calculate.AverageAuxHeaterPower; // .SI(Of LiterPerHour)()
		//	}
		//}

		public Watt EngineWasteHeat { get; protected set; }
		
		
		
		// Dynamicly Get Fuel having re-adjusted Engine Heat Waste, this was originally supposed to be Solid State. Late adjustment request 24/3/2015
		public Watt AverageAuxHeaterPower(Watt averageUseableEngineWasteHeat)
		{
			// Set Engine Waste Heat
			//SSMInputs.AuxHeater.EngineWasteHeatkW = AverageUseableEngineWasteHeatKW;
			EngineWasteHeat = averageUseableEngineWasteHeat;

			var fba = Calculate.AverageAuxHeaterPower;

			// Dim FuelFiredWarning As Boolean = fba * SSMInputs.BC_AuxHeaterEfficiency * HVACConstants.FuelDensity * SSMInputs.BC_GCVDieselOrHeatingOil * 1000 > (AverageUseableEngineWasteHeatKW + SSMInputs.AH_FuelFiredHeaterkW)
			// If Not FuelFiredHeaterInsufficientWarned AndAlso FuelFiredWarning Then
			// FuelFiredHeaterInsufficientWarned = True
			// OnMessage(Me, " HVAC SSM : Fuel fired heater insufficient for heating requirements, run continues assuming it was sufficient.", AdvancedAuxiliaryMessageType.Warning)
			// End If

			return fba;
		}

	}
}
