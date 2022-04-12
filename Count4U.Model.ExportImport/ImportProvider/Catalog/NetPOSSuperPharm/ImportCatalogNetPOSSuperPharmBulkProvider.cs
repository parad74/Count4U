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
	public class ImportCatalogNetPOSSuperPharmBulkProvider : BaseProvider, IImportProvider
	{
		private readonly IImportCatalogBlukRepository _importCatalogBlukRepository;

		public ImportCatalogNetPOSSuperPharmBulkProvider(
				IImportCatalogBlukRepository importCatalogBlukRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importCatalogBlukRepository = importCatalogBlukRepository;
			this._importCatalogBlukRepository = importCatalogBlukRepository;

			this._importTypes.Add(ImportDomainEnum.ImportCatalog);
			this._importTypes.Add(ImportDomainEnum.ExistMakat);
			this._importTypes.Add(ImportDomainEnum.BarcodeApplyMask);
			this._importTypes.Add(ImportDomainEnum.MakatApplyMask);
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importCatalogBlukRepository.InsertProduct(this.FromPathFile, this.ToPathDB,
				ProductSimpleParserEnum.ProductNetPOSSuperPharmParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms, this.ColumnMappings);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding(862);
			this._separators = new string[] { SeparatorField.Empty };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importCatalogBlukRepository.ClearProduct(this.ToPathDB);
		}

	}
}
