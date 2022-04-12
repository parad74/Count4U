using System;

namespace Count4U.Model.Count4U
{
    [Serializable]
	public class UnitPlanValue
	{
        public long ID { get; set; }
		public string UnitPlanCode { get; set; }
		//public int UnitPlanStatusBit { get; set; }
		//public int UnitPalnGroupStatusBit { get; set; }
		public int TotalItur { get; set; }
		public int DoneItur { get; set; }
		public int Done { get; set; }
		public double TotalItem { get; set; }
		public double SumQuantityEdit { get; set; }
		public double DiffQuantityEdit { get; set; }
		public int StatusUnitPlanBit { get; set; }
		public int StatusGroupUnitPlanBit { get; set; }

	
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(UnitPlan)) return false;
			return Equals((UnitPlan)obj);
		}

		public bool Equals(UnitPlan other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.UnitPlanCode, this.UnitPlanCode);
		}

		public override int GetHashCode()
		{
			return (UnitPlanCode != null ? UnitPlanCode.GetHashCode() : 0);
		}

		public UnitPlanValue()
		{
			UnitPlanCode = "";
			StatusUnitPlanBit = 0;
			StatusGroupUnitPlanBit = 0;
			TotalItur = 0;
			DoneItur = 0;
			Done = 0;
			TotalItem = 0;
			SumQuantityEdit = 0;
			DiffQuantityEdit = 0;

		}
	}
}
