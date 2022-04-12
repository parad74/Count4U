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

namespace Count4U.Model.Count4U
{
	public class IturAnalyzesSourceReaderBulkRepository : BaseADORepository, IIturAnalyzesCaseSourceRepository
	{
		private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public IturAnalyzesSourceReaderBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository)
			: base(connection, dbSettings, log, serviceLocator)
        {
	
	    }

		//=============== InsertIturAnalyzes =====================================
		public void InsertIturAnalyzes(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null)
		{
			IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesReader;
	
			IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
				this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

			importIturAnalyzesBlukRepository.InsertIturAnalyzes(
				pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn);
		}


		//==================== InsertIturAnalyzesSimple ===========================================
		public void InsertIturAnalyzesSimple(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null)
		{

			IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesSimpleReader;

			IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
				this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

			importIturAnalyzesBlukRepository.InsertIturAnalyzes(
				pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn);
		}

		// ==================== InsertIturAnalyzesSumSimple =============================================
		public void InsertIturAnalyzesSumSimple(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, bool addResult = true, List<ImportDomainEnum> importType = null)
		{
			IturAnalyzesReaderEnum iturAnalyzesReaderEnum = IturAnalyzesReaderEnum.IturAnalyzesSimpleSumReader;
			//IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository =
			//		this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>();

			IImportIturAnalyzesBlukRepository importIturAnalyzesBlukRepository = 
				this._serviceLocator.GetInstance<IImportIturAnalyzesBlukRepository>(IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString());

			importIturAnalyzesBlukRepository.InsertIturAnalyzes(
				pathDB, iturAnalyzesReaderEnum, refill, refillCatalogDictionary,	selectParms, parmsIn, addResult	);
		}



		private void CountLong(long count)
		{
		}
		


		public void InsertIturAnalyzesFamily(string pathDB, bool refill = true, bool refillCatalogDictionary = false, SelectParams selectParms = null, Dictionary<object, object> parmsIn = null, bool addResult = true)
		{
			throw new NotImplementedException();
		}
	}
}
