using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;

namespace Count4U.Model.Main
{
    public class CustomerReportRepository: ICustomerReportRepository
    {
        //private CustomerReports this._customerReports;

         #region ICustomerReportRepository Members

        public CustomerReports GetCustomerReports()
        {
            throw new NotImplementedException();
        }

        #endregion

		#region ICustomerReportRepository Members


		public CustomerReport GetCustomerReport(long id)
		{
			throw new NotImplementedException();
		}

		public CustomerReports GetCustomerReports(SelectionParams.SelectParams selectParams)
		{
			throw new NotImplementedException();
		}

		public void Delete(long id)
		{
			throw new NotImplementedException();
		}

		public void Insert(CustomerReport customerReport)
		{
			throw new NotImplementedException();
		}

		public void Update(CustomerReport customerReport)
		{
			throw new NotImplementedException();
		}

		public void Insert(CustomerReports customerReports)
		{
			throw new NotImplementedException();
		}

		public void DeleteAll(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void SaveTag(CustomerReports customerReports)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, CustomerReport> GetCustomerReportDictionary()
		{
			throw new NotImplementedException();
		}

		public void DeleteAllNotTag()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerReportRepository Members


		public CustomerReport GetCustomerReportByCodeAndContext(string code, string context)
		{
			throw new NotImplementedException();
		}

		public void UpdateCustomerReportByCodeAndContext(CustomerReport customerReport)
		{
			throw new NotImplementedException();
		}

		public List<CustomerReport> GetCustomerReportListByCodeAndContext(string code, string context)
		{
			throw new NotImplementedException();
		}

		public void DeleteAllCodeAndContext(string code, string context)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
