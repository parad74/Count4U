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
	public class ImportIturClalitXslx2SdfProvider : BaseProvider, IImportProvider
	{
		private readonly IImportIturBlukRepository _importIturBlukRepository;

		public ImportIturClalitXslx2SdfProvider(IImportIturBlukRepository importIturBlukRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importIturBlukRepository == null) throw new ArgumentNullException("ImportIturBlukRepository");
			this._importIturBlukRepository = importIturBlukRepository;
			this._importTypes.Add(ImportDomainEnum.ImportLocation);
			this._importTypes.Add(ImportDomainEnum.ImportItur);
			
		}

		public void Import()
		{
		
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importIturBlukRepository.InsertIturs(this.FromPathFile, 
				this.ToPathDB, IturParserEnum.IturClalitXslx2SdfParser,
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
			this._importIturBlukRepository.ClearIturs(this.ToPathDB);
			this._importIturBlukRepository.ClearLocations(this.ToPathDB);
		}

	}
}		 
	