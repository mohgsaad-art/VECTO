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
	public class RetarderViewModel : AbstractComponentViewModel, IRetarderViewModel
	{
		private readonly ObservableCollection<RetarderLossMapEntry> _lossMap =
			new ObservableCollection<RetarderLossMapEntry>();

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

		#region Implementation of IEditComponentRetarderViewModel

		public IRetarderInputData ModelData
		{
			get { return AdapterFactory.RetarderDeclarationAdapter(this); }
		}

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
				//return DeclarationData.RetarderCertificationMethods.Select(x => AllowedEntry.Create(x, x.GetLabel())).ToArray();
			}
		}

		public ObservableCollection<RetarderLossMapEntry> LossMap
		{
			get { return _lossMap; }
		}

		public RetarderType Type
		{
			get { return (ParentViewModel as IVehicleViewModel).RetarderType; }
			set { (ParentViewModel as IVehicleViewModel).RetarderType = value; }
		}

		public double Ratio
		{
			get { return (ParentViewModel as IVehicleViewModel).RetarderRatio; }
			set { (ParentViewModel as IVehicleViewModel).RetarderRatio = value; }
		}

		#endregion

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.RetarderInputData))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.RetarderInputData));
		}

		private void SetValues(IRetarderInputData retarder)
		{
			Model = retarder.Model;
			Manufacturer = retarder.Manufacturer;
			CertificationNumber = retarder.CertificationNumber;
			CertificationMethod = retarder.CertificationMethod;
			//ToDo
			//Date = DateTime.Parse(retarder.Date);
			//ToDo
			//var lossMap = RetarderLossMapReader.Create(retarder.LossMap).Entries.OrderBy(x => x.RetarderSpeed);
			//foreach (var entry in lossMap) {
			//	LossMap.Add(new RetarderLossMapEntry(entry));
			//}
		}
	}
}
