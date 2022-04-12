using System;
using System.Collections.Generic;
using System.Data.Entity;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Count4U.Model.ServiceContract.Models
{
	public class Process4ULogContextInitializer : CreateDatabaseIfNotExists<Process4ULogContext>  //DropCreateDatabaseAlways<Sip2Context>
	{
		protected override void Seed(Process4ULogContext context)
		{
			context.SaveChanges();
		}
	}
}
