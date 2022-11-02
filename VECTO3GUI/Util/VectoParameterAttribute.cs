using System;

namespace VECTO3GUI.Util {
	[AttributeUsage(AttributeTargets.Property)]
	public class VectoParameterAttribute : Attribute
	{
		public VectoParameterAttribute(Type type, string property)
		{
			Type = type;
			PropertyName = property;
		}

		public Type Type { get; }

		public string PropertyName { get; }
	}
}