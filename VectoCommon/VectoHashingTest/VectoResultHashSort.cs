using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using TUGraz.VectoHashing.Util;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoHashing.Impl;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace VectoHashingTest
{
    public class VectoResultHashSort
    {
		private const string UnsortedJobPath = @"TestData/XML/Sort/Results/Unsorted/";
		private const string SortedJobPath = @"TestData/XML/Sort/Results/Sorted/"; 

		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		/*
         * How to get the Hardcoded Hashes
         *	- Manually Sort the file you want to Hash
         *  - Comment out the new code in the XSLT that you want to test
         *  - Run the Code and replace the existing hash with the new one
         *  - Comment in the new code in the XSLT and run the code
         */
		// EnergyConsumption, FC_ZEV_AuxHeater,CO2_ZEV_AuxHeater, Result
		[TestCase(@"PEV_completedBus_2.RSLT_MANUFACTURER.xml", XmlDocumentType.ManufacturerReport, "hokoWwm/3OOwwTyvEYVm9p5p3jpJ6P5fjsJaqomQCQQ="),
		// Fuel (dual fuel), Fuel Consumption, CO2, Result
		 TestCase(@"Conventional_heavyLorry_AMT.RSLT_MANUFACTURER.xml", XmlDocumentType.ManufacturerReport, "1lccsm0FBy28nmoZ3+svSSYNGVDObaHN9xmnrPKMbUE="),
		//Fuel Consumption, CO2, Result, Summary
		TestCase(@"HEV_completedBus_2.RSLT_CUSTOMER.xml", XmlDocumentType.CustomerReport, "/vQ5KuMptNzZfmJBDAToZOdPu6LqJI7n044Q+0sh9e8="),
		//OVC Mode, Fuel Consumption, CO2, Result, EnergyConsumption
		TestCase(@"HEV-S_heavyLorry_S3_ovc.RSLT_MANUFACTURER.xml", XmlDocumentType.ManufacturerReport, "GnoRUnF67vM+fX/L3olad4Ozs009B6muSw0j91rIo5U="),
		//EnergyConsumption, FC_ZEV_AuxHeater, CO2_ZEV_AuxHeater, Result, Summary
		TestCase(@"PEV_completedBus_2.RSLT_CUSTOMER.xml", XmlDocumentType.CustomerReport, "iebdqUqvC1bt0NyEe+/4kRf+YMA9SGSb+955nOeSBQM=")]
		public void TestValidation(string filename, XmlDocumentType documentType, string sortedHash)
		{

			//var hashsorted = ComputeHash(filename, true, documentType);
			var hashunsorted = ComputeHash(filename, false, documentType);
			Assert.AreEqual(sortedHash,hashunsorted);

		}

		public string ComputeHash(string jobname, bool sorted, XmlDocumentType documentType)
		{
			
			string path = sorted ? SortedJobPath + jobname : UnsortedJobPath + jobname;

			var doc = XDocument.Load(XmlReader.Create(path));

			/*
			MrfDocument = XDocument.Load(XmlReader.Create(mainFile));

			var result = doc.XPathSelectElement("//*[local-name()='Results']");
			var oldResult = MrfDocument.XPathSelectElement("//*[local-name()='Results']");

			oldResult.ReplaceWith(result);
			*/
			var stream = new MemoryStream();
			stream.Seek(0, SeekOrigin.Begin);
			var writer = new StreamWriter(stream);


			writer.Write(doc);
			writer.Flush();

			stream.Seek(0, SeekOrigin.Begin);
			var validator = new XMLValidator(XmlReader.Create(stream));
			Assert.IsTrue(validator.ValidateXML(documentType), validator.ValidationError);

			stream.Seek(0, SeekOrigin.Begin);
			var hash = VectoHash.Load(stream).ComputeHash();
			return hash;
		}

		

	}
}
