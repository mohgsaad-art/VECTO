using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.XML
{
    [TestFixture]
    internal class XMLSchemaTest
    {
		XmlSchemaSet _declJobSchemaSet = XMLValidator.GetXMLSchema(XmlDocumentType.DeclarationJobData);




        [TestCase("conventional", AlternatorType.Conventional)]
		[TestCase("no alternator", AlternatorType.None)]
		[TestCase("smart", AlternatorType.Smart),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void ParseAlternatorEnum(string xmlValue, AlternatorType expected)
		{
			var enumResult = AlternatorTypeHelper.Parse(xmlValue);
            Assert.AreEqual(expected, enumResult);
		}

       





    }
}
