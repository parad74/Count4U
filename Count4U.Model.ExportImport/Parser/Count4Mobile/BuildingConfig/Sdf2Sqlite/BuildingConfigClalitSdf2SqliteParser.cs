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
	public class BuildingConfigClalitSdf2SqliteParser : IBuildingConfigParser
	{
		private readonly ILog _log;
		private Dictionary<string, BuildingConfig> _buildingConfigDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		private IPropertyStrRepository _propertyStrRepository;

		public BuildingConfigClalitSdf2SqliteParser(IServiceLocator serviceLocator,
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
			this._propertyStrRepository = this._serviceLocator.GetInstance<IPropertyStrRepository>();
			foreach (PropertyStr propertyStr in this._propertyStrRepository.GetPropertyStrs(DomainObjectTypeEnum.BuildingConfig.ToString(), 
				fromPathFile))
			{
				if (propertyStr == null) continue;

				BuildingConfig newBuildingConfig = new BuildingConfig();
				//propertyStr.Name = recordEmpty[0];	 //	 Name
				//propertyStr.TypeCode = recordEmpty[1];	 //	
				
							//string nameEn = "En=" + recordEmpty[4].CutLength(20);
							//string nameHe = "He=" + recordEmpty[5].CutLength(20);
							//String[] names = new String[] { nameEn, nameHe };
							//string nameTranslate = names.JoinRecord("^");		  // TypeCode

				//propertyStr.PropertyStrCode = recordEmpty[3];	 //	 Code
				//propertyStr.Code = recordEmpty[2];	 //	 Ord
				//propertyStr.DomainObject = DomainObjectTypeEnum.BuildingConfig.ToString();

				newBuildingConfig.Name = propertyStr.Name;
				int ord = 0;
				bool ret = Int32.TryParse(propertyStr.Code, out ord);
				newBuildingConfig.Ord = ord;
		
				//string nameTranslate = propertyStr.TypeCode;
				//String[] names = nameTranslate.Split("^".ToCharArray());
				//string nameEn = "";
				//string nameHe = "";
				//if (names.Length >= 1) nameEn = names[0];
				//if (names.Length >= 2) nameHe = names[1];			   //String[] { nameEn, nameHe };
				//nameHe = nameHe.Replace("He=", "");
				//nameEn = nameEn.Replace("En=", "");
				//newBuildingConfig.NameEn = nameEn;
				//newBuildingConfig.NameHe = nameHe;
				//newBuildingConfig.Code = propertyStr.PropertyStrCode;
				//newBuildingConfig.Description = propertyStr.TypeCode;
				newBuildingConfig.Uid = Guid.NewGuid().ToString();

				if (_buildingConfigDictionary.ContainsKey(newBuildingConfig.Name) == false)
				{
					this._buildingConfigDictionary[newBuildingConfig.Name] = newBuildingConfig;
					buildingConfigFromDBDictionary[newBuildingConfig.Name] = null;
				}
			}

			return this._buildingConfigDictionary;

		}

	}
}
