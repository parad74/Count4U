using System;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	[Serializable]
	public class PropertyStr
	{
		public long ID { get; set; }
		[PropertyLink]	public string DomainObject { get; set; }
		public string TypeCode { get; set; }	   //тип 	PropertyStr
		public string PropertyStrCode { get; set; }	   //код 	PropertyStr
		public string Name { get; set; }
		public string Code { get; set; }	 //на случай если связь будет 1:N или 1:1, на случай M:N другая таблица


		public PropertyStr()
		{
			TypeCode = "";
			Name = "";
			PropertyStrCode = "";
			DomainObject = "Unknown";
			Code = "";
		}


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PropertyStr)) return false;
			return Equals((PropertyStr)obj);
		}

		public bool Equals(PropertyStr other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.DomainObject, this.DomainObject) && Equals(other.TypeCode, this.TypeCode) && Equals(other.PropertyStrCode, this.PropertyStrCode);
		}

		public override int GetHashCode()
		{
			return (PropertyStrCode != null ? PropertyStrCode.GetHashCode() : 0);
		}

		public PropertyStr Clone()
		{
			return new PropertyStr()
			{
				TypeCode = this.TypeCode,
				PropertyStrCode = this.PropertyStrCode,
				Name = this.Name,
				DomainObject = this.DomainObject,
				Code = this.Code
			};


		}

		
	}
}
