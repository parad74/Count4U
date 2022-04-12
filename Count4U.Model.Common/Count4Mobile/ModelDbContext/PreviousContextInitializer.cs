using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Count4U.Model.ModelDbContext
{
	public class AnalyticDBContextInitializer : DropCreateDatabaseAlways<AnalyticDBContext>  // DropCreateDatabaseAlways	<PreviousContext>  //DropCreateDatabaseIfModelChanges
	{
		protected override void Seed(AnalyticDBContext context)
		{
			context.SaveChanges();
		}
	}
}
