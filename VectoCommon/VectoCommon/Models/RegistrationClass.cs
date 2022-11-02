using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	public enum RegistrationClass
	{

		[GuiLabel("unknown")]
		unknown,
		[GuiLabel("I")]
		I,

		[GuiLabel("I & II")]
		I_II,

		[GuiLabel("II")]
		II,

		[GuiLabel("II & III")]
		II_III,

		[GuiLabel("III")]
		III,

		[GuiLabel("A")]
		A,

		[GuiLabel("B")]
		B,
		
	}
	
	public static class RegistrationClassHelper
	{
		public static string GetLabel(this RegistrationClass? self)
		{
			switch (self) {
				case RegistrationClass.I_II:
					return "I+II";
				case RegistrationClass.II_III:
					return "II+III";
				default:
					return self.ToString();
			}
		}




		public static string ToXMLFormat(this RegistrationClass? self)
		{
			return self.GetLabel();
		}

		public static RegistrationClass?[] Parse(string registrationClasses)
		{
			var classes = registrationClasses.Split('/');
			if (classes.Length > 0) {
				var result = new List<RegistrationClass?>();
				for (int i = 0; i < classes.Length; i++) {

					var regClass = ParseRegistrationClassString(classes[i]);
					if(regClass != RegistrationClass.unknown)
						result.Add(regClass);
				}
				return result.ToArray();
			}
			return null;
		}
		
		private static RegistrationClass? ParseRegistrationClassString(string registrationClass)
		{
			if (((RegistrationClass?)RegistrationClass.I).GetLabel() == registrationClass)
				return RegistrationClass.I;

			if (((RegistrationClass?)RegistrationClass.II).GetLabel() == registrationClass)
				return RegistrationClass.II;

			if (((RegistrationClass?)RegistrationClass.III).GetLabel() == registrationClass)
				return RegistrationClass.III;

			if (((RegistrationClass?)RegistrationClass.II_III).GetLabel() == registrationClass)
				return RegistrationClass.II_III;

			if (((RegistrationClass?)RegistrationClass.I_II).GetLabel() == registrationClass)
				return RegistrationClass.I_II;

			if (((RegistrationClass?)RegistrationClass.A).GetLabel() == registrationClass)
				return RegistrationClass.A;

			if (((RegistrationClass?)RegistrationClass.B).GetLabel() == registrationClass)
				return RegistrationClass.B;

			return RegistrationClass.unknown;
		}
	}
}
