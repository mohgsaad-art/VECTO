using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC 
{

	public interface ISSMPowerDemand
	{
		Watt ElectricalWAdjusted { get; } // Watt
		Watt MechanicalWBaseAdjusted { get; } // Watt

		Watt AverageAuxHeaterPower(Watt averageUseableEngineWasteHeat);

	}

	public interface ISSMTOOL : ISSMPowerDemand
	{
		ISSMDeclarationInputs SSMInputs { get;  }
		ISSMTechnologyBenefits TechList { get;  }
		ISSMCalculate Calculate { get;  }
        IHVACConstants HVACConstants { get; set; }

        //Watt ElectricalWBase { get; } // Watt
		//Watt MechanicalWBase { get; } // Watt
		//KilogramPerSecond FuelPerHBase { get; } // LiterPerHour


		//KilogramPerSecond FuelPerHBaseAdjusted { get; } // LiterPerHour

		Watt EngineWasteHeat { get; }


		///// <summary>
		///// 
		///// </summary>
		///// <param name="averageUseableEngineWasteHeat"></param>
		///// <returns></returns>
		///// <remarks></remarks>
		//Watt AverageAuxHeaterPower(Watt averageUseableEngineWasteHeat);

	}

	//public delegate void MessageEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);
}