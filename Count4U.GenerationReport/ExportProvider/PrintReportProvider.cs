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

namespace Count4U.GenerationReport
{
	public class PrintReportProvider : BaseProvider, IReportPrintProvider
	{
		protected readonly IGenerateReportRepository _generateReportRepository;

		public PrintReportProvider(
				IServiceLocator serviceLocator,
			    IGenerateReportRepository generateReportRepository,
				ILog log)
			: base(log, serviceLocator)
		{
			//this._importTypes.Add(ImportDomainEnum.PrintReport);
			this._generateReportRepository = generateReportRepository;
  		}

		public void Print(GenerateReportArgs args)
		{
			this._generateReportRepository.RunPrintReport(args);
		}

		protected override void InitDefault()
		{
			//_args = new GenerateReportArgs();
		}


	}
}
