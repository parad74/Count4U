using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using Count4U.Model.Main.MappingEF;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;

namespace Count4U.Model.Main
{
	public class ImportAdapterEFRepository : BaseEFRepository, IImportAdapterRepository
	{
		public ImportAdapterEFRepository(IConnectionDB connection)
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
	
		public ImportAdapters GetImportAdapters()
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = dc.ImportAdapter.ToList().Select(e => e.ToDomainObject());
				return ImportAdapters.FromEnumerable(domainObjects);
			}
		}

		public ImportAdapters GetImportAdapters(SelectParams selectParams)
		{
			if (selectParams == null)
				return GetImportAdapters();

			long totalCount = 0;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				// Получение сущностей и общего количества из БД.
				// Getting entities and total count from database.
				var entities = GetEntities(dc, AsQueryable(dc.ImportAdapter), 
					dc.ImportAdapter.AsQueryable(), selectParams, out totalCount);

				// Преобразование сущностей в объекты предметной области.
				// Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToDomainObject());

				// Возврат результата.
				// Returning result.
				ImportAdapters result = ImportAdapters.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}


		}

		public ImportAdapters GetAllowedImportAdapters(string customerCode, string branchCode,
			string inventorCode, ImportDomainEnum importDomainType)
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var adapters = new ImportAdapters();
				string any = ImportDomainEnum.Any.ToString();
				string importType = importDomainType.ToString();
				var domainObjects = dc.ImportAdapter.Where(e => e.Code == any
					&& e.DomainType == importType)
					.ToList().Select(e => e.ToDomainObject());
				ImportAdapters.Fill(adapters, domainObjects);

				if (string.IsNullOrWhiteSpace(customerCode) == false)
				{
					domainObjects = dc.ImportAdapter.Where(e => e.Code == customerCode
						&& e.DomainType == importType)
						.ToList().Select(e => e.ToDomainObject());
					ImportAdapters.Fill(adapters, domainObjects);
				}
				if (string.IsNullOrWhiteSpace(branchCode) == false)
				{
					domainObjects = dc.ImportAdapter.Where(e => e.Code == branchCode
						&& e.DomainType == importType)
						.ToList().Select(e => e.ToDomainObject());
					ImportAdapters.Fill(adapters, domainObjects);
				}
				if (string.IsNullOrWhiteSpace(inventorCode) == false)
				{
					domainObjects = dc.ImportAdapter.Where(e => e.Code == inventorCode
								&& e.DomainType == importType)
							  .ToList().Select(e => e.ToDomainObject());
					ImportAdapters.Fill(adapters, domainObjects);
				}
				return adapters;
			}
		}

		public void Delete(long id)
		{
			if (id == null) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = dc.ImportAdapter.FirstOrDefault(e => e.ID.CompareTo(id) == 0);
				if (entity == null) return;
				dc.ImportAdapter.DeleteObject(entity);
				dc.SaveChanges();
			}
		}

		private List<App_Data.ImportAdapter> GetEntitiesByCode(App_Data.MainDB db, string code)
		{
			var entitis = db.ImportAdapter.Where(e => (e.Code == code)).ToList();
			return entitis;
		}

		private List<App_Data.ImportAdapter> GetEntitiesByCodes(App_Data.MainDB db, string code1,
			string code2, string code3)
		{
			var entitis = db.ImportAdapter.Where(e => (e.Code == code1 || e.Code == code2 || e.Code == code3)).ToList();
			return entitis;
		}

		private List<App_Data.ImportAdapter> GetEntitiesByCodes(App_Data.MainDB db, string code1,
		string code2)
		{
			var entitis = db.ImportAdapter.Where(e => (e.Code == code1 || e.Code == code2 )).ToList();
			return entitis;
		}

		private List<App_Data.ImportAdapter> GetEntitiesByEmptyCode(App_Data.MainDB db)
		{
			var entitis = db.ImportAdapter.Where(e => (string.IsNullOrWhiteSpace(e.Code) == true)).ToList();
			return entitis;
		}
	}

	
}
