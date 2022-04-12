using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Main
{
    /// <summary>
    /// Интерфейс репозитория для доступа к отчетам -  CustomerReport объектам
    /// </summary>
    public interface ICustomerReportRepository
    {
		CustomerReports GetCustomerReports();
		CustomerReport GetCustomerReport(long id);
		CustomerReport GetCustomerReportByCodeAndContext(string code, string context);
		List<CustomerReport> GetCustomerReportListByCodeAndContext(string code, string context);
		void UpdateCustomerReportByCodeAndContext(CustomerReport customerReport);
		CustomerReports GetCustomerReports(SelectParams selectParams);
		void Delete(long id);
		void Insert(CustomerReport customerReport);
		void Insert(CustomerReports customerReports);
		 void DeleteAllNotTag();
		void DeleteAllCodeAndContext(string code, string context);
		void Update(CustomerReport customerReport);
		void SaveTag(CustomerReports customerReports);
		Dictionary<string, CustomerReport> GetCustomerReportDictionary();

	}
}
