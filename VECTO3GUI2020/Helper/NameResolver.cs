using System.Resources;

namespace VECTO3GUI2020.Helper
{
	public static class NameResolver
	{
		public static string ResolveName(string propertyName, params ResourceManager[] resourceManagers)
		{
			foreach (var resourceManager in resourceManagers)
			{
				var resolvedName = resourceManager?.GetString(propertyName);
				if (!string.IsNullOrEmpty(resolvedName))
				{
					return resolvedName;
				}
			}


			return propertyName;
		}
	}
}