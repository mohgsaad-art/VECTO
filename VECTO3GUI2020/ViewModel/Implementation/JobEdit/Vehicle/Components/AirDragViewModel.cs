using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class AirDragViewModel : ViewModelBase, IComponentViewModel, IAirDragViewModel
    {
        private static readonly string _name = "AirDrag";

        protected IAirdragDeclarationInputData _inputData;

		public XmlNode XMLSource { get; }

		private ICommonComponentViewModel _commonComponentViewModel;
		public ICommonComponentViewModel CommonComponentViewModel
		{
			get => _commonComponentViewModel;
			set => SetProperty(ref _commonComponentViewModel, value);
		}








        public AirDragViewModel(IXMLAirdragDeclarationInputData inputData, IComponentViewModelFactory vmFactory)
		{
			LabelVisible = true;
			IsReadOnly = false;
			_inputData = inputData as IAirdragDeclarationInputData;
            Debug.Assert(_inputData != null);
            _isPresent = (_inputData?.DataSource?.SourceFile != null);

			XMLSource = (inputData as AbstractCommonComponentType)?.XMLSource;

            _commonComponentViewModel = vmFactory.CreateCommonComponentViewModel(_inputData);
			SetProperties();
		}

		public abstract void SetProperties();

        public string Name { get { return _name; } }
		private bool _isPresent;
		public bool IsPresent { get { return _isPresent; } }

		#region Implementation of IAirDragDeclarationInputData
		
		protected SquareMeter _airDragArea;
		protected SquareMeter _transferredAirDragArea;
		protected SquareMeter _airDragArea_0;
		private bool _isReadOnly;
		private bool _labelVisible;

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


		public virtual SquareMeter AirDragArea
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual SquareMeter TransferredAirDragArea { 
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
		public virtual SquareMeter AirDragArea_0 { 
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		#endregion

		public bool LabelVisible
		{
			get => _labelVisible;
			set => SetProperty(ref _labelVisible, value);
		}

		public bool IsReadOnly
		{
			get => _isReadOnly;
			set => SetProperty(ref _isReadOnly, value);
		}
	}


    public class AirDragViewModel_v1_0 : AirDragViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationAirdragDataProviderV10).FullName;
        public AirDragViewModel_v1_0(IXMLAirdragDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }

		public override void SetProperties()
		{
			throw new NotImplementedException();
		}
	}

    public class AirDragViewModel_v2_0 : AirDragViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationAirdragDataProviderV20).FullName;

		public AirDragViewModel_v2_0(IXMLAirdragDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }

		public override void SetProperties()
		{
			_airDragArea = _inputData.AirDragArea;
			_airDragArea_0 = _inputData.AirDragArea_0;
			_transferredAirDragArea = _inputData.TransferredAirDragArea;
		}

		public override SquareMeter AirDragArea
		{
			get => _airDragArea;
			set => SetProperty(ref _airDragArea, value);
		}

		public override SquareMeter TransferredAirDragArea { 
			get => _transferredAirDragArea;
			set => SetProperty(ref _transferredAirDragArea, value);
		}

		public override SquareMeter AirDragArea_0
		{
			get => _airDragArea_0;
			set => SetProperty(ref _airDragArea_0, value);
		}
	}

	public class AirDragViewModel_v2_4 : AirDragViewModel_v2_0
	{
		public new static readonly string VERSION = typeof(XMLDeclarationAirdragDataProviderV24).FullName;

		public AirDragViewModel_v2_4(IXMLAirdragDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
		{
			LabelVisible = false;
			IsReadOnly = true;
		}

		public override void SetProperties()
		{
			base.SetProperties();

		}
	}
}
