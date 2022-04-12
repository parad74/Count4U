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
	public class ImportIturBulkRepository : BaseCopyBulkRepository, IImportIturBlukRepository
	{
		private readonly IIturRepository _iturRepository;
		public readonly List<IIturParser> _iturParserList;
		private IIturParser _iturParser;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportIturBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IIturRepository iturRepository)
			: base(connection,dbSettings, log, serviceLocator)
		{
			if (iturRepository == null) throw new ArgumentNullException("iturRepository");

			this._iturRepository = iturRepository;
		}

		private Dictionary<string, Itur> GetIturDictionary(string pathDB, bool refill = false)
		{
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "IturDictionary"));
				Dictionary<string, Itur> iturFromDBDictionary =
					this._iturRepository.GetIturDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "IturDictionary"));
				return iturFromDBDictionary;
			}
			else
			{
				return this._iturRepository.GetIturDictionary(pathDB, false);
			}
		}

		public void InsertIturs(string fromPathFile, string pathDB, 
			IturParserEnum iturParserEnum,
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
			this._iturParser =
						this._serviceLocator.GetInstance<IIturParser>(iturParserEnum.ToString());
			if (this._iturParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, iturParserEnum.ToString()));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}
			//this._inventProductAdvancedParser   =  
			//    this._serviceLocator.GetInstance<IInventProductSimpleParser>(inventProductAdvancedParser.ToString());

			Dictionary<string, Itur> iturFromDBDictionary = new Dictionary<string, Itur>();
			if (importType.Contains(ImportDomainEnum.ExistItur) == true)
			{
				iturFromDBDictionary = this.GetIturDictionary(pathDB, true);
			}
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, " ImportItur Repository", "[ImportIturBulkRepository]"));
			//Localization.Resources.Log_TraceRepository1040%"{0} is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "IturParser", iturParserEnum.ToString()));

			int i = 0;

			string sessionCodeIn = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			//========
			#region Itur
			string tableName = @"Itur";
			if (cancellationToken.IsCancellationRequested == true) return;
			countAction(0);
			//Localization.Resources.Log_TraceRepositoryResult1061%"Source Table [{0}] in DB [{1}] {2} have [{3}] Row"
			//this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1061,
			//    tableName, fromPathDB, Environment.NewLine, count));

			if (importType.Contains(ImportDomainEnum.ImportItur) == true)
			{
				IEnumerable<Itur> iturs = this._iturParser.GetItursEnumerable(fromPathFile, encoding, separators,
				countExcludeFirstString, iturFromDBDictionary, parms);

				DoBulkCopyMapping<Itur>(tableName, iturs, true, pathDB, columnMappings);
			}
			long count = base.CountRow(tableName, pathDB);
			countAction(count);
			//Localization.Resources.Log_TraceRepositoryResult1062%"Destination Table [{0}] in DB [{1}] {2} after Copy, have [{3}] Row"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1062,
				tableName, pathDB, Environment.NewLine, count));

			this._iturParser = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			#endregion Itur
		}

		public void InsertItursFromList(string pathDB,
		  List<Itur> iturs,
		List<string[]> columnMappings = null)
		{
			if (iturs == null)
			{
				return;
			}

			string tableName = @"Itur";

			DoBulkCopyMapping<Itur>(tableName, iturs, true, pathDB, columnMappings);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
		public void ClearLocations(string pathDB)
		{
			DeleteTable(@"Location", pathDB);
		}


		public void ClearIturs(string pathDB)
		{
			DeleteTable(@"Itur", pathDB);
		}

	
	}
}


