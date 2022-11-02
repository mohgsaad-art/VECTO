namespace VECTO3GUI2020.Util
{

	public class AllowedEntry
	{
		public static AllowedEntry<T> Create<T>(T label, string description)
		{
			return new AllowedEntry<T>
			{
				Value = label,
				Label = description
			};
		}
	}
	public class AllowedEntry<T>
	{
		public string Label { get; set; }
		public T Value { get; set; }
	}
}
