using System;
using System.Diagnostics;
using System.Xml.Linq;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;
using VECTO3GUI2020.Resources.XML;
using VECTO3GUI2020.Util.XML;
using VECTO3GUI2020.Util.XML.Components;

using VECTO3GUI2020.Util.XML.Vehicle;

namespace VECTO3GUI2020.Ninject.Factories
{
    public class XMLWriterFactoryModule : NinjectModule
	{
		private const string vehicleScope = nameof(IXMLWriterFactoryInternal);
		public override void Load()
		{
			Bind<IXMLWriterFactory>().To<XMLWriterFactory>().InSingletonScope();


			Bind<IXMLWriterFactoryInternal>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(false, new [] {
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = ((args) => {
						if (args[0] is DataSource dataSource) {
							return GetName(dataSource.TypeVersion, dataSource.Type);
						}
						throw new ArgumentException();
					}),
					methods = new [] {
						typeof(IXMLWriterFactoryInternal).GetMethod(nameof(IXMLWriterFactoryInternal.CreateWriter)),
                        typeof(IXMLWriterFactoryInternal).GetMethod(nameof(IXMLWriterFactoryInternal.CreateComponentsWriter)),
						typeof(IXMLWriterFactoryInternal).GetMethod(nameof(IXMLWriterFactoryInternal.CreateComponentWriter))
                    },
					skipArguments = 1,
					takeArguments = 1,
				}
			})).InSingletonScope().Named(vehicleScope);

			Array.ForEach(XMLCompletedBusVehicleWriterConventional.SUPPORTEDVERSIONS, sv => AddVehicleWriterBinding<XMLCompletedBusVehicleWriterConventional>(sv.version, sv.type));

			Array.ForEach(XMLCompletedBusVehicleWriterExempted.SUPPORTEDVERSIONS, sv => AddVehicleWriterBinding<XMLCompletedBusVehicleWriterExempted>(sv.version, sv.type));
            #region Vehicle
            AddVehicleWriterBinding<XMLCompletedBusVehicleWriterHEV>(XMLCompletedBusVehicleWriterHEV.VERSION);
			AddVehicleWriterBinding<XMLCompletedBusVehicleWriterPEV>(XMLCompletedBusVehicleWriterPEV.VERSION);
			AddVehicleWriterBinding<XMLCompletedBusVehicleWriterFCHV>(XMLCompletedBusVehicleWriterFCHV.VERSION);

            #endregion Vehicle

            #region Components
            AddComponentsWriterBinding<XMLCompletedBusComponentWriter_Conventional>(XMLCompletedBusComponentWriter_Conventional.VERSION);
            AddComponentsWriterBinding<XMLCompletedBusComponentsWriter_xEV>(XMLCompletedBusComponentsWriter_xEV.VERSION);
            //AddComponentsWriterBinding<XMLCompletedBusComponentsWriter_xEV_v27>(XMLCompletedBusComponentsWriter_xEV_v27.VERSION);


            #endregion

            #region Airdrag
            AddComponentWriterBinding<XMLAirDragWriter_v2_0>(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20, XMLTypes.AirDragDataDeclarationType);
			AddComponentWriterBinding<XMLAirDragWriter_v1_0>(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10, XMLTypes.AirDragDataDeclarationType);
			AddComponentWriterBinding<XMLAirDragWriter_v2_4>(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24, XMLTypes.AirDragModifiedUseStandardValueType);

            #endregion

            #region BusAux
            AddBusAuxWriterBinding<XMLCompletedBusAuxiliariesWriterConventional>(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27, XMLTypes.AUX_Conventional_CompletedBusType);
            AddBusAuxWriterBinding<XMLCompletedBusAuxiliariesWriter_xEV>(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27, XMLTypes.AUX_xEV_CompletedBusType);
            //AddBusAuxWriterBinding<XMLCompletedBusAuxiliariesWriter_xEV_v27>(XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27, XMLTypes.AUX_xEV_CompletedBusType);
			#endregion


            #region





            //Array.ForEach(
            //    XMLComponentsWriter_v1_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentsWriter>().To<XMLComponentsWriter_v1_0>().Named(sv));

            //Array.ForEach(
            //    XMLComponentsWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentsWriter>().To<XMLComponentsWriter_v2_0>().Named(sv));

            //Array.ForEach(
            //    XMLEngineWriter_v1_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLEngineWriter_v1_0>().Named(sv));

            //Array.ForEach(
            //    XMLEngineWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLEngineWriter_v2_0>().Named(sv));

            //Array.ForEach(
            //    XMLGearboxWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLGearboxWriter_v2_0>().Named(sv));

            //Array.ForEach(
            //    XMLRetarderWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLRetarderWriter_v2_0>().Named(sv));

            //Array.ForEach(
            //    XMLAxleGearWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLAxleGearWriter_v2_0>().Named(sv));

            //Array.ForEach(
            //    XMLAxleWheelsWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLAxleWheelsWriter_v2_0>().Named(sv));

            //Array.ForEach(
            //    XMLAxleWheelWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLAxleWheelWriter_v2_0>().Named(sv));

            //Array.ForEach(
            //    XMLTyreWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLTyreWriter_v2_0>().Named(sv));

            //Array.ForEach(
            //    XMLTyreWriter_v2_3.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLTyreWriter_v2_3>().Named(sv));

            //Array.ForEach(
            //    XMLAuxiliariesWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLAuxiliariesWriter_v2_0>().Named(sv));
            //Array.ForEach(
            //    XMLAirDragWriter_v2_0.SUPPORTED_VERSIONS,
            //    sv => Bind<IXMLComponentWriter>().To<XMLAirDragWriter_v2_0>().Named(sv));

            //Bind<IXMLBusAuxiliariesWriter>().To<XMLCompletedBusAuxiliariesWriterConventional>();

            #endregion

        }

        public string GetName(XNamespace version, string xsdType)
		{
			return XMLHelper.CombineNamespace(version, xsdType);
		}

		private void AddVehicleWriterBinding<TConcrete>((XNamespace ns, string xsdType) version) where TConcrete : class, IXMLVehicleWriter
		{
			Debug.WriteLine($"Create Binding for {typeof(TConcrete)} named '{GetName(version.ns, version.xsdType)}'");
			Bind<IXMLVehicleWriter>().To<TConcrete>().WhenParentNamed(vehicleScope).Named(GetName(version.ns, version.xsdType));
		}
        private void AddVehicleWriterBinding<TConcrete>(XNamespace version, string xsdType) where TConcrete : class, IXMLVehicleWriter
		{
			Debug.WriteLine($"Create Binding for {typeof(TConcrete)} named '{GetName(version, xsdType)}'");
			Bind<IXMLVehicleWriter>().To<TConcrete>().WhenParentNamed(vehicleScope).Named(GetName(version, xsdType));
		}
		private void AddComponentsWriterBinding<TConcrete>((XNamespace version, string xsdType) version)
			where TConcrete : class, IXMLComponentsWriter
		{
			Debug.WriteLine($"Create Binding for {typeof(TConcrete)} named '{GetName(version.version, version.xsdType)}'");
			Bind<IXMLComponentsWriter>().To<TConcrete>().WhenParentNamed(vehicleScope).Named(GetName(version.version, version.xsdType));
		}

        private void AddComponentsWriterBinding<TConcrete>(XNamespace version, string xsdType)
			where TConcrete : class, IXMLComponentsWriter
		{
			Debug.WriteLine($"Create Binding for {typeof(TConcrete)} named '{GetName(version, xsdType)}'");
			Bind<IXMLComponentsWriter>().To<TConcrete>().WhenParentNamed(vehicleScope).Named(GetName(version, xsdType));
        }

		private void AddComponentWriterBinding<TConcrete>(XNamespace version, string xsdType) where TConcrete : class, IXMLComponentWriter
        {
			Debug.WriteLine($"Create Binding for {typeof(TConcrete)} named '{GetName(version, xsdType)}'");
			Bind<IXMLComponentWriter>().To<TConcrete>().WhenParentNamed(vehicleScope).Named(GetName(version, xsdType));
		}

		private void AddBusAuxWriterBinding<TConcrete>(XNamespace version, string xsdType)
			where TConcrete : class, IXMLBusAuxiliariesWriter
		{
			Debug.WriteLine($"Create Binding for {typeof(TConcrete)} named '{GetName(version, xsdType)}'");
			Bind<IXMLBusAuxiliariesWriter>().To<TConcrete>().WhenParentNamed(vehicleScope).Named(GetName(version, xsdType));
        }
	}
}
