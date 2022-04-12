using System;
using System.Collections.Generic;
using Count4U.Model.Interface.Count4U;
using System.Data.SqlServerCe;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.SelectionParams;
using NLog;
using System.Threading;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using Count4U.Model.Count4Mobile;
using System.IO;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using System.Reflection;

namespace Count4U.Model.Count4U
{
	public class CurrentInventoryAdvancedReader : BaseADORepository, ICurrentInventoryAdvancedReader
	{
		private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private Dictionary<string, string> _dictionaryPropertyStr;

		public CurrentInventoryAdvancedReader(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository)
			: base(connection, dbSettings,log, serviceLocator)
        {
			this._makatRepository = makatRepository;
			this._dictionaryPropertyStr = new Dictionary<string, string>();
	    }


	
		public IEnumerable<CurrentInventoryAdvanced> GetCurrentInventoryAdvanceds(string pathDB,
			bool refill = true,
			bool refillCatalogDictionary = false,
			SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null,
			//bool addResult = true,
			List<ImportDomainEnum> importType = null)


		{
			if (refill == true)
			{
				this.ClearCurrentInventoryAdvanced( pathDB);
			}

			Action<long> countAction = null;
			CancellationToken cancellationToken = CancellationToken.None;
			PriceCodeEnum priceCode = PriceCodeEnum.PriceBuy;
			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
			if (parmsIn != null)
			{
				parms = parmsIn.ConvertToImportProviderParmEnum();
				cancellationToken = parms.GetCancellationTokenFromParm();
				priceCode = parms.GetPriceCodeEnumFromParm();
			}
			if (cancellationToken == CancellationToken.None)	//throw new ArgumentNullException("CancellationToken.None");
			{
				var c = new CancellationTokenSource();
				cancellationToken = c.Token;
			}
			countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null)// throw new ArgumentNullException("ActionUpdateProgress is null");
			{
				countAction = CountLong;
			}

			string photoPath = parms.GetStringValueFromParm(ImportProviderParmEnum.Path1);
			string photoPropertyName = parms.GetStringValueFromParm(ImportProviderParmEnum.PropertyName); 

			IInventorConfigRepository inventorConfigRepository = this._serviceLocator.GetInstance<IInventorConfigRepository>();
			string inventorCode = "none";
			string inventorName = "none";
			string inventorSubFolder = inventorConfigRepository.GetPathDBFromRelativePathDB(pathDB, "Inventor", out inventorCode);
			IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
			Inventor inventor = inventorRepository.GetInventorByCode(inventorCode);
			if (inventor != null)
			{
				inventorName = inventor.Name;
			}

			//IIturAnalyzesRepository iturAnalyzesRepository =  this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			//Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refillCatalogDictionary);

			this.Log.Add(MessageTypeEnum.TraceRepository, "GetCurrentInventoryAdvanceds");

			int k = 0;

			//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));

			InventProducts inventProductList = null;
			if (selectParms == null)
			{
				selectParms = new SelectParams();
			}
			//if (selectParms.FilterStringListParams.ContainsKey("TypeMakat") == false)
			//{
			//	List<string> typeMakatList = new List<string>();
			//	typeMakatList.Add(TypeMakatEnum.M.ToString());
			//	typeMakatList.Add(TypeMakatEnum.B.ToString());
			//	typeMakatList.Add(TypeMakatEnum.W.ToString());
			//	selectParms.FilterStringListParams.Add("TypeMakat", new FilterStringListParam()
			//	{
			//		Values = typeMakatList
			//	});
			//}

			IIturRepository itruERRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Dictionary<string, Itur> dictionaryERPItur = itruERRepository.GetERPIturDictionary(pathDB);

			IPropertyStrRepository	propertyStrRepository = this._serviceLocator.GetInstance<IPropertyStrRepository>();
			this._dictionaryPropertyStr = propertyStrRepository.GetNamePropertyStrDictionaryByDomainAndCodeAsKey(pathDB);

			IPreviousInventoryRepository _previousInventoryEFRepository = this._serviceLocator.GetInstance<IPreviousInventoryRepository>();
			Dictionary<string, PreviousInventory> dictionaryPreviousInventory = _previousInventoryEFRepository.GetDictionaryPreviousInventorys(pathDB);


			IInventProductRepository _inventProductRepositoryRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			InventProducts _inventProductList = new InventProducts();
			try
			{
				inventProductList = _inventProductRepositoryRepository.GetInventProducts(pathDB);
			}
			catch { }

			if (inventProductList != null)
			{
				foreach (InventProduct inventProduct in inventProductList)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}

					k++;
					CurrentInventoryAdvanced newCurrentInventoryAdvanced = new CurrentInventoryAdvanced();
					try
					{
						//===================Inventor======================================
						//newCurrentInventoryAdvanced.InventorCode = inventorCode;
						//newCurrentInventoryAdvanced.InventorName = inventorName;
						//==================== PDA =========================================
						//newCurrentInventoryAdvanced.SectionCode = DomainUnknownCode.UnknownSection;
						//newCurrentInventoryAdvanced.DocumentCode = inventProduct.DocumentCode;
					
						//string UID = inventProduct.Code;
						string UID = inventProduct.Barcode;
						newCurrentInventoryAdvanced.Uid = UID;
						string[] uids = UID.Split('|');
						string erpIturCode = inventProduct.ERPIturCode;
						string locationCode = inventProduct.ERPIturCode;
						if (uids.Length > 2)
						{
							if (string.IsNullOrWhiteSpace(uids[2]) == false) locationCode = uids[2];		
						}

						string makat = inventProduct.Makat;
						if (uids.Length > 1)
						{
							if (string.IsNullOrWhiteSpace(uids[1]) == false) makat = uids[1];
						}
							
						string serialNumber = inventProduct.SerialNumber;
						if (uids.Length > 0)
						{
							if (string.IsNullOrWhiteSpace(uids[0]) == false) serialNumber = uids[0];		//попросил Нив, потому как используется и для адаптеров без UID					
						}

						string[] ids = new string[] { makat, locationCode };
						string makatInItur = ids.JoinRecord("|");
						newCurrentInventoryAdvanced.PropertyStr20 = makatInItur.CutLength(49);	   //для Merkava ExportCurrentInventoryMerkavaXslxWriter2_2

						if (dictionaryERPItur.ContainsKey(locationCode) == true)
						{
							Itur currentItur = dictionaryERPItur[locationCode];
							if (currentItur != null)
							{
								newCurrentInventoryAdvanced.IturCode = currentItur.IturCode;
								newCurrentInventoryAdvanced.LocationDescription = currentItur.Description;
								newCurrentInventoryAdvanced.LocationLevel1Code = currentItur.Level1;
								newCurrentInventoryAdvanced.LocationLevel1Name = currentItur.Name1;
								newCurrentInventoryAdvanced.LocationLevel2Code = currentItur.Level2;
								newCurrentInventoryAdvanced.LocationLevel2Name = currentItur.Name2;
								newCurrentInventoryAdvanced.LocationLevel3Code = currentItur.Level3;
								newCurrentInventoryAdvanced.LocationLevel3Name = currentItur.Name3;
								newCurrentInventoryAdvanced.LocationLevel4Code = currentItur.Level4;
								newCurrentInventoryAdvanced.LocationLevel4Name = currentItur.Name4;
								newCurrentInventoryAdvanced.LocationInvStatus = currentItur.InvStatus.ToString();
								newCurrentInventoryAdvanced.LocationNodeType = currentItur.NodeType.ToString();
								newCurrentInventoryAdvanced.LocationLevelNum = currentItur.LevelNum.ToString();
								newCurrentInventoryAdvanced.LocationTotal = currentItur.Total.ToString();
							}
						}
						
						newCurrentInventoryAdvanced.LocationCode = locationCode;
						newCurrentInventoryAdvanced.ItemCode = makat;
						newCurrentInventoryAdvanced.SerialNumberLocal = serialNumber;

						newCurrentInventoryAdvanced.DomainObject = "CurrentInventory";
						newCurrentInventoryAdvanced.Table = "";
						newCurrentInventoryAdvanced.Adapter = "Clalit";
						newCurrentInventoryAdvanced.SerialNumberSupplier = inventProduct.SupplierCode;
						newCurrentInventoryAdvanced.CatalogItemName = inventProduct.ProductName;
	 					double quantityEdit = inventProduct.QuantityEdit != null ? Convert.ToDouble(inventProduct.QuantityEdit) : 0;
						newCurrentInventoryAdvanced.QuantityDouble = quantityEdit;
						newCurrentInventoryAdvanced.Quantity = quantityEdit.ToString();


						newCurrentInventoryAdvanced.DateModified = Convert.ToDateTime(inventProduct.ModifyDate).ToString("yyyy-MM-dd");
						newCurrentInventoryAdvanced.DateCreated = Convert.ToDateTime(inventProduct.CreateDate).ToString("yyyy-MM-dd");
						newCurrentInventoryAdvanced.PropertyStr1 = Convert.ToDateTime(inventProduct.ModifyDate).ToString("dd-MM-yyyy");
						newCurrentInventoryAdvanced.PropertyStr2 = Convert.ToDateTime(inventProduct.CreateDate).ToString("dd-MM-yyyy");
						newCurrentInventoryAdvanced.PropertyStr7 = Convert.ToDateTime(inventProduct.ModifyDate).ToString("dd.MM.yyyy HH:mm");
						newCurrentInventoryAdvanced.PropertyStr8 = Convert.ToDateTime(inventProduct.CreateDate).ToString("dd.MM.yyyy HH:mm");
						//link фото
					//	string photoPropertyName = this._userSettingsManager.InventProductPropertyPhotoSelectedItemGet();
						string photoName = this.GetPropertyStringValueByName(inventProduct, photoPropertyName);
						if (string.IsNullOrWhiteSpace(photoName) == false)
						{
							newCurrentInventoryAdvanced.PropertyStr3 = photoName;
						}
						//
						newCurrentInventoryAdvanced.ItemStatus = inventProduct.ItemStatus;
						newCurrentInventoryAdvanced.UnitTypeCode = inventProduct.ImputTypeCodeFromPDA;

						newCurrentInventoryAdvanced.PropertyStr1Code = inventProduct.IPValueStr1;
						newCurrentInventoryAdvanced.PropertyStr1Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr1,
							newCurrentInventoryAdvanced.PropertyStr1Code);

						newCurrentInventoryAdvanced.PropertyStr2Code = inventProduct.IPValueStr2;
						newCurrentInventoryAdvanced.PropertyStr2Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr2,
							newCurrentInventoryAdvanced.PropertyStr2Code);

						newCurrentInventoryAdvanced.PropertyStr3Code = inventProduct.IPValueStr3;
						newCurrentInventoryAdvanced.PropertyStr3Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr3,
							newCurrentInventoryAdvanced.PropertyStr3Code);

						newCurrentInventoryAdvanced.PropertyStr4Code = inventProduct.IPValueStr4;
						newCurrentInventoryAdvanced.PropertyStr4Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr4,
							newCurrentInventoryAdvanced.PropertyStr4Code);

						newCurrentInventoryAdvanced.PropertyStr5Code = inventProduct.IPValueStr5;
						newCurrentInventoryAdvanced.PropertyStr5Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr5,
							newCurrentInventoryAdvanced.PropertyStr5Code);

						newCurrentInventoryAdvanced.PropertyStr6Code = inventProduct.IPValueStr6;
						newCurrentInventoryAdvanced.PropertyStr6Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr6,
							newCurrentInventoryAdvanced.PropertyStr6Code); 

						newCurrentInventoryAdvanced.PropertyStr7Code = inventProduct.IPValueStr7;
						newCurrentInventoryAdvanced.PropertyStr7Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr7,
							newCurrentInventoryAdvanced.PropertyStr7Code);
						//особый случай для Merkava export	Code => ID					
						newCurrentInventoryAdvanced.PropertyStr6 = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr6,
							newCurrentInventoryAdvanced.PropertyStr7Code).CutLength(49);		//в теориии 	PropertyStr7Code== PropertyStr6Code

						newCurrentInventoryAdvanced.PropertyStr8Code = inventProduct.IPValueStr8;
						newCurrentInventoryAdvanced.PropertyStr8Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr8,
							newCurrentInventoryAdvanced.PropertyStr8Code);

						newCurrentInventoryAdvanced.PropertyStr9Code = inventProduct.IPValueStr9;
						newCurrentInventoryAdvanced.PropertyStr9Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr9,
							newCurrentInventoryAdvanced.PropertyStr9Code);

						newCurrentInventoryAdvanced.PropertyStr10Code = inventProduct.IPValueStr10;
						newCurrentInventoryAdvanced.PropertyStr10Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr10,
							newCurrentInventoryAdvanced.PropertyStr10Code);

						newCurrentInventoryAdvanced.PropertyStr11Code = inventProduct.IPValueStr11;
						newCurrentInventoryAdvanced.PropertyStr11Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr11,
							newCurrentInventoryAdvanced.PropertyStr11Code);

						newCurrentInventoryAdvanced.PropertyStr12Code = inventProduct.IPValueStr12;
						newCurrentInventoryAdvanced.PropertyStr12Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr12,
							newCurrentInventoryAdvanced.PropertyStr12Code);

						newCurrentInventoryAdvanced.PropertyStr13Code = inventProduct.IPValueStr13;
						newCurrentInventoryAdvanced.PropertyStr13Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr13,
							newCurrentInventoryAdvanced.PropertyStr13Code);

						newCurrentInventoryAdvanced.PropertyStr14Code = inventProduct.IPValueStr14;
						newCurrentInventoryAdvanced.PropertyStr14Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr14,
							newCurrentInventoryAdvanced.PropertyStr14Code);

						newCurrentInventoryAdvanced.PropertyStr15Code = inventProduct.IPValueStr15;
						newCurrentInventoryAdvanced.PropertyStr15Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr15,
							newCurrentInventoryAdvanced.PropertyStr15Code);

						newCurrentInventoryAdvanced.PropertyStr16Code = inventProduct.IPValueStr16;
						newCurrentInventoryAdvanced.PropertyStr16Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr16,
							newCurrentInventoryAdvanced.PropertyStr16Code);

						newCurrentInventoryAdvanced.PropertyStr17Code = inventProduct.IPValueStr17;
						newCurrentInventoryAdvanced.PropertyStr17Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr17,
							newCurrentInventoryAdvanced.PropertyStr17Code);

						newCurrentInventoryAdvanced.PropertyStr18Code = inventProduct.IPValueStr18;
						newCurrentInventoryAdvanced.PropertyStr18Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr18,
							newCurrentInventoryAdvanced.PropertyStr18Code);

						newCurrentInventoryAdvanced.PropertyStr19Code = inventProduct.IPValueStr19;
						newCurrentInventoryAdvanced.PropertyStr19Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr19,
							newCurrentInventoryAdvanced.PropertyStr19Code);

						newCurrentInventoryAdvanced.PropertyStr20Code = inventProduct.IPValueStr20;
						newCurrentInventoryAdvanced.PropertyStr20Name = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr20,
							newCurrentInventoryAdvanced.PropertyStr20Code);


						if (dictionaryPreviousInventory.ContainsKey(UID) == true)
						{
							PreviousInventory previousInventory = dictionaryPreviousInventory[UID];
							if (previousInventory != null)
							{
								newCurrentInventoryAdvanced.PropExtenstion1 = previousInventory.PropExtenstion1;
								newCurrentInventoryAdvanced.PropExtenstion2 = previousInventory.PropExtenstion2;
								newCurrentInventoryAdvanced.PropExtenstion3 = previousInventory.PropExtenstion3;
								newCurrentInventoryAdvanced.PropExtenstion4 = previousInventory.PropExtenstion4;
								newCurrentInventoryAdvanced.PropExtenstion5 = previousInventory.PropExtenstion5;
								newCurrentInventoryAdvanced.PropExtenstion6 = previousInventory.PropExtenstion6;
								newCurrentInventoryAdvanced.PropExtenstion7 = previousInventory.PropExtenstion7;
								newCurrentInventoryAdvanced.PropExtenstion8 = previousInventory.PropExtenstion8;
								newCurrentInventoryAdvanced.PropExtenstion9 = previousInventory.PropExtenstion9;
								newCurrentInventoryAdvanced.PropExtenstion10 = previousInventory.PropExtenstion10;
								newCurrentInventoryAdvanced.PropExtenstion11 = previousInventory.PropExtenstion11;
								newCurrentInventoryAdvanced.PropExtenstion12 = previousInventory.PropExtenstion12;
								newCurrentInventoryAdvanced.PropExtenstion13 = previousInventory.PropExtenstion13;
								newCurrentInventoryAdvanced.PropExtenstion14 = previousInventory.PropExtenstion14;
								newCurrentInventoryAdvanced.PropExtenstion15 = previousInventory.PropExtenstion15;
								newCurrentInventoryAdvanced.PropExtenstion16 = previousInventory.PropExtenstion16;
								newCurrentInventoryAdvanced.PropExtenstion17 = previousInventory.PropExtenstion17;
								newCurrentInventoryAdvanced.PropExtenstion18 = previousInventory.PropExtenstion18;
								newCurrentInventoryAdvanced.PropExtenstion19 = previousInventory.PropExtenstion19;
								newCurrentInventoryAdvanced.PropExtenstion20 = previousInventory.PropExtenstion20;
								newCurrentInventoryAdvanced.PropExtenstion21 = previousInventory.PropExtenstion21;
								newCurrentInventoryAdvanced.PropExtenstion22 = previousInventory.PropExtenstion22;

							}
						}
																																			
							
						//if (productSimpleDictionary.ContainsKey(inventProduct.Makat) == true)
						//{
						//	ProductSimple productSimple = productSimpleDictionary[inventProduct.Makat];
						//	newCurrentInventoryAdvanced.FromCatalogType = productSimple.FromCatalogType;
						//	//newIturAnalyzes.PriceString = productSimple.PriceString;
						//	double price = 0;
						//	double priceBuy = productSimple.PriceBuy != null ? Convert.ToDouble(productSimple.PriceBuy) : 0;
						//	double priceSale = productSimple.PriceSale != null ? Convert.ToDouble(productSimple.PriceSale) : 0;
						//	double priceExtra = productSimple.PriceExtra != null ? Convert.ToDouble(productSimple.PriceExtra) : 0;
						//	price = priceCode.GetPriceByPriceCode(priceBuy, priceSale, priceExtra);

						//	newCurrentInventoryAdvanced.PriceBuy = priceBuy;
						//	newCurrentInventoryAdvanced.PriceSale = priceSale;
						//	newCurrentInventoryAdvanced.PriceExtra = priceExtra;
						//	newCurrentInventoryAdvanced.Price = price;

						//	newCurrentInventoryAdvanced.Code = productSimple.Code != null ? productSimple.Code : "";

						//	//newIturAnalyzes.ValueBuyQriginal = productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityOriginal);
						//	int countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;
						//	double quantityEditAndPartial = quantityEdit;
						//	if (countInParentPack != 0) quantityEditAndPartial = quantityEdit + (double)quantityInPackEdit / (double)countInParentPack;
						//	newCurrentInventoryAdvanced.QuantityEdit = quantityEditAndPartial;
						//	newCurrentInventoryAdvanced.ValueBuyEdit = price * quantityEditAndPartial;// productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityEdit);

						//	newCurrentInventoryAdvanced.QuantityDifference = quantityEditAndPartial - quantityOriginal;
						//	//inventProduct.QuantityEdit - inventProduct.QuantityOriginal;
						//	//newIturAnalyzes.ValueBuyDifference = productSimple.PriceBuy *
						//	//    (Convert.ToDouble(inventProduct.QuantityOriginal) - Convert.ToDouble(inventProduct.QuantityEdit));
						//}

					
					}
					catch (Exception error)
					{
						_logger.ErrorException("GetCurrentInventoryAdvanceds", error);
						this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
					}
					yield return newCurrentInventoryAdvanced;
				}		 //foreach
			}
		}

		private void CountLong(long count)
		{
		}

		private string GetPropertyStr(DomainObjectTypeEnum domainPropertyStr, string propertyStr1Code)
		{
			string name = "";
			if (string.IsNullOrWhiteSpace(propertyStr1Code) == true) return name;
			name = propertyStr1Code; //Add 29.10.2017
			if (string.IsNullOrWhiteSpace(domainPropertyStr.ToString()) == true) return name;
			if (this._dictionaryPropertyStr == null) return name;

			string key = domainPropertyStr.ToString() + "|" + propertyStr1Code;
			if (this._dictionaryPropertyStr.ContainsKey(key) == true)
			{
				name = this._dictionaryPropertyStr[key];
			}
			return name;
		}

	
		public void ClearCurrentInventoryAdvanced(string pathDB)
		{
			string analyticDBFile = DbSettings.AnalyticDBFile;
			string analyticPathDB = base.ConnectionADO.BuildPathFileADO(pathDB, analyticDBFile);
			if (File.Exists(analyticPathDB) == false) return;

			//this.Log.Clear();
			//string sql1 = @"DROP TABLE [PreviousInventory]";
			string sql1 = @"DELETE FROM [CurrentInventoryAdvanced]";

			SqlCeTransaction tran = null;
			string connectionString = this.BuildADOConnectionStringBySubFolder(pathDB, analyticDBFile);
			SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString);

			try
			{
				sqlCeConnection.Open();
				tran = sqlCeConnection.BeginTransaction();
				var cmd = new SqlCeCommand(sql1, sqlCeConnection, tran);
				//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB [{1}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "PreviousInventory", connectionString));
				//Localization.Resources.Log_TraceRepository1001%"Start Process: Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1001, "PreviousInventory"));
				//LogPrint();
				cmd.ExecuteNonQuery();
				//Localization.Resources.Log_TraceRepositoryResult1015%"Clear [{0}] Via ADO.NET"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1015, "PreviousInventory"));
				tran.Commit();
			}
			catch (Exception error)
			{
				_logger.ErrorException("CurrentInventoryAdvanced", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + " : " + error.StackTrace);
				tran.Rollback();
			}
			finally
			{
				sqlCeConnection.Close();
			}
		}



		private string GetPropertyStringValueByName(InventProduct inventProduct, string propertyName)
		{
			string propertyValue = "";
			if (string.IsNullOrWhiteSpace(propertyName) == true) return propertyValue;
			System.Type t = typeof(InventProduct);
			PropertyInfo pi = t.GetProperty(propertyName);
			if (pi == null) return propertyValue;
			propertyValue = pi.GetValue(inventProduct, null).ToString();
			return propertyValue;
		}
		
	}
}
