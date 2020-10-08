using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
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
		private TrailerComponentData _componentData;

		private string _manufacturer;
		private string _manufacturerAddress;
		private string _model;
		private string _vin;
		private DateTime _date;
		private string _legislativeCategory;
		private Meter _externalLengthOfTheBody;
		private Meter _externalWidthOfTheBody;
		private Meter _externalHeightOfTheBody;
		private Meter _totalHeightOfTheTrailer;
		private Meter _positionXFirstAxle;
		private Meter _positionXLastAxle;
		private Meter _internalHeightOfTheBody;
		private CubicMeter _cargoVolume;
		private NumberOfTrailerAxles _numberOfAxles;
		private TypeTrailer _trailerType;
		private BodyWorkCode _bodyCode;
		private bool _sideSkirtsShort;
		private bool _sideSkirtsLong;
		private bool _boatTailShort;
		private bool _boatTailLong;
		private Kilogram _curbMassTrailer;
		private Kilogram _tPLMTotalTrailer;
		private Kilogram _tPLMAxleAssembly;
		private string _axle1FilePath;
		private string _axle2FilePath;
		private string _axle3FilePath;
		private string _axle1Xml;
		private string _axle2Xml;
		private string _axle3Xml;
		private bool _axle1IsTwin;
		private bool _axle2IsTwin;
		private bool _axle3IsTwin;
		private ICommand _selectFirstFileCommand;
		private JobFileType _firstFileType;

		private string _firstFilePath;
		private readonly bool _editJob;
		protected SettingsModel Settings = new SettingsModel();
		protected JobEntry JobEntry;

		#endregion

		#region ICompleteTrailerViewModel

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

		public ITrailerDeclarationInputData ModelData
		{
			get { return AdapterFactory.TrailerDeclarationAdapter(this); }
		}

		public string Manufacturer
		{
			get { return _manufacturer; }
			set
			{
				if (!SetProperty(ref _manufacturer, value))
					return;
				IsDataChanged(_manufacturer, _componentData);
			}
		}
		public string ManufacturerAddress
		{
			get { return _manufacturerAddress; }
			set
			{
				if (!SetProperty(ref _manufacturerAddress, value))
					return;
				IsDataChanged(_manufacturerAddress, _componentData);
			}
		}
		public string Model
		{
			get { return _model; }
			set
			{
				if (!SetProperty(ref _model, value))
					return;

				IsDataChanged(_model, _componentData);
			}
		}
		public string VIN
		{
			get { return _vin; }
			set
			{
				if (!SetProperty(ref _vin, value))
					return;
				IsDataChanged(_vin, _componentData);
			}
		}
		public DateTime Date
		{
			get { return _date; }
			set
			{
				if (!SetProperty(ref _date, value))
					return;
				IsDataChanged(_date, _componentData);
			}
		}
		public string LegislativeCategory
		{
			get { return _legislativeCategory; }
			set
			{
				if (!SetProperty(ref _legislativeCategory, value))
					return;
				IsDataChanged(_legislativeCategory, _componentData);
			}
		}

		public Meter ExternalLengthOfTheBody
		{
			get { return _externalLengthOfTheBody; }
			set
			{
				if (!SetProperty(ref _externalLengthOfTheBody, value))
					return;
				IsDataChanged(_externalLengthOfTheBody, _componentData);
			}
		}

		public Meter ExternalWidthOfTheBody
		{
			get { return _externalWidthOfTheBody; }
			set
			{
				if (!SetProperty(ref _externalWidthOfTheBody, value))
					return;
				IsDataChanged(_externalWidthOfTheBody, _componentData);
			}
		}

		public Meter ExternalHeightOfTheBody
		{
			get { return _externalHeightOfTheBody; }
			set
			{
				if (!SetProperty(ref _externalHeightOfTheBody, value))
					return;
				IsDataChanged(_externalHeightOfTheBody, _componentData);
			}
		}

		public Meter TotalHeightOfTheTrailer
		{
			get { return _totalHeightOfTheTrailer; }
			set
			{
				if (!SetProperty(ref _totalHeightOfTheTrailer, value))
					return;
				IsDataChanged(_totalHeightOfTheTrailer, _componentData);
			}
		}

		public Meter PositionXFirstAxle
		{
			get { return _positionXFirstAxle; }
			set
			{
				if (!SetProperty(ref _positionXFirstAxle, value))
					return;
				IsDataChanged(_positionXFirstAxle, _componentData);
			}
		}

		public Meter PositionXLastAxle
		{
			get { return _positionXLastAxle; }
			set
			{
				if (!SetProperty(ref _positionXLastAxle, value))
					return;
				IsDataChanged(_positionXLastAxle, _componentData);
			}
		}

		public Meter InternalHeightOfTheBody
		{
			get { return _internalHeightOfTheBody; }
			set
			{
				if (!SetProperty(ref _internalHeightOfTheBody, value))
					return;
				IsDataChanged(_internalHeightOfTheBody, _componentData);
			}
		}

		public CubicMeter CargoVolume
		{
			get { return _cargoVolume; }
			set
			{
				if (!SetProperty(ref _cargoVolume, value))
					return;
				IsDataChanged(_cargoVolume, _componentData);
			}
		}

		public NumberOfTrailerAxles NumberOfAxles
		{
			get { return _numberOfAxles; }
			set
			{
				if (!SetProperty(ref _numberOfAxles, value))
					return;
				IsDataChanged(_numberOfAxles, _componentData);
			}
		}

		public TypeTrailer TrailerType
		{
			get { return _trailerType; }
			set
			{
				if (!SetProperty(ref _trailerType, value))
					return;
				IsDataChanged(_trailerType, _componentData);
			}
		}

		public BodyWorkCode BodyCode
		{
			get { return _bodyCode; }
			set
			{
				if (!SetProperty(ref _bodyCode, value))
					return;
				IsDataChanged(_bodyCode, _componentData);
			}
		}
		public bool SideSkirtsShort
		{
			get { return _sideSkirtsShort; }
			set { SetProperty(ref _sideSkirtsShort, value); }
		}
		public bool SideSkirtsLong
		{
			get { return _sideSkirtsLong; }
			set { SetProperty(ref _sideSkirtsLong, value); }
		}
		public bool BoatTailShort
		{
			get { return _boatTailShort; }
			set { SetProperty(ref _boatTailShort, value); }
		}
		public bool BoatTailLong
		{
			get { return _boatTailLong; }
			set { SetProperty(ref _boatTailLong, value); }
		}

		public Kilogram CurbMassTrailer
		{
			get { return _curbMassTrailer; }
			set
			{
				if (!SetProperty(ref _curbMassTrailer, value))
					return;
				IsDataChanged(_curbMassTrailer, _componentData);
			}
		}
		public Kilogram TPLMTotalTrailer
		{
			get { return _tPLMTotalTrailer; }
			set
			{
				if (!SetProperty(ref _tPLMTotalTrailer, value))
					return;
				IsDataChanged(_tPLMTotalTrailer, _componentData);
			}
		}
		public Kilogram TPLMAxleAssembly
		{
			get { return _tPLMAxleAssembly; }
			set
			{
				if (!SetProperty(ref _tPLMAxleAssembly, value))
					return;
				IsDataChanged(_tPLMAxleAssembly, _componentData);
			}
		}

		public string Axle1FilePath
		{
			get { return _axle1FilePath; }
			set
			{
				if (!SetProperty(ref _axle1FilePath, value))
					return;
				IsDataChanged(_axle1FilePath, _componentData);
			}
		}

		public string Axle2FilePath
		{
			get { return _axle2FilePath; }
			set
			{
				if (!SetProperty(ref _axle2FilePath, value))
					return;
				IsDataChanged(_axle2FilePath, _componentData);
			}
		}


		public string Axle3FilePath
		{
			get { return _axle3FilePath; }
			set
			{
				if (!SetProperty(ref _axle3FilePath, value))
					return;
				IsDataChanged(_axle3FilePath, _componentData);
			}
		}
		public string Axle1Xml
		{
			get { return _axle1Xml; }
			set
			{
				if (!SetProperty(ref _axle1Xml, value))
					return;
				IsDataChanged(_axle1Xml, _componentData);
			}
		}
		public string Axle2Xml
		{
			get { return _axle2Xml; }
			set
			{
				if (!SetProperty(ref _axle2Xml, value))
					return;
				IsDataChanged(_axle2Xml, _componentData);
			}
		}
		public string Axle3Xml
		{
			get { return _axle3Xml; }
			set
			{
				if (!SetProperty(ref _axle3Xml, value))
					return;
				IsDataChanged(_axle3Xml, _componentData);
			}
		}
		public bool Axle1IsTwin
		{
			get { return _axle1IsTwin; }
			set
			{
				if (!SetProperty(ref _axle1IsTwin, value))
					return;
				IsDataChanged(_axle1IsTwin, _componentData);
			}
		}
		public bool Axle2IsTwin
		{
			get { return _axle2IsTwin; }
			set
			{
				if (!SetProperty(ref _axle2IsTwin, value))
					return;
				IsDataChanged(_axle2IsTwin, _componentData);
			}
		}
		public bool Axle3IsTwin
		{
			get { return _axle3IsTwin; }
			set
			{
				if (!SetProperty(ref _axle3IsTwin, value))
					return;
				IsDataChanged(_axle3IsTwin, _componentData);
			}
		}
		
		public Dictionary<string, string> XmlNamesToPropertyMapping { get; private set; }
		public AllowedEntry<TypeTrailer>[] AllowedTrailerTypes { get; private set; }
		public AllowedEntry<BodyWorkCode>[] AllowedBodyCodes { get; private set; }
		public  AllowedEntry<AeroFeatureTechnologies>[] AllowedTechnologies { get; private set; }
		public AllowedEntry<NumberOfTrailerAxles>[] AllowedNumberOfAxles { get; private set; }

		#endregion


		#region Set XML Data

		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationTrailerInputDataProvider;
			_trailer = inputData?.JobInputData.Trailer;
			SetVehicleValues(_trailer);
			SetAllowedEntries();
			XmlNamesToPropertyMapping = _componentData.XmlNamesToPropertyMapping;

		}

		private void SetVehicleValues(ITrailerDeclarationInputData vehicle)
		{
			if (vehicle == null)
			{
				_componentData = new TrailerComponentData(this, true);
			}
			else
			{
				Manufacturer = vehicle.Manufacturer;
				ManufacturerAddress = vehicle.ManufacturerAddress;
				Model = vehicle.Model;
				VIN = vehicle.VIN;
				Date = vehicle.Date;
				LegislativeCategory = vehicle.LegislativeCategory;
				NumberOfAxles = vehicle.NumberOfAxles;
				TrailerType = vehicle.TrailerType;
				BodyCode = vehicle.BodyworkCode;
				CurbMassTrailer = vehicle.CurbMass;
				TPLMTotalTrailer = vehicle.TPMLMTotalTrailer;
				ExternalLengthOfTheBody = vehicle.ExternalBodyLength;
				ExternalWidthOfTheBody = vehicle.ExternalBodyWidth;
				ExternalHeightOfTheBody = vehicle.ExternalBodyHeight;
				TotalHeightOfTheTrailer = vehicle.TotalTrailerHeight;
				PositionXFirstAxle = vehicle.LengthTrailerFrontToFirstAxleCenter;
				PositionXLastAxle = vehicle.LengthTrailerFrontToLastAxleCenter;
				InternalHeightOfTheBody = vehicle.InternalBodyHeight;
				CargoVolume = vehicle.CargoVolume;

				_componentData = new TrailerComponentData(this);
			}

			ClearChangedProperties();
		}

		private void SetAllowedEntries()
		{

			AllowedTrailerTypes = EnumHelper.GetValues<TypeTrailer>().Where(x => x != TypeTrailer.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedBodyCodes = EnumHelper.GetValues<BodyWorkCode>().Where(x => x != BodyWorkCode.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedTechnologies = EnumHelper.GetValues<AeroFeatureTechnologies>().Where(x => x != AeroFeatureTechnologies.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();

			AllowedNumberOfAxles = EnumHelper.GetValues<NumberOfTrailerAxles>().Where(x => x != NumberOfTrailerAxles.Unknown)
				.Select(vc => AllowedEntry.Create(vc, vc.GetLabel())).ToArray();
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
			get { return _firstFileType; }
			set { SetProperty(ref _firstFileType, value); }
		}

		public string FirstFilePath
		{
			get { return _firstFilePath; }
			set { SetProperty(ref _firstFilePath, value); }
		}
		public ICommand SelectFirstFileCommand
		{
			get
			{
				return _selectFirstFileCommand ??
						(_selectFirstFileCommand = new RelayCommand<JobFileType>(DoSelectFirstFileCommand));
			}
		}
		private void DoSelectFirstFileCommand(JobFileType jobFileType)
		{
			FirstFilePath = OpenFileSelector(jobFileType, nameof(FirstFilePath), FirstFilePath);
		}
		private string OpenFileSelector(JobFileType jobFileType, string textPropertyName, string filePath)
		{
			var folderPath = GetFolderPath(filePath);

			var dialogResult = FileDialogHelper.ShowSelectFilesDialog(false, FileDialogHelper.XMLFilter, folderPath);
			if (dialogResult != null) {

				filePath = dialogResult.FirstOrDefault();
				var validationResult = IsValideXml(jobFileType, filePath);

				if (!validationResult)
					AddPropertyError(textPropertyName, $"Selected XML-File is not a valid {jobFileType.GetLable()}!");
				else
					RemovePropertyError(textPropertyName);

				return !validationResult ? null : filePath;
			}
			
			return filePath;
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
		
		private bool IsValideXml(JobFileType jobFileType, string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return false;

			var xmlInputReader = Kernel.Get<IXMLInputDataReader>();

			using (var reader = XmlReader.Create(filePath))
			{

				var readerResult = xmlInputReader.Create(reader); // Create -> CreateDeclarationTrailer
				if (readerResult is IDeclarationTrailerInputDataProvider)
				{

					var inputData = readerResult as IDeclarationTrailerInputDataProvider;
					if (jobFileType == JobFileType.CompletedTrailerFile &&
						inputData.JobInputData.Trailer is XMLDeclarationVehicleDataProviderV27)
					{
						return true;
					}
				}
				return false;
			}
		}

	}

}
