using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public class ImportPreviousInventoryMerkavaSqliteXslxProvider : BaseProvider, IImportProvider
	{
		private readonly IImportPreviousInventorysSQLiteADORepository _importPreviousInventoryADORepository;

		public ImportPreviousInventoryMerkavaSqliteXslxProvider(
				IImportPreviousInventorysSQLiteADORepository importPreviousInventoryRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			//if (importLocationRepository == null) throw new ArgumentNullException("importLocationRepository");
			//IConnectionADO connection = serviceLocator.GetInstance<IConnectionADO>();
			//IImportLocationSQLiteADORepository rep = serviceLocator.GetInstance<IImportLocationSQLiteADORepository>();
			//this._importLocationADORepository = new ImportLocationSQLiteADORepository(connection, serviceLocator, log);

			//this._importLocationADORepository = serviceLocator.GetInstance<IImportLocationSQLiteADORepository>();
			this._importPreviousInventoryADORepository = importPreviousInventoryRepository;
			this._countExcludeFirstString = 3;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importPreviousInventoryADORepository.InsertPreviousInventorys(this.FromPathFile, this.ToPathDB,
			PreviousInventorySQLiteParserEnum.PreviousInventoryMerkavaXslx2SqliteParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 3;
		}

		public void Clear()
		{
			this._importPreviousInventoryADORepository.ClearPreviousInventory(this.ToPathDB);
			this._importPreviousInventoryADORepository.VacuumPreviousInventory(this.ToPathDB);
			
		}



	}
}

