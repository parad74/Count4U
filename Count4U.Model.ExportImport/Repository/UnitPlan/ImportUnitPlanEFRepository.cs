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
using Count4U.Model.Interface.Main;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Main;
using System.IO;

namespace Count4U.Model.Count4U
{
	public class ImportUnitPlanEFRepository : IImportUnitPlanRepository					  //BaseImportEFRepository, 
	{
		private readonly IUnitPlanRepository _unitPlanRepository;
		private IUnitPlanParser _unitPlanParser;
		private readonly ILog _log;
		public readonly IServiceLocator _serviceLocator;

		public ImportUnitPlanEFRepository(
			//IConnectionDB connection,
			IServiceLocator serviceLocator,
			ILog log,
			IUnitPlanRepository unitPlanRepository)
			//: base(connection, log, serviceLocator)
        {
			if (unitPlanRepository == null) throw new ArgumentNullException("unitPlanRepository");
			this._unitPlanRepository = unitPlanRepository;
			this._log = log;
			this._serviceLocator = serviceLocator;
	    }

		public ILog Log
		{
			get { return this._log; }
		}

		public void InsertUnitPlans(string fromPathFile, string pathDB, UnitPlanParserEnum unitPlanParserEnum,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._unitPlanParser = this._serviceLocator.GetInstance<IUnitPlanParser>(unitPlanParserEnum.ToString());
			if (this._unitPlanParser == null)
			{
				//Localization.Resources.Log_Error1007%"In  IturParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1007, unitPlanParserEnum));
				return;
			}

			if (File.Exists(fromPathFile) == false)
			{
				this.Log.Add(MessageTypeEnum.Warning, String.Format(Localization.Resources.Log_ParserFileErrorMessage_FileIsNotExist, fromPathFile));
				return;
			}

			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");
			Dictionary<string, UnitPlan> unitPlanFromDBDictionary = this._unitPlanRepository.FillUnitPlanDictionary(pathDB);

			this._unitPlanParser.GetUnitPlans(fromPathFile, encoding, separators, countExcludeFirstString,
				 importType, unitPlanFromDBDictionary, parms);
			Dictionary<string, UnitPlan> unitPlanToDBDictionary = this._unitPlanParser.UnitPlanDictionary;

			this._unitPlanRepository.Insert(unitPlanToDBDictionary, pathDB);
			this.FillLogFromErrorBitList(this._unitPlanParser.ErrorBitList);
		}

		public void ClearUnitPlans(string pathDB)
		{
			//Пока не удаляем все
			this._unitPlanRepository.DeleteAll(pathDB);
		}


		//public void FromDictionaryToDB(string pathDB, Dictionary<string, Branch> branchToDBDictionary, 
		//    CancellationToken cancellationToken, Action<long> countAction)
		//{
		//    foreach (KeyValuePair<string, Branch> keyValuePair in branchToDBDictionary)
		//    {
		//        string key = keyValuePair.Key;
		//        Branch val = keyValuePair.Value;
		//    }
		//}

		private void FillLogFromErrorBitList(List<BitAndRecord> errorBitList)
		{
			if (errorBitList == null) return;
			if (errorBitList.Count == 0) return;
			//Log_TraceParser1001% "Parser Error And Message : "
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
						this.Log.Add(MessageTypeEnum.Error, // bitAndRecord.ErrorType.ToString() + " : "+
							BranchValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}

				if (errorType == MessageTypeEnum.WarningParser)
				{
					List<ConvertDataErrorCodeEnum> bitList = BitAndRecord.Bit2ConvertDataErrorCodeEnumList(bit);
					foreach (ConvertDataErrorCodeEnum b in bitList)
					{
						this.Log.Add(MessageTypeEnum.WarningParser, //bitAndRecord.ErrorType.ToString() + " : "+
							BranchValidate.ConvertDataErrorCode2ErrorMessage(b) + " [ " + record + " ] ");
					}
				}
			}
		}

	}
}
