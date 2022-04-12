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

	//Пока не забираем c db3 данные
	public class ImportTemplateInventoryNativSqlite2SdfProvider : BaseProvider, IImportProvider
	{

		private readonly IImportTemplateInventoryRepository _importTemplateInventoryDbSetRepository;

		public ImportTemplateInventoryNativSqlite2SdfProvider(
				IImportTemplateInventoryRepository importTemplateInventoryDbSetRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			if (importTemplateInventoryDbSetRepository == null) throw new ArgumentNullException("importTemporaryInventoryDbSetRepository");
			this._importTemplateInventoryDbSetRepository = importTemplateInventoryDbSetRepository;
  			this._countExcludeFirstString = 1;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importTemplateInventoryDbSetRepository.InsertTemplateInventorys(this.FromPathFile, this.ToPathDB,
			TemplateInventorySQLiteParserEnum.TemplateInventoryFromTemplateSqlite2SdfParser,
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
		//	this._importLocationADORepository.ClearLocations(this.ToPathDB);
		}



	}
}

