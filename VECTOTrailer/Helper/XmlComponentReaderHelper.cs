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
using VECTOTrailer.Model.TempDataObject;

namespace VECTOTrailer.Helper
{
	public class XmlComponentReaderHelper : AbstractXMLType
	{
		public XmlComponentReaderHelper(XmlNode node) : base(node) { }

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
