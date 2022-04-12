using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U
{
	//из таблицы TemporaryInventory добавляем в   PropertyStr новые записи
	public class PropertyStrNativSqlite2SdfParser : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStrNativSqlite2SdfParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._propertyStrDictionary = new Dictionary<string, PropertyStr>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, PropertyStr> PropertyStrDictionary
		{
			get { return this._propertyStrDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, PropertyStr> GetPropertyStrs(string fromPathFile,
			DomainObjectTypeEnum domainObjectType,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, PropertyStr> propertyStrFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._propertyStrDictionary.Clear();
			this._errorBitList.Clear();

			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);	  //Текущая БД
		
			IPropertyStrRepository propertyStrRepository = this._serviceLocator.GetInstance<IPropertyStrRepository>();
			propertyStrFromDBDictionary.Clear();
			propertyStrFromDBDictionary = propertyStrRepository.GetFullPropertyStrDictionary(toDBPath);     //k => k.DomainObject + "|" + k.PropertyStrCode  PropertyStr1|Dolev

			ITemporaryInventoryRepository temporaryInventoryRepository = this._serviceLocator.GetInstance<ITemporaryInventoryRepository>();
			Dictionary<string, TemporaryInventory> dictionaryTemporaryInsertPropertyStr
			= temporaryInventoryRepository.GetDictionaryTemporaryInventorys(toDBPath, "PropertyStr", "INSERT");

			var propertyStrList = dictionaryTemporaryInsertPropertyStr.Values.Select(x => x).AsEnumerable();

			foreach (TemporaryInventory tempPropertyStr in propertyStrList)
			{
				if (tempPropertyStr == null) continue;
				tempPropertyStr.Domain = tempPropertyStr.Domain.Replace("List", "");
				if (propertyStrFromDBDictionary.ContainsKey(tempPropertyStr.Domain + "|" + tempPropertyStr.NewKey) == true) continue;
			

				String[] recordEmpty = { "", ""};

				PropertyStr newPropertyStr = new PropertyStr();
				PropertyStr propertyStr = new PropertyStr();

				propertyStr.PropertyStrCode = tempPropertyStr.NewKey;
				propertyStr.Name = tempPropertyStr.NewKey;
				propertyStr.DomainObject = tempPropertyStr.Domain;

					int retBit = newPropertyStr.ValidateError(propertyStr);
					if (retBit != 0)  //Error
					{
						this._errorBitList.Add(new BitAndRecord
						{
							Bit = retBit,
							Record = tempPropertyStr.NewKey,
							ErrorType = MessageTypeEnum.Error
						});
						continue;
					}
					else //	Error  retBit == 0 
					{
						retBit = newPropertyStr.ValidateWarning(propertyStr); //Warning
																			  //if (propertyStrFromDBDictionary.ContainsKey(newPropertyStr.PropertyStrCode) == false) // Было
					if (propertyStrFromDBDictionary.ContainsKey(tempPropertyStr.Domain + "|" + tempPropertyStr.NewKey) == false)
					{
							this._propertyStrDictionary[newPropertyStr.PropertyStrCode] = newPropertyStr;
						//propertyStrFromDBDictionary[newPropertyStr.PropertyStrCode] = null;     //	  Было
						propertyStrFromDBDictionary[tempPropertyStr.Domain + "|" + tempPropertyStr.NewKey] = null;     //
					}
						if (retBit != 0)
						{
							this._errorBitList.Add(new BitAndRecord
							{
								Bit = retBit,
								Record = tempPropertyStr.NewKey,
								ErrorType = MessageTypeEnum.WarningParser
							});
						}
					}
			}
	
			return this._propertyStrDictionary;
		}

	}
}
