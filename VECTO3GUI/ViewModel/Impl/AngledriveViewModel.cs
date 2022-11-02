using System;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Adapter.Declaration;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class AngledriveViewModel : AbstractComponentViewModel, IAngledriveViewModel
	{
		private CertificationMethod _certificationMethod;
		private readonly ObservableCollection<GearLossMapEntry> _lossMap = new ObservableCollection<GearLossMapEntry>();
		private double _ratio;

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


		#region Implementation of IAngledriveViewModel

		public IAngledriveInputData ModelData { get { return AdapterFactory.AngledriveDeclarationAdapter(this); } }

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
				//return DeclarationData.AngledriveCertificationMethods.Select(x => AllowedEntry.Create(x, x.GetLabel())).ToArray();
			}
		}

		public double Ratio
		{
			get { return _ratio; }
			set { SetProperty(ref _ratio, value); }
		}

		public ObservableCollection<GearLossMapEntry> LossMap
		{
			get { return _lossMap; }
		}

		public AngledriveType AngledriveType
		{
			get { return (ParentViewModel as IVehicleViewModel).AngledriveType; }
			set { (ParentViewModel as IVehicleViewModel).AngledriveType = value; }
		}

		#endregion

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.AngledriveInputData))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.AngledriveInputData));
		}

		private void SetValues(IAngledriveInputData angledrive)
		{
			Model = angledrive.Model;
			Manufacturer = angledrive.Manufacturer;
			CertificationNumber = angledrive.CertificationNumber;
			CertificationMethod = angledrive.CertificationMethod;
			//ToDo
			//Date = DateTime.Parse(angledrive.Date);
			Ratio = angledrive.Ratio;
			//ToDo
			//var lossMap = TransmissionLossMapReader.Create(angledrive.LossMap, angledrive.Ratio, "Angledirve").Entries.OrderBy(x => x.InputSpeed).ThenBy(x => x.InputTorque);
			//foreach (var entry in lossMap) {
			//	LossMap.Add(new GearLossMapEntry(entry));
			//}
		}
	}
}