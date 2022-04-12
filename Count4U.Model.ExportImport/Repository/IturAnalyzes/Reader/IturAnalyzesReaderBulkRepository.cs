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
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Count4U
{
	public class IturAnalyzesReaderBulkRepository : BaseCopyBulkRepository, IImportIturAnalyzesBlukRepository
	{
		private IIturAnalyzesReader _iturAnalyzesReader;

		public IturAnalyzesReaderBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log
			)
			: base(connection, dbSettings, log, serviceLocator)
		{
		}

		public void InsertIturAnalyzes(string pathDB, IturAnalyzesReaderEnum iturAnalyzesReaderEnum, 
			bool refill = true, 
			bool refillCatalogDictionary = false,
			SelectParams selectParms = null, 
			Dictionary<object, object> parmsIn = null, 
			bool addResult = true, 
			//Dictionary<ImportProviderParmEnum, object> parms = null, 
			List<ImportDomainEnum> importType = null, 
			List<string[]> сolumnMappings = null)
		{

			Action<long> countAction = null;
			CancellationToken cancellationToken = CancellationToken.None;
			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
			if (parmsIn != null)
			{
				parms = parmsIn.ConvertToImportProviderParmEnum();
				cancellationToken = parms.GetCancellationTokenFromParm();
			}
			if (cancellationToken == CancellationToken.None)	//throw new ArgumentNullException("CancellationToken.None");
			{
				var c = new CancellationTokenSource();
				cancellationToken = c.Token;
			}
			countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}
	
	
			this._iturAnalyzesReader =
						this._serviceLocator.GetInstance<IIturAnalyzesReader>(iturAnalyzesReaderEnum.ToString());
			if (this._iturAnalyzesReader == null)
			{
				//TODO Localization
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, this._iturAnalyzesReader.ToString()));
				return;
			}

			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, " IturAnalyzesBulk Repository", "[IturAnalyzesBulkRepository]"));
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "IturAnalyzesReader", _iturAnalyzesReader.ToString()));

			int i = 0;

			string sessionCodeIn = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			//========
			#region IturAnalyzes
			string tableName = @"IturAnalyzes";
			//if (cancellationToken.IsCancellationRequested == true) return;
			//countAction(0);

			//if (importType.Contains(ImportDomainEnum.ImportIturAnalyzes) == true)
			//{

			IEnumerable<IturAnalyzes> dataIturAnalyzes = this._iturAnalyzesReader.GetIturAnalyzes(pathDB,
					refill, refillCatalogDictionary, selectParms, parmsIn, addResult, importType);

			//CopyBulkIEnumerable<T>(SqlCeBulkCopy bulkCopy, IEnumerable<T> data)
			IEnumerable<IturAnalyzes> test1 = dataIturAnalyzes.ToList();
			long count1 = base.CountRow(tableName, pathDB);

			//DoBulkCopyMapping<IturAnalyzes>(tableName, dataIturAnalyzes, true, pathDB, сolumnMappings);
			DoBulkCopyMapping<IturAnalyzes>(tableName, test1, true, pathDB, сolumnMappings);
				//string queryString = "SELECT  Product.*  FROM  Product";
				//this.CopyTable(tableName, fromPathDB, toPathDB);
			//}
			//var test = dataIturAnalyzes.ToList();
			long count = base.CountRow(tableName, pathDB);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1062,
				tableName, pathDB, Environment.NewLine, count));

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

				#endregion IturAnalyzes
		}

		private void CountLong(long count)
		{
		}



	}
}




