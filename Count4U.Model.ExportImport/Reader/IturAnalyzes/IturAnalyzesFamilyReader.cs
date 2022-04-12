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


namespace Count4U.Model.Count4U
{
	public class IturAnalyzesFamilyReader : BaseADORepository, IIturAnalyzesReader
	{
		private readonly IMakatRepository _makatRepository;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public IturAnalyzesFamilyReader(
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


			IIturAnalyzesRepository iturAnalyzesRepository =
				this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			Dictionary<string, Itur> iturDictionary = iturAnalyzesRepository.GetIturDictionary(pathDB, refill);
			Dictionary<string, DocumentHeader> documentHeaderDictionary = iturAnalyzesRepository.GetDocumentHeaderDictionary(pathDB, refill);
			Dictionary<string, Location> locationDictionary = iturAnalyzesRepository.GetLocationDictionary(pathDB, refill);
			//ISectionRepository sectionRepository = this._serviceLocator.GetInstance<ISectionRepository>();
			//Dictionary<string, Section> sectionFromDBDictionary = sectionRepository.GetSectionDictionary(pathDB, true);
			IFamilyRepository familyRepository = this._serviceLocator.GetInstance<IFamilyRepository>();
			Dictionary<string, Family> familyFromDBDictionary = familyRepository.GetFamilyDictionary(pathDB, true);
			//bool refillCatalogDictionary = false;
			Dictionary<string, ProductMakat> productMakatDictionary = iturAnalyzesRepository.GetProductMakatDictionary(pathDB, refillCatalogDictionary);
			Dictionary<string, ProductSimple> productSimpleDictionary = iturAnalyzesRepository.GetProductSimpleDictionary(pathDB, refillCatalogDictionary);


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

			try
			{
				inventProductList = iturAnalyzesRepository.GetInventProductList(selectParms, pathDB);
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
				
						//==================== PDA =========================================
						newIturAnalyzes.DocumentHeaderCode = inventProduct.DocumentHeaderCode;
						newIturAnalyzes.DocumentCode = inventProduct.DocumentCode;
						newIturAnalyzes.IturCode = inventProduct.IturCode;
						newIturAnalyzes.Barcode = inventProduct.Barcode;
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
						newIturAnalyzes.ModifyDate = inventProduct.ModifyDate;
						newIturAnalyzes.ProductName = inventProduct.ProductName;
						newIturAnalyzes.PDA_StatusInventProductBit = inventProduct.StatusInventProductBit;
						//newIturAnalyzes.PDA_DocumentHeaderCode = inventProduct.DocumentHeaderCode;
						newIturAnalyzes.Makat = inventProduct.Makat;
						newIturAnalyzes.InputTypeCode = inventProduct.InputTypeCode;
						newIturAnalyzes.IPNum = inventProduct.IPNum;
						newIturAnalyzes.PDA_ID = inventProduct.ID;
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
						newIturAnalyzes.FromCatalogType = inventProduct.FromCatalogType;
						newIturAnalyzes.SectionNum = inventProduct.SectionNum != null ? inventProduct.SectionNum : 0;
						newIturAnalyzes.TypeCode = "";
						newIturAnalyzes.TypeMakat = inventProduct.TypeMakat != null ? inventProduct.TypeMakat : "";

						newIturAnalyzes.QuantityOriginalERP = 0;
						newIturAnalyzes.BalanceQuantityPartialERP = 0;
						newIturAnalyzes.QuantityDifferenceOriginalERP = 0;
						newIturAnalyzes.ValueOriginalERP = 0;
						newIturAnalyzes.ValueDifferenceOriginalERP = 0;


						if (productMakatDictionary.ContainsKey(inventProduct.Makat) == true)
						{
							ProductMakat productMakat = productMakatDictionary[inventProduct.Makat];
							newIturAnalyzes.MakatOriginal = productMakat.MakatOriginal;
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
							}
						}
						if (productSimpleDictionary.ContainsKey(inventProduct.Makat) == true)
						{
							ProductSimple productSimple = productSimpleDictionary[inventProduct.Makat];
							//double quantityOriginalERP = productSimple.BalanceQuantityERP != null ? Convert.ToDouble(productSimple.BalanceQuantityERP) : 0;
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

							newIturAnalyzes.FromCatalogType = productSimple.FromCatalogType;
							
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
								productSimple.BalanceQuantityPartialERP / countInParentPack;
							newIturAnalyzes.QuantityOriginalERP = balanceQuantityERPAndPartial;//balanceQuantityERP;

							double valueBuyQriginalERP = price * balanceQuantityERPAndPartial;
							newIturAnalyzes.ValueOriginalERP = valueBuyQriginalERP;
							//------------- Section (From Catalog) --------------------

							//string sectionCode = string.IsNullOrWhiteSpace(productSimple.SectionCode) == false ? productSimple.SectionCode : DomainUnknownCode.UnknownSection;

							//newIturAnalyzes.SectionCode = sectionCode;
							//if (sectionFromDBDictionary.Count > 0)
							//{
							//	if (sectionFromDBDictionary.ContainsKey(sectionCode) == true
							//	&& sectionFromDBDictionary[sectionCode] != null)
							//	{
							//		Section section = sectionFromDBDictionary[sectionCode];
							//		newIturAnalyzes.SectionName = section.Name;
							//	}
							//}

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

							//newIturAnalyzes.ValueOriginalERP = quantityOriginalERP * priceBuy;
							//newIturAnalyzes.QuantityOriginalERP = quantityOriginalERP;
							//newIturAnalyzes.QuantityDifferenceOriginalERP = quantityEdit- quantityOriginalERP;
							//newIturAnalyzes.ValueDifferenceOriginalERP = 0;
						}

						//===================== Doc ========================================
						if (documentHeaderDictionary.ContainsKey(inventProduct.DocumentCode) == true)
						{
							DocumentHeader docHeader = documentHeaderDictionary[inventProduct.DocumentCode];
							string iturCode = docHeader.IturCode != null ? docHeader.IturCode : "";
							//newIturAnalyzes.Code = iturCode;
							newIturAnalyzes.Doc_Name = docHeader.Name != null ? docHeader.Name : "";
							newIturAnalyzes.Doc_Approve = docHeader.Approve != null ? docHeader.Approve : false;
							//newIturAnalyzes.Doc_IturCode = docHeader.IturCode;
							newIturAnalyzes.Doc_WorkerGUID = docHeader.WorkerGUID != null ? docHeader.WorkerGUID : "";
							newIturAnalyzes.Doc_StatusDocHeaderBit = docHeader.StatusDocHeaderBit;
							newIturAnalyzes.Doc_StatusInventProductBit = docHeader.StatusInventProductBit;
							newIturAnalyzes.Doc_StatusApproveBit = docHeader.StatusApproveBit;
							newIturAnalyzes.DocNum = (int)docHeader.ID;
							//==================== Itur ==================================
							if (iturDictionary.ContainsKey(iturCode) == true
								&& iturDictionary[iturCode] != null)
							{
								Itur itur = iturDictionary[iturCode];
								string locationCode = itur.LocationCode != null ? itur.LocationCode : DomainUnknownCode.UnknownLocation;
								newIturAnalyzes.ERPIturCode = itur.ERPIturCode;
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

					yield return newIturAnalyzes;
				}  //foreach
			}
		}

	



		
		private void CountLong(long count)
		{
		}

	}
}
