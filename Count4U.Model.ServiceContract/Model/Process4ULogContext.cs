using System.Data.Entity;
using Count4U.Model.ServiceContract.DataContract;

namespace Count4U.Model.ServiceContract.Models
{
	public class Process4ULogContext : DbContext
	{
		public Process4ULogContext()
			: base("name=Process4ULogContext")
		{
			this.Configuration.ProxyCreationEnabled = false;
		}

		public DbSet<Process4UBaseData> Process4UBaseDatas { get; set; }
	}
}