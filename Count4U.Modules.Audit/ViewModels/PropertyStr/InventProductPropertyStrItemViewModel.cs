using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.ViewModel;
using System;
using Count4U.Common.Interfaces;
using System.ComponentModel;
using Count4U.Model;

namespace Count4U.Modules.Audit.ViewModels
{
	public class InventProductPropertyStrItemViewModel : NotificationObject, IDataErrorInfo, IRowEdititing
	{
		private InventProduct _inventProduct;
		private PropertyStr _propertyStr;
		private PropertyStr _backup;
		private string _title;
		private string _propertyStrCode;
		private string _propertyStrName;
		private string _typeCode;
		private string _code;
		private string _domainObject;

		//public InventProductPropertyStrItemViewModel(PropertyStr propertyStr, InventProduct inventProduct)
		//{
			//this._inventProduct = inventProduct;
			//this._propertyStr = propertyStr;
			//if (_propertyStr != null)
			//{
			//	this._title = this._propertyStr.DomainObject; //TODO
			//	this._propertyStrCode = this._propertyStr.PropertyStrCode;
			//	this._propertyStrName = this._propertyStr.Name;
			//	this._typeCode = this._propertyStr.TypeCode;
			//	this._code = this._propertyStr.Code;
			//	this._domainObject = this._propertyStr.DomainObject;
			//}
		//	InventProductSet(propertyStr, inventProduct);
		//}

		public InventProductPropertyStrItemViewModel(string domainObjectType, string title, string propertyStrCode, string propertyStrName,
			PropertyStr propertyStr, InventProduct inventProduct)
		{
			//this._inventProduct = inventProduct;
			//this._title = title;
			//this._propertyStrCode = propertyStrCode;
			//this._propertyStrName = propertyStrName;	
			InventProductSet(domainObjectType, title, propertyStrCode, propertyStrName, propertyStr, inventProduct);
		}


		public string PropertyStrCode
		{
			get { return this._propertyStrCode; }
			set
			{
				this._propertyStrCode = value;
				this.RaisePropertyChanged(() => PropertyStrCode);
			}
		}

		public string DomainObject
		{
			get { return this._domainObject; }
		}

		public string TypeCode
		{
			get { return this._typeCode; }
		}

		public string Code
		{
			get { return this._code; }
		}

		public string PropertyStrName
		{
			get { return this._propertyStrName; }
			set
			{
				this._propertyStrName = value;
				this.RaisePropertyChanged(() => PropertyStrName);
			}
		}

		public string Title
		{
			get { return this._title; }
			set
			{
				this._title = value;
				this.RaisePropertyChanged(() => Title);
			}
		}

		public InventProduct InventProduct
		{
			get { return this._inventProduct; }
		}

		//public PropertyStr PropertyStr
		//{
		//	get { return this._propertyStr; }
		//}

		public void UpdateUIAfterDbSave()
		{
			RaisePropertyChanged(() => PropertyStrCode);			   //TODO
			RaisePropertyChanged(() => PropertyStrName);	
		}


		public void InventProductUpdate()
		{
			if (string.IsNullOrWhiteSpace(this._domainObject) == true) return;
			DomainObjectTypeEnum domain = DomainObjectTypeEnum.Unknown;
			bool ret = Enum.TryParse<DomainObjectTypeEnum>(this._domainObject, out domain);
			switch (domain)
			{
				case DomainObjectTypeEnum.PropertyStr1:
					this._inventProduct.IPValueStr1 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr2:
					this._inventProduct.IPValueStr2 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr3:
					this._inventProduct.IPValueStr3 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr4:
					this._inventProduct.IPValueStr4 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr5:
					this._inventProduct.IPValueStr5 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr6:
					this._inventProduct.IPValueStr6 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr7:
					this._inventProduct.IPValueStr7 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr8:
					this._inventProduct.IPValueStr8 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr9:
					this._inventProduct.IPValueStr9 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr10:
					this._inventProduct.IPValueStr10 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr11:
					this._inventProduct.IPValueStr11 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr12:
					this._inventProduct.IPValueStr12 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr13:
					this._inventProduct.IPValueStr13 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr14:
					this._inventProduct.IPValueStr14 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr15:
					this._inventProduct.IPValueStr15 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr16:
					this._inventProduct.IPValueStr16 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr17:
					this._inventProduct.IPValueStr17 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr18:
					this._inventProduct.IPValueStr18 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr19:
					this._inventProduct.IPValueStr19 = this._propertyStrCode;
					break;
				case DomainObjectTypeEnum.PropertyStr20:
					this._inventProduct.IPValueStr20 = this._propertyStrCode;
					break;
		
			}
			

		}

		public void InventProductSet(string domainObjectType, string title, string propertyStrCode, string propertyStrName, 
			PropertyStr propertyStr, InventProduct inventProduct)
		{
			this._inventProduct = inventProduct;
			this._propertyStr = propertyStr;
			this._domainObject = domainObjectType;
			this._title = title;
			this._propertyStrCode = propertyStrCode;
			this._propertyStrName = propertyStrName;

			this.RaisePropertyChanged(() => this.PropertyStrCode);
			this.RaisePropertyChanged(() => this.DomainObject);
			//	this.RaisePropertyChanged(() => this.TypeCode);
			this.RaisePropertyChanged(() => this.PropertyStrName);
			//	this.RaisePropertyChanged(() => this.Code);
			this.RaisePropertyChanged(() => this.Title);
		}

		//public void InventProductSet(PropertyStr propertyStr, InventProduct inventProduct)
		//{
		//	this._inventProduct = inventProduct;
		//	this._propertyStr = propertyStr;
		//	if (_propertyStr != null)
		//	{
		//		//this._title = this._propertyStr.DomainObject; //TODO
		//		this._propertyStrCode = this._propertyStr.PropertyStrCode;
		//		this._propertyStrName = this._propertyStr.Name;
		//		this._typeCode = this._propertyStr.TypeCode;
		//		this._code = this._propertyStr.Code;
		//		this._domainObject = this._propertyStr.DomainObject;
		//	}
		//	//this.RaisePropertyChanged(() => this.Title);
		//	this.RaisePropertyChanged(() => this.PropertyStrCode);
		//	this.RaisePropertyChanged(() => this.PropertyStrName);
		//	this.RaisePropertyChanged(() => this.TypeCode);
		//	this.RaisePropertyChanged(() => this.Code);
		//	this.RaisePropertyChanged(() => this.DomainObject);
		//}

		#region IRowEdititing Members

		public void BeginEditing()
		{
			this.RaisePropertyChanged(() => this.PropertyStrCode);
			this._backup = new PropertyStr();
			this._backup.PropertyStrCode = this.PropertyStrCode;
		}

		public void CancelEditing()
		{
			if (_backup == null) return;

			this._propertyStrCode = _backup.PropertyStrCode;
		}

		public void CommitEditing()
		{
		//	this._propertyStr.PropertyStrCode = PropertyStrCode;
			this._propertyStrCode = PropertyStrCode;
			RaisePropertyChanged(() => PropertyStrCode);
		}

		#endregion

		#region IDataErrorInfo Members

		public string this[string propertyName]
		{
			get
			{
				switch (propertyName)
				{
					case "PropertyStrCode":
						{
							//                            string validate = InventProductValidate.QuantityEditValidate(this._quantityEdit.ToString());
							//                            if (!String.IsNullOrWhiteSpace(validate))
							//                                return validate;
							//
							//                            if (_quantityEdit <= 0)
							//                                return Localization.Resources.ViewModel_InventProductItem_QuantityEdit;

							return String.Empty;
						}
				}
				return String.Empty;
			}
		}

		public string Error
		{
			get { return String.Empty; }
		}

		#endregion
	}
}