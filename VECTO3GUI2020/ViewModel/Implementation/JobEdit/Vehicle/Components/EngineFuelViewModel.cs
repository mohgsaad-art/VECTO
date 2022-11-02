using System;
using System.Collections.Generic;
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
    public class EngineFuelViewModel : ViewModelBase, IEngineFuelViewModel
    {
		protected IEngineFuelDeclarationInputData _inputData;

		public EngineFuelViewModel(IEngineFuelDeclarationInputData inputData)
		{
			_inputData = inputData; 
			_fuelType = inputData.FuelType;
			_whtcMotorway = inputData.WHTCMotorway;
			_whtcUrban = inputData.WHTCUrban;
			_whtcRural = inputData.WHTCRural;
			_coldHotBalancingFactor = inputData.ColdHotBalancingFactor;
			_correctionFactorRegPer = inputData.CorrectionFactorRegPer;
			_fuelConsumptionMap = inputData.FuelConsumptionMap;
			_fuelConsumptionMap.TableName = "Fuel Consumption Map";
		}
		#region implementation of IEngineFuelDeclarationInputData

		private FuelType _fuelType;
		private double _whtcMotorway;
		private double _whtcRural;
		private double _whtcUrban;
		private double _coldHotBalancingFactor;
		private double _correctionFactorRegPer;
		private TableData _fuelConsumptionMap;


		public FuelType FuelType
		{
			get => _fuelType;
			set => _fuelType = value;
		}

		public double WHTCMotorway
		{
			get => _whtcMotorway;
			set => _whtcMotorway = value;
		}

		public double WHTCRural
		{
			get => _whtcRural;
			set => _whtcRural = value;
		}

		public double WHTCUrban
		{
			get => _whtcUrban;
			set => _whtcUrban = value;
		}

		public double ColdHotBalancingFactor
		{
			get => _coldHotBalancingFactor;
			set => _coldHotBalancingFactor = value;
		}

		public double CorrectionFactorRegPer
		{
			get => _correctionFactorRegPer;
			set => _correctionFactorRegPer = value;
		}

		public TableData FuelConsumptionMap
		{
			get => _fuelConsumptionMap;
			set => _fuelConsumptionMap = value;
		}
	}
	#endregion
}
