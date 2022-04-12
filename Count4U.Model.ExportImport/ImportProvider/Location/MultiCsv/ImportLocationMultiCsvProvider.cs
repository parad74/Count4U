using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public class ImportLocationMultiCsvProvider : BaseProvider, IImportProvider
	{
		private readonly IImportLocationRepository _importLocationADORepository;

		public ImportLocationMultiCsvProvider(IImportLocationRepository importLocationRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			if (importLocationRepository == null) throw new ArgumentNullException("importLocationRepository");
			this._importLocationADORepository = importLocationRepository;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;	// not use eyt
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importLocationADORepository.InsertLocations(this.FromPathFile, this.ToPathDB,
			LocationParserEnum.LocationMultyCsvParser,
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
			//this._importLocationADORepository.ClearLocations(this.ToPathDB);
		}



	}
}

