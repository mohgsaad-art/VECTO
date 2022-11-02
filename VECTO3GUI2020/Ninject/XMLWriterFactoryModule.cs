using System;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using VECTO3GUI2020.Ninject.Util;
using VECTO3GUI2020.Util.XML.Implementation;
using VECTO3GUI2020.Util.XML.Implementation.ComponentWriter;
using VECTO3GUI2020.Util.XML.Implementation.DocumentWriter;
using VECTO3GUI2020.Util.XML.Interfaces;

namespace VECTO3GUI2020.Ninject
{
    public class XMLWriterFactoryModule : NinjectModule
    {
		public override void Load()
		{
	

			Array.ForEach(
				XMLDeclarationJobWriter_v1_0.SUPPORTED_VERSIONS,
				(sv=> 
					Bind<IXMLDeclarationJobWriter>().To<XMLDeclarationJobWriter_v1_0>().Named(sv)));

			Array.ForEach(
				XMLDeclarationJobWriter_v2_0.SUPPORTED_VERSIONS,
				(sv =>
					Bind<IXMLDeclarationJobWriter>().To<XMLDeclarationJobWriter_v2_0>().Named(sv)));

			Array.ForEach(
				XMLPTOWriter_v1_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLPTOWriter_v1_0>().Named(sv));

			Array.ForEach(
				XMLPTOWriter_v2_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLPTOWriter_v2_0>().Named(sv));


			Array.ForEach(
				XMLVehicleWriter_v1_0.SUPPORTEDVERSIONS,
				(sv) => 
					Bind<IXMLVehicleWriter>().To<XMLVehicleWriter_v1_0>().Named(sv));

			Array.ForEach(
				XMLVehicleWriter_v2_0.SUPPORTEDVERSIONS,
				sv =>
					Bind<IXMLVehicleWriter>().To<XMLVehicleWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLVehicleWriter_v2_10.SUPPORTEDVERSIONS,
				sv =>
					Bind<IXMLVehicleWriter>().To<XMLVehicleWriter_v2_10>().Named(sv));

			Array.ForEach(
				XMLComponentsWriter_v1_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentsWriter>().To<XMLComponentsWriter_v1_0>().Named(sv));

			Array.ForEach(
				XMLComponentsWriter_v2_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentsWriter>().To<XMLComponentsWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLEngineWriter_v1_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLEngineWriter_v1_0>().Named(sv));

			Array.ForEach(
				XMLEngineWriter_v2_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLEngineWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLGearboxWriter_v2_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLGearboxWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLRetarderWriter_v2_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLRetarderWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLAxleGearWriter_v2_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLAxleGearWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLAxleWheelsWriter_v2_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLAxleWheelsWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLAxleWheelWriter_v2_0.SUPPORTED_VERSIONS,
				sv=> Bind<IXMLComponentWriter>().To<XMLAxleWheelWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLTyreWriter_v2_0.SUPPORTED_VERSIONS,
				sv=> Bind<IXMLComponentWriter>().To<XMLTyreWriter_v2_0>().Named(sv));

			Array.ForEach(
				XMLTyreWriter_v2_3.SUPPORTED_VERSIONS,
				sv=>Bind<IXMLComponentWriter>().To<XMLTyreWriter_v2_3>().Named(sv));

			Array.ForEach(
				XMLAuxiliariesWriter_v2_0.SUPPORTED_VERSIONS, 
				sv => Bind<IXMLComponentWriter>().To<XMLAuxiliariesWriter_v2_0>().Named(sv));
			Array.ForEach(
				XMLAirDragWriter_v2_0.SUPPORTED_VERSIONS,
				sv => Bind<IXMLComponentWriter>().To<XMLAirDragWriter_v2_0>().Named(sv));

			Bind<IXMLBusAuxiliariesWriter>().To<XMLBusAuxiliariesWriterMultistage>();

		}
	}
}
