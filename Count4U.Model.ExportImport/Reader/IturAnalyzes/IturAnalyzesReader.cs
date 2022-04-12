using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.SelectionParams;
using NLog;
using System.Diagnostics;
using System.Threading;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;


namespace Count4U.Model.Count4U
{
	public class IturAnalyzesReader : BaseADORepository, IIturAnalyzesReader
	{
		private readonly IMakatRepository _makatRepository;
		//private readonly IInventorConfigRepository _inventorConfigRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private Dictionary<string, string> _dictionaryPropertyStr;

		public IturAnalyzesReader(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository)
			: base(connection, dbSettings, log, serviceLocator)
        {
			this._makatRepository = makatRepository;
			//this._inventorConfigRepository = inventorConfigRepository;
		
	    }


		public IEnumerable<IturAnalyzes> GetIturAnalyzes(string pathDB,
		bool refill = true,
		bool refillCatalogDictionary = false,
		SelectParams selectParms = null,
		Dictionary<object, object> parmsIn = null,
		bool addResult = true,
			//Dictionary<ImportProviderParmEnum, object> parms = null, ???
		List<ImportDomainEnum> importType = null)
		{

			//public void InsertIturAnalyzes1(string pathDB, bool refill = true, SelectParams selectParms = null,
			//    Dictionary<object, object> parmsIn = null)
			//{
			if (selectParms != null)
			{
				string selectParmsString = selectParms.ToString();
			}
			if (refill == true)
			{
				base.ClearIturAnalyzes(pathDB);
			}


			Action<long> countAction = null;
			CancellationToken cancellationToken = CancellationToken.None;
			PriceCodeEnum priceCode = PriceCodeEnum.PriceBuy;
			string createDateTime = "";
			string codeReport = "";
			string fromBuildAnalyze = "";

			bool includeProductMakatWithoutInventProduct = false;
			if (importType != null)
			{
				if (importType.Contains(ImportDomainEnum.IncludeProductMakatWithoutInventProduct) == true)
				{
					includeProductMakatWithoutInventProduct = true;
				}
			}

			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
			if (parmsIn != null)
			{
				parms = parmsIn.ConvertToImportProviderParmEnum();
			}
			if (parms != null)
			{
				cancellationToken = parms.GetCancellationTokenFromParm();
				priceCode = parms.GetPriceCodeEnumFromParm();
				createDateTime = parms.GetStringValueFromParm(ImportProviderParmEnum.CreateDateTime);
				codeReport = parms.GetStringValueFromParm(ImportProviderParmEnum.CodeReport);
				fromBuildAnalyze = parms.GetStringValueFromParm(ImportProviderParmEnum.FromBuildAnalyze);
				//includeProductMakatWithoutInventProduct = parms.GetBoolValueFromParm(ImportProviderParmEnum.IncludeProductMakatWithoutInventProduct);
			}

				
			Dictionary<string, string> startupArgumentDictionary = base.DbSettings.SettingsRepository.StartupArgumentDictionary;
			bool isMplAdapter = false;
			if (startupArgumentDictionary.Count > 0)
			{
				isMplAdapter = startupArgumentDictionary.ContainsKey("MPL");
			}
			
			Dictionary<string, ProductSimple> productBarcodeSimpleDictionary = new Dictionary<string, ProductSimple>();

			//if (includeProductMakatWithoutInventProduct == true)
			//{
				//IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
				//productRepository.FillProductSimpleDictionary(pathDB, TypeMakatEnum.M.ToString(), out productBarcodeSimpleDictionary);// "B"
			//}

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

			

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();

			IIturAnalyzesRepository iturAnalyzesRepository =
				this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			Dictionary<string, Itur> iturDictionary = iturAnalyzesRepository.GetIturDictionary(pathDB, refill);

			Dictionary<string, DocumentHeader> documentHeaderDictionary = iturAnalyzesRepository.GetDocumentHeaderDictionary(pathDB, refill);
			Dictionary<string, Location> locationDictionary = iturAnalyzesRepository.GetLocationDictionary(pathDB, refill);
			ISectionRepository sectionRepository = this._serviceLocator.GetInstance<ISectionRepository>();
			Dictionary<string, Section> sectionFromDBDictionary = sectionRepository.GetSectionDictionary(pathDB, true);
			IFamilyRepository familyRepository = this._serviceLocator.GetInstance<IFamilyRepository>();
			//_logger.Info("familyFromDBDictionary before");
			Dictionary<string, Family> familyFromDBDictionary = familyRepository.GetFamilyDictionary(pathDB, true);

		

			if (fromBuildAnalyze == "BuildAnalyzeTableSimple" 
				|| codeReport == "[Rep-IS1-63v]"
				|| codeReport == "[Rep-IS1-63s]"
				|| codeReport == "[Rep-IS1-63q]")
			{
				includeProductMakatWithoutInventProduct = true;
				IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
				productRepository.FillProductSimpleDictionary(pathDB, TypeMakatEnum.B.ToString(), out productBarcodeSimpleDictionary);// "B"

			}

			if (codeReport == "[Rep-IS1-62n]"
				|| codeReport == "[Rep-IS1-62np]")
			{
				includeProductMakatWithoutInventProduct = true;
				IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
				productRepository.FillProductSimpleDictionary(pathDB, TypeMakatEnum.M.ToString(), out productBarcodeSimpleDictionary);// "B"

			}

			
			//_logger.Info("familyFromDBDictionary after" + familyFromDBDictionary.Count().ToString());
			//Dictionary<int, IturStatusEnum> statusIturDictionary = iturAnalyzesRepository.GetStatusIturDictionary();
			//Dictionary<int, IturStatusGroupEnum> statusIturGroupDictionary = iturAnalyzesRepository.GetStatusIturGroupDictionary();
			//bool refillCatalogDictionary = false;
			Dictionary<string, ProductMakat> productMakatDictionary = iturAnalyzesRepository.GetProductMakatDictionary(pathDB, refillCatalogDictionary);// "M" & "B" FillProductBarcodeDictionary все баркоды и макаты
			Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refillCatalogDictionary);// "M"
			
				
			IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
			Dictionary<string, DocumentHeader> iturDictionaryMaxDateTime = documentHeaderRepository.GetIturDictionaryMaxDateTime(pathDB);

			Dictionary<string, ProductSimple> productSimpleBarcodeOnlyDictionary = new Dictionary<string, ProductSimple>();

			if (codeReport == "[Rep-IS1-58]" || codeReport == "[Rep-IS1-58p]" || inventorCode == "3c6dadfe-c408-4988-aacf-874692b9cf61" || isMplAdapter) //Adapter - MPL & Oren Original
			{
				productSimpleBarcodeOnlyDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, true, TypeMakatEnum.B.ToString()); //"B"
			}

			IPropertyStrRepository propertyStrRepository = this._serviceLocator.GetInstance<IPropertyStrRepository>();
			if (codeReport == "[Rep-IS1-93]"
				|| codeReport == "[Rep-IS1-94]")	
			{
			this._dictionaryPropertyStr = propertyStrRepository.GetNamePropertyStrDictionaryByDomainAndCodeAsKey(pathDB);
			}

			int k = 0;


			//Localization.Resources.Log_TraceRepositoryResult1003%"[{0}] in DB {1}"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1003, "IturAnalyzes", pathDB));

			InventProducts inventProductList = null;

			if (selectParms == null)
			{
				selectParms = new SelectParams();
			}


			if (selectParms.FilterStringListParams.ContainsKey("TypeMakat") == false)
			{
				List<string> typeMakatList = new List<string>();
				typeMakatList.Add(TypeMakatEnum.M.ToString());
				typeMakatList.Add(TypeMakatEnum.B.ToString());
				typeMakatList.Add(TypeMakatEnum.W.ToString());
				selectParms.FilterStringListParams.Add("TypeMakat", new FilterStringListParam()
				{
					Values = typeMakatList
				});
			}

			
			List<string> iturList = new List<string>();
			//LocationCode потому что InventProduct связан с Location через Itur!!! 
			if (selectParms.FilterStringListParams.ContainsKey("LocationCode") == true)
			{
				List<string> locationCodeList = selectParms.FilterStringListParams["LocationCode"].Values;
				selectParms.FilterStringListParams.Remove("LocationCode");

			
				foreach (var locationCode in locationCodeList)
				{
					Iturs iturs = iturRepository.GetItursByLocationCode(locationCode, pathDB);
					foreach (var itur in iturs)
					{
						iturList.Add(itur.IturCode);
					}
				}
				if (iturList.Count > 1000) _logger.Info("GetIturAnalyzes : IturListCode.Count  == " + iturList.Count.ToString() + "=> possible stack overflow error(FilterStringListParam)"); 
				selectParms.FilterStringListParams.Add("IturCode", new FilterStringListParam() { Values = iturList });
			}

			Dictionary<string, ProductSimple> productBarcodeExcludeDictionary = new Dictionary<string, ProductSimple>();
			if (codeReport == "[Rep-IS1-85L]")
			{
				IMakatRepository makatRepository = this._serviceLocator.GetInstance<IMakatRepository>();
				productBarcodeExcludeDictionary = makatRepository.FillProductBarcodeExcludeItursDictionary(iturList, pathDB);
			}

			try
			{
				inventProductList = iturAnalyzesRepository.GetInventProductList(selectParms, pathDB);
			}
			catch (Exception exp)
			{
				_logger.Error("GetIturAnalyzes : GetInventProductList : ", exp.Message); 
			}

			if (inventProductList != null)
			{


				foreach (InventProduct inventProduct in inventProductList)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
					k++;
					IturAnalyzes newIturAnalyzes = new IturAnalyzes();

					try
					{
						//===================Inventor======================================
						newIturAnalyzes.InventorCode = inventorCode;
						newIturAnalyzes.InventorName = inventorName;

						//==================== PDA =========================================
						newIturAnalyzes.DocumentHeaderCode = inventProduct.DocumentHeaderCode;
						newIturAnalyzes.DocumentCode = inventProduct.DocumentCode;
						newIturAnalyzes.IturCode = inventProduct.IturCode;
						newIturAnalyzes.Barcode = inventProduct.Barcode;
						newIturAnalyzes.Doc_Name = inventProduct.StatusInventProductCode != null ? inventProduct.StatusInventProductCode : "";

						newIturAnalyzes.SupplierCode = inventProduct.SupplierCode != null ? inventProduct.SupplierCode : "";
						newIturAnalyzes.SerialNumber = inventProduct.SerialNumber != null ? inventProduct.SerialNumber : "";

						newIturAnalyzes.TypeMakat = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";

						newIturAnalyzes.IPValueStr1 = inventProduct.IPValueStr1;		//Made4Net Boxes Quantity
						newIturAnalyzes.IPValueStr2 = inventProduct.IPValueStr2;	// Made4Net ExpireDate string ?		 and //orderID  InventProductWarehouseParser 
						newIturAnalyzes.IPValueStr10 = inventProduct.IPValueStr10; //remark LadyComfort in InventProduct
						newIturAnalyzes.IPValueStr8 = inventProduct.IPValueStr8; //IS1-93
                        if (codeReport == "[Rep-IS1-90]"
							|| codeReport == "[Rep-IS1-90c]"
							|| codeReport == "[Rep-IS1-90cd]"
							|| codeReport == "[Rep-IS1-92]"
							|| codeReport == "[Rep-IS1-92L]"
							)
                        {
                            newIturAnalyzes.IPValueStr5 = "R";// для суммы и группировке по всем сразу
                        }


						if (codeReport == "[Rep-IS1-85L]")
						{
							newIturAnalyzes.IPValueBit5 = false;			//нет товара в продаже
							if (productBarcodeExcludeDictionary.ContainsKey(inventProduct.Makat) == true)
							{
								newIturAnalyzes.IPValueBit5 = true;			 //Есть товар в продаже ()
							}
						}

						if (codeReport == "[Rep-IS1-93]"
							|| codeReport == "[Rep-IS1-94]")	
						{
							newIturAnalyzes.IPValueBit5 = false;
							if (string.IsNullOrWhiteSpace(inventProduct.SupplierCode) == true
							|| inventProduct.SupplierCode == inventProduct.IPValueStr8)
							{
								newIturAnalyzes.IPValueBit5 = true; //IS1-93
								newIturAnalyzes.IPValueStr18 = inventProduct.IPValueStr18;		//18Name
								//   newCurrentInventoryAdvanced.PropertyStr18Code = inventProduct.IPValueStr18;
								newIturAnalyzes.IPValueStr18 = this.GetPropertyStr(DomainObjectTypeEnum.PropertyStr18,
									inventProduct.IPValueStr18);
								newIturAnalyzes.IPValueStr19 = inventProduct.IPValueStr19;		 //19Code
							}
						}

					//	newIturAnalyzes.IPValueStr5 = inventProduct.IPValueStr5;	 //orderID  InventProductWarehouseParser 
						int ipValueInt1 = 0;
						bool ret1 = Int32.TryParse(inventProduct.IPValueStr1, out ipValueInt1);
						newIturAnalyzes.IPValueInt1 = ipValueInt1;  //Made4Net Boxes Quantity int
						double ipValueFloat1 = 0;
						bool ret2 = Double.TryParse(inventProduct.IPValueStr1, out ipValueFloat1); //Made4Net Boxes Quantity float
						newIturAnalyzes.IPValueFloat1 = ipValueFloat1;

						newIturAnalyzes.IPValueStr9 = "";

						//newIturAnalyzes.QuantityDifference =  inventProduct.QuantityEdit - inventProduct.QuantityOriginal;
						newIturAnalyzes.ValueFloat = inventProduct.QuantityOriginal - inventProduct.QuantityEdit;

						double quantityEdit = inventProduct.QuantityEdit != null ? Convert.ToDouble(inventProduct.QuantityEdit) : 0;
						double quantityOriginal = inventProduct.QuantityOriginal != null ? Convert.ToDouble(inventProduct.QuantityOriginal) : 0;
						int quantityInPackEdit = inventProduct.QuantityInPackEdit != null ? Convert.ToInt32(inventProduct.QuantityInPackEdit) : 0;

						newIturAnalyzes.QuantityEdit = quantityEdit + quantityInPackEdit;						 //??
						newIturAnalyzes.IPValueFloat5 = quantityEdit;
						newIturAnalyzes.QuantityInPackEdit = quantityInPackEdit; // inventProduct.QuantityInPackEdit;
						newIturAnalyzes.QuantityOriginal = quantityOriginal;
						newIturAnalyzes.QuantityDifference = quantityEdit - quantityOriginal;

						newIturAnalyzes.ValueBuyQriginal = 0;
						newIturAnalyzes.ValueBuyEdit = 0;
						newIturAnalyzes.ValueBuyDifference = 0;

						newIturAnalyzes.IsUpdateERP = false;
						//newIturAnalyzes.QuantityOriginal = inventProduct.QuantityOriginal;
						//newIturAnalyzes.SerialNumber = inventProduct.SerialNumber != null ? inventProduct.SerialNumber : "";
						//newIturAnalyzes.ShelfCode = inventProduct.ShelfCode != null ? inventProduct.ShelfCode : "";
						newIturAnalyzes.ModifyDate = inventProduct.ModifyDate != null ? Convert.ToDateTime(inventProduct.ModifyDate) : inventProduct.CreateDate;
						if (codeReport == "[Rep-IS1-90c]" ||
							codeReport == "[Rep-IS1-90cd]" ||
							codeReport == "[Rep-IS1-54cd]" ||
							codeReport == "[Rep-IS1-53ps]"	||
							codeReport == "[Rep-IS1-53pt]")
						{
							newIturAnalyzes.FromTime = inventProduct.CreateDate;
						}

						newIturAnalyzes.IPValueStr17 = inventProduct.CreateDate.ToString("dd-MM-yyyy");
						newIturAnalyzes.IPValueStr15 = inventProduct.CreateDate.ToString("dd") + @"/" + inventProduct.CreateDate.ToString("MM") + @"/" + inventProduct.CreateDate.ToString("yyyy"); ;		   //DD/MM/YYYY
						newIturAnalyzes.IPValueStr20 = inventProduct.CreateDate.ToString("HH:mm:ss");
						 //=Microsoft.VisualBasic.Strings.Format(Fields!FromTime.Value, "dd-MM-yyyy")
						 //=Microsoft.VisualBasic.Strings.Format(Fields!FromTime.Value, "HH:mm:ss")
					
					
						newIturAnalyzes.ProductName = inventProduct.ProductName;
						//if (newIturAnalyzes.ProductName.Contains("USB") == true)
						//{
						//	int len = newIturAnalyzes.ProductName.Length;
						//	if (len > 100)
						//	{
						//		newIturAnalyzes.ProductName = inventProduct.ProductName.Substring(0,99); 
						//	}
						//}
						newIturAnalyzes.PDA_StatusInventProductBit = inventProduct.StatusInventProductBit;
						//newIturAnalyzes.PDA_DocumentHeaderCode = inventProduct.DocumentHeaderCode;
						newIturAnalyzes.Makat = inventProduct.Makat;
						newIturAnalyzes.InputTypeCode = inventProduct.InputTypeCode;
						newIturAnalyzes.IPNum = inventProduct.IPNum;
						newIturAnalyzes.ImputTypeCodeFromPDA = inventProduct.ImputTypeCodeFromPDA;
						newIturAnalyzes.PDA_ID = inventProduct.ID;
						newIturAnalyzes.WorkerID = inventProduct.WorkerID;
						//===========Catalog =============

						newIturAnalyzes.MakatOriginal = inventProduct.Makat;			 //W 
						newIturAnalyzes.BarcodeOriginal = inventProduct.Barcode;	  //W
						newIturAnalyzes.PriceString = "";
						newIturAnalyzes.Price = 0;
						newIturAnalyzes.PriceBuy = 0;
						newIturAnalyzes.PriceSale = 0;
						newIturAnalyzes.PriceExtra = 0;
						newIturAnalyzes.CountInParentPack = 1;
						//newIturAnalyzes.ValueChar = DomainUnknownCode.UnknownFamily; //TODO Family
						newIturAnalyzes.FamilyCode = DomainUnknownCode.UnknownFamily;
						newIturAnalyzes.FamilyName = DomainUnknownName.UnknownFamily;
						newIturAnalyzes.SectionCode = DomainUnknownCode.UnknownSection;
						newIturAnalyzes.SectionName = DomainUnknownName.UnknownSection;
						//newIturAnalyzes.FromCatalogType = inventProduct.FromCatalogType;
						newIturAnalyzes.SectionNum = inventProduct.SectionNum != null ? inventProduct.SectionNum : 0;
						newIturAnalyzes.TypeCode = "";
						newIturAnalyzes.TypeMakat = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";

						newIturAnalyzes.QuantityOriginalERP = 0;
						newIturAnalyzes.BalanceQuantityPartialERP = 0;
						newIturAnalyzes.QuantityDifferenceOriginalERP = 0;
						newIturAnalyzes.ValueOriginalERP = 0;
						newIturAnalyzes.ValueDifferenceOriginalERP = 0;

						newIturAnalyzes.UnitTypeCode = "";
						newIturAnalyzes.Code = inventProduct.Makat;// +" - NotExistInCatalog";
						newIturAnalyzes.IturCodeExpected = "";
						newIturAnalyzes.IturCodeDiffer = false;
						newIturAnalyzes.FromCatalogType = (int)FromCatalogTypeEnum.Unknown;

						if (productMakatDictionary.ContainsKey(inventProduct.Makat) == true)
						{
							ProductMakat productMakat = productMakatDictionary[inventProduct.Makat];
							if (string.IsNullOrWhiteSpace(productMakat.MakatOriginal) == false) newIturAnalyzes.MakatOriginal = productMakat.MakatOriginal;
							else newIturAnalyzes.MakatOriginal = productMakat.Makat;
							//	newIturAnalyzes.MakatOriginal = productMakat.MakatOriginal;
							//newIturAnalyzes.BarcodeOriginal = "";// productMakat.MakatOriginal;//?
							newIturAnalyzes.TypeCode = productMakat.TypeCode;
						}

						if (String.IsNullOrWhiteSpace(inventProduct.Barcode) == false
							&& inventProduct.Barcode != inventProduct.Makat)
						{
							if (productMakatDictionary.ContainsKey(inventProduct.Barcode) == true)
							{
								ProductMakat productMakat = productMakatDictionary[inventProduct.Barcode];
								newIturAnalyzes.BarcodeOriginal = productMakat.MakatOriginal;
								newIturAnalyzes.TypeCode = productMakat.TypeCode;
								//double quantityBarcodeERP = productMakat.BalanceQuantityERP != null ? Convert.ToDouble(productMakat.BalanceQuantityERP) : 0;
								newIturAnalyzes.IPValueFloat1 = 0; //для Makat_Itur_BalanceQuantityERP. Report IS1-58 . Adapter - MPL
								//newIturAnalyzes.IsUpdateERP = productMakat.IsUpdateERP != null ? Convert.ToBoolean(productMakat.IsUpdateERP) : false;
							}
						}

						//для Makat_Itur_BalanceQuantityERP. Report IS1-58 . Adapter - MPL & Oren Original
						if (codeReport == "[Rep-IS1-58]" || codeReport == "[Rep-IS1-58p]" || inventorCode == "3c6dadfe-c408-4988-aacf-874692b9cf61" || isMplAdapter)
						{
							string barcodeMakatItur = inventProduct.Makat + "##" + inventProduct.IturCode;
							//newIturAnalyzes.BarcodeOriginal = barcodeMakatItur;
							newIturAnalyzes.Barcode = barcodeMakatItur;
							newIturAnalyzes.TypeCode = "B";
							newIturAnalyzes.IPValueFloat1 = 0; //для Makat_Itur_BalanceQuantityERP. Report IS1-58 . Adapter - MPL
							//if (productSimpleBarcodeOnlyDictionary.ContainsKey(barcodeMakatItur) == true)
							//{
							//	ProductSimple productSimple = productSimpleBarcodeOnlyDictionary[barcodeMakatItur];
							//	productSimpleBarcodeOnlyDictionary[inventProduct.Makat].FromCatalogTypeWithoutInventProduct = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
							//	double quantityBarcodeERP = productSimple.BalanceQuantityERP != null ? Convert.ToDouble(productSimple.BalanceQuantityERP) : 0;
							//	newIturAnalyzes.IPValueFloat1 = quantityBarcodeERP; //для Makat_Itur_BalanceQuantityERP. Report IS1-58 . Adapter - MPL
							//	//newIturAnalyzes.IsUpdateERP = productMakat.IsUpdateERP != null ? Convert.ToBoolean(productMakat.IsUpdateERP) : false;
							//}
						}// if (codeReport == "[Rep-IS1-58]" || codeReport == "[Rep-IS1-58p]")

						
						if (productSimpleDictionary.ContainsKey(inventProduct.Makat) == true)
						{
							ProductSimple productSimple = productSimpleDictionary[inventProduct.Makat];
							//double quantityOriginalERP = productSimple.BalanceQuantityERP != null ? Convert.ToDouble(productSimple.BalanceQuantityERP) : 0;
							productSimpleDictionary[inventProduct.Makat].FromCatalogTypeWithoutInventProduct = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
							newIturAnalyzes.FromCatalogType = productSimple.FromCatalogType;
							double price = 0;
							double priceBuy = productSimple.PriceBuy != null ? Convert.ToDouble(productSimple.PriceBuy) : 0;
							double priceSale = productSimple.PriceSale != null ? Convert.ToDouble(productSimple.PriceSale) : 0;
							double priceExtra = productSimple.PriceExtra != null ? Convert.ToDouble(productSimple.PriceExtra) : 0;
							price = priceCode.GetPriceByPriceCode(priceBuy, priceSale, priceExtra);
							newIturAnalyzes.Price = price;
							newIturAnalyzes.PriceBuy = priceBuy;
							newIturAnalyzes.PriceSale = priceSale;
							newIturAnalyzes.PriceExtra = priceExtra;
							newIturAnalyzes.PriceString = productSimple.PriceString;

							newIturAnalyzes.Code = productSimple.Code != null ? productSimple.Code : "";

							//if (newIturAnalyzes.Code.Contains("USB") == true)
							//{
							//	int len = newIturAnalyzes.Code.Length;
							//	if (len > 100)
							//	{
							//		newIturAnalyzes.Code = inventProduct.Code.Substring(0, 99);
							//	}
							//}
							string desc = productSimple.Description != null ? productSimple.Description : ""; // BazanCSV в каталоге много разных нестандартных полей - зедесь хранятся через запятую

							int len = desc.Length;
							if (len > 100)
							{
								desc = desc.Substring(0, 99);
							}

							newIturAnalyzes.IPValueStr9	= desc;
							newIturAnalyzes.FromCatalogType = productSimple.FromCatalogType;

							newIturAnalyzes.IPValueFloat5 = quantityEdit;
							int countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;
							newIturAnalyzes.CountInParentPack = countInParentPack;
							double quantityEditAndPartial = quantityEdit;
							if (countInParentPack != 0) quantityEditAndPartial = quantityEdit + (double)quantityInPackEdit / (double)countInParentPack;
							newIturAnalyzes.QuantityEdit = quantityEditAndPartial;
							//newIturAnalyzes.QuantityOriginal = productSimple.BalanceQuantityERP;		//??
							newIturAnalyzes.IsUpdateERP = productSimple.IsUpdateERP;
							//newIturAnalyzes.QuantityDifference = inventProduct.QuantityEdit - inventProduct.QuantityOriginal ;
							newIturAnalyzes.ValueBuyQriginal = quantityOriginal * price;
							double valueEditAndPartial = quantityEditAndPartial * price;
							newIturAnalyzes.ValueBuyEdit = valueEditAndPartial; //quantityEdit * priceBuy;

							//-------- new 23.04.2013
							double balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP;
							if (countInParentPack != 0) balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP +
								(double)productSimple.BalanceQuantityPartialERP / (double)countInParentPack;
							newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;//balanceQuantityERP;

							double valueBuyQriginalERP = price * balanceQuantityERPAndPartial;
							newIturAnalyzes.ValueOriginalERP = valueBuyQriginalERP;
							//------------- Section (From Catalog) --------------------

							string sectionCode = string.IsNullOrWhiteSpace(productSimple.SectionCode) == false ? productSimple.SectionCode : DomainUnknownCode.UnknownSection;

							newIturAnalyzes.SectionCode = sectionCode;
							if (sectionFromDBDictionary.Count > 0)
							{
								if (sectionFromDBDictionary.ContainsKey(sectionCode) == true
								&& sectionFromDBDictionary[sectionCode] != null)
								{
									Section section = sectionFromDBDictionary[sectionCode];
									newIturAnalyzes.SectionName = section.Name;
								}
							}

							if (productMakatDictionary.ContainsKey(inventProduct.Makat) == true)
							{
								ProductMakat productMakat = productMakatDictionary[inventProduct.Makat];
								string subSectionCode = string.IsNullOrWhiteSpace(productMakat.SubSectionCode) == false ? productMakat.SubSectionCode : "";
								newIturAnalyzes.SubSessionCode = subSectionCode;   //SubSessionCode значит subSectionCode
							}
							

							//------------- Family (From Catalog) --------------------
							string familyCode = string.IsNullOrWhiteSpace(productSimple.FamilyCode) == false ? productSimple.FamilyCode : DomainUnknownCode.UnknownFamily;

							//newIturAnalyzes.ValueChar = familyCode; // TODO см отчет с Family and ValueChar <=> FamilyCode
							newIturAnalyzes.FamilyCode = familyCode;
							if (familyFromDBDictionary.Count > 0)
							{
								if (familyFromDBDictionary.ContainsKey(familyCode) == true
								&& familyFromDBDictionary[familyCode] != null)
								{
									Family family = familyFromDBDictionary[familyCode];
									newIturAnalyzes.FamilyName = family.Name;
									newIturAnalyzes.FamilySize = family.Size;
									newIturAnalyzes.FamilyType = family.Type;
									newIturAnalyzes.FamilyExtra1 = family.Extra1;
									newIturAnalyzes.FamilyExtra2 = family.Extra2;
								}
							}

							newIturAnalyzes.UnitTypeCode = productSimple.UnitTypeCode != null ? productSimple.UnitTypeCode : "";

							newIturAnalyzes.IturCodeExpected = productSimple.IturCodeExpected;


							//newIturAnalyzes.ValueOriginalERP = quantityOriginalERP * priceBuy;
							//newIturAnalyzes.QuantityOriginalERP = quantityOriginalERP;
							//newIturAnalyzes.QuantityDifferenceOriginalERP = quantityEdit- quantityOriginalERP;
							//newIturAnalyzes.ValueDifferenceOriginalERP = 0;
						}


						//if (fromBuildAnalyze == "BuildAnalyzeTableSimple" || codeReport == "[Rep-IS1-63]")
						if (includeProductMakatWithoutInventProduct == true )	
						{
							if (codeReport != "[Rep-IS1-62n]"
								|| codeReport != "[Rep-IS1-62np]")	  
							{
								if (productBarcodeSimpleDictionary.ContainsKey(inventProduct.Barcode) == true)
								{
									productBarcodeSimpleDictionary[inventProduct.Barcode].FromCatalogTypeWithoutInventProduct = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
									ProductMakat productSimple = productMakatDictionary[inventProduct.Barcode];
									if (productSimple.BalanceQuantityERP == null) productSimple.BalanceQuantityERP = 0.0;

									if (productSimple.BalanceQuantityERP != 0.0)
									{
										double quantityOriginalERP = Convert.ToDouble(productSimple.BalanceQuantityERP);

										newIturAnalyzes.QuantityOriginalERP = quantityOriginalERP;
										newIturAnalyzes.QuantityDifferenceOriginalERP = quantityEdit - quantityOriginalERP;

										newIturAnalyzes.ValueOriginalERP = quantityOriginalERP * newIturAnalyzes.Price;
										newIturAnalyzes.ValueBuyEdit = quantityEdit * newIturAnalyzes.Price;
										newIturAnalyzes.ValueDifferenceOriginalERP = newIturAnalyzes.ValueBuyEdit - newIturAnalyzes.ValueOriginalERP;
									}
								}
							}
						}

						
						//===================== Doc ========================================
						if (documentHeaderDictionary.ContainsKey(inventProduct.DocumentCode) == true)
						{
							DocumentHeader docHeader = documentHeaderDictionary[inventProduct.DocumentCode];
							string iturCode = docHeader.IturCode != null ? docHeader.IturCode : "";
							//newIturAnalyzes.Code = iturCode;
							//newIturAnalyzes.Doc_Name = docHeader.Name != null ? docHeader.Name : "";		  //пока StatusInventProductCode пишем
							newIturAnalyzes.Doc_Approve = docHeader.Approve != null ? docHeader.Approve : false;
							//newIturAnalyzes.Doc_IturCode = docHeader.IturCode;
							newIturAnalyzes.Doc_Name = docHeader.Name != null ? docHeader.Name : "";
							newIturAnalyzes.Doc_WorkerGUID = docHeader.WorkerGUID != null ? docHeader.WorkerGUID : "";
							newIturAnalyzes.Doc_StatusDocHeaderBit = docHeader.StatusDocHeaderBit;
							newIturAnalyzes.Doc_StatusInventProductBit = docHeader.StatusInventProductBit;
							newIturAnalyzes.Doc_StatusApproveBit = docHeader.StatusApproveBit;
							newIturAnalyzes.DocNum = (int)docHeader.ID;
							if (codeReport == "[Rep-IS1-58dt]")
							{
								newIturAnalyzes.FromTime = docHeader.CreateDate;
							}
							newIturAnalyzes.IPValueStr19 = docHeader.CreateDate.ToString();
							//==================== Itur ==================================
							if (iturDictionary.ContainsKey(iturCode) == true
								&& iturDictionary[iturCode] != null)
							{
								Itur itur = iturDictionary[iturCode];
								string locationCode = itur.LocationCode != null ? itur.LocationCode : DomainUnknownCode.UnknownLocation;
								newIturAnalyzes.ERPIturCode = itur.ERPIturCode;
								if (codeReport == "[Rep-IS1-59]" || codeReport == "[Rep-IS1-59p]"
									|| codeReport == "[Rep-IS1-69]" || codeReport == "[Rep-IS1-69p]")
								{
								 	newIturAnalyzes.IPValueStr5 = itur.ERPIturCode != "" ? itur.ERPIturCode : iturCode;
								}
								newIturAnalyzes.IturName = itur.Name;
								newIturAnalyzes.LocationCode = locationCode;
								newIturAnalyzes.LocationName = locationCode;
								newIturAnalyzes.Itur_Disabled = itur.Disabled != null ? itur.Disabled : false;
								newIturAnalyzes.Itur_Publishe = itur.Publishe != null ? itur.Publishe : false;
								newIturAnalyzes.Itur_StatusIturBit = itur.StatusIturBit != null ? itur.StatusIturBit : 0;
								newIturAnalyzes.Itur_Number = itur.Number != null ? itur.Number : 0;
								newIturAnalyzes.Itur_NumberPrefix = itur.NumberPrefix != null ? itur.NumberPrefix : "";
								newIturAnalyzes.Itur_NumberSufix = itur.NumberSufix != null ? itur.NumberSufix : "";
								newIturAnalyzes.Itur_LocationCode = locationCode;
								newIturAnalyzes.Itur_StatusIturGroupBit = itur.StatusIturGroupBit != null ? itur.StatusIturGroupBit : 0;
								newIturAnalyzes.Itur_StatusDocHeaderBit = itur.StatusDocHeaderBit != null ? itur.StatusDocHeaderBit : 0;
								//newIturAnalyzes.SectionCode = DomainUnknownCode.UnknownSection;
								if (locationDictionary.ContainsKey(locationCode) == true
								&& locationDictionary[locationCode] != null)
								{
									Location location = locationDictionary[locationCode];
									newIturAnalyzes.LocationName = location.Name;
									newIturAnalyzes.ValueChar = location.Tag;
								}

								if (createDateTime != "InventProductCreateDateTime")
								{
									newIturAnalyzes.ModifyDate = DateTime.Now;
									if (iturDictionaryMaxDateTime.ContainsKey(iturCode) == true
									&& iturDictionaryMaxDateTime[iturCode] != null)
									{
										DocumentHeader docMaxDate = iturDictionaryMaxDateTime[iturCode];
										newIturAnalyzes.ModifyDate = docMaxDate.CreateDate;
									}
								}

							}
							else
							{
								newIturAnalyzes.ERPIturCode = "";
								newIturAnalyzes.LocationCode = DomainUnknownCode.UnknownLocation;
								newIturAnalyzes.Itur_Disabled = false;
								newIturAnalyzes.Itur_Publishe = false;
								newIturAnalyzes.Itur_StatusIturBit = 0;
								newIturAnalyzes.Itur_Number = 0;
								newIturAnalyzes.Itur_NumberPrefix = "";
								newIturAnalyzes.Itur_NumberSufix = "";
								newIturAnalyzes.Itur_LocationCode = DomainUnknownCode.UnknownLocation;
								//newIturAnalyzes.SectionCode = DomainUnknownCode.UnknownSection;
								newIturAnalyzes.Itur_StatusIturGroupBit = 0;
								newIturAnalyzes.Itur_StatusDocHeaderBit = 0;
								newIturAnalyzes.ModifyDate = DateTime.Now;
								newIturAnalyzes.LocationName = DomainUnknownName.UnknownLocation;
							}
						}
						else
						{
							newIturAnalyzes.Code = "None";
							newIturAnalyzes.Doc_Name = "";
							newIturAnalyzes.Doc_Approve = false;
							//newIturAnalyzes.Doc_IturCode = "";
							newIturAnalyzes.Doc_WorkerGUID = "";
							newIturAnalyzes.Doc_StatusDocHeaderBit = 0;
							newIturAnalyzes.Doc_StatusInventProductBit = 0;
							newIturAnalyzes.Doc_StatusApproveBit = 0;
							newIturAnalyzes.DocNum = 0;
							newIturAnalyzes.ERPIturCode = "";
							newIturAnalyzes.LocationCode = DomainUnknownCode.UnknownLocation;
							newIturAnalyzes.Itur_Disabled = false;
							newIturAnalyzes.Itur_Publishe = false;
							newIturAnalyzes.Itur_StatusIturBit = 0;
							newIturAnalyzes.Itur_Number = 0;
							newIturAnalyzes.Itur_NumberPrefix = "";
							newIturAnalyzes.Itur_NumberSufix = "";
							newIturAnalyzes.ModifyDate = DateTime.Now;
							newIturAnalyzes.Itur_LocationCode = DomainUnknownCode.UnknownLocation;
							newIturAnalyzes.Itur_StatusIturGroupBit = 0;
							newIturAnalyzes.Itur_StatusDocHeaderBit = 0;
							//newIturAnalyzes.SectionCode =DomainUnknownCode.UnknownSection;
							newIturAnalyzes.LocationName = DomainUnknownName.UnknownLocation;
						}
					}
					catch (Exception error)
					{
						_logger.ErrorException("GetIturAnalyzes", error);
						this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
					}

					if (string.IsNullOrWhiteSpace(newIturAnalyzes.IturCodeExpected) != true)
					{
						if (newIturAnalyzes.IturCodeExpected != "0" && newIturAnalyzes.ERPIturCode != newIturAnalyzes.IturCodeExpected)
						{
							newIturAnalyzes.IturCodeDiffer = true;
						}
					}

					newIturAnalyzes.ResultCode = "XXX";
					newIturAnalyzes.IsUpdateERP = false;

					yield return newIturAnalyzes;
				}  //foreach
			}//if (inventProductList != null)

			inventProductList = null;
			GC.Collect();

			//ProductMakatWithoutInventProduct //29.10.2015
			//для Makat_Itur_BalanceQuantityERP. Report IS1-58 . Adapter - MPL
			// только баркоды == Makat_IturCode


			if (codeReport == "[Rep-IS1-58]" || codeReport == "[Rep-IS1-58p]" || inventorCode == "3c6dadfe-c408-4988-aacf-874692b9cf61" || isMplAdapter) //Adapter - MPL & Oren Original
			{
				//Dictionary<string, ProductSimple> productSimpleBarcodeOnlyDictionary = new Dictionary<string, ProductSimple>();
				//productSimpleBarcodeOnlyDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, true, TypeMakatEnum.B.ToString()); //"B"
	
				foreach (KeyValuePair<string, ProductSimple> keyValuePair in productSimpleBarcodeOnlyDictionary) // "B"
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
					ProductSimple productSimple = keyValuePair.Value;


					//string barcodeMakatItur = inventProduct.Makat + "##" + inventProduct.IturCode;
					//newIturAnalyzes.Barcode = barcodeMakatItur;
					//newIturAnalyzes.TypeCode = "B";
					//newIturAnalyzes.IPValueFloat1 = 0; 

					if (productSimple.BalanceQuantityERP != 0.0
					&& productSimple.Makat.Contains("##") == true)
					{
						k++;
						IturAnalyzes newIturAnalyzes = new IturAnalyzes();
						try
						{
							//===================Inventor======================================
							newIturAnalyzes.InventorCode = inventorCode;
							newIturAnalyzes.InventorName = inventorName;
							string barcodeMakatItur = productSimple.ParentMakat + "##";
							newIturAnalyzes.BarcodeOriginal = productSimple.MakatOriginal;
							newIturAnalyzes.Barcode = productSimple.Makat;
							newIturAnalyzes.Makat = productSimple.ParentMakat;
							newIturAnalyzes.TypeCode = "B";
							newIturAnalyzes.TypeMakat = TypeMakatEnum.B.ToString();
							newIturAnalyzes.InputTypeCode = "B";
							newIturAnalyzes.ProductName = productSimple.Name;
							double quantityBarcodeERP = productSimple.BalanceQuantityERP != null ? Convert.ToDouble(productSimple.BalanceQuantityERP) : 0;
							newIturAnalyzes.IPValueFloat1 = quantityBarcodeERP; //для Makat_Itur_BalanceQuantityERP. Report IS1-58 . Adapter - MPL
							string iturCode = productSimple.Makat.Replace(barcodeMakatItur, "");
							newIturAnalyzes.IturCode = iturCode;
							newIturAnalyzes.ERPIturCode = iturCode;
							newIturAnalyzes.QuantityEdit = 0;
							if (productMakatDictionary.ContainsKey(newIturAnalyzes.Makat) == true)
							{
								ProductMakat productMakat = productMakatDictionary[newIturAnalyzes.Makat];
								newIturAnalyzes.ProductName = productMakat.Name;
							}
			

							Itur itur = iturDictionary[iturCode];
							if (itur != null)
							{
								string locationCode0 = itur.LocationCode != null ? itur.LocationCode : DomainUnknownCode.UnknownLocation;
								newIturAnalyzes.ERPIturCode = itur.ERPIturCode;
								newIturAnalyzes.IturName = itur.Name;
								newIturAnalyzes.LocationCode = locationCode0;
								newIturAnalyzes.LocationName = locationCode0;
								if (locationDictionary.ContainsKey(locationCode0) == true
									&& locationDictionary[locationCode0] != null)
								{
									Location location = locationDictionary[locationCode0];
									newIturAnalyzes.LocationName = location.Name;
									newIturAnalyzes.ValueChar = location.Tag;
								}
								newIturAnalyzes.IsUpdateERP = false;
							}
						
							//}// if (codeReport == "[Rep-IS1-58]" || codeReport == "[Rep-IS1-58p]")
					
						}
						catch (Exception error)
						{
							_logger.ErrorException("GerIturAnalyzes", error);
							this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
						}
						yield return newIturAnalyzes;
					}
				} // end ProductMakatWithoutInventProduct // add 29.10.2015 copy from IturAnalyzesSimpleSumReader

				productSimpleBarcodeOnlyDictionary.Clear();
				GC.Collect();
				GC.Collect();
			}//if (codeReport == "[Rep-IS1-58]" || codeReport == "[Rep-IS1-58p]")  //Adapter - MPL & Oren Original



			//========================================	 NEW 17.10.2017
			//ProductMakatWithoutInventProduct
			//if (fromBuildAnalyze == "BuildAnalyzeTableSimple" || codeReport == "[Rep-IS1-63]")
			if (includeProductMakatWithoutInventProduct == true)
			{
		
				foreach (KeyValuePair<string, ProductSimple> keyValuePair in productBarcodeSimpleDictionary)
				{
					if (cancellationToken.IsCancellationRequested == true)
					{
						break;
					}
						ProductSimple productSimpleWithBarcode = keyValuePair.Value;
					if (productSimpleWithBarcode.FromCatalogTypeWithoutInventProduct == (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct)
					{
						string makat = productSimpleWithBarcode.ParentMakat;
						string barcode = productSimpleWithBarcode.Makat;
						if (codeReport == "[Rep-IS1-62n]"
								|| codeReport == "[Rep-IS1-62np]")
						{
							makat = productSimpleWithBarcode.Makat;
							barcode = "";
						}
						ProductSimple productMakat 	= new ProductSimple();
						if (productSimpleDictionary.ContainsKey(makat) == true)
						{
							productMakat = productSimpleDictionary[makat];
						}

						if (productSimpleWithBarcode.BalanceQuantityERP == null) productSimpleWithBarcode.BalanceQuantityERP = 0.0;
						if (productSimpleWithBarcode.BalanceQuantityERP != 0.0	)
							//|| includeProductMakatWithoutInventProduct == true)		  // большой вопрос пока так . машу - машу
						{
							k++;
							IturAnalyzes newIturAnalyzes = new IturAnalyzes();
							try
							{
								//===================Inventor======================================
								newIturAnalyzes.InventorCode = inventorCode;
								newIturAnalyzes.InventorName = inventorName;
								//===================================================================
								string sectionCode = string.IsNullOrWhiteSpace(productMakat.SectionCode) == false ? productMakat.SectionCode : DomainUnknownCode.UnknownSection;
								string supplierCode = string.IsNullOrWhiteSpace(productMakat.SupplierCode) == false ? productMakat.SupplierCode : DomainUnknownCode.UnknownSupplier;
								string familyCode = string.IsNullOrWhiteSpace(productMakat.FamilyCode) == false ? productMakat.FamilyCode : DomainUnknownCode.UnknownFamily;
								newIturAnalyzes.FamilyCode = familyCode;
								newIturAnalyzes.SectionCode = sectionCode;
								newIturAnalyzes.SupplierCode = supplierCode;
								string sectionName = sectionCode;
								string supplierName = supplierCode;

								newIturAnalyzes.UnitTypeCode = productMakat.UnitTypeCode != null ? productMakat.UnitTypeCode : "";

								newIturAnalyzes.TypeCode = productMakat.TypeCode;
								newIturAnalyzes.TypeMakat = TypeMakatEnum.W.ToString();	 //??
								newIturAnalyzes.InputTypeCode = "B";

								double price = 0;
								double priceBuy = productMakat.PriceBuy != null ? Convert.ToDouble(productMakat.PriceBuy) : 0;
								double priceSale = productMakat.PriceSale != null ? Convert.ToDouble(productMakat.PriceSale) : 0;
								double priceExtra = productMakat.PriceExtra != null ? Convert.ToDouble(productMakat.PriceExtra) : 0;
								price = priceCode.GetPriceByPriceCode(priceBuy, priceSale, priceExtra);
							//	int countInParentPack = productMakat.CountInParentPack == 0 ? 1 : productMakat.CountInParentPack;

								newIturAnalyzes.QuantityOriginalERP = 0;
								newIturAnalyzes.FromCatalogType = (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct;
								newIturAnalyzes.BalanceQuantityPartialERP = productSimpleWithBarcode.BalanceQuantityPartialERP;
								newIturAnalyzes.QuantityEdit = 0;
								newIturAnalyzes.QuantityInPackEdit = 0;

								newIturAnalyzes.QuantityDifferenceOriginalERP = 0;
								//newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;//balanceQuantityERP;
								//newIturAnalyzes.QuantityDifferenceOriginalERP = 0 - balanceQuantityERPAndPartial;//balanceQuantityERP;

								//===
								newIturAnalyzes.QuantityDifference = 0;// quantityEdit;
								newIturAnalyzes.QuantityOriginal = 0;	  //- quantityOriginal
								//=====
								newIturAnalyzes.Makat = makat;
								newIturAnalyzes.Barcode = barcode;
								if (string.IsNullOrWhiteSpace(productSimpleWithBarcode.MakatOriginal) == false) newIturAnalyzes.BarcodeOriginal = productSimpleWithBarcode.MakatOriginal;
								else newIturAnalyzes.BarcodeOriginal = barcode;
								newIturAnalyzes.MakatOriginal = productMakat.MakatOriginal;
								newIturAnalyzes.Count = 1;
								newIturAnalyzes.Price = price;
								newIturAnalyzes.PriceBuy = priceBuy;
								newIturAnalyzes.PriceSale = priceSale;
								newIturAnalyzes.PriceExtra = priceExtra;
								newIturAnalyzes.ValueOriginalERP = 0;
								//newIturAnalyzes.ValueOriginalERP = balanceQuantityERPAndPartial * price;//balanceQuantityERP * priceBuy;
								newIturAnalyzes.ValueBuyEdit = 0;
								newIturAnalyzes.ValueDifferenceOriginalERP = 0;
								//newIturAnalyzes.ValueDifferenceOriginalERP = 0 - balanceQuantityERPAndPartial * price;//0 - balanceQuantityERP * priceBuy;
								//===
								newIturAnalyzes.ValueBuyQriginal = 0;
								newIturAnalyzes.ValueBuyDifference = 0;
								//===
								//newIturAnalyzes.SectionCode = sectionCode;
								newIturAnalyzes.ProductName = productMakat.Name;
								//newIturAnalyzes.MakatOriginal = productSimple.MakatOriginal;
								newIturAnalyzes.ResultCode = "XXX";
								newIturAnalyzes.IsUpdateERP = false;

								if (productSimpleWithBarcode.BalanceQuantityERP != 0.0)
								{
									double balanceQuantityERPAndPartial = productSimpleWithBarcode.BalanceQuantityERP;	   //??
									//if (countInParentPack != 0) balanceQuantityERPAndPartial = productSimpleWithBarcode.BalanceQuantityERP +
									//	productSimpleWithBarcode.BalanceQuantityPartialERP / countInParentPack;
									//newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;

									newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;//balanceQuantityERP;
									newIturAnalyzes.QuantityDifferenceOriginalERP = 0 - balanceQuantityERPAndPartial;//balanceQuantityERP;
									newIturAnalyzes.ValueOriginalERP = balanceQuantityERPAndPartial * price;//balanceQuantityERP * priceBuy;
									newIturAnalyzes.ValueDifferenceOriginalERP = 0 - balanceQuantityERPAndPartial * price;//0 - balanceQuantityERP * priceBuy;

								}
							}

							catch (Exception error)
							{
								_logger.ErrorException("GerIturAnalyzes", error);
								this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
							}
							yield return newIturAnalyzes;
						}
					}

				} // end ProductMakatWithoutInventProduct

			}
			//=========================================	END   NEW 17.10.2017
			
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

		private void CountLong(long count)
		{
		}

	}
}
