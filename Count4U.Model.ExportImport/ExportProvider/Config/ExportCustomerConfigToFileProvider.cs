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
	public class ExportCustomerConfigToFileProvider : BaseProvider, IExportProvider
	{
		protected readonly IExportConfigIniRepository _exportRepository;

		public ExportCustomerConfigToFileProvider(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			this._importTypes.Add(ImportDomainEnum.ExportCustomerConfig);
			this._exportRepository =
				this._serviceLocator.GetInstance<IExportConfigIniRepository>(ExportRepositoryEnum.ExportCustomerConfigFileRepository.ToString());

		}


		public void Export()
		{
			base.FillInfoLog(this.GetType().Name, this._importTypes);
			this._exportRepository.WriteToFile(this.FromPathDB, this.ToPathFile, 
			WriterEnum.ExportCustomerConfigFileWriter,
			//ExportProviderEnum.ExportCustomerConfigToFileProvider,
			this.ProviderEncoding, this._separators, 
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Equal };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._exportRepository.DeleteFile(this.ToPathFile);
		}
	}
}
