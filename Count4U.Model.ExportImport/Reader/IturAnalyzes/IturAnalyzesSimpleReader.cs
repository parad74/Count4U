using System;
using System.Collections.Generic;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.SelectionParams;
using NLog;
using System.Threading;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;

namespace Count4U.Model.Count4U
{
	public class IturAnalyzesSimpleReader : BaseADORepository, IIturAnalyzesReader
	{
		private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public IturAnalyzesSimpleReader(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository)
			: base(connection, dbSettings,log, serviceLocator)
        {
			this._makatRepository = makatRepository;
		
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
			if (refill == true)
			{
				base.ClearIturAnalyzes(pathDB);
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

			IIturAnalyzesRepository iturAnalyzesRepository =
				this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			//Dictionary<string, Itur> iturDictionary = iturAnalyzesRepository.GetIturDictionary(pathDB, refill);
			//Dictionary<string, DocumentHeader> documentHeaderDictionary = iturAnalyzesRepository.GetDocumentHeaderDictionary(pathDB, refill);
			//Dictionary<string, ProductMakat> productMakatDictionary = iturAnalyzesRepository.GetProductMakatDictionary(pathDB, refill);
			//bool refillCatalogDictionary = false;
			Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refillCatalogDictionary);

			this.Log.Add(MessageTypeEnum.TraceRepository, "GetIturAnalyzes");

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
				IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();

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

			try
			{
				inventProductList = iturAnalyzesRepository.
					GetInventProductList(selectParms, pathDB);
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
					IturAnalyzes newIturAnalyzes = new IturAnalyzes();
					try
					{
						//===================Inventor======================================
						newIturAnalyzes.InventorCode = inventorCode;
						newIturAnalyzes.InventorName = inventorName;
						//==================== PDA =========================================
						newIturAnalyzes.SectionCode = DomainUnknownCode.UnknownSection;
						newIturAnalyzes.DocumentCode = inventProduct.DocumentCode;
						newIturAnalyzes.IturCode = inventProduct.IturCode;
						//newIturAnalyzes.ERPIturCode = "";		//TODO ?
						//newIturAnalyzes.LocationCode = locationCode;
						newIturAnalyzes.Barcode = inventProduct.Barcode;
						newIturAnalyzes.Doc_Name = inventProduct.StatusInventProductCode != null ? inventProduct.StatusInventProductCode : "";
						newIturAnalyzes.TypeMakat = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";

						double quantityEdit = inventProduct.QuantityEdit != null ? Convert.ToDouble(inventProduct.QuantityEdit) : 0;
						double quantityOriginal = inventProduct.QuantityOriginal != null ? Convert.ToDouble(inventProduct.QuantityOriginal) : 0;
						int quantityInPackEdit = inventProduct.QuantityInPackEdit != null ? Convert.ToInt32(inventProduct.QuantityInPackEdit) : 0;

						newIturAnalyzes.QuantityEdit = quantityEdit + quantityInPackEdit;
						newIturAnalyzes.IPValueFloat5 = quantityEdit;
						newIturAnalyzes.QuantityInPackEdit = quantityInPackEdit;
						newIturAnalyzes.QuantityDifference = quantityEdit - quantityOriginal;
						newIturAnalyzes.QuantityOriginal = quantityOriginal;
						//newIturAnalyzes.SerialNumber = inventProduct.SerialNumber != null ? inventProduct.SerialNumber : "";
						//newIturAnalyzes.ShelfCode = inventProduct.ShelfCode != null ? inventProduct.ShelfCode : "";
						//newIturAnalyzes.ModifyDate = inventProduct.ModifyDate;
						newIturAnalyzes.ProductName = inventProduct.ProductName;
						//newIturAnalyzes.PDA_StatusInventProductBit = inventProduct.StatusInventProductBit;
						//newIturAnalyzes.PDA_DocumentHeaderCode = inventProduct.DocumentHeaderCode;
						newIturAnalyzes.Makat = inventProduct.Makat;
						//newIturAnalyzes.MakatOriginal = inventProduct.Makat;
						newIturAnalyzes.InputTypeCode = inventProduct.InputTypeCode;
						newIturAnalyzes.IPNum = inventProduct.IPNum;
						//newIturAnalyzes.PDA_ID = inventProduct.ID;
						//===========Catalog =============

						//newIturAnalyzes.MakatOriginal = inventProduct.Makat;			 //W 
						//newIturAnalyzes.BarcodeOriginal = inventProduct.Barcode;	  //W
						//newIturAnalyzes.PriceString = "";
						newIturAnalyzes.Price = 0;
						newIturAnalyzes.PriceBuy = 0;
						newIturAnalyzes.PriceSale = 0;
						newIturAnalyzes.PriceExtra = 0;
						newIturAnalyzes.ValueBuyQriginal = 0;
						newIturAnalyzes.ValueBuyEdit = 0;
						newIturAnalyzes.ValueBuyDifference = 0;
						newIturAnalyzes.FromCatalogType = inventProduct.FromCatalogType;
						//newIturAnalyzes.SectionNum = inventProduct.SectionNum != null ? inventProduct.SectionNum : 0;
						//newIturAnalyzes.TypeCode = "";
						newIturAnalyzes.TypeMakat = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";
						newIturAnalyzes.Code = inventProduct.Makat;// +" - NotExistInCatalog";
						//if (productMakatDictionary.ContainsKey(inventProduct.Makat) == true)
						//{
						//    ProductMakat productMakat = productMakatDictionary[inventProduct.Makat];
						//    newIturAnalyzes.MakatOriginal = productMakat.MakatOriginal;
						//    newIturAnalyzes.BarcodeOriginal = productMakat.MakatOriginal;//?
						//   newIturAnalyzes.TypeCode = productMakat.TypeCode;
						//}
						//if (inventProduct.Barcode != inventProduct.Makat)
						//{
						//    if (productMakatDictionary.ContainsKey(inventProduct.Barcode) == true)
						//    {
						//        ProductMakat productMakat = productMakatDictionary[inventProduct.Barcode];
						//        newIturAnalyzes.BarcodeOriginal = productMakat.MakatOriginal;
						//        newIturAnalyzes.TypeCode = productMakat.TypeCode;
						//    }
						//}
						if (productSimpleDictionary.ContainsKey(inventProduct.Makat) == true)
						{
							ProductSimple productSimple = productSimpleDictionary[inventProduct.Makat];
							newIturAnalyzes.FromCatalogType = productSimple.FromCatalogType;
							//newIturAnalyzes.PriceString = productSimple.PriceString;
							double price = 0;
							double priceBuy = productSimple.PriceBuy != null ? Convert.ToDouble(productSimple.PriceBuy) : 0;
							double priceSale = productSimple.PriceSale != null ? Convert.ToDouble(productSimple.PriceSale) : 0;
							double priceExtra = productSimple.PriceExtra != null ? Convert.ToDouble(productSimple.PriceExtra) : 0;
							price = priceCode.GetPriceByPriceCode(priceBuy, priceSale, priceExtra);

							newIturAnalyzes.PriceBuy = priceBuy;
							newIturAnalyzes.PriceSale = priceSale;
							newIturAnalyzes.PriceExtra = priceExtra;
							newIturAnalyzes.Price = price;

							newIturAnalyzes.Code = productSimple.Code != null ? productSimple.Code : "";

							//newIturAnalyzes.ValueBuyQriginal = productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityOriginal);
							int countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;
							double quantityEditAndPartial = quantityEdit;
							if (countInParentPack != 0) quantityEditAndPartial = quantityEdit + (double)quantityInPackEdit / (double)countInParentPack;
							newIturAnalyzes.QuantityEdit = quantityEditAndPartial;
							newIturAnalyzes.ValueBuyEdit = price * quantityEditAndPartial;// productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityEdit);

							newIturAnalyzes.QuantityDifference = quantityEditAndPartial - quantityOriginal;
							//inventProduct.QuantityEdit - inventProduct.QuantityOriginal;
							//newIturAnalyzes.ValueBuyDifference = productSimple.PriceBuy *
							//    (Convert.ToDouble(inventProduct.QuantityOriginal) - Convert.ToDouble(inventProduct.QuantityEdit));
						}

						//===================== Doc ========================================
						//if (documentHeaderDictionary.ContainsKey(inventProduct.DocumentCode) == true)
						//{
						//    DocumentHeader docHeader = documentHeaderDictionary[inventProduct.DocumentCode];
						//   newIturAnalyzes.Code = docHeader.IturCode;
						//    newIturAnalyzes.Doc_Name = docHeader.Name != null ? docHeader.Name : "";
						//   newIturAnalyzes.Doc_Approve = docHeader.Approve != null ? docHeader.Approve : false;
						//    //newIturAnalyzes.Doc_IturCode = docHeader.IturCode;
						//   newIturAnalyzes.Doc_WorkerGUID = docHeader.WorkerGUID != null ? docHeader.WorkerGUID : "";
						//    newIturAnalyzes.Doc_StatusDocHeaderBit = docHeader.StatusDocHeaderBit;
						//    newIturAnalyzes.Doc_StatusInventProductBit = docHeader.StatusInventProductBit;
						//    newIturAnalyzes.Doc_StatusApproveBit = docHeader.StatusApproveBit;
						//    newIturAnalyzes.DocNum = docHeader.ID;
						//==================== Itur ==================================
						//if (iturDictionary.ContainsKey(docHeader.IturCode) == true)
						//{
						//    Itur itur = iturDictionary[docHeader.IturCode];
						//    newIturAnalyzes.LocationCode = itur.LocationCode;
						//    newIturAnalyzes.Itur_Disabled = itur.Disabled != null ? itur.Disabled : false;
						//    newIturAnalyzes.Itur_Publishe = itur.Publishe != null ? itur.Publishe : false;
						//    newIturAnalyzes.Itur_StatusIturBit = itur.StatusIturBit;
						//    newIturAnalyzes.Itur_Number = itur.Number;
						//    newIturAnalyzes.Itur_NumberPrefix = itur.NumberPrefix;
						//    newIturAnalyzes.Itur_NumberSufix = itur.NumberSufix;
						//    newIturAnalyzes.Itur_LocationCode = itur.LocationCode;
						//    newIturAnalyzes.Itur_StatusIturGroupBit = itur.StatusIturGroupBit;
						//    newIturAnalyzes.Itur_StatusDocHeaderBit = itur.StatusDocHeaderBit;
						//}
						//}
						//else
						//{
						//    newIturAnalyzes.Code = "";
						//    newIturAnalyzes.Doc_Name = "";
						//    newIturAnalyzes.Doc_Approve = false;
						//    //newIturAnalyzes.Doc_IturCode = "";
						//    newIturAnalyzes.Doc_WorkerGUID = "";
						//    newIturAnalyzes.Doc_StatusDocHeaderBit = 0;
						//    newIturAnalyzes.Doc_StatusInventProductBit = 0;
						//    newIturAnalyzes.Doc_StatusApproveBit = 0;
						//   newIturAnalyzes.DocNum = 0;
						//    newIturAnalyzes.LocationCode = "";
						//    newIturAnalyzes.Itur_Disabled = false;
						//    newIturAnalyzes.Itur_Publishe = false;
						//   newIturAnalyzes.Itur_StatusIturBit = 0;
						//   newIturAnalyzes.Itur_Number = 0;
						//    newIturAnalyzes.Itur_NumberPrefix = "";
						//    newIturAnalyzes.Itur_NumberSufix = "";
						//    newIturAnalyzes.Itur_LocationCode = "";
						//    newIturAnalyzes.Itur_StatusIturGroupBit = 0;
						//    newIturAnalyzes.Itur_StatusDocHeaderBit = 0;
						//}
					}
					catch (Exception error)
					{
						_logger.ErrorException("InsertIturAnalyzesSimple", error);
						this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
					}
					yield return newIturAnalyzes;
				}		 //foreach
			}
		}

		private void CountLong(long count)
		{
		}

	}
}
