using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27;

namespace VECTO3GUI2020.Resources.XML
{
	public static class XMLTypes
	{
		#region Vehicle

		public const string Vehicle_Conventional_CompletedBusDeclarationType =
			XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE;
		public const string Vehicle_Hev_CompletedBusDeclarationType =
			XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE;
		public const string Vehicle_Pev_CompletedBusDeclarationType =
			XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE;
		public const string Vehicle_Iepc_CompletedBusDeclarationType =
            XMLDeclaration_PEV_CompletedBus_DataProviderV27.XSD_TYPE;
		public const string Vehicle_Exempted_CompletedBusDeclarationType =
			XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE;

		public const string Vehicle_FCHV_CompletedBusDeclarationType =
			XMLDeclaration_FCHV_CompletedBus_DataProviderV27.XSD_TYPE;

		#endregion

		#region Components
		public const string Components_xEV_CompletedBusType = "Components_xEV_CompletedBusType";
		public const string Components_Conventional_CompletedBusType = "Components_Conventional_CompletedBusType";
		#endregion 


		#region Airdrag
		public const string AirDragDataDeclarationType = "AirDragDataDeclarationType";
		public const string AirDragModifiedUseStandardValueType = "AirDragModifiedUseStandardValueType";

        #endregion


        #region BusAux

        public const string AUX_Conventional_CompletedBusType = "AUX_Conventional_CompletedBusType";
		public const string AUX_xEV_CompletedBusType = "AUX_xEV_CompletedBusType";

		#endregion


		#region ADAS

		public const string ADAS_Conventional_Type = "ADAS_Conventional_Type";
		public const string ADAS_HEV_Type = "ADAS_HEV_Type";
		public const string ADAS_PEV_Type = "ADAS_PEV_Type";
		public const string ADAS_IEPC_Type = "ADAS_IEPC_Type";


		#endregion
	}
}

