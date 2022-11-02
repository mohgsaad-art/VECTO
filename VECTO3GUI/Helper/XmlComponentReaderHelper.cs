using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using VECTO3GUI.Model.TempDataObject;

namespace VECTO3GUI.Helper
{
	public class XmlComponentReaderHelper : AbstractXMLType
	{
		public XmlComponentReaderHelper(XmlNode node) : base(node) { }
		
		public AirdragComponentData GetAirdragComponentData()
		{
			if (!ElementExists(XMLNames.Component_AirDrag))
				return null;

			return new AirdragComponentData {
				Manufacturer =  GetString(XMLNames.Component_Manufacturer),
				Model = GetString(XMLNames.Component_Model),
				CertificationNumber = GetString(XMLNames.Component_CertificationNumber),
				Date = DateTime.Parse(GetString(XMLNames.Component_Date)).ToUniversalTime(),
				AppVersion = GetString(XMLNames.Component_AppVersion),
				CdxA_0 = GetDouble("CdxA_0").SI<SquareMeter>(),
				TransferredCdxA = GetDouble("TransferredCdxA").SI<SquareMeter>(),
				DeclaredCdxA = GetDouble(XMLNames.AirDrag_DeclaredCdxA).SI<SquareMeter>(),
				DigestValue = new DigestData(GetNode(XMLNames.DI_Signature))
			};
		}

		public SquareMeter ReadTransferredCdxA()
		{
			return GetDouble("TransferredCdxA").SI<SquareMeter>();
		}

		public SquareMeter ReadCdxA_0()
		{
			return GetDouble("CdxA_0").SI<SquareMeter>();
		}
	}
}
