using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Util.XML.Components;
using VECTO3GUI2020.Util.XML.Documents;
using VECTO3GUI2020.Util.XML.Vehicle;

namespace VECTO3GUI2020.Util.XML
{
    public interface IXMLWriterFactory
    {
        IXMLDeclarationJobWriter CreateJobWriter(IDeclarationJobInputData inputData);
        IXMLVehicleWriter CreateVehicleWriter(IVehicleDeclarationInputData inputData);



        //IXMLComponentWriter CreateComponentWriter(IComponentInputData inputData);
        //IXMLComponentWriter CreateComponentWriter(IAuxiliariesDeclarationInputData inputData);
        //IXMLComponentWriter CreateComponentWriter(IAxlesDeclarationInputData inputData);
        //IXMLComponentWriter CreateComponentWriter(IAxleDeclarationInputData inputData);


        //IXMLComponentWriter CreateComponentWriter(IPTOTransmissionInputData inputData);
        //IXMLComponentsWriter CreateComponentsWriter(IVehicleComponentsDeclaration inputData);

        IXMLComponentsWriter CreateComponentsWriterWithVersion(XNamespace ns, string xsdType,
            IVehicleComponentsDeclaration inputData);
		IXMLComponentsWriter CreateComponentsWriterWithVersion(XNamespace ns, string xsdType);
		//IXMLBusAuxiliariesWriter CreateBuxAuxiliariesWriter(IBusAuxiliariesDeclarationData inputData);


        #region Airdrag
		IXMLComponentWriter CreateComponentWriter(IAirdragDeclarationInputData inputData);
        #endregion

        #region BusAux

		IXMLBusAuxiliariesWriter CreateComponentWriter(IBusAuxiliariesDeclarationData inputData);
        #endregion BusAux

        TWriter CreateComponentWriterWithVersion<TWriter>(XNamespace ns, string xsdType);
	}

    public interface IXMLWriterFactoryInternal
    {
        TWriter CreateWriter<TWriter, TData>(DataSource source, TData inputData);
		TWriter CreateComponentsWriter<TWriter>(DataSource source);
		TWriter CreateComponentWriter<TWriter>(DataSource source);
	}

    public class XMLWriterFactory : IXMLWriterFactory
    {
        private IXMLWriterFactoryInternal _internalFactory;

        public XMLWriterFactory(IXMLWriterFactoryInternal internalFactory)
        {
            _internalFactory = internalFactory;
        }
        private TWriter CreateWriter<TWriter, TData>(DataSource source, TData inputData)
        {
            return _internalFactory.CreateWriter<TWriter, TData>(source, inputData);
        }




        #region Implementation of IXMLWriterFactory

        public IXMLDeclarationJobWriter CreateJobWriter(IDeclarationJobInputData inputData)
        {
            throw new NotImplementedException();
        }

        public IXMLVehicleWriter CreateVehicleWriter(IVehicleDeclarationInputData inputData)
        {
            return _internalFactory.CreateWriter<IXMLVehicleWriter, IVehicleDeclarationInputData>(inputData.DataSource, inputData);
            //throw new System.NotImplementedException();
        }

        public IXMLComponentWriter CreateComponentWriter(IComponentInputData inputData)
        {
            throw new NotImplementedException();
        }

        public IXMLComponentWriter CreateComponentWriter(IAuxiliariesDeclarationInputData inputData)
        {
            throw new NotImplementedException();
        }

        public IXMLComponentWriter CreateComponentWriter(IAxlesDeclarationInputData inputData)
        {
            throw new NotImplementedException();
        }

        public IXMLComponentWriter CreateComponentWriter(IAxleDeclarationInputData inputData)
        {
            throw new NotImplementedException();
        }

        public IXMLComponentWriter CreateComponentWriter(IAirdragDeclarationInputData inputData)
		{
			return _internalFactory.CreateComponentWriter<IXMLComponentWriter>(inputData.DataSource).Init(inputData);
		}

		public IXMLBusAuxiliariesWriter CreateComponentWriter(IBusAuxiliariesDeclarationData inputData)
		{
			if (inputData.DataSource == null) {
				throw new VectoException("No version specified in datasource", ErrorCodes.AUXILIARIES_MISSING_DATASOURCE);
			}
			return _internalFactory.CreateComponentWriter<IXMLBusAuxiliariesWriter>(inputData.DataSource).Init(inputData);
		}


		public TWriter CreateComponentWriterWithVersion<TWriter>(XNamespace ns, string xsdType)
		{
			return _internalFactory.CreateComponentWriter<TWriter>(new DataSource() {
				TypeVersion = ns.ToString(),
				Type = xsdType,
			});
		}

        public IXMLComponentWriter CreateComponentWriter(IPTOTransmissionInputData inputData)
        {
            throw new NotImplementedException();
        }

        public IXMLComponentsWriter CreateComponentsWriter(IVehicleComponentsDeclaration inputData)
        {
            throw new NotImplementedException();
        }

		public IXMLComponentsWriter CreateComponentsWriterWithVersion(XNamespace ns, string xsdType)
		{
			return _internalFactory.CreateComponentsWriter<IXMLComponentsWriter>(new DataSource()
			{
				TypeVersion = ns.ToString(),
				Type = xsdType,
			});
        }

		public IXMLComponentsWriter CreateComponentsWriterWithVersion(XNamespace ns, string xsdType,
            IVehicleComponentsDeclaration inputData)
		{
			return CreateComponentsWriterWithVersion(ns, xsdType).Init(inputData);
		}

		//public IXMLBusAuxiliariesWriter CreateBuxAuxiliariesWriter(IBusAuxiliariesDeclarationData inputData)
  //      {
  //          throw new NotImplementedException();
  //      }


		#endregion
    }
}
