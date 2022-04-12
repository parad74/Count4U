using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using Count4U.Model.Main.MappingEF;
using Count4U.Model.Interface;
using Count4U.Localization;

namespace Count4U.Model.Main
{
	public class CustomerConfigEFRepository : BaseEFRepository, ICustomerConfigRepository
    {

		Dictionary<CustomerConfigIniEnum, string> _customerConfigDictionary;
		public CustomerConfigEFRepository(IConnectionDB connection)
            : base(connection)
        {

        }
		#region BaseEFRepository Members

		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}

		#endregion

        #region ICustomerConfigRepository Members

		public CustomerConfigs GetCustomerConfigs()
		{
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = AsQueryable(db.CustomerConfig).ToList().Select(e => e.ToDomainObject());
				return CustomerConfigs.FromEnumerable(domainObjects);
			}
		}

		public CustomerConfigs GetCustomerConfigs(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetCustomerConfigs();

			long totalCount = 0;
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				// Получение сущностей и общего количества из БД.
				// Getting entities and total count from database.
				var entities = GetEntities(db, AsQueryable(db.CustomerConfig), db.CustomerConfig.AsQueryable(),
					selectParams, out totalCount);

				// Преобразование сущностей в объекты предметной области.
				// Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToDomainObject());

				// Возврат результата.
				// Returning result.
				var result = CustomerConfigs.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

        public CustomerConfigs GetCustomerConfigsByCode(string сustomerСode)
        {
			if (string.IsNullOrWhiteSpace(сustomerСode) == true) return null;
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = AsQueryable(db.CustomerConfig).Where(
					e => e.CustomerCode.CompareTo(сustomerСode) == 0)
					.ToList().Select(e => e.ToDomainObject());
				return CustomerConfigs.FromEnumerable(domainObjects);
			}
        }

        public CustomerConfig GetCustomerConfig(string сustomerСode, string name)
        {
			if (string.IsNullOrWhiteSpace(сustomerСode) == true) return null;
			if (string.IsNullOrWhiteSpace(name) == true) return null;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = GetEntityByCode(dc, сustomerСode, name);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
        }

		public void Insert(CustomerConfig customerConfig)
        {
			if (customerConfig == null) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = customerConfig.ToEntity();
				if (entity == null) return;
				dc.CustomerConfig.AddObject(entity);
				dc.SaveChanges();
			}
        }

		public void Delete(string сustomerСode, string name)
		{
			if (string.IsNullOrWhiteSpace(сustomerСode) == true) return;
			if (string.IsNullOrWhiteSpace(name) == true) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = GetEntityByCode(dc, сustomerСode, name);
				if (entity == null) return;
				dc.CustomerConfig.DeleteObject(entity);
				dc.SaveChanges();
			}
        }

		public void Delete(string сustomerСode)
		{
			if (string.IsNullOrWhiteSpace(сustomerСode) == true) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entitys = dc.CustomerConfig.Where(e => e.CustomerCode == сustomerСode).ToList();
				if (entitys == null) return;
				entitys.ForEach(e => dc.CustomerConfig.DeleteObject(e));
				dc.SaveChanges();
			}
        }

		public void Update(CustomerConfig customerConfig)
		{
			if (customerConfig == null) return;
			if (string.IsNullOrWhiteSpace(customerConfig.CustomerCode) == true) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, customerConfig.CustomerCode, customerConfig.Name);
				if (entity == null) return;
				entity.ApplyChanges(customerConfig);
				dc.SaveChanges();
			}
		}

        #endregion
		public Dictionary<string, CustomerConfig> 
			GetCustomerConfigIniDictionary(string keyCode)
		{
			Dictionary<string, CustomerConfig> customerConfigIniDictionary =
				new Dictionary<string, CustomerConfig>();
			CustomerConfigs customerConfigs = this.GetCustomerConfigsByCode(keyCode);

			foreach (CustomerConfig customerConfig in customerConfigs)
			{
				customerConfigIniDictionary[customerConfig.Name] = customerConfig;
			}
			//customerConfigIniDictionary = customerConfigs.Select(e => e).Distinct().ToDictionary(k => k.Name);
			return customerConfigIniDictionary;
		}


		public Dictionary<CustomerConfigIniEnum, string>  GetCustomerConfigDefaultValueDictionary()
		{
			if (this._customerConfigDictionary == null)
			{
				this._customerConfigDictionary = new Dictionary<CustomerConfigIniEnum, string>();
			 this._customerConfigDictionary.Add(CustomerConfigIniEnum.Hash, 
				 PropertiesSettings.CustomerConfigHash);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.FileType, 
				 PropertiesSettings.CustomerConfigFileType);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.QType, 
				 PropertiesSettings.CustomerConfigQType);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.Password,
				 PropertiesSettings.CustomerConfigPassword);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.UseAlphaKey, 
				 PropertiesSettings.CustomerConfigUseAlphaKey);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.ClientID, 
				 PropertiesSettings.CustomerConfigClientID);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.NewItem, 
				 PropertiesSettings.CustomerConfigNewItem);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.NewItemBool,
			 PropertiesSettings.CustomerConfigNewItemBool);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.ChangeQuantityType,
			 PropertiesSettings.CustomerConfigChangeQuantityType);	
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.IturNameType,
				PropertiesSettings.CustomerConfigIturNameType);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.IturNamePrefix,
				 PropertiesSettings.CustomerConfigIturNamePrefix);
			//this._customerConfigDictionary.Add(CustomerConfigIniEnum.IturInvertPrefix,
			//	PropertiesSettings.CustomerConfigInvertPrefix);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.HTcalculateLookUp,
				 PropertiesSettings.CustomerConfigHTcalculateLookUp);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.LookUpEXE,
		 PropertiesSettings.MISiDnextDataPath);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.Host1,
		 PropertiesSettings.Host1);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.Host2,
		 PropertiesSettings.Host2);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.Timeout,
	 PropertiesSettings.Timeout);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.Retry,
		 PropertiesSettings.Retry);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.SameBarcodeInLocation,
		 PropertiesSettings.SameBarcodeInLocation);
		  	this._customerConfigDictionary.Add(CustomerConfigIniEnum.AddNewLocation,
		 PropertiesSettings.AddNewLocation);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.DefaultHost,
		 PropertiesSettings.DefaultHost);
				
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.AddExtraInputValueSelectFromBatchListForm,
	 PropertiesSettings.AddExtraInputValueSelectFromBatchListForm);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.AllowNewValueFromBatchListForm,
PropertiesSettings.AllowNewValueFromBatchListForm);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.SearchIfExistInBatchList,
PropertiesSettings.SearchIfExistInBatchList);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.AllowMinusQuantity,
PropertiesSettings.AllowMinusQuantity);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.FractionCalculate,
PropertiesSettings.FractionCalculate);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.PartialQuantity,
PropertiesSettings.PartialQuantity);

			this._customerConfigDictionary.Add(CustomerConfigIniEnum.AllowZeroQuantity,
		 PropertiesSettings.AllowZeroQuantity);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.MaxQuantity,
		 PropertiesSettings.MaxQuantity);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.LastSync,
		 PropertiesSettings.LastSync);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.SearchDef,
		 PropertiesSettings.SearchDef);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.MaxLen,
				   PropertiesSettings.MaxLen);
		 this._customerConfigDictionary.Add(CustomerConfigIniEnum.ConfirmNewLocation,
			 PropertiesSettings.ConfirmNewLocation);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.ConfirmNewItem,
				 PropertiesSettings.ConfirmNewItem);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.AutoSendData,
				   PropertiesSettings.AutoSendData);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.AllowQuantityFraction,
				   PropertiesSettings.AllowQuantityFraction);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.AddExtraInputValue,
				   PropertiesSettings.AddExtraInputValue);
			this._customerConfigDictionary.Add(CustomerConfigIniEnum.AddExtraInputValueHeaderName,
				   PropertiesSettings.AddExtraInputValueHeaderName);
					
		}
			return this._customerConfigDictionary;
		}

		public void InsertCustomerConfigIniDictionary(string customerCode, 
			Dictionary<string, CustomerConfig> customerConfigIniDictionary)
		{
			if (string.IsNullOrWhiteSpace(customerCode) == true) return;
			this.Delete(customerCode);
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (KeyValuePair<string, CustomerConfig> keyValuePair in
						customerConfigIniDictionary)
				{
					CustomerConfig val = keyValuePair.Value;
					if (val != null)
					{
						val.CustomerCode = customerCode;
						var entity = val.ToEntity();
						dc.CustomerConfig.AddObject(entity);
						dc.SaveChanges();
					}
				}
			}
		}

		public void UpdateCustomerConfigIniDictionary(
			Dictionary<string, CustomerConfig> customerConfigIniDictionary)
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (KeyValuePair<string, CustomerConfig> keyValuePair in
						customerConfigIniDictionary)
				{
					CustomerConfig val = keyValuePair.Value;
					var entity = this.GetEntityByCode(dc, val.CustomerCode, val.Name);
					if (entity == null) return;
					entity.ApplyChanges(val);
					dc.SaveChanges();
				}
			}
		}
	

		private App_Data.CustomerConfig GetEntityByCode(App_Data.MainDB db, string customerCode,
			string name)
		{
			var entity = db.CustomerConfig.FirstOrDefault(e => (e.CustomerCode.CompareTo(customerCode) == 0)
				&& (e.Name.CompareTo(name) == 0));
			return entity;
		}
    }
}
