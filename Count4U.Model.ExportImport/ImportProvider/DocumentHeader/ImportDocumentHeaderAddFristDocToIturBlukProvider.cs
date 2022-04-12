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
	//ипользуется для добавления первого DocumentHeader в Itur если там нет ни одного документа
	public class ImportDocumentHeaderAddFristDocToIturBlukProvider : BaseProvider, IImportProvider
	{
		private readonly IImportDocumentHeaderBlukRepository _importDocumentHeaderBlukRepository;

		public ImportDocumentHeaderAddFristDocToIturBlukProvider(
			IImportDocumentHeaderBlukRepository importDocumentHeaderBlukRepository,
			IServiceLocator serviceLocator,
			ILog log)
			: base(log, serviceLocator)
		{

			if (importDocumentHeaderBlukRepository == null) throw new ArgumentNullException("IImportDocumentHeaderBlukRepository");
			this._importDocumentHeaderBlukRepository = importDocumentHeaderBlukRepository;
			//this._importDocumentHeaderBlukRepository = serviceLocator.GetInstance<IImportDocumentHeaderBlukRepository>();

			//this._importTypes.Add(ImportDomainEnum.ExistDocumentHeader);
			this._importTypes.Add(ImportDomainEnum.ImportDocumentHeader);
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importDocumentHeaderBlukRepository.InsertDocumentHeaders(this.FromPathFile, this.ToPathDB,
			DocumentHeaderParseEnum.DocumentHeaderAddFristDocToIturParser,
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
			this._importDocumentHeaderBlukRepository.ClearDocumentHeaders(this.ToPathDB);
		}

	}
}		 
	