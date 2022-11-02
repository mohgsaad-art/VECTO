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

namespace VECTO3GUI.ViewModel.Impl {
	public class TorqueConverterViewModel : AbstractComponentViewModel, ITorqueConverterViewModel
	{
		private readonly ObservableCollection<TorqueConverterCharacteristics> _characteristics = new ObservableCollection<TorqueConverterCharacteristics>();
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
		#region Implementation of IEditComponentTorqueConverterViewModel

		public ITorqueConverterDeclarationInputData ModelData { get { return AdapterFactory.TorqueConverterDeclarationAdapter(this); } }

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
				//return DeclarationData.TorqueConverterCertificationMethods.Select(x => AllowedEntry.Create(x, x.GetLabel())).ToArray();
			}
		}

		public ObservableCollection<TorqueConverterCharacteristics> Characteristics
		{
			get { return _characteristics; }
		}

		#endregion

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.TorqueConverterInputData))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.TorqueConverterInputData));
		}

		private void SetValues(ITorqueConverterDeclarationInputData tc)
		{
			Model = tc.Model;
			Manufacturer = tc.Manufacturer;
			CertificationNumber = tc.CertificationNumber;
			CertificationMethod = tc.CertificationMethod;
			//ToDo
			//Date = DateTime.Parse(tc.Date);

			var ratio = 1; // TODO!

			//ToDo
			//var lossMap = TorqueConverterDataReader.Create(tc.TCData,
			//												DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
			//												ExecutionMode.Declaration, ratio,
			//												DeclarationData.TorqueConverter.CLUpshiftMinAcceleration, DeclarationData.TorqueConverter.CCUpshiftMinAcceleration).Entries.OrderBy(x => x.SpeedRatio);
			//foreach (var entry in lossMap) {
			//	Characteristics.Add(new TorqueConverterCharacteristics(entry));
			//}
		}
	}
}