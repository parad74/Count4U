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
	public class ImportFamilyAS400HamashbirProvider : BaseProvider, IImportProvider
	{
		private readonly IImportFamilyRepository _importFamilyRepository;

		public ImportFamilyAS400HamashbirProvider(IImportFamilyRepository importFamilyRepository,
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
				FamilyParserEnum.FamilyAS400HamashbirParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = System.Text.Encoding.GetEncoding(862);
			this._separators = new string[] { SeparatorField.Empty };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importFamilyRepository.ClearFamilys(this.ToPathDB);
		}
	}
}		 
	