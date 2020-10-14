using System;
using System.Collections.Generic;
using System.Windows.Input;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface ICompleteTrailer
	{
		string Manufacturer { get; set; }
		string ManufacturerAddress { get; set; }
		string Model { get; set; }
		string VIN { get; set; }
		DateTime Date { get; set; }
		string LegislativeCategory { get; set; }
		Meter ExternalLengthOfTheBody { get; set; }
		Meter ExternalWidthOfTheBody { get; set; }
		Meter ExternalHeightOfTheBody { get; set; }
		Meter TotalHeightOfTheTrailer { get; set; }
		Meter PositionXFirstAxle { get; set; }
		Meter PositionXLastAxle { get; set; }
		Meter InternalHeightOfTheBody { get; set; }
		CubicMeter CargoVolume { get; set; }
		NumberOfTrailerAxles NumberOfAxles { get; set; }
		TypeTrailer TrailerType { get; set; }
		BodyWorkCode BodyCode { get; set; }
		bool SideSkirtsShort { get; set; }
		bool SideSkirtsLong { get; set; }
		bool BoatTailShort { get; set; }
		bool BoatTailLong { get; set; }
		Kilogram CurbMassTrailer { get; set; }
		Kilogram TPLMTotalTrailer { get; set; }
		Kilogram TPLMAxleAssembly { get; set; }
		string Axle1FilePath { get; set; }
		string Axle2FilePath { get; set; }
		string Axle3FilePath { get; set; }
		string Axle1Xml{ get; set; }
		string Axle2Xml { get; set; }
		string Axle3Xml { get; set; }
		bool Axle1IsTwin { get; set; }
		bool Axle2IsTwin { get; set; }
		bool Axle3IsTwin { get; set; }
		ICommand SelectFirstFileCommand { get; }
		ICommand SelectSecondFileCommand { get; }
		ICommand SelectThirdFileCommand { get; }
		JobFileType FirstFileType { get; }
		JobFileType SecondFileType { get; }
		JobFileType ThirdFileType { get; }
		Dictionary<string,string> XmlNamesToPropertyMapping { get; }
	}
}
