using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.Models;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{


    public abstract class GearboxViewModel : ViewModelBase, IComponentViewModel, IGearBoxViewModel
    {
        private static readonly string _name = "Gearbox";
        public string Name { get { return _name; } }

        protected IXMLGearboxDeclarationInputData _inputData;



        private bool _isPresent = true;
        public bool IsPresent { get { return _isPresent; } }

		protected ObservableCollection<IGearViewModel> _gearViewModels = new ObservableCollection<IGearViewModel>();

		public ObservableCollection<IGearViewModel> GearViewModels
		{
			get { return _gearViewModels; }
			set { SetProperty(ref _gearViewModels, value); }
		}

		protected ICommonComponentViewModel _commonComponentViewModel;


		public ICommonComponentViewModel CommonComponentViewModel
		{
			get => _commonComponentViewModel;
			set => SetProperty(ref _commonComponentViewModel, value);

		}

		public GearboxViewModel(IXMLGearboxDeclarationInputData inputData, IComponentViewModelFactory componentViewModelFactory)
        {
            _inputData = inputData as IXMLGearboxDeclarationInputData;
            Debug.Assert(_inputData != null);
            if (!(_isPresent = ((IComponentInputData)_inputData).DataSource != null))
            {
                return;
            }

            _commonComponentViewModel = componentViewModelFactory.CreateCommonComponentViewModel(_inputData);


			SetProperties();
			foreach (var gear in _inputData.Gears)
            {
				_gearViewModels.Add((IGearViewModel)componentViewModelFactory.CreateComponentViewModel(gear));
            }
        }

		public abstract void SetProperties();

        #region Implementation of IGearBoxDeclarationInputData
		protected GearboxType _type;
		protected IList<ITransmissionInputData> _gears;
		protected bool _differentialIncluded;
		protected double _axlegearRatio;
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

		public virtual GearboxType Type {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public virtual IList<ITransmissionInputData> Gears {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public virtual bool DifferentialIncluded {get => throw new NotImplementedException(); set => throw new NotImplementedException(); } 
		public virtual double AxlegearRatio {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion
    }

    public class GearboxViewModel_v1_0 : GearboxViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationGearboxDataProviderV10).FullName;

        public GearboxViewModel_v1_0(IXMLGearboxDeclarationInputData inputData,  IComponentViewModelFactory componentViewModelFactory) : base(inputData, componentViewModelFactory)
        {


        }

		public override void SetProperties()
		{
			throw new NotImplementedException();
		}
	}

    public class GearboxViewModel_v2_0 : GearboxViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationGearboxDataProviderV20).FullName;

		public GearboxViewModel_v2_0(IXMLGearboxDeclarationInputData inputData, IComponentViewModelFactory componentViewModelFactory) : base(inputData, componentViewModelFactory)
        {

        }

		public override void SetProperties()
		{
			_type = _inputData.Type;
			//_axlegearRatio = _inputData.AxlegearRatio;
			//_differentialIncluded = _inputData.DifferentialIncluded;
		}
		public override GearboxType Type
		{
			get => _type;
			set => SetProperty(ref _type, value);
		}

		public override IList<ITransmissionInputData> Gears
		{
			get => _gearViewModels.Cast<ITransmissionInputData>().ToList();
		}
	}
}
