using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Util;


namespace VECTO3GUI.ViewModel.Adapter.Declaration {
	public class EngineDeclarationAdapter : AbstractDeclarationAdapter, IEngineDeclarationInputData
	{
		private IEngineViewModel ViewModel;

		public EngineDeclarationAdapter(EngineViewModel viewModel) : base(viewModel)
		{
			ViewModel = viewModel;
		}

		#region Implementation of IComponentInputData
		
		public DataSource DataSource { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get { return CertificationMethod.Measured; } }
		
		#endregion

		#region Implementation of IEngineDeclarationInputData

		public CubicMeter Displacement { get { return ViewModel.Displacement; } }
		public PerSecond IdleSpeed { get { return ViewModel.IdlingSpeed; } }
		public double WHTCMotorway { get { return ViewModel.WHTCMotorway; } }
		public double WHTCRural { get { return ViewModel.WHTCRural; } }
		public double WHTCUrban { get { return ViewModel.WHTCUrban; } }
		public double ColdHotBalancingFactor { get { return ViewModel.BFColdHot; } }
		public double CorrectionFactorRegPer { get { return ViewModel.CFRegPer; } }
		public double CorrectionFactorNCV { get { return ViewModel.CFNCV; } }
		public FuelType FuelType { get { return ViewModel.FuelType; } }
		public TableData FuelConsumptionMap { get { return TableDataConverter.Convert(ViewModel.FCMap); } }
		public TableData FullLoadCurve { get { return TableDataConverter.Convert(ViewModel.FullLoadCurve); } }
		public Watt RatedPowerDeclared { get { return ViewModel.RatedPower; } }
		public PerSecond RatedSpeedDeclared { get { return ViewModel.RatedSpeed; } }
		public NewtonMeter MaxTorqueDeclared { get { return ViewModel.MaxEngineTorque; } }
		public IList<IEngineModeDeclarationInputData> EngineModes { get; }
		public WHRType WHRType { get; }

		#endregion
	}
}