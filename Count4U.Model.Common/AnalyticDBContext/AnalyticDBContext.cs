using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Common
{
	//public sealed class Configuration : DbMigrationsConfiguration<AnalyticDBContext>
	//{
	//	public Configuration()
	//	{
	//		AutomaticMigrationsEnabled = true;
	//	}

	//	protected override void Seed(AnalyticDBContext context)
	//	{

	//	}
	//}

	//	using (var client = new AnalyticDBContext())
	//{
	//	client.Database.EnsureCreated();
	//}

	public class AnalyticDBContext : DbContext
	{
	
		public AnalyticDBContext(DbConnection connection	)
			: base(connection, true)
		{
			Database.SetInitializer<AnalyticDBContext>(new CreateDatabaseIfNotExists<AnalyticDBContext>());
			//Database.SetInitializer<UserDbContext>(new MigrateDatabaseToLatestVersion<UserDbContext, Configuration>());
			this.Configuration.ProxyCreationEnabled = false;
			//this.Configuration.AutoDetectChangesEnabled = true;
		}

	
		public AnalyticDBContext()
			: base("AnalyticDB")
		{
			Database.SetInitializer<AnalyticDBContext>(new CreateDatabaseIfNotExists<AnalyticDBContext>());
			//Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AnalyticDBContext>());
			this.Configuration.ProxyCreationEnabled = false;
			//this.Configuration.AutoDetectChangesEnabled = true;
		}

		//public AnalyticDBContext()
		//	: base("AnalyticDB")
		//{
		//	Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AnalyticDBContext>());
		//}

		public DbSet<PreviousInventory> PreviousInventoryDatas { get; set; }
		public DbSet<CurrentInventory> CurrentInventoryDatas { get; set; }
		public DbSet<CurrentInventoryAdvanced> CurrentInventoryAdvancedDatas { get; set; }
		public DbSet<TemporaryInventory> TemporaryInventoryDatas { get; set; }
		public DbSet<TemplateInventory> TemplateInventoryDatas { get; set; }
		
																			 

		protected override void OnModelCreating(
						DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new PreviousInventoryConfiguration());
			modelBuilder.Configurations.Add(new CurrentInventoryConfiguration());
			modelBuilder.Configurations.Add(new CurrentInventoryAdvancedConfiguration());
			modelBuilder.Configurations.Add(new TemporaryInventoryConfiguration());
			modelBuilder.Configurations.Add(new TemplateInventoryConfiguration());
		
			
			// ...и так для каждого объекта конфигурации 
			//modelBuilder.Entity<PreviousInventory>().Property(u => u.PropertyStr1).HasDatabaseGeneratedOption;

			base.OnModelCreating(modelBuilder);
		}

		public void TryCreateDB()
		{
			try
			{
				this.PreviousInventoryDatas.Add(new PreviousInventory() { Uid = "-1000" });
				this.SaveChanges();
				PreviousInventory entity = this.PreviousInventoryDatas.Where(e => e.Uid == "-1000").FirstOrDefault();
				if (entity != null) this.PreviousInventoryDatas.Remove(entity);
				this.SaveChanges();

				try
				{
					this.TemporaryInventoryDatas.Add(new TemporaryInventory() { NewUid = "-1000" });
					this.SaveChanges();
					TemporaryInventory entity1 = this.TemporaryInventoryDatas.Where(e => e.NewUid == "-1000").FirstOrDefault();
					if (entity1 != null) this.TemporaryInventoryDatas.Remove(entity1);
					this.SaveChanges();
				}
				catch { }

				try
				{
					this.TemplateInventoryDatas.Add(new TemplateInventory() { Tag = "-1000" });
					this.SaveChanges();
					TemplateInventory entity1 = this.TemplateInventoryDatas.Where(e => e.Tag == "-1000").FirstOrDefault();
					if (entity1 != null) this.TemplateInventoryDatas.Remove(entity1);
					this.SaveChanges();
				}
				catch { }
			}
			catch { }
		}

		public void TryCreateCurrentInventoryAdvanced()
		{
			try
			{
				this.CurrentInventoryAdvancedDatas.Add(new CurrentInventoryAdvanced() { Uid = "-1000" });
				this.SaveChanges();
				PreviousInventory entity = this.PreviousInventoryDatas.Where(e => e.Uid == "-1000").FirstOrDefault();
				if (entity != null) this.PreviousInventoryDatas.Remove(entity);
				this.SaveChanges();

				try
				{
					this.TemporaryInventoryDatas.Add(new TemporaryInventory() { NewUid = "-1000" });
					this.SaveChanges();
					TemporaryInventory entity1 = this.TemporaryInventoryDatas.Where(e => e.NewUid == "-1000").FirstOrDefault();
					if (entity1 != null) this.TemporaryInventoryDatas.Remove(entity1);
					this.SaveChanges();
				}
				catch { }
			}
			catch { }
		}

		public void TryCreateTemporaryInventory()
		{
			try
			{
				this.TemporaryInventoryDatas.Add(new TemporaryInventory() { NewUid = "-1000" });
				this.SaveChanges();
				TemporaryInventory entity = this.TemporaryInventoryDatas.Where(e => e.NewUid == "-1000").FirstOrDefault();
				if (entity != null) this.TemporaryInventoryDatas.Remove(entity);
				this.SaveChanges();
			}
			catch (Exception error)
			{
				;
			}
		}


		public void TryCreateTemplateInventory()
		{
			try
			{
				this.TemplateInventoryDatas.Add(new TemplateInventory() { Tag = "-1000" });
				this.SaveChanges();
				TemplateInventory entity1 = this.TemplateInventoryDatas.Where(e => e.Tag == "-1000").FirstOrDefault();
				if (entity1 != null) this.TemplateInventoryDatas.Remove(entity1);
				this.SaveChanges();
			}
			catch (Exception error)
			{
				;
			}
		}


		public void TryCreateCurrentInventory()
		{
			try
			{
				this.CurrentInventoryDatas.Add(new CurrentInventory() { Uid = "-1000" });
				this.SaveChanges();
				CurrentInventory entity = this.CurrentInventoryDatas.Where(e => e.Uid == "-1000").FirstOrDefault();
				if (entity != null) this.CurrentInventoryDatas.Remove(entity);
				this.SaveChanges();
			}
			catch (Exception error)
			{
				;
			}
		}

		public void TryCreatePreviousInventory()
		{
			try
			{
				this.PreviousInventoryDatas.Add(new PreviousInventory() { Uid = "-1000" });
				this.SaveChanges();
				PreviousInventory entity = this.PreviousInventoryDatas.Where(e => e.Uid == "-1000").FirstOrDefault();
				if (entity != null) this.PreviousInventoryDatas.Remove(entity);
				this.SaveChanges();
			}
			catch (Exception error)
			{
				;
			}
		}

		//protected override void OnModelCreating(DbModelBuilder modelBuilder)
		//{
			//modelBuilder.Entity<Product>().ToTable("products");
			//modelBuilder.Entity<User>().Property(u => u.Age).HasDefaultValue(18);
																																								   
		//	base.OnModelCreating(modelBuilder);
		//}

	
	}
}