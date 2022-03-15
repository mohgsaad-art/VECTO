namespace TUGraz.VectoCore.Models.Declaration
{
	
	public class SSMTechnology
	{
		public string Category { get; set; }
		public string BenefitName { get; set; }

		public double LowFloorH { get; set; }
		public double LowFloorV { get; set; }
		public double LowFloorC { get; set; }

		public double SemiLowFloorH { get; set; }
		public double SemiLowFloorV { get; set; }
		public double SemiLowFloorC { get; set; }

		public double RaisedFloorH { get; set; }
		public double RaisedFloorV { get; set; }
		public double RaisedFloorC { get; set; }

		
		public bool ActiveVH { get; set; }
		public bool ActiveVV { get; set; }
		public bool ActiveVC { get; set; }


		
	}
}
