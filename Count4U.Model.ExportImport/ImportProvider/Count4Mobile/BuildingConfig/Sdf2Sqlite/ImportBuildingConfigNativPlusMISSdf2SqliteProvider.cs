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
	public class ImportBuildingConfigNativPlusMISSdf2SqliteProvider : BaseProvider, IImportProvider
	{
		private readonly IImportBuildingConfigSQLiteADORepository _importBuildingConfigSQLiteADORepository;

		public ImportBuildingConfigNativPlusMISSdf2SqliteProvider(
				IImportBuildingConfigSQLiteADORepository importBuildingConfigSQLiteRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			//if (importLocationRepository == null) throw new ArgumentNullException("importLocationRepository");
			//IConnectionADO connection = serviceLocator.GetInstance<IConnectionADO>();
			//IImportLocationSQLiteADORepository rep = serviceLocator.GetInstance<IImportLocationSQLiteADORepository>();
			//this._importLocationADORepository = new ImportLocationSQLiteADORepository(connection, serviceLocator, log);

			//this._importLocationADORepository = serviceLocator.GetInstance<IImportLocationSQLiteADORepository>();
			this._importBuildingConfigSQLiteADORepository = importBuildingConfigSQLiteRepository;
			//this._importTypes.Add(ImportDomainEnum.ExistCode);
			this._countExcludeFirstString = 1;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;	  //GetDbPath	
			if (this.IsEmptyPath(this.ToPathDB) == true) return;		  //db3Path
			this._importBuildingConfigSQLiteADORepository.InsertBuildingConfig(this.FromPathFile, this.ToPathDB,
			BuildingConfigParserEnum.BuildingConfigNativPlusMISSdf2SqliteParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 1;
		}

		public void Clear()
		{
			this._importBuildingConfigSQLiteADORepository.ClearBuildingConfig(this.ToPathDB);
		}



	}
}

