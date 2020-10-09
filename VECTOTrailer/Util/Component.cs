using TUGraz.VectoCommon.Resources;

namespace VECTOTrailer.Util {
	public enum Component
	{
		CompleteTrailer = 1,
	}

	public static class ComponentHelper
	{
		public static string GetLabel(this Component component)
		{
			return XMLNames.Component_Vehicle;
		}
	}
}