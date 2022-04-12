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
	public class ImportBranchAS400LeumitProvider  : BaseProvider, IImportProvider
	{
		private readonly IImportBranchRepository _importBranchRepository;

		public ImportBranchAS400LeumitProvider(IImportBranchRepository importBranchRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			if (importBranchRepository == null) throw new ArgumentNullException("importBranchRepository");
			this._importBranchRepository = importBranchRepository;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importBranchRepository.InsertBranchs(this.FromPathFile, this.ToPathDB,
			BranchParserEnum.BranchAS400LeumitParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 1;
		}

		public void Clear()
		{
			//this._importBranchRepository.ClearBranchs(this.ToPathDB);
		}



	
	}
}

