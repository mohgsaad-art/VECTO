using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
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
		public string CommercialName { get; set; }
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
		public string Axle1FilePath { get; set; }
		public string Axle2FilePath { get; set; }
		public string Axle3FilePath { get; set; }
		public string Axle1Xml{ get; set; }
		public string Axle2Xml { get; set; }
		public string Axle3Xml { get; set; }
		public bool Axle1IsTwin { get; set; }
		public bool Axle2IsTwin { get; set; }
		public bool Axle3IsTwin { get; set; }
		public bool Axle1IsLiftable { get; set; }
		public bool Axle2IsLiftable { get; set; }
		public bool Axle3IsLiftable { get; set; }
		public bool Axle1IsSteered { get; set; }
		public bool Axle2IsSteered { get; set; }
		public bool Axle3IsSteered { get; set; }
		public bool Axle1IsNone { get; set; }
		public bool Axle2IsNone { get; set; }
		public bool Axle3IsNone { get; set; }
		public ICommand SelectFirstFileCommand { get; set;  }
		public ICommand SelectSecondFileCommand { get; set;  }
		public ICommand SelectThirdFileCommand { get; set;  }
		public ICommand DownloadFirstAxle { get; set; }
		public ICommand DeleteFirstAxle { get; set; }
		public ICommand DownloadSecondAxle { get; set; }
		public ICommand DeleteSecondAxle { get; set; }
		public ICommand DownloadThirdAxle { get; set; }
		public ICommand DeleteThirdAxle { get; set; }
		public JobFileType FirstFileType { get; set;  }
		public JobFileType SecondFileType { get; set;  }
		public JobFileType ThirdFileType { get; set;  }
		public ITrailerDeclarationInputData Trailer { get; set; }

		public bool Axle1ButtonsVisibility { get; set; }
		public bool Axle1NewVisibility { get; set; }
		public bool Axle2ButtonsVisibility { get; set; }
		public bool Axle2NewVisibility { get; set; }
		public bool Axle3ButtonsVisibility { get; set; }
		public bool Axle3NewVisibility { get; set; }
		public bool TPMLMAxleAssemblyVisibility { get; set; }
		public bool TrailerCouplingPointVisibility { get; set; }
		public TrailerCouplingPoint TrailerCouplingPoint { get; set; }
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
			viewModel.CommercialName = CommercialName;
			viewModel.VIN = VIN;
			viewModel.Date = Date;
			viewModel.LegislativeCategory = LegislativeCategory;
			viewModel.ExternalHeightOfTheBody = ExternalHeightOfTheBody;
			viewModel.ExternalLengthOfTheBody = ExternalLengthOfTheBody;
			viewModel.ExternalWidthOfTheBody = ExternalWidthOfTheBody;
			viewModel.TotalHeightOfTheTrailer = TotalHeightOfTheTrailer;
			viewModel.LengthFromFrontToFirstAxle = LengthFromFrontToFirstAxle;
			viewModel.InternalHeightOfTheBody = InternalHeightOfTheBody;
			viewModel.CargoVolume = CargoVolume;
			viewModel.NumberOfAxles = NumberOfAxles;
			viewModel.TrailerType = TrailerType;
			viewModel.BodyCode = BodyCode;
			viewModel.MassInRunningOrder = MassInRunningOrder;
			viewModel.TPLMTotalTrailer = TPLMTotalTrailer;
			viewModel.TPLMAxleAssembly = TPLMAxleAssembly;
			viewModel.Axle1IsTwin = Axle1IsTwin;
			viewModel.Axle2IsTwin = Axle2IsTwin;
			viewModel.Axle3IsTwin = Axle3IsTwin;
			viewModel.Axle1IsLiftable = Axle1IsLiftable;
			viewModel.Axle2IsLiftable = Axle2IsLiftable;
			viewModel.Axle3IsLiftable = Axle3IsLiftable;
			viewModel.Axle1IsSteered = Axle1IsSteered;
			viewModel.Axle2IsSteered = Axle2IsSteered;
			viewModel.Axle3IsSteered = Axle3IsSteered;

			viewModel.Axle1IsNone = !Axle1IsSteered && !Axle1IsLiftable;
			viewModel.Axle2IsNone = !Axle2IsSteered && !Axle2IsLiftable;
			viewModel.Axle3IsNone = !Axle3IsSteered && !Axle3IsLiftable;

			viewModel.Axle1Xml = Axle1Xml;
			viewModel.Axle2Xml = Axle2Xml;
			viewModel.Axle3Xml = Axle3Xml;

			viewModel.IsStandard = IsStandard;
			viewModel.IsCertified = IsCertified;

			viewModel.ShowLengthBetweenCentersOfAxles = NumberOfAxles != NumberOfTrailerAxles.One;
			viewModel.LengthBetweenCentersOfAxles = viewModel.ShowLengthBetweenCentersOfAxles ? viewModel.LengthBetweenCentersOfAxles : 0.SI<Meter>();

			viewModel.Technologies = Technologies;
			viewModel.SideSkirtsShort = Technologies != null && Technologies.Contains(AeroFeatureTechnology.SideSkirtsShort);
			viewModel.SideSkirtsLong = Technologies != null && Technologies.Contains(AeroFeatureTechnology.SideSkirtsLong);
			viewModel.BoatTailShort = Technologies != null && Technologies.Contains(AeroFeatureTechnology.BoatTailShort);
			viewModel.BoatTailLong = Technologies != null && Technologies.Contains(AeroFeatureTechnology.BoatTailLong);

			viewModel.YawAngle0 = YawAngle0;
			viewModel.YawAngle3 = YawAngle3;
			viewModel.YawAngle6 = YawAngle6;
			viewModel.YawAngle9 = YawAngle9;

			viewModel.Axle1ButtonsVisibility = Axle1ButtonsVisibility;
			viewModel.Axle1NewVisibility = Axle1NewVisibility;
			viewModel.Axle2ButtonsVisibility = Axle2ButtonsVisibility;
			viewModel.Axle2NewVisibility = Axle2NewVisibility;
			viewModel.Axle3ButtonsVisibility = Axle3ButtonsVisibility;
			viewModel.Axle3NewVisibility = Axle3NewVisibility;

			viewModel.TPMLMAxleAssemblyVisibility = TPMLMAxleAssemblyVisibility;
			viewModel.TrailerCouplingPointVisibility = TrailerCouplingPointVisibility;

			viewModel.TrailerCouplingPoint = viewModel.TrailerCouplingPoint;
		}

		public void ClearValues(ICompleteTrailerViewModel viewModel)
		{
			viewModel.Manufacturer = default;
			viewModel.ManufacturerAddress = default;
			viewModel.CommercialName = default;
			viewModel.VIN = default;
			viewModel.Date = DateTime.Now;
			viewModel.LegislativeCategory = default;
			viewModel.ExternalHeightOfTheBody = default;
			viewModel.ExternalLengthOfTheBody = default;
			viewModel.ExternalWidthOfTheBody = default;
			viewModel.TotalHeightOfTheTrailer = default;
			viewModel.LengthFromFrontToFirstAxle = default;
			viewModel.LengthBetweenCentersOfAxles = default;
			viewModel.InternalHeightOfTheBody = default;
			viewModel.CargoVolume = default;
			viewModel.NumberOfAxles = NumberOfTrailerAxles.One;
			viewModel.TrailerType = TypeTrailer.DA;
			viewModel.BodyCode = BodyWorkCode.Drybody;
			viewModel.TrailerCouplingPoint = TrailerCouplingPoint.Unknown;
			viewModel.SideSkirtsShort = false;
			viewModel.SideSkirtsLong = false;
			viewModel.BoatTailShort = false;
			viewModel.BoatTailLong = false;

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
			viewModel.Axle1IsTwin = false;
			viewModel.Axle2IsTwin = false;
			viewModel.Axle3IsTwin = false;
			viewModel.Axle1IsLiftable = false;
			viewModel.Axle2IsLiftable = false;
			viewModel.Axle3IsLiftable = false;
			viewModel.Axle1IsSteered = false;
			viewModel.Axle2IsSteered = false;
			viewModel.Axle3IsSteered = false;

			viewModel.Axle1IsNone = true;
			viewModel.Axle2IsNone = true;
			viewModel.Axle3IsNone = true;

			viewModel.Axle1ButtonsVisibility = false;
			viewModel.Axle1NewVisibility = true;
			viewModel.Axle2ButtonsVisibility = false;
			viewModel.Axle2NewVisibility = true;
			viewModel.Axle3ButtonsVisibility = false;
			viewModel.Axle3NewVisibility = true;

			TPMLMAxleAssemblyVisibility = true;
			TrailerCouplingPointVisibility = true;

			viewModel.TrailerCouplingPoint = TrailerCouplingPoint.Low;
		}

		
		private void SetCurrentValues(ICompleteTrailerViewModel trailer)
		{
			Manufacturer = trailer.Manufacturer;
			ManufacturerAddress = trailer.ManufacturerAddress;
			CommercialName = trailer.CommercialName;
			VIN = trailer.VIN;
			Date = DateTime.UtcNow;//Set DateTime UTC of current save
			LegislativeCategory = trailer.LegislativeCategory;
			ExternalHeightOfTheBody = trailer.ExternalHeightOfTheBody;
			ExternalLengthOfTheBody = trailer.ExternalLengthOfTheBody;
			ExternalWidthOfTheBody = trailer.ExternalWidthOfTheBody;
			TotalHeightOfTheTrailer = trailer.TotalHeightOfTheTrailer;
			LengthFromFrontToFirstAxle = trailer.LengthFromFrontToFirstAxle;
			InternalHeightOfTheBody = trailer.InternalHeightOfTheBody;
			CargoVolume = trailer.CargoVolume;
			NumberOfAxles = trailer.NumberOfAxles;
			TrailerType = trailer.TrailerType;
			BodyCode = trailer.BodyCode;
			MassInRunningOrder = trailer.MassInRunningOrder;
			TPLMTotalTrailer = trailer.TPLMTotalTrailer;
			TPLMAxleAssembly = trailer.TPLMAxleAssembly;
			Axle1IsTwin = trailer.Axle1IsTwin;
			Axle2IsTwin = trailer.Axle2IsTwin;
            Axle3IsTwin = trailer.Axle3IsTwin;

			ShowLengthBetweenCentersOfAxles = trailer.NumberOfAxles != NumberOfTrailerAxles.One;
			LengthBetweenCentersOfAxles = ShowLengthBetweenCentersOfAxles ? trailer.LengthBetweenCentersOfAxles : 0.SI<Meter>();

			IsNone = true;
			IsStandard = trailer.IsStandard;
			IsCertified = trailer.IsCertified;

			YawAngle0 = trailer.YawAngle0;
			YawAngle3 = trailer.YawAngle3;
			YawAngle6 = trailer.YawAngle6;
			YawAngle9 = trailer.YawAngle9;

			Axle1IsLiftable = trailer.Axle1IsLiftable;
			Axle2IsLiftable = trailer.Axle2IsLiftable;
			Axle3IsLiftable = trailer.Axle3IsLiftable;
			Axle1IsSteered = trailer.Axle1IsSteered;
			Axle2IsSteered = trailer.Axle2IsSteered;
			Axle3IsSteered = trailer.Axle3IsSteered;

			Axle1Xml = trailer.Axle1Xml;
			Axle2Xml = trailer.Axle2Xml;
			Axle3Xml = trailer.Axle3Xml;
			
			Technologies = trailer.Technologies;
			SideSkirtsShort = trailer.SideSkirtsShort;
			SideSkirtsLong = trailer.SideSkirtsLong;
			BoatTailShort = trailer.BoatTailShort;
			BoatTailLong = trailer.BoatTailLong;

			Axle1FilePath = trailer.Axle1FilePath;
			Axle2FilePath = trailer.Axle2FilePath;
			Axle3FilePath = trailer.Axle3FilePath;

			Axle1ButtonsVisibility = trailer.Axle1ButtonsVisibility;
			Axle1NewVisibility = trailer.Axle1NewVisibility;
			Axle2ButtonsVisibility = trailer.Axle2ButtonsVisibility;
			Axle2NewVisibility = trailer.Axle2NewVisibility;
			Axle3ButtonsVisibility = trailer.Axle3ButtonsVisibility;
			Axle3NewVisibility = trailer.Axle3NewVisibility;

			TrailerCouplingPoint = trailer.TrailerType == TypeTrailer.DC ? trailer.TrailerCouplingPoint : TrailerCouplingPoint.Unknown;
		}

		private void SetXmlNamesToPropertyMapping()
		{
			XmlNamesToPropertyMapping = new Dictionary<string, string> {
				{XMLNames.Component_Manufacturer, nameof(Manufacturer)},
				{XMLNames.Component_ManufacturerAddress, nameof(ManufacturerAddress)},
				{XMLNames.Component_Model, nameof(CommercialName)},
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
				{XMLNames.AxleWheels_Axles, nameof(Axle1FilePath)},
			};
		}


	}
}
