using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class EngineModeViewModel : ViewModelBase, IEngineModeViewModel
    {
		protected IEngineModeDeclarationInputData _inputData;
		protected IComponentViewModelFactory _componentVmFactory;

		public EngineModeViewModel(IEngineModeDeclarationInputData inputData, IComponentViewModelFactory componentVmFactory)
		{
			_inputData = inputData;
			_componentVmFactory = componentVmFactory;
		}

		#region implementation of IEngineModeDeclaration 
		public virtual PerSecond IdleSpeed
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); } 
		}

		public virtual TableData FullLoadCurve
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public virtual IList<IEngineFuelDeclarationInputData> Fuels
		{
			get { throw new NotImplementedException(); }
		}

		public virtual IWHRData WasteHeatRecoveryDataElectrical
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public virtual IWHRData WasteHeatRecoveryDataMechanical
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		#endregion
	}


	public class EngineModeViewModelSingleFuel : EngineModeViewModel
	{
		public static readonly string VERSION =
			typeof(XMLDeclarationEngineDataProviderV10.XMLSingleFuelEngineMode).FullName;

		private PerSecond _idleSpeed;
		private TableData _fullLoadCurve;
		private IWHRData _wasteHeatRecoveryDataElectrical;
		private IWHRData _wasteHeatRecoveryDataMechanical;
		private ObservableCollection<IEngineFuelViewModel> _fuelViewModels;

		public EngineModeViewModelSingleFuel(IEngineModeDeclarationInputData inputData, IComponentViewModelFactory componentVmFactory) : base(inputData,
			componentVmFactory)
		{
			
			_fuelViewModels = new ObservableCollection<IEngineFuelViewModel>();
			_idleSpeed = inputData.IdleSpeed;
			_fullLoadCurve = inputData.FullLoadCurve;
			_wasteHeatRecoveryDataElectrical = inputData.WasteHeatRecoveryDataElectrical;
			_wasteHeatRecoveryDataMechanical = inputData.WasteHeatRecoveryDataMechanical;

			_fullLoadCurve.TableName = "Full Load Curve";


			foreach (var fuel in inputData.Fuels) {
				FuelViewModels.Add(_componentVmFactory.CreateEngineFuelViewModel(fuel));
			}
		}

		public ObservableCollection<IEngineFuelViewModel> FuelViewModels { get => _fuelViewModels; set => SetProperty(ref _fuelViewModels, value); }

		public IEngineFuelViewModel FuelViewModel
		{
			get => FuelViewModels[0];
		}
		public override PerSecond IdleSpeed
		{
			get => _idleSpeed;
			set => SetProperty(ref _idleSpeed, value);
		}

		public override TableData FullLoadCurve
		{
			get => _fullLoadCurve;
			set => SetProperty(ref _fullLoadCurve, value);
		}

		public DataTable FullLoadDT
		{
			get => (DataTable)_fullLoadCurve;
			//get => _fullLoadCurve.AsDataView().ToTable();
		}

		public override IList<IEngineFuelDeclarationInputData> Fuels => 
			FuelViewModels.Cast<IEngineFuelDeclarationInputData>().ToList();

		public override IWHRData WasteHeatRecoveryDataElectrical
		{
			get { return _wasteHeatRecoveryDataElectrical;}
			set => SetProperty(ref _wasteHeatRecoveryDataElectrical, value);
		}

		public override IWHRData WasteHeatRecoveryDataMechanical
		{
			get => _wasteHeatRecoveryDataMechanical;
			set => SetProperty(ref _wasteHeatRecoveryDataMechanical, value);
		}
	}
}
