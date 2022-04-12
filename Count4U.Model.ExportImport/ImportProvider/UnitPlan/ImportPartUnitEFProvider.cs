using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public class ImportPartUnitEFProvider : BaseProvider, IImportProvider
	{
		private readonly IImportUnitPlanRepository _importUnitPlanRepository;

		public ImportPartUnitEFProvider(IImportUnitPlanRepository importUnitPlanRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			if (importUnitPlanRepository == null) throw new ArgumentNullException("importUnitPlanRepository");
			this._importUnitPlanRepository = importUnitPlanRepository;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importUnitPlanRepository.InsertUnitPlans(this.FromPathFile, this.ToPathDB,
			UnitPlanParserEnum.UnitPlanParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importUnitPlanRepository.ClearUnitPlans(this.ToPathDB);
		}



	}
}

