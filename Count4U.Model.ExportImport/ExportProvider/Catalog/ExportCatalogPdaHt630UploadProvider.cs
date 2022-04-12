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
	public class ExportCatalogPdaHt630UploadProvider : BaseProvider, IExportProvider
	{
		//? не используется
		protected readonly IUploadCatalogFileRepository _uploadRepository;

		public ExportCatalogPdaHt630UploadProvider(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.UploadCatalog);
			this._uploadRepository = this._serviceLocator.GetInstance
				<IUploadCatalogFileRepository>(ExportRepositoryEnum.UploadCatalogFileRepository.ToString());

		}

		public void Export()
		{
			//?? не используется
			base.FillInfoLog(this.FromPathDB, this.GetType().Name, this._importTypes);
			this._uploadRepository.WriteToFile(this.FromPathDB, this.ToPathFile, 
			ProductSimpleParserEnum.ProductFromDBParser, 
			//ExportProviderEnum.ExportCatalogToFileProvider,
			WriterEnum.ExportCatalogPdaHt630FileWriter,
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
			this._uploadRepository.DeleteFile(this.FromPathFile);
		}
	}
}
