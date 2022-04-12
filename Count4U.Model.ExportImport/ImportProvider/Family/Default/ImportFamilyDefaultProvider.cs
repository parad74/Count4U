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
	public class ImportFamilyDefaultProvider : BaseProvider, IImportProvider
	{
		private readonly IImportFamilyRepository _importFamilyRepository;

		public ImportFamilyDefaultProvider(IImportFamilyRepository importFamilyRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importFamilyRepository == null) throw new ArgumentNullException("importFamilyRepository");
			this._importFamilyRepository = importFamilyRepository;
			this._importTypes.Add(ImportDomainEnum.ImportFamily);
			this._countExcludeFirstString = 0;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importFamilyRepository.InsertFamilys(this.FromPathFile, this.ToPathDB,
				FamilyParserEnum.FamilyDefaultParser,
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
			this._importFamilyRepository.ClearFamilys(this.ToPathDB);
		}

	
	}
}		 
	