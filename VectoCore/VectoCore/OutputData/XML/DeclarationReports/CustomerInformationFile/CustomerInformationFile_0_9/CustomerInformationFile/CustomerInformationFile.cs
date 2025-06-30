using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CustomerInformationFile
{
    public abstract class CustomerInformationFile : AbstractCustomerReport
    {
        public static XNamespace Cif => Namespace;
		protected CustomerInformationFile(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }
	}

    #region LorryCIF
    public class ConventionalLorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "Conventional_LorryOutputType";
		public ConventionalLorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetConventionalLorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_PxLorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => XMLNames.CIF_OutputDataType_HEV_Px_LorryOutputType;
		public HEV_PxLorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_PxLorryVehicleType().GetElement(inputData);
		}
		#endregion
	}

	public class HEV_S2_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "HEV_Sx_LorryOutputType";
		public HEV_S2_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S2_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_S3_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "HEV_Sx_LorryOutputType";
		public HEV_S3_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_S4_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "HEV_Sx_LorryOutputType";
		public HEV_S4_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S4_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class HEV_IEPC_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "HEV_Sx_LorryOutputType";
		public HEV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_IEPC_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class Multiple_FCHV_Lorry_CIF : CustomerInformationFile
	{
        public Multiple_FCHV_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

        public override string OutputDataType => "FCHV_Fx_LorryOutputType";

        public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
            Vehicle = _cifFactory.GetMultiple_FCHV_LorryVehicleType().GetElement(inputData);
        }
    }

    public class Multiple_PEV_Lorry_CIF : CustomerInformationFile
    {
        public Multiple_PEV_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

        public override string OutputDataType => "PEV_Ex_LorryOutputType";

        public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
            Vehicle = _cifFactory.GetMultiple_PEV_LorryVehicleType().GetElement(inputData);
        }
    }

    public class Multiple_SHEV_Lorry_CIF : CustomerInformationFile
    {
        public Multiple_SHEV_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

        public override string OutputDataType => "HEV_Sx_LorryOutputType";

        public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
            Vehicle = _cifFactory.GetMultiple_SHEV_LorryVehicleType().GetElement(inputData);
        }
    }

    public class FCHV_F2_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "FCHV_Fx_LorryOutputType";
		public FCHV_F2_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetFCHV_F2_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class FCHV_F3_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "FCHV_Fx_LorryOutputType";
		public FCHV_F3_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetFCHV_F3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class FCHV_F4_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "FCHV_Fx_LorryOutputType";

		public FCHV_F4_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetFCHV_F4_LorryVehicleType().GetElement(inputData);
		}
	}

	public class FCHV_IEPC_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "FCHV_Fx_LorryOutputType";
		
		public FCHV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetFCHV_IEPC_LorryVehicleType().GetElement(inputData);
		}
	}

	public class PEV_E2_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "PEV_Ex_LorryOutputType";
		public PEV_E2_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E2_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class PEV_E3_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "PEV_Ex_LorryOutputType";
		public PEV_E3_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E3_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	public class PEV_E4_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "PEV_Ex_LorryOutputType";
		public PEV_E4_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E4_LorryVehicleType().GetElement(inputData);

		}

		#endregion
	}



	public class PEV_IEPC_Lorry_CIF : CustomerInformationFile
	{
		public override string OutputDataType => "PEV_Ex_LorryOutputType";
		public PEV_IEPC_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_IEPC_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}


	public class Exempted_Lorry_CIF : CustomerInformationFile
	{
		public Exempted_Lorry_CIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override string OutputDataType => XMLNames.CIF_OutputDataType_Exempted_LorryOutputType;

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetExempted_LorryVehicleType().GetElement(inputData);
		}

		#endregion
	}

	#endregion


	#region CompletedBus

	public abstract class CustomerInformationFileCompletedBus : CustomerInformationFile, IXMLCustomerReportCompletedBus
	{
		private bool _allSuccess;
		private int _resultCount;

		protected XElement InputDataIntegrityPrimaryVehicle { get; set; }

		protected XElement ManufacturerReportIntegrityPrimaryVehicle { get; set; }

		protected CustomerInformationFileCompletedBus(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) :
			base(cifFactory, resultWriterFactory) { }

		public override void Initialize(VectoRunData modelData)
		{
			InitializeVehicleData(modelData.InputData);
			_ovc = modelData.VehicleData.OffVehicleCharging;

			var inputData = modelData.InputData as IMultistepBusInputDataProvider;
			if (inputData == null) {
				throw new VectoException("CompletedBus CustomerInformationFile requires MultistepBusInputData");
			}
			Input = inputData.JobInputData.PrimaryVehicle.Vehicle;
			Results = _resultFactory.GetCIFResultsWriter(modelData.InputData, modelData.VehicleData.VehicleCategory.GetVehicleType(),
				modelData.JobType, modelData.VehicleData.OffVehicleCharging, modelData.Exempted);
			InputDataIntegrity = new XElement(Namespace + XMLNames.Report_InputDataSignature,
				inputData.JobInputData.ConsolidateManufacturingStage.Signature == null
					? XMLHelper.CreateDummySig(_di)
					: inputData.JobInputData.ConsolidateManufacturingStage.Signature.ToXML(_di));
					//new XElement());
			InputDataIntegrityPrimaryVehicle = new XElement(Namespace + "InputDataSignaturePrimaryVehicle",
				inputData.JobInputData.PrimaryVehicle.PrimaryVehicleInputDataHash.ToXML(_di));
			ManufacturerReportIntegrityPrimaryVehicle =
				new XElement(Namespace + "ManufacturerRecordSignaturePrimaryVehicle", inputData.JobInputData.PrimaryVehicle.ManufacturerRecordHash.ToXML(_di));
		}

		protected override IList<XElement> GetReportContents(XElement resultSignature)
		{
			return new[] {
				Vehicle,
				InputDataIntegrityPrimaryVehicle,
				ManufacturerReportIntegrityPrimaryVehicle,
				InputDataIntegrity,
				new XElement(Namespace + XMLNames.Report_ManufacturerRecord_Signature, resultSignature),
				Results.GenerateResults(_results),
				XMLHelper.GetApplicationInfo(Namespace)
			};
		}

		#region Implementation of IXMLCustomerReportCompletedBus

		public void WriteResult(IResultEntry genericResult, IResultEntry specificResult, IResult primaryResult, Func<IResultEntry, IResultEntry, IResult, IResultEntry> getCompletedResult)
		{
			_allSuccess &= genericResult.Status == VectoRun.Status.Success;
			_allSuccess &= specificResult.Status == VectoRun.Status.Success;
			_resultCount++;

			_results.Add(getCompletedResult(genericResult, specificResult, primaryResult));
			//Results.Add(
			//	genericResult.Status == VectoRun.Status.Success && specificResult.Status == VectoRun.Status.Success
			//		? GetSuccessResultEntry(genericResult, specificResult, primaryResult)
			//		: GetErrorResultEntry(genericResult, specificResult, primaryResult));
		}

		#endregion
	}

	public class Conventional_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "Conventional_CompletedBusOutputType";

		public Conventional_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
        {
        }

        public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
			Vehicle = _cifFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
		}
    }

	public class HEV_Px_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "HEV_CompletedBusOutputType";

		public HEV_Px_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_Px_CompletedBusVehicleType().GetElement(inputData);
		}
	}

    public class HEV_IHPC_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
        public override string OutputDataType => "HEV_CompletedBusOutputType";

        public HEV_IHPC_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
        {
        }

        public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
            Vehicle = _cifFactory.GetHEV_IHPC_CompletedBusVehicleType().GetElement(inputData);
        }
    }

    public class HEV_S2_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "HEV_CompletedBusOutputType";

		public HEV_S2_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S2_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	public class HEV_S3_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "HEV_CompletedBusOutputType";

		public HEV_S3_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S3_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	public class HEV_S4_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "HEV_CompletedBusOutputType";

		public HEV_S4_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_S4_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	public class HEV_IEPC_S_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "HEV_CompletedBusOutputType";

		public HEV_IEPC_S_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetHEV_IEPC_S_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	public class PEV_E2_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "PEV_CompletedBusOutputType";

		public PEV_E2_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E2_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	public class PEV_E3_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "PEV_CompletedBusOutputType";

		public PEV_E3_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E3_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	public class PEV_E4_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "PEV_CompletedBusOutputType";

		public PEV_E4_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_E4_CompletedBusVehicleType().GetElement(inputData);
		}
	}

	public class PEV_IEPC_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public override string OutputDataType => "PEV_CompletedBusOutputType";

		public PEV_IEPC_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory)
		{
		}

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetPEV_IEPC_CompletedBusVehicleType().GetElement(inputData);
		}
	}


	public class Exempted_CompletedBusCIF : CustomerInformationFileCompletedBus
	{
		public Exempted_CompletedBusCIF(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultWriterFactory) : base(cifFactory, resultWriterFactory) { }

		#region Overrides of AbstractCustomerReport

		public override string OutputDataType => XMLNames.CIF_OutputDataType_Exempted_CompletedBusOutputType;

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _cifFactory.GetExemptedCompletedBusVehicleType().GetElement(inputData);
		}

		#endregion
	}

	#endregion
}
