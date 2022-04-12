using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Count4U.Model.Common
{
	public class AnalyticDBContextInitializer : CreateDatabaseIfNotExists<AnalyticDBContext>  //DropCreateDatabaseIfModelChanges<AnalyticDBContext>  // DropCreateDatabaseAlways	<AnalyticDBContext>  //DropCreateDatabaseIfModelChanges
	{
		protected override void Seed(AnalyticDBContext context)
		{
			context.SaveChanges();
		}
	}

	//public class AnalyticDBContextInitializer : MigrateDatabaseToLatestVersion<AnalyticDBContext, Configuration>//DropCreateDatabaseIfModelChanges<AnalyticDBContext>  // DropCreateDatabaseAlways	<AnalyticDBContext>  //DropCreateDatabaseIfModelChanges
	//{
	//}
}
