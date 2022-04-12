using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using Count4U.Model.Audit.MappingEF;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.ProcessC4U.MappingEF;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using System.IO;
using System.Security.AccessControl;
using Count4U.Model.Count4U;
using NLog;
using System.Text;
using Count4U.Model.Interface.ProcessC4U;

namespace Count4U.Model.ProcessC4U
{
	public class PortEFRepository : BaseEFRepository, IPortRepository
	{

		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public PortEFRepository(IConnectionDB connection)
			: base(connection)
		{
		}

		public IConnectionDB Connection
		{
			get { return this._connection; }
			set { this._connection = value; }
		}

		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}
	
		public Ports GetPorts()
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var domainObjects = dc.Port.ToList().Select(e => e.ToDomainObject());
				return Ports.FromEnumerable(domainObjects);
			}
		}
  
		public Ports GetPorts(SelectParams selectParams)
		{
			if (selectParams == null)
				return this.GetPorts();

			long totalCount = 0;
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entities = this.GetEntities(dc, AsQueryable(dc.Port), dc.Port.AsQueryable(), selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				Ports result = Ports.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}


		public Port GetPortByCode(string portCode)
		{
			if (string.IsNullOrEmpty(portCode) == true) return null;

			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, portCode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}


		//public void SetCurrent(Port currentInventor, AuditConfig auditConfig)
		//{
		//	if (auditConfig == null) return;
		//	if (currentInventor == null)
		//	{
		//		auditConfig.ClearInventor();
		//	}
		//	else
		//	{
		//		if (auditConfig.InventorCode != currentInventor.Code)
		//		{
		//			auditConfig.ClearInventor();
		//			auditConfig.InventorCode = currentInventor.Code;
		//			auditConfig.InventorName = currentInventor.Name;
		//			auditConfig.InventorDate = currentInventor.InventorDate;
		//		}
		//	}
		//}

		//public Port GetCurrent(AuditConfig auditConfig)
		//{
		//	if (auditConfig == null) return null;
		//	return this.GetInventorByCode(auditConfig.InventorCode);
		//}



		//public Ports GetInventorsByStatus(string statusCode)
		//{
		//	if (string.IsNullOrEmpty(statusCode) == true) return null;
		//	using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString))
		//	{
		//		var domainObjects = dc.Port.Where(e => e.StatusInventorCode == statusCode)
		//											 .ToList().Select(e => e.ToDomainObject());
		//		return Ports.FromEnumerable(domainObjects);
		//	}
		//}

		//public Ports GetInventorsByStatusCode(string statusCode)
		//{
		//	if (string.IsNullOrEmpty(statusCode) == true) return null;
		//	using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString))
		//	{
		//		var domainObjects = dc.Port.Where(e => e.StatusInventorCode == statusCode)
		//											 .ToList().Select(e => e.ToDomainObject());
		//		return Ports.FromEnumerable(domainObjects);
		//	}
		//}

		public List<string> GetCodeList()
		{
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = dc.Port.Select(e => e.PortCode).Distinct().ToList();
				return entity;
			}
		}


		public void Delete(Port port, bool full = true)
		{
			if (port == null) return;
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, port.PortCode);
				if (entity == null) return;
				dc.Port.DeleteObject(entity);
				dc.SaveChanges();
			}
		}

		public void Delete(string code)
		{
			if (string.IsNullOrEmpty(code) == true) return;
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, code);
				if (entity == null) return;
				dc.Port.DeleteObject(entity);
				dc.SaveChanges();
			}
		}


		public void Insert(Ports ports)
		{
			if (ports == null) return;
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				foreach (Port port in ports)
				{
					if (port == null) { continue; }
					App_Data.Port entity = GetEntityByCode(dc, port.PortCode);
					// если нет объекта с таким кодом, только тогда добавляем
					if (entity == null)
					{
						entity = port.ToEntity();
						dc.Port.AddObject(entity);
					}
				}
				dc.SaveChanges();
			}
		}



		public void Update(Port port)
		{
			if (port == null) return;
			using (App_Data.ProcessDB dc = new App_Data.ProcessDB(this.ProcessDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, port.PortCode);
				if (entity == null) return;
				entity.ApplyChanges(port);
				dc.SaveChanges();

				//var entityPort = dc.Port.FirstOrDefault(e => e.PortCode.CompareTo(port.PortCode) == 0);
				//if (entityPort == null) return;
				//entityPort.Name = entity.Name;
				//entityPort.Description = entityPort.Description;
				//dc.SaveChanges();
			}

		}


		private App_Data.Port GetEntityByCode(App_Data.ProcessDB dc, string portCode)
		{
			var entity = dc.Port.FirstOrDefault(e => e.PortCode.CompareTo(portCode) == 0);
			return entity;
		}
	}
}


