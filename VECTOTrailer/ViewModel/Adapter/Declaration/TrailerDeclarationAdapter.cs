using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Impl;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Adapter.Declaration
{
	public class TrailerDeclarationAdapter : ITrailerDeclarationInputData
	{
		protected ICompleteTrailerViewModel ViewModel;
		private DateTime _date;

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

		public TrailerDeclarationAdapter(ICompleteTrailerViewModel trailerViewModel)
		{
			ViewModel = trailerViewModel;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }

		public string Manufacturer
		{
			get { return ViewModel.Manufacturer; }
		}

		public string Model
		{
			get { return ViewModel.Model; }
		}

		DateTime IComponentInputData.Date => _date;

		public string AppVersion { get; }

		public string Date
		{
			get { return ViewModel.Date.ToString(); }
		}

		public CertificationMethod CertificationMethod
		{
			get { throw new NotImplementedException(); }
		}

		public string CertificationNumber
		{
			get { throw new NotImplementedException(); }
		}

		public DigestData DigestValue
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region Implementation of ITrailerDeclarationInputData

		public string Identifier { get; }
		public AxleConfiguration AxleConfiguration { get; }
		public bool ExemptedVehicle { get; }
		public IList<IAxleDeclarationInputData> Axles { get; }
		public IVehicleComponentsDeclaration Components { get; }
		public string ManufacturerAddress { get { return ViewModel.ManufacturerAddress; } }
		public string VIN { get; }
		public string LegislativeCategory { get; }
		public NumberOfTrailerAxles NumberOfAxles { get; }
		public TypeTrailer TrailerType { get; }
		public BodyWorkCode BodyworkCode { get; }
		public Kilogram CurbMass { get; }
		public Kilogram TPMLMTotalTrailer { get; }
		public Kilogram TPMLMAxleAssembly { get; }
		public Meter ExternalBodyLength { get; }
		public Meter ExternalBodyWidth { get; }
		public Meter ExternalBodyHeight { get; }
		public Meter TotalTrailerHeight { get; }
		public Meter LengthTrailerFrontToFirstAxleCenter { get; }
		public Meter LengthTrailerFrontToLastAxleCenter { get; }
		public Meter InternalBodyHeight { get; }
		public CubicMeter CargoVolume { get; }
		public IList<AeroFeatureTechnology> AeroFeatureTechnologies { get; }

		public XmlNode XMLSource { get; }

		#endregion

		private T GetComponentViewModel<T>(Component component) where T : class
		{
			var vm = ViewModel.GetComponentViewModel(component);
			var cvm = vm as T;
			return cvm;
		}

	}

}
