using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Model.TempDataObject
{
	public class AirdragComponentData : IAirdrag, ITempDataObject<IAirdragViewModel>
	{
		#region IAirdrag Interface

		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public string CertificationNumber { get; set; }
		public DateTime? Date { get; set; }
		public bool UseMeasuredValues { get; set; }
		public SquareMeter CdxA_0 { get; set; }
		public SquareMeter TransferredCdxA { get; set; }
		public SquareMeter DeclaredCdxA { get; set; }
		public string AppVersion { get; set; }
		public DigestData DigestValue { get; set; }
		public bool NoAirdragData { get; set; }
		public bool UseMeasurementData { get; set; }

		#endregion
		

		public AirdragComponentData(){}

		public AirdragComponentData(IAirdragViewModel viewModel , bool defaultValues)
		{
			if(defaultValues)
				ClearValues(viewModel);
			UseMeasurementData = viewModel.UseMeasurementData;
			NoAirdragData = viewModel.NoAirdragData;
		}

		public AirdragComponentData(IAirdragViewModel airdrag)
		{
			SetValues(airdrag);
		}
		
		public void UpdateCurrentValues(IAirdragViewModel airdrag)
		{
			SetValues(airdrag);
		}

		public void ResetToComponentValues(IAirdragViewModel viewModel)
		{
			viewModel.UseMeasurementData = UseMeasurementData;
			viewModel.NoAirdragData = NoAirdragData;
			viewModel.Model = Model;
			viewModel.Manufacturer = Manufacturer;
			viewModel.CertificationNumber = CertificationNumber;
			viewModel.Date = Date;
			viewModel.AppVersion = AppVersion;
			viewModel.DeclaredCdxA = DeclaredCdxA;
			viewModel.CdxA_0 = CdxA_0;
			viewModel.TransferredCdxA = TransferredCdxA;
			viewModel.DigestValue = DigestValue;
		}

		public void ClearValues(IAirdragViewModel viewModel)
		{
			viewModel.Model = default(string);
			viewModel.Manufacturer = default(string);
			viewModel.CertificationNumber = default(string);
			viewModel.Date = default(DateTime?);
			viewModel.AppVersion = default(string);
			viewModel.DeclaredCdxA = default(SquareMeter);
			viewModel.CdxA_0 = default(SquareMeter);
			viewModel.TransferredCdxA = default(SquareMeter);
			viewModel.DigestValue = default(DigestData);
		}

		private void SetValues(IAirdragViewModel viewModel)
		{
			UseMeasurementData = viewModel.UseMeasurementData;
			NoAirdragData = viewModel.NoAirdragData;
			Model = viewModel.Model;
			Manufacturer = viewModel.Manufacturer;
			CertificationNumber = viewModel.CertificationNumber;
			Date = viewModel.Date;
			AppVersion = viewModel.AppVersion;
			DeclaredCdxA = viewModel.DeclaredCdxA;
			CdxA_0 = viewModel.CdxA_0;
			TransferredCdxA = viewModel.TransferredCdxA;
			DigestValue = viewModel.DigestValue;
		}
	}
}
