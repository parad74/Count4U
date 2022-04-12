using System;
using System.Collections.Generic;
using Count4U.Model.Extensions;

namespace Count4U.Model.Count4Mobile
{
    public class PropertyDecorator
    {
    	public long ID { get; set; }
		public string DomainObject { get; set; }		 //PropertyDecorator
		public string DocumentSheet { get; set; }		  //TypeCode
		public string Row { get; set; }						 //PropertyStrCode 
		public string Col { get; set; }	 						//Code
		public string Name { get; set; }						//значение 
	


		public PropertyDecorator()
		{
			DomainObject = "Doc1";
			DocumentSheet = "1";
			Row = "0";
			Col = "0";
			Name = "";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyDecorator)) return false;
			return Equals((PropertyDecorator)obj);
		}

		public bool Equals(PropertyDecorator other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.ID, this.ID));
		}

		public override int GetHashCode()
		{
			return (ID != null ? ID.GetHashCode() : 0);
		}
    }

	
}
