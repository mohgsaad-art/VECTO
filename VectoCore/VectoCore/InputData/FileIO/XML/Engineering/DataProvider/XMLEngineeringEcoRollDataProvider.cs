using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringEcoRollDataProviderV10 : AbstractXMLType, IXMLEngineeringEcoRollData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public const string XSD_TYPE = "EcoRollEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);
		
		public XMLEngineeringEcoRollDataProviderV10(IXMLEngineeringDriverData driverData, XmlNode node) : base(node) { }

		#region Implementation of IEcoRollEngineeringInputData

		public MeterPerSecond MinSpeed => GetDouble("MinSpeed", DeclarationData.Driver.EcoRoll.MinSpeed.AsKmph).KMPHtoMeterPerSecond();

		public Second ActivationDelay => GetDouble("ActivationDelay", DeclarationData.Driver.EcoRoll.ActivationDelay.Value()).SI<Second>();

		public MeterPerSecond UnderspeedThreshold => GetDouble("Underspeed", DeclarationData.Driver.EcoRoll.UnderspeedThreshold.AsKmph).KMPHtoMeterPerSecond();

		public MeterPerSquareSecond AccelerationUpperLimit =>
			GetDouble("MaxAcceleration", DeclarationData.Driver.EcoRoll.AccelerationUpperLimit.Value())
				.SI<MeterPerSquareSecond>();

		#endregion
	}
}
