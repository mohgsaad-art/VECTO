using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
    public class XMLDeclarationInputDataV27InjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IXMLAxleDeclarationInputData>().To<XMLDeclarationAxleDataProviderV27>().Named(
                XMLDeclarationAxleDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxleReader>().To<XMLComponentReaderV27>().Named(
                XMLComponentReaderV27.AXLE_READER_QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlesDeclarationInputData>().To<XMLDeclarationAxlesDataProviderV27>().Named(
                XMLDeclarationAxlesDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLAxlesReader>().To<XMLComponentReaderV27>().Named(
                XMLComponentReaderV27.AXLES_READER_QUALIFIED_XSD_TYPE);

            Bind<IXMLElectricStorageSystemDeclarationInputData>().To<XMLElectricStorageSystemDeclarationInputDataV27>()
                .Named(XMLElectricStorageSystemDeclarationInputDataV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLREESSReader>().To<XMLREESSReaderV27>().Named(XMLREESSReaderV27.QUALIFIED_XSD_TYPE);

            // IXMLVehicleComponentsDeclaration
            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHEVPxLorryComponentsDataProviderV27>()
                .Named(XMLDeclarationHEVPxLorryComponentsDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV27>()
                .Named(XMLDeclarationPrimaryBusHEVPxComponentsDataProviderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPEVE2ComponentDataV27>()
                .Named(XMLDeclarationHeavyLorryPEVE2ComponentDataV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPevExComponentDataV27>()
                .Named(XMLDeclarationHeavyLorryPevExComponentDataV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationHeavyLorryPevExComponentDataV27>()
                .Named(XMLDeclarationHeavyLorryPevExComponentDataV27.QUALIFIED_XSD_PEV_E4_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPEVE2ComponentDataV27>()
                .Named(XMLDeclarationPrimaryBusPEVE2ComponentDataV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPevExComponentDataV27>()
                .Named(XMLDeclarationPrimaryBusPevExComponentDataV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationPrimaryBusPevExComponentDataV27>()
                .Named(XMLDeclarationPrimaryBusPevExComponentDataV27.QUALIFIED_XSD_PEV_E4_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationIEPCHeavyLorryComponentDataV27>()
                .Named(XMLDeclarationIEPCHeavyLorryComponentDataV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationIEPCPrimaryBusComponentDataV27>()
                .Named(XMLDeclarationIEPCPrimaryBusComponentDataV27.QUALIFIED_XSD_TYPE);

            // IXMLComponentReader
            Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE4DeclarationComponentReaderV27>()
                .Named(XMLPEVHeavyLorryE4DeclarationComponentReaderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLComponentReaderV27_HEV_PxHeavyLorry>()
                .Named(XMLComponentReaderV27_HEV_PxHeavyLorry.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLComponentReaderV27_HEV_PxHeavyLorry>()
                .Named(XMLComponentReaderV27_HEV_PxHeavyLorry.QUALIFIED_XSD_HEV_Px_TYPE);

            Bind<IXMLComponentReader>().To<XMLPrimaryBusHEVPxDeclarationComponentReaderV27>()
                .Named(XMLPrimaryBusHEVPxDeclarationComponentReaderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLIEPCHeavyLorryComponentReaderV27>()
                .Named(XMLIEPCHeavyLorryComponentReaderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLIEPCPrimaryBusComponentReaderV27>()
                .Named(XMLIEPCPrimaryBusComponentReaderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE2DeclarationComponentReaderV27>()
                .Named(XMLPEVHeavyLorryE2DeclarationComponentReaderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE2DeclarationComponentReaderV27>()
                .Named(XMLPEVPrimaryBusE2DeclarationComponentReaderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLPEVHeavyLorryE3DeclarationComponentReaderV27>()
                .Named(XMLPEVHeavyLorryE3DeclarationComponentReaderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE3DeclarationComponentReaderV27>()
                .Named(XMLPEVPrimaryBusE3DeclarationComponentReaderV27.QUALIFIED_XSD_TYPE);

            Bind<IXMLComponentReader>().To<XMLPEVPrimaryBusE4DeclarationComponentReaderV27>()
                .Named(XMLPEVPrimaryBusE4DeclarationComponentReaderV27.QUALIFIED_XSD_TYPE);
        }
    }
}
