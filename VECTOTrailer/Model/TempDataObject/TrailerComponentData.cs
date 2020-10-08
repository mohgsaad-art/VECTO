using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
		public string Model { get; set; }
		public string VIN { get; set; }
		public DateTime Date { get; set; }
		public string LegislativeCategory { get; set; }
		public Meter ExternalLengthOfTheBody { get; set; }
		public Meter ExternalWidthOfTheBody { get; set; }
		public Meter ExternalHeightOfTheBody { get; set; }
		public Meter TotalHeightOfTheTrailer { get; set; }
		public Meter PositionXFirstAxle { get; set; }
		public Meter PositionXLastAxle { get; set; }
		public Meter InternalHeightOfTheBody { get; set; }
		public CubicMeter CargoVolume { get; set; }
		public NumberOfTrailerAxles NumberOfAxles { get; set; }
		public TypeTrailer TrailerType { get; set; }
		public BodyWorkCode BodyCode { get; set; }
		public bool SideSkirtsShort { get; set; }
		public bool SideSkirtsLong { get; set; }
		public bool BoatTailShort { get; set; }
		public bool BoatTailLong { get; set; }
		public Kilogram CurbMassTrailer { get; set; }
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
		public ICommand SelectFirstFileCommand { get; set;  }
		public ICommand SelectSecondFileCommand { get; set;  }
		public ICommand SelectThirdFileCommand { get; set;  }
		public JobFileType FirstFileType { get; set;  }
		public JobFileType SecondFileType { get; set;  }
		public JobFileType ThirdFileType { get; set;  }

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
			viewModel.Model = Model;
			viewModel.VIN = VIN;
			viewModel.Date = Date;
			viewModel.LegislativeCategory = LegislativeCategory;
			viewModel.ExternalHeightOfTheBody = ExternalHeightOfTheBody;
			viewModel.ExternalLengthOfTheBody = ExternalLengthOfTheBody;
			viewModel.ExternalWidthOfTheBody = ExternalWidthOfTheBody;
			viewModel.TotalHeightOfTheTrailer = TotalHeightOfTheTrailer;
			viewModel.PositionXFirstAxle = PositionXFirstAxle;
			viewModel.PositionXLastAxle = PositionXFirstAxle;
			viewModel.InternalHeightOfTheBody = InternalHeightOfTheBody;
			viewModel.CargoVolume = CargoVolume;
			viewModel.NumberOfAxles = NumberOfAxles;
			viewModel.TrailerType = TrailerType;
			viewModel.BodyCode = BodyCode;
			viewModel.SideSkirtsShort = SideSkirtsShort;
			viewModel.SideSkirtsLong = SideSkirtsLong;
			viewModel.BoatTailShort = BoatTailShort;
			viewModel.BoatTailLong = BoatTailLong;
			viewModel.CurbMassTrailer = CurbMassTrailer;
			viewModel.TPLMTotalTrailer = TPLMTotalTrailer;
			viewModel.TPLMAxleAssembly = TPLMAxleAssembly;
			viewModel.Axle1IsTwin = Axle1IsTwin;
			viewModel.Axle2IsTwin = Axle2IsTwin;
			viewModel.Axle3IsTwin = Axle3IsTwin;
		}

		public void ClearValues(ICompleteTrailerViewModel viewModel)
		{
			viewModel.Manufacturer = default;
			viewModel.ManufacturerAddress = default;
			viewModel.Model = default;
			viewModel.VIN = default;
			viewModel.Date = DateTime.Now;
			viewModel.LegislativeCategory = default;
			viewModel.ExternalHeightOfTheBody = default;
			viewModel.ExternalLengthOfTheBody = default;
			viewModel.ExternalWidthOfTheBody = default;
			viewModel.TotalHeightOfTheTrailer = default;
			viewModel.PositionXFirstAxle = default;
			viewModel.PositionXLastAxle = default;
			viewModel.InternalHeightOfTheBody = default;
			viewModel.CargoVolume = default;
			viewModel.NumberOfAxles = NumberOfTrailerAxles.One;
			viewModel.TrailerType = TypeTrailer.DA;
			viewModel.BodyCode = BodyWorkCode.Drybody;
			viewModel.SideSkirtsShort = default;
			viewModel.SideSkirtsLong = default;
			viewModel.BoatTailShort = default;
			viewModel.BoatTailLong = default;
			viewModel.CurbMassTrailer = default;
			viewModel.TPLMTotalTrailer = default;
			viewModel.TPLMAxleAssembly = default;
			Axle1IsTwin = default;
			Axle2IsTwin = default;
			Axle3IsTwin = default;
		}

		
		private void SetCurrentValues(ICompleteTrailerViewModel trailer)
		{
			Manufacturer = trailer.Manufacturer;
			ManufacturerAddress = trailer.ManufacturerAddress;
			Model = trailer.Model;
			VIN = trailer.VIN;
			Date = DateTime.UtcNow;//Set DateTime UTC of current save
			LegislativeCategory = trailer.LegislativeCategory;
			ExternalHeightOfTheBody = trailer.ExternalHeightOfTheBody;
			ExternalLengthOfTheBody = trailer.ExternalLengthOfTheBody;
			ExternalWidthOfTheBody = trailer.ExternalWidthOfTheBody;
			TotalHeightOfTheTrailer = trailer.TotalHeightOfTheTrailer;
			PositionXFirstAxle = trailer.PositionXFirstAxle;
			PositionXLastAxle = trailer.PositionXLastAxle;
			InternalHeightOfTheBody = trailer.InternalHeightOfTheBody;
			CargoVolume = trailer.CargoVolume;
			NumberOfAxles = trailer.NumberOfAxles;
			TrailerType = trailer.TrailerType;
			BodyCode = trailer.BodyCode;
			SideSkirtsShort = trailer.SideSkirtsShort;
			SideSkirtsLong = trailer.SideSkirtsLong;
			BoatTailShort = trailer.BoatTailShort;
			BoatTailLong = trailer.BoatTailLong;
			CurbMassTrailer = trailer.CurbMassTrailer;
			TPLMTotalTrailer = trailer.TPLMTotalTrailer;
			TPLMAxleAssembly = trailer.TPLMAxleAssembly;
			Axle1IsTwin = trailer.Axle1IsTwin;
			Axle2IsTwin = trailer.Axle2IsTwin;
			Axle3IsTwin = trailer.Axle3IsTwin;
			Axle1Xml = trailer.Axle1Xml;
			Axle2Xml = trailer.Axle2Xml;
			Axle3Xml = trailer.Axle3Xml;
		}

		private void SetXmlNamesToPropertyMapping()
		{
			XmlNamesToPropertyMapping = new Dictionary<string, string> {
				{XMLNames.Component_Manufacturer, nameof(Manufacturer)},
				{XMLNames.Component_ManufacturerAddress, nameof(ManufacturerAddress)},
				{XMLNames.Component_Model, nameof(Model)},
				{XMLNames.Vehicle_VIN, nameof(VIN)},
				{XMLNames.Component_Date, nameof(Date)},
				{XMLNames.Vehicle_LegislativeClass, nameof(LegislativeCategory)},
				{XMLNames.Trailer_ExternalHeightBody, nameof(ExternalHeightOfTheBody)},
				{XMLNames.Trailer_ExternalLenghtBody, nameof(ExternalLengthOfTheBody)},
				{XMLNames.Trailer_ExternalWidthBody, nameof(ExternalWidthOfTheBody)},
				{XMLNames.Trailer_TotalHeightTrailer, nameof(TotalHeightOfTheTrailer)},
				{XMLNames.Trailer_PositionXFirstAxle, nameof(PositionXFirstAxle)},
				{XMLNames.Trailer_PositionXLastAxle, nameof(PositionXLastAxle)},
				{XMLNames.Trailer_InternalHeightBody, nameof(InternalHeightOfTheBody)},
				{XMLNames.Trailer_CargoVolume, nameof(CargoVolume)},
				{XMLNames.Trailer_AxleCount, nameof(NumberOfAxles)},
				{XMLNames.Trailer_TrailerType, nameof(TrailerType)},
				{XMLNames.Trailer_BodyCode, nameof(BodyCode)},
				{XMLNames.Trailer_AeroFeatureTechnologies, nameof(SideSkirtsShort)},
				{XMLNames.Trailer_CurbMass, nameof(CurbMassTrailer)},
				{XMLNames.Trailer_TPMLMTotalTrailer, nameof(TPLMTotalTrailer)},
				{XMLNames.Trailer_TPLMAxleAssembly, nameof(TPLMAxleAssembly)},
			};
		}


	}
}
