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
using System.Data.SqlClient;
using System.IO;

namespace Count4U.Model.Count4U
{
	public class ImportStatusInventProductBulkRepository : BaseCopyBulkRepository, IImportStatusInventProductBlukRepository
	{
		//private readonly ICatalogConfigRepository _catalogConfigRepository;
		//private ISupplierRepository _supplierRepository;
		//private readonly IMakatRepository _makatRepository;
		//private readonly IIturRepository _iturRepository;
		public readonly List<IInventProductSimpleParser> _statusInventProductParserList;
		private IStatusInventProductSimpleParser _statusInventProductParser;
		//private IStatusInventProductSimpleParser _statusInventProductAdvancedParser;

		public ImportStatusInventProductBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log
			)
			: base(connection,dbSettings, log, serviceLocator)
		{
	
		}

	
		public void InsertStatusInventProducts(string fromPathFile, string pathDB, 
			StatusInventProductSimpleParserEnum inventProductSimpleParserEnum, 
			Encoding encoding, string[] separators, 
			int countExcludeFirstString, List<ImportDomainEnum> importType, 
			Dictionary<ImportProviderParmEnum, object> parms = null, List<string[]> ColumnMappings = null)

		{

			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1059%"Cancel Insert [{0}] in DB [{1}] from file [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1059, "[StatusInventProduct]", pathDB, fromPathFile));
				return;
			}
			this._statusInventProductParser =
						this._serviceLocator.GetInstance<IStatusInventProductSimpleParser>(inventProductSimpleParserEnum.ToString());
			if (this._statusInventProductParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, inventProductSimpleParserEnum.ToString()));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}
	
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, " Import Repository", "[ImportStatusInventProductBulkRepository]"));
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "StatusInventProductParser", inventProductSimpleParserEnum.ToString()));

			int i = 0;

			//========
			#region StatusInventProduct
			string tableName = @"StatusInventProduct";
			if (cancellationToken.IsCancellationRequested == true) return;
			countAction(0);

			IEnumerable<StatusInventProduct> dataInventProduct = this._statusInventProductParser.GetStatusInventProducts(
			   fromPathFile, encoding, separators,
			   countExcludeFirstString,
			   //sessionCodeIn,
			   // productMakatDictionary, //productBarcodeDictionary, 
			   // iturFromDBDictionary,
				importType,
				parms) ;

			DoBulkCopyMapping<StatusInventProduct>(tableName, dataInventProduct, true, pathDB, ColumnMappings);

		
			long count = base.CountRow(tableName, pathDB);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1062,
				tableName, pathDB, Environment.NewLine, count));

			this._statusInventProductParser = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			#endregion InventProduct
		}

	 	public void ClearStatusInventProducts(string pathDB)
		{
			DeleteTable(@"StatusInventProduct", pathDB);
		}




	
	}
}


