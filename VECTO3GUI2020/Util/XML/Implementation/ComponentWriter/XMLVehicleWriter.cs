using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.OutputData.XML.ComponentWriter;
using TUGraz.VectoCore.OutputData.XML.GroupWriter;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
    public abstract class XMLVehicleWriter : IXMLVehicleWriter
    {



        #region XML
		protected XElement _Xelement;

		protected XNamespace _defaultNamespace;
		#endregion

        //Template Methods
        protected abstract void Initialize();
		protected abstract void CreateElements();

        protected readonly IVehicleDeclarationInputData _inputData;
		protected IXMLWriterFactory _xmlWriterFactory;

		public XMLVehicleWriter(IVehicleDeclarationInputData inputData, IXMLWriterFactory xmlWriterFactory)
        {
            Debug.Assert(inputData != null);
            this._inputData = inputData;
			_xmlWriterFactory = xmlWriterFactory;
		}



		public XElement GetElement()
        {
			if (_Xelement == null) {
				Initialize();
				CreateElements();
			}
			return _Xelement;
        }
	}
	public  class XMLVehicleWriter_v1_0 : XMLVehicleWriter
	{
		public static readonly string[] SUPPORTEDVERSIONS = {
			typeof(XMLDeclarationVehicleDataProviderV10).ToString(),
            typeof(VehicleViewModel_v1_0).ToString()
		};
		
		public XMLVehicleWriter_v1_0(IVehicleDeclarationInputData inputData, IXMLWriterFactory xmlWriterFactory) : base(inputData, xmlWriterFactory)
        {
            
        }

		protected override void CreateElements()
		{
			throw new NotImplementedException();
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer),
				new XElement(_defaultNamespace + XMLNames.Component_ManufacturerAddress,
					_inputData.ManufacturerAddress),
				new XElement(_defaultNamespace + XMLNames.Component_Model, _inputData.Model),
				new XElement(_defaultNamespace + XMLNames.Vehicle_VIN, _inputData.VIN),
				new XElement(_defaultNamespace + XMLNames.Component_Date, _inputData.Date),
				new XElement(_defaultNamespace + XMLNames.Vehicle_LegislativeClass, _inputData.LegislativeClass),
				new XElement(_defaultNamespace + XMLNames.Vehicle_AxleConfiguration,
					AxleConfigurationHelper.ToXMLFormat(_inputData.AxleConfiguration)),
				new XElement(_defaultNamespace + XMLNames.Vehicle_CurbMassChassis,
					_inputData.CurbMassChassis.ToXMLFormat()),
				new XElement(_defaultNamespace + XMLNames.Vehicle_GrossVehicleMass,
					_inputData.GrossVehicleMassRating.ToXMLFormat()),
				new XElement(_defaultNamespace + XMLNames.Vehicle_IdlingSpeed,
					_inputData.EngineIdleSpeed.ToXMLFormat()));

			//new XElement(_defaultNamespace + XMLNames.Vehicle_RetarderType, _inputData.RetarderType.ToXMLFormat()),

			//_inputData.RetarderRatio == null ? null : new XElement(_defaultNamespace + XMLNames.Vehicle_RetarderRatio, _inputData.RetarderRatio),

			//new XElement(_defaultNamespace + XMLNames.Vehicle_AngledriveType, _inputData.AngledriveType.ToXMLFormat()),


			//https://stackoverflow.com/questions/24743916/how-to-convert-xmlnode-into-xelement
			//Remove this when PTOType is handled correct.
			//XElement.Load(_inputData.PTONode.CreateNavigator().ReadSubtree()),

			//new XElement(_defaultNamespace + XMLNames.Vehicle_Components, 
			//  new XAttribute(_xsi + "type", ComponentsXSD))


			//);


		}

		protected override void Initialize()
        {
            throw new NotImplementedException();
		}
    }


    public class XMLVehicleWriter_v2_0 : XMLVehicleWriter_v1_0
    {
		public new static readonly string[] SUPPORTEDVERSIONS = {
			typeof(XMLDeclarationVehicleDataProviderV10).ToString(),
            typeof(VehicleViewModel_v2_0).ToString()
		};

		public XMLVehicleWriter_v2_0(IVehicleDeclarationInputData inputData,
			IXMLWriterFactory xmlWriterFactory) : base(inputData, xmlWriterFactory)
        {


        }

        protected override void CreateElements()
		{
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_ManufacturerAddress,
				_inputData.ManufacturerAddress));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_Model, _inputData.Model));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_VIN, _inputData.VIN));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_Date, _inputData.Date));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_LegislativeClass,
				_inputData.LegislativeClass));
            _Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_VehicleCategory, _inputData.VehicleCategory));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_AxleConfiguration,
				_inputData.AxleConfiguration.ToXMLFormat()));

            _Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_CurbMassChassis, _inputData.CurbMassChassis.ToXMLFormat(0)));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_GrossVehicleMass,
				_inputData.GrossVehicleMassRating.ToXMLFormat(0)));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_IdlingSpeed,
				_inputData.EngineIdleSpeed.AsRPM.ToXMLFormat(0)));

            //TODO: Remove when IVehicleDeclarationInputData is updated
			if (_inputData is IVehicleViewModel viewModelInputData) {
				_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_RetarderType,
					viewModelInputData.RetarderType.ToXMLFormat()));
				_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_RetarderRatio, viewModelInputData.RetarderRatio.ToXMLFormat(3)));
				_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_AngledriveType,
					viewModelInputData.AngledriveType.ToXMLFormat()));
				_Xelement.Add(_xmlWriterFactory.CreateComponentWriter(viewModelInputData.PTOTransmissionInputData)
					.GetElement());
			} else {
				_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_RetarderType,
					RetarderType.None.ToXMLFormat()));
				//_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_RetarderRatio, "1.000")); 
				_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_AngledriveType,
					AngledriveType.None.ToXMLFormat()));
				_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_PTO, 
					new XElement(_defaultNamespace + XMLNames.Vehicle_PTO_ShaftsGearWheels, "none"), 
					new XElement(_defaultNamespace + XMLNames.Vehicle_PTO_OtherElements, "none")));
			}

			

			_Xelement.Add(_xmlWriterFactory.CreateComponentsWriter(_inputData.Components).GetComponents());
		}

		protected override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V20;
			_Xelement = new XElement(_defaultNamespace + XMLNames.Component_Vehicle);
			_Xelement.Add(new XAttribute(XMLNames.Component_ID_Attr, _inputData.Identifier));
			_Xelement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, XMLNames.VehicleAttr_VehicleDeclarationType));
		}
    }

    public class XMLVehicleWriter_v2_1 { }

	public class XMLVehicleWriter_v2_7 { }

	public class XMLVehicleWriter_PrimaryBus_v2_6 {}
	
    public class XMLVehicleWriter_ExcemptedVehicle_v2_2 { }


	public class XMLVehicleWriter_v2_10 : XMLVehicleWriter
	{
		private readonly bool _exempted;
		public static readonly string[] SUPPORTEDVERSIONS = {
			typeof(InterimStageBusVehicleViewModel_v2_8).ToString(),
			typeof(StageInputViewModel).ToString()
		};

		private readonly IGroupWriterFactory _groupWriterFactory;
		private readonly bool _conventional;
		private readonly IComponentWriterFactory _componentWriterFactory;

		public XMLVehicleWriter_v2_10(IVehicleDeclarationInputData inputData, 
			IXMLWriterFactory xmlWriterFactory, 
			IGroupWriterFactory groupWriterFactory, 
			IComponentWriterFactory componentWriterFactory) : base(inputData, xmlWriterFactory)
		{
			
			//TODO: CHECK ALL POSSIBIBILITIES FOR VEHICLES
			_exempted = inputData.ExemptedVehicle;

			_conventional = !_exempted;
			_groupWriterFactory = groupWriterFactory;
			_componentWriterFactory = componentWriterFactory;
		}

		#region Overrides of XMLVehicleWriter

		protected override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.v2_10_2;

			_Xelement = new XElement(XMLNamespaces.V20 + XMLNames.Component_Vehicle, 
				new XAttribute("xmlns",  _defaultNamespace));
			
			_Xelement.Add(new XAttribute(XMLNames.Component_ID_Attr, _inputData.Identifier ?? ("VEH-" + Guid.NewGuid().ToString("n").Substring(0, 20))));
			if (_conventional) {
				_Xelement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, "Vehicle_Conventional_CompletedBusDeclarationType"));
			} else if(_exempted) {
				_Xelement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, "Vehicle_Exempted_CompletedBusDeclarationType"));
			}
			
		}

		protected override void CreateElements()
		{
			if (_conventional) {
				CreateConventionalElements();
				return;
			}

			if (_exempted) {
				CreateExemptedElements();
				return;
			}

			throw new NotImplementedException("Old Implementation");
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_ManufacturerAddress,
				_inputData.ManufacturerAddress));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_VIN, _inputData.VIN));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_Date, XMLExtension.ToXmlFormat(DateTime.Today)));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Component_Model, _inputData.Model));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_LegislativeCategory, _inputData.LegislativeClass.ToXMLFormat()));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.CorrectedActualMass, _inputData.CurbMassChassis?.ToXMLFormat(0)));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_TPMLM,
				_inputData.GrossVehicleMassRating?.ToXMLFormat(0)));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_AirdragModifiedMultistep, _inputData.AirdragModifiedMultistep));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_NgTankSystem, _inputData.TankSystem));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_RegisteredClass, _inputData.RegisteredClass.ToXMLFormat()));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_NumberPassengerSeatsLowerDeck, _inputData.NumberPassengerSeatsLowerDeck));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_NumberPassengersStandingLowerDeck, _inputData.NumberPassengersStandingLowerDeck));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_NumberPassengerSeatsUpperDeck, _inputData.NumberPassengerSeatsUpperDeck));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_NumberPassengersStandingUpperDeck, _inputData.NumberPassengersStandingUpperDeck));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode, _inputData.VehicleCode.ToXMLFormat()));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_HeightIntegratedBody, _inputData.Height?.ConvertToMilliMeter()));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleLength, _inputData.Length?.ConvertToMilliMeter()));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleWidth, _inputData.Width?.ConvertToMilliMeter()));
			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_EntranceHeight, _inputData.EntranceHeight?.ConvertToMilliMeter()));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology, 
				_inputData.DoorDriveTechnology != null 
					? _inputData.DoorDriveTechnology.ToXMLFormat()
					: null));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleDeclarationType, _inputData.VehicleDeclarationType));

			if (_inputData.ADAS != null) {
				var adasNamespace = XMLNamespaces.V23;
				var aDASElement = new XElement(_defaultNamespace + XMLNames.Vehicle_ADAS);
				_Xelement.Add(aDASElement);
				aDASElement.Add(new XElement(adasNamespace + XMLNames.Vehicle_ADAS_EngineStopStart, _inputData.ADAS.EngineStopStart));

				bool ecoRollWithoutEngineStop = _inputData.ADAS.EcoRoll == EcoRollType.WithoutEngineStop;
				bool ecoRollWithEngineStop = _inputData.ADAS.EcoRoll == EcoRollType.WithEngineStop;

				aDASElement.Add(new XElement(adasNamespace + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, ecoRollWithoutEngineStop));
				aDASElement.Add(new XElement(adasNamespace + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, ecoRollWithEngineStop));
				aDASElement.Add(new XElement(adasNamespace + XMLNames.Vehicle_ADAS_PCC,
					_inputData.ADAS.PredictiveCruiseControl.ToXMLFormat()));
				aDASElement.Add(new XElement(adasNamespace + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch, _inputData.ADAS?.ATEcoRollReleaseLockupClutch ));
			}
			_Xelement.DescendantsAndSelf().Where(e => string.IsNullOrEmpty(e.Value)).Remove();

			if (_inputData.Components != null) {
				var componentElement = new XElement(
					_defaultNamespace + XMLNames.Vehicle_Components,
					new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type,
						"CompletedVehicleComponentsDeclarationType"));

				//Airdrag
				if (_inputData.Components.AirdragInputData != null) {
					var airDragElement = _xmlWriterFactory.CreateComponentWriter(_inputData.Components.AirdragInputData)
						.GetElement();
					var tempAirDragElement = new XElement(_defaultNamespace + XMLNames.Component_AirDrag);

					airDragElement.Name = tempAirDragElement.Name;
					componentElement.Add(airDragElement);
				}

				//auxiliaries
				if (_inputData.Components.BusAuxiliaries != null) {
					var auxiliaryElement = _xmlWriterFactory.CreateBuxAuxiliariesWriter(_inputData.Components.BusAuxiliaries)
						.GetElement();
					
					componentElement.Add(auxiliaryElement);

				}
				_Xelement.Add(componentElement);

			}
		}

		private void CreateConventionalElements()
		{
			_Xelement.Add(
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, _defaultNamespace).GetGroupElements(_inputData),
				_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBusParametersSequenceGroup, _defaultNamespace).GetGroupElements(_inputData)
			);
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_NgTankSystem, _inputData.TankSystem));

			// ReSharper disable once CoVariantArrayConversion
			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
				GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, _defaultNamespace).GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode, _inputData.VehicleCode.ToXMLFormat()));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));


			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(
					GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, _defaultNamespace)
				.GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology,
			_inputData.DoorDriveTechnology != null
				? _inputData.DoorDriveTechnology.ToXMLFormat()
				: null));

			_Xelement.Add(new XElement(_defaultNamespace + XMLNames.Bus_VehicleDeclarationType, _inputData.VehicleDeclarationType));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.VehicleTypeApprovalNumber, _inputData.VehicleTypeApprovalNumber));

			if (_inputData.ADAS != null) {
				_Xelement.Add(_componentWriterFactory.getDeclarationAdasWriter(GroupNames.ADAS_Conventional_Type, _defaultNamespace).GetComponent(_inputData.ADAS));
			}


			if (_inputData.Components != null)
			{
				var componentElement = new XElement(
					_defaultNamespace + XMLNames.Vehicle_Components,
					new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type,
						GUILabels.Components_Conventional_CompletedBusType));

				//Airdrag
				if (_inputData.Components.AirdragInputData != null)
				{
					var airDragElement = _xmlWriterFactory.CreateComponentWriter(_inputData.Components.AirdragInputData)
						.GetElement();
					var tempAirDragElement = new XElement(_defaultNamespace + XMLNames.Component_AirDrag);

					airDragElement.Name = tempAirDragElement.Name;
					componentElement.Add(airDragElement);
				}

				//auxiliaries
				if (_inputData.Components.BusAuxiliaries != null)
				{
					var auxiliaryElement = _xmlWriterFactory.CreateBuxAuxiliariesWriter(_inputData.Components.BusAuxiliaries)
						.GetElement();

					componentElement.Add(auxiliaryElement);

				}
				_Xelement.Add(componentElement);

			}

		}


		private void CreateExemptedElements()
		{


			// ReSharper disable once CoVariantArrayConversion
			_Xelement.Add(_groupWriterFactory
				.GetVehicleDeclarationGroupWriter(GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup,
					_defaultNamespace).GetGroupElements(_inputData));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Component_Model, _inputData.Model));
            _Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_LegislativeCategory, _inputData.LegislativeClass.ToXMLFormat()));
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.CorrectedActualMass, _inputData.CurbMassChassis?.ToXMLFormat(0)));
            _Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_TPMLM,
                _inputData.GrossVehicleMassRating?.ToXMLFormat(0)));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_RegisteredClass, _inputData.RegisteredClass.ToXMLFormat()));

			_Xelement.Add(_groupWriterFactory.GetVehicleDeclarationGroupWriter(GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, _defaultNamespace).GetGroupElements(_inputData));
			
			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Vehicle_BodyworkCode, _inputData.VehicleCode.ToXMLFormat()));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_LowEntry, _inputData.LowEntry));

			_Xelement.AddIfContentNotNull(new XElement(_defaultNamespace + XMLNames.Bus_HeightIntegratedBody, _inputData.Height?.ConvertToMilliMeter()));
		}

		#endregion
	}

}
