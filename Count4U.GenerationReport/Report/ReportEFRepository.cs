using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using Count4U.GenerationReport.MappingEF;
using Count4U.Model;
using Count4U.Model.Count4U;
using System.IO;
using Count4U.Common.Constants;
using Count4U.Model.Interface;
//https://www.office365blog.at/2018/03/26/rdlc-reports-in-visual-studio-2017/
namespace Count4U.GenerationReport
{
	public class ReportEFRepository : BaseEFRepository, IReportRepository
	{
		public Dictionary<ViewDomainContextEnum, string> _domainContextPathDictionary;
		public Dictionary<string, string> _domainContextDataSetDictionary;
		public Dictionary<string, ViewDomainContextEnum> _domainContextDictionary;

		public ReportEFRepository(IConnectionDB connection)
			: base(connection)
		{
		}

		public IConnectionDB Connection
		{
			get { return this._connection; }
			set { this._connection = value; }
		}

		#region BaseEFRepository Members
		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}
		#endregion

		public Reports GetReports(string code, string fileName, string path, string allowedReportTemplate)
		{
			if (allowedReportTemplate == AllowedReportTemplate.Main.ToString())
			{
				using (var db = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entities = AsQueryable(db.Report).
						Where(e => e.FileName.ToLower() == fileName.ToLower()
						&& e.Path.ToLower() == path.ToLower()
						&& e.Code.ToLower() == code.ToLower()).OrderBy(e => e.NN).
						ToList().Select(e => e.ToReportMainDomainObject());
					return Reports.FromEnumerable(entities);
				}
			}
			else if (allowedReportTemplate == AllowedReportTemplate.Audit.ToString())
			{
				using (var db = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var entities = AsQueryable(db.AuditReport).
						Where(e => e.FileName.ToLower() == fileName.ToLower()
						&& e.Path.ToLower() == path.ToLower()
						&& e.Code.ToLower() == code.ToLower()).OrderBy(e => e.NN).
						ToList().Select(e => e.ToReportAuditDomainObject());
					return Reports.FromEnumerable(entities);
				}
			}
			else
			{
				return null;
			}
		}

		//public Reports GetReports(SelectParams selectParams)
		//{
		//    if (selectParams == null)
		//        return GetReports();

		//    long totalCount = 0;
		//    using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString))
		//    {
		//        // Получение сущностей и общего количества из БД.
		//        // Getting entities and total count from database.
		//        var entities = GetEntities(dc, AsQueryable(dc.Report), dc.Report.AsQueryable(), selectParams, out totalCount);

		//        // Преобразование сущностей в объекты предметной области.
		//        // Converting entites to domain objects.
		//        var domainObjects = entities.Select(e => e.ToReportMainDomainObject());

		//        // Возврат результата.
		//        // Returning result.
		//        Reports result = Reports.FromEnumerable(domainObjects);
		//        result.TotalCount = totalCount;
		//        return result;
		//    }
		//}


		public Reports GetAllowedReportTemplate(string customerCode, string branchCode,
			string inventorCode, ViewDomainContextEnum viewDomainContextType,
			List<AllowedReportTemplate> allowedReportTemplates)
		{
			string any = ReportTemplateDomainEnum.Any.ToString();
			string domain = viewDomainContextType.ToString();

			if (allowedReportTemplates.Contains(AllowedReportTemplate.PrintInventProduct) == true)
			{
				if (viewDomainContextType == ViewDomainContextEnum.ItursIturDoc)
				{
					Reports printReports = new Reports();
					using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
					{
						var domainObjects = dc.Report.Where(e => e.Code == any
							&& e.DomainType == domain && e.Print == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
						Reports.Fill(printReports, domainObjects);
					}
					return printReports;
				}
			}

			if (this.IsSearchView(viewDomainContextType) == true)
			{
				return GetReportListForSearchMenu(viewDomainContextType);
			}

			else if (allowedReportTemplates.Contains(AllowedReportTemplate.Menu) == true)
			{
				Reports menuReports = new Reports();

				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.Menu == true//)
						&& e.DomainType == domain).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);

					if (string.IsNullOrWhiteSpace(customerCode) == false)
					{
						domainObjects = dc.AuditReport.Where(e => e.Code == customerCode && e.Menu == true//)
							&& e.DomainType == domain).OrderBy(e => e.NN)
							.ToList().Select(e => e.ToReportAuditDomainObject());
						Reports.Fill(menuReports, domainObjects);
					}
					if (string.IsNullOrWhiteSpace(branchCode) == false)
					{
						domainObjects = dc.AuditReport.Where(e => e.Code == branchCode && e.Menu == true//)
							&& e.DomainType == domain).OrderBy(e => e.NN)
							.ToList().Select(e => e.ToReportAuditDomainObject());
						Reports.Fill(menuReports, domainObjects);
					}
					if (string.IsNullOrWhiteSpace(inventorCode) == false)
					{
						domainObjects = dc.AuditReport.Where(e => e.Code == inventorCode && e.Menu == true//)
							&& e.DomainType == domain).OrderBy(e => e.NN)
								  .ToList().Select(e => e.ToReportAuditDomainObject());
						Reports.Fill(menuReports, domainObjects);
					}
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.Menu == true//)
						&& e.DomainType == domain).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);

					if (string.IsNullOrWhiteSpace(customerCode) == false)
					{
						domainObjects = dc.Report.Where(e => e.Code == customerCode && e.Menu == true//)
							&& e.DomainType == domain).OrderBy(e => e.NN)
							.ToList().Select(e => e.ToReportMainDomainObject());
						Reports.Fill(menuReports, domainObjects);
					}
					if (string.IsNullOrWhiteSpace(branchCode) == false)
					{
						domainObjects = dc.Report.Where(e => e.Code == branchCode && e.Menu == true//)
							&& e.DomainType == domain).OrderBy(e => e.NN)
							.ToList().Select(e => e.ToReportMainDomainObject());
						Reports.Fill(menuReports, domainObjects);
					}
					if (string.IsNullOrWhiteSpace(inventorCode) == false)
					{
						domainObjects = dc.Report.Where(e => e.Code == inventorCode && e.Menu == true//)
							&& e.DomainType == domain).OrderBy(e => e.NN)
								  .ToList().Select(e => e.ToReportMainDomainObject());
						Reports.Fill(menuReports, domainObjects);
					}
				}

				var retMenuReports = menuReports.AsEnumerable().OrderBy(n => n.GetType().GetProperty("NN").GetValue(n, null) as string);
				//menuReports = Reports.SortNN(menuReports);
				return Reports.FromEnumerable(retMenuReports);
			}// if menuReport

			//=================================================================

			var reports = new Reports();
			if (allowedReportTemplates.Contains(AllowedReportTemplate.Main) == true)
			{
				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					//DomainEnum domainType = GetDomainContextDictionary()[domainContextType];		  //TODO: test

					var domainObjects = dc.Report.Where(e => e.Code == any).OrderBy(e => e.NN)
						//&& e.DomainType == domain)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(reports, domainObjects);

					if (string.IsNullOrWhiteSpace(customerCode) == false)
					{
						domainObjects = dc.Report.Where(e => e.Code == customerCode).OrderBy(e => e.NN)
							//&& e.DomainType == domain)
							.ToList().Select(e => e.ToReportMainDomainObject());
						Reports.Fill(reports, domainObjects);
					}
					if (string.IsNullOrWhiteSpace(branchCode) == false)
					{
						domainObjects = dc.Report.Where(e => e.Code == branchCode).OrderBy(e => e.NN)
							//&& e.DomainType == domain)
							.ToList().Select(e => e.ToReportMainDomainObject());
						Reports.Fill(reports, domainObjects);
					}
					if (string.IsNullOrWhiteSpace(inventorCode) == false)
					{
						domainObjects = dc.Report.Where(e => e.Code == inventorCode).OrderBy(e => e.NN)
							//&& e.DomainType == domain)
								  .ToList().Select(e => e.ToReportMainDomainObject());
						Reports.Fill(reports, domainObjects);
					}
				}
			}
			if (allowedReportTemplates.Contains(AllowedReportTemplate.Audit) == true)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					//DomainEnum domainType = GetDomainContextDictionary()[domainContextType];		  //TODO: test

					var domainObjects = dc.AuditReport.Where(e => e.Code == any).OrderBy(e => e.NN)
						//&& e.DomainType == domain)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(reports, domainObjects);

					if (string.IsNullOrWhiteSpace(customerCode) == false)
					{
						domainObjects = dc.AuditReport.Where(e => e.Code == customerCode).OrderBy(e => e.NN)
							//&& e.DomainType == domain)
							.ToList().Select(e => e.ToReportAuditDomainObject());
						Reports.Fill(reports, domainObjects);
					}
					if (string.IsNullOrWhiteSpace(branchCode) == false)
					{
						domainObjects = dc.AuditReport.Where(e => e.Code == branchCode).OrderBy(e => e.NN)
							//&& e.DomainType == domain)
							.ToList().Select(e => e.ToReportAuditDomainObject());
						Reports.Fill(reports, domainObjects);
					}
					if (string.IsNullOrWhiteSpace(inventorCode) == false)
					{
						domainObjects = dc.AuditReport.Where(e => e.Code == inventorCode).OrderBy(e => e.NN)
							//&& e.DomainType == domain)
								  .ToList().Select(e => e.ToReportAuditDomainObject());
						Reports.Fill(reports, domainObjects);
					}
				}
			}

			if (allowedReportTemplates.Contains(AllowedReportTemplate.Context) == true)
			{
				string contextFolder = this.GetViewDomainContextStartPathDictionary()[viewDomainContextType];
				string fromPath = this._connection.DBSettings.ReportTemplatePath() + "\\" + contextFolder;
				this.FillAllAllowedReportTemplate(reports, fromPath);
			}

			if (allowedReportTemplates.Contains(AllowedReportTemplate.All) == true)
			{
				Reports reportfiles = new Reports();
				this.FillAllAllowedReportTemplate(reportfiles, this._connection.DBSettings.ReportTemplatePath());
				Reports.Fill(reports, reportfiles);
			}

			foreach (Report report in reports)
			{
				bool allowed = this.IsAllowedSelectParmReport(viewDomainContextType, report.Path);
				report.AllowedContextSelectParm = allowed;
			}
			return reports;
		}

		private Reports GetReportListForSearchMenu(ViewDomainContextEnum viewDomainContextType)
		{
			Reports menuReports = new Reports();
			string domain = viewDomainContextType.ToString();
			string any = ReportTemplateDomainEnum.Any.ToString();

			if (viewDomainContextType == ViewDomainContextEnum.AuditConfigSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.AuditConfigSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.AuditConfigSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.BranchSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.BranchSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.BranchSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.CustomerSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.CustomerSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.CustomerSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.InventorSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.InventorSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.InventorSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.InventProductAdvancedSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.InventProductAdvancedSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.InventProductAdvancedSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.InventProductSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.InventProductSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.InventProductSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.InventProductSumAdvancedSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.InventProductSumAdvancedSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.InventProductSumAdvancedSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.IturAdvancedSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.IturAdvancedSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.IturAdvancedSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.IturSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.IturSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.IturSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.LocationSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.LocationSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.LocationSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.SupplierSearch)	 
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.SupplierSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.SupplierSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.FamilySearch)	 //TODO add FamilySearch
			{
				string domaintype = ViewDomainContextEnum.Family.ToString();
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.DomainType == domaintype).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.DomainType == domaintype).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.SectionSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.SectionSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.SectionSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}

			else if (viewDomainContextType == ViewDomainContextEnum.ProductSearch)
			{
				using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.ProductSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportAuditDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}

				using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var domainObjects = dc.Report.Where(e => e.Code == any && e.ProductSearchMenu == true).OrderBy(e => e.NN)
						.ToList().Select(e => e.ToReportMainDomainObject());
					Reports.Fill(menuReports, domainObjects);
				}
			}


			var retMenuReports = menuReports.AsEnumerable().OrderBy(n => n.GetType().GetProperty("NN").GetValue(n, null) as string);
			//menuReports = Reports.SortNN(menuReports);
			return Reports.FromEnumerable(retMenuReports);
		}


		public Count4U.GenerationReport.Report GetReportFastPrint(ViewDomainContextEnum viewDomainContextType, 
			string customerCode, string branchCode, string inventorCode)
		{
			Reports menuReports = new Reports();
			string domain = viewDomainContextType.ToString();
			string any = ReportTemplateDomainEnum.Any.ToString();

			if (viewDomainContextType == ViewDomainContextEnum.ItursItur)
			{

				if (string.IsNullOrWhiteSpace(customerCode) == false)
				{
					GetIturPrintPopupMenu(customerCode, menuReports);
				}

				if (string.IsNullOrWhiteSpace(branchCode) == false)
				{
					GetIturPrintPopupMenu(branchCode, menuReports);
				}

				if (string.IsNullOrWhiteSpace(inventorCode) == false)
				{
					GetIturPrintPopupMenu(inventorCode, menuReports);
				}

				GetIturPrintPopupMenu(any, menuReports);
			}

			//else if (viewDomainContextType == ViewDomainContextEnum.Iturs)
			//{
			//	using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString))
			//	{
			//		var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.ItursPopupMenu == true).OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportAuditDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}

			//	using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString))
			//	{
			//		var domainObjects = dc.Report.Where(e => e.Code == any && e.ItursPopupMenu == true).OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportMainDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}
			//}

				// to do e.ItursPopupMenu == true заменить ItursFamilyPopupMenu  [Rep-IS1-55]
			//else if (viewDomainContextType == ViewDomainContextEnum.ItursFamilyPrintMenu)
			//{
			//	using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString))
			//	{
			//		var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.ItursPopupMenu == true && e.CodeReport == "[Rep-IS1-55]").OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportAuditDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}

			//	using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString))
			//	{
			//		var domainObjects = dc.Report.Where(e => e.Code == any && e.ItursPopupMenu == true && e.CodeReport == "[Rep-IS1-55]").OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportMainDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}
			//}
				
			//// TODO [Rep-IS1-60]
			//else if (viewDomainContextType == ViewDomainContextEnum.ItursPrintMenu60)
			//{
			//	using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString))
			//	{
			//		var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.ItursPopupMenu == true && e.CodeReport == "[Rep-IS1-60]").OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportAuditDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}

			//	using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString))
			//	{
			//		var domainObjects = dc.Report.Where(e => e.Code == any && e.ItursPopupMenu == true && e.CodeReport == "[Rep-IS1-60]").OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportMainDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}

			//}

			//	// TODO [Rep-IS1-70]
			//else if (viewDomainContextType == ViewDomainContextEnum.ItursPrintMenu70)
			//{
			//	using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString))
			//	{
			//		var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.CodeReport == "[Rep-IS1-70]").OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportAuditDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}

			//	using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString))
			//	{
			//		var domainObjects = dc.Report.Where(e => e.Code == any && e.CodeReport == "[Rep-IS1-70]").OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportMainDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}
			//}

			//// TODO [Rep-IT2-03]
			//else if (viewDomainContextType == ViewDomainContextEnum.IturPrintMenuIT2_03)
			//{
			//	using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString))
			//	{
			//		var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.CodeReport == "[Rep-IT2-03]").OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportAuditDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}

			//	using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString))
			//	{
			//		var domainObjects = dc.Report.Where(e => e.Code == any && e.CodeReport == "[Rep-IT2-03]").OrderBy(e => e.NN)
			//			.ToList().Select(e => e.ToReportMainDomainObject());
			//		Reports.Fill(menuReports, domainObjects);
			//	}
			//}
			
			//var retMenuReports = menuReports.AsEnumerable().OrderBy(n => n.GetType().GetProperty("NN").GetValue(n, null) as string);
			//menuReports = Reports.SortNN(menuReports);
			var retMenuReport = menuReports.FirstOrDefault();
			return retMenuReport;
		}

		private void GetIturPrintPopupMenu(string code, Reports menuReports)
		{
			using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
			{
				var domainObjects = dc.AuditReport.Where(e => e.Code == code && e.IturPopupMenu == true).OrderBy(e => e.NN)
					.ToList().Select(e => e.ToReportAuditDomainObject());
				Reports.Fill(menuReports, domainObjects);
			}
			using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = dc.Report.Where(e => e.Code == code && e.IturPopupMenu == true).OrderBy(e => e.NN)
					.ToList();
				var domainObjects1 = domainObjects.Select(e => e.ToReportMainDomainObject());
				Reports.Fill(menuReports, domainObjects1);
			}
		}


		public Count4U.GenerationReport.Report GetReportFastPrintByReportCode(string reportCode)
		{
			Reports menuReports = new Reports();
			string any = ReportTemplateDomainEnum.Any.ToString();

			using (Count4U.Model.App_Data.AuditDB dc = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
			{
				var domainObjects = dc.AuditReport.Where(e => e.Code == any && e.CodeReport == reportCode).OrderBy(e => e.NN)
					.ToList().Select(e => e.ToReportAuditDomainObject());
				Reports.Fill(menuReports, domainObjects);
			}

			using (Count4U.Model.App_Data.MainDB dc = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = dc.Report.Where(e => e.Code == any && e.CodeReport == reportCode).OrderBy(e => e.NN)
					.ToList().Select(e => e.ToReportMainDomainObject());
				Reports.Fill(menuReports, domainObjects);
			}

			var retMenuReport = menuReports.FirstOrDefault();


			return retMenuReport;
		}

		private void FillAllAllowedReportTemplate(Reports reports, string reportTemplatePath)
		{
			//IsAllowedSelectParmReport(DomainContextEnum domainContextType, string pathReportTemplate)
			DirectoryInfo reportTemplateDrectoryInfo = new DirectoryInfo(reportTemplatePath);
			if (Directory.Exists(reportTemplateDrectoryInfo.FullName) == true)
			{
				string rootReportTemplatePath = this._connection.DBSettings.ReportTemplatePath();
				int letRootPath = rootReportTemplatePath.Length;
				string path = reportTemplateDrectoryInfo.FullName;
				string subPath = path.Substring(letRootPath).Trim('\\');
				if (GetDomainContextDataSetDictionary().ContainsKey(subPath) == true)
				{
					string domainContext = GetDomainContextDataSetDictionary()[subPath].ToString();
					foreach (FileInfo fi in reportTemplateDrectoryInfo.GetFiles("*.rdlc"))
					{
						reports.Add(new Report { FileName = fi.Name, Path = subPath.Trim('\\'), DomainContext = domainContext });
					}
				}


				foreach (DirectoryInfo diSubDir in reportTemplateDrectoryInfo.GetDirectories())
				{
					this.FillAllAllowedReportTemplate(reports, diSubDir.FullName);
				}
			}
		}

		public Reports GetAllowedReportTemplateTag(string customerCode, string branchCode,
			string inventorCode, ViewDomainContextEnum viewDomainContextEnum,
			List<AllowedReportTemplate> allowedReportTemplates)
		{
			Reports reports = this.GetAllowedReportTemplate(customerCode, branchCode,
			inventorCode, viewDomainContextEnum, allowedReportTemplates);

			Reports retReports = new Reports();
			foreach (var report in reports)
			{
				if (string.IsNullOrWhiteSpace(report.Tag) == false)
				{
					string[] tags = report.Tag.Split(',');
					if (tags.Length > 1)
					{
						foreach (var tag in tags)
						{
							report.Tag = tag.Trim(' ');
							retReports.Add(report);
						}
					}
					else if (tags.Length == 1)
					{
						retReports.Add(report);
					}
				}
			}
			return retReports;
		}


		public void Insert(Report report, AllowedReportTemplate allowedReportTemplate)
		{
			this.Delete(report, allowedReportTemplate);

			if (allowedReportTemplate == AllowedReportTemplate.Main)
			{
				using (var db = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entity = report.ToMainReportEntity();
					db.Report.AddObject(entity);
					db.SaveChanges();
				}
			}
			else if (allowedReportTemplate == AllowedReportTemplate.Audit)
			{
				using (var db = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var entity = report.ToAuditReportEntity();
					db.AuditReport.AddObject(entity);
					db.SaveChanges();
				}
			}
		}

		public void Update(Report report, AllowedReportTemplate allowedReportTemplate)
		{
			this.Insert(report, allowedReportTemplate);
		}

		public void Delete(Report report, AllowedReportTemplate allowedReportTemplate)
		{
			if (report == null) return;
			string reportFileName = report.FileName.ToLower();
			if (reportFileName == null) return;
			string reportPath = report.Path.ToLower();
			string reportCode = report.Code.ToLower();

			if (allowedReportTemplate == AllowedReportTemplate.Main)
			{
				using (var db = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entities = db.Report.Where(e => e.FileName.ToLower() == reportFileName
						&& e.Path.ToLower() == reportPath
						&& e.Code.ToLower() == reportCode).ToList();
					if (entities == null) return;
					entities.ForEach(e => db.Report.DeleteObject(e));
					db.SaveChanges();
				}
			}
			else if (allowedReportTemplate == AllowedReportTemplate.Audit)
			{
				using (var db = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var entities = db.AuditReport.Where(e => e.FileName.ToLower() == reportFileName
				&& e.Path.ToLower() == reportPath
				&& e.Code.ToLower() == reportCode).ToList();
					if (entities == null) return;
					entities.ForEach(e => db.AuditReport.DeleteObject(e));
					db.SaveChanges();
				}
			}
		}


		public bool IsAllowedSelectParmReport(ViewDomainContextEnum viewDomainContextEnum,
			string reportTemplatePath)
		{
			bool ret = false;
			//Dictionary<string, string> domainContextDataSetDictionary  = this.GetDomainContextDataSetDictionary();
			switch (viewDomainContextEnum)
			{
				case ViewDomainContextEnum.Catalog:
					if (reportTemplatePath == DomainContextPath.Catalog) ret = true;
					break;
				case ViewDomainContextEnum.Iturs:
					//case ViewDomainContextEnum.ItursAddIn:
					if (reportTemplatePath == DomainContextPath.Iturs
						|| reportTemplatePath == DomainContextPath.ItursAddIn
						|| reportTemplatePath == DomainContextPath.ItursAddInExcludeLocation
						|| reportTemplatePath == DomainContextPath.ItursItur
						|| reportTemplatePath == DomainContextPath.ItursIturAddIn
						|| reportTemplatePath == DomainContextPath.ItursIturDoc
						|| reportTemplatePath == DomainContextPath.ItursIturDocAddIn
						|| reportTemplatePath == DomainContextPath.ItursIturDocPDA
						|| reportTemplatePath == DomainContextPath.ItursIturDocPDAAddIn
						|| reportTemplatePath == DomainContextPath.ItursAddInSum
						|| reportTemplatePath == DomainContextPath.ItursAddInWorkerSession
						) ret = true;
					break;
				case ViewDomainContextEnum.Itur:
					//case ViewDomainContextEnum.ItursAddIn:
					if (reportTemplatePath == DomainContextPath.Itur
						 || reportTemplatePath == DomainContextPath.IturAddIn  
						) ret = true;
					break;
				case ViewDomainContextEnum.ItursItur:
					//case ViewDomainContextEnum.ItursIturAddIn:
					if (reportTemplatePath == DomainContextPath.ItursItur
						|| reportTemplatePath == DomainContextPath.ItursIturAddIn
						|| reportTemplatePath == DomainContextPath.ItursIturDoc
						|| reportTemplatePath == DomainContextPath.ItursIturDocAddIn
						|| reportTemplatePath == DomainContextPath.ItursIturDocPDA
						|| reportTemplatePath == DomainContextPath.ItursIturDocPDAAddIn
						|| reportTemplatePath == DomainContextPath.ItursAddInSum
						|| reportTemplatePath == DomainContextPath.ItursAddInWorkerSession
						) ret = true;
					break;
				case ViewDomainContextEnum.ItursIturDoc:
					//case ViewDomainContextEnum.ItursIturDocAddIn:
					if (reportTemplatePath == DomainContextPath.ItursIturDoc
						|| reportTemplatePath == DomainContextPath.ItursIturDocAddIn
						|| reportTemplatePath == DomainContextPath.ItursIturDocPDA
						|| reportTemplatePath == DomainContextPath.ItursIturDocPDAAddIn
						|| reportTemplatePath == DomainContextPath.ItursIturDocAddInSimple
						|| reportTemplatePath == DomainContextPath.ItursAddInSum
						|| reportTemplatePath == DomainContextPath.ItursAddInWorkerSession
						) ret = true;
					break;
				case ViewDomainContextEnum.ItursIturDocPDA:
					//case ViewDomainContextEnum.ItursIturDocPDAAddIn:
					if (reportTemplatePath == DomainContextPath.ItursIturDocPDA
						|| reportTemplatePath == DomainContextPath.ItursIturDocPDAAddIn)
						ret = true;
					break;
				case ViewDomainContextEnum.Doc:
					if (reportTemplatePath == DomainContextPath.ItursIturDoc
							|| reportTemplatePath == DomainContextPath.ItursIturDocAddIn)
						ret = true;
					break;
				//case DomainContextEnum.ItursIturDocAddInSimple:
				//if (reportTemplatePath == DomainContextPath.ItursIturDocAddInSimple)
				//        ret = true;
				//    break;
				//case ViewDomainContextEnum.ItursIturDocAddInSimple:	
				case ViewDomainContextEnum.PDA:
					//case ViewDomainContextEnum.ItursAddIn:
					if (reportTemplatePath == DomainContextPath.PDA	   
						) ret = true;
					break;
				case ViewDomainContextEnum.Location:
				if (reportTemplatePath == DomainContextPath.Location)
						ret = true;
					break;
				case ViewDomainContextEnum.Supplier:
					if (reportTemplatePath == DomainContextPath.Supplier)
						ret = true;
					break;
				case ViewDomainContextEnum.Device:
					if (reportTemplatePath == DomainContextPath.Device)
						ret = true;
					break;
				case ViewDomainContextEnum.DeviceWorker:
					if (reportTemplatePath == DomainContextPath.DeviceWorker)
						ret = true;
					break;
				case ViewDomainContextEnum.Family:
					if (reportTemplatePath == DomainContextPath.Family)
						ret = true;
					break;
				case ViewDomainContextEnum.Section:
					if (reportTemplatePath == DomainContextPath.Section)
						ret = true;
					break;
				//case ViewDomainContextEnum.DocAddIn:
				//case ViewDomainContextEnum.DocPDA:
				//case ViewDomainContextEnum.DocPDAAddIn:
				//case ViewDomainContextEnum.PDA:
				//case ViewDomainContextEnum.PDAAddIn:
				case ViewDomainContextEnum.Customer:
					if (reportTemplatePath == DomainContextPath.Customer)
						ret = true;
					break;
				case ViewDomainContextEnum.Branch:
					if (reportTemplatePath == DomainContextPath.Branch)
						ret = true;
					break;
				case ViewDomainContextEnum.Inventor:
					if (reportTemplatePath == DomainContextPath.Inventor)
						ret = true;
					break;
				case ViewDomainContextEnum.AuditConfig:
					if (reportTemplatePath == DomainContextPath.AuditConfig)
						ret = true;
					break;
				case ViewDomainContextEnum.All:
				default:
					break;
			}
			return ret;
		}

		/// <summary>
		/// key - ViewDomainContextEnum   - from view
		/// value - DomainContextPath  - startFolderPath for ReportTemplate
		/// </summary>
		/// <returns></returns>
		public Dictionary<ViewDomainContextEnum, string> GetViewDomainContextStartPathDictionary()
		{
			if (this._domainContextPathDictionary == null)
			{
				this._domainContextPathDictionary = new Dictionary<ViewDomainContextEnum, string>();
				this._domainContextPathDictionary[ViewDomainContextEnum.Location] = DomainContextPath.Location;
				this._domainContextPathDictionary[ViewDomainContextEnum.Section] = DomainContextPath.Section;
				this._domainContextPathDictionary[ViewDomainContextEnum.Supplier] = DomainContextPath.Supplier;
				this._domainContextPathDictionary[ViewDomainContextEnum.Device] = DomainContextPath.Device;
				this._domainContextPathDictionary[ViewDomainContextEnum.DeviceWorker] = DomainContextPath.DeviceWorker;
				this._domainContextPathDictionary[ViewDomainContextEnum.Family] = DomainContextPath.Family;
				this._domainContextPathDictionary[ViewDomainContextEnum.Catalog] = DomainContextPath.Catalog;
				this._domainContextPathDictionary[ViewDomainContextEnum.ItursItur] = DomainContextPath.ItursItur;
				//this._domainContextPathDictionary.Add(ViewDomainContextEnum.ItursIturAddIn, DomainContextPath.ItursIturAddIn);
				this._domainContextPathDictionary[ViewDomainContextEnum.Iturs] = DomainContextPath.Iturs;
				//this._domainContextPathDictionary.Add(ViewDomainContextEnum.ItursAddIn, DomainContextPath.ItursAddIn);
				this._domainContextPathDictionary[ViewDomainContextEnum.Itur] = DomainContextPath.Itur;
				this._domainContextPathDictionary[ViewDomainContextEnum.ItursIturDoc] = DomainContextPath.ItursIturDoc;
				//this._domainContextPathDictionary.Add(ViewDomainContextEnum.ItursIturDocAddIn, DomainContextPath.ItursIturDocPDAAddIn);
				//this._domainContextPathDictionary.Add(ViewDomainContextEnum.ItursIturDocAddInSimple, DomainContextPath.ItursIturDocAddInSimple);
				this._domainContextPathDictionary[ViewDomainContextEnum.ItursIturDocPDA] = DomainContextPath.ItursIturDocPDA;
				//this._domainContextPathDictionary.Add(ViewDomainContextEnum.ItursIturDocPDAAddIn, DomainContextPath.ItursIturDocPDAAddIn);
				this._domainContextPathDictionary[ViewDomainContextEnum.Doc] = DomainContextPath.Doc;
				//this._domainContextPathDictionary.Add(ViewDomainContextEnum.DocAddIn, DomainContextPath.DocAddIn);
				this._domainContextPathDictionary[ViewDomainContextEnum.DocPDA] = DomainContextPath.DocPDA;
				//this._domainContextPathDictionary.Add(ViewDomainContextEnum.DocPDAAddIn, DomainContextPath.DocPDAAddIn);
				this._domainContextPathDictionary[ViewDomainContextEnum.PDA] = DomainContextPath.PDA;
				//this._domainContextPathDictionary.Add(ViewDomainContextEnum.PDAAddIn, DomainContextPath.PDAAddIn);
				this._domainContextPathDictionary[ViewDomainContextEnum.Customer] = DomainContextPath.Customer;
				this._domainContextPathDictionary[ViewDomainContextEnum.Branch] = DomainContextPath.Branch;
				this._domainContextPathDictionary[ViewDomainContextEnum.Inventor] = DomainContextPath.Inventor;
				this._domainContextPathDictionary[ViewDomainContextEnum.AuditConfig] = DomainContextPath.AuditConfig;
				this._domainContextPathDictionary[ViewDomainContextEnum.All] = DomainContextPath.All;
			}
			return this._domainContextPathDictionary;
		}

		/// <summary>
		/// key - PathReportTemplate 
		/// value - DS Name
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetDomainContextDataSetDictionary()		//DomainContextName, DomainContextDataSet
		{
			if (this._domainContextDataSetDictionary == null)
			{
				this._domainContextDataSetDictionary = new Dictionary<string, string>();
				this._domainContextDataSetDictionary[DomainContextPath.Location] = DomainContextDataSet.Location;
				this._domainContextDataSetDictionary[DomainContextPath.Section] = DomainContextDataSet.Section;
				this._domainContextDataSetDictionary[DomainContextPath.Supplier] = DomainContextDataSet.Supplier;
				this._domainContextDataSetDictionary[DomainContextPath.Device] = DomainContextDataSet.Device;
				this._domainContextDataSetDictionary[DomainContextPath.DeviceWorker] = DomainContextDataSet.DeviceWorker;
				this._domainContextDataSetDictionary[DomainContextPath.Family] = DomainContextDataSet.Family;
				this._domainContextDataSetDictionary[DomainContextPath.Catalog] = DomainContextDataSet.Catalog;
				this._domainContextDataSetDictionary[DomainContextPath.Itur] = DomainContextDataSet.Itur;
				this._domainContextDataSetDictionary[DomainContextPath.IturAddIn] = DomainContextDataSet.Itur;
				this._domainContextDataSetDictionary[DomainContextPath.Iturs] = DomainContextDataSet.IturDocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.ItursAddIn] = DomainContextDataSet.IturDocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.ItursAddInExcludeLocation] = DomainContextDataSet.IturDocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.ItursAddInSum] = DomainContextDataSet.IturAddInSum;
				this._domainContextDataSetDictionary[DomainContextPath.ItursAddInWorkerSession] = DomainContextDataSet.IturAddInWorkerSession;
				this._domainContextDataSetDictionary[DomainContextPath.ItursItur] = DomainContextDataSet.IturDocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.ItursIturAddIn] = DomainContextDataSet.IturDocPDA;  
				this._domainContextDataSetDictionary[DomainContextPath.ItursIturDoc] = DomainContextDataSet.IturDocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.ItursIturDocAddIn] = DomainContextDataSet.IturDocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.ItursIturDocAddInSimple] = DomainContextDataSet.IturDocAddInSimple;
				this._domainContextDataSetDictionary[DomainContextPath.ItursIturDocPDA] = DomainContextDataSet.IturDocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.ItursIturDocPDAAddIn] = DomainContextDataSet.IturDocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.Doc] = DomainContextDataSet.Doc;
				this._domainContextDataSetDictionary[DomainContextPath.DocAddIn] = DomainContextDataSet.Doc;
				this._domainContextDataSetDictionary[DomainContextPath.DocPDA] = DomainContextDataSet.DocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.DocPDAAddIn] = DomainContextDataSet.DocPDA;
				this._domainContextDataSetDictionary[DomainContextPath.PDA] = DomainContextDataSet.PDA;
				this._domainContextDataSetDictionary[DomainContextPath.PDAAddIn] = DomainContextDataSet.PDA;
				this._domainContextDataSetDictionary[DomainContextPath.Context] = DomainContextDataSet.ContextReport;
				this._domainContextDataSetDictionary[DomainContextPath.ContextIturList] = DomainContextDataSet.ContextReportIturList;
				
				this._domainContextDataSetDictionary[DomainContextPath.Customer] = DomainContextDataSet.Customer;
				this._domainContextDataSetDictionary[DomainContextPath.Branch] = DomainContextDataSet.Branch;
				this._domainContextDataSetDictionary[DomainContextPath.Inventor] = DomainContextDataSet.Inventor;
				this._domainContextDataSetDictionary[DomainContextPath.AuditConfig] = DomainContextDataSet.AuditConfig;
				this._domainContextDataSetDictionary[DomainContextPath.All] = DomainContextDataSet.ContextReport;

			}
			return this._domainContextDataSetDictionary;
		}

		//public Dictionary<string, string> GetDomainContextDataSetDictionary()		//DomainContextName, DomainContextDataSet
		//{
		//    if (this._domainContextDataSetDictionary == null)
		//    {
		//        this._domainContextDataSetDictionary = new Dictionary<string, string>();
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.Location, DomainContextDataSet.Location);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.Catalog, DomainContextDataSet.Catalog);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.Itur, DomainContextDataSet.Itur);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.IturAddIn, DomainContextDataSet.Itur);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.IturDoc, DomainContextDataSet.IturDoc);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.IturDocAddIn, DomainContextDataSet.IturDoc);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.IturDocPDA, DomainContextDataSet.IturDocPDA);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.IturDocPDAAddIn, DomainContextDataSet.IturDocPDA);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.Doc, DomainContextDataSet.Doc);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.DocAddIn, DomainContextDataSet.Doc);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.DocPDA, DomainContextDataSet.DocPDA);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.DocPDAAddIn, DomainContextDataSet.DocPDA);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.PDA, DomainContextDataSet.PDA);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.PDAAddIn, DomainContextDataSet.PDA);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.Context, DomainContextDataSet.ContextReport);
		//        this._domainContextDataSetDictionary.Add(DomainContextPath.All, DomainContextDataSet.ContextReport);
		//    }
		//    return this._domainContextDataSetDictionary;

		//}

		/// <summary>
		/// key - PathReportTemplate 
		/// value - ViewDomainContext
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, ViewDomainContextEnum> GetViewDomainContextEnumDictionary()		//DomainContextName, DomainContextDataSet
		{
			if (this._domainContextDictionary == null)
			{
				this._domainContextDictionary = new Dictionary<string, ViewDomainContextEnum>();
				this._domainContextDictionary.Add(DomainContextPath.Location, ViewDomainContextEnum.Location);
				this._domainContextDictionary.Add(DomainContextPath.Section, ViewDomainContextEnum.Section);
				this._domainContextDictionary.Add(DomainContextPath.Supplier, ViewDomainContextEnum.Supplier);
				this._domainContextDictionary.Add(DomainContextPath.Device, ViewDomainContextEnum.Device);
				this._domainContextDictionary.Add(DomainContextPath.DeviceWorker, ViewDomainContextEnum.DeviceWorker);
				this._domainContextDictionary.Add(DomainContextPath.Family, ViewDomainContextEnum.Family);
				this._domainContextDictionary.Add(DomainContextPath.Catalog, ViewDomainContextEnum.Catalog);
				this._domainContextDictionary.Add(DomainContextPath.Itur, ViewDomainContextEnum.Itur);
				this._domainContextDictionary.Add(DomainContextPath.IturAddIn, ViewDomainContextEnum.Itur);
				this._domainContextDictionary.Add(DomainContextPath.Iturs, ViewDomainContextEnum.Iturs);
				this._domainContextDictionary.Add(DomainContextPath.ItursAddIn, ViewDomainContextEnum.Iturs);
				this._domainContextDictionary.Add(DomainContextPath.ItursAddInExcludeLocation, ViewDomainContextEnum.Iturs);
				this._domainContextDictionary.Add(DomainContextPath.ItursAddInSum, ViewDomainContextEnum.Iturs);
				this._domainContextDictionary.Add(DomainContextPath.ItursAddInWorkerSession, ViewDomainContextEnum.Iturs);
				this._domainContextDictionary.Add(DomainContextPath.ItursItur, ViewDomainContextEnum.ItursItur);
				this._domainContextDictionary.Add(DomainContextPath.ItursIturAddIn, ViewDomainContextEnum.ItursItur);
				this._domainContextDictionary.Add(DomainContextPath.ItursIturDoc, ViewDomainContextEnum.ItursIturDoc);
				this._domainContextDictionary.Add(DomainContextPath.ItursIturDocAddIn, ViewDomainContextEnum.ItursIturDoc);
				this._domainContextDictionary.Add(DomainContextPath.ItursIturDocAddInSimple, ViewDomainContextEnum.ItursIturDoc);
				this._domainContextDictionary.Add(DomainContextPath.ItursIturDocPDA, ViewDomainContextEnum.ItursIturDocPDA);
				this._domainContextDictionary.Add(DomainContextPath.ItursIturDocPDAAddIn, ViewDomainContextEnum.ItursIturDocPDA);
				this._domainContextDictionary.Add(DomainContextPath.Doc, ViewDomainContextEnum.Doc);
				this._domainContextDictionary.Add(DomainContextPath.DocAddIn, ViewDomainContextEnum.Doc);
				this._domainContextDictionary.Add(DomainContextPath.DocPDA, ViewDomainContextEnum.DocPDA);
				this._domainContextDictionary.Add(DomainContextPath.DocPDAAddIn, ViewDomainContextEnum.DocPDA);
				this._domainContextDictionary.Add(DomainContextPath.PDA, ViewDomainContextEnum.PDA);
				this._domainContextDictionary.Add(DomainContextPath.PDAAddIn, ViewDomainContextEnum.PDA);

				this._domainContextDictionary.Add(DomainContextPath.Customer, ViewDomainContextEnum.Customer);
				this._domainContextDictionary.Add(DomainContextPath.Branch, ViewDomainContextEnum.Branch);
				this._domainContextDictionary.Add(DomainContextPath.Inventor, ViewDomainContextEnum.Inventor);
				this._domainContextDictionary.Add(DomainContextPath.AuditConfig, ViewDomainContextEnum.AuditConfig);
			}
			return this._domainContextDictionary;

		}

		public bool IsSearchView(ViewDomainContextEnum clickOnView)
		{
			bool ret = false;

			if (clickOnView == ViewDomainContextEnum.InventProductAdvancedSearch
				|| clickOnView == ViewDomainContextEnum.InventProductSumAdvancedSearch
				|| clickOnView == ViewDomainContextEnum.IturAdvancedSearch
				|| clickOnView == ViewDomainContextEnum.CustomerSearch
				|| clickOnView == ViewDomainContextEnum.BranchSearch
				|| clickOnView == ViewDomainContextEnum.InventorSearch
				|| clickOnView == ViewDomainContextEnum.AuditConfigSearch
				|| clickOnView == ViewDomainContextEnum.IturSearch
				|| clickOnView == ViewDomainContextEnum.InventProductSearch
				|| clickOnView == ViewDomainContextEnum.LocationSearch
				|| clickOnView == ViewDomainContextEnum.ProductSearch
				|| clickOnView == ViewDomainContextEnum.SectionSearch
				|| clickOnView == ViewDomainContextEnum.SupplierSearch
				|| clickOnView == ViewDomainContextEnum.FamilySearch
				
				)
			{
				ret = true;
			}
			return ret;
		}

		public Reports GetReportByCodeReport(string codeReport, 
			AllowedReportTemplate allowedReportTemplate = AllowedReportTemplate.Main)
		{
			if (allowedReportTemplate == AllowedReportTemplate.Main)
			{
				using (var db = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entities = db.Report.Where(e => e.CodeReport.ToLower() == codeReport.ToLower()).
								ToList().Select(e => e.ToReportMainDomainObject());
					return Reports.FromEnumerable(entities);
				}
			}
			else if (allowedReportTemplate == AllowedReportTemplate.Audit)
			{
				using (var db = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var entities = db.AuditReport.Where(e => e.CodeReport.ToLower() == codeReport.ToLower()).
								ToList().Select(e => e.ToReportAuditDomainObject());
					return Reports.FromEnumerable(entities);
				}
			}
			else return new Reports();
		}

		public void ClearTag(AllowedReportTemplate allowedReportTemplate = AllowedReportTemplate.Main)
		{
			if (allowedReportTemplate == AllowedReportTemplate.Main)
			{
				using (var db = new Count4U.Model.App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entities = db.Report.Select(e => e);
					foreach (var entity in  entities)
					{
						entity.Tag = "";
						entity.MenuCaption = "";
					}
					db.SaveChanges();
				}
			}
			else if (allowedReportTemplate == AllowedReportTemplate.Audit)
			{
				using (var db = new Count4U.Model.App_Data.AuditDB(this.AuditConnectionString()))
				{
					var entities = db.AuditReport.Select(e => e);
					foreach (var entity in entities)
					{
						entity.Tag = "";
						entity.MenuCaption = "";
					}
					db.SaveChanges();
				}
			}
		}
	}
}
