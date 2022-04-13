using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Castle.Core;
using Castle.Core.Internal;
using Microsoft.Win32;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using VECTOTrailer.Helper;
using VECTOTrailer.Model;
using VECTOTrailer.Model.TempDataObject;
using VECTOTrailer.Properties;
using VECTOTrailer.Util;
using VECTOTrailer.ViewModel.Adapter;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Impl
{
	public class CompleteTrailerViewModel : AbstractComponentViewModel, ICompleteTrailerViewModel
	{
		#region Members

		private ITrailerDeclarationInputData _trailer;
		private ITrailerDeclarationInputData _trailerObject;
		private TrailerComponentData _componentData;

		private string _manufacturer;
		private string _manufacturerAddress;
		private string _trailerModel;
		private string _model;
		private string _vin;
		private DateTime _date;
		private string _legislativeCategory;
		private Meter _externalLengthOfTheBody;
		private Meter _externalWidthOfTheBody;
		private Meter _externalHeightOfTheBody;
		private Meter _totalHeightOfTheTrailer;
		private Meter _lengthFromFrontToFirstAxle;
		private Meter _lengthBetweenCentersOfAxles;
		private CubicMeter _cargoVolume;
		private NumberOfTrailerAxles _numberOfAxles;
		private TypeTrailer _trailerType;
		private BodyWorkCode _bodyCode;
		private IList<AeroFeatureTechnology> _technologies;

		private bool _isStandard;
		private bool _isCertified;
		private bool _isNone;
		private bool _showLengthBetweenCentersOfAxles;

		private bool _sideSkirtsShort;
		private bool _sideSkirtsLong;
		private bool _boatTailShort;
		private bool _boatTailLong;

		private double _yawAngle0;
		private double _yawAngle3;
		private double _yawAngle6;
		private double _yawAngle9;

		private Kilogram _curbMassTrailer;
		private Kilogram _tPLMTotalTrailer;
		private Kilogram _tPLMAxleAssembly;

		private bool _volumeOrientation;

		private string _axle1FilePath;
		private string _axle2FilePath;
		private string _axle3FilePath;

		private string _certifiedAeroDeviceFilePath;

		private string _axle1Xml;
		private string _axle2Xml;
		private string _axle3Xml;

		private string _certifiedAeroDeviceXml;

		private bool _axle1IsTwin;
		private bool _axle2IsTwin;
		private bool _axle3IsTwin;
		private bool _axle1IsLiftable;
		private bool _axle2IsLiftable;
		private bool _axle3IsLiftable;
		private bool _axle1IsSteered;
		private bool _axle2IsSteered;
		private bool _axle3IsSteered;

		private bool _axle1IsNone;
		private bool _axle2IsNone;
		private bool _axle3IsNone;

		private ICommand _selectFirstFileCommand;
		private ICommand _selectSecondFileCommand;
		private ICommand _selectThirdFileCommand;

		private ICommand _selectCertifiedAeroDeviceCommand;


		private JobFileType _firstFileType;
		private JobFileType _secondFileType;
		private JobFileType _thirdFileType;

		private JobFileType _certifiedAeroDeviceType;

		private string _firstFilePath;
		private string _secondFilePath;
		private string _thirdFilePath;

		private bool _certifiedAeroDeviceButtonsVisibility;
		private bool _certifiedAeroDeviceNewVisibility;

		private bool _tPMLMAxleAssemblyVisibility;
		private bool _trailerCouplingPointVisibility;
		private TrailerCouplingPoint _TrailerCouplingPoint;

		private readonly bool _editJob;
		protected SettingsModel Settings = new SettingsModel();
		protected JobEntry JobEntry;

		#endregion

		#region ICompleteTrailerViewModel

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

		public ITrailerDeclarationInputData ModelData => AdapterFactory.TrailerDeclarationAdapter(this);

		public ITrailerDeclarationInputData Trailer { get => _trailerObject; set => SetProperty(ref _trailerObject, value); }

		public string Manufacturer
		{
			get => _manufacturer;
			set
			{
				if (!SetProperty(ref _manufacturer, value))
					return;
				IsDataChanged(_manufacturer, _componentData);
			}
		}
		public string ManufacturerAddress
		{
			get => _manufacturerAddress;
			set
			{
				if (!SetProperty(ref _manufacturerAddress, value))
					return;
				IsDataChanged(_manufacturerAddress, _componentData);
			}
		}
		public string TrailerModel
		{
			get => _trailerModel;
			set
			{
				if (!SetProperty(ref _trailerModel, value))
					return;
				IsDataChanged(_trailerModel, _componentData);
			}
		}

		public string VIN
		{
			get => _vin;
			set
			{
				if (!SetProperty(ref _vin, value))
					return;
				IsDataChanged(_vin, _componentData);
			}
		}
		public DateTime Date
		{
			get => DateTime.Now;
			set
			{
				if (!SetProperty(ref _date, value))
					return;
				IsDataChanged(_date, _componentData);
			}
		}
		public string LegislativeCategory
		{
			get => _legislativeCategory;
			set
			{
				if (!SetProperty(ref _legislativeCategory, value))
					return;
				IsDataChanged(_legislativeCategory, _componentData);
			}
		}

		public Meter ExternalLengthOfTheBody
		{
			get => _externalLengthOfTheBody;
			set
			{
				if (!SetProperty(ref _externalLengthOfTheBody, value))
					return;
				IsDataChanged(_externalLengthOfTheBody, _componentData);
			}
		}

		public Meter ExternalWidthOfTheBody
		{
			get => _externalWidthOfTheBody;
			set
			{
				if (!SetProperty(ref _externalWidthOfTheBody, value))
					return;
				IsDataChanged(_externalWidthOfTheBody, _componentData);
			}
		}

		public Meter ExternalHeightOfTheBody
		{
			get => _externalHeightOfTheBody;
			set
			{
				if (!SetProperty(ref _externalHeightOfTheBody, value))
					return;
				IsDataChanged(_externalHeightOfTheBody, _componentData);
			}
		}

		public Meter TotalHeightOfTheTrailer
		{
			get => _totalHeightOfTheTrailer;
			set
			{
				if (!SetProperty(ref _totalHeightOfTheTrailer, value))
					return;
				IsDataChanged(_totalHeightOfTheTrailer, _componentData);
			}
		}

		public Meter LengthFromFrontToFirstAxle
		{
			get => _lengthFromFrontToFirstAxle;
			set
			{
				if (!SetProperty(ref _lengthFromFrontToFirstAxle, value))
					return;
				IsDataChanged(_lengthFromFrontToFirstAxle, _componentData);
			}
		}

		public Meter LengthBetweenCentersOfAxles
		{
			get => _lengthBetweenCentersOfAxles;
			set
			{
				if (!SetProperty(ref _lengthBetweenCentersOfAxles, value))
					return;
				IsDataChanged(_lengthBetweenCentersOfAxles, _componentData);
			}
		}

		public CubicMeter CargoVolume
		{
			get => _cargoVolume;
			set
			{
				if (!SetProperty(ref _cargoVolume, value))
					return;
				IsDataChanged(_cargoVolume, _componentData);
			}
		}

		public NumberOfTrailerAxles NumberOfAxles
		{
			get => _numberOfAxles;
			set
			{
				if (!SetProperty(ref _numberOfAxles, value))
					return;

				ShowLengthBetweenCentersOfAxles = NumberOfAxles != NumberOfTrailerAxles.One;
				LengthBetweenCentersOfAxles = ShowLengthBetweenCentersOfAxles ? LengthBetweenCentersOfAxles : 0.SI<Meter>();

				IsDataChanged(_numberOfAxles, _componentData);
			}
		}

		public TypeTrailer TrailerType
		{
			get => _trailerType;
			set
			{
				if (!SetProperty(ref _trailerType, value)) {
					return;
				}

				if (value == TypeTrailer.DB) {
					Axle1IsSteered = true;
				}

				TPMLMAxleAssemblyVisibility = TrailerType != TypeTrailer.DB;

				TrailerCouplingPointVisibility = TrailerType is TypeTrailer.DC;

				if(TrailerType != TypeTrailer.DC)
					TrailerCouplingPoint = TrailerCouplingPoint.Unknown;

				if (TrailerType == TypeTrailer.DB)
					TPLMAxleAssembly = 0.SI<Kilogram>();

				IsDataChanged(_trailerType, _componentData);
			}
		}

		public bool VolumeOrientation
		{
			get => _volumeOrientation;
			set => SetProperty(ref _volumeOrientation, value);
		}

		public BodyWorkCode BodyCode
		{
			get => _bodyCode;
			set
			{
				if (!SetProperty(ref _bodyCode, value))
					return;
				IsDataChanged(_bodyCode, _componentData);
			}
		}
		public bool IsNone
		{
			get => _isNone;
			set => SetProperty(ref _isNone, value);
		}

		public bool ShowLengthBetweenCentersOfAxles
		{
			get => _showLengthBetweenCentersOfAxles;
			set => SetProperty(ref _showLengthBetweenCentersOfAxles, value);
		}

		public bool IsStandard
		{
			get => _isStandard;
			set => SetProperty(ref _isStandard, value);
		}

		public bool IsCertified
		{
			get => _isCertified;
			set => SetProperty(ref _isCertified, value);
		}

		public bool SideSkirtsShort
		{
			get => _sideSkirtsShort;
			set => SetProperty(ref _sideSkirtsShort, value);
		}
		public bool SideSkirtsLong
		{
			get => _sideSkirtsLong;
			set => SetProperty(ref _sideSkirtsLong, value);
		}
		public bool BoatTailShort
		{
			get => _boatTailShort;
			set => SetProperty(ref _boatTailShort, value);
		}
		public bool BoatTailLong
		{
			get => _boatTailLong;
			set => SetProperty(ref _boatTailLong, value);
		}

		public double YawAngle0
		{
			get => _yawAngle0;
			set => SetProperty(ref _yawAngle0, value);
		}
		public double YawAngle3
		{
			get => _yawAngle3;
			set => SetProperty(ref _yawAngle3, value);
		}
		public double YawAngle6
		{
			get => _yawAngle6;
			set => SetProperty(ref _yawAngle6, value);
		}
		public double YawAngle9
		{
			get => _yawAngle9;
			set => SetProperty(ref _yawAngle9, value);
		}

		public Kilogram MassInRunningOrder
		{
			get => _curbMassTrailer;
			set
			{
				if (!SetProperty(ref _curbMassTrailer, value))
					return;
				IsDataChanged(_curbMassTrailer, _componentData);
			}
		}
		public Kilogram TPLMTotalTrailer
		{
			get => _tPLMTotalTrailer;
			set
			{
				if (!SetProperty(ref _tPLMTotalTrailer, value))
					return;
				IsDataChanged(_tPLMTotalTrailer, _componentData);
			}
		}
		public Kilogram TPLMAxleAssembly
		{
			get => _tPLMAxleAssembly;
			set
			{
				if (!SetProperty(ref _tPLMAxleAssembly, value))
					return;
				IsDataChanged(_tPLMAxleAssembly, _componentData);
			}
		}

		public string Axle1FilePath
		{
			get => _axle1FilePath;
			set => SetProperty(ref _axle1FilePath, value);
		}

		public string Axle2FilePath
		{
			get => _axle2FilePath;
			set => SetProperty(ref _axle2FilePath, value);
		}


		public string Axle3FilePath
		{
			get => _axle3FilePath;
			set => SetProperty(ref _axle3FilePath, value);
		}

		public string CertifiedAeroDeviceFilePath
		{
			get => _certifiedAeroDeviceFilePath;
			set => SetProperty(ref _certifiedAeroDeviceFilePath, value);
		}

		public string Axle1Xml
		{
			get
			{
				if (string.IsNullOrEmpty(_firstFilePath))
					return _axle1Xml;

				return File.ReadAllText(_firstFilePath);
			}
			set => SetProperty(ref _axle1Xml, value);
		}
		public string Axle2Xml
		{
			get
			{
				if (string.IsNullOrEmpty(_secondFilePath))
					return _axle2Xml;

				return File.ReadAllText(_secondFilePath);
			}
			set => SetProperty(ref _axle2Xml, value);
		}
		public string Axle3Xml
		{
			get
			{
				if (string.IsNullOrEmpty(_thirdFilePath))
					return _axle3Xml;

				return File.ReadAllText(_thirdFilePath);
			}
			set => SetProperty(ref _axle3Xml, value);
		}

		public string CertifiedAeroDeviceXml
		{
			get
			{
				if (string.IsNullOrEmpty(_certifiedAeroDeviceFilePath) || !_certifiedAeroDeviceFilePath.Contains(".xml"))
					return _certifiedAeroDeviceXml;

				return File.ReadAllText(_certifiedAeroDeviceFilePath);
			}
			set => SetProperty(ref _certifiedAeroDeviceXml, value);
		}

		public bool Axle1IsTwin
		{
			get => _axle1IsTwin;
			set => SetProperty(ref _axle1IsTwin, value);
		}
		public bool Axle2IsTwin
		{
			get => _axle2IsTwin;
			set => SetProperty(ref _axle2IsTwin, value);
		}
		public bool Axle3IsTwin
		{
			get => _axle3IsTwin;
			set => SetProperty(ref _axle3IsTwin, value);
		}

		public bool Axle1IsLiftable
		{
			get => _axle1IsLiftable;
			set => SetProperty(ref _axle1IsLiftable, value);
		}
		public bool Axle2IsLiftable
		{
			get => _axle2IsLiftable;
			set => SetProperty(ref _axle2IsLiftable, value);
		}
		public bool Axle3IsLiftable
		{
			get => _axle3IsLiftable;
			set => SetProperty(ref _axle3IsLiftable, value);
		}

		public bool Axle1IsSteered
		{
			get => _axle1IsSteered;
			set => SetProperty(ref _axle1IsSteered, value);
		}


		public bool Axle2IsSteered
		{
			get => _axle2IsSteered;
			set => SetProperty(ref _axle2IsSteered, value);
		}
		public bool Axle3IsSteered
		{
			get => _axle3IsSteered;
			set => SetProperty(ref _axle3IsSteered, value);
		}

		public bool Axle1IsNone
		{
			get => _axle1IsNone;
			set => SetProperty(ref _axle1IsNone, value);
		}
		public bool Axle2IsNone
		{
			get => _axle2IsNone;
			set => SetProperty(ref _axle2IsNone, value);
		}
		public bool Axle3IsNone
		{
			get => _axle3IsNone;
			set => SetProperty(ref _axle3IsNone, value);
		}

		public IList<AeroFeatureTechnology> Technologies
		{
			get => _technologies;
			set
			{
				if (!SetProperty(ref _technologies, value))
					return;
				IsDataChanged(_technologies, _componentData);
			}
		}
		public TrailerCouplingPoint TrailerCouplingPoint
		{
			get => _TrailerCouplingPoint;
			set => SetProperty(ref _TrailerCouplingPoint, value);
		}
		
		public bool CertifiedAeroDeviceButtonsVisibility
		{
			get => _certifiedAeroDeviceButtonsVisibility;
			set => SetProperty(ref _certifiedAeroDeviceButtonsVisibility, value);
		}
		public bool CertifiedAeroDeviceNewVisibility
		{
			get => _certifiedAeroDeviceNewVisibility;
			set => SetProperty(ref _certifiedAeroDeviceNewVisibility, value);
		}

		public bool TPMLMAxleAssemblyVisibility {
			get => _tPMLMAxleAssemblyVisibility;
			set => SetProperty(ref _tPMLMAxleAssemblyVisibility, value);
		}

		public bool TrailerCouplingPointVisibility
		{
			get => _trailerCouplingPointVisibility;
			set => SetProperty(ref _trailerCouplingPointVisibility, value);
		}


		public Dictionary<string, string> XmlNamesToPropertyMapping { get; private set; }
		public AllowedEntry<TypeTrailer>[] AllowedTrailerTypes { get; private set; }
		public AllowedEntry<BodyWorkCode>[] AllowedBodyCodes { get; private set; }
		public AllowedEntry<AeroFeatureTechnology>[] AllowedTechnologies { get; private set; }
		public AllowedEntry<NumberOfTrailerAxles>[] AllowedNumberOfAxles { get; private set; }
		public AllowedEntry<string>[] AllowedLegislativeCategory { get; private set; }
		public AllowedEntry<TrailerCouplingPoint>[] AllowedTrailerCouplingPoint { get; private set; }

		#endregion

		[Inject] public IDeclarationInjectFactory Factory { protected get; set; }

		#region Set XML Data

		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationTrailerInputDataProvider;
			_trailer = inputData?.JobInputData.Trailer;
			SetVehicleValues(_trailer);
			SetAllowedEntries();
			XmlNamesToPropertyMapping = _componentData.XmlNamesToPropertyMapping;

		}

		private void SetVehicleValues(ITrailerDeclarationInputData trailer)
		{
			if (trailer == null)
			{
				_componentData = new TrailerComponentData(this, true);
			}
			else
			{
				Manufacturer = trailer.Manufacturer;
				ManufacturerAddress = trailer.ManufacturerAddress;
				TrailerModel = trailer.TrailerModel;
				VIN = trailer.VIN;
				Date = trailer.Date;
				LegislativeCategory = trailer.LegislativeCategory;
				NumberOfAxles = trailer.NumberOfAxles;
				TrailerType = trailer.TrailerType;
				BodyCode = trailer.BodyworkCode;
				MassInRunningOrder = trailer.MassInRunningOrder;
				TPLMTotalTrailer = trailer.TPMLMTotalTrailer;
				TPLMAxleAssembly = trailer.TPMLMAxleAssembly;
				ExternalLengthOfTheBody = trailer.ExternalBodyLength;
				ExternalWidthOfTheBody = trailer.ExternalBodyWidth;
				ExternalHeightOfTheBody = trailer.ExternalBodyHeight;
				TotalHeightOfTheTrailer = trailer.TotalTrailerHeight;
				LengthFromFrontToFirstAxle = trailer.LengthFromFrontToFirstAxle;
				LengthBetweenCentersOfAxles = trailer.LengthBetweenCentersOfAxles;
				CargoVolume = trailer.CargoVolume;
				Technologies = trailer.AeroFeatureTechnologies;
				VolumeOrientation = trailer.VolumeOrientation;

				IsCertified = (trailer.YawAngle0 != 0 || trailer.YawAngle3 != 0 || trailer.YawAngle6 != 0 || trailer.YawAngle9 != 0);

				YawAngle0 = trailer.YawAngle0;
				YawAngle3 = trailer.YawAngle3;
				YawAngle6 = trailer.YawAngle6;
				YawAngle9 = trailer.YawAngle9;

				SideSkirtsShort = trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.SideSkirtsShort);
				SideSkirtsLong = trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.SideSkirtsLong);
				BoatTailShort = trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.BoatTailShort);
				BoatTailLong = trailer.AeroFeatureTechnologies.Contains(AeroFeatureTechnology.BoatTailLong);

				IsStandard = (SideSkirtsShort || SideSkirtsLong || BoatTailShort || BoatTailLong);

				IsNone = (!IsCertified && !IsStandard);

				ShowLengthBetweenCentersOfAxles = trailer.NumberOfAxles != NumberOfTrailerAxles.One;
				
				TrailerCouplingPoint = trailer.TrailerType == TypeTrailer.DC ? trailer.TrailerCouplingPoint : TrailerCouplingPoint.Unknown;

				Trailer = _trailer;

				var xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(_trailer.XMLSource.OuterXml.ToString(CultureInfo.InvariantCulture));

				var axles = (xmlDoc.GetElementsByTagName("Axles").Item(0) ?? throw new Exception("Cannot parse axle information")).Cast<XmlNode>().ToList();

				switch (axles.Count) {
					case 0: break;
					case 1:
						EnableFirstAxle(axles[0]);
						break;
					case 2:
						EnableFirstAxle(axles[0]);
						EnableSecondAxle(axles[1]);
						break;
					case 3:
						EnableFirstAxle(axles[0]);
						EnableSecondAxle(axles[1]);
						EnableThirdAxle(axles[2]);
						break;
				}

				if (IsCertified) {
					var device = (xmlDoc.GetElementsByTagName(XMLNames.Trailer_CertifiedAeroDevice) ??
								throw new Exception("Cannot parse aero device information")).Cast<XmlNode>().ToList();

					EnableCertifiedAeroDevice(device[0]);
				}



				_componentData = new TrailerComponentData(this);
				var errors = new List<string>() as IList<string>;
				if(!DeclarationTrailerDataAdapter.ValidateTrailerInput(trailer, ref errors)) {
					MessageBox.Show(caption: "Invalid input file!", messageBoxText: string.Join("\n", errors),button:MessageBoxButton.OK, icon:MessageBoxImage.Information);
				}
			}

			ClearChangedProperties();
		}

		private void EnableFirstAxle(XmlNode axle)
		{
			Axle1Xml = axle.OuterXml.ToString(CultureInfo.InvariantCulture);
			Axle1FilePath = axle.ChildNodes[3].ChildNodes[0].ChildNodes[1].InnerText + 
				", " + axle.ChildNodes[3].ChildNodes[0].ChildNodes[5].InnerText;
			Axle1IsTwin = axle.ChildNodes[0].InnerText == "true";
			Axle1IsLiftable = axle.ChildNodes[1].InnerText == "true";
			Axle1IsSteered = axle.ChildNodes[2].InnerText == "true";

			Axle1IsNone = axle.ChildNodes[1].InnerText == "false" && axle.ChildNodes[2].InnerText == "false";
		}

		private void EnableSecondAxle(XmlNode axle)
		{
			Axle2Xml = axle.OuterXml.ToString(CultureInfo.InvariantCulture);
			Axle2FilePath = axle.ChildNodes[3].ChildNodes[0].ChildNodes[1].InnerText +
							", " + axle.ChildNodes[3].ChildNodes[0].ChildNodes[5].InnerText;
			Axle2IsTwin = axle.ChildNodes[0].InnerText == "true";
			Axle2IsLiftable = axle.ChildNodes[1].InnerText == "true";
			Axle2IsSteered = axle.ChildNodes[2].InnerText == "true";

			Axle2IsNone = axle.ChildNodes[1].InnerText == "false" && axle.ChildNodes[2].InnerText == "false";
		}

		private void EnableThirdAxle(XmlNode axle)
		{
			Axle3Xml = axle.OuterXml.ToString(CultureInfo.InvariantCulture);
			Axle3FilePath = axle.ChildNodes[3].ChildNodes[0].ChildNodes[1].InnerText +
							", " + axle.ChildNodes[3].ChildNodes[0].ChildNodes[5].InnerText;
			Axle3IsTwin = axle.ChildNodes[0].InnerText == "true";
			Axle3IsLiftable = axle.ChildNodes[1].InnerText == "true";
			Axle3IsSteered = axle.ChildNodes[2].InnerText == "true";

			Axle3IsNone = axle.ChildNodes[1].InnerText == "false" && axle.ChildNodes[2].InnerText == "false";
		}

		private void EnableCertifiedAeroDevice(XmlNode device)
		{
			CertifiedAeroDeviceXml = device.OuterXml.ToString(CultureInfo.InvariantCulture);
			CertifiedAeroDeviceFilePath = device.ChildNodes[0].ChildNodes[0].InnerText +
										", " + device.ChildNodes[0].ChildNodes[1].InnerText;
			
			CertifiedAeroDeviceButtonsVisibility = true;
			CertifiedAeroDeviceNewVisibility = false;
		}

		private void SetAllowedEntries()
		{

			AllowedTrailerTypes = EnumHelper.GetValues<TypeTrailer>().Where(x => x != TypeTrailer.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedBodyCodes = EnumHelper.GetValues<BodyWorkCode>().Where(x => x != BodyWorkCode.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedTechnologies = EnumHelper.GetValues<AeroFeatureTechnology>().Where(x => x != AeroFeatureTechnology.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedNumberOfAxles = EnumHelper.GetValues<NumberOfTrailerAxles>().Where(x => x != NumberOfTrailerAxles.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedTrailerCouplingPoint = EnumHelper.GetValues<TrailerCouplingPoint>().Where(x => x != TrailerCouplingPoint.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedLegislativeCategory = new[] { AllowedEntry.Create("O3", "O3"), AllowedEntry.Create("O4", "O4") };
		}

		#endregion

		public override void ResetComponentData()
		{
			_componentData.ResetToComponentValues(this);
		}

		public override object CommitComponentData()
		{
			_componentData.UpdateCurrentValues(this);
			ClearChangedProperties();
			return _componentData;
		}

		public override void ShowValidationErrors(Dictionary<string, string> errors)
		{
			if (errors.IsNullOrEmpty())
				return;

			foreach (var error in errors)
			{
				string propertyName;
				if (XmlNamesToPropertyMapping.TryGetValue(error.Key, out propertyName))
					AddPropertyError(propertyName, error.Value);
			}
		}

		public override void RemoveValidationErrors(Dictionary<string, string> errors)
		{
			if (errors.IsNullOrEmpty())
				return;

			foreach (var error in errors)
			{
				string propertyName;
				if (XmlNamesToPropertyMapping.TryGetValue(error.Key, out propertyName))
					RemovePropertyError(propertyName);
			}
		}

		public JobFileType FirstFileType
		{
			get => _firstFileType;
			set => SetProperty(ref _firstFileType, value);
		}

		public JobFileType SecondFileType
		{
			get => _secondFileType;
			set => SetProperty(ref _secondFileType, value);
		}

		public JobFileType ThirdFileType
		{
			get => _thirdFileType;
			set => SetProperty(ref _thirdFileType, value);
		}

		public JobFileType CertifiedAeroDeviceType
		{
			get => _certifiedAeroDeviceType;
			set => SetProperty(ref _certifiedAeroDeviceType, value);
		}

		public string FirstFilePath
		{
			get => _firstFilePath;
			set => SetProperty(ref _firstFilePath, value);
		}

		public string SecondFilePath
		{
			get => _secondFilePath;
			set => SetProperty(ref _secondFilePath, value);
		}

		public string ThirdFilePath
		{
			get => _thirdFilePath;
			set => SetProperty(ref _thirdFilePath, value);
		}

		public ICommand SelectFirstAxleXmlCommand =>
			_selectFirstFileCommand ??
			(_selectFirstFileCommand = new RelayCommand<JobFileType>(DoSelectFirstFileCommand));

		public ICommand SelectSecondAxleXmlCommand =>
			_selectSecondFileCommand ??
			(_selectSecondFileCommand = new RelayCommand<JobFileType>(DoSelectSecondFileCommand));

		public ICommand SelectThirdAxleXmlCommand =>
			_selectThirdFileCommand ??
			(_selectThirdFileCommand = new RelayCommand<JobFileType>(DoSelectThirdFileCommand));

		public ICommand SelectCertifiedAeroDeviceCommand =>
			_selectCertifiedAeroDeviceCommand ??
			(_selectCertifiedAeroDeviceCommand = new RelayCommand<JobFileType>(DoSelectCertifiedAeroDeviceFileCommand));



		private void DoSelectFirstFileCommand(JobFileType jobFileType)
		{
			SelectFile(jobFileType, "Invalid tyre xml", (result) => {
				FirstFilePath = result;
				this.Axle1FilePath = result;
			});
			//if (OpenFileSelector(jobFileType, nameof(FirstFilePath), FirstFilePath, out var result))
			//{
			//	FirstFilePath = result;
			//	this.Axle1FilePath = FirstFilePath;
			//}
		}
		private void DoSelectSecondFileCommand(JobFileType jobFileType)
		{
			SelectFile(jobFileType, "Invalid tyre xml", (result) => { 
				SecondFilePath = result;
				this.Axle2FilePath = result;
			});
			//if (OpenFileSelector(jobFileType, nameof(SecondFilePath), SecondFilePath, out var result))
			//{
			//	SecondFilePath = result;
			//	this.Axle2FilePath = SecondFilePath;
			//}
		}
		private void DoSelectThirdFileCommand(JobFileType jobFileType)
		{
			SelectFile(jobFileType, "Invalid tyre xml", (result) => {
				ThirdFilePath = result;
				this.Axle3FilePath = result;
			});
			//if (OpenFileSelector(jobFileType, nameof(ThirdFilePath), ThirdFilePath, out var result)) {
			//	ThirdFilePath = result;
			//	this.Axle3FilePath = ThirdFilePath;
			//}
		}

		private void SelectFile(JobFileType jobFileType, string errorCaption, Action<string> successAction)
		{
			if (OpenFileSelector(jobFileType, nameof(CertifiedAeroDeviceFilePath),
					CertifiedAeroDeviceFilePath, out var result))
			{
				var aeroDeviceXDoc = XDocument.Load(result);
				if (!XmlHelper.ValidateXDocument(aeroDeviceXDoc, validationErrorAction: ValidationErrorAction))
				{
					MessageBox.Show(caption: errorCaption,
						messageBoxText: string.Join("\n", xmlFileErrors),
						button: MessageBoxButton.OK,
						icon: MessageBoxImage.Error);
					xmlFileErrors.Clear();
				}
				else
				{
					if (!VectoHash.Load(result).ValidateHash()) {
						MessageBox.Show(caption: "Invalid Hash",
							messageBoxText: $"Component file {result} has invalid hash\n",
							icon: MessageBoxImage.Warning,
							button: MessageBoxButton.OK);
					}
					
					successAction.Invoke(result);
					//CertifiedAeroDeviceFilePath = result;
				}

			}
		}

		private IList<string> xmlFileErrors = new List<string>();
		private void DoSelectCertifiedAeroDeviceFileCommand(JobFileType jobFileType)
		{ 
			SelectFile(jobFileType, "Invalid aero device xml", (result) => {
				CertifiedAeroDeviceFilePath = result;
			});
		}

		private void ValidationErrorAction(XmlSeverityType type, ValidationEvent validationEvent)
		{
			var xmlException = validationEvent?.ValidationEventArgs?.Exception as XmlSchemaValidationException;
			if (xmlException != null) {
				xmlFileErrors.Add(xmlException.Message);
			}
			return;
		}

		private bool OpenFileSelector(JobFileType jobFileType, string textPropertyName, string initPath, out string selectedFile)
		{
			selectedFile = null;
			var folderPath = GetFolderPath(initPath);

			var dialogResult = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.XMLFilter, folderPath);
			if (dialogResult != null) {

				var result = dialogResult.FirstOrDefault();
				
				RemovePropertyError(textPropertyName);
				selectedFile = result;
				return true;

				//TODO Enable xml validation on file open
				/*var validationResult = IsValidXml(jobFileType, filePath);

				if (!validationResult)
					AddPropertyError(textPropertyName, $"Selected XML-File is not a valid {jobFileType.GetLabel()}!");
				else
					RemovePropertyError(textPropertyName);

				return !validationResult ? null : filePath;*/
			}


			return false;
		}

		private string GetFolderPath(string filePath)
		{
			if (!_editJob || filePath.IsNullOrEmpty())
				return Settings.XmlFilePathFolder;
			
			if (IsFileName(filePath)) {
				return !JobEntry.JobEntryFilePath.IsNullOrEmpty()
					? Path.GetDirectoryName(JobEntry.JobEntryFilePath)
					: Path.GetDirectoryName(Settings.XmlFilePathFolder);
			}

			return filePath;
		}
		
		private bool IsFileName( string filePath)
		{
			return !Directory.Exists(filePath);
		}

	}

}
