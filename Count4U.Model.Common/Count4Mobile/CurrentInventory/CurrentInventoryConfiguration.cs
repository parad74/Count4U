using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.ModelConfiguration;

namespace Count4U.Model.Count4Mobile
{
	public class CurrentInventoryConfiguration : EntityTypeConfiguration<CurrentInventory>
	{
		public CurrentInventoryConfiguration()
		{
			this.ToTable("CurrentInventory");
			this.Property(p => p.Uid).HasMaxLength(250).IsRequired();
			this.Property(p => p.SerialNumberLocal).HasMaxLength(250).IsRequired();
			this.Property(p => p.ItemCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.SerialNumberSupplier).HasMaxLength(50).IsRequired();
			this.Property(p => p.Quantity).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr1).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr2).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr3).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr4).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr5).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr6).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr7).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr8).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr9).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr10).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr11).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr12).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr13).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr14).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr15).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr16).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr17).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr18).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr19).HasMaxLength(50).IsRequired();
			this.Property(p => p.PropertyStr20).HasMaxLength(50).IsRequired();
  			this.Property(p => p.LocationCode).HasMaxLength(250).IsRequired();
			this.Property(p => p.DateModified).HasMaxLength(50).IsRequired();
			this.Property(p => p.DateCreated).HasMaxLength(50).IsRequired();
		
			//TO DO
			// public string ItemStatus { get; set; }
			//this.Property(p => p.UnitTypeCode).HasMaxLength(50).IsRequired();
			//this.Property(p => p.ItemType).HasMaxLength(100).IsRequired();

		}
	}
}

