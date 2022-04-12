using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public class ExportCatalogToFileProvider : BaseProvider, IExportProvider
	{ //!! не понятно - не используется
		protected readonly IExportCatalogSimpleRepository _exportRepository;

		public ExportCatalogToFileProvider(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.ExportCatalog);
			this._exportRepository =	this._serviceLocator.GetInstance
				<IExportCatalogSimpleRepository>(ExportRepositoryEnum.ExportCatalogSimpleFileRepository.ToString());

		}

		public void Export()
		{
			//!! не понятно - не используется
			base.FillInfoLog(this.FromPathDB, this.GetType().Name, this._importTypes);
			this._exportRepository.WriteToFile(this.FromPathDB, this.ToPathFile, 
			ProductSimpleParserEnum.ProductFromDBParser, 
			//ExportProviderEnum.ExportCatalogToFileProvider,
			WriterEnum.ExportCatalogFileWriter,
			this.ProviderEncoding, this._separators, 
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
			this._exportRepository.DeleteFile(this.FromPathFile);
		}
	}
}
