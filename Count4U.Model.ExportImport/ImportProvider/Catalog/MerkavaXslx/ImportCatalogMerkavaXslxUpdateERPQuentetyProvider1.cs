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
	public class ImportCatalogMerkavaXslxUpdateERPQuentetyProvider1 : BaseProvider, IImportProvider
	{
		private readonly IImportCatalogADORepository _importCatalogSimpleRepository;
		private readonly IImportCatalogBlukRepository _importCatalogBlukRepository;

		public ImportCatalogMerkavaXslxUpdateERPQuentetyProvider1(
				IImportCatalogADORepository importCatalogSimpleRepository,
			IImportCatalogBlukRepository importCatalogBlukRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importCatalogSimpleRepository == null) throw new ArgumentNullException("importCatalogSimpleRepository");
			this._importCatalogSimpleRepository = importCatalogSimpleRepository;
			this._importCatalogBlukRepository = importCatalogBlukRepository;
	
			this._importTypes.Add(ImportDomainEnum.ImportCatalog);
			this._importTypes.Add(ImportDomainEnum.ExistMakat);
			this._importTypes.Add(ImportDomainEnum.BarcodeApplyMask);
			this._importTypes.Add(ImportDomainEnum.MakatApplyMask);
			//this._countExcludeFirstString = 1;
		}

		public  void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this.Parms[ImportProviderParmEnum.DBPath] = this.ToPathDB;
			if (this.FastImport == true)
			{
				this._importCatalogBlukRepository.InsertProduct(this.FromPathFile, this.ToPathDB,
				ProductSimpleParserEnum.ProductCatalogMerkavaXslx2SdfUpdateERPQuentetyDBParser1,
				this.ProviderEncoding, this._separators, this._countExcludeFirstString,
				this._importTypes, this.Parms, this.ColumnMappings);
			}
			else
			{
				this._importCatalogSimpleRepository.InsertProducts(this.FromPathFile, this.ToPathDB,
					ProductSimpleParserEnum.ProductCatalogMerkavaXslx2SdfUpdateERPQuentetyDBParser1,
					this.ProviderEncoding, this._separators, this._countExcludeFirstString,
					this._importTypes, this.Parms);
			}
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding(1255);
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 4;
		}

		public void Clear()
		{
			this._importCatalogSimpleRepository.ClearProducts(this.ToPathDB);
		}
	}
}
