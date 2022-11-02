using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider {
	internal class XMLEngineeringPCCInputDataProviderV10 : AbstractXMLType, IXMLEngineeringPCCInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public const string XSD_TYPE = "PCCParametersType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringPCCInputDataProviderV10(IXMLEngineeringDriverData driverData, XmlNode node) : base(node) { }

		#region Implementation of IPCCEngineeringInputData

		public MeterPerSecond PCCEnabledSpeed => GetDouble("EnablingSpeed", DeclarationData.Driver.PCC.PCCEnableSpeed.AsKmph).KMPHtoMeterPerSecond();

		public MeterPerSecond MinSpeed => GetDouble("MinSpeed", DeclarationData.Driver.PCC.MinSpeed.AsKmph).KMPHtoMeterPerSecond();

		public Meter PreviewDistanceUseCase1 => GetDouble("PreviewDistanceUseCase1", DeclarationData.Driver.PCC.PreviewDistanceUseCase1.Value()).SI<Meter>();

		public Meter PreviewDistanceUseCase2 => GetDouble("PreviewDistanceUseCase2", DeclarationData.Driver.PCC.PreviewDistanceUseCase2.Value()).SI<Meter>();

		public MeterPerSecond Underspeed => GetDouble("AllowedUnderspeed", DeclarationData.Driver.PCC.Underspeed.AsKmph).KMPHtoMeterPerSecond();

		public MeterPerSecond OverspeedUseCase3 => GetDouble("AllowedOverspeed", DeclarationData.Driver.PCC.OverspeedUseCase3.AsKmph).KMPHtoMeterPerSecond();

		#endregion
	}
}