using System;
using System.Data;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class AxleGearViewModel : ViewModelBase, IComponentViewModel, IAxleGearViewModel
    {
        private readonly static string _name = "Axle Gear";
        public string Name { get { return _name; } }

        private bool _isPresent = true;
        public bool IsPresent { get { return _isPresent; } }

        protected IAxleGearInputData _inputData;
        protected ICommonComponentViewModel _commonComponentViewModel;


		public ICommonComponentViewModel CommonComponentViewModel { 
            get => _commonComponentViewModel; 
            set => SetProperty(ref _commonComponentViewModel, value); }

		protected abstract void SetProperties();

        public AxleGearViewModel(IXMLAxleGearInputData inputData, IComponentViewModelFactory vmFactory)
        {
            _inputData = inputData as IAxleGearInputData;
            Debug.Assert(_inputData != null);
            _isPresent = (_inputData?.DataSource != null);

            CommonComponentViewModel = vmFactory.CreateCommonComponentViewModel(_inputData);

			SetProperties();
		}

		#region implementation of IAxleGearInputData
		protected double _ratio;
		protected TableData _lossMap;
		protected double _efficiency;
		protected AxleLineType _lineType;
		public DataSource DataSource
		{
			get => _commonComponentViewModel.DataSource;
			set => _commonComponentViewModel.DataSource = value;
		}

		public string Manufacturer
		{
			get => _commonComponentViewModel.Manufacturer;
			set => _commonComponentViewModel.Manufacturer = value;
		}

		public string Model
		{
			get => _commonComponentViewModel.Model;
			set => _commonComponentViewModel.Model = value;
		}

		public DateTime Date
		{
			get => _commonComponentViewModel.Date;
			set => _commonComponentViewModel.Date = value;
		}

		public string CertificationNumber
		{
			get => _commonComponentViewModel.CertificationNumber;
			set => _commonComponentViewModel.CertificationNumber = value;
		}

		public CertificationMethod CertificationMethod
		{
			get => _commonComponentViewModel.CertificationMethod;
			set => _commonComponentViewModel.CertificationMethod = value;
		}

		public bool SavedInDeclarationMode
		{
			get => _commonComponentViewModel.SavedInDeclarationMode;
			set => _commonComponentViewModel.SavedInDeclarationMode = value;
		}

		public DigestData DigestValue
		{
			get => _commonComponentViewModel.DigestValue;
			set => _commonComponentViewModel.DigestValue = value;
		}

		public string AppVersion => _commonComponentViewModel.AppVersion;

		public virtual double Ratio {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual TableData LossMap {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual double Efficiency {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual AxleLineType LineType {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		#endregion
    }



    public class AxleGearViewModel_v1_0 : AxleGearViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationAxlegearDataProviderV10).FullName;
        public AxleGearViewModel_v1_0(IXMLAxleGearInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
		{
		}


		protected override void SetProperties()
		{
			throw new NotImplementedException();
		}
	}

    public class AxleGearViewModel_v2_0 : AxleGearViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationAxlegearDataProviderV20).FullName;
		


		public override double Ratio
		{
			get => _ratio;
			set => SetProperty(ref _ratio, value);
		}

		public override TableData LossMap
		{
			get => _lossMap;
			set => SetProperty(ref _lossMap, value);
		}

		public DataTable LossMapDT
		{
			get => (DataTable)_lossMap;
		}

		public override AxleLineType LineType
		{
			get => _lineType;
			set => SetProperty(ref _lineType, value);
		}

		public override double Efficiency
		{
			get => _efficiency;
			set => SetProperty(ref _efficiency, value);
		}

		public AxleGearViewModel_v2_0(IXMLAxleGearInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }

		protected override void SetProperties()
		{
			_ratio = _inputData.Ratio;
			_lineType = _inputData.LineType;
			_lossMap = _inputData.LossMap;

			_lossMap.TableName = "Loss Map";
			// not supported in Declaration mode_efficiency = _inputData.Efficiency;
		}

        #region override Properties







		#endregion

    }
}

