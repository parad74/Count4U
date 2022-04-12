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
	//?? не используется
	public class ImportIturMerkavaSqliteXslxProvider : BaseProvider, IImportProvider
	{
		private readonly IImportIturRepository _importIturADORepository;

		public ImportIturMerkavaSqliteXslxProvider(IImportIturRepository importIturADORepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importIturADORepository == null) throw new ArgumentNullException("ImportIturADOProvider");
			this._importIturADORepository = importIturADORepository;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importIturADORepository.InsertIturs(this.FromPathFile, this.ToPathDB,
			IturParserEnum.IturMerkavaXslx2SdfParser,
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
			//this._importIturADORepository.ClearIturs(this.ToPathDB);
		}

	}
}		 
	