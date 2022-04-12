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
	public class ImportLocationNativPlusMISSdf2SqliteProvider : BaseProvider, IImportProvider
	{
		private readonly IImportLocationSQLiteADORepository _importLocationSQLiteADORepository;

		public ImportLocationNativPlusMISSdf2SqliteProvider(
				IImportLocationSQLiteADORepository importLocationRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
	 	this._importLocationSQLiteADORepository = importLocationRepository;

		//this._importTypes.Add(ImportDomainEnum.ExistCode);		 //работаем с одной и той же БД, надо проверить
		this._countExcludeFirstString = 0;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;		//GetDbPath					
			if (this.IsEmptyPath(this.ToPathDB) == true) return;			//db3Path

			////решили не использвать 11/04/2018  Юваль
			//this._importLocationSQLiteADORepository.InsertLocations(
			//	this.FromPathFile, 		  //GetDbPath		Count4U
			//	this.ToPathDB,				 //db3Path		  sqlite
			//LocationSQLiteParserEnum.LocationNativPlusMISSdf2SqliteParserIturCode,
			//this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			//this._importTypes, this.Parms);

			//this._importLocationSQLiteADORepository.InsertLocations(
			//this.FromPathFile, 		  //GetDbPath		Count4U
			//this.ToPathDB,				 //db3Path		  sqlite
			//LocationSQLiteParserEnum.LocationNativPlusMISSdf2SqliteParserERP,
			//this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			//this._importTypes, this.Parms);


			this._importLocationSQLiteADORepository.InsertLocations(
				this.FromPathFile, 		  //GetDbPath		Count4U
				this.ToPathDB,				 //db3Path		  sqlite
			LocationSQLiteParserEnum.LocationNativPlusMISSdf2SqliteParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importLocationSQLiteADORepository.ClearLocations(this.ToPathDB);
			this._importLocationSQLiteADORepository.VacuumLocation(this.ToPathDB);
			
		}



	}
}

