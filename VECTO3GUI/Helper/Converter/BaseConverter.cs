using System;
using System.Windows.Markup;

namespace VECTO3GUI.Helper.Converter {
	public class BaseConverter : MarkupExtension
	{
		#region Overrides of MarkupExtension

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		#endregion
	}
}