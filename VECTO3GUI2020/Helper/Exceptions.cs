using System;

namespace VECTO3GUI2020.Helper
{
	/// <summary>
	/// Exception to notify the view that a field is not allowed to be empty. Should be ignored by the debugger
	/// </summary>
	public class VectoEmptyFieldException : Exception
	{
		public VectoEmptyFieldException()
			: base("Field must not be empty")
		{

		}

		public VectoEmptyFieldException(string message)
			: base(message)
		{
		}

		public VectoEmptyFieldException(string message, Exception inner)
			: base(message, inner)
		{
		}
    }
}