using System;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces {
	public class Gear : ValidatingViewModel
	{
		private PerSecond _maxSpeed;
		private NewtonMeter _maxTorque;
		private double _ratio;
		private int _gearNumber;

		private ObservableCollection<Gear> _gears;
		
		public Gear(ObservableCollection<Gear> container)
		{
			_gears = container;
			LossMap = new ObservableCollection<GearLossMapEntry>();
		}

		public Gear(ObservableCollection<Gear> container, ITransmissionInputData gearData) : this(container)
		{
			GearNumber = gearData.Gear;
			Ratio = gearData.Ratio;
			MaxTorque = gearData.MaxTorque;
			MaxSpeed = gearData.MaxInputSpeed;
			LossMap.Clear();
			//ToDo
			//var lossMap = TransmissionLossMapReader.Create(gearData.LossMap, Ratio, $"Gear {GearNumber}")
			//										.Entries.OrderBy(x => x.InputSpeed).ThenBy(x => x.InputTorque);
			//foreach (var entry in lossMap) {
			//	LossMap.Add(new GearLossMapEntry(entry));
			//}
		}

		// not required - readonly property! [CustomValidation(typeof(Gear), "ValidateGearNumber")]
		public int GearNumber
		{
			get { return _gearNumber; }
			private set { SetProperty(ref _gearNumber, value); }
		}

		[VectoParameter(typeof(TransmissionData), "Ratio")]
		public double Ratio
		{
			get { return _ratio; }
			set { SetProperty(ref _ratio, value); }
		}

		[SIRange(0, Double.MaxValue)]
		public NewtonMeter MaxTorque
		{
			get { return _maxTorque; }
			set { SetProperty(ref _maxTorque, value); }
		}

		[VectoParameter(typeof(GearData), "MaxSpeed")]
		public PerSecond MaxSpeed
		{
			get { return _maxSpeed; }
			set { SetProperty(ref _maxSpeed, value); }
		}

		public ObservableCollection<GearLossMapEntry> LossMap { get; }

		//public static ValidationResult ValidateGearNumber(int gearNbr, ValidationContext context)
		//{
		//	var entry = context.ObjectInstance as Gear;
		//	if (entry == null) {
		//		return new ValidationResult("Unknown object instance");
		//	}

		//	var otherGears = entry._gears.Where(x => x != entry).ToArray();

		//	if (otherGears.Any(x =>  x.GearNumber == gearNbr)) {
		//		return new ValidationResult("Gear number has to be unique!", new [] {context.MemberName});
		//	}

		//	return ValidationResult.Success;
		//}
	}
}