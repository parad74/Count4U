using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using Count4U.Common.Constants;
using Count4U.Model.Interface.Main;

namespace Count4U.GenerationReport
{
	public class ScriptReportRepository : IScriptReportRepository
    {
		private readonly IServiceLocator _serviceLocator;
		public ScriptReportRepository(
			 IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
		}

		public void RunReportScriptFromFile(bool isMain, bool isClear, bool isClearTag, bool toSetupDB, string path, Encoding encoding)
		{
			if (isClearTag == true)
			{
				this.ClearTagReportFromDb();
			}
			else
			{
				this.SaveTagReportToDb();
			}

			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString());
			string sql = "";
			IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
			foreach (String record in fileParser.GetRecords(path, encoding, 0))
			{
				if (record.ToUpper().Contains("DROP") == false
					&& record.ToUpper().Contains("DELETE") == false
					&& record.ToUpper().Contains("UPDATE") == false
					&& record.ToUpper().Contains("ALTER") == false
					&& record.ToUpper().Contains("CREATE") == false
					&& record.ToUpper().Contains("SELECT") == false)
				{
					if (record.ToUpper().Contains("INSERT") == true)
					{
						string reportMainTable = @"[Report]";
						string reportAuditTable = @"[AuditReport]";
						if (isMain == true)
						{
							if (record.Contains(reportMainTable) == true)
							{
								sql = sql + record + Environment.NewLine;
							}
							else if (record.Contains(reportAuditTable) == true)
							{
								string record1 = record.Replace(reportAuditTable, reportMainTable);
								sql = sql + record1 + Environment.NewLine;
							}
						}
						else	  //Audit
						{
							if (record.Contains(reportAuditTable) == true)
							{
								sql = sql + record + Environment.NewLine;
							}
							else if (record.Contains(reportMainTable) == true)
							{
								string record1 = record.Replace(reportMainTable, reportAuditTable);
								sql = sql + record1 + Environment.NewLine;
							}
						}
					}
				}
			}  //foreach record

			if (isMain == true)
			{
				alterADOProvider.ImportMainReport(sql, isClear, toSetupDB);
			}
			else
			{
				alterADOProvider.ImportAuditReport(sql, isClear, toSetupDB);
			}

			if (isClearTag == false)
			{
				this.RefstoreTagReportToDb();
			}
		}

		public void SaveTagReportToDb(/*bool isMain*/)
		{
			IReportRepository reportRepository = this._serviceLocator.GetInstance<IReportRepository>();
			ICustomerReportRepository customerReportRepository = this._serviceLocator.GetInstance<ICustomerReportRepository>();
			List<AllowedReportTemplate> allowedReportTemplates = new List<AllowedReportTemplate>();
			Reports reports = null;

			//if (isMain == true)
			//{
				allowedReportTemplates.Add(AllowedReportTemplate.Main);
				reports = reportRepository.GetAllowedReportTemplate("", "", "",	ViewDomainContextEnum.All, allowedReportTemplates);
			//}
			//else
			//{
			//	allowedReportTemplates.Add(AllowedReportTemplate.Audit);
			//	reports = reportRepository.GetAllowedReportTemplate("", "", "",	ViewDomainContextEnum.All, allowedReportTemplates);
			//}

			CustomerReports customerReports = new CustomerReports();
			foreach (var report in reports)
			{
				if (report == null) continue;
				if (string.IsNullOrWhiteSpace(report.CodeReport) == true) continue;

				if (string.IsNullOrWhiteSpace(report.Tag) == true && string.IsNullOrWhiteSpace(report.MenuCaption)) continue;

				if (string.IsNullOrWhiteSpace(report.Tag) == true) report.Tag = "";
				string reportMenuCaption ="";
				if (string.IsNullOrWhiteSpace(report.MenuCaption) == false) reportMenuCaption = report.MenuCaption;
				if (reportMenuCaption == "-")
				{
					reportMenuCaption = "";
				}

				CustomerReport customerReport = new CustomerReport { ReportCode = report.CodeReport, Name = report.Tag, Description = reportMenuCaption };
				customerReports.Add(customerReport);
			}

			customerReportRepository.SaveTag(customerReports);
		}

		public void RefstoreTagReportToDb()
		{
			IReportRepository reportRepository = this._serviceLocator.GetInstance<IReportRepository>();
			ICustomerReportRepository customerReportRepository = this._serviceLocator.GetInstance<ICustomerReportRepository>();
			List<AllowedReportTemplate> allowedReportTemplates = new List<AllowedReportTemplate>();
			Reports reports = null;

			allowedReportTemplates.Add(AllowedReportTemplate.Main);
			reports = reportRepository.GetAllowedReportTemplate("", "", "", ViewDomainContextEnum.All, allowedReportTemplates);

			Dictionary<string, CustomerReport> customerReportsDic = customerReportRepository.GetCustomerReportDictionary();
			foreach (var report in reports)
			{
				if (report == null) continue;
				if (string.IsNullOrWhiteSpace(report.CodeReport) == true) continue;
				if (customerReportsDic.ContainsKey(report.CodeReport) == false)
				{
					report.Tag = "";						  //Add 01.12.2021
					report.MenuCaption = "";
				}
				else 
				{
					if (string.IsNullOrWhiteSpace(customerReportsDic[report.CodeReport].Name) == false)
					{
						report.Tag = customerReportsDic[report.CodeReport].Name;
					
					}
					if (string.IsNullOrWhiteSpace(customerReportsDic[report.CodeReport].Description) == true)
					{
							report.MenuCaption = "";
						}
						else
						{
							report.MenuCaption = customerReportsDic[report.CodeReport].Description;
					}
				}

				reportRepository.Update(report, AllowedReportTemplate.Main);
			}
		}

		public void ClearTagReportFromDb()
		{
			ICustomerReportRepository customerReportRepository = this._serviceLocator.GetInstance<ICustomerReportRepository>();
			customerReportRepository.DeleteAllNotTag();
		}

		public void SaveReportScriptToFile(bool isMain, string path, Encoding encoding)
		{
			ILog log = this._serviceLocator.GetInstance<ILog>();
			IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
			IReportRepository reportRepository = this._serviceLocator.GetInstance<IReportRepository>();
			List<AllowedReportTemplate> allowedReportTemplates = new List<AllowedReportTemplate>();
			Reports reports = null;
			string sql = @"INSERT INTO [Report] ";
			string retSql = "";
			if (isMain == true)
			{
				allowedReportTemplates.Add(AllowedReportTemplate.Main);
				reports = reportRepository.GetAllowedReportTemplate("", "", "",
				ViewDomainContextEnum.All, allowedReportTemplates);
			}
			else
			{
				allowedReportTemplates.Add(AllowedReportTemplate.Audit);
				reports = reportRepository.GetAllowedReportTemplate("", "", "",
				ViewDomainContextEnum.All, allowedReportTemplates);
				sql = @"INSERT INTO [AuditReport] ";
			}
			if (reports != null)
			{
				foreach (var report in reports)
				{
					if (string.IsNullOrWhiteSpace(report.FileName) == true) continue;
					string code = "Any";
					string reportMenu = report.Menu ? "1" : "0";
					string print = report.Print ? "1" : "0";
					string landscape = report.Landscape ? "1" : "0";
					string iturAdvancedSearchMenu = report.IturAdvancedSearchMenu ? "1" : "0";
					string inventProductAdvancedSearchMenu = report.InventProductAdvancedSearchMenu ? "1" : "0";
					string inventProductSumAdvancedSearchMenu = report.InventProductSumAdvancedSearchMenu ? "1" : "0";
					string customerSearchMenu = report.CustomerSearchMenu ? "1" : "0";
					string branchSearchMenu = report.BranchSearchMenu ? "1" : "0";
					string inventorSearchMenu = report.InventorSearchMenu ? "1" : "0";
					string auditConfigSearchMenu = report.AuditConfigSearchMenu ? "1" : "0";
					string iturSearchMenu = report.IturSearchMenu ? "1" : "0";
					string inventProductSearchMenu = report.InventProductSearchMenu ? "1" : "0";
					string locationSearchMenu = report.LocationSearchMenu ? "1" : "0";
					string productSearchMenu = report.ProductSearchMenu ? "1" : "0";

					string supplierSearchMenu = report.SupplierSearchMenu ? "1" : "0";
					string sectionSearchMenu = report.SectionSearchMenu ? "1" : "0";
					string itursPopupMenu = report.ItursPopupMenu ? "1" : "0";
					string iturPopupMenu = report.IturPopupMenu ? "1" : "0";
					string documentHeaderPopupMenu = report.DocumentHeaderPopupMenu ? "1" : "0";
					string itursListPopupMenu = report.ItursListPopupMenu ? "1" : "0";
		
					string codeReport = string.IsNullOrWhiteSpace(report.CodeReport) ? "" : report.CodeReport.Replace("'", "''");
					string reportDomainContext = string.IsNullOrWhiteSpace(report.DomainContext) ? "NULL" : report.DomainContext.Replace("'", "''");
					string reportTypeDS = string.IsNullOrWhiteSpace(report.TypeDS) ? "NULL" : report.TypeDS.Replace("'", "''");
					string reportTag = string.IsNullOrWhiteSpace(report.Tag) ? "" : report.Tag.Replace("'", "''");
					string reportMenuCaption = string.IsNullOrWhiteSpace(report.MenuCaption) ? "" : report.MenuCaption.Replace("'", "''");
					string reportDescription = string.IsNullOrWhiteSpace(report.Description) ? "" : report.Description.Replace("'", "''");
					string reportPath = string.IsNullOrWhiteSpace(report.Path) ? "" : report.Path.Replace("'", "''");
					string reportDomainType = string.IsNullOrWhiteSpace(report.DomainType) ? "" : report.DomainType.Replace("'", "''");
					string nn = string.IsNullOrWhiteSpace(report.NN.ToString()) ? "" : report.NN.ToString();
					string menuCaptionLocalizationCode = string.IsNullOrWhiteSpace(report.MenuCaptionLocalizationCode) ? "" :
						report.MenuCaptionLocalizationCode.Replace("'", "''");
					try
					{
						string sql1 = sql +
							@"([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[Print],[NN],[MenuCaptionLocalizationCode],[MenuCaption]," +
								@"[IturAdvancedSearchMenu],[InventProductAdvancedSearchMenu],[InventProductSumAdvancedSearchMenu],[CustomerSearchMenu],"+
								@"[BranchSearchMenu],[InventorSearchMenu],[AuditConfigSearchMenu],[IturSearchMenu],"+
								@"[InventProductSearchMenu],[LocationSearchMenu],[ProductSearchMenu],[CodeReport],[Landscape]," +  //24
								@"[SupplierSearchMenu],[SectionSearchMenu],[ItursPopupMenu]," +					   //{25},{26},{27},
								@"[IturPopupMenu],[DocumentHeaderPopupMenu],[ItursListPopupMenu]" +			 //{28},{29},{30},
								@") " +
							@"VALUES " +
							//	  {0}		{1}					{2}						 {3}		 {4}		 {5}			{6}				 {7}		{8}	  {9}
							// ("(N'Any',N'Corporative Report',null,null,N'Iturs',N'ItursCorporativeReport.rdlc',N'Iturs',1,N'Corporative Report - Iturs', , , ,N'CorporativeReport')");
							String.Format("(N'{0}',N'{1}',{2},{3},N'{4}',N'{5}','{6}','{7}',{8},{9},{10}, N'{11}',N'" + reportMenuCaption.Trim() + "'"
							+ ",{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},N'{23}',{24}, {25},{26},{27},{28},{29},{30});" + Environment.NewLine,
							code.Trim(), reportDescription.Trim(), reportDomainContext.Trim(), reportTypeDS.Trim(), reportPath.Trim(), report.FileName.Trim(),
							reportDomainType.Trim(), reportTag.Trim(), reportMenu.Trim(), print.Trim(), nn, menuCaptionLocalizationCode.Trim(),
							iturAdvancedSearchMenu,inventProductAdvancedSearchMenu,inventProductSumAdvancedSearchMenu,customerSearchMenu,
							branchSearchMenu,inventorSearchMenu,auditConfigSearchMenu,iturSearchMenu,
							inventProductSearchMenu, locationSearchMenu, productSearchMenu, codeReport.Trim(), landscape,
							supplierSearchMenu,	sectionSearchMenu, itursPopupMenu, 	
							iturPopupMenu, documentHeaderPopupMenu, 	itursListPopupMenu);
						retSql = retSql + sql1;
					}
					catch (Exception exp)
					{
						//this.Log = this.Log + exp.Message + " : " + report.FileName;
						log.Add(Model.MessageTypeEnum.Error, exp.Message + " : " + report.FileName);
					}
				}
				File.WriteAllText(path, retSql, encoding);
			}
		}

		
	}
}