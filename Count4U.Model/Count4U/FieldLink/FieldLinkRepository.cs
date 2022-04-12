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
	public class FieldLinkRepository : IFieldLinkRepository
	{
		 private FieldLinks _fieldLinks;

		public FieldLinkRepository()
		{
		}

   		public FieldLinks GetFieldLinks(string pathDB)
		{
			if (_fieldLinks == null)
			{
                _fieldLinks = new FieldLinks();
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueStr1", DomainType = "InventProduct", FieldNameInTable = "IPValueStr1", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 1, NumStringInRecord = 7, PropertyNameInDomainType = "IPValueStr1", TableName = "InventProduct", Validator = "String_100" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueStr2", DomainType = "InventProduct", FieldNameInTable = "IPValueStr2", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 2, NumStringInRecord = 8, PropertyNameInDomainType = "IPValueStr2", TableName = "InventProduct", Validator = "String_100" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueFloat1", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat1", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 3, NumStringInRecord = 9, PropertyNameInDomainType = "IPValueFloat1", TableName = "InventProduct", Validator = "Int" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueFloat2", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat2", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 4, NumStringInRecord = 10, PropertyNameInDomainType = "IPValueFloat2", TableName = "InventProduct", Validator = "Float" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueBit1", DomainType = "InventProduct", FieldNameInTable = "IPValueBit1", Editor = "Bool", InGrid = true, InAdd = false, InEdit = false, NN = 5, NumStringInRecord = 11, PropertyNameInDomainType = "IPValueBit1", TableName = "InventProduct", Validator = "Bool" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "QuantityInPackEdit", DomainType = "InventProduct", FieldNameInTable = "QuantityInPackEdit", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 6, NumStringInRecord = 100, PropertyNameInDomainType = "QuantityInPackEdit", TableName = "InventProduct", Validator = "String_100" });
			}
			return this._fieldLinks;
	
		}

		public FieldLinks GetFieldLinks(string domainType, string pathDB)
		{
			if (_fieldLinks == null)
			{
                _fieldLinks = new FieldLinks();
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueStr1", DomainType = "InventProduct", FieldNameInTable = "IPValueStr1", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 1, NumStringInRecord = 7, PropertyNameInDomainType = "IPValueStr1", TableName = "InventProduct", Validator = "String_100" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueStr2", DomainType = "InventProduct", FieldNameInTable = "IPValueStr2", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 2, NumStringInRecord = 8, PropertyNameInDomainType = "IPValueStr2", TableName = "InventProduct", Validator = "String_100" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueFloat1", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat1", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 3, NumStringInRecord = 9, PropertyNameInDomainType = "IPValueFloat1", TableName = "InventProduct", Validator = "Int" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueFloat2", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat2", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 4, NumStringInRecord = 10, PropertyNameInDomainType = "IPValueFloat2", TableName = "InventProduct", Validator = "Float" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueBit1", DomainType = "InventProduct", FieldNameInTable = "IPValueBit1", Editor = "Bool", InGrid = true, InAdd = false, InEdit = false, NN = 5, NumStringInRecord = 11, PropertyNameInDomainType = "IPValueBit1", TableName = "InventProduct", Validator = "Bool" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "QuantityInPackEdit", DomainType = "InventProduct", FieldNameInTable = "QuantityInPackEdit", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 4, NumStringInRecord = 100, PropertyNameInDomainType = "QuantityInPackEdit", TableName = "InventProduct", Validator = "String_100" });
			}
			return this._fieldLinks;
		}

		public FieldLinks GetFieldLinks(SelectParams selectParams, string pathDB)
		{
			if (_fieldLinks == null)
			{
                _fieldLinks = new FieldLinks();
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueStr1", DomainType = "InventProduct", FieldNameInTable = "IPValueStr1", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 1, NumStringInRecord = 7, PropertyNameInDomainType = "IPValueStr1", TableName = "InventProduct", Validator = "String_100" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueStr2", DomainType = "InventProduct", FieldNameInTable = "IPValueStr2", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 2, NumStringInRecord = 8, PropertyNameInDomainType = "IPValueStr2", TableName = "InventProduct", Validator = "String_100" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueFloat1", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat1", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 3, NumStringInRecord = 9, PropertyNameInDomainType = "IPValueFloat1", TableName = "InventProduct", Validator = "Int" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueFloat2", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat2", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 4, NumStringInRecord = 10, PropertyNameInDomainType = "IPValueFloat2", TableName = "InventProduct", Validator = "Float" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "IPValueBit1", DomainType = "InventProduct", FieldNameInTable = "IPValueBit1", Editor = "Bool", InGrid = true, InAdd = false, InEdit = false, NN = 5, NumStringInRecord = 11, PropertyNameInDomainType = "IPValueBit1", TableName = "InventProduct", Validator = "Bool" });
				this._fieldLinks.Add(new FieldLink { DefaultEditorLable = "QuantityInPackEdit", DomainType = "InventProduct", FieldNameInTable = "QuantityInPackEdit", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 6, NumStringInRecord = 100, PropertyNameInDomainType = "QuantityInPackEdit", TableName = "InventProduct", Validator = "String_100" });
			}
			return this._fieldLinks;
		}

		public void Delete(FieldLink fieldLink, string pathDB)
		{
		}

		public void DeleteAll(string pathDB)
		{
		}

		public void Insert(FieldLink fieldLink, string pathDB)
		{
		}

		public void Update(FieldLink fieldLink, string pathDB)
		{
		}

		public FieldLink GetFieldLinkByKeys(string domainType, string propertyNameInDomainType, string pathDB)
		{
			var entity = this.GetEntityByKey(domainType, propertyNameInDomainType);
			if (entity == null) return null;
			return entity;

		}

		public Dictionary<string, FieldLink> GetFieldLinkDictionary(string domainType, string pathDB)
		{
			Dictionary<string, FieldLink> fieldLinkDictionary = new Dictionary<string, FieldLink>();
			try
			{
				FieldLinks fieldLinks = GetFieldLinks(domainType, pathDB);
				fieldLinkDictionary = fieldLinks.Select(e => e).Distinct().ToDictionary(k => k.PropertyNameInDomainType);
			}
			catch (Exception exp)
			{
				//_logger.ErrorException("FillDocumentHeaderDictionary", exp);
			}

			return fieldLinkDictionary;
		}

		public Dictionary<string, int> GetIndexInRecordDictionary(string domainType, string pathDB)
		{
			Dictionary<string, int> fieldLinkDictionary = new Dictionary<string, int>();
			try
			{
				FieldLinks fieldLinks = GetFieldLinks(domainType, pathDB);
				foreach (var fieldLink in fieldLinks)
				{
					fieldLinkDictionary[fieldLink.PropertyNameInDomainType] = fieldLink.NumStringInRecord;
				}
			}
			catch (Exception exp)
			{
				//_logger.ErrorException("GetIndexInRecordDictionary", exp);
			}

			return fieldLinkDictionary;
		}

		#region private

		private FieldLink GetEntityByKey(string domainType, string propertyNameInDomainType)
		{
			var entity = this._fieldLinks	.FirstOrDefault(e => e.DomainType.CompareTo(domainType) == 0
				&& e.PropertyNameInDomainType.CompareTo(propertyNameInDomainType) == 0);
			return entity;
		}


		//private App_Data.FieldLinks GetEntityByDomainType(App_Data.Count4UDB db, string name)
		//{
		//    var entity = db.FieldLink.FirstOrDefault(e => e.Name.CompareTo(name) == 0);
		//    return entity;
		//}

		//public List<string> GetLocationCodeList(string pathDB)
		//{
		//    List<string> ret = new List<string>();
		//    using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        try
		//        {
		//            var entitys = db.FieldLink.Select(e => e.Code).Distinct().ToList();
		//            return entitys;
		//        }
		//        catch (Exception exp)
		//        {
		//            _logger.ErrorException("GetLocationCodeList", exp);
		//        }
		//    }
		//    return ret;
		//}

		#endregion

		

		#region IFieldLinkRepository Members


		public FieldLink GetFieldLinkByViewName(string viewName, string pathDB)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, string> GetEditorTemplateDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IFieldLinkRepository Members

		public IConnectionDB Connection
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
