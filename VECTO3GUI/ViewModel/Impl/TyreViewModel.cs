using System;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class TyreViewModel : AbstractComponentViewModel, ITyreViewModel
	{
		private string _dimension;
		private double _rollingResistanceCoefficient;
		private Newton _fzIso;
		private string _manufacturer;
		private string _model;
		private string _certificationNumber;
		private DateTime? _date;

		#region Implementation of ICommonComponentParameters

		public virtual string Manufacturer
		{
			get { return _manufacturer; }
			set { SetProperty(ref _manufacturer, value); }
		}

		public virtual string Model
		{
			get { return _model; }
			set { SetProperty(ref _model, value); }
		}

		public virtual string CertificationNumber
		{
			get { return _certificationNumber; }
			set { SetProperty(ref _certificationNumber, value); }
		}

		public virtual DateTime? Date
		{
			get { return _date; }
			set { SetProperty(ref _date, value); }
		}

		#endregion


		#region Implementation of ITyreViewModel

		public ITyreDeclarationInputData ModelData { get { return AdapterFactory.TyreDeclarationAdapter(this); } }

		public string Dimension
		{
			get { return _dimension; }
			set { SetProperty(ref _dimension, value); }
		}

		public AllowedEntry<string>[] AllowedDimensions
		{
			get { return DeclarationData.Wheels.GetWheelsDimensions().Select(w => AllowedEntry.Create(w, w)).ToArray(); }
		}

		public double RollingResistanceCoefficient
		{
			get { return _rollingResistanceCoefficient; }
			set { SetProperty(ref _rollingResistanceCoefficient, value); }
		}

		public Newton FzISO
		{
			get { return _fzIso; }
			set { SetProperty(ref _fzIso, value); }
		}

		#endregion

		public void SetValues(ITyreDeclarationInputData tyre)
		{
			Manufacturer = tyre.Manufacturer;
			Model = tyre.Model;
			CertificationNumber = tyre.CertificationNumber;
			//ToDo
			//Date = DateTime.Parse(tyre.Date ?? "1/1/1970");

			Dimension = tyre.Dimension;
			RollingResistanceCoefficient = tyre.RollResistanceCoefficient;
			FzISO = tyre.TyreTestLoad;
		}
	}
}
