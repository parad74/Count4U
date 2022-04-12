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
	public class ExportIturToFileProvider : BaseProvider, IExportProvider
	{
		protected readonly IExportIturRepository _exportRepository;

		public ExportIturToFileProvider(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.ExportItur);
			this._exportRepository =
				this._serviceLocator.GetInstance<IExportIturRepository>(ExportRepositoryEnum.ExportIturFileRepository.ToString());
  		}


		public void Export()
		{
			base.FillInfoLog(this.FromPathDB, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathDB) == true) return; 
			this._exportRepository.WriteToFile(this.FromPathDB, this.ToPathFile, 
			IturParserEnum.IturDBParser,
			WriterEnum.ExportIturFileWriter,
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
			this._exportRepository.DeleteFile(this.ToPathFile);
		}
	}
}
