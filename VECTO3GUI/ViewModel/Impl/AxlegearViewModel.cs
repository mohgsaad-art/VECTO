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
	public class AxlegearViewModel : AbstractComponentViewModel, IAxlegearViewModel
	{
		private double _ratio;
		private AxleLineType _lineType;
		private readonly ObservableCollection<GearLossMapEntry> _lossMap = new ObservableCollection<GearLossMapEntry>();
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
		#region Implementation of IEditComponentAxlegearViewModel

		public IAxleGearInputData ModelData { get { return AdapterFactory.AxlegearDeclarationAdapter(this); } }

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
				//return DeclarationData.AxlegearCertificationMethods.Select(x => AllowedEntry.Create(x, x.GetLabel())).ToArray();
			}
		}

		public double Ratio
		{
			get { return _ratio; }
			set { SetProperty(ref _ratio, value); }
		}

		public AxleLineType LineType
		{
			get { return _lineType; }
			set { SetProperty(ref _lineType, value); }
		}

		public AllowedEntry<AxleLineType>[] AllowedLineTypes
		{
			get
			{
				//ToDo
				return null;
				//return Enum.GetValues(typeof(AxleLineType)).Cast<AxleLineType>().Select(x => AllowedEntry.Create(x, x.GetLabel()))
				//			.ToArray();
			}
		}

		public ObservableCollection<GearLossMapEntry> LossMap
		{
			get { return _lossMap; }
		}

		#endregion

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.AxleGearInputData))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.AxleGearInputData));
		}

		private void SetValues(IAxleGearInputData axle)
		{
			Model = axle.Model;
			Manufacturer = axle.Manufacturer;
			CertificationNumber = axle.CertificationNumber;
			CertificationMethod = axle.CertificationMethod;
			//ToDo
			//Date = DateTime.Parse(axle.Date);
			LineType = axle.LineType;
			Ratio = axle.Ratio;

			//ToDo
			//var lossMap = TransmissionLossMapReader.Create(axle.LossMap, axle.Ratio, "Axlegear").Entries.OrderBy(x => x.InputSpeed).ThenBy(x => x.InputTorque);
			//foreach (var entry in lossMap) {
			//	LossMap.Add(new GearLossMapEntry(entry));
			//}
		}
	}
}