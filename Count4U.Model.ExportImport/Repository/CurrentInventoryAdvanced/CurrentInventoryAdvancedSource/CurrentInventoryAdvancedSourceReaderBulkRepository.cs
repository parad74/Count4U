using System.Collections.Generic;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.SelectionParams;
using NLog;

namespace Count4U.Model.Count4U
{
	public class CurrentInventoryAdvancedSourceReaderBulkRepository : BaseADORepository, ICurrentInventoryAdvancedSourceRepository
	{
		private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public CurrentInventoryAdvancedSourceReaderBulkRepository(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log)
			: base(connection, dbSettings, log, serviceLocator)
        {
	
	    }


		public void InsertCurrentInventoryAdvanced(string pathDB,
			bool refill = true,
				bool refillCatalogDictionary = false, 
			SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null,
			List<ImportDomainEnum> importType = null)
		{
			CurrentInventoryAdvancedReaderEnum currentInventoryAdvancedReaderEnum = 
				CurrentInventoryAdvancedReaderEnum.CurrentInventoryAdvancedReader;

			IImportCurrentInventoryAdvancedBlukRepository importCurrentInventoryAdvancedBlukRepository =
				this._serviceLocator.GetInstance<IImportCurrentInventoryAdvancedBlukRepository>();

			importCurrentInventoryAdvancedBlukRepository.InsertCurrentInventoryAdvanced(
			 pathDB, currentInventoryAdvancedReaderEnum, refill, refillCatalogDictionary, selectParms, parmsIn, importType);
		}

		public void ClearCurrentInventoryAdvanced(string pathDB)
		{
			IImportCurrentInventoryAdvancedBlukRepository importCurrentInventoryAdvancedBlukRepository =
				this._serviceLocator.GetInstance<IImportCurrentInventoryAdvancedBlukRepository>();
			importCurrentInventoryAdvancedBlukRepository.ClearCurrentInventoryAdvanced(pathDB);
			
		}

		public void AlterTableCurrentInventoryAdvanced(string pathDB)
		{
			IImportCurrentInventoryAdvancedBlukRepository importCurrentInventoryAdvancedBlukRepository =
			this._serviceLocator.GetInstance<IImportCurrentInventoryAdvancedBlukRepository>();
			importCurrentInventoryAdvancedBlukRepository.DropCurrentInventoryAdvanced(pathDB);

		}


	}
}
