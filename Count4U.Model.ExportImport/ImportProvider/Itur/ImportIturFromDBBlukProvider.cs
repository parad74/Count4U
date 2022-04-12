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
	//ипользуется только для добавления отсутствующего в Itur
	public class ImportIturFromDBBlukProvider : BaseProvider, IImportProvider
	{
		//private readonly IImportIturRepository _importIturADORepository;
		private readonly IImportIturBlukRepository _importIturBlukRepository;

		public ImportIturFromDBBlukProvider(IImportIturRepository importIturADORepository,
		    IImportIturBlukRepository importIturBlukRepository,
			IServiceLocator serviceLocator,
			ILog log)
			: base(log, serviceLocator)
		{
			//if (importIturADORepository == null) throw new ArgumentNullException("importIturADORepository");
			//this._importIturADORepository = importIturADORepository;

			if (importIturBlukRepository == null) throw new ArgumentNullException("ImportIturBlukRepository");
			this._importIturBlukRepository = importIturBlukRepository;

			this._importTypes.Add(ImportDomainEnum.ImportItur);
			this._importTypes.Add(ImportDomainEnum.ExistItur);
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importIturBlukRepository.InsertIturs(this.FromPathFile, this.ToPathDB,
			IturParserEnum.IturFromDBParser1,
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
	//		this._importIturBlukRepository.ClearIturs(this.ToPathDB);
		}

	}
}		 
	