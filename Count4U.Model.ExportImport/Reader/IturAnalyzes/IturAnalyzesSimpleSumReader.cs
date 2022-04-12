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
	// SumQuantityEditDifferenceByMakats 
	// not Difference by Iturs, not Difference By Locations
	public class IturAnalyzesSimpleSumReader : BaseADORepository, IIturAnalyzesReader
	{
		private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public IturAnalyzesSimpleSumReader(
			IConnectionADO connection,
			IDBSettings dbSettings,
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository)
			: base(connection, dbSettings, log, serviceLocator)
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
			string createDateTime = "";
			string codeReport = "";

			Dictionary<ImportProviderParmEnum, object> parms = new Dictionary<ImportProviderParmEnum, object>();
			if (parmsIn != null)
			{
				parms = parmsIn.ConvertToImportProviderParmEnum();
				cancellationToken = parms.GetCancellationTokenFromParm();
				priceCode = parms.GetPriceCodeEnumFromParm();
				codeReport = parms.GetStringValueFromParm(ImportProviderParmEnum.CodeReport);
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

			bool includeProductMakatWithoutInventProduct = false;
			if (importType != null)
			{
				if (importType.Contains(ImportDomainEnum.IncludeProductMakatWithoutInventProduct) == true) includeProductMakatWithoutInventProduct = true;
			}

			Stopwatch stopwatch;
			IIturAnalyzesRepository iturAnalyzesRepository = this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			IInventProductRepository inventProductRepository = this._serviceLocator.GetInstance<IInventProductRepository>();
			ISectionRepository sectionRepository = this._serviceLocator.GetInstance<ISectionRepository>();
			ISupplierRepository supplierRepository = this._serviceLocator.GetInstance<ISupplierRepository>();
			IFamilyRepository familyRepository = this._serviceLocator.GetInstance<IFamilyRepository>();

			Dictionary<string, Section> sectionFromDBDictionary = sectionRepository.GetSectionDictionary(pathDB, true);
			Dictionary<string, Supplier> supplierFromDBDictionary = supplierRepository.GetSupplierDictionary(pathDB, true);
			Dictionary<string, Family> familyFromDBDictionary = familyRepository.GetFamilyDictionary(pathDB, true);

			//Dictionary<string, Itur> iturDictionary = iturAnalyzesRepository.GetIturDictionary(pathDB, refill);
			//Dictionary<string, DocumentHeader> documentHeaderDictionary = iturAnalyzesRepository.GetDocumentHeaderDictionary(pathDB, refill);
			//Dictionary<string, ProductMakat> productMakatDictionary = iturAnalyzesRepository.GetProductMakatDictionary(pathDB, refill);

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

			//if (selectParms.FilterStringListParams.ContainsKey("SectionCode") == true)
			//{
				
			//}

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

			stopwatch = Stopwatch.StartNew();
			IEnumerable<IturAnalyzesSimple> inventProductList = null;
			try
			{
				//inventProductList = iturAnalyzesRepository.GetIPSumQuantityByMakats(selectParms, pathDB, true);  test
				inventProductList = iturAnalyzesRepository.GetIPSumQuantityEditDifferenceByMakats(selectParms, pathDB);
			}
			catch { }
			stopwatch.Stop();
			System.Diagnostics.Debug.Print(String.Format("GetIPSumQuantityEditDifferenceByMakats: {0}", stopwatch.ElapsedTicks.ToString()));


			stopwatch = Stopwatch.StartNew();
			//bool refillCatalogDictionary = false;
			Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refillCatalogDictionary);
			base.ClearIturAnalyzes(pathDB);
			stopwatch.Stop();
			System.Diagnostics.Debug.Print(String.Format("GetProductSimpleDictionary: {0}", stopwatch.ElapsedTicks.ToString()));


			int k = 0;

			stopwatch = Stopwatch.StartNew();

			if (inventProductList != null)
			{
				//все суммы IP 

				foreach (IturAnalyzesSimple inventProduct in inventProductList)
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
						//==================== SumQuantityByMakats =========================================
						//newIturAnalyzes.DocumentHeaderCode = inventProduct.DocumentHeaderCode;
						//newIturAnalyzes.DocumentCode = inventProduct.DocumentCode;
						//newIturAnalyzes.IturCode = inventProduct.IturCode;
						//newIturAnalyzes.Barcode = inventProduct.Barcode;
						//newIturAnalyzes.QuantityDifference = inventProduct.QuantityEdit - inventProduct.QuantityOriginal ;
						//double quantityEdit = Convert.ToDouble(inventProduct != null ? inventProduct.QuantityEdit : 0);
						//newIturAnalyzes.QuantityOriginal = inventProduct.QuantityOriginal;
						//newIturAnalyzes.SerialNumber = inventProduct.SerialNumber != null ? inventProduct.SerialNumber : "";
						//newIturAnalyzes.ShelfCode = inventProduct.ShelfCode != null ? inventProduct.ShelfCode : "";
						//newIturAnalyzes.ModifyDate = inventProduct.ModifyDate;
						//newIturAnalyzes.ProductName = inventProduct.ProductName;
						//newIturAnalyzes.PDA_StatusInventProductBit = inventProduct.StatusInventProductBit;
						//newIturAnalyzes.PDA_DocumentHeaderCode = inventProduct.DocumentHeaderCode;
						newIturAnalyzes.Makat = inventProduct.Makat;
						newIturAnalyzes.MakatOriginal = inventProduct.Makat;
						
						//newIturAnalyzes.InputTypeCode = inventProduct.InputTypeCode;
						//newIturAnalyzes.IPNum = inventProduct.IPNum;
						//newIturAnalyzes.PDA_ID = inventProduct.ID;
						newIturAnalyzes.Count = inventProduct.Count;

						//===========Catalog =============

						//newIturAnalyzes.MakatOrigina = inventProduct.Makat;			 //W 
						//newIturAnalyzes.BarcodeOriginal = inventProduct.Barcode;	  //W
						//newIturAnalyzes.PriceString = "";
						newIturAnalyzes.Price = 0;
						newIturAnalyzes.PriceBuy = 0;
						newIturAnalyzes.PriceSale = 0;
						newIturAnalyzes.PriceExtra = 0;
						newIturAnalyzes.ValueBuyQriginal = 0;
						newIturAnalyzes.ValueBuyEdit = 0;
						newIturAnalyzes.ValueBuyDifference = 0;
						newIturAnalyzes.ValueOriginalERP = 0;
						newIturAnalyzes.ValueDifferenceOriginalERP = 0;
						newIturAnalyzes.CountInParentPack = 1;
						newIturAnalyzes.BalanceQuantityPartialERP = 0;

						//newIturAnalyzes.QuantityEdit"].Value = quantityEdit;
						//newIturAnalyzes.QuantityDifference"].Value = quantityDifference; // 0;// - quantityEdit;
						newIturAnalyzes.QuantityOriginal = 0;	  //quantityOriginal

						newIturAnalyzes.SectionCode = DomainUnknownCode.UnknownSection;
						newIturAnalyzes.SupplierCode = DomainUnknownCode.UnknownSupplier;
						newIturAnalyzes.SupplierName = DomainUnknownName.UnknownSupplier;
						newIturAnalyzes.ProductName = "NotExistInCatalog";
						newIturAnalyzes.Code = inventProduct.Makat; //+" - NotExistInCatalog";
						//newIturAnalyzes.MakatOriginal = "";
						//newIturAnalyzes.FromCatalogType = inventProduct.FromCatalogType;
						//newIturAnalyzes.SectionNum = inventProduct.SectionNum != null ? inventProduct.SectionNum : 0;
						//newIturAnalyzes.TypeCode = "";
						newIturAnalyzes.ResultCode = "XXX";
						newIturAnalyzes.TypeMakat = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";
						newIturAnalyzes.FromCatalogType = (int)FromCatalogTypeEnum.Unknown;
						double balanceQuantityERP = 0;
						double balanceQuantityPartialERP = 0;
						//double quantityDifference = 0;
						double priceBuy = 0;
						double priceSale = 0;
						double priceExtra = 0;
						double price = 0;
						bool isUpdateERP = false;
						string sectionName = DomainUnknownName.UnknownSection;
						string sectionDescription = "##";
						string supplierName = DomainUnknownName.UnknownSupplier;
						string familyName =  DomainUnknownName.UnknownFamily;
						string familyDescription = "##";

						newIturAnalyzes.UnitTypeCode = "";
						newIturAnalyzes.IturCodeExpected = "";

						//if (productMakatDictionary.ContainsKey(inventProduct.Makat) == true)
						//{
						//    ProductMakat productMakat = productMakatDictionary[inventProduct.Makat];
						//    newIturAnalyzes.MakatOriginal = productMakat.MakatOriginal;
						//    newIturAnalyzes.BarcodeOriginal = productMakat.MakatOriginal;//?
						//    newIturAnalyzes.TypeCode = productMakat.TypeCode;
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
						double quantityEdit = Convert.ToDouble(inventProduct.QuantityEdit != null ? inventProduct.QuantityEdit : 0);
						double quantityInPackEdit = inventProduct.QuantityInPackEdit;

						newIturAnalyzes.IPValueFloat5 = quantityEdit;

						//double quantityDifference = Convert.ToDouble(inventProduct.QuantityDifference != null ? inventProduct.QuantityDifference : 0);

						int countInParentPack = 1;
						if (productSimpleDictionary.ContainsKey(inventProduct.Makat) == true)
						{
							ProductSimple productSimple = productSimpleDictionary[inventProduct.Makat];
							newIturAnalyzes.TypeMakat = productSimple.TypeCode != null ? productSimple.TypeCode : "";

							 if (string.IsNullOrWhiteSpace(productSimple.MakatOriginal) == false) newIturAnalyzes.MakatOriginal = productSimple.MakatOriginal;
							 else newIturAnalyzes.MakatOriginal = productSimple.Makat;

							string sectionCode = string.IsNullOrWhiteSpace(productSimple.SectionCode) == false ? productSimple.SectionCode : DomainUnknownCode.UnknownSection;
							string supplierCode = string.IsNullOrWhiteSpace(productSimple.SupplierCode) == false ? productSimple.SupplierCode : DomainUnknownCode.UnknownSupplier;
							string familyCode = string.IsNullOrWhiteSpace(productSimple.FamilyCode) == false ? productSimple.FamilyCode : DomainUnknownCode.UnknownFamily;

							newIturAnalyzes.SupplierCode = supplierCode;
							newIturAnalyzes.FamilyCode = familyCode;
							newIturAnalyzes.SectionCode = sectionCode;

							productSimpleDictionary[inventProduct.Makat].FromCatalogTypeWithoutInventProduct = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
							newIturAnalyzes.FromCatalogType = productSimple.FromCatalogType;
							//newIturAnalyzes.FromCatalogType"].Value = productSimple.FromCatalogType;
							//newIturAnalyzes.PriceString"].Value = productSimple.PriceString;

							price = 0;
							priceBuy = productSimple.PriceBuy != null ? Convert.ToDouble(productSimple.PriceBuy) : 0;
							priceSale = productSimple.PriceSale != null ? Convert.ToDouble(productSimple.PriceSale) : 0;
							priceExtra = productSimple.PriceExtra != null ? Convert.ToDouble(productSimple.PriceExtra) : 0;
							price = priceCode.GetPriceByPriceCode(priceBuy, priceSale, priceExtra);

							balanceQuantityERP = productSimple.BalanceQuantityERP;
							balanceQuantityPartialERP = productSimple.BalanceQuantityPartialERP;
							countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;
							newIturAnalyzes.CountInParentPack = countInParentPack;
							newIturAnalyzes.BalanceQuantityPartialERP = productSimple.BalanceQuantityPartialERP;
							//newIturAnalyzes.MakatOriginal = productSimple.MakatOriginal;
							newIturAnalyzes.ProductName = productSimple.Name;
							newIturAnalyzes.UnitTypeCode = productSimple.UnitTypeCode != null ? productSimple.UnitTypeCode : "";
							newIturAnalyzes.IturCodeExpected = productSimple.IturCodeExpected;
							
						

							//------------- Family (From Family) --------------------
							if (codeReport == "[Rep-IS1-12f]") // 	Family
							{
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
							}

							newIturAnalyzes.Code = productSimple.Code != null ? productSimple.Code : "";

							//------------- section (From Catalog) --------------------
				
							if (sectionFromDBDictionary.ContainsKey(sectionCode) == true)
							{
								sectionName = sectionFromDBDictionary[sectionCode].Name;
								sectionDescription = sectionFromDBDictionary[sectionCode].Description;

								if (codeReport == "[Rep-IS1-27p]") // TODO subSectionCode, subSectionName
								{
									string subSectionCode = "";
									string subSectionName = "";
									if (sectionFromDBDictionary[sectionCode].TypeCode == "SS")
									{
										subSectionCode = sectionCode ;
										subSectionName =  sectionName;
										string parentSectionCode=  sectionFromDBDictionary[sectionCode].ParentSectionCode;
										if (sectionFromDBDictionary.ContainsKey(parentSectionCode) == true)
										{
											sectionCode = parentSectionCode;
											sectionName = sectionFromDBDictionary[sectionCode].Name;
											sectionDescription = sectionFromDBDictionary[sectionCode].Description;
										}
									}

									newIturAnalyzes.IPValueStr6 = subSectionCode;	   //TODO  add in table	 subSectionCode
									newIturAnalyzes.IPValueStr7 = subSectionName;	   //TODO  add in table	 subSectionName
								}
							}

							//newIturAnalyzes.SectionCode = sectionCode;
							newIturAnalyzes.SectionName = sectionName;

						

							//if (codeReport == "[Rep-IS1-23p]") //TafnitMatrix (SectionTafnitMatrixParser)
							//{
							//	string[] subsection = sectionDescription.Split('#');
							//	newIturAnalyzes.IPValueStr1 = "";
							//	newIturAnalyzes.IPValueStr2 = "";
							//	newIturAnalyzes.IPValueStr3 = "";
							//	newIturAnalyzes.IPValueStr4 = "";
							//	if (subsection.Length >= 1) newIturAnalyzes.IPValueStr1 = subsection[0];
							//	if (subsection.Length >= 2) newIturAnalyzes.IPValueStr2 = subsection[1];
							//	if (subsection.Length >= 3) newIturAnalyzes.IPValueStr3 = subsection[2];
							//	if (subsection.Length >= 4) newIturAnalyzes.IPValueStr4 = subsection[3];
							//}

							//------------- Family (From Catalog) --------------------
							if (codeReport == "[Rep-IS1-23p]")
							{
								if (familyFromDBDictionary.Count > 0)
								{
									if (familyFromDBDictionary.ContainsKey(familyCode) == true)
									//&& familyFromDBDictionary[familyCode] != null)
									{
										Family family = familyFromDBDictionary[familyCode];
										if (family != null)
										{
											familyName = family.Name;
											familyDescription = family.Description;
										}

										newIturAnalyzes.FamilyName = familyName;
										//if (codeReport == "[Rep-IS1-23p]")
										//{
										string[] subsection = familyDescription.Split('#');

										newIturAnalyzes.IPValueStr1 = "";
										newIturAnalyzes.IPValueStr2 = "";
										newIturAnalyzes.IPValueStr3 = "";
										newIturAnalyzes.IPValueStr4 = "";
										if (subsection.Length >= 1) newIturAnalyzes.IPValueStr1 = subsection[0];
										if (subsection.Length >= 2) newIturAnalyzes.IPValueStr2 = subsection[1];
										if (subsection.Length >= 3) newIturAnalyzes.IPValueStr3 = subsection[2];
										if (subsection.Length >= 4) newIturAnalyzes.IPValueStr4 = subsection[3];

										//newIturAnalyzes.FamilySize = family.Size;
										//newIturAnalyzes.FamilyType = family.Type;
										//newIturAnalyzes.FamilyExtra1 = family.Extra1;
										//newIturAnalyzes.FamilyExtra2 = family.Extra2;
										//} // codeReport
									}
								}
							} // codeReport
							//------------- supplier (From Catalog) --------------------
							if (supplierFromDBDictionary.ContainsKey(supplierCode) == true)
							{
								supplierName = supplierFromDBDictionary[supplierCode].Name;
							}
							isUpdateERP = productSimple.IsUpdateERP;
							//newIturAnalyzes.ValueBuyQriginal = productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityOriginal);
							//newIturAnalyzes.ValueBuyEdit = productSimple.PriceBuy * Convert.ToDouble(inventProduct.QuantityEdit);
							//newIturAnalyzes.ValueBuyDifference = productSimple.PriceBuy *
							//    (Convert.ToDouble(inventProduct.QuantityOriginal) - Convert.ToDouble(inventProduct.QuantityEdit));
						}

						double balanceQuantityERPAndPartial = balanceQuantityERP;
						if (countInParentPack != 0) balanceQuantityERPAndPartial = balanceQuantityERP + balanceQuantityPartialERP / countInParentPack;
						newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;//balanceQuantityERP;


						double quantityEditAndPartial = quantityEdit + quantityInPackEdit;	  //???
						if (countInParentPack != 0) quantityEditAndPartial = quantityEdit + (double)quantityInPackEdit / (double)countInParentPack;
						newIturAnalyzes.QuantityEdit = quantityEditAndPartial;
						//newIturAnalyzes.QuantityEdit = quantityEdit;
						newIturAnalyzes.QuantityInPackEdit = (int)quantityInPackEdit;
						double quantityDifference = quantityEditAndPartial - inventProduct.QuantityOriginal;
						newIturAnalyzes.QuantityDifference = quantityDifference;
						newIturAnalyzes.QuantityDifferenceOriginalERP = quantityEditAndPartial - balanceQuantityERPAndPartial;//quantityEdit - balanceQuantityERP;
			
						newIturAnalyzes.PriceBuy = priceBuy;
						newIturAnalyzes.PriceSale = priceSale;
						newIturAnalyzes.PriceExtra = priceExtra;
						newIturAnalyzes.Price = price;

						double valueBuyEdit = price * quantityEditAndPartial; //priceBuy * quantityEdit;
						double valueBuyDifference = price * quantityDifference;
						double valueBuyQriginalERP = price * balanceQuantityERPAndPartial;//priceBuy * balanceQuantityERP;

						newIturAnalyzes.ValueOriginalERP = valueBuyQriginalERP;
						newIturAnalyzes.ValueBuyEdit = valueBuyEdit;
						newIturAnalyzes.ValueBuyDifference = valueBuyDifference;
						newIturAnalyzes.ValueDifferenceOriginalERP = valueBuyEdit - valueBuyQriginalERP;

				
						newIturAnalyzes.SupplierName = supplierName;
						newIturAnalyzes.ResultCode = "XXX";
						newIturAnalyzes.IsUpdateERP = isUpdateERP;
					} //IP Sum

					catch (Exception error)
					{
						_logger.ErrorException("GerIturAnalyzesSumSimple", error);
						this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
					}

					//if (newIturAnalyzes.IturCode != newIturAnalyzes.IturCodeExpected) newIturAnalyzes.IturCodeDiffer = true;

					yield return newIturAnalyzes;
				}
			}// if inventProductList

			inventProductList = null;
			GC.Collect();

			stopwatch.Stop();
			System.Diagnostics.Debug.Print(String.Format("inventProductList cmd.ExecuteNonQuery: {0}", stopwatch.ElapsedTicks.ToString()));


			stopwatch = Stopwatch.StartNew();
			//ProductMakatWithoutInventProduct
			foreach (KeyValuePair<string, ProductSimple> keyValuePair in productSimpleDictionary)
			{
				if (cancellationToken.IsCancellationRequested == true)
				{
					break;
				}
				ProductSimple productSimple = keyValuePair.Value;
				if (productSimple.FromCatalogTypeWithoutInventProduct == (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct)
				{
					//    keyValuePair.Key.ToString();
					if (productSimple.BalanceQuantityERP != 0.0 					
						|| includeProductMakatWithoutInventProduct == true)
					{
						k++;
						IturAnalyzes newIturAnalyzes = new IturAnalyzes();
						newIturAnalyzes.TypeMakat =  TypeMakatEnum.W.ToString();
						try
						{
							//===================Inventor======================================
							newIturAnalyzes.InventorCode = inventorCode;
							newIturAnalyzes.InventorName = inventorName;
							//===================================================================
							string sectionCode = string.IsNullOrWhiteSpace(productSimple.SectionCode) == false ? productSimple.SectionCode : DomainUnknownCode.UnknownSection;
							string supplierCode = string.IsNullOrWhiteSpace(productSimple.SupplierCode) == false ? productSimple.SupplierCode : DomainUnknownCode.UnknownSupplier;
							string familyCode = string.IsNullOrWhiteSpace(productSimple.FamilyCode) == false ? productSimple.FamilyCode : DomainUnknownCode.UnknownFamily;
							newIturAnalyzes.FamilyCode = familyCode;
							newIturAnalyzes.SectionCode = sectionCode;
							newIturAnalyzes.SupplierCode = supplierCode;
							string sectionName = sectionCode;
							string supplierName = DomainUnknownName.UnknownSupplier;
							string sectionDescription = "##";
							string familyName = familyCode;
							string familyDescription = "##";

							//------------- supplier (From Catalog) --------------------
							//if (sectionFromDBDictionary.ContainsKey(sectionCode) == true)
							//{
							//	sectionName = sectionFromDBDictionary[sectionCode].Name;
							//	sectionDescription = sectionFromDBDictionary[sectionCode].Description;
							//}
							//newIturAnalyzes.SectionName = sectionName;


							if (sectionFromDBDictionary.ContainsKey(sectionCode) == true)
							{
								sectionName = sectionFromDBDictionary[sectionCode].Name;
								sectionDescription = sectionFromDBDictionary[sectionCode].Description;

								if (codeReport == "[Rep-IS1-27p]") // TODO subSectionCode, subSectionName
								{
									string subSectionCode = "";
									string subSectionName = "";
									if (sectionFromDBDictionary[sectionCode].TypeCode == "SS")
									{
										subSectionCode = sectionCode;
										subSectionName = sectionName;
										string parentSectionCode = sectionFromDBDictionary[sectionCode].ParentSectionCode;
										if (sectionFromDBDictionary.ContainsKey(parentSectionCode) == true)
										{
											sectionCode = parentSectionCode;
											sectionName = sectionFromDBDictionary[sectionCode].Name;
											sectionDescription = sectionFromDBDictionary[sectionCode].Description;
										}
									}

									newIturAnalyzes.IPValueStr6 = subSectionCode;	   //TODO  add in table	 subSectionCode
									newIturAnalyzes.IPValueStr7 = subSectionName;	   //TODO  add in table	 subSectionName
								}
							}

							//newIturAnalyzes.SectionCode = sectionCode;
							newIturAnalyzes.SectionName = sectionName;

							//if (codeReport == "[Rep-IS1-23p]")
							//{
							//	string[] subsection = sectionDescription.Split('#'); //TafnitMatrix (SectionTafnitMatrixParser)
							//	newIturAnalyzes.IPValueStr1 = "";
							//	newIturAnalyzes.IPValueStr2 = "";
							//	newIturAnalyzes.IPValueStr3 = "";
							//	newIturAnalyzes.IPValueStr4 = "";
							//	if (subsection.Length >= 1) newIturAnalyzes.IPValueStr1 = subsection[0];
							//	if (subsection.Length >= 2) newIturAnalyzes.IPValueStr2 = subsection[1];
							//	if (subsection.Length >= 3) newIturAnalyzes.IPValueStr3 = subsection[2];
							//	if (subsection.Length >= 4) newIturAnalyzes.IPValueStr4 = subsection[3];
							//}

							//------------- Family (From Catalog) --------------------
							if (codeReport == "[Rep-IS1-23p]")
							{
								if (familyFromDBDictionary.Count > 0)
								{
									if (familyFromDBDictionary.ContainsKey(familyCode) == true)
									//&& familyFromDBDictionary[familyCode] != null)
									{
										Family family = familyFromDBDictionary[familyCode];
										if (family != null)
										{
											familyName = family.Name;
											familyDescription = family.Description;
										}

										newIturAnalyzes.FamilyName = familyName;
										//if (codeReport == "[Rep-IS1-23p]")
										//{
										string[] subsection = familyDescription.Split('#');

										newIturAnalyzes.IPValueStr1 = "";
										newIturAnalyzes.IPValueStr2 = "";
										newIturAnalyzes.IPValueStr3 = "";
										newIturAnalyzes.IPValueStr4 = "";
										if (subsection.Length >= 1) newIturAnalyzes.IPValueStr1 = subsection[0];
										if (subsection.Length >= 2) newIturAnalyzes.IPValueStr2 = subsection[1];
										if (subsection.Length >= 3) newIturAnalyzes.IPValueStr3 = subsection[2];
										if (subsection.Length >= 4) newIturAnalyzes.IPValueStr4 = subsection[3];

										//newIturAnalyzes.FamilySize = family.Size;
										//newIturAnalyzes.FamilyType = family.Type;
										//newIturAnalyzes.FamilyExtra1 = family.Extra1;
										//newIturAnalyzes.FamilyExtra2 = family.Extra2;
										//} // codeReport
									}
								}
							} // codeReport
							//------------- supplier (From Catalog) --------------------
							if (supplierFromDBDictionary.ContainsKey(supplierCode) == true)
							{
								supplierName = supplierFromDBDictionary[supplierCode].Name;
							}
							newIturAnalyzes.SectionName = sectionName;
							newIturAnalyzes.SupplierName = supplierName;

							newIturAnalyzes.UnitTypeCode = productSimple.UnitTypeCode != null ? productSimple.UnitTypeCode : "";

							double price = 0;
							double priceBuy = productSimple.PriceBuy != null ? Convert.ToDouble(productSimple.PriceBuy) : 0;
							double priceSale = productSimple.PriceSale != null ? Convert.ToDouble(productSimple.PriceSale) : 0;
							double priceExtra = productSimple.PriceExtra != null ? Convert.ToDouble(productSimple.PriceExtra) : 0;
							price = priceCode.GetPriceByPriceCode(priceBuy, priceSale, priceExtra);


							int countInParentPack = productSimple.CountInParentPack == 0 ? 1 : productSimple.CountInParentPack;

							newIturAnalyzes.QuantityOriginalERP = 0;
							//double balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP;	   //??
							//if (countInParentPack != 0) balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP +
							//	productSimple.BalanceQuantityPartialERP / countInParentPack;
							//newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;


							//newIturAnalyzes.FromCatalogType = productSimple.FromCatalogType;
							newIturAnalyzes.FromCatalogType = (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct;
							newIturAnalyzes.BalanceQuantityPartialERP = productSimple.BalanceQuantityPartialERP;
							newIturAnalyzes.QuantityEdit = 0;
							newIturAnalyzes.QuantityInPackEdit = 0;
							newIturAnalyzes.IPValueFloat5 = 0;					 //	QuantityEdit

							newIturAnalyzes.QuantityDifferenceOriginalERP = 0;
							//newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;//balanceQuantityERP;
							//newIturAnalyzes.QuantityDifferenceOriginalERP = 0 - balanceQuantityERPAndPartial;//balanceQuantityERP;

							//===
							newIturAnalyzes.QuantityDifference = 0;// quantityEdit;
							newIturAnalyzes.QuantityOriginal = 0;	  //- quantityOriginal
							//=====
							newIturAnalyzes.Makat = productSimple.Makat;
							if (string.IsNullOrWhiteSpace(productSimple.MakatOriginal) == false) newIturAnalyzes.MakatOriginal = productSimple.MakatOriginal;
							else newIturAnalyzes.MakatOriginal = productSimple.Makat;
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
							newIturAnalyzes.ProductName = productSimple.Name;
							//newIturAnalyzes.MakatOriginal = productSimple.MakatOriginal;
							//newIturAnalyzes.TypeMakat = "";
							newIturAnalyzes.ResultCode = "XXX";
							newIturAnalyzes.IsUpdateERP = false;

							if (productSimple.BalanceQuantityERP != 0.0)
							{
								double balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP;	   //??
								if (countInParentPack != 0) balanceQuantityERPAndPartial = productSimple.BalanceQuantityERP +
									productSimple.BalanceQuantityPartialERP / countInParentPack;
								newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;

								newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;//balanceQuantityERP;
								newIturAnalyzes.QuantityDifferenceOriginalERP = 0 - balanceQuantityERPAndPartial;//balanceQuantityERP;
								newIturAnalyzes.ValueOriginalERP = balanceQuantityERPAndPartial * price;//balanceQuantityERP * priceBuy;
								newIturAnalyzes.ValueDifferenceOriginalERP = 0 - balanceQuantityERPAndPartial * price;//0 - balanceQuantityERP * priceBuy;

							}
						}

						catch (Exception error)
						{
							_logger.ErrorException("GerIturAnalyzesSumSimple", error);
							this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
						}
						yield return newIturAnalyzes;
					}
				}

			} // end ProductMakatWithoutInventProduct

			stopwatch.Stop();
			System.Diagnostics.Debug.Print(String.Format("productSimpleDictionary cmd.ExecuteNonQuery: {0}", stopwatch.ElapsedTicks.ToString()));

			stopwatch = Stopwatch.StartNew();

			if (cancellationToken.IsCancellationRequested == false)
			{
				stopwatch.Stop();

				System.Diagnostics.Debug.Print(String.Format("Commit: {0}", stopwatch.ElapsedTicks.ToString()));

				if (addResult == true)
				{
					
					string countItemsInsertFromBarcode = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountItems_InsertFromBarcode, pathDB);
					yield return iturAnalyzesRepository.SetIturAnalyzesResulteValue(AnalezeValueTypeEnum.CountItems_InsertFromBarcode, countItemsInsertFromBarcode, "Items - Insert from Barcode", pathDB, inventorCode, inventorName);

					string countItemsInsertManually = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountItems_InsertManually, pathDB);
					yield return iturAnalyzesRepository.SetIturAnalyzesResulteValue(AnalezeValueTypeEnum.CountItems_InsertManually, countItemsInsertManually, "Items - Insert Manually", pathDB, inventorCode, inventorName);

					//string countItemsTotal = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountItems_Total, pathDB);
					//yield return iturAnalyzesRepository.SetIturAnalyzesResulteValue(AnalezeValueTypeEnum.CountItems_Total, countItemsTotal, "Items Total", pathDB);

					string countPDAMakatsTotal = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountPDAMakats_Total, pathDB);
					yield return iturAnalyzesRepository.SetIturAnalyzesResulteValue(AnalezeValueTypeEnum.CountPDAMakats_Total, countPDAMakatsTotal, "Total Makat (Counted)", pathDB, inventorCode, inventorName);

					string countERPMakatsTotal = inventProductRepository.GetCountMakatTotal(AnalezeValueTypeEnum.CountERPMakats_Total, pathDB);
					yield return iturAnalyzesRepository.SetIturAnalyzesResulteValue(AnalezeValueTypeEnum.CountERPMakats_Total, countERPMakatsTotal, "Total Makat (ERP)", pathDB, inventorCode, inventorName);
				}
				//string ret = iturAnalyzesRepository.GetResulteValue(AnalezeValueTypeEnum.CountItems_Total,  pathDB);
				//Localization.Resources.Log_TraceRepositoryResult1011%"Add {0} {1} in DB [{2}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1011, k, "IturAnalyzes", pathDB));
				//}

				//catch (Exception error)
				//{
				//    _logger.ErrorException("GerIturAnalyzesSumSimple", error);
				//    this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + "k=" + k.ToString() + " : " + error.StackTrace);
				//}
			}
		}

	
		private void CountLong(long count)
		{
		}

	}
}
