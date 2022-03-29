namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IGearboxControl
	{
		bool DisengageGearbox { set; }
		
		// used by hybrid controller later on
		//void TriggerGearshift(Second absTime, Second dt);
	}
}