using System;

namespace TUGraz.VectoCommon.Utils
{
	public class GuiLabelAttribute : Attribute
	{
		public string Label { get; private set; }
		public GuiLabelAttribute(string label)
		{
			Label = label;
		}
	}
}