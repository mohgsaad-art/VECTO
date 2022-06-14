using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
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
using TUGraz.VectoHashing.Util;
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
		public CompleteTrailerViewModel(IXMLInputDataReader inputDataReader)
		{
			_inputDataReader = inputDataReader;
		}
		#region Members

		private ITrailerDeclarationInputData _trailer;
		private ITrailerDeclarationInputData _trailerObject;
		private TrailerComponentData _componentData;

		private string _manufacturer;
		private string _manufacturerAddress;
		private string _trailerModel;
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


		private string _certifiedAeroDeviceFilePath;

		private string _certifiedAeroDeviceXml;




		private ICommand _selectFirstFileCommand;

		private ICommand _selectCertifiedAeroDeviceCommand;

		private JobFileType _certifiedAeroDeviceType;

		private bool _certifiedAeroDeviceButtonsVisibility;
		private bool _certifiedAeroDeviceNewVisibility;

		private bool _tPMLMAxleAssemblyVisibility;
		private bool _showShowTrailerCouplingPoint;
		private TrailerCouplingPoint _trailerCouplingPoint;


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
				if (SetProperty(ref _externalLengthOfTheBody, value)) {
					IsDataChanged(_externalLengthOfTheBody, _componentData);
				}
				
			}
		}

		public Meter ExternalWidthOfTheBody
		{
			get => _externalWidthOfTheBody;
			set
			{
				if (SetProperty(ref _externalWidthOfTheBody, value)) {
					IsDataChanged(_externalWidthOfTheBody, _componentData);
				}
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
				LengthBetweenCentersOfAxles = ShowLengthBetweenCentersOfAxles ? LengthBetweenCentersOfAxles : null; //0.SI<Meter>();

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
				OnPropertyChanged(nameof(ShowTrailerCouplingPoint));

				if (value == TypeTrailer.DB) {
                    Axles[0].Steered = true;
					if (NumberOfAxles.ToInt() < 2) {
						NumberOfAxles = NumberOfTrailerAxles.Two;
					}
				}

				TPMLMAxleAssemblyVisibility = TrailerType != TypeTrailer.DB;

				

				if (TrailerType != TypeTrailer.DC) {
					TrailerCouplingPoint = TrailerCouplingPoint.Unknown;
				}

				if (TrailerType == TypeTrailer.DB) {
					//TPLMAxleAssembly = 0.SI<Kilogram>();
				}
					

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
			set
			{
				if (SetProperty(ref _showLengthBetweenCentersOfAxles, value)) {
					////HACK TO TRIGGER CONVERTER
					//var tmp = LengthBetweenCentersOfAxles;
					//LengthBetweenCentersOfAxles = 0.SI<Meter>();
					//LengthBetweenCentersOfAxles = tmp;
				}
			}
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


		private ObservableCollectionEx<TrailerAxleViewModel> _axles = new ObservableCollectionEx<TrailerAxleViewModel>() {
			new TrailerAxleViewModel(), new TrailerAxleViewModel(), new TrailerAxleViewModel(),
		};
		ICollection<TrailerAxleViewModel> ICompleteTrailer.Axles => Axles.Take(NumberOfAxles.ToInt()).ToList();

		public ObservableCollectionEx<TrailerAxleViewModel> Axles => _axles;

		private string _aeroDeviceInfo;
		public string AeroDeviceInfo
		{
			get => _aeroDeviceInfo;
			set => SetProperty(ref _aeroDeviceInfo, value);
		}

		public string CertifiedAeroDeviceFilePath
		{
			get => _certifiedAeroDeviceFilePath;
			set => SetProperty(ref _certifiedAeroDeviceFilePath, value);
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
			get => _trailerCouplingPoint;
			set
			{

				if (SetProperty(ref _trailerCouplingPoint, value)) {
					IsDataChanged(_trailerCouplingPoint, _componentData);
				}
			}
		}

		public XDocument SourceDocument { get; }

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

		public bool ShowTrailerCouplingPoint
		{
			get => TrailerType == TypeTrailer.DC;
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
			else {
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
				LengthBetweenCentersOfAxles = trailer.LengthBetweenCentersOfAxles == 0.SI<Meter>() ? null : trailer.LengthBetweenCentersOfAxles; //always saved as 0 in XML
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

				var xDoc = _trailer.XMLSource.OwnerDocument.ToXDocument();

				var count = 0;
				foreach (var trailerAxleDeclarationInputData in trailer.Axles) {
					EnableAxle(trailerAxleDeclarationInputData, xDoc, count++);
				}

				if (IsCertified)
                {
                    var device = (xDoc.XPathSelectElements($"//*[local-name()='{XMLNames.Trailer_CertifiedAeroDevice}']") ??

                                throw new Exception("Cannot parse aero device information")).ToList();
					
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

		private void EnableAxle(ITrailerAxleDeclarationInputData trailerAxleDeclarationInputData, XDocument sourceDoc,
			int count)
		{
			var tyre = trailerAxleDeclarationInputData.Tyre;
			var tyreDescription = CreateTyreDescription(tyre);
			var axle = _axles.ElementAt(count);
			axle.TwinTyres = trailerAxleDeclarationInputData.TwinTyres;
			axle.Liftable = trailerAxleDeclarationInputData.Liftable;
			axle.Steered = trailerAxleDeclarationInputData.Steered;
			axle.TyreInfo = tyreDescription;
			axle.SourceDoc = sourceDoc;

		}

		private static string CreateTyreDescription(ITyreDeclarationInputData tyre)
		{
			var tyreDescription = $"{tyre.Manufacturer}, {tyre.Dimension}";
			return tyreDescription;
		}



		private void EnableCertifiedAeroDevice(XElement device)
		{

			CertifiedAeroDeviceXml = device.ToString();

			var manufacturer = device.XPathSelectElement($"//*[local-name()='{XMLNames.Component_Manufacturer}']")?.Value;
			var model = device.XPathSelectElement($"//*[local-name()='{XMLNames.Component_Model}']")?.Value;


			CertifiedAeroDeviceFilePath = $"{manufacturer}, {model}";
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

		public JobFileType CertifiedAeroDeviceType
		{
			get => _certifiedAeroDeviceType;
			set => SetProperty(ref _certifiedAeroDeviceType, value);
		}


		public ICommand SelectAxleXmlCommand =>
			_selectFirstFileCommand ??
			(_selectFirstFileCommand = new RelayCommand<string>(DoSelectTyreCommand));

		public ICommand SelectCertifiedAeroDeviceCommand =>
			_selectCertifiedAeroDeviceCommand ??
			(_selectCertifiedAeroDeviceCommand = new RelayCommand<JobFileType>(DoSelectCertifiedAeroDeviceFileCommand));



		private void DoSelectTyreCommand(string count)
		{
			var axleIdx = int.Parse(count);
			if (OpenFileSelector(nameof(CertifiedAeroDeviceFilePath),
					CertifiedAeroDeviceFilePath, out var result))
			{
				var tyreXml = XDocument.Load(result);
				if (!XmlHelper.ValidateXDocument(tyreXml, validationErrorAction: ValidationErrorAction))
				{
					MessageBox.Show(caption: "Invalid Tyre XML",
						messageBoxText: string.Join("\n", xmlFileErrors),
						button: MessageBoxButton.OK,
						icon: MessageBoxImage.Error);
					xmlFileErrors.Clear();
				}
				else
				{
					if (!VectoHash.Load(result).ValidateHash())
					{
						MessageBox.Show(caption: "Invalid Hash",
							messageBoxText: $"Component file {result} has invalid hash\n",
							icon: MessageBoxImage.Warning,
							button: MessageBoxButton.OK);
					}

                    Axles[axleIdx].TyreInfo = 
					Axles[axleIdx].TyreXmlPath = result;

					var inputData = _inputDataReader.CreateComponent(tyreXml.CreateReader(ReaderOptions.None)) as ITyreDeclarationInputData;
					if (inputData == null) {
						MessageBox.Show(caption: "Invalid Tyre XML",
							messageBoxText: "Selected File is not a valid tyre XML",
							button: MessageBoxButton.OK,
							icon: MessageBoxImage.Error);
						xmlFileErrors.Clear();
						return;
					}
					Axles[axleIdx].TyreInfo = CreateTyreDescription(inputData);
				}

			}
		}

		private void SelectFile(JobFileType jobFileType, string errorCaption, Action<string> successAction, string propertyName)
		{
            if (OpenFileSelector(propertyName,
                    null, out var result))
            {
                var xDoc = XDocument.Load(result);
                if (!XmlHelper.ValidateXDocument(xDoc, validationErrorAction: ValidationErrorAction))
                {
                    MessageBox.Show(caption: errorCaption,
                        messageBoxText: string.Join("\n", xmlFileErrors),
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    xmlFileErrors.Clear();
                }
                else
                {
                    if (!VectoHash.Load(result).ValidateHash())
                    {
                        MessageBox.Show(caption: "Invalid Hash",
                            messageBoxText: $"Component file {result} has invalid hash\n",
                            icon: MessageBoxImage.Warning,
                            button: MessageBoxButton.OK);
                    }

                    successAction.Invoke(result);
				}

            }
        }

		private IList<string> xmlFileErrors = new List<string>();
		private readonly IXMLInputDataReader _inputDataReader;

		private void DoSelectCertifiedAeroDeviceFileCommand(JobFileType jobFileType)
		{ 
			SelectFile(jobFileType, "Invalid aero device xml", (result) => {
				CertifiedAeroDeviceFilePath = result;
			}, nameof(CertifiedAeroDeviceFilePath));
		}

		private void ValidationErrorAction(XmlSeverityType type, ValidationEvent validationEvent)
		{
			var xmlException = validationEvent?.ValidationEventArgs?.Exception as XmlSchemaValidationException;
			if (xmlException != null) {
				xmlFileErrors.Add(xmlException.Message);
			}
			return;
		}

		private bool OpenFileSelector(string textPropertyName, string initPath, out string selectedFile)
		{
			selectedFile = null;
			//var folderPath = GetFolderPath(initPath);

			var dialogResult = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.XMLFilter, null);
			if (dialogResult != null) {

				var result = dialogResult.FirstOrDefault();
				
				RemovePropertyError(textPropertyName);
				selectedFile = result;
				return true;
			}


			return false;
		}
	}
}
