using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TrailerTypeHelper = TUGraz.VectoCommon.Models.TrailerTypeHelper;

namespace TUGraz.VectoCore.Tests.Helper.TrailerTool
{
    internal static class XDocumentCreator
	{
		private const string TrailerTypeLookUp = "Trailer_type";
		private const string GenericVehicles = @"Resources\Declaration\GenericVehicles\";

		private const string SampleTrailer = @"TestData\Trailer\trailerSample.xml";
		private static TrailerSegments _trailerSegments = new TrailerSegments();

		public static XDocument CreateXDocument(int vehicleGroup)
		{
			var sampleDoc = XDocument.Load(SampleTrailer);
			var row = _trailerSegments.GetRow(vehicleGroup.ToString());
			//private DataRow GetTrailerDataRow(
			//	NumberOfTrailerAxles numberOfAxles, TypeTrailer typeTrailer, BodyWorkCode bodyWorkCode, bool volumeOrientation, Kilogram tpmlmAxleAssembly)
			//reference trailer is determined via vehiclegroup;

			var trailerType = TrailerTypeHelper.Parse(row[TrailerTypeLookUp].ToString());
			var numberOfTrailerAxles = NumberOfTrailerAxlesHelper.Parse(row["Number_of_axles"].ToString());
			var bodyWorkCode = BodyWorkCodeHelper.Parse(row["Bodywork_code"].ToString());
			var volumeOrientation = row["Volume_orientation"].ToString() == "Yes";
			

			
			var setTpmlmAxleAssembly = row["TPMLM_axle_assembly"].ToString() != "---" && row["TPMLM_axle_assembly"].ToString() != "all weights";
			int tpmlmAxleAssemblyVal = 0;
			if (setTpmlmAxleAssembly) {
				try {
					tpmlmAxleAssemblyVal = GetTPMLMAxleAssembly(row);
				} catch (Exception ex) {
					Assert.Fail();
				}
			}

			var xElements = sampleDoc.Descendants();
			Assert.NotNull(xElements);
			
			var trailerTypeElement = xElements.Single(xElement => xElement.Name.LocalName == XMLNames.Trailer_TrailerType);
			trailerTypeElement.Value = trailerType.ToXMLFormat();

            var axleCountElement = xElements.Single(xElement => xElement.Name.LocalName == XMLNames.Trailer_AxleCount);
			axleCountElement.Value = numberOfTrailerAxles.ToXMLFormat();

			var couplingPointElement =
				xElements.Single(xElement => xElement.Name.LocalName == XMLNames.Trailer_TrailerCouplingPoint);
			if (trailerType == TypeTrailer.DC) {
				couplingPointElement.Value = TrailerCouplingPoint.High.ToXMLFormat();
			} else {
				couplingPointElement.Remove();
			}

			switch (numberOfTrailerAxles) {
				case NumberOfTrailerAxles.Unknown:
					Assert.Fail();
					break;
				case NumberOfTrailerAxles.One:
					break;
				case NumberOfTrailerAxles.Two:
					AddAxle(2, xElements);
					break;
				case NumberOfTrailerAxles.Three:
					AddAxle(2, xElements);
					AddAxle(3, xElements);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}


			///For the calculation of the Reference Ratio, the LengthBetweenCentresOfAxles doesn't matter because only reference trailers are simulated
			var lengthBetweenCentresOfAxles = xElements.Single(xElement =>
				xElement.Name.LocalName == XMLNames.Trailer_LengthBetweenCentersOfAxles);
			if (numberOfTrailerAxles == NumberOfTrailerAxles.One) {
				lengthBetweenCentresOfAxles.Remove();
			} else {
				lengthBetweenCentresOfAxles.Value = "9.999";
			}


			var bodyWorkElement = xElements.Single(xElement => xElement.Name.LocalName == XMLNames.Trailer_BodyCode);
			bodyWorkElement.Value = bodyWorkCode.ToXMLFormat();

			var volumeOrientationElement =
				xElements.Single(xElement => xElement.Name.LocalName == XMLNames.Trailer_VolumeOrientation);
			volumeOrientationElement.Value = volumeOrientation ? "true" : "false";


			if (setTpmlmAxleAssembly) {
				var tpmlmAxleAssemblyElement =
					xElements.Single(xElement => xElement.Name.LocalName == XMLNames.Trailer_TPLMAxleAssembly);
				tpmlmAxleAssemblyElement.Value = tpmlmAxleAssemblyVal.ToString();
				var tpmlmTrailerElement =
					xElements.Single(xElement => xElement.Name.LocalName == XMLNames.Trailer_TPMLMTotalTrailer);
				tpmlmTrailerElement.Value = (tpmlmAxleAssemblyVal + 1).ToString();


			} else {

			}

			if (trailerType == TypeTrailer.DB) {
				var Axle = xElements.Where((e) => e.Name.LocalName == XMLNames.AxleWheels_Axles_Axle)
					.Single(e => e.LastAttribute.Value == "1");
				Axle.Descendants().Single(e => e.Name.LocalName == XMLNames.AxleWheels_Axles_Axle_Steered).Value =
					"true";
			}

            return sampleDoc;
		}

		public static XDocument GetTowingVehicleXml(string XMLCode, out string path)
		{
			var fileName = XMLCode + ".xml";
			path = Path.Combine(GenericVehicles, fileName);


			return XDocument.Load(path);
		}

		public static int GetTPMLMAxleAssembly(DataRow row)
		{
			//HACK
			var possibleValues = new List<int> {
				8,
				13,
				19
			};
			foreach (var possibleValue in possibleValues) {
				var valueInTons = Kilogram.Create(possibleValue * 1000).ConvertToTon();
				if (_trailerSegments.getTPMLMAxleAssemblyFunction(row["TPMLM_axle_assembly"].ToString())
					.Invoke(valueInTons)) {
					return possibleValue*1000;
				}
			}
			Assert.Fail("No tpmlm axle assembly value was found for the given row");


			//var tpmlmAxleAssemblyMin = double.Parse(row["TPMLM_min"].ToString(), NumberFormatInfo.InvariantInfo);
			//var tpmlmAxleAssemblyMax =	double.Parse(row["TPMLM_max"].ToString(), NumberFormatInfo.InvariantInfo);

			//return GetMaxIntBetween(tpmlmAxleAssemblyMin, tpmlmAxleAssemblyMax) * 1000;
			return -1;
		}

		public static void AddAxle(int count, IEnumerable<XElement> xElements)
		{
			var axlesElement = xElements.Single(xElement => xElement.Name.LocalName == XMLNames.AxleWheels_Axles);
			var elementToAdd = new XElement(axlesElement.Descendants().First()); //deep copy
			elementToAdd.SetAttributeValue(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, count);
			axlesElement.Add(elementToAdd);
		}
	}
}
