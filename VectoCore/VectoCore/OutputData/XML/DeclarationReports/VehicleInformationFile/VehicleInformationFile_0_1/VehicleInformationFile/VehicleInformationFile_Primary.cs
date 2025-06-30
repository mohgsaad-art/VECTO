using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.VIFReport
{
	internal abstract class VehicleInformationFile_PrimaryStep : AbstractVehicleInformationFile
	{
		public VehicleInformationFile_PrimaryStep(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory)
		{
			_tns = VIF;
		}
	}

	internal class Conventional_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public Conventional_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetConventionalVehicleType().GetElement(inputData);
		}
	}

	internal class HEV_Px_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_Px_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevPxVehicleType().GetElement(inputData);
		}
	}

	internal class HEV_S2_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_S2_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevS2VehicleType().GetElement(inputData);
		}
	}

	internal class HEV_S3_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_S3_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevS3VehicleType().GetElement(inputData);
		}
	}

	internal class HEV_S4_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_S4_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevS4VehicleType().GetElement(inputData);
		}
	}

	internal class HEV_IEPC_S_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_IEPC_S_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevIepcSVehicleType().GetElement(inputData);
		}
	}

	internal class Multiple_SHEV_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
        public Multiple_SHEV_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

        protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
            Vehicle = _vifFactory.GetMultipleSHEVVehicleType().GetElement(inputData);
        }
    }

	internal class Multiple_PEV_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
        public Multiple_PEV_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

        protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
            Vehicle = _vifFactory.GetMultiplePEVVehicleType().GetElement(inputData);
        }
    }

    internal class Multiple_FCHV_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
        public Multiple_FCHV_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

        protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
        {
            Vehicle = _vifFactory.GetMultipleFCHVVehicleType().GetElement(inputData);
        }
    }

	internal class HEV_F2_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_F2_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevF2VehicleType().GetElement(inputData);
		}
	}

	internal class HEV_F3_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_F3_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevF3VehicleType().GetElement(inputData);
		}
	}

	internal class HEV_F4_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_F4_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevF4VehicleType().GetElement(inputData);
		}
	}

	internal class HEV_IEPC_F_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public HEV_IEPC_F_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetHevIepcFVehicleType().GetElement(inputData);
		}
	}

	internal class PEV_E2_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public PEV_E2_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetPevE2VehicleType().GetElement(inputData);
		}
	}

	internal class PEV_E3_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public PEV_E3_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetPevE3VehicleType().GetElement(inputData);
		}
	}

	internal class PEV_E4_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public PEV_E4_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetPevE4VehicleType().GetElement(inputData);
		}
	}

	internal class PEV_IEPC_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public PEV_IEPC_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetPevIEPCVehicleType().GetElement(inputData);
		}
	}

	internal class Exempted_PrimaryBus_VIF : VehicleInformationFile_PrimaryStep
	{
		public Exempted_PrimaryBus_VIF(IVIFReportFactory vifFactory, IResultsWriterFactory resultFactory) : base(vifFactory, resultFactory) { }

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetExemptedVehicleType().GetElement(inputData);
		}
	}
}
