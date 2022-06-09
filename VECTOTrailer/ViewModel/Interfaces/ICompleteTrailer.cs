using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.ViewModel.Interfaces
{
	public interface ICompleteTrailer
	{
		string Manufacturer { get; set; }
		string ManufacturerAddress { get; set; }
		string TrailerModel { get; set; }
		string VIN { get; set; }
		DateTime Date { get; set; }
		string LegislativeCategory { get; set; }
		Meter ExternalLengthOfTheBody { get; set; }
		Meter ExternalWidthOfTheBody { get; set; }
		Meter ExternalHeightOfTheBody { get; set; }
		Meter TotalHeightOfTheTrailer { get; set; }
		Meter LengthFromFrontToFirstAxle { get; set; }
		Meter LengthBetweenCentersOfAxles { get; set; }
		CubicMeter CargoVolume { get; set; }
		NumberOfTrailerAxles NumberOfAxles { get; set; }
		TypeTrailer TrailerType { get; set; }
		BodyWorkCode BodyCode { get; set; }
		IList<AeroFeatureTechnology> Technologies { get; set; }
		bool ShowLengthBetweenCentersOfAxles { get; set; }
		bool IsStandard { get; set; }
		bool IsCertified { get; set; }
		bool IsNone { get; set; }
		bool SideSkirtsShort { get; set; }
		bool SideSkirtsLong { get; set; }
		bool BoatTailShort { get; set; }
		bool BoatTailLong { get; set; }
		double YawAngle0 { get; set; }
		double YawAngle3 { get; set; }
		double YawAngle6 { get; set; }
		double YawAngle9 { get; set; }
		Kilogram MassInRunningOrder { get; set; }
		Kilogram TPLMTotalTrailer { get; set; }
		Kilogram TPLMAxleAssembly { get; set; }
		bool VolumeOrientation { get; set; }
		ICollection<TrailerAxleViewModel> Axles { get; }
		string CertifiedAeroDeviceFilePath { get; set; }
		string CertifiedAeroDeviceXml { get; set; }
		ICommand SelectAxleXmlCommand { get; }
		ICommand SelectCertifiedAeroDeviceCommand { get; }
		JobFileType CertifiedAeroDeviceType { get; }
		Dictionary<string,string> XmlNamesToPropertyMapping { get; }
		bool TPMLMAxleAssemblyVisibility { get; set; }
		bool TrailerCouplingPointVisibility { get; set; }
		bool CertifiedAeroDeviceButtonsVisibility { get; set; }
		bool CertifiedAeroDeviceNewVisibility { get; set; }
		TrailerCouplingPoint TrailerCouplingPoint { get; set; }
		XDocument SourceDocument { get; }
	}
}
