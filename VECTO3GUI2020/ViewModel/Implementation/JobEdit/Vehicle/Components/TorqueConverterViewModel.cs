using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class TorqueConverterViewModel : ViewModelBase, ITorqueConverterViewModel, IComponentViewModel
    {
		private IComponentViewModelFactory _vmFactory;
		private ITorqueConverterDeclarationInputData _inputData;
		private ICommonComponentViewModel _commonComponentViewModel;
		private TableData _tcData;
		private bool _isPresent;


		public TorqueConverterViewModel(ITorqueConverterDeclarationInputData inputData, IComponentViewModelFactory vmFactory)
		{
			_commonComponentViewModel =
				vmFactory.CreateCommonComponentViewModel(inputData);
			_inputData = inputData;
			_vmFactory = vmFactory;
		}

		#region Implementation of ITorqueConverterDeclarationInputData
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

		public virtual TableData TCData
		{
			get => _tcData;
			set => SetProperty(ref _tcData, value);
		} 

		#endregion

		public string Name => "Torque Converter";

		public bool IsPresent{
			get => _isPresent;
			set => SetProperty(ref _isPresent, value);
		}
	}

	public class TorqueConverterViewModel_v1_0 : TorqueConverterViewModel
	{
		public static readonly string VERSION = typeof(XMLDeclarationTorqueConverterDataProviderV10).FullName;
		public TorqueConverterViewModel_v1_0(ITorqueConverterDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory) { }
	}

	public class TorqueConverterViewModel_v2_0 : TorqueConverterViewModel_v1_0
	{
		public static readonly new string VERSION = typeof(XMLDeclarationTorqueConverterDataProviderV20).FullName;

		public TorqueConverterViewModel_v2_0(ITorqueConverterDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory) { }
	}


}
