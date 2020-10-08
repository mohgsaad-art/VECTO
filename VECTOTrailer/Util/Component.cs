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
			switch (component)
			{
				case Component.CompleteTrailer:
					return XMLNames.Component_Vehicle;
			}

			return string.Empty;
		}
	}
}