using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.ModelConfiguration;

namespace Count4U.Model.Count4Mobile
{
	public class TemporaryInventoryConfiguration : EntityTypeConfiguration<TemporaryInventory>
	{

		public TemporaryInventoryConfiguration()
		{
			this.ToTable("TemporaryInventory");

			this.Property(p => p.Operation).HasMaxLength(50).IsRequired();
			this.Property(p => p.Domain).HasMaxLength(50).IsRequired();
			this.Property(p => p.OldUid).HasMaxLength(250).IsRequired();			  //parent					 250
			this.Property(p => p.NewUid).HasMaxLength(250).IsRequired();			//	chalde					 250

			this.Property(p => p.OldSerialNumber).HasMaxLength(250).IsRequired();		 // 250
			this.Property(p => p.OldItemCode).HasMaxLength(250).IsRequired();			   // 250
			this.Property(p => p.OldLocationCode).HasMaxLength(250).IsRequired();		 // 250
			this.Property(p => p.OldProductCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.OldKey).HasMaxLength(50).IsRequired();

			this.Property(p => p.NewSerialNumber).HasMaxLength(250).IsRequired();  // 250
			this.Property(p => p.NewItemCode).HasMaxLength(250).IsRequired();		  // 250
			this.Property(p => p.NewLocationCode).HasMaxLength(250).IsRequired();	   // 250
			this.Property(p => p.NewProductCode).HasMaxLength(50).IsRequired();
			this.Property(p => p.NewKey).HasMaxLength(50).IsRequired();

			this.Property(p => p.DateModified).HasMaxLength(50).IsRequired();
			this.Property(p => p.Device ).HasMaxLength(50).IsRequired();
			this.Property(p => p.DbFileName ).HasMaxLength(250).IsRequired();
			this.Property(p => p.Tag).HasMaxLength(50).IsRequired();
			this.Property(p => p.Description).HasMaxLength(250).IsRequired();
		

		}
	}
}

