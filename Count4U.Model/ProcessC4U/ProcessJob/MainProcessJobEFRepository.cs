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
	public class MainProcessJobEFRepository : BaseEFRepository, IMainProcessJobRepository
    {

		public MainProcessJobEFRepository(IConnectionDB connection)
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
	
		public ProcessJobs GetMainProcessJobs()
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				try
				{
					var entertis = dc.MainProcessJob;
					var entertiList = entertis.ToList();
					var domainObjects = entertiList.Select(e => e.ToMainDomainObject());
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

		public ProcessJobs GetMainProcessJobs(SelectParams selectParams)
		{
		    if (selectParams == null)
				return GetMainProcessJobs();

		    long totalCount = 0;
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
		    {
				 //Получение сущностей и общего количества из БД.
				 //Getting entities and total count from database.
				var entities = GetEntities(dc, AsQueryable(dc.MainProcessJob), dc.MainProcessJob.AsQueryable(), selectParams, out totalCount);

				 //Преобразование сущностей в объекты предметной области.
				 //Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToMainDomainObject());

				 //Возврат результата.
				 //Returning result.
				ProcessJobs result = ProcessJobs.FromEnumerable(domainObjects);
		        result.TotalCount = totalCount;
		        return result;
		    }
		}

		public ProcessJob GetMainProcessJob(long id)
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(dc, id);
				if (entity == null) return null;
				return entity.ToMainDomainObject();
			}
		}

		public void Delete(long id)
		{
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(db, id);
				if (entity == null) return;
				db.MainProcessJob.DeleteObject(entity);
				db.SaveChanges();
			}
		}



		public void Insert(ProcessJob mainProcessJob)
		{
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = mainProcessJob.ToMainEntity();
				db.MainProcessJob.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Update(ProcessJob mainProcessJob)
		{
			if (mainProcessJob == null) return;
			using (var db = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntitiesById(db, mainProcessJob.ID);
				if (entity == null) return;
				entity.ApplyMainChanges(mainProcessJob);
				db.SaveChanges();
			}
		}
		#endregion

		#region private
											
	
		private App_Data.MainProcessJob GetEntitiesById(App_Data.ProcessDB db, long id)
		{
			var entity = db.MainProcessJob.Where(e => (e.ID == id)).FirstOrDefault();
			return entity;
		}
		#endregion


		
	}
}
