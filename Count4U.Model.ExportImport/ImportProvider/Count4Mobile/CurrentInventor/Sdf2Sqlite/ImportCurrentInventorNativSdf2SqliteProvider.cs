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
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model
{
	public class ImportCurrentInventorNativSdf2SqliteProvider : BaseProvider, IImportProvider
	{
		private readonly IImportCurrentInventorSQLiteADORepository _importCurrentInventorSQLiteADORepository;

		public ImportCurrentInventorNativSdf2SqliteProvider(
				IImportCurrentInventorSQLiteADORepository importCurrentInventorRepository,
				//IConnectionADO connection ,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			//if (importCatalogRepository == null) throw new ArgumentNullException("importCatalogSimpleRepository");
			this._importCurrentInventorSQLiteADORepository = importCurrentInventorRepository;
			//IConnectionADO connection = serviceLocator.GetInstance<IConnectionADO>();
			//this._importCatalogRepository = new ImportCatalogSQLiteADORepository(connection, serviceLocator, log);
			//this._importTypes.Add(ImportDomainEnum.ImportCatalog);
			this._importTypes.Add(ImportDomainEnum.ExistMakat); //? надо или нет заполнять словарь с макатами
			this._importTypes.Add(ImportDomainEnum.ImportСurrentInventory);
			//this._importTypes.Add(ImportDomainEnum.MakatApplyMask);
			this._countExcludeFirstString = 1;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;		//GetDbPath					
			if (this.IsEmptyPath(this.ToPathDB) == true) return;			//db3Path

   			this._importCurrentInventorSQLiteADORepository.InsertCurrentInventors(this.FromPathFile, 
				this.ToPathDB,
			CurrentInventorSQLiteParserEnum.CurrentInventoryNativSdf2SqliteParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding(1255);
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 1;
		}

		public void Clear()
		{
//			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
//this._serviceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
//			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(this.FromPathFile);

			this._importCurrentInventorSQLiteADORepository.ClearCurrentInventors(this.ToPathDB);
			this._importCurrentInventorSQLiteADORepository.VacuumCurrentInventory(this.ToPathDB);
		}

	}
}
