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
{	  //не используется перенесено в  ImportIturClalitXslx2SdfProvider
	// большая входящая  таблица, чтобы был один проход
	public class ImportLocationClalitXslx2SdfProvider : BaseProvider, IImportProvider
	{
		private readonly IImportLocationRepository _importLocationADORepository;

		public ImportLocationClalitXslx2SdfProvider(
			    IImportLocationRepository importLocationRepository,
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
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			//Location
			this._importLocationADORepository.InsertLocations(this.FromPathFile, this.ToPathDB,
			LocationParserEnum.LocationClalitXslx2SdfParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 6;
		}

		public void Clear()
		{
			this._importLocationADORepository.ClearLocations(this.ToPathDB);
		}



	}
}

