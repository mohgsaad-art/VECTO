using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.GroupWriter;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration;
using VECTO3GUI2020.Resources.XML;

namespace VECTO3GUI2020.Util.XML.Components
{
    public interface IXMLBusAuxiliariesWriter
	{
		XElement GetElement();
		IXMLBusAuxiliariesWriter Init(IBusAuxiliariesDeclarationData inputData);
	}

    public abstract class XMLCompletedBusAuxiliariesWriter : IXMLBusAuxiliariesWriter
    {
        protected IBusAuxiliariesDeclarationData _inputData;
        protected XElement _xElement;
		protected readonly IGroupWriterFactory _groupWriterFactory;

		protected abstract string AuxType { get; }

		// todo amogoda: delete virtual property
		protected virtual XNamespace DefaultNamespace => XMLNamespaces.V27;

		protected XMLCompletedBusAuxiliariesWriter(IGroupWriterFactory groupWriterFactory)
		{
			_groupWriterFactory = groupWriterFactory;
		}

        public XElement GetElement()
        {
            if (_xElement == null)
            {
				if (_inputData == null) {
					throw new VectoException($"Writer not initialized call {nameof(Init)} first", ErrorCodes.AUXILIARIES_WRITER_NOT_INITIALIZED);
				}
                Initialize();
                CreateElements();
            }

            return _xElement;
        }

		public IXMLBusAuxiliariesWriter Init(IBusAuxiliariesDeclarationData inputData)
		{
			_inputData = inputData;
			return this;
		}

		public XElement GetElement(XNamespace wrapperNamespace)
        {
            throw new System.NotImplementedException();
        }

		protected void Initialize()
		{
			_xElement = new XElement(DefaultNamespace + XMLNames.Component_Auxiliaries);
        }

        public abstract void CreateElements();
    }



    public class XMLCompletedBusAuxiliariesWriterConventional : XMLCompletedBusAuxiliariesWriter
    {
		protected override string AuxType => XMLTypes.AUX_Conventional_CompletedBusType;

		public XMLCompletedBusAuxiliariesWriterConventional(IGroupWriterFactory groupWriterFactory) : base(groupWriterFactory)
        {

        }

        #region Overrides of XMLCompletedBusAuxiliariesWriter

		public override void CreateElements()
        {
            CreateElementsWithGroupWriters();
		}

        private void CreateElementsWithGroupWriters()
        {

            var dataElement = new XElement(DefaultNamespace + XMLNames.ComponentDataWrapper,
                new XAttribute("xmlns", XMLNamespaces.V27),
                new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, AuxType));

            if (_inputData.ElectricConsumers != null)
            {
                var electricSystemElement = new XElement(DefaultNamespace + XMLNames.BusAux_ElectricSystem);
                var ledLightsElement = new XElement(DefaultNamespace + "LEDLights");
                ledLightsElement.Add(
                    // ReSharper disable once CoVariantArrayConversion
                    _groupWriterFactory.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxElectricSystemLightsGroup, DefaultNamespace)
                        .GetGroupElements(_inputData));
                electricSystemElement.Add(ledLightsElement);
                dataElement.Add(electricSystemElement);

            }

            if (_inputData.HVACAux != null)
            {
                var hvacElement = new XElement(DefaultNamespace + "HVAC");
				hvacElement.Add(_groupWriterFactory
					.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxHVACConventionalSequenceGroup,
						DefaultNamespace)
					.GetGroupElements(_inputData));
				dataElement.Add(hvacElement);
            }

            _xElement.Add(dataElement);

        }

        private XElement GetHeatPumpTypeElement(string xmlName, string value)
        {
            if (value == "~null~")
            {
                value = null;
            }
            return new XElement(DefaultNamespace + xmlName, value);
        }

        private XElement GetHeatPumpGroupElement(string xmlNameWrapper, string xmlNameFirstComponent,
            string xmlNameSecondComponent, string firstValue, string secondValue)
        {
            var element = new XElement(DefaultNamespace + xmlNameWrapper,
                new XElement(DefaultNamespace + xmlNameFirstComponent, firstValue),
                new XElement(DefaultNamespace + xmlNameSecondComponent, secondValue));

            return element;
        }


        #endregion
    }


	public class XMLCompletedBusAuxiliariesWriter_xEV : XMLCompletedBusAuxiliariesWriter
	{
		public XMLCompletedBusAuxiliariesWriter_xEV(IGroupWriterFactory groupWriterFactory) : base(groupWriterFactory) { }
        protected override string AuxType => XMLTypes.AUX_xEV_CompletedBusType;

		public override void CreateElements()
		{
			CreateElementsWithGroupWriters();
		}

		private void CreateElementsWithGroupWriters()
		{

			var dataElement = new XElement(DefaultNamespace + XMLNames.ComponentDataWrapper,
				new XAttribute("xmlns", XMLNamespaces.V27),
				new XAttribute(XMLNamespaces.Xsi + XMLNames.Attr_Type, AuxType));

			if (_inputData.ElectricConsumers != null)
			{
				var electricSystemElement = new XElement(DefaultNamespace + XMLNames.BusAux_ElectricSystem);
				var ledLightsElement = new XElement(DefaultNamespace + XMLNames.BusAux_LEDLights);
				ledLightsElement.Add(
					// ReSharper disable once CoVariantArrayConversion
					_groupWriterFactory.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxElectricSystemLightsGroup, DefaultNamespace)
						.GetGroupElements(_inputData));
				electricSystemElement.Add(ledLightsElement);
				dataElement.Add(electricSystemElement);

			}

			if (_inputData.HVACAux != null)
			{
				var hvacElement = new XElement(DefaultNamespace + XMLNames.BusAux_HVAC);
				hvacElement.Add(_groupWriterFactory.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxHVACConventionalSequenceGroup, DefaultNamespace)
					.GetGroupElements(_inputData));
				hvacElement.Add(_groupWriterFactory.GetBusAuxiliariesDeclarationGroupWriter(GroupNames.BusAuxHVACxEVSequenceGroup, DefaultNamespace)
					.GetGroupElements(_inputData));
                dataElement.Add(hvacElement);
			}

			_xElement.Add(dataElement);
		}
	}
}