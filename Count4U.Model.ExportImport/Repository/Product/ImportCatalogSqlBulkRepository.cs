using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using System.Threading;
using ErikEJ.SqlCe;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ImportCatalogSqlBulkRepository : BaseCopyBulkRepository, IImportCatalogSqlBulkRepository
	{
		private readonly ICatalogConfigRepository _catalogConfigRepository;
		private ISupplierRepository _supplierRepository;
		private IProductRepository _productRepository;


		public ImportCatalogSqlBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			ICatalogConfigRepository catalogConfigRepository,
			IProductRepository productRepository,
			ISupplierRepository supplierRepository)
			: base(connection, dbSettings, log, serviceLocator)
        {
			if (catalogConfigRepository == null) throw new ArgumentNullException("catalogConfigRepository");
			if (supplierRepository == null) throw new ArgumentNullException("supplierRepository");
			
			this._catalogConfigRepository = catalogConfigRepository;
			this._supplierRepository = supplierRepository;
			this._productRepository = productRepository;
	    }

		public void CopyCatalog(string fromPathDB, string toPathDB,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			#region Product
			string tableName = @"Product";
			if (cancellationToken.IsCancellationRequested == true) return;
			long count = base.CountRow(tableName, fromPathDB);
			countAction(0);
			//Localization.Resources.Log_TraceRepositoryResult1061%"Source Table [{0}] in DB [{1}] {2} have [{3}] Row"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1061,
				tableName, fromPathDB, Environment.NewLine, count));
			if (importType.Contains(ImportDomainEnum.ClearProduct) == true)
			{
				this.ClearProduct(toPathDB);
			}
			if (importType.Contains(ImportDomainEnum.ImportCatalog) == true)
			{
				//string queryString = "SELECT  Product.*  FROM  Product";
				this.CopyTable(tableName, fromPathDB, toPathDB);
			}
			count = base.CountRow(tableName, toPathDB);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1062, 
				tableName, toPathDB, Environment.NewLine, count));

			this._productRepository.SetLastUpdatedCatalog(toPathDB);

			this.Log.Add(MessageTypeEnum.TraceRepository, "");
			#endregion Product

			#region Supplier
			tableName = @"Supplier";
			if (cancellationToken.IsCancellationRequested == true) return;
			//countAction(0);
			//Localization.Resources.Log_TraceRepositoryResult1061%"Source Table [{0}] in DB [{1}] {2} have [{3}] Row"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1061, 
				tableName, fromPathDB ,Environment.NewLine ,count));
			if (importType.Contains(ImportDomainEnum.ClearSupplier) == true)
			{
				this.ClearSupplier(toPathDB);
			}
			if (importType.Contains(ImportDomainEnum.ImportSupplier) == true)
			{
				//string queryString = "SELECT  Supplier.*  FROM  Supplier";
				this.CopyTable(tableName, fromPathDB, toPathDB);
			}
			count = base.CountRow(tableName, toPathDB);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1062,
				tableName, toPathDB, Environment.NewLine, count));

			#endregion Supplier
		}

		public void ClearProduct(string pathDB)
		{
			DeleteTable(@"Product", pathDB);
			this._productRepository.SetLastUpdatedCatalog(pathDB);
		}

		public void ClearSupplier(string pathDB)
		{
			DeleteTable(@"Supplier", pathDB);
		}

		public void FillLogFromErrorBitList(List<BitAndRecord> errorBitList)
		{
			if (errorBitList == null) return;
			if (errorBitList.Count == 0) return;
			//Log_TraceParser1001%"Parser Error And Message : "
			this.Log.Add(MessageTypeEnum.TraceParser, Localization.Resources.Log_TraceParser1001);
			foreach (BitAndRecord bitAndRecord in errorBitList)
			{
				int bit = bitAndRecord.Bit;
				string record = bitAndRecord.Record;
				MessageTypeEnum errorType = bitAndRecord.ErrorType;
				if (errorType == MessageTypeEnum.Error)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.Error, //bitAndRecord.ErrorType.ToString() + " : " +
							 ProductValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : "  +
							 ProductValidate.ConvertDataErrorCode2WarningMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}
	 
	
	}
}
