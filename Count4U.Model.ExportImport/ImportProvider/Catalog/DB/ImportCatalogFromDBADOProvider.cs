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
using Count4U.Model.Interface.Count4U;
using ErikEJ.SqlCe;

namespace Count4U.Model
{
	public class ImportCatalogFromDBADOProvider : BaseProvider, IImportProvider
	{
	//	private readonly IImportCatalogSqlBulkRepository _importCatalogSqlBulkRepository;
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;
		private readonly IImportCatalogBlukRepository _importCatalogBlukRepository;
		//private readonly IProductRepository _productRepository;

		public ImportCatalogFromDBADOProvider(
				//IImportCatalogSqlBulkRepository importCatalogSqlBulkRepository,
				IImportCatalogADORepository importCatalogSimpleRepository,
			IImportCatalogBlukRepository importCatalogBlukRepository,
				//IProductRepository productRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			//if (importCatalogSqlBulkRepository == null) throw new ArgumentNullException("importCatalogSqlBulkRepository");
			//this._importCatalogSqlBulkRepository = importCatalogSqlBulkRepository;
			if (importCatalogSimpleRepository == null) throw new ArgumentNullException("importCatalogSimpleRepository");
			this._importCatalogSimpleRepository = importCatalogSimpleRepository;
			this._importCatalogBlukRepository = importCatalogBlukRepository;

			//this._importTypes.Add(ImportDomainEnum.ClearProduct);
			//this._importTypes.Add(ImportDomainEnum.ClearSupplier);
			this._importTypes.Add(ImportDomainEnum.ImportCatalog);
			this._importTypes.Add(ImportDomainEnum.ImportSupplier);
		}


		public void Import()
		{
			base.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;

			this._importCatalogSimpleRepository.InsertProducts(this.FromPathFile, this.ToPathDB,
			ProductSimpleParserEnum.ProductFromDBParser,
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
			this._importCatalogSimpleRepository.ClearProducts(this.ToPathDB);
			this._importCatalogSimpleRepository.ClearSupplier(this.ToPathDB);	
		}
	}
}
