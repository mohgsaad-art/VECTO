using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider {
	internal class XMLEngineeringEngineStopStartDataProviderV10 : AbstractXMLType, IXMLEngineeringEngineStopStartData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public const string XSD_TYPE = "EngineStartStopParametersEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringEngineStopStartDataProviderV10(IXMLEngineeringDriverData driverData, XmlNode node) : base(node) { }

		public virtual Second ActivationDelay => GetDouble("ActivationDelay", DeclarationData.Driver.EngineStopStart.ActivationDelay.Value()).SI<Second>();

		public virtual Second MaxEngineOffTimespan => GetDouble("MaxEngineOffTime", DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan.Value()).SI<Second>();

		public virtual double UtilityFactorStandstill => GetDouble("UtilityFactor", DeclarationData.Driver.EngineStopStart.UtilityFactor);

		public double UtilityFactorDriving => GetDouble("UtilityFactorDriving", DeclarationData.Driver.EngineStopStart.UtilityFactor);

		protected XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}