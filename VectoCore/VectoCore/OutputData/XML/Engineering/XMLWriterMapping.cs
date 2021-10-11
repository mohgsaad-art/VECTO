/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer;

namespace TUGraz.VectoCore.OutputData.XML.Engineering {
	internal static class XMLWriterMapping
	{
		static Dictionary<Type, Entry> mapping = new Dictionary<Type, Entry>() {
			
			{
				typeof(IEngineEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringEngineWriter),
					XMLTag = XMLNames.Component_Engine,
					FilenameTemplate = "Engine-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IVehicleEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLVehicleDataWriter),
					XMLTag = XMLNames.Component_Vehicle,
					FilenameTemplate = "Vehicle-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IVehicleComponentsEngineering),
				new Entry {
					WriterType = typeof(IXMLEngineeringComponentWriter),
				}
			}, {
				typeof(IGearboxEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringGearboxWriter),
					XMLTag = XMLNames.Component_Gearbox,
					FilenameTemplate = "Gearbox-{0}_{1}{2}.xml"
				}
			}, {
				typeof(ITransmissionInputData),
				new Entry { WriterType = typeof(IXMLEngineeringGearWriter) }
			}, {
				typeof(IDriverEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringDriverDataWriter),
				}
			}, {
				typeof(IRetarderInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringRetarderWriter),
					XMLTag = XMLNames.Component_Retarder,
					FilenameTemplate = "Retarder-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IAxleGearInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringAxlegearWriter),
					XMLTag = XMLNames.Component_Axlegear,
					FilenameTemplate = "Axlegear-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IAxlesEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringAxlesWriter),
					XMLTag = XMLNames.Component_AxleWheels,
				}
			}, {
				typeof(IAxleEngineeringInputData),
				new Entry { WriterType = typeof(IXMLEngineeringAxleWriter) }
			}, {
				typeof(ITyreEngineeringInputData),
				new Entry {
					WriterType = typeof(IXmlEngineeringTyreWriter),
					XMLTag = XMLNames.AxleWheels_Axles_Axle_Tyre,
					FilenameTemplate = "Tyre-{0}_{1}{2}.xml"
				}
			}, {
				typeof(ITorqueConverterEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringTorqueconverterWriter),
					XMLTag = XMLNames.Component_TorqueConverter,
					FilenameTemplate = "TorqueConverter-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IAirdragEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringAirdragWriter),
					XMLTag = XMLNames.Component_AirDrag,
					FilenameTemplate = "Airdrag-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IAngledriveInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringAngledriveWriter),
					XMLTag = XMLNames.Component_Angledrive,
					FilenameTemplate = "Angledrive{0}_{1}{2}.xml"
				}
			}, {
				typeof(ILookaheadCoastingInputData),
				new Entry { WriterType = typeof(IXMLLookaheadDataWriter) }
			}, {
				typeof(IOverSpeedEngineeringInputData),
				new Entry { WriterType = typeof(IXMLOverspeedDataWriter) }
			}, {
				typeof(IDriverAccelerationData),
				new Entry { WriterType = typeof(IXMLAccelerationDataWriter) }
			}, {
				typeof(IGearshiftEngineeringInputData),
				new Entry { WriterType = typeof(IXMLGearshiftDataWriter) }
			}, {
				typeof(IAuxiliariesEngineeringInputData),
				new Entry { WriterType = typeof(IXMLAuxiliariesWriter) }
			}, {
				typeof(IAuxiliaryEngineeringInputData),
				new Entry { WriterType = typeof(IXMLAuxiliaryWriter)}
			}, {
				typeof(IAdvancedDriverAssistantSystemsEngineering), new Entry() {WriterType = typeof(IXMLEngineeringADASWriter), XMLTag = XMLNames.Vehicle_AdvancedDriverAssist}
			}

		};

		public class Entry
		{
			public Type WriterType { get; set; }

			public string XMLTag { get; set; }

			public string FilenameTemplate { get; set; }
		}

		public static Type GetWriterType(Type inputType)
		{
			return mapping.ContainsKey(inputType) ? mapping[inputType].WriterType : null;
		}

		public static string GetXMLTag(Type inputType)
		{
			return mapping.ContainsKey(inputType) ? mapping[inputType].XMLTag : null;
		}

		public static string GetFilenameTemplate(Type inputType)
		{
			return mapping.ContainsKey(inputType) ? mapping[inputType].FilenameTemplate : "{0}_{1}.xml";
		}
	}
}