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
	public class ImportLocationUpdateNativSimpleSqlite2SdfProvider : BaseProvider, IImportProvider
	{
		private readonly IImportLocationRepository _importLocationADORepository;
		//private readonly IImportIturRepository _importIturADORepository;
		private readonly IImportIturBlukRepository _importIturBulkRepository;

		public ImportLocationUpdateNativSimpleSqlite2SdfProvider(
			IImportLocationRepository importLocationRepository,
				//IImportIturRepository importIturRepository,
				IImportIturBlukRepository importIturBulkRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			if (importLocationRepository == null) throw new ArgumentNullException("importLocationRepository");
			this._importLocationADORepository = importLocationRepository;
			this._importIturBulkRepository = importIturBulkRepository;
			this._importTypes.Add(ImportDomainEnum.ImportItur);
			this._importTypes.Add(ImportDomainEnum.ExistItur);
  			this._countExcludeFirstString = 1;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			//Location		   не менятся в Clalit на android
			//this._importLocationADORepository.InsertLocations(
			//	this.FromPathFile, 		 //db3Path		  sqlite	
			//	this.ToPathDB,				//GetDbPath		Count4U				
			//LocationParserEnum.LocationUpdateClalitSqlite2SdfParser,
			//this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			//this._importTypes, this.Parms);

			//Itur
			this._importIturBulkRepository.InsertIturs(
				this.FromPathFile, 	  //db3Path		  sqlite	
				this.ToPathDB,			//GetDbPath		Count4U	
				IturParserEnum.IturUpdateNativSimpleSqlite2SdfParser,
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
			//this._importIturBulkRepository.ClearIturs(this.ToPathDB);
			//this._importIturBulkRepository.ClearLocations(this.ToPathDB);
		}



	}
}

