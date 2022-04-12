using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;

namespace Count4U.Model.Main
{
    public class CustomerConfig// : ICustomerConfig
    {
        public long ID { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
		public string CustomerCode { get; set; }
		//TO ADD in COUNT4U
		// сохранить  Value -> 	Description
		//UPDATE CustomerConfig
		//SET          Description = Value
		//Text = @"UPDATE CustomerConfig  SET  Description = Value;"
		//Text = @"ALTER TABLE [CustomerConfig] DROP COLUMN [Value];"
		//Text = @"ALTER TABLE [CustomerConfig] add [Value] nvarchar(500) NULL DEFAULT '';"
		//Text = @"UPDATE CustomerConfig  SET  Value = Description;"

		// public string Value { get; set; } добавить длинну 10 => 500
		// скопировать  Description	->	Value

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(CustomerConfig)) return false;
			return Equals((CustomerConfig)obj);
		}

		public bool Equals(CustomerConfig other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Name, this.Name);
		}

		public override int GetHashCode()
		{
			return (this.Name.GetHashCode());
		}


	}
}
