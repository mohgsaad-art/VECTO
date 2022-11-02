namespace TUGraz.VectoCommon.BusAuxiliaries {
	public enum FloorType
	{
		Unknown = 0,
		LowFloor,
		HighFloor,
		SemiLowFloor, // no longer required?
	}

	//public static class FloorTypeHelper
	//{
	//	public static string GetLabel(this FloorType self)
	//	{
	//		switch (self)
	//		{
	//			case FloorType.SemiLowFloor:
	//			case FloorType.LowFloor:
	//				return "Low Floor";
	//			case FloorType.HighFloor:
	//				return "High Floor";
	//			default:
	//				return "Unknown";

	//		}
	//	}
	//}
}