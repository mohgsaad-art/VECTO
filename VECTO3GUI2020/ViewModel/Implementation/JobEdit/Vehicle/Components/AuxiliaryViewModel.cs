using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
	public abstract class AuxiliaryViewModel : ViewModelBase, IComponentViewModel, IAuxiliaryViewModel
	{

		protected string _name;
		private string _technologyName;
		private IList<string> _technologyList;

		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		public string TechnologyName
		{
			get => _technologyName;
			set
			{
				_technology[0] = value;
				SetProperty(ref _technologyName, value);
			}
		}

		public IList<string> TechnologyList
		{
			get => _technologyList;
		}



		public bool IsPresent => true;

		protected IDeclarationAuxiliaryTable _auxiliaryTable;
		protected IXMLAuxiliaryDeclarationInputData _inputData;


		public AuxiliaryViewModel(IXMLAuxiliaryDeclarationInputData inputData,
			IAuxiliaryModelFactory auxiliaryModelFactory)
		{
			_inputData = inputData;
			_type = _inputData.Type;
			_name = AuxiliaryTypeHelper.ToString(_type);
			_technology = _inputData.Technology;
			_technologyName = _inputData.Technology[0];
			Debug.Assert(_inputData.Technology.Count == 1);

			_auxiliaryTable = auxiliaryModelFactory.CreateAuxiliaryModel(_type);
			_technologyList = _auxiliaryTable.GetTechnologies().ToList();
		}

		#region Implementation of IAuxiliaryDeclarationType
		protected AuxiliaryType _type;
		protected IList<string> _technology;

		public AuxiliaryType Type
		{
			get => _type;
		}

		public IList<string> Technology {get => _technology; }
		#endregion
	}

	public class AuxiliaryViewModel_v1_0 : AuxiliaryViewModel
	{

		public static readonly string VERSION = typeof(XMLAuxiliaryDeclarationDataProviderV10).FullName;
		public AuxiliaryViewModel_v1_0(IXMLAuxiliaryDeclarationInputData inputData, IAuxiliaryModelFactory auxiliaryModelFactory) : base(inputData, auxiliaryModelFactory)
		{
		}
	}

	public class AuxiliaryViewModel_v2_0 : AuxiliaryViewModel_v1_0
	{
		public static new readonly string VERSION = typeof(XMLAuxiliaryDeclarationDataProviderV20).FullName;
		public AuxiliaryViewModel_v2_0(IXMLAuxiliaryDeclarationInputData inputData, IAuxiliaryModelFactory auxiliaryModelFactory) : base(inputData, auxiliaryModelFactory)
		{
		}
	}

	public class AuxiliaryViewModel_v2_3 : AuxiliaryViewModel_v2_0
	{
		public static new readonly string VERSION = typeof(XMLAuxiliaryDeclarationDataProviderV24_Lorry).FullName;
		public AuxiliaryViewModel_v2_3(IXMLAuxiliaryDeclarationInputData inputData, IAuxiliaryModelFactory auxiliaryModelFactory) : base(inputData, auxiliaryModelFactory)
		{

		}
	}


}
