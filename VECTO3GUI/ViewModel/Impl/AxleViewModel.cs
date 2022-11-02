using System;
using System.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Adapter;
using VECTO3GUI.ViewModel.Adapter.Declaration;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.ViewModel.Interfaces {
	public class AxleViewModel : AbstractViewModel, IAxleViewModel
	{
		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

		private AxleType _axleType;
		private bool _twinTyres;
		private bool _steered;
		private int _axleIndex;


		public IAxleDeclarationInputData ModelData
		{
			get { return AdapterFactory.AxleDeclarationAdapter(this); }
		}

		public int AxleIndex
		{
			get { return _axleIndex; }
			private set { SetProperty(ref _axleIndex, value);}
		}

		public AxleType AxleType
		{
			get { return _axleType; }
			set { SetProperty(ref _axleType, value); }
		}

		public bool TwinTyres
		{
			get { return _twinTyres; }
			set { SetProperty(ref _twinTyres, value); }
		}

		public bool Steered
		{
			get { return _steered; }
			set { SetProperty(ref _steered, value); }
		}

		public ITyreViewModel Tyre { get; private set; }

		public AllowedEntry<AxleType>[] AllowedAxleTypes
		{
			get
			{
				return null;
				//ToDo
				//var axletypes = DeclarationMode
				//	? DeclarationData.AllowedTruckAxleTypes
				//	: Enum.GetValues(typeof(AxleType)).Cast<AxleType>();
				//return axletypes.Select(a => AllowedEntry.Create(a, a.GetLabel())).ToArray();
			}
		}

		public void SetValues(int i, IAxleDeclarationInputData axle)
		{
			AxleIndex = i;
			AxleType = axle.AxleType;
			TwinTyres = axle.TwinTyres;
			//ToDo
			//Steered = axle.Steered;
			Tyre = Kernel.Get<ITyreViewModel>();
			Tyre.JobViewModel = JobViewModel;

			var tyre = Tyre as TyreViewModel;
			if (tyre == null) {
				throw new Exception("Unknown Tyre ViewModel");
			}

			tyre.SetValues(axle.Tyre);
		}

		internal void SetValues(int i)
		{
			AxleIndex = i;
			Tyre = Kernel.Get<ITyreViewModel>();
			Tyre.JobViewModel = JobViewModel;
		}
	}
}