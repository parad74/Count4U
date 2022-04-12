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

namespace Count4U.Model.Count4U
{
	public class PropertyStrBuildingConfigClalitXslx2SdfParser : IPropertyStrParser
	{
		private readonly ILog _log;
		private Dictionary<string, PropertyStr> _propertyStrDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;

		public PropertyStrBuildingConfigClalitXslx2SdfParser(IServiceLocator serviceLocator,
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

			//Name = Site  		Code = 0
			//Name =  Building	Code = 1
			//Name = Floor		Code = 2
			//Name = Room		Code = 3
				string domainObject = DomainObjectTypeEnum.BuildingConfig.ToString();
				PropertyStr propertyStr0 = new PropertyStr() { Name = "Site", Code = "0", DomainObject = domainObject };
				PropertyStr propertyStr1 = new PropertyStr() { Name = "Building", Code = "1", DomainObject = domainObject };
				PropertyStr propertyStr2 = new PropertyStr() { Name = "Floor", Code = "2", DomainObject = domainObject };
				PropertyStr propertyStr3 = new PropertyStr() { Name = "Room", Code = "3", DomainObject = domainObject };

				this._propertyStrDictionary["0"] = propertyStr0;
				this._propertyStrDictionary["1"] = propertyStr1;
				this._propertyStrDictionary["2"] = propertyStr2;
				this._propertyStrDictionary["3"] = propertyStr3;

			return this._propertyStrDictionary;
		}

	}
}
