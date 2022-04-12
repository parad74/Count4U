using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using Count4U.Model.ProcessC4U.MappingEF;
using Count4U.Model.Interface.ProcessC4U;

namespace Count4U.Model.ProcessC4U
{
	public class ProcessEFRepository : BaseEFRepository, IProcessRepository
    {
		public ProcessEFRepository(IConnectionDB connection)
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
		//Processes GetProcesses();
		//Process GetProcess(long id);
		//Processes GetProcesses(SelectParams selectParams);
		//void Delete(long id);
		//void Insert(Process Process);
		//void Update(Process Process);

		public Processes GetProcesses()
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				try
				{
					var entertis = dc.Process;
					var entertiList = entertis.ToList();
					var domainObjects = entertiList.Select(e => e.ToDomainObject());
					//var domainObjects = dc.Process.ToList().Select(e => e.ToDomainObject());
					return Processes.FromEnumerable(domainObjects);
				}
				catch(Exception ext)
				{
					string message = ext.Message;
					return new Processes();
				}
			}
		}

		public Processes GetProcesses(SelectParams selectParams)
		{
		    if (selectParams == null)
				return GetProcesses();

		    long totalCount = 0;
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
		    {
				 //Получение сущностей и общего количества из БД.
				 //Getting entities and total count from database.
				var entities = GetEntities(dc, AsQueryable(dc.Process), dc.Process.AsQueryable(), selectParams, out totalCount);

				 //Преобразование сущностей в объекты предметной области.
				 //Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToDomainObject());

				 //Возврат результата.
				 //Returning result.
				Processes result = Processes.FromEnumerable(domainObjects);
		        result.TotalCount = totalCount;
		        return result;
		    }
		}

		public Process GetProcess(long id)
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(dc, id);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public Process GetProcessByProcessCode(string processCode)
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesByProcessCode(dc, processCode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public void SetStatusInProcess(string processCode)
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				foreach (var pr in dc.Process)
				{
					pr.StatusCode = StatusAuditConfigEnum.NotCurrent.ToString();
				}
				var entity = this.GetEntitiesByProcessCode(dc, processCode);
				if (entity == null) return;
				entity.StatusCode = StatusAuditConfigEnum.InProcess.ToString();
				dc.SaveChanges();
			}
		}


		public void ResetStatusInProcess()
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				foreach (var pr in dc.Process)
				{
					pr.StatusCode = StatusAuditConfigEnum.NotCurrent.ToString();
				}
			dc.SaveChanges();
			}
		}

		public Process GetProcess_InProcess()
		{
			string inprocess = StatusAuditConfigEnum.InProcess.ToString();
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = dc.Process.Where(e => (e.StatusCode == inprocess)).FirstOrDefault();
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public string GetProcessCode_InProcess()
		{
			string inprocess = StatusAuditConfigEnum.InProcess.ToString();

			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = dc.Process.Where(e => (e.StatusCode == inprocess)).FirstOrDefault();
				if (entity == null) return "";
				return entity.ProcessCode;
			}
		}


		public Process GetProcessByCode(string code)
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesByCode(dc, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public void Delete(long id)
		{
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(db, id);
				if (entity == null) return;
				db.Process.DeleteObject(entity);
				db.SaveChanges();
			}
		}


		public void Delete(string processCode)
		{
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesByProcessCode(db, processCode);
				if (entity == null) return;
				db.Process.DeleteObject(entity);
				db.SaveChanges();
			}
		}
		


		public void Insert(Process Process)
		{
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = Process.ToEntity();
				db.Process.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Update(Process process)
		{
			if (process == null) return;
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(db, process.ID);
				if (entity == null) return;
				entity.ApplyChanges(process);
				db.SaveChanges();
			}
		}
		#endregion

		#region private
	
		private App_Data.Process GetEntitiesById(App_Data.ProcessDB db, long id)
		{
			var entity = db.Process.Where(e => (e.ID == id)).FirstOrDefault();
			return entity;
		}

		private App_Data.Process GetEntitiesByProcessCode(App_Data.ProcessDB db, string processCode)
		{
			var entity = db.Process.Where(e => (e.ProcessCode == processCode)).FirstOrDefault();
			return entity;
		}

		private App_Data.Process GetEntitiesByCode(App_Data.ProcessDB db, string code)
		{
			var entity = db.Process.Where(e => (e.Code == code)).FirstOrDefault();
			return entity;
		}
		#endregion


		
	}
}
