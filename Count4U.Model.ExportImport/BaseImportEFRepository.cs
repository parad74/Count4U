using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using NLog;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public abstract class BaseImportEFRepository : BaseEFRepository
	{
		private readonly ILog _log;
 		public readonly IServiceLocator _serviceLocator;

		public BaseImportEFRepository(IConnectionDB connection,
			ILog log, IServiceLocator serviceLocator) :
			base(connection)
		{
			this._log = log;
			this._serviceLocator = serviceLocator;
		}

		public ILog Log
		{
			get { return this._log; }
		}

	}
}  
 