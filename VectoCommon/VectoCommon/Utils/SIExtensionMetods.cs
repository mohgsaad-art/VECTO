namespace TUGraz.VectoCommon.Utils {
	public static class SIExtensionMetods
	{
		/// <summary>
		/// Returns a default value if the SI object is null.
		/// </summary>
		/// <typeparam name="T">The SI Type.</typeparam>
		/// <param name="self">The SI Instance.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>If self is null, the default value as SI-Type is returned. Otherwise self is returned.</returns>
		/// <code>
		/// NewtonMeter t = null;
		/// var x = t.DefaultIfNull(0);
		/// </code>
		public static T DefaultIfNull<T>(this T self, double defaultValue) where T : SIBase<T>
		{
			return self ?? defaultValue.SI<T>();
		}
	}
}