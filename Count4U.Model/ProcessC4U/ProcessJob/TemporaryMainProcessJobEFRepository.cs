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
	public class TemporaryMainProcessJobEFRepository : BaseEFRepository, ITemporaryMainProcessJobRepository
    {

		public TemporaryMainProcessJobEFRepository(IConnectionDB connection)
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


		public ProcessJobs GetTemporaryMainProcessJobs()
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				try
				{
					var entertis = dc.TemporaryMainProcessJob;
					var entertiList = entertis.ToList();
					var domainObjects = entertiList.Select(e => e.ToTemporaryMainDomainObject());
					//var domainObjects = dc.MainProcessJob.ToList().Select(e => e.ToDomainObject());
					return ProcessJobs.FromEnumerable(domainObjects);
				}
				catch(Exception ext)
				{
					string message = ext.Message;
					return new ProcessJobs();
				}
			}
		}

		public ProcessJobs GetTemporaryMainProcessJobs(SelectParams selectParams)
		{
		    if (selectParams == null)
				return GetTemporaryMainProcessJobs();

		    long totalCount = 0;
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
		    {
				 //Получение сущностей и общего количества из БД.
				 //Getting entities and total count from database.
				var entities = GetEntities(dc, AsQueryable(dc.TemporaryMainProcessJob), dc.TemporaryMainProcessJob.AsQueryable(), selectParams, out totalCount);

				 //Преобразование сущностей в объекты предметной области.
				 //Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToTemporaryMainDomainObject());

				 //Возврат результата.
				 //Returning result.
				ProcessJobs result = ProcessJobs.FromEnumerable(domainObjects);
		        result.TotalCount = totalCount;
		        return result;
		    }
		}

		public ProcessJob GetTemporaryMainProcessJob(long id)
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(dc, id);
				if (entity == null) return null;
				return entity.ToTemporaryMainDomainObject();
			}
		}

		public void Delete(long id)
		{
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(db, id);
				if (entity == null) return;
				db.TemporaryMainProcessJob.DeleteObject(entity);
				db.SaveChanges();
			}
		}



		public void Insert(ProcessJob temporaryMainProcessJob)
		{
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = temporaryMainProcessJob.ToTemporaryMainEntity();
				db.TemporaryMainProcessJob.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Update(ProcessJob temporaryMainProcessJob)
		{
			if (temporaryMainProcessJob == null) return;
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(db, temporaryMainProcessJob.ID);
				if (entity == null) return;
				entity.ApplyTemporaryMainChanges(temporaryMainProcessJob);
				db.SaveChanges();
			}
		}

		private App_Data.TemporaryMainProcessJob GetEntitiesById(App_Data.ProcessDB db, long id)
		{
			var entity = db.TemporaryMainProcessJob.Where(e => (e.ID == id)).FirstOrDefault();
			return entity;
		}


	
	}
}
