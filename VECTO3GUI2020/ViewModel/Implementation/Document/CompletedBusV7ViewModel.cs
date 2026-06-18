using System;
using System.IO;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.Implementation.Document
{
    internal class CompletedBusV7ViewModel : ViewModelBase, IJobViewModel, IMultistageVIFInputData
    {
		private bool _selected;
		private DataSource _dataSource;

		private IMultistageVIFInputData _inputData;
		private string _documentName;
		private readonly IMultiStageJobViewModel _multistageViewModel;

		#region Implementation of IDocumentViewModel

		public CompletedBusV7ViewModel(IMultistageVIFInputData multistep, IMultiStageViewModelFactory vmFactory)
		{
			_dataSource = multistep.DataSource;
			_inputData = multistep;
			if (_inputData == null) {
				throw new Exception("Invalid input file");
			}

			_documentName = Path.GetFileNameWithoutExtension(_inputData.DataSource.SourceFile);
			_multistageViewModel = vmFactory.GetMultiStageJobViewModel(multistep.MultistageJobInputData);

			_multistageViewModel.ManufacturingStageViewModel.LoadStageInputData(multistep.VehicleInputData.DataSource.SourceFile);
		}

		public string DocumentName => _documentName;

		public XmlDocumentType? DocumentType => throw new NotImplementedException();

		public string DocumentTypeName => "CompletedBus";

		public DataSource DataSource => _dataSource;

		public IEditViewModel EditViewModel => _multistageViewModel.EditViewModel;

		public bool Selected
		{
			get => _selected;
			set => SetProperty(ref _selected, value);
		}

		public bool CanBeSimulated
		{
			get => true;
			set => throw new NotImplementedException();
		}

		public IAdditionalJobInfoViewModel AdditionalJobInfoVm
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}







		#endregion

		#region Implementation of IMultistageVIFInputData

		public IVehicleDeclarationInputData VehicleInputData => _inputData.VehicleInputData;

		public IMultistepBusInputDataProvider MultistageJobInputData => _inputData.MultistageJobInputData;

		public bool SimulateResultingVIF => _inputData.SimulateResultingVIF;

        public string MonitoringData => _inputData.VehicleInputData.VehicleMonitoringData;

        #endregion
    }
}
