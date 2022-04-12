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
	public class ImportSectionTafnitMatrixProvider : BaseProvider, IImportProvider
	{
		private readonly IImportSectionRepository _importSectionRepository;

		public ImportSectionTafnitMatrixProvider(IImportSectionRepository importSectionRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importSectionRepository == null) throw new ArgumentNullException("importSectionRepository");
			this._importSectionRepository = importSectionRepository;
			this._importTypes.Add(ImportDomainEnum.ImportSection);
			this._countExcludeFirstString = 0;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importSectionRepository.InsertSections(this.FromPathFile, this.ToPathDB,
				SectionParserEnum.SectionTafnitMatrixParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.DotComma };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importSectionRepository.ClearSections(this.ToPathDB);
		}
	}
}		 
	