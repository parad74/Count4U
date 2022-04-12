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
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public class ImportCatalogAS400HonigmanProvider2 : BaseProvider, IImportProvider
	{
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;
		private readonly IImportCatalogBlukRepository _importCatalogBlukRepository;
	//	private ISupplierRepository _supplierRepository;

		public ImportCatalogAS400HonigmanProvider2(
				IImportCatalogADORepository importCatalogSimpleRepository,
			IImportCatalogBlukRepository importCatalogBlukRepository,
			//	ISupplierRepository supplierRepository,
				IServiceLocator serviceLocator,
				ILog log
			)
			: base(log, serviceLocator)
		{
			if (importCatalogSimpleRepository == null) throw new ArgumentNullException("importCatalogSimpleRepository");
		//	if (supplierRepository == null) throw new ArgumentNullException("supplierRepository");

			this._importCatalogSimpleRepository = importCatalogSimpleRepository;
			this._importCatalogBlukRepository = importCatalogBlukRepository;
		//	this._supplierRepository = supplierRepository;
			
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
			if (this.FastImport == true)
			{
				this._importCatalogBlukRepository.InsertProduct(this.FromPathFile, this.ToPathDB,
				ProductSimpleParserEnum.ProductCatalogAS400HonigmanParser2,
				this.ProviderEncoding, this._separators, this._countExcludeFirstString,
				this._importTypes, this.Parms, this.ColumnMappings);
			}
			else
			{
				this._importCatalogSimpleRepository.InsertProducts(this.FromPathFile, this.ToPathDB,
				ProductSimpleParserEnum.ProductCatalogAS400HonigmanParser2,
				this.ProviderEncoding, this._separators, this._countExcludeFirstString,
				this._importTypes, this.Parms);
			}
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Tab };
			this._countExcludeFirstString = 1;
		}

		public void Clear()
		{
			this._importCatalogSimpleRepository.ClearProducts(this.ToPathDB);
	
		}
	}
}
