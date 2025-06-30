using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
    public class XMLDeclarationInputDataV26InjectModule : NinjectModule
    {
        public override void Load()
        {    
            Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataStandardV26>()
                .Named(XMLBatteryPackDeclarationInputDataStandardV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLBatteryPackDeclarationInputData>().To<XMLBatteryPackDeclarationInputDataMeasuredV26>()
                .Named(XMLBatteryPackDeclarationInputDataMeasuredV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLSuperCapDeclarationInputData>().To<XMLSuperCapDeclarationInputDataV26>()
                .Named(XMLSuperCapDeclarationInputDataV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCInputDataProviderV26>().Named(XMLElectricMotorIEPCInputDataProviderV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCStandardInputDataProviderV26>().Named(XMLElectricMotorIEPCStandardInputDataProviderV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV26>().Named(XMLDeclarationEngineDataProviderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLFuelCellSystemDeclarationInputData>().To<XMLFuelCellSystemDeclarationInputDataProviderV26>().Named(
				XMLFuelCellSystemDeclarationInputDataProviderV26.QUALIFIED_XSD_TYPE);

			Bind<IXMLFuelCellDeclarationInputData>().To<XMLFuelCellDeclarationInputDataProviderV26>()
				.Named(XMLFuelCellDeclarationInputDataProviderV26.QUALIFIED_XSD_TYPE);

            Bind<IXMLAirdragDeclarationInputData>().To<XMLDeclarationAirdragDataProviderV26>()
                .Named(XMLDeclarationAirdragDataProviderV26.QUALIFIED_XSD_TYPE);
		}
	}
}
