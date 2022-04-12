using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Count4U.Configuration.Interfaces;

namespace Count4U.Configuration.Dynamic
{
    public class PropertyLinkRepository : IPropertyLinkRepository
	{
		 private List<PropertyLink> _propertyLinks;

		public PropertyLinkRepository()
		{
		}

        public List<PropertyLink> GetPropertyLinks(string filePath)
		{
			if (_propertyLinks == null)
			{
                _propertyLinks = new List<PropertyLink>();
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueStr1", DomainType = "InventProduct", FieldNameInTable = "IPValueStr1", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 8, NumStringInRecord = 7, PropertyNameInDomainType = "IPValueStr1", TableName = "InventProduct", Validator = "String_100" });
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueStr2", DomainType = "InventProduct", FieldNameInTable = "IPValueStr2", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 9, NumStringInRecord = 8, PropertyNameInDomainType = "IPValueStr2", TableName = "InventProduct", Validator = "String_100" });
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueFloat1", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat1", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 10, NumStringInRecord = 9, PropertyNameInDomainType = "IPValueFloat1", TableName = "InventProduct", Validator = "Int" });
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueFloat2", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat2", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 11, NumStringInRecord = 10, PropertyNameInDomainType = "IPValueFloat2", TableName = "InventProduct", Validator = "Float" });
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueBit1", DomainType = "InventProduct", FieldNameInTable = "IPValueBit1", Editor = "Bool", InGrid = true, InAdd = false, InEdit = false, NN = 12, NumStringInRecord = 11, PropertyNameInDomainType = "IPValueBit1", TableName = "InventProduct", Validator = "Bool" });
				this._propertyLinks.Add(new PropertyLink { DefaultEditorLabel = "Quantity Partial",  NN = 3, PropertyNameInDomainType = "QuantityInPackEdit",  });
			}
			return this._propertyLinks;
	
		}

        public List<PropertyLink> GetPropertyLinks(string domainType, string filePath)
		{
			if (_propertyLinks == null)
			{
                _propertyLinks = new List<PropertyLink>();
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueStr1", DomainType = "InventProduct", FieldNameInTable = "IPValueStr1", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 8, NumStringInRecord = 7, PropertyNameInDomainType = "IPValueStr1", TableName = "InventProduct", Validator = "String_100" });
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueStr2", DomainType = "InventProduct", FieldNameInTable = "IPValueStr2", Editor = "String", InGrid = true, InAdd = false, InEdit = false, NN = 9, NumStringInRecord = 8, PropertyNameInDomainType = "IPValueStr2", TableName = "InventProduct", Validator = "String_100" });
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueFloat1", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat1", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 10, NumStringInRecord = 9, PropertyNameInDomainType = "IPValueFloat1", TableName = "InventProduct", Validator = "Int" });
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueFloat2", DomainType = "InventProduct", FieldNameInTable = "IPValueFloat2", Editor = "Float", InGrid = true, InAdd = false, InEdit = false, NN = 11, NumStringInRecord = 10, PropertyNameInDomainType = "IPValueFloat2", TableName = "InventProduct", Validator = "Float" });
				//this._propertyLinks.Add(new PropertyLink { DefaultEditorLable = "IPValueBit1", DomainType = "InventProduct", FieldNameInTable = "IPValueBit1", Editor = "Bool", InGrid = true, InAdd = false, InEdit = false, NN = 12, NumStringInRecord = 11, PropertyNameInDomainType = "IPValueBit1", TableName = "InventProduct", Validator = "Bool" });
                this._propertyLinks.Add(new PropertyLink { DefaultEditorLabel = "Quantity Partial",  NN = 3, PropertyNameInDomainType = "QuantityInPackEdit" });
			}
			return this._propertyLinks;
		}




//		public PropertyLink GetPropertyLinksByKeys(string domainType, string propertyNameInDomainType, string filePath)
//		{
//			var entity = this.GetEntityByKey(domainType, propertyNameInDomainType);
//			if (entity == null) return null;
//			return entity;
//
//		}

		public Dictionary<string, PropertyLink> GetPropertyLinksDictionary(string domainType, string filePath)
		{
			Dictionary<string, PropertyLink> fieldLinkDictionary = new Dictionary<string, PropertyLink>();
            List<PropertyLink> propertyLinks = GetPropertyLinks(domainType, filePath);
				fieldLinkDictionary = propertyLinks.Select(e => e).Distinct().ToDictionary(k => k.PropertyNameInDomainType);
			
			return fieldLinkDictionary;
		}

		
		#region private

//		private PropertyLink GetEntityByKey(string domainType, string propertyNameInDomainType)
//		{
//			var entity = this._propertyLinks	.FirstOrDefault(e => e.DomainType.CompareTo(domainType) == 0
//				&& e.PropertyNameInDomainType.CompareTo(propertyNameInDomainType) == 0);
//			return entity;
//		}
	

		#endregion

	
	}
}
