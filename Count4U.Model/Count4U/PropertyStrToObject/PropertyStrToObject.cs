using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	[Serializable]
	public class PropertyStrToObject
	{						
		//на случай если связь будет  M:N 
		public long ID { get; set; }
		public string DomainObject { get; set; }
		public string PropertyStrCode { get; set; }	   //код 	PropertyStr	   M
		public string ObjectCode { get; set; }		 //код  DomainObject N 


		public PropertyStrToObject()
		{
			PropertyStrCode = "";
			DomainObject = "";
			ObjectCode = "";
		}


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStrToObject)) return false;
			return Equals((PropertyStrToObject)obj);
		}

		public bool Equals(PropertyStrToObject other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.PropertyStrCode, this.PropertyStrCode) && Equals(other.DomainObject, this.DomainObject) && Equals(other.ObjectCode, this.ObjectCode);
		}

		public override int GetHashCode()
		{
			return (PropertyStrCode != null ? PropertyStrCode.GetHashCode() : 0);
		}


	}
}
