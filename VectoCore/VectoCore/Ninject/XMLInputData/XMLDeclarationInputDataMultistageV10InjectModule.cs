using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace TUGraz.VectoCore.Ninject.XMLInputData
{
    public class XMLDeclarationInputDataMultistageV10InjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IXMLIEPCInputData>().To<XMLElectricMotorIEPCIInputDataProviderV10>().Named(XMLElectricMotorIEPCIInputDataProviderV10.QUALIFIED_XSD_TYPE);
        }
    }
}
