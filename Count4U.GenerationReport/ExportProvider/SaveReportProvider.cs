using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data;
using Count4U.Model.Interface;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;
using Count4U.GenerationReport;

namespace Count4U.GenerationReport
{
	public class SaveReportProvider : BaseProvider, IReportSaveProvider
	{
		protected readonly IGenerateReportRepository _generateReportRepository;

		public SaveReportProvider(
				IServiceLocator serviceLocator,
			    IGenerateReportRepository generateReportRepository,
				ILog log)
			: base(log, serviceLocator)
		{
			//this._importTypes.Add(ImportDomainEnum.ExportReport);
			this._generateReportRepository = generateReportRepository;
  		}

		public string Save(GenerateReportArgs args, string outputPath, string outputFormat, ReportInfo info = null)
		{
			return this._generateReportRepository.RunSaveReport(args, outputPath, outputFormat, info);
		}

		protected override void InitDefault()
		{
			//_args = new GenerateReportArgs();
		}

		public void Delete()
		{
			//DeleteFiles(this.ToPathFile);
		}
	
	}
}
