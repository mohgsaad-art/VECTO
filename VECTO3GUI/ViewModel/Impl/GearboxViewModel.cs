using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Adapter.Declaration;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl {
	public class GearboxViewModel : AbstractComponentViewModel, IGearboxViewModel
	{
		
		private GearboxType _transmissionType;
		private readonly ObservableCollection<Gear> _gears = new ObservableCollection<Gear>();
		private Gear _selectedGear;
		private CertificationMethod _certificationMethod;
		private string _manufacturer;
		private string _model;
		private string _certificationNumber;
		private DateTime? _date;

		#region Implementation of ICommonComponentParameters

		public virtual string Manufacturer
		{
			get { return _manufacturer; }
			set { SetProperty(ref _manufacturer, value); }
		}

		public virtual string Model
		{
			get { return _model; }
			set { SetProperty(ref _model, value); }
		}

		public virtual string CertificationNumber
		{
			get { return _certificationNumber; }
			set { SetProperty(ref _certificationNumber, value); }
		}

		public virtual DateTime? Date
		{
			get { return _date; }
			set { SetProperty(ref _date, value); }
		}

		#endregion
		private void SetValues(IGearboxDeclarationInputData gbx)
		{
			Manufacturer = gbx.Manufacturer;
			Model = gbx.Model;
			CertificationNumber = gbx.CertificationNumber;
			CertificationMethod = gbx.CertificationMethod;
			//ToDo
			//Date = DateTime.Parse(gbx.Date);
			TransmissionType = gbx.Type;
			Gears.Clear();
			foreach (var entry in gbx.Gears.OrderBy(g => g.Gear)) {
				Gears.Add(new Gear(Gears, entry));
			}

			if (Gears.Count > 0) {
				SelectedGear = Gears.First();
			}
		}

		#region Implementation of IEditComponentGearboxViewModel

		public IGearboxDeclarationInputData ModelData { get { return AdapterFactory.GearboxDeclarationAdapter(this); } }

		public CertificationMethod CertificationMethod
		{
			get { return _certificationMethod; }
			set { SetProperty(ref _certificationMethod, value); }
		}

		public AllowedEntry<CertificationMethod>[] AllowedCertificationMethods
		{
			get
			{
				return null;
				//ToDo
				//return DeclarationData.GearboxCertificationMethods.Select(x => AllowedEntry.Create(x, x.GetLabel())).ToArray();
			}
		}

		public GearboxType TransmissionType
		{
			get { return _transmissionType; }
			set { SetProperty(ref _transmissionType, value); }
		}

		public AllowedEntry<GearboxType>[] AllowedTransmissionTypes
		{
			get
			{
				return null;
				//ToDo
				//var gears = DeclarationMode ? 
				//	DeclarationData.Gearbox.GearboxTypes()
				//	: Enum.GetValues(typeof(GearboxType)).Cast<GearboxType>();
				//return gears.Select(g => AllowedEntry.Create(g, g.GetLabel())).ToArray();
			}
		}

		[CustomValidation(typeof(GearboxViewModel), "ValidateGears")]
		public ObservableCollection<Gear> Gears
		{
			get { return _gears; }
		}

		public Gear SelectedGear
		{
			get { return _selectedGear; }
			set { SetProperty(ref _selectedGear, value); }
		}

		#endregion

		
		#region Overrides of AbstractEditComponentViewModel

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.GearboxInputData))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.GearboxInputData));
		}

		#endregion
	}
}