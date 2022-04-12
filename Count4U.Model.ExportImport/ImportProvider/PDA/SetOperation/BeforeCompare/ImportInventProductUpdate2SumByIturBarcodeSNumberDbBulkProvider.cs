using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Interface;
using System.Data.SqlServerCe;
using System.Data;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;
using ErikEJ.SqlCe;

namespace Count4U.Model
{
	public class ImportInventProductUpdate2SumByIturBarcodeSNumberDbBulkProvider : BaseProvider, IImportProvider
	{
		private readonly IImportInventProductBlukRepository _importInventProductRepository;
//		public SqlCeBulkCopyColumnMappingCollection ColumnMappings { set; get; }

		public ImportInventProductUpdate2SumByIturBarcodeSNumberDbBulkProvider(
				IImportInventProductBlukRepository importInventProductRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importInventProductRepository == null) throw new ArgumentNullException("importInventProductRepository");
			this._importInventProductRepository = importInventProductRepository;

			this._importTypes.Add(ImportDomainEnum.ImportInventProduct);
			//this._importTypes.Add(ImportDomainEnum.ImportDocumentHeader);
			//this._importTypes.Add(ImportDomainEnum.ImportItur);
			//this._importTypes.Add(ImportDomainEnum.ImportParentProductAdvanced);
			//this._importTypes.Add(ImportDomainEnum.ExistItur);
			//this._importTypes.Add(ImportDomainEnum.ExistMakat);
			
			//this._importType.Add(ImportDomainEnum.ImportMakat);
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importInventProductRepository.InsertInventProducts(this.FromPathFile, this.ToPathDB,
			InventProductSimpleParserEnum.InventProductUpdate2SumByIturBarcodeSNumberFromDBParser,	 
			 this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms, this.ColumnMappings);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importInventProductRepository.ClearInventProducts(this.ToPathDB);
			//this._importInventProductRepository.ClearDocumentHeaders(this.ToPathDB);
			//this._importInventProductRepository.ClearSession(this.ToPathDB);
			//this._importInventProductRepository.ClearItur(pathDB);
		}

	}
}


