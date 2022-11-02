using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Impl;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Adapter.Declaration {
	public class GearboxDeclarationAdapter : AbstractDeclarationAdapter, IGearboxDeclarationInputData
	{
		protected IGearboxViewModel ViewModel;
		public GearboxDeclarationAdapter(GearboxViewModel viewModel) : base(viewModel)
		{
			ViewModel = viewModel;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get { return ViewModel.CertificationMethod; } }
		
		#endregion

		#region Implementation of IGearboxDeclarationInputData

		public GearboxType Type { get { return ViewModel.TransmissionType; } }

		public IList<ITransmissionInputData> Gears
		{
			get
			{
				return null;
				//ToDo
				//return ViewModel.Gears.OrderBy(x => x.GearNumber).Select(
				//	g => new TransmissionInputData {
				//		Gear = g.GearNumber,
				//		Ratio = g.Ratio,
				//		MaxTorque = g.MaxTorque,
				//		MaxInputSpeed = g.MaxSpeed,
				//		LossMap = TableDataConverter.Convert(g.LossMap)
				//	}).Cast<ITransmissionInputData>().ToArray();
			}
		}

		public bool DifferentialIncluded { get; }
		public double AxlegearRatio { get; }

		public ITorqueConverterDeclarationInputData TorqueConverter
		{
			get { return (ViewModel.ParentViewModel as IVehicleViewModel)?.ModelData.Components.TorqueConverterInputData; }
		}

		#endregion
	}
}