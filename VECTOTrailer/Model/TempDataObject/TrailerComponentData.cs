using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.ViewModel.Impl;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.Model.TempDataObject
{
	public class TrailerComponentData : ICompleteTrailer, ITempDataObject<ICompleteTrailerViewModel>
	{
		protected const int NotSelected = -1;

		#region ICompleteVehicleTrailer Interface

		public string Manufacturer { get; set; }
		public string ManufacturerAddress { get; set; }
		public string TrailerModel { get; set; }
		public string VIN { get; set; }
		public DateTime Date { get; set; }
		public string LegislativeCategory { get; set; }
		public Meter ExternalLengthOfTheBody { get; set; }
		public Meter ExternalWidthOfTheBody { get; set; }
		public Meter ExternalHeightOfTheBody { get; set; }
		public Meter TotalHeightOfTheTrailer { get; set; }
		public Meter LengthFromFrontToFirstAxle { get; set; }
		public Meter LengthBetweenCentersOfAxles { get; set; }
		public Meter InternalHeightOfTheBody { get; set; }
		public CubicMeter CargoVolume { get; set; }
		public NumberOfTrailerAxles NumberOfAxles { get; set; }
		public TypeTrailer TrailerType { get; set; }
		public BodyWorkCode BodyCode { get; set; }
		public IList<AeroFeatureTechnology> Technologies { get; set; }

		public bool ShowLengthBetweenCentersOfAxles { get; set; }
		public bool IsNone { get; set; }
		public bool IsStandard { get; set; }
		public bool IsCertified { get; set; }

		public bool SideSkirtsShort { get; set; }
		public bool SideSkirtsLong { get; set; }
		public bool BoatTailShort { get; set; }
		public bool BoatTailLong { get; set; }
		public double YawAngle0 { get; set; }
		public double YawAngle3 { get; set; }
		public double YawAngle6 { get; set; }
		public double YawAngle9 { get; set; }
		public Kilogram MassInRunningOrder { get; set; }
		public Kilogram TPLMTotalTrailer { get; set; }
		public Kilogram TPLMAxleAssembly { get; set; }
		public bool VolumeOrientation { get; set; }

		public ICollection<TrailerAxleViewModel> Axles { get; set; } =
			new List<TrailerAxleViewModel>();

		public string CertifiedAeroDeviceFilePath { get; set; }

		public string CertifiedAeroDeviceXml { get; set; }
		public ICommand SelectAxleXmlCommand { get; set;  }

		public ICommand SelectCertifiedAeroDeviceCommand { get; set; }

		public JobFileType CertifiedAeroDeviceType { get; set; }
		public ITrailerDeclarationInputData Trailer { get; set; }


		public bool CertifiedAeroDeviceButtonsVisibility { get; set; }
		public bool CertifiedAeroDeviceNewVisibility { get; set; }
		public bool TPMLMAxleAssemblyVisibility { get; set; }
		public bool TrailerCouplingPointVisibility { get; set; }
		public TrailerCouplingPoint TrailerCouplingPoint { get; set; }

		public XDocument SourceDocument { get; }

		public Dictionary<string, string> XmlNamesToPropertyMapping { get;  private set; }

		#endregion

		public TrailerComponentData(ICompleteTrailerViewModel viewModel, bool defaultValues)
		{
			if(defaultValues)
				ClearValues(viewModel);
			SetXmlNamesToPropertyMapping();
		}

		public TrailerComponentData(ICompleteTrailerViewModel viewModel)
		{
			SetCurrentValues(viewModel);
			SetXmlNamesToPropertyMapping();
		}

		public void UpdateCurrentValues(ICompleteTrailerViewModel viewModel)
		{
			SetCurrentValues(viewModel);
		}

		public void ResetToComponentValues(ICompleteTrailerViewModel viewModel)
		{
			viewModel.Manufacturer = Manufacturer;
			viewModel.ManufacturerAddress = ManufacturerAddress;
			viewModel.TrailerModel = TrailerModel;
			viewModel.VIN = VIN;
			viewModel.Date = Date;
			viewModel.LegislativeCategory = LegislativeCategory;
			viewModel.ExternalHeightOfTheBody = ExternalHeightOfTheBody;
			viewModel.ExternalLengthOfTheBody = ExternalLengthOfTheBody;
			viewModel.ExternalWidthOfTheBody = ExternalWidthOfTheBody;
			viewModel.TotalHeightOfTheTrailer = TotalHeightOfTheTrailer;
			viewModel.LengthFromFrontToFirstAxle = LengthFromFrontToFirstAxle;
			viewModel.CargoVolume = CargoVolume;
			viewModel.NumberOfAxles = NumberOfAxles;
			viewModel.TrailerType = TrailerType;
			viewModel.BodyCode = BodyCode;
			viewModel.MassInRunningOrder = MassInRunningOrder;
			viewModel.TPLMTotalTrailer = TPLMTotalTrailer;
			viewModel.TPLMAxleAssembly = TPLMAxleAssembly;
			viewModel.VolumeOrientation = VolumeOrientation;
			for (int i = 0; i < viewModel.Axles.Count; i++) {
				if (i >= Axles.Count) {
					break;
				}
				viewModel.Axles[i] = Axles.ElementAt(i);
			}


			viewModel.CertifiedAeroDeviceXml = CertifiedAeroDeviceXml;

			viewModel.IsStandard = IsStandard;
			viewModel.IsCertified = IsCertified;

			viewModel.ShowLengthBetweenCentersOfAxles = NumberOfAxles != NumberOfTrailerAxles.One;
			viewModel.LengthBetweenCentersOfAxles = viewModel.ShowLengthBetweenCentersOfAxles ? viewModel.LengthBetweenCentersOfAxles : 0.SI<Meter>();

			viewModel.Technologies = Technologies;
			viewModel.SideSkirtsShort = Technologies != null && Technologies.Contains(AeroFeatureTechnology.SideSkirtsShort);
			viewModel.SideSkirtsLong = Technologies != null && Technologies.Contains(AeroFeatureTechnology.SideSkirtsLong);
			viewModel.BoatTailShort = Technologies != null && Technologies.Contains(AeroFeatureTechnology.BoatTailShort);
			viewModel.BoatTailLong = Technologies != null && Technologies.Contains(AeroFeatureTechnology.BoatTailLong);

			viewModel.IsNone = IsNone;

			viewModel.YawAngle0 = YawAngle0;
			viewModel.YawAngle3 = YawAngle3;
			viewModel.YawAngle6 = YawAngle6;
			viewModel.YawAngle9 = YawAngle9;


			viewModel.CertifiedAeroDeviceButtonsVisibility = CertifiedAeroDeviceButtonsVisibility;
			viewModel.CertifiedAeroDeviceNewVisibility = CertifiedAeroDeviceNewVisibility;

			viewModel.TPMLMAxleAssemblyVisibility = TPMLMAxleAssemblyVisibility;
			viewModel.TrailerCouplingPointVisibility = TrailerCouplingPointVisibility;

			viewModel.TrailerCouplingPoint = viewModel.TrailerCouplingPoint;
		}

		public void ClearValues(ICompleteTrailerViewModel viewModel)
		{
			viewModel.Manufacturer = default;
			viewModel.ManufacturerAddress = default;
			viewModel.TrailerModel = default;
			viewModel.VIN = default;
			viewModel.Date = DateTime.Now;
			viewModel.LegislativeCategory = default;
			viewModel.ExternalHeightOfTheBody = default;
			viewModel.ExternalLengthOfTheBody = default;
			viewModel.ExternalWidthOfTheBody = default;
			viewModel.TotalHeightOfTheTrailer = default;
			viewModel.LengthFromFrontToFirstAxle = default;
			viewModel.LengthBetweenCentersOfAxles = null;
			viewModel.CargoVolume = default;
			viewModel.NumberOfAxles = NumberOfTrailerAxles.One;
			viewModel.TrailerType = TypeTrailer.DA;
			viewModel.BodyCode = BodyWorkCode.Drybody;
			viewModel.TrailerCouplingPoint = TrailerCouplingPoint.Unknown;
			viewModel.SideSkirtsShort = false;
			viewModel.SideSkirtsLong = false;
			viewModel.BoatTailShort = false;
			viewModel.BoatTailLong = false;

			viewModel.IsNone = true;

			viewModel.IsStandard = false;
			viewModel.IsCertified = false;

			viewModel.ShowLengthBetweenCentersOfAxles = false;

			viewModel.YawAngle0 = default;
			viewModel.YawAngle3 = default;
			viewModel.YawAngle6 = default;
			viewModel.YawAngle9 = default;

			viewModel.MassInRunningOrder = default;
			viewModel.TPLMTotalTrailer = default;
			viewModel.TPLMAxleAssembly = default;
			viewModel.VolumeOrientation = false;

			foreach (var viewModelAxle in viewModel.Axles) {
				viewModelAxle.TwinTyres = false;
				viewModelAxle.Liftable = false;
				viewModelAxle.Steered = false;
			}
			



			viewModel.CertifiedAeroDeviceButtonsVisibility = false;
			viewModel.CertifiedAeroDeviceNewVisibility = true;

			TPMLMAxleAssemblyVisibility = true;
			TrailerCouplingPointVisibility = true;

			viewModel.TrailerCouplingPoint = TrailerCouplingPoint.Low;
		}

		
		private void SetCurrentValues(ICompleteTrailerViewModel trailer)
		{
			Manufacturer = trailer.Manufacturer;
			ManufacturerAddress = trailer.ManufacturerAddress;
			TrailerModel = trailer.TrailerModel;
			VIN = trailer.VIN;
			Date = DateTime.UtcNow;//Set DateTime UTC of current save
			LegislativeCategory = trailer.LegislativeCategory;
			ExternalHeightOfTheBody = trailer.ExternalHeightOfTheBody;
			ExternalLengthOfTheBody = trailer.ExternalLengthOfTheBody;
			ExternalWidthOfTheBody = trailer.ExternalWidthOfTheBody;
			TotalHeightOfTheTrailer = trailer.TotalHeightOfTheTrailer;
			LengthFromFrontToFirstAxle = trailer.LengthFromFrontToFirstAxle;
			CargoVolume = trailer.CargoVolume;
			NumberOfAxles = trailer.NumberOfAxles;
			TrailerType = trailer.TrailerType;
			BodyCode = trailer.BodyCode;
			MassInRunningOrder = trailer.MassInRunningOrder;
			TPLMTotalTrailer = trailer.TPLMTotalTrailer;
			TPLMAxleAssembly = trailer.TPLMAxleAssembly;

			VolumeOrientation = trailer.VolumeOrientation;

			Axles = trailer.Axles.Take(trailer.NumberOfAxles.ToInt()).ToList();

			ShowLengthBetweenCentersOfAxles = trailer.NumberOfAxles != NumberOfTrailerAxles.One;
			LengthBetweenCentersOfAxles = ShowLengthBetweenCentersOfAxles ? trailer.LengthBetweenCentersOfAxles : 0.SI<Meter>();

			IsNone = true;
			IsStandard = trailer.IsStandard;
			IsCertified = trailer.IsCertified;

			YawAngle0 = trailer.YawAngle0;
			YawAngle3 = trailer.YawAngle3;
			YawAngle6 = trailer.YawAngle6;
			YawAngle9 = trailer.YawAngle9;


			CertifiedAeroDeviceXml = trailer.CertifiedAeroDeviceXml;

			Technologies = trailer.Technologies;
			SideSkirtsShort = trailer.SideSkirtsShort;
			SideSkirtsLong = trailer.SideSkirtsLong;
			BoatTailShort = trailer.BoatTailShort;
			BoatTailLong = trailer.BoatTailLong;


			CertifiedAeroDeviceFilePath = trailer.CertifiedAeroDeviceFilePath;


			TrailerCouplingPoint = trailer.TrailerType == TypeTrailer.DC ? trailer.TrailerCouplingPoint : TrailerCouplingPoint.Unknown;
		}

		private void SetXmlNamesToPropertyMapping()
		{
			XmlNamesToPropertyMapping = new Dictionary<string, string> {
				{XMLNames.Component_Manufacturer, nameof(Manufacturer)},
				{XMLNames.Component_ManufacturerAddress, nameof(ManufacturerAddress)},
				{XMLNames.Component_Model, nameof(TrailerModel)},
				{XMLNames.Vehicle_VIN, nameof(VIN)},
				{XMLNames.Component_Date, nameof(Date)},
				{XMLNames.Vehicle_LegislativeClass, nameof(LegislativeCategory)},
				{XMLNames.Trailer_ExternalHeightBody, nameof(ExternalHeightOfTheBody)},
				{XMLNames.Trailer_ExternalLengthBody, nameof(ExternalLengthOfTheBody)},
				{XMLNames.Trailer_ExternalWidthBody, nameof(ExternalWidthOfTheBody)},
				{XMLNames.Trailer_TotalHeightTrailer, nameof(TotalHeightOfTheTrailer)},
				{XMLNames.Trailer_LengthFromFrontToFirstAxle, nameof(LengthFromFrontToFirstAxle)},
				{XMLNames.Trailer_LengthBetweenCentersOfAxles, nameof(LengthBetweenCentersOfAxles)},
				{XMLNames.Trailer_InternalHeightBody, nameof(InternalHeightOfTheBody)},
				{XMLNames.Trailer_CargoVolume, nameof(CargoVolume)},
				{XMLNames.Trailer_AxleCount, nameof(NumberOfAxles)},
				{XMLNames.Trailer_TrailerType, nameof(TrailerType)},
				{XMLNames.Trailer_BodyCode, nameof(BodyCode)},
				{XMLNames.Trailer_AeroFeatureTechnologies, nameof(Technologies)},
				{XMLNames.Trailer_CurbMass, nameof(MassInRunningOrder)},
				{XMLNames.Trailer_TPMLMTotalTrailer, nameof(TPLMTotalTrailer)},
				{XMLNames.Trailer_TPLMAxleAssembly, nameof(TPLMAxleAssembly)},
				{XMLNames.Trailer_VolumeOrientation, nameof(VolumeOrientation)},
				{XMLNames.AxleWheels_Axles, nameof(Axles)},
			};
		}


	}
}
