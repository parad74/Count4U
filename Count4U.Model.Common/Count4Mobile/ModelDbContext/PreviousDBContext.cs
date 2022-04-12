using System.Data.Common;
using System.Data.Entity;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.ModelDbContext
{

	public class AnalyticDBContext : DbContext
	{
		public AnalyticDBContext(DbConnection connection	)
			: base(connection, true)
		{
			this.Configuration.ProxyCreationEnabled = false;
		}

		public AnalyticDBContext()
			: base("name=PreviousDBContext")
		{
			this.Configuration.ProxyCreationEnabled = false;
		}

		public DbSet<PreviousInventory> PreviousInventoryDatas { get; set; }

		protected override void OnModelCreating(
						DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new PreviousInventoryConfiguration());
			// ...и так для каждого объекта конфигурации 
			//modelBuilder.Entity<PreviousInventory>().Property(u => u.PropertyStr1).HasDatabaseGeneratedOption;

			base.OnModelCreating(modelBuilder);
		}

		//protected override void OnModelCreating(DbModelBuilder modelBuilder)
		//{
			//modelBuilder.Entity<Product>().ToTable("products");
			//modelBuilder.Entity<User>().Property(u => u.Age).HasDefaultValue(18);
																																								   
		//	base.OnModelCreating(modelBuilder);
		//}

	
	}
}