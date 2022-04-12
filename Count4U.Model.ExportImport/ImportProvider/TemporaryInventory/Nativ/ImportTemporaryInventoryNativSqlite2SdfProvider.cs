using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public class ImportTemporaryInventoryNativSqlite2SdfProvider : BaseProvider, IImportProvider
	{

		private readonly IImportTemporaryInventoryRepository _importTemporaryInventoryDbSetRepository;

		public ImportTemporaryInventoryNativSqlite2SdfProvider(
				IImportTemporaryInventoryRepository importTemporaryInventoryDbSetRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			if (importTemporaryInventoryDbSetRepository == null) throw new ArgumentNullException("importTemporaryInventoryDbSetRepository");
			this._importTemporaryInventoryDbSetRepository = importTemporaryInventoryDbSetRepository;
  			this._countExcludeFirstString = 0;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importTemporaryInventoryDbSetRepository.InsertTemporaryInventorys(this.FromPathFile, this.ToPathDB,
			TemporaryInventorySQLiteParserEnum.TemporaryInventoryFromDeletedInventorySqlite2SdfParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);

			this._importTemporaryInventoryDbSetRepository.InsertTemporaryInventorys(this.FromPathFile, this.ToPathDB,
			TemporaryInventorySQLiteParserEnum.TemporaryInventoryFromAddedInventorySqlite2SdfParser,
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
		//	this._importLocationADORepository.ClearLocations(this.ToPathDB);
		}



	}
}

