using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;
														
namespace Count4U.Model.Count4U
{	  //на первом шаге парсим ImportInfrastructure.Xslx и записываем Count4U.Itur 
	//здесь добавлям недостающие - пропущенные уровни и проверяем NodeType - для androida
	public class IturNativXslx2SdfUpdateUpLevelParser : IIturParser
	{
		private readonly ILog _log;
		private Dictionary<string, Itur> _iturDictionary;		  //результат 
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;
		public IServiceLocator _serviceLocator;
		private List<Location> _locationToDBList;
		private IIturRepository _iturRepository;

		public IturNativXslx2SdfUpdateUpLevelParser(
			IIturRepository iturRepository,
			IServiceLocator serviceLocator,
			ILog log)
		{
			this._serviceLocator = serviceLocator;
			this._iturRepository = iturRepository;
			this._log = log;
			this._iturDictionary = new Dictionary<string, Itur>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public Dictionary<string, Itur> IturDictionary
		{
			get { return this._iturDictionary; }
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}

		public List<Location> LocationToDBList
		{
			get { return this._locationToDBList; }
		}

		/// <summary>
		/// Получение списка Product 
		/// </summary>
		/// <returns></returns>
		//public Dictionary<string, Itur> GetIturs(string fromPathFile,
		public IEnumerable<Itur> GetItursEnumerable(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString,
			Dictionary<string, Itur> iturFromDBDictionary,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			this._iturDictionary.Clear();
			this._errorBitList.Clear();

			string toDBPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);	  //Текущая БД

			//IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = this._iturRepository.GetERPIturDictionary(toDBPath);
			}	catch{}

			//
			//Iturs itursFromDB = this._iturRepository.GetIturs(fromPathFile);
			//foreach (var iturFromDB in itursFromDB)
			//{
			//	this._iturDictionary[iturFromDB.IturCode] = iturFromDB;
			//}

			////this._iturDictionary =  this._iturRepository.GetIturDictionary(fromPathFile, true);
			//return this._iturDictionary;
			

			ILocationRepository  locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();
			//Dictionary<string, Location> dictionaryLocation = locationRepositorym.GetLocationDictionary(toDBPath, true);
			List<string> locations = locationRepository.GetLocationCodeList(toDBPath);
			Dictionary<string, int> dictionaryPrffixIndex = new Dictionary<string, int>();
			foreach(string code in locations)
			{
				dictionaryPrffixIndex[code] = 1;
				int maxNumber = this._iturRepository.GetMaxNumber(code, toDBPath);
				if (maxNumber > 1) dictionaryPrffixIndex[code] = maxNumber;

			}

			//==========================================================
			Iturs itursFromDB = this._iturRepository.GetIturs(toDBPath);
			foreach (var iturFromDB in itursFromDB)
			{
				string iturCodeERP = iturFromDB.ERPIturCode;				  //base

				if (string.IsNullOrWhiteSpace(iturCodeERP) == true) continue;

				string[] locationCodes = iturCodeERP.Split('-');
			
				if (locationCodes.Length  == 0)		continue;
	
				int deepNode = locationCodes.Length;

				string locationCode = this.GetLocationCode(locationCodes);								//base
  				if (string.IsNullOrWhiteSpace(locationCode) == true) continue;
						//0 = Terminal Node & Container (for InventProduct)
						//1 = Not Terminal Node & Not Container (for InventProduct)
						//2 = Not Terminal node & Container (for InventProduct)
						//iturFromDB.Disabled = false;//Contaner изначально предполагаем что все  контейнеры
						iturFromDB.NodeType = 0;  //0 = Terminal Node  - изначально предполагаем что все терминальный
						
						if (deepNode == 1)		//root 
						{
						}
						if (deepNode == 2)			//root +1
						{
							iturFromDB.ParentIturCode = locationCodes[0];// newItur.Level1;
						}
						if (deepNode == 3)		  //root +2
						{
							iturFromDB.ParentIturCode = locationCodes[0] + "-" + locationCodes[1];//newItur.Level1 + "-" + newItur.Level2;
						}
						if (deepNode == 4)		  //root +3
						{
							iturFromDB.ParentIturCode = locationCodes[0] + "-" + locationCodes[1] + "-" + locationCodes[2];//newItur.Level1 + "-" + newItur.Level2;
						}

						this._iturDictionary[iturFromDB.IturCode] = iturFromDB;
						if (dictionaryIturCodeERP.ContainsKey(iturFromDB.ERPIturCode) == true)
						{
							dictionaryIturCodeERP[iturFromDB.ERPIturCode] = iturFromDB;
						}
						//this._iturDictionary.AddToDictionary(newItur.Code, newItur, record.JoinRecord(separator), Log);
					//}
				//yield return newItur;
			}


			Dictionary<string, Itur> tempDictionryAddedItur = new Dictionary<string, Itur>();

			foreach (KeyValuePair<string, Itur> keyValueItur in dictionaryIturCodeERP)	   // источник Count4udb Itur
			{
				Itur itur = keyValueItur.Value;
				if (itur == null) continue;

				if (string.IsNullOrWhiteSpace(itur.ParentIturCode) == false)
				{
					if (dictionaryIturCodeERP.ContainsKey(itur.ParentIturCode) == true)		 // источник Count4udb Itur
					{																								//только если ключ в словаре ERPIturCode
						Itur parentItur = dictionaryIturCodeERP[itur.ParentIturCode];			 //Вытаскиваем родительский и eго меняем
						if (parentItur != null)
						{
							if (this._iturDictionary.ContainsKey(parentItur.IturCode) == true)
							{
								Itur iturget = this._iturDictionary[parentItur.IturCode];
								iturget.NodeType = 2;
								this._iturDictionary[parentItur.IturCode] = iturget; 				   //Записываем в коллекцию другую, которая будет ухожить на запись
							}
							else
							{
								;//ошибка, такого не должно быть
							}
						}
					}
					else
					{	//ops Parent не найден в БД надо добавить 
						// Eсли еще не добавили как новый на предыдущих шагах
						if (tempDictionryAddedItur.ContainsKey(itur.ParentIturCode) == false)
						{
							Itur newParenItur = new Itur();
							string erpIturCode = itur.ParentIturCode;
							string[] locationCodes = erpIturCode.Split('-');
							int deepNode = locationCodes.Length;
							string locationCode = this.GetLocationCode(locationCodes);
							Itur tempItur = this.GetNewIturCode(toDBPath, dictionaryPrffixIndex,
								erpIturCode, locationCode, deepNode);
							if (tempItur.NumberSufix == "-1")
							{
								Log.Add(MessageTypeEnum.Error, "Can't insert in db Itur [" + erpIturCode + "] Itur suffix too long");
							}
							else
							{
								if (deepNode > 0)
								{
									newParenItur.IturCode = tempItur.IturCode;
									newParenItur.Number = tempItur.Number;
									newParenItur.NumberPrefix = tempItur.NumberPrefix;
									newParenItur.NumberSufix = tempItur.NumberSufix;
									newParenItur.LocationCode = locationCode;
									newParenItur.ERPIturCode = itur.ParentIturCode;
									newParenItur.StatusIturBit = 0;
									newParenItur.Disabled = false;
									newParenItur.LevelNum = deepNode;
									if (deepNode == 1)		  //root 	Not Terminal 
									{
										newParenItur.Level1 = locationCodes[0];
										newParenItur.Name1 = locationCodes[0];
										newParenItur.Name = newParenItur.Name1; 	 // "Added automatic";
										newParenItur.NodeType = 2; //Not terminal Contaner
									}
									if (deepNode == 2)			//root + 1 , Not Terminal  
									{
										newParenItur.Level1 = locationCodes[0];
										newParenItur.Level2 = locationCodes[1];
										newParenItur.Name1 = locationCodes[0];
										newParenItur.Name2 = locationCodes[1];
										newParenItur.Name = newParenItur.Name2; 	 // "Added automatic";
										newParenItur.ParentIturCode = locationCodes[0];		 //root
										newParenItur.NodeType = 2; 	 //2; 	 //?? менять здесь если добавится 3уровень Terminal Contaner Может быть 2 если станет нетерминальной
									}
									if (deepNode == 3)			//root + 2 Not Terminal  
									{
										newParenItur.Level1 = locationCodes[0];
										newParenItur.Level2 = locationCodes[1];
										newParenItur.Level3 = locationCodes[2];
										newParenItur.Name1 = locationCodes[0];
										newParenItur.Name2 = locationCodes[1];
										newParenItur.Name3 = locationCodes[2];
										newParenItur.Name = newParenItur.Name3; 	 // "Added automatic";
										newParenItur.ParentIturCode = locationCodes[0] + "-" + locationCodes[1]; 	 //root +1
										newParenItur.NodeType = 2; 	 //2; 	 //Not Terminal Contaner
									}


									newParenItur.Description = "Added automatic";
									tempDictionryAddedItur[itur.ParentIturCode] = newParenItur;
									this._iturDictionary[newParenItur.IturCode] = newParenItur;
								}
							}
						}
						//}
					}//ops Parent не найден надо добавить 

				}

			}

			//come to end
			IImportIturRepository importIturRepository = _serviceLocator.GetInstance<IImportIturRepository>();
			importIturRepository.ClearIturs(toDBPath);

			//foreach (KeyValuePair<string, Itur> keyValueItur in this._iturDictionary)	   // источник db3
			foreach (KeyValuePair<string, Itur> keyValueItur in this._iturDictionary)	   // источник db3
			{
				yield return keyValueItur.Value;
			}
			
			//return this._iturDictionary;
		}

		private string GetLocationCode(string[] locationCodes)
		{
			string locationCode = "";
			locationCode = locationCodes[0].Trim();
			locationCode = locationCode.CutLength(49);
			if (locationCode.Trim().ToLower() == "locationcode") locationCode = "";
			return locationCode;
		}

		private string GetIturCodeERP(string locationCodeFrom)
		{
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');
			locationCodeFrom = locationCodeFrom.TrimEnd('-');

			string iturCodeERP = locationCodeFrom.CutLength(249);
			return iturCodeERP;
		}

		private Itur GetNewIturCode(string toDBPath, Dictionary<string, int> dictionaryPrffixIndex, string iturCodeERP, string locationCode, int deepNode)
		{
			Itur tempItur = new Itur();
			if (dictionaryPrffixIndex.ContainsKey(locationCode) == false)
			{
				dictionaryPrffixIndex[locationCode] = 1;	   //добавляем новый счетчик специально для локейшена (считаем намбер для суффикса)
				int maxNumber = this._iturRepository.GetMaxNumber(locationCode, toDBPath);
				if (maxNumber > 9998)
				{
					tempItur.NumberSufix = "-1";
					return tempItur;
				}
				if (maxNumber > 1) dictionaryPrffixIndex[locationCode] = maxNumber;
			}
			//================================
			string prefix = locationCode;
			string suffix = "";
			if (deepNode == 1)
			{
				suffix = "1";
			}
			else
			{
				int lastIndex = dictionaryPrffixIndex[locationCode];
				lastIndex++;
				if (lastIndex > 9998)
				{
					tempItur.NumberSufix = "-1";
					return tempItur;
				}
				suffix = lastIndex.ToString();
				dictionaryPrffixIndex[locationCode] = lastIndex;
			}

		
			int num  =  0;
			bool ret = Int32.TryParse (suffix.TrimStart('0'), out num);
			tempItur.Number = num;
			tempItur.NumberPrefix = prefix.PadLeft(4, '0');
			tempItur.NumberSufix = suffix.ToString().PadLeft(4, '0');
			string newIturCode = tempItur.NumberPrefix + tempItur.NumberSufix;
			tempItur.IturCode = newIturCode;
			return tempItur;
		}

		#region IIturParser Members


		//public IEnumerable<Itur> GetItursEnumerable(string fromPathFile, 
		public Dictionary<string, Itur> GetIturs(string fromPathFile,
			Encoding encoding, string[] separators, int countExcludeFirstString, Dictionary<string, Itur> IturFromDBDictionary, Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			throw new NotImplementedException();
		}

		#endregion

	}


}
