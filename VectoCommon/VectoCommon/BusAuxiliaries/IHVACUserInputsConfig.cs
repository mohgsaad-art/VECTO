namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IHVACUserInputsConfig
	{
		// Property  SteadyStateModel As IHVACSteadyStateModel
		/// <summary>
		/// 	PathName of the Steady State Model File
		/// 	</summary>
		/// 	<value></value>
		/// 	<returns></returns>
		/// 	<remarks></remarks>
		//string SSMFilePath { get; set; }

		//string BusDatabasePath { get; set; }

		//bool SSMDisabled { get; set; }

		//ISSMInputs SSMInputs { get; }

		IHVACConstants HVACConstants { get; }

	}
}
