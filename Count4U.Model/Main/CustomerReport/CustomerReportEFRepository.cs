using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Main.MappingEF;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Main
{
	public class CustomerReportEFRepository : BaseEFRepository, ICustomerReportRepository
    {
		private Branch _current;

		public CustomerReportEFRepository(IConnectionDB connection)
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

		#region IMaskRepository Members
		//CustomerReports GetCustomerReports();
		//CustomerReport GetCustomerReport(long id);
		//CustomerReports GetCustomerReports(SelectParams selectParams);
		//void Delete(long id);
		//void Insert(CustomerReport customerReport);
		//void Update(CustomerReport customerReport);

		public CustomerReports GetCustomerReports()
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				try
				{
					var entertis = dc.CustomerReport;
					var entertiList = entertis.ToList();
					var domainObjects = entertiList.Select(e => e.ToDomainObject());
					//var domainObjects = dc.CustomerReport.ToList().Select(e => e.ToDomainObject());
					return CustomerReports.FromEnumerable(domainObjects);
				}
				catch(Exception ext)
				{
					string message = ext.Message;
					return new CustomerReports();
				}
			}
		}

		public CustomerReports GetCustomerReports(SelectParams selectParams)
		{
		    if (selectParams == null)
				return GetCustomerReports();

		    long totalCount = 0;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
		    {
				 //Получение сущностей и общего количества из БД.
				 //Getting entities and total count from database.
				var entities = GetEntities(dc, AsQueryable(dc.CustomerReport), dc.CustomerReport.AsQueryable(), selectParams, out totalCount);

				 //Преобразование сущностей в объекты предметной области.
				 //Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToDomainObject());

				 //Возврат результата.
				 //Returning result.
				CustomerReports result = CustomerReports.FromEnumerable(domainObjects);
		        result.TotalCount = totalCount;
		        return result;
		    }
		}

		public Dictionary<string, CustomerReport> GetCustomerReportDictionary()
		{
			CustomerReports customerReports = GetCustomerReports();
			Dictionary<string, CustomerReport> dic = customerReports.Select(e => e).Distinct().ToDictionary(k => k.ReportCode);
			return dic;
		}

		public CustomerReport GetCustomerReport(long id)
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntitiesById(dc, id);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public CustomerReport GetCustomerReportByCodeAndContext(string code, string context)
		{
			if (string.IsNullOrWhiteSpace(code) == true) return null;
			if (string.IsNullOrWhiteSpace(context) == true) return null;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntitieByCodeAndContext(dc, code, context);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public List<CustomerReport> GetCustomerReportListByCodeAndContext(string code, string context)
		{
			if (string.IsNullOrWhiteSpace(code) == true) return null;
			if (string.IsNullOrWhiteSpace(context) == true) return null;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entities = this.GetEntitiesByCodeAndContext(dc, code, context);
				if (entities == null) return null;
				var domainObjects = entities.Select(e => e.ToDomainObject());
				return domainObjects.ToList();
			}
		}

		public void UpdateCustomerReportByCodeAndContext(CustomerReport customerReport)
		{
			if (customerReport == null) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntitieByCodeAndContext(dc, customerReport.ReportCode, customerReport.CustomerCode);
				if (entity != null)
				{
					dc.CustomerReport.DeleteObject(entity);
				}
				dc.SaveChanges();

				var newEntity = customerReport.ToEntity();
				dc.CustomerReport.AddObject(newEntity);
				dc.SaveChanges();
			}
		}


		public void Delete(long id)
		{
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntitiesById(db, id);
				if (entity == null) return;
				db.CustomerReport.DeleteObject(entity);
				db.SaveChanges();
			}
		}


		public void Insert(CustomerReport customerReport)
		{
			if (customerReport == null) return;
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = customerReport.ToEntity();
				db.CustomerReport.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(CustomerReports customerReports)
		{
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (var customerReport in customerReports)
				{
					var entity = customerReport.ToEntity();
					db.CustomerReport.AddObject(entity);
				}
				db.SaveChanges();
			}
		}

		public void DeleteAllNotTag()
		{
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				db.CustomerReport.Where(e => (e.ReportCode != "Tag")).ToList().ForEach(e => db.CustomerReport.DeleteObject(e));
			//	db.CustomerReport.Where(e => (e.MenuCaption != "Tag")).ToList().ForEach(e => db.CustomerReport.DeleteObject(e));
				
				db.SaveChanges();
			}
		}

	
		public void DeleteAllCodeAndContext(string code, string context)
		{
			if (string.IsNullOrWhiteSpace(code) == true) return ;
			if (string.IsNullOrWhiteSpace(context) == true) return ;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entities = this.GetEntitiesByCodeAndContext(dc, code, context);
				if (entities == null) return;
				entities.ToList().ForEach(e => dc.CustomerReport.DeleteObject(e));
				dc.SaveChanges();
			}
		}

		public void Update(CustomerReport customerReport)
		{
			if (customerReport == null) return;
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntitieByCode(db, customerReport.ReportCode);
				if (entity == null) return;
				entity.ApplyChanges(customerReport);
				db.SaveChanges();
			}
		}

		public void SaveTag(CustomerReports customerReports)
		{
			if (customerReports == null) return;
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				db.CustomerReport.Where(e => (e.ReportCode != "Tag")).ToList().ForEach(e => db.CustomerReport.DeleteObject(e));
				db.SaveChanges();

				foreach (var customerReport in customerReports)
				{
					if (string.IsNullOrWhiteSpace(customerReport.ReportCode) == true) continue;
					var entity = customerReport.ToEntity();
					db.CustomerReport.AddObject(entity);
				}
				db.SaveChanges();
			}
		}

		
		#endregion

		#region private

		private List<App_Data.CustomerReport> GetEntitiesByCode(App_Data.MainDB db, string code)
		{
			var entitis = db.CustomerReport.Where(e => (e.ReportCode == code)).ToList();
			return entitis;
		}

		private App_Data.CustomerReport GetEntitieByCodeAndContext(App_Data.MainDB db, string code, string context)
		{
			var entity = db.CustomerReport.Where(e => (e.ReportCode == code && e.CustomerCode == context)).FirstOrDefault();
			return entity;
		}

		private List<App_Data.CustomerReport> GetEntitiesByCodeAndContext(App_Data.MainDB db, string code, string context)
		{
			var entitis = db.CustomerReport.Where(e => (e.ReportCode == code && e.CustomerCode == context)).ToList();
			return entitis;
		}
		private App_Data.CustomerReport GetEntitieByCode(App_Data.MainDB db, string code)
		{
			var entiti = db.CustomerReport.Where(e => (e.ReportCode == code)).FirstOrDefault();
			return entiti;
		}
		private App_Data.CustomerReport GetEntitiesById(App_Data.MainDB db, long id)
		{
			var entity = db.CustomerReport.Where(e => (e.ID == id)).FirstOrDefault();
			return entity;
		}
		#endregion


		
	}
}
