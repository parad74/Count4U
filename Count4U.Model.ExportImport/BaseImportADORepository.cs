using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public abstract class BaseImportADORepository : BaseADORepository
    {
		public BaseImportADORepository(IConnectionADO connectionADO,
			IDBSettings dbSettings,
			ILog log, IServiceLocator serviceLocator) :
			base(connectionADO, dbSettings, log, serviceLocator)
		{
		
		}
    }
}
