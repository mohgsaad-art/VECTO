namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public abstract class AbstractModule
	{
		protected bool calculationValid { get; private set; }

		public AbstractModule()
		{
			calculationValid = false;
		}

		public virtual void ResetCalculations()
		{
			calculationValid = false;
		}

		protected virtual void Calculate()
		{
			DoCalculate();
			calculationValid = true;
		}

		protected virtual void DoCalculate() { }
	}
}
