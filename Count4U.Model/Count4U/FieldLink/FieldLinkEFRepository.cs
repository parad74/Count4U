using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model;
using Count4U.Model.Count4U;
using System.IO;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
	public class FieldLinkEFRepository : BaseEFRepository, IFieldLinkRepository
	{
		public FieldLinkEFRepository(IConnectionDB connection)
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

		public FieldLinks GetFieldLinks(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.FieldLinks.ToList().Select(e => e.ToDomainObject());
				return FieldLinks.FromEnumerable(domainObjects);
			}
		}

		//public FieldLinks GetFieldLinks(string domainType, string pathDB)
		//{
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        var domainObjects = db.FieldLink.Where(e => e.DomainType == domainType).ToList().
		//            Select(e => e.ToDomainObject());
		//        return FieldLinks.FromEnumerable(domainObjects);
		//    }
			//FieldLinks retFieldLinks = new FieldLinks();
			//retFieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueStr1", DomainType = "InventProduct", FieldNameInTable = "IPValueStr1", Editor = "String", InGrid = true, InAdd = true, InEdit = true, NN = 1, NumStringInRecord = 7, PropertyNameInDomainType = "IPValueStr1", TableName = "InventProduct", Validator = "String_100" });
			//retFieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueInt1", DomainType = "InventProduct", FieldNameInTable = "IPValueInt1", Editor = "Int", InGrid = true, InAdd = true, InEdit = true, NN = 2, NumStringInRecord = 8, PropertyNameInDomainType = "IPValueInt1", TableName = "InventProduct", Validator = "Int" });
			//retFieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueFloat1", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat1", Editor = "Float", InGrid = true, InAdd = true, InEdit = true, NN = 3, NumStringInRecord = 9, PropertyNameInDomainType = "IPValueFloat1", TableName = "InventProduct", Validator = "Float" });
			//retFieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueBit1", DomainType = "InventProduct", FieldNameInTable = "IPValueBit1", Editor = "Bool", InGrid = true, InAdd = true, InEdit = true, NN = 4, NumStringInRecord = 10, PropertyNameInDomainType = "IPValueBit1", TableName = "InventProduct", Validator = "Bool" });
			//return retFieldLinks;
		//}

		public FieldLinks GetFieldLinks(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetFieldLinks(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.FieldLinks), db.FieldLinks.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = FieldLinks.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public void Delete(FieldLink fieldLink, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByViewName(db, fieldLink.ViewName);
				if (entity == null) return;
				db.FieldLinks.DeleteObject(entity);
				db.SaveChanges();
			}
		}


		public void DeleteAll(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				db.FieldLinks.ToList().ForEach(e => db.FieldLinks.DeleteObject(e));
				db.SaveChanges();
			}
		}

		public void Insert(FieldLink fieldLink, string pathDB)
		{
			if (fieldLink == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = fieldLink.ToEntity();
				db.FieldLinks.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Update(FieldLink fieldLink, string pathDB)
		{
			if (fieldLink == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByViewName(db, fieldLink.ViewName);
				if (entity == null) return;
				entity.ApplyChanges(fieldLink);
				db.SaveChanges();
			}
		}
 
		public FieldLink GetFieldLinkByViewName(string viewName, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByViewName(db, viewName);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}
	
		public Dictionary<string, string> GetEditorTemplateDictionary(string pathDB)
		{
			Dictionary<string, string> fieldLinkDictionary = new Dictionary<string, string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					FieldLinks fieldLinks = GetFieldLinks(pathDB);
					foreach (var fieldLink in fieldLinks)
					{
						fieldLinkDictionary[fieldLink.ViewName] = fieldLink.EditorTemplate;
					}
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetEditorTemplateDictionary", exp);
				}
			}
			return fieldLinkDictionary;
		}

		#region private

		private App_Data.FieldLink GetEntityByViewName(App_Data.Count4UDB db, string viewName)
		{
			var entity = db.FieldLinks.FirstOrDefault(e => e.ViewName.CompareTo(viewName) == 0);
			return entity;
		}


		#endregion
	}
}
