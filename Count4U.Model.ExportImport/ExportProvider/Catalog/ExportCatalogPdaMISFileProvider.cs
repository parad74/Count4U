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
	public class ExportCatalogPdaMISFileProvider : BaseProvider, IExportProvider
	{
		protected readonly IExportCatalogSimpleRepository _exportRepository;

		public ExportCatalogPdaMISFileProvider(
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
			base.FillInfoLog(this.FromPathDB, this.GetType().Name, this._importTypes);
			this._exportRepository.WriteToFile(this.FromPathDB, this.ToPathFile, 
			ProductSimpleParserEnum.ProductFromDBParser, 
			//ExportProviderEnum.ExportCatalogToFileProvider,
			WriterEnum.ExportCatalogPdaHt630FileWriter,
			this.ProviderEncoding, this._separators, 
			this._importTypes, false, this.Parms);
		}


		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");	   //Сегодня!
			//this.ProviderEncoding = Encoding.Unicode;
			//UnicodeEncoding unicode = new UnicodeEncoding();
			//String encodingName = unicode.EncodingName;
			this._separators = new string[] { SeparatorField.I };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._exportRepository.DeleteFile(this.FromPathFile);
		}
	}
}
