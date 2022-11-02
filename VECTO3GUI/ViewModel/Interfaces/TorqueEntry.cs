using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.ViewModel.Impl;


namespace VECTO3GUI.ViewModel.Interfaces {

	
	public class TorqueEntry : ValidatingViewModel
	{
		private int _gear;
		private NewtonMeter _maxTorque;

		private ObservableCollection<TorqueEntry> _parent;

		public TorqueEntry(ObservableCollection<TorqueEntry> parent, int gear, NewtonMeter maxTorque)
		{
			_parent = parent;
			Gear = gear;
			MaxTorque = maxTorque;
		}

		public TorqueEntry(ObservableCollection<TorqueEntry> parent, ITorqueLimitInputData entry)
		{
			_parent = parent;
			Gear = entry.Gear;
			MaxTorque = entry.MaxTorque;
		}

		[CustomValidation(typeof(TorqueEntry), "ValidateUniqueGear"),
		 Range(1, 99)]
		public int Gear
		{
			get { return _gear; }
			set { SetProperty(ref _gear, value); }
		}

		[SIRange(double.Epsilon, double.MaxValue)]
		public NewtonMeter MaxTorque
		{
			get { return _maxTorque; }
			set { SetProperty(ref _maxTorque, value); }
		}

		public static ValidationResult ValidateUniqueGear(int gear, ValidationContext validationContext)
		{
			var entry = validationContext.ObjectInstance as TorqueEntry;
			if (entry == null) {
				return new ValidationResult("Unknown object instance");
			}
			if (entry._parent.Any(x => x != entry && x.Gear == gear)) {
				return new ValidationResult("Gear number has to be uniqe!", new []{ validationContext.MemberName});
			}
			return ValidationResult.Success;
		}
	}
}