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
	
	public class ImportCatalogNativPlusMISSdf2SqliteProvider : BaseProvider, IImportProvider
	{
		private readonly IImportCatalogSQLiteADORepository _importCatalogSQLiteADORepository;

		public ImportCatalogNativPlusMISSdf2SqliteProvider(
				IImportCatalogSQLiteADORepository importCatalogRepository,
				//IConnectionADO connection ,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			//if (importCatalogRepository == null) throw new ArgumentNullException("importCatalogSimpleRepository");
			this._importCatalogSQLiteADORepository = importCatalogRepository;
			//IConnectionADO connection = serviceLocator.GetInstance<IConnectionADO>();
			//this._importCatalogRepository = new ImportCatalogSQLiteADORepository(connection, serviceLocator, log);
			this._importTypes.Add(ImportDomainEnum.ImportCatalog);
			//this._importTypes.Add(ImportDomainEnum.ExistMakat);
			//this._importTypes.Add(ImportDomainEnum.BarcodeApplyMask);
			//this._importTypes.Add(ImportDomainEnum.MakatApplyMask);
			this._countExcludeFirstString = 1;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;		//GetDbPath					
			if (this.IsEmptyPath(this.ToPathDB) == true) return;			//db3Path

			this._importCatalogSQLiteADORepository.DropCatalogIndexTable(this.ToPathDB);

   			this._importCatalogSQLiteADORepository.InsertCatalogs(this.FromPathFile, 
				this.ToPathDB,
				CatalogSQLiteParserEnum.CatalogNativPlusMISSdf2SqliteParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);

			this._importCatalogSQLiteADORepository.CreateCatalogIndexTable(this.ToPathDB);
		
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding(1255);
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 1;
		}

		public void Clear()
		{
			this._importCatalogSQLiteADORepository.DropCatalogIndexTable(this.ToPathDB);
			this._importCatalogSQLiteADORepository.ClearCatalogs(this.ToPathDB);
			this._importCatalogSQLiteADORepository.VacuumCatalogs(this.ToPathDB);
		}

	}
}
