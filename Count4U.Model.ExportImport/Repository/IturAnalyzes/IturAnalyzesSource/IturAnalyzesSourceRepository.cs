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
using Count4U.Model.Count4U.Validate;
using Count4U.Model.SelectionParams;
using NLog;
using System.Diagnostics;
using System.Threading;
using Count4U.GenerationReport.Settings;

namespace Count4U.Model.Count4U
{
	public class IturAnalyzesSourceRepository : BaseADORepository, IIturAnalyzesSourceRepository
	{
		//private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private ISettingsRepository _settingsManager;
		private IturAnalyzesRepositoryEnum _currentIturAnalyzesRepository;

		public IturAnalyzesSourceRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			//IMakatRepository makatRepository,
			ISettingsRepository settingsManager)
			: base(connection, dbSettings, log, serviceLocator)
        {
			this._settingsManager = settingsManager;
			//this._currentIturAnalyzesRepository = this._settingsManager.ReportRepositoryGet;
	    }
		
		// ----------------------------- InsertIturAnalyzes

		public void InsertIturAnalyzes(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null)
		{
			this._currentIturAnalyzesRepository = this._settingsManager.ReportRepositoryGet;
			
			if (this._currentIturAnalyzesRepository == IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository)  //запись в БД через ADO 
			{
				IIturAnalyzesCaseSourceRepository importIturAnalyzesReaderADORepository =
					this._serviceLocator.GetInstance<IIturAnalyzesCaseSourceRepository>
					(IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository.ToString());

				importIturAnalyzesReaderADORepository.InsertIturAnalyzes(
					pathDB, refill, refillCatalogDictionary, selectParms, parmsIn);
			}

			else if (this._currentIturAnalyzesRepository == IturAnalyzesRepositoryEnum.IturAnalyzesBulkRepository) //запись в БД через Bulk
			{
				IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesReader;
				IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
					this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

				importIturAnalyzesBlukRepository.InsertIturAnalyzes(
					pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn);
			}

			else
			{
				IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesReader;
				IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
					this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

				importIturAnalyzesBlukRepository.InsertIturAnalyzes(
					pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn);
			}

		}

		// ----------------------------- InsertIturAnalyzesSimple

		public void InsertIturAnalyzesSimple(string pathDB, bool refill = true, bool refillCatalogDictionary = false,SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null)
		{
			this._currentIturAnalyzesRepository = this._settingsManager.ReportRepositoryGet;

			if (this._currentIturAnalyzesRepository == IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository) //запись в БД через ADO 
			{
				IIturAnalyzesCaseSourceRepository importIturAnalyzesReaderADORepository =
					this._serviceLocator.GetInstance<IIturAnalyzesCaseSourceRepository>
					(IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository.ToString());

				importIturAnalyzesReaderADORepository.InsertIturAnalyzesSimple(
					pathDB, refill, refillCatalogDictionary, selectParms, parmsIn);
			}

			else if (this._currentIturAnalyzesRepository == IturAnalyzesRepositoryEnum.IturAnalyzesBulkRepository)  //запись в БД через Bulk 
			{
				IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesSimpleReader;
				IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
					this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

				importIturAnalyzesBlukRepository.InsertIturAnalyzes(
					pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn);
			}

			else
			{
				IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesSimpleReader;
				IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
					this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

				importIturAnalyzesBlukRepository.InsertIturAnalyzes(
					pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn);

			}
		}


		// ----------------------------- InsertIturAnalyzesSumSimple

		public void InsertIturAnalyzesSumSimple(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, bool addResult = true, List<ImportDomainEnum> importType = null)
		{
			this._currentIturAnalyzesRepository = this._settingsManager.ReportRepositoryGet;

			if (this._currentIturAnalyzesRepository == IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository) //запись в БД через ADO 
			{
				IIturAnalyzesCaseSourceRepository importIturAnalyzesReaderADORepository =
					this._serviceLocator.GetInstance<IIturAnalyzesCaseSourceRepository>
					(IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository.ToString());

				importIturAnalyzesReaderADORepository.InsertIturAnalyzesSumSimple(
					pathDB, refill, refillCatalogDictionary, selectParms, parmsIn, addResult, importType);
			}

			else if (this._currentIturAnalyzesRepository == IturAnalyzesRepositoryEnum.IturAnalyzesBulkRepository)  //запись в БД через Bulk 
			{
				IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesSimpleSumReader;
				IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
						this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

				importIturAnalyzesBlukRepository.InsertIturAnalyzes(
					pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn, addResult, importType);
			}
			else
			{
				IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesSimpleSumReader;
				IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
						this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

				importIturAnalyzesBlukRepository.InsertIturAnalyzes(
					pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn, addResult, importType);

			}
		}

		//------------------------------InsertIturAnalyzesFamily -- ?? не используется пока

		public void InsertIturAnalyzesFamily(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null, 
			Dictionary<object, object> parmsIn = null, bool addResult = true)
		{
			this._currentIturAnalyzesRepository = this._settingsManager.ReportRepositoryGet;

			if (this._currentIturAnalyzesRepository == IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository) //запись в БД через ADO 
			{
				IIturAnalyzesCaseSourceRepository importIturAnalyzesReaderADORepository = this._serviceLocator.GetInstance<IIturAnalyzesCaseSourceRepository>
					(IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository.ToString());

				importIturAnalyzesReaderADORepository.InsertIturAnalyzesFamily(
					pathDB, refill, refillCatalogDictionary, selectParms, parmsIn);
			}

			else if (this._currentIturAnalyzesRepository == IturAnalyzesRepositoryEnum.IturAnalyzesBulkRepository)  //запись в БД через Bulk 
			{
				IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesFamilyReader;
				IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository = 	this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(
					IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

				importIturAnalyzesBlukRepository.InsertIturAnalyzes(
					pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn, addResult);
			}
			else
			{
				IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesFamilyReader;
				IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository = this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(
						IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

				importIturAnalyzesBlukRepository.InsertIturAnalyzes(
					pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn, addResult);
			}
		}

		
		private void CountLong(long count)
		{
		}


	}
}
