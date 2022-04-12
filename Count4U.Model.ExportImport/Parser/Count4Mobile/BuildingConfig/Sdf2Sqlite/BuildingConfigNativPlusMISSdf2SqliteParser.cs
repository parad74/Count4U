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
	public class BuildingConfigNativPlusMISSdf2SqliteParser : IBuildingConfigParser
	{
		private readonly ILog _log;
		private Dictionary<string, BuildingConfig> _buildingConfigDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		private IPropertyStrRepository _propertyStrRepository;

		public BuildingConfigNativPlusMISSdf2SqliteParser(IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._log = log;
			this._buildingConfigDictionary = new Dictionary<string, BuildingConfig>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, BuildingConfig> BuildingConfigDictionary
		{
			get { return this._buildingConfigDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

	   	public Dictionary<string, BuildingConfig> GetBuildingConfigs(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, string> buildingConfigFromDBDictionary,
			DomainObjectTypeEnum domainObjectType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
	
				BuildingConfig newBuildingConfig = new BuildingConfig();
				//propertyStr.Name = recordEmpty[0];	 //	 Name
				//propertyStr.TypeCode = recordEmpty[1];	 //	 Description
				//propertyStr.PropertyStrCode = recordEmpty[3];	 //	 Code
				//propertyStr.Code = recordEmpty[2];	 //	 Ord
				//propertyStr.DomainObject = DomainObjectTypeEnum.BuildingConfig.ToString();


				//Name				0
				//Description		1
				//Ord					2
				//Code					3
				// NameEn			4
				// NameHe			5
				newBuildingConfig.Name = "IturCode";
				newBuildingConfig.Ord = 0;
				newBuildingConfig.NameEn = "Shelf Code";
				newBuildingConfig.NameHe = "קוד איתור";

				newBuildingConfig.Uid = Guid.NewGuid().ToString();

				if (_buildingConfigDictionary.ContainsKey(newBuildingConfig.Name) == false)
				{
					this._buildingConfigDictionary[newBuildingConfig.Name] = newBuildingConfig;
					buildingConfigFromDBDictionary[newBuildingConfig.Name] = null;
				}

			return this._buildingConfigDictionary;

		}

	}
}
