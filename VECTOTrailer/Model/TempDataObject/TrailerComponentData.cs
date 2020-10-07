using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TUGraz.VectoCommon.BusAuxiliaries;
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

		#region ICompleteVehicleBus Interface

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
		public AeroFeatureTechnologies Technologies { get; set; }
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
		public JobFileType FirstFileType { get; set;  }

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
			viewModel.Technologies = Technologies;
			viewModel.CurbMassTrailer = CurbMassTrailer;
			viewModel.TPLMTotalTrailer = TPLMTotalTrailer;
			viewModel.TPLMAxleAssembly = TPLMAxleAssembly;
		}

		public void ClearValues(ICompleteTrailerViewModel viewModel)
		{
			viewModel.Manufacturer = default(string);
			viewModel.ManufacturerAddress = default(string);
			viewModel.Model = default(string);
			viewModel.VIN = default(string);
			viewModel.Date = default(DateTime);
			viewModel.LegislativeCategory = default(string);
			viewModel.ExternalHeightOfTheBody = default(Meter);
			viewModel.ExternalLengthOfTheBody = default(Meter);
			viewModel.ExternalWidthOfTheBody = default(Meter);
			viewModel.TotalHeightOfTheTrailer = default(Meter);
			viewModel.PositionXFirstAxle = default(Meter);
			viewModel.PositionXLastAxle = default(Meter);
			viewModel.InternalHeightOfTheBody = default(Meter);
			viewModel.CargoVolume = default(CubicMeter);
			viewModel.NumberOfAxles = default(int);
			viewModel.TrailerType = default(TypeTrailer);
			viewModel.BodyCode = default(BodyWorkCode);
			viewModel.Technologies = default(AeroFeatureTechnologies);
			viewModel.CurbMassTrailer = default(Kilogram);
			viewModel.TPLMTotalTrailer = default(Kilogram);
			viewModel.TPLMAxleAssembly = default(Kilogram);
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
			Technologies = trailer.Technologies;
			CurbMassTrailer = trailer.CurbMassTrailer;
			TPLMTotalTrailer = trailer.TPLMTotalTrailer;
			TPLMAxleAssembly = trailer.TPLMAxleAssembly;
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
				{XMLNames.Trailer_AeroFeatureTechnologies, nameof(Technologies)},
				{XMLNames.Trailer_CurbMass, nameof(CurbMassTrailer)},
				{XMLNames.Trailer_TPMLMTotalTrailer, nameof(TPLMTotalTrailer)},
				{XMLNames.Trailer_TPLMAxleAssembly, nameof(TPLMAxleAssembly)},
			};
		}


	}
}
