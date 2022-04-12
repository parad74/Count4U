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
using System.IO;
using System.Threading;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4Mobile;
using System.Data.Entity;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class ExportCurrentInventorAdvancedERPRepository : BaseExportFileRepository, IExportCurrentInventorAdvancedRepository
	{
		private IExportCurrentInventoryWriter _exportCurrentInventoryWriter;
		private static long CountRow;
		//private Dictionary<ImportProviderParmEnum, object> Parms;
		private Dictionary<string, ProductMakat> _productMakatDictionary;

		public ExportCurrentInventorAdvancedERPRepository(
			IServiceLocator serviceLocator,
			ILog log)
			: base(log, serviceLocator)
		{
			this._productMakatDictionary = new Dictionary<string, ProductMakat>();
		}

		

		public void WriteToFile(string fromPathDB, string toPathFile,
			WriterEnum currentInventoryWriter,
			Encoding encoding, string[] separators,
			List<ImportDomainEnum> importType,
			StreamWriter sw,
			Dictionary<ImportProviderParmEnum, object> parms = null,
			bool refill = true
			)
		{
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

		
				
			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1057%"Cancel Write to File [{0}] "
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1057, toPathFile));
				return;
			}

			PriceCodeEnum priceCode = parms.GetPriceCodeEnumFromParm();

	  		//Localization.Resources.Log_TraceRepository1015%"Export to [{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1015, "CurrentInventory", "ExportCurrentInventorAdvancedERPRepository"));

			this._exportCurrentInventoryWriter = this._serviceLocator.GetInstance<IExportCurrentInventoryWriter>(currentInventoryWriter.ToString());
			if (this._exportCurrentInventoryWriter == null) throw new ArgumentNullException(currentInventoryWriter.ToString()  + " is null");

			//Localization.Resources.Log_TraceRepository1040%"[{0}]  is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "WriterEnum", currentInventoryWriter.ToString()));

			//Dictionary<string, ProductMakat> productMakatDictionary = base.GetProductMakatDictionary(fromPathDB, true);
			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1057%"Cancel Write to File [{0}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1057, toPathFile));
				return;
			}

			Dictionary<object, object> parmsIn = new Dictionary<object, object>();
			//string photoPropertyName = this._userSettingsManager.InventProductPropertyPhotoSelectedItemGet();
			parmsIn = parms.ConvertToObjectFromImportProviderParmEnum();
			//parmsIn[ImportProviderParmEnum.Path1] = parms.GetStringValueFromParm(ImportProviderParmEnum.Path1);
			//parmsIn[ImportProviderParmEnum.PropertyName] = parms.GetStringValueFromParm(ImportProviderParmEnum.PropertyName); ;

			//Localization.Resources.Log_TraceRepositoryResult1016%"Start Write to File [{0}] "
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1016, toPathFile));

			string ERPNum = parms.GetStringValueFromParm(ImportProviderParmEnum.ERPNum);
			string INVDate = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorDate);
			string INVDate1 = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorDate1);

			if (currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusMateAsherXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryStockSonigoXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1_Q
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1_SN
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter2
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1_Q
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1_SN
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter2
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusLadpcXslxWriter2
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusLadpcCsvWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusMisradApnimXslxWriter)
			{
				//string photoPath 
				ERPNum = parms.GetStringValueFromParm(ImportProviderParmEnum.Path1) + @"/";
			}

			//ExportFileType fileType = parms.GetFileTypeFromParm();
			//bool barcodeWithoutMask = parms.GetBoolValueFromParm(ImportProviderParmEnum.BarcodeWithoutMask);
			//ICurrentInventoryAdvancedReader iturAnalyzesRepository = this._serviceLocator.GetInstance<ICurrentInventoryAdvancedReader>();
			string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			IPreviousInventoryRepository _previousInventoryEFRepository = this._serviceLocator.GetInstance<IPreviousInventoryRepository>();

			if (importType.Contains(ImportDomainEnum.SectionCodeContains) == true
		|| importType.Contains(ImportDomainEnum.UnitTypeCodeContains) == true
		|| importType.Contains(ImportDomainEnum.ProductNameContains) == true)
			{
				IMakatRepository makatRepository = base._serviceLocator.GetInstance<IMakatRepository>();
				this._productMakatDictionary = makatRepository.GetProductUnitTypeDictionary(fromPathDB, refill);
			}

			//if (importType.Contains(ImportDomainEnum.TagContains) == true)
			//{
			//	IProductRepository productRepository = base._serviceLocator.GetInstance<IProductRepository>();
			//	this._productTagSimpleDictionary = productRepository.GetProductTagDictionary(fromPathDB);
			//}
		

			SelectParams selectParams = new SelectParams();
			//List<string> typeMakatList = new List<string>();

			//bool excludeNotExistingInCatalog = parms.GetBoolValueFromParm(ImportProviderParmEnum.ExcludeNotExistingInCatalog);

			//if (importType.Contains(ImportDomainEnum.ExportInventProductW) == true)  //Add record for every line that is in the Invetor and the makat is not exist in Catalog
			//{
			//	typeMakatList.Add(TypeMakatEnum.W.ToString());
			//}
			//else if (excludeNotExistingInCatalog == true) //Exclude Items with Makat not exists in Catalog
			//{
			//	typeMakatList.Add(TypeMakatEnum.M.ToString());
			//	typeMakatList.Add(TypeMakatEnum.B.ToString());
			//}
			//else //writer в фойл для всех продуктов 
			//{
			//	typeMakatList.Add(TypeMakatEnum.M.ToString());
			//	typeMakatList.Add(TypeMakatEnum.B.ToString());		//??
			//	typeMakatList.Add(TypeMakatEnum.W.ToString());
			//}

			//selectParams.FilterStringListParams["TypeMakat"] = new FilterStringListParam() { Values = typeMakatList };
			//if (inventProductWriter == WriterEnum.ExportInventProductXtechMeuhedetERPFileWriter)
			//{
			//	selectParams.SortParams = "IturCode ASC";
			//}

			if (importType.Contains(ImportDomainEnum.ExportInventProductByLocationCode) == true)
			{
				string locationCode = parms.GetStringValueFromParm(ImportProviderParmEnum.LocationCode);
				if (string.IsNullOrWhiteSpace(locationCode) == false)
				{									   //!! ??
					selectParams.FilterStringListParams.Add("LocationLevel1Code", new FilterStringListParam() { Values = new List<string>() { locationCode } });
				}
			}

			if (importType.Contains(ImportDomainEnum.ExportInventProductByModifyDate) == true)
			{
				string modifyDate = parms.GetStringValueFromParm(ImportProviderParmEnum.ModifyDate);
				if (string.IsNullOrWhiteSpace(modifyDate) == false)
				{                                      //yyyy-MM-dd
					selectParams.FilterStringListParams.Add("DateModified", new FilterStringListParam() { Values = new List<string>() { modifyDate } });
				}
			}

			else if (importType.Contains(ImportDomainEnum.ExportInventProductByIturCode) == true
				|| importType.Contains(ImportDomainEnum.ExportInventProductGroupByItur) == true)   // для группировки по итурам в одном файле 
			{
				string iturCode = parms.GetStringValueFromParm(ImportProviderParmEnum.IturCode);
				if (string.IsNullOrWhiteSpace(iturCode) == false)
				{
					selectParams.FilterStringListParams.Add("IturCode", new FilterStringListParam() { Values = new List<string>() { iturCode } });
				}
			}

			//ICurrentInventoryAdvancedSourceRepository  currentInventoryAdvancedSourceRepository	=	 this._serviceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
			//	currentInventoryAdvancedSourceRepository.ClearCurrentInventoryAdvanced(fromPathDB);
			//	currentInventoryAdvancedSourceRepository.InsertCurrentInventoryAdvanced(fromPathDB, true);

			ICurrentInventoryAdvancedRepository currentInventoryAdvancedEFRepository = this._serviceLocator.GetInstance<ICurrentInventoryAdvancedRepository>();

			//		Dictionary<Pair<string, string, string>, PropertyStr> dictionary = GetDictionaryDecoratorProperty(string domainObjectType, string pathDB);
			object argument = null;
			if (currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusMateAsherXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryStockSonigoXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1_Q
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1_SN
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter2
				||currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1_Q
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1_SN
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter2
				||	currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusLadpcXslxWriter2
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusLadpcCsvWriter1
				|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusMisradApnimXslxWriter)
			{
				 IPropertyStrRepository propertyStrRepository = _serviceLocator.GetInstance<IPropertyStrRepository>();
				string domainObjectType = DomainObjectTypeEnum.PropertyDecorator.ToString();
				Dictionary<Pair<string, string, string>, PropertyStr> dictionary = 
					propertyStrRepository.GetDictionaryDecoratorProperty(domainObjectType, fromPathDB);
				argument = dictionary;
			}

	  		try
			{
				CountRow = 0;
				countAction(CountRow);
				if (importType.Contains(ImportDomainEnum.ExportInventProductNotHeader) == false) //не заполнять заголовок всего файла
				{
					this._exportCurrentInventoryWriter.AddHeader(sw, ERPNum, INVDate, separator, parms, argument);
				}
				
				//WriteHeader(sw, writerEnum, ERPNum);

				//===================WriteRowSimple - Sum - IturAnalyzesSimple
				if (importType.Contains(ImportDomainEnum.ExportCurrentInventoryAdvanced) == true)		//WriteRow
				{
					List<CurrentInventoryAdvanced> currentInventoryAdvancedList  = null;
					if (currentInventoryWriter == WriterEnum.ExportCurrentInventoryClaitXslxWriter1
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter1
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter3
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter4
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter5
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativXslxWriter1
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusMateAsherXslxWriter1
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryStockSonigoXslxWriter1
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1_Q
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1_SN
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter2
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1_Q
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1_SN
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter2
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter2_1		  //"SN"
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter2_2
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaGovXslxWriter2_1		  //"SN"
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaGovXslxWriter2_2
						||currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusLadpcXslxWriter2
						||currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusLadpcCsvWriter1
						|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusMisradApnimXslxWriter)			//"Q"
					{


						currentInventoryAdvancedList = 
							currentInventoryAdvancedEFRepository.GetCurrentInventoryAdvancedList( fromPathDB, refill, true,
							selectParams, parmsIn);
						if (currentInventoryWriter == WriterEnum.ExportCurrentInventoryNativPlusXslxWriter2)				
						{
							var sorted = currentInventoryAdvancedList.OrderBy(x => x.LocationCode).ThenBy(x => x.ItemCode).Select(x => x);
							currentInventoryAdvancedList = sorted.ToList();						
						}

						if (currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter2_2
							|| currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaGovXslxWriter2_2)					//"Q"
						{

							var sumQuantity = from e in currentInventoryAdvancedList
											  where e.UnitTypeCode == "Q"
											  orderby e.PropertyStr20
											  group e by e.PropertyStr20 into g
											  select new CurrentInventoryAdvanced
											  {
												  ItemCode = g.Max(x => x.ItemCode),
												  CatalogItemCode = g.Max(x => x.CatalogItemCode),
												  QuantityDouble = g.Sum(x => x.QuantityDouble),
											
												  CatalogItemName = g.Max(x => x.CatalogItemName),
												  LocationLevel1Code = g.Max(x => x.LocationLevel1Code),
												  LocationLevel2Code = g.Max(x => x.LocationLevel2Code),
												  LocationLevel3Code = g.Max(x => x.LocationLevel3Code),
												  PropertyStr1Code = g.Max(x => x.PropertyStr1Code),
												  PropertyStr4Code = g.Max(x => x.PropertyStr4Code),
												  PropertyStr5Code = g.Max(x => x.PropertyStr5Code),
												  PropertyStr6Code = g.Max(x => x.PropertyStr6Code),
												  PropertyStr7Code = g.Max(x => x.PropertyStr7Code),
												  PropertyStr8Code = g.Max(x => x.PropertyStr8Code),
												  PropertyStr10Code = g.Max(x => x.PropertyStr10Code),
												  PropertyStr11Code = g.Max(x => x.PropertyStr11Code),
												  PropertyStr7Name = g.Max(x => x.PropertyStr7Name),
												  PropertyStr6Name = g.Max(x => x.PropertyStr6Name),
												  SerialNumberSupplier = g.Max(x => x.SerialNumberSupplier),

											  };
							currentInventoryAdvancedList = sumQuantity.ToList();
							
						}
					}

				
				

					 if (currentInventoryAdvancedList != null)
					 {
						//===================ImportDomainEnum.ExportInventProduct
						//===================WriteRow - All - iturAnalyzes
						foreach (CurrentInventoryAdvanced current in currentInventoryAdvancedList) //selectParams
						{
							if (cancellationToken.IsCancellationRequested == true) break;
							if (CountRow % 100 == 0) countAction(CountRow);
							CountRow++;

							List<PreviousInventory> details = new List<PreviousInventory>();
							if (currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter3)
							{
								if (string.IsNullOrWhiteSpace(current.SerialNumberLocal) == false)
								{
									List<PreviousInventory> listPreviousInventory = _previousInventoryEFRepository.
										GetListBySerialNumberLocal(current.SerialNumberLocal, fromPathDB);
									foreach (var detail in listPreviousInventory)
									{
										if (this._productMakatDictionary.ContainsKey(detail.ItemCode) == true)
										{
											ProductMakat pm = this._productMakatDictionary[detail.ItemCode];
											detail.PropExtenstion9 = pm.Name != null ? pm.Name : "";
										}
									}
									details = listPreviousInventory;
								}
							}

							if (currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter4)
							{
								//if (string.IsNullOrWhiteSpace(current.SerialNumberSupplier) == false)
								//{
								//	List<PreviousInventory> listPreviousInventory = _previousInventoryEFRepository.
								//	GetListBySerialNumberSupplier(current.SerialNumberSupplier, fromPathDB);
								if (string.IsNullOrWhiteSpace(current.ItemCode) == false)
								{
									List<PreviousInventory> listPreviousInventory = _previousInventoryEFRepository.
										GetListByItemCode(current.ItemCode, fromPathDB);
									foreach (var detail in listPreviousInventory)
									{
										if (this._productMakatDictionary.ContainsKey(detail.ItemCode) == true)
										{
											ProductMakat pm = this._productMakatDictionary[detail.ItemCode];
											detail.PropExtenstion9 = pm.Name != null ? pm.Name : "";
											//detail.PropExtenstion10 = pm.UnitTypeCode != null ? pm.UnitTypeCode : "";
										}
									}
									details = listPreviousInventory;
								}
							}

							if (currentInventoryWriter == WriterEnum.ExportCurrentInventoryMerkavaXslxWriter5)
							{
								if (string.IsNullOrWhiteSpace(current.ItemCode) == false)
								{
									List<PreviousInventory> listPreviousInventory = _previousInventoryEFRepository.
										GetListByItemCode(current.ItemCode, fromPathDB);
									foreach (var detail in listPreviousInventory)
									{
										if (this._productMakatDictionary.ContainsKey(detail.ItemCode) == true)
										{
											ProductMakat pm = this._productMakatDictionary[detail.ItemCode];
											detail.PropExtenstion9 = pm.Name != null ? pm.Name : "";
											//detail.PropExtenstion10 = pm.UnitTypeCode != null ? pm.UnitTypeCode : "";
										}
									}
									details = listPreviousInventory;
								}
							}

							//AddRow
							this._exportCurrentInventoryWriter.AddRow(sw, current, CountRow, ERPNum, INVDate, separator, parms, details );
						
						}
					 }
					 
				}

				//AddFooter
				this._exportCurrentInventoryWriter.AddFooter(sw, CountRow, ERPNum, INVDate, separator, parms);
			}
			catch (Exception error)
			{
				_logger.ErrorException("WriteToFile", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);
			}

			//if (needCloseSw == true)
			//{
			//	sw.Close();
			//}
			//Localization.Resources.Log_TraceRepositoryResult1058%"Write to File [{0}]"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1058, toPathFile));
		}


		private void ReFillProductNameFromCatalog(CurrentInventoryAdvanced currentInventoryAdvanced, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				currentInventoryAdvanced.CatalogItemName = pm.Name != null ? pm.Name : "";
			}
			else
			{
				currentInventoryAdvanced.CatalogItemName = "";
			}
		}

		//private void ReFillUnitCodeFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		//{
		//	//string makat = iturAnalyzes.Makat; 
		//	if (this._productMakatDictionary.ContainsKey(makat) == true)
		//	{
		//		ProductMakat pm = this._productMakatDictionary[makat];
		//		iturAnalyzes.UnitTypeCode = pm.UnitTypeCode != null ? pm.UnitTypeCode : "";
		//	}
		//	else
		//	{
		//		iturAnalyzes.UnitTypeCode = "";
		//	}
		//}

		//private void ReFillSectionCodeFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		//{
		//	//string makat = iturAnalyzes.Makat;
		//	if (this._productMakatDictionary.ContainsKey(makat) == true)
		//	{
		//		ProductMakat pm = this._productMakatDictionary[makat];
		//		iturAnalyzes.SectionCode = pm.SectionCode != null ? pm.SectionCode : "";
		//	}
		//	else
		//	{
		//		iturAnalyzes.SectionCode = "";
		//	}
		//}

		//private void ReFillProductNameFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		//{
		//	//string makat = iturAnalyzes.Makat;
		//	if (this._productMakatDictionary.ContainsKey(makat) == true)
		//	{
		//		ProductMakat pm = this._productMakatDictionary[makat];
		//		iturAnalyzes.ProductName = pm.Name != null ? pm.Name : "";
		//	}
		//	else
		//	{
		//		iturAnalyzes.ProductName = "";
		//	}
		//}

		//private void ReFillNameAndBalanceOriginalERPAndCountInParentPackAndPriceFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat, PriceCodeEnum priceCode)
		//{
		//	//string makat = iturAnalyzes.Makat;
		//	if (this._productSimpleDictionary.ContainsKey(makat) == true)
		//	{
		//		ProductSimple productSimple = this._productSimpleDictionary[makat];
		//		iturAnalyzes.ProductName = productSimple.Name != null ? productSimple.Name : "";
		//		iturAnalyzes.QuantityOriginalERP = productSimple.BalanceQuantityERP;
		//		iturAnalyzes.PriceBuy = 	productSimple.PriceBuy;
		//		iturAnalyzes.PriceSale = productSimple.PriceSale;
		//		iturAnalyzes.Price = iturAnalyzes.PriceBuy;
		//		if (priceCode == PriceCodeEnum.PriceSale) iturAnalyzes.Price = iturAnalyzes.PriceSale;
		//		//else if (priceCode == PriceCodeEnum.PriceExtra) iturAnalyzes.PriceExtra = iturAnalyzes.PriceExtra;
		//		iturAnalyzes.CountInParentPack = productSimple.CountInParentPack;			 //???
		//	}
		//	else
		//	{
		//		iturAnalyzes.ProductName = "";
		//		iturAnalyzes.QuantityOriginalERP = 0.0;
		//		iturAnalyzes.PriceBuy = 0.0;
		//		iturAnalyzes.PriceSale = 0.0;
		//		iturAnalyzes.Price = 0.0;
		//		iturAnalyzes.CountInParentPack = 1;
		//	}
		//}

		//private static string ReFillBarcodeFromCatalog(Dictionary<string, string> productMakatBarcodesDictionary, string makat)
		//{
		//	string barcode = "";
		//	if (productMakatBarcodesDictionary.ContainsKey(makat) == true)
		//	{
		//		barcode = productMakatBarcodesDictionary[makat];
		//	}
		//	return barcode;
		//}

		//// если вместо баркода Makat - его заменить на первый barcode в списке , если нет не одного баркода - оставляем макат
		//private static string ChangeMakat2BarcodeFromCatalog(Dictionary<string, string> productMakatBarcodesDictionary, string makat)
		//{
		//	string barcode = makat;
		//	if (productMakatBarcodesDictionary.ContainsKey(makat) == true)
		//	{
		//		string barcodes = productMakatBarcodesDictionary[makat];
		//		string[] barcodesArray = barcodes.Split(',');
		//		if (barcodesArray.Length > 0)
		//			barcode = barcodesArray[0];
		//	}
		//	else
		//	{
		//		barcode = makat;
		//	}
			
		//	return barcode;
		//}

	
	
	}
}
