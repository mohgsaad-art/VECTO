using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
	public static class ResultWriterNamingHelper
	{
		public const string RESULT_WRITER_2nd_AMDM = "ResultWriter_v0.9";

		public const string RESULT_WRITER_3rd_AMDM = "ResultWriter_v1.0";

		public static string GetResultWriterName(IDeclarationInputDataProvider inputData)
		{
			switch (inputData.DataSource.SourceType) {
				case DataSourceType.XMLFile:
					return GetXMLResultWriterName(inputData);
				case DataSourceType.JSONFile:
					return GetJSONResultWriterName(inputData);
			}

			throw new VectoException($"unhandled input data source type {inputData.DataSource.SourceType}", ErrorCodes.INPUT_INVALID_DATASOURCE);
		}

		private static string GetXMLResultWriterName(IDeclarationInputDataProvider inputData)
		{
			if (inputData is IMultistepBusInputDataProvider multistep) {
				switch (multistep.JobInputData.PrimaryVehicle.DataSource.TypeVersion) {
					case XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1:
						return RESULT_WRITER_2nd_AMDM;
					case XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V11:
						return RESULT_WRITER_3rd_AMDM;
				}
				throw new VectoException($"unknown XML type of multistep input data {inputData.JobInputData.Vehicle.DataSource.TypeVersion}", ErrorCodes.INPUT_INVALID_DATASOURCE);
			}

            switch (inputData.JobInputData.Vehicle.DataSource.TypeVersion) {
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10:
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20:
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21:
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V22:
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V221:
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24:
					return RESULT_WRITER_2nd_AMDM;
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27:
					return RESULT_WRITER_3rd_AMDM;
			}

			throw new VectoException($"unknown XML type of input data {inputData.JobInputData.Vehicle.DataSource.TypeVersion}", ErrorCodes.INPUT_INVALID_DATASOURCE);
		}

		private static string GetJSONResultWriterName(IDeclarationInputDataProvider inputData)
		{
			return RESULT_WRITER_2nd_AMDM;
		}

    }
}