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
using NLog;
using System.IO;

namespace Count4U.Model.Count4U
{
	public class ImportDocumentHeaderBulkRepository : BaseCopyBulkRepository, IImportDocumentHeaderBlukRepository
	{
		private readonly IDocumentHeaderRepository _documentHeaderRepository;

		private IDocumentHeaderParser _documentHeaderParser;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportDocumentHeaderBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IDocumentHeaderRepository documentHeaderRepository)
			: base(connection, dbSettings, log, serviceLocator)
		{
			if (documentHeaderRepository == null) throw new ArgumentNullException("ImportDocumentHeaderBulkRepository");

			this._documentHeaderRepository = documentHeaderRepository;
		}

		private Dictionary<string, DocumentHeader> GetDocumentHeaderDictionary(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "IturDictionary"));
				Dictionary<string, DocumentHeader> documentHeaderFromDBDictionary =
					this._documentHeaderRepository.GetDocumentHeaderDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "IturDictionary"));
				return documentHeaderFromDBDictionary;
			}
			else
			{
				return this._documentHeaderRepository.GetDocumentHeaderDictionary(pathDB, false);
			}
		}

		public void InsertDocumentHeaders(string fromPathFile, string pathDB,
			DocumentHeaderParseEnum documentHeaderParseEnumParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null,
			List<string[]> columnMappings = null)
		{

			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1059%"Cancel Insert [{0}] in DB [{1}] from file [{2}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1059, "[Itur]", pathDB, fromPathFile));
				return;
			}
			this._documentHeaderParser =
						this._serviceLocator.GetInstance<IDocumentHeaderParser>(documentHeaderParseEnumParserEnum.ToString());
			if (this._documentHeaderParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, documentHeaderParseEnumParserEnum.ToString()));
				return;
			}

			//if (File.Exists(fromPathFile) == false)
			//{
				//if (fromPathFile != pathDB)
				//{
					//this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
					//return;
				//}
			//}
			//this._inventProductAdvancedParser   =  
			//    this._serviceLocator.GetInstance<IInventProductSimpleParser>(inventProductAdvancedParser.ToString());

			Dictionary<string, DocumentHeader> documentHeaderFromDBDictionary = new Dictionary<string, DocumentHeader>();
			if (importType.Contains(ImportDomainEnum.ExistDocumentHeader) == true)
			{
				documentHeaderFromDBDictionary = this.GetDocumentHeaderDictionary(pathDB, true);
			}
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, " ImportDocumentHeader Repository", "[ImportDocumentHeaderBulkRepository]"));
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "DocumentHeaderParser", documentHeaderParseEnumParserEnum.ToString()));

			int i = 0;

			string sessionCodeIn = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			//========
			#region DocumentHeader
			string tableName = @"DocumentHeader";
			if (cancellationToken.IsCancellationRequested == true) return;
			countAction(0);
			//Localization.Resources.Log_TraceRepositoryResult1061%"Source Table [{0}] in DB [{1}] {2} have [{3}] Row"
			//this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1061,
			//    tableName, fromPathDB, Environment.NewLine, count));

			if (importType.Contains(ImportDomainEnum.ImportDocumentHeader) == true)
			{
				Dictionary<string, DocumentHeader> documentHeadersDictionary = this._documentHeaderParser.GetDocumentHeaders(fromPathFile, encoding, separators,
				countExcludeFirstString, documentHeaderFromDBDictionary, parms);

				DocumentHeaders documentHeaders = new DocumentHeaders();
				int k = 0;
				foreach (var item in documentHeadersDictionary)
				{
					k++;
					documentHeaders.Add(item.Value);
					if (k % 100 == 0) countAction(k);
				}

				DoBulkCopyMapping<DocumentHeader>(tableName, documentHeaders, true, pathDB, columnMappings);
			}
			long count = base.CountRow(tableName, pathDB);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1062,
				tableName, pathDB, Environment.NewLine, count));

			this._documentHeaderParser = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			#endregion Itur
		}

		public void ClearDocumentHeaders(string pathDB)
		{
			DeleteTable(@"DocumentHeader", pathDB);
		}

		public void InsertDocumentHeaders(DocumentHeaders documentHeaders, string pathDB)
		{
			string tableName = @"DocumentHeader";

			DoBulkCopyMapping<DocumentHeader>(tableName, documentHeaders, true, pathDB, null);
			long count = base.CountRow(tableName, pathDB);

		}

	}
}




