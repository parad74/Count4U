using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.ModelConfiguration;

namespace Count4U.Model.Count4Mobile
{
	public class TemplateInventoryConfiguration : EntityTypeConfiguration<TemplateInventory>
	{

		public TemplateInventoryConfiguration()
		{
			this.ToTable("TemplateInventory");

			this.Property(p => p.Uid).HasMaxLength(250).IsRequired();
			this.Property(p => p.Level1Code).HasMaxLength(50).IsRequired();
			this.Property(p => p.Level2Code).HasMaxLength(50).IsRequired();
			this.Property(p => p.Level3Code).HasMaxLength(50).IsRequired();
			this.Property(p => p.Level4Code).HasMaxLength(50).IsRequired();
			this.Property(p => p.ItemCode).HasMaxLength(250).IsRequired();
			this.Property(p => p.QuantityExpected).HasMaxLength(50).IsRequired();
			this.Property(p => p.Tag).HasMaxLength(250).IsRequired();
			this.Property(p => p.Domain).HasMaxLength(50).IsRequired();
		}
	}
}

