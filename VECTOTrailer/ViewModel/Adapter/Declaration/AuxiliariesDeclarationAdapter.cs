using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Impl;
using VECTOTrailer.ViewModel.Interfaces;

namespace VECTOTrailer.ViewModel.Adapter.Declaration {
	public class AuxiliariesDeclarationAdapter : IAuxiliariesDeclarationInputData
	{
		protected IAuxiliariesViewModel ViewModel;

		public AuxiliariesDeclarationAdapter(IAuxiliariesViewModel auxiliariesViewModel)
		{
			ViewModel = auxiliariesViewModel;
		}

		#region Implementation of IAuxiliariesDeclarationInputData

		public bool SavedInDeclarationMode { get { return true; } }

		public IList<IAuxiliaryDeclarationInputData> Auxiliaries
		{
			get {
				var retVal = new List<IAuxiliaryDeclarationInputData>();

				retVal.Add(
					new AuxiliaryDataInputData {
						Type = AuxiliaryType.Fan,
						Technology = new List<string> { ViewModel.FanTechnology }
					});
				retVal.Add(new AuxiliaryDataInputData {
						Type = AuxiliaryType.ElectricSystem,
						Technology = new List<string> { ViewModel.ElectricSystemTechnology }
					});
				retVal.Add(new AuxiliaryDataInputData {
						Type = AuxiliaryType.HVAC,
						Technology = new List<string> { ViewModel.HVACTechnology }
					});
				retVal.Add(new AuxiliaryDataInputData {
						Type = AuxiliaryType.PneumaticSystem,
						Technology = new List<string> { ViewModel.PneumaticSystemTechnology }
					});
				retVal.Add(new AuxiliaryDataInputData {
						Type = AuxiliaryType.SteeringPump,
						Technology = ViewModel.SteeringPumpTechnologies
							//.OrderBy(x => x.SteeredAxle)
							.Select(x => x.SteeringPumpTechnology).ToList()
					});
			return retVal;
			}
		}

		#endregion
	}
}