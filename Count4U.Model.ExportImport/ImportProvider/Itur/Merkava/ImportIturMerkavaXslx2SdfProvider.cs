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
	public class ImportIturMerkavaXslx2SdfProvider : BaseProvider, IImportProvider
	{
		//private readonly IImportIturRepository _importIturADORepository;
		private readonly IImportIturBlukRepository _importIturBlukRepository;

		public ImportIturMerkavaXslx2SdfProvider(//IImportIturRepository importIturADORepository,
				IImportIturBlukRepository importIturBlukRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			//if (importIturADORepository == null) throw new ArgumentNullException("ImportIturADOProvider");
			//this._importIturADORepository = importIturADORepository;
			if (importIturBlukRepository == null) throw new ArgumentNullException("ImportIturBlukRepository");
			this._importIturBlukRepository = importIturBlukRepository;

			
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importTypes.Add(ImportDomainEnum.ImportItur);
			this._importTypes.Add(ImportDomainEnum.ExistItur);
			this._importIturBlukRepository.InsertIturs(this.FromPathFile, 
				this.ToPathDB, IturParserEnum.IturMerkavaXslx2SdfParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);

			this._importTypes.Remove(ImportDomainEnum.ExistItur);
			//update level для андроида
			this._importIturBlukRepository.InsertIturs(this.FromPathFile,
				this.ToPathDB, IturParserEnum.IturMerkavaXslx2SdfUpdateUpLevelParser,	//IturDBParser,		  
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
			this._importIturBlukRepository.ClearIturs(this.ToPathDB);
			this._importIturBlukRepository.ClearLocations(this.ToPathDB);
		}

	}
}		 
	