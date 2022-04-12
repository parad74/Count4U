using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStrListMerkavaSdf2SqliteParser : IPropertyStrListSQLiteParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr1> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		private IPropertyStrRepository _propertyStrRepository;

		public PropertyStrListMerkavaSdf2SqliteParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._propertyStrDictionary = new Dictionary<string, PropertyStr1>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		//public Dictionary<string, PropertyStr1> PropertyStrDictionary
		//{
		//	get { return this._propertyStrDictionary; }
		//}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}


		public Dictionary<string, PropertyStr1> GetPropertyStrList(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> propertyStrFromDBDictionary,
			DomainObjectTypeEnum domainObjectType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._propertyStrDictionary.Clear();
			this._propertyStrRepository = this._serviceLocator.GetInstance<IPropertyStrRepository>();
			foreach (PropertyStr propertyStr in this._propertyStrRepository.GetPropertyStrs(domainObjectType.ToString(), fromPathFile))
			{
				if (propertyStr == null) continue;

				PropertyStr1 newPropertyStr = new PropertyStr1();
				//	PropStrCode		
				//PropStrName	 

				newPropertyStr.PropStr1Code = propertyStr.PropertyStrCode;
				newPropertyStr.PropStr1Name = propertyStr.Name;
				newPropertyStr.Uid = Guid.NewGuid().ToString();

				if (propertyStrFromDBDictionary.ContainsKey(newPropertyStr.PropStr1Code) == false)
				{
					this._propertyStrDictionary[newPropertyStr.PropStr1Code] = newPropertyStr;
					propertyStrFromDBDictionary[newPropertyStr.PropStr1Code] = null;
				}
			}
	
			return this._propertyStrDictionary;
		}

	}
}
