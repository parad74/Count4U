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
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductSimpleERPFileRepository : BaseExportFileRepository, IExportInventProductSimpleRepository
	{
		//private readonly IMakatRepository _makatRepository;
		//private Dictionary<string, ProductMakat> _makatDictionary;
		private IInventProductRepository _inventProductRepository;
		private IExportInventProductFileWriter _exportInventProductFileWriter;
		private Dictionary<string, ProductMakat> _productMakatDictionary;
		private Dictionary<string, ProductMakat> _inventProductMakatContansOnlyOneIturDictionary;
		private Dictionary<string, string> _iturERPCodeDictionary;
		private Dictionary<string, ProductSimple> _productSimpleDictionary;
		private Dictionary<string, ProductTagSimple> _productTagSimpleDictionary;
	
		//private IInventProductSimpleParser _inventProductParser;
		private static long CountRow;
		private Dictionary<ImportProviderParmEnum, object> Parms;

		public ExportInventProductSimpleERPFileRepository(
			IServiceLocator serviceLocator,
			ILog log,
			IInventProductRepository inventProductRepository
			//IMakatRepository makatRepository
			)
			: base(log, serviceLocator)
		{
			//if (makatRepository == null) throw new ArgumentNullException("makatRepository");
			//this._makatDictionary = new Dictionary<string, ProductMakat>();
			//this._makatRepository = makatRepository;
			this._inventProductRepository = inventProductRepository;
			this._productMakatDictionary = new Dictionary<string, ProductMakat>();
			this._productSimpleDictionary = new Dictionary<string, ProductSimple>();
			this._productTagSimpleDictionary = new Dictionary<string, ProductTagSimple>();
			this._inventProductMakatContansOnlyOneIturDictionary = new Dictionary<string, ProductMakat>();
			this._iturERPCodeDictionary = new Dictionary<string, string>();
		}

		

		public void WriteToFile(string fromPathDB, string toPathFile,
			//InventProductSimpleParserEnum inventProductSimpleParserEnum,
			//ExportProviderEnum exportProviderEnum,
			WriterEnum inventProductWriter,
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

			Parms = parms;
			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1057%"Cancel Write to File [{0}] "
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1057, toPathFile));
				return;
			}

			PriceCodeEnum priceCode = parms.GetPriceCodeEnumFromParm();

			//this._inventProductParser = base.GetInventProductSimpleParserInstance(inventProductSimpleParserEnum);
			//if (this._inventProductParser == null) return;

			//Localization.Resources.Log_TraceRepository1015%"Export to [{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1015, "InventProduct", "ExportInventProductSimpleERPFileRepository"));

			this._exportInventProductFileWriter = this._serviceLocator.GetInstance<IExportInventProductFileWriter>(inventProductWriter.ToString());
			if (this._exportInventProductFileWriter == null) throw new ArgumentNullException(inventProductWriter.ToString()  + " is null");

			//Localization.Resources.Log_TraceRepository1040%"[{0}]  is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "WriterEnum", inventProductWriter.ToString()));

			//Dictionary<string, ProductMakat> productMakatDictionary = base.GetProductMakatDictionary(fromPathDB, true);
			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1057%"Cancel Write to File [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1057, toPathFile));
				return;
			}

				Dictionary<string, string> productMakatBarcodesDictionary = new Dictionary<string, string>();
			bool makatWithoutMask = parms.GetBoolValueFromParm(ImportProviderParmEnum.MakatWithoutMask);



			if (   importType.Contains(ImportDomainEnum.ExistBarcode) == true
				|| importType.Contains(ImportDomainEnum.ChangeMakat2Barcode) == true)
			{
				//if (makatWithoutMask == true)	  // MakatOriginal
				//{
				productMakatBarcodesDictionary = base.GetProductMakatBarcodesDictionary(fromPathDB, refill);  //!!
				//}
				//else	   // Makat with mask
				//{
				//    productMakatBarcodesDictionary = base.GetProductMakatBarcodesDictionary(fromPathDB, true);
				//}
			}

			
		
			   	if (importType.Contains(ImportDomainEnum.SectionCodeContains) == true
				|| importType.Contains(ImportDomainEnum.UnitTypeCodeContains) == true
                || importType.Contains(ImportDomainEnum.FamilyCodeContains) == true
				 || importType.Contains(ImportDomainEnum.SupplierCodeContains) == true
				|| importType.Contains(ImportDomainEnum.SubSectionCodeContains) == true 
				|| importType.Contains(ImportDomainEnum.ProductNameContains) == true)
			{
				IMakatRepository makatRepository = base._serviceLocator.GetInstance<IMakatRepository>();
				this._productMakatDictionary = makatRepository.GetProductUnitTypeDictionary(fromPathDB, refill);
			}

				if (importType.Contains(ImportDomainEnum.FullDataFromContains) == true
					|| importType.Contains(ImportDomainEnum.CountInParentPackAndPricesFromCatalog) == true
					|| importType.Contains(ImportDomainEnum.NameAndBalanceOriginalERPAndCountInParentPackAndPriceContains) == true)
				{
					IProductRepository productRepository = base._serviceLocator.GetInstance<IProductRepository>();
					productRepository.FillProductSimpleDictionary(fromPathDB,  "M", out this._productSimpleDictionary);
				}

				if (importType.Contains(ImportDomainEnum.TagContains) == true)
				{
					IProductRepository productRepository = base._serviceLocator.GetInstance<IProductRepository>();
					this._productTagSimpleDictionary = productRepository.GetProductTagDictionary(fromPathDB );
				}
		

			if (importType.Contains(ImportDomainEnum.IturERPCodeContains) == true)  //!!
			{
				IIturRepository iturRepository = base._serviceLocator.GetInstance<IIturRepository>();
				Dictionary<string, Itur> iturDictionary = iturRepository.GetIturDictionary(fromPathDB, refill);
				foreach (KeyValuePair<string, Itur> keyValuePair in iturDictionary)
				{
					if (string.IsNullOrWhiteSpace(keyValuePair.Key) == false)
					{
						if (string.IsNullOrWhiteSpace(keyValuePair.Value.ERPIturCode) == false)
						{
							this._iturERPCodeDictionary[keyValuePair.Key] = keyValuePair.Value.ERPIturCode;
						}
						else
						{
							this._iturERPCodeDictionary[keyValuePair.Key] = keyValuePair.Key;
						}
					}
				}
			}
			

			if (importType.Contains(ImportDomainEnum.IncludeIPIfContansOnlyOneItur) == true)
			{
				this._inventProductMakatContansOnlyOneIturDictionary = this._inventProductRepository.GetIPCountByMakatsAndIturCode(null, fromPathDB); //!!
			}

			//Localization.Resources.Log_TraceRepositoryResult1016%"Start Write to File [{0}] "
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1016, toPathFile));

			string ERPNum = parms.GetStringValueFromParm(ImportProviderParmEnum.ERPNum);
			string INVDate = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorDate);
			string INVDate1 = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorDate1);
			
			//ExportFileType fileType = parms.GetFileTypeFromParm();
			//bool barcodeWithoutMask = parms.GetBoolValueFromParm(ImportProviderParmEnum.BarcodeWithoutMask);
			IIturAnalyzesRepository iturAnalyzesRepository = this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			 string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			if (inventProductWriter == WriterEnum.ExportInventProductAS400JaforaERPFileWriter
				|| inventProductWriter == WriterEnum.ExportInventProductAS400JaforaERPFileWriter1
				|| inventProductWriter == WriterEnum.ExportInventProductOrenOriginalsERPFileWriter
				|| inventProductWriter == WriterEnum.ExportInventProductAS400HamashbirERPFileWriter
				)
			{
				// надо передать вторую дату 
				separator = INVDate1;
			}

			//var domainObjects = iturAnalyzesRepository.GetIturAnalyzesCollection(fromPathDB, true);

			SelectParams selectParams = new SelectParams();
			//selectParams.SortParams = "MakatOriginal";
			List<string> typeMakatList = new List<string>();

	
			bool excludeNotExistingInCatalog = parms.GetBoolValueFromParm(ImportProviderParmEnum.ExcludeNotExistingInCatalog);

			if (importType.Contains(ImportDomainEnum.ExportInventProductW) == true)  //Add record for every line that is in the Invetor and the makat is not exist in Catalog
			{
				typeMakatList.Add(TypeMakatEnum.W.ToString());
			}
			else if (excludeNotExistingInCatalog == true) //Exclude Items with Makat not exists in Catalog
			{
				typeMakatList.Add(TypeMakatEnum.M.ToString());
				typeMakatList.Add(TypeMakatEnum.B.ToString());		
			}
			else //writer в фойл для всех продуктов 
			{
				typeMakatList.Add(TypeMakatEnum.M.ToString());
				typeMakatList.Add(TypeMakatEnum.B.ToString());		//??
				typeMakatList.Add(TypeMakatEnum.W.ToString());
			}
			selectParams.FilterStringListParams["TypeMakat"] = new FilterStringListParam()	{Values = typeMakatList };
			if (inventProductWriter == WriterEnum.ExportInventProductXtechMeuhedetERPFileWriter)
			{
				selectParams.SortParams = "IturCode ASC";
			}

			if (importType.Contains(ImportDomainEnum.ExportInventProductByLocationCode) == true)
			{
				string locationCode = parms.GetStringValueFromParm(ImportProviderParmEnum.LocationCode);
				if (string.IsNullOrWhiteSpace(locationCode) == false)
				{
					selectParams.FilterStringListParams.Add("LocationCode", new FilterStringListParam() { Values = new List<string>() { locationCode } });
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

			//bool needCloseSw = false;
			//if (sw == null)
			//{
			//	sw = GetStreamWriter(toPathFile, encoding);
			//	needCloseSw = true;
			//}

			try
			{
				CountRow = 0;
				countAction(CountRow);
				if (importType.Contains(ImportDomainEnum.ExportInventProductNotHeader) == false) //не заполнять заголовок всего файла
				{
					this._exportInventProductFileWriter.AddHeader(sw, ERPNum, INVDate, parms);
				}

				//WriteHeader(sw, writerEnum, ERPNum);

				//===================WriteRowSimple - Sum - IturAnalyzesSimple
				if (importType.Contains(ImportDomainEnum.ExportSumInventProduct) == true)
				{
					// =========== Makat Without Mask  => MakatOriginal
					if (makatWithoutMask == true)
					{
						IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null;
						if (inventProductWriter == WriterEnum.ExportInventProductYarpaERPFileWriter
							//		|| inventProductWriter == WriterEnum.ExportInventProductWarehouseXslxFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductGeneralCSVERPFileWriter) //???
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditInPackByMakatsOriginal(selectParams, fromPathDB, true);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductAS400LeumitERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditInPackByMakatsOriginalWithoutAddQuantityInPackEdit(selectParams, fromPathDB);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductMaccabiPharmSAPERPFileWriter
						 || inventProductWriter == WriterEnum.ExportInventProductNikeIntERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditInPackByMakatsOriginalLikeNumber(selectParams, fromPathDB, true);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductNibitERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductTafnitMatrixERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductOrenOriginalsERPFileWriter1
							//|| inventProductWriter == WriterEnum.ExportInventProductNimrodAvivERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductAS400AprilERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductPriorityKedsShowRoomERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsAndIturCode(selectParams, fromPathDB, refill);
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.IturCode).ThenBy(x => x.Makat).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductMPLFileWriter1)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsAndERPIturCode(selectParams, fromPathDB, refill);//!!
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.IturCode).ThenBy(x => x.Makat).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductNimrodAvivERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductAS400HamashbirERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByBakcodesAndIturCode(selectParams, fromPathDB, refill);
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.IturCode).ThenBy(x => x.Barcode).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductPriorityAldoERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatAndBakcodes(selectParams, fromPathDB, refill);
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.Makat).ThenBy(x => x.Barcode).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductAS400JaforaERPFileWriter1) // список продуктов, которые не попали в инвентаризацию
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIturAnalyzesSumEnumerable(null, fromPathDB, true, true, null, false, importType).ToList();
							iturAnalyzesList = iturAnalyzesList.Where(x => x.FromCatalogType == (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct).ToList();
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductH_MERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsPlusCode(selectParams, fromPathDB, true).ToList();
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductXtechMeuhedetXlsxERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductXtechMeuhedetXlsxERPFileWriter2)
						{
							//Dictionary<object, object> param = new Dictionary<object, object>();
							//param[ImportProviderParmEnum.IncludeProductMakatWithoutInventProduct] = "1";
							//iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsOriginal(selectParams, fromPathDB, true, param).ToList();
							//включая згачения из каталога, которые не были использованы в инвентаризации

							iturAnalyzesList = iturAnalyzesRepository.GetIturAnalyzesSumEnumerable(null, fromPathDB, true, true, null, false, importType).ToList();// список продуктов, которые не попали в инвентаризацию
						}
						else
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsOriginal(selectParams, fromPathDB, true);
							//if (inventProductWriter == WriterEnum.ExportInventProductAS400JaforaERPFileWriter)
							//{
							//	iturAnalyzesList = iturAnalyzesList.Where(x => x.QuantityEdit != 0).ToList();
							//}
							//else 
						}

						//AddHeaderSum
						string prevIturCode = "";
						this._exportInventProductFileWriter.AddHeaderSum(sw, iturAnalyzesList, ERPNum);
						//============ ImportDomainEnum.ExportSumInventProduct
						// =========== Makat Without Mask  => MakatOriginal
						if (makatWithoutMask == true)
							foreach (IturAnalyzesSimple iturAnalyzes in iturAnalyzesList) //selectParams
							{
								if (cancellationToken.IsCancellationRequested == true) break;
								if (CountRow % 100 == 0) countAction(CountRow);
								CountRow++;

								if (importType.Contains(ImportDomainEnum.ExistBarcode) == true)
								{   //inMakat = Makat Without Mask  => MakatOriginal
									//надо получить сначала просто Makat
									string makat1 = iturAnalyzes.MakatOriginal;
									iturAnalyzes.Barcode = ReFillBarcodeFromCatalog(productMakatBarcodesDictionary, makat1);
								}
								if (importType.Contains(ImportDomainEnum.ChangeMakat2Barcode) == true)
								{
									//inMakat = Makat Without Mask  => MakatOriginal
									//надо получить сначала просто Makat
									string makat1 = iturAnalyzes.MakatOriginal;
									if (iturAnalyzes.Barcode.Trim() == makat1)
									{
										iturAnalyzes.Barcode = ChangeMakat2BarcodeFromCatalog(productMakatBarcodesDictionary, makat1);
									}
								}

								if (importType.Contains(ImportDomainEnum.UnitTypeCodeContains) == true)
								{
									ReFillUnitCodeFromCatalog(iturAnalyzes, iturAnalyzes.MakatOriginal);
								}
								if (importType.Contains(ImportDomainEnum.TagContains) == true)
								{
									ReFillTagFromCatalog(iturAnalyzes, iturAnalyzes.MakatOriginal);
								}
								if (importType.Contains(ImportDomainEnum.SectionCodeContains) == true)
								{
									ReFillSectionCodeFromCatalog(iturAnalyzes, iturAnalyzes.MakatOriginal);
								}
								if (importType.Contains(ImportDomainEnum.SubSectionCodeContains) == true)
								{
									ReFillSubSectionCodeFromCatalog(iturAnalyzes, iturAnalyzes.MakatOriginal);
								}
								if (importType.Contains(ImportDomainEnum.SupplierCodeContains) == true)
								{
									ReFillSupplierCodeFromCatalog(iturAnalyzes, iturAnalyzes.MakatOriginal);
								}
								if (importType.Contains(ImportDomainEnum.FamilyCodeContains) == true)
								{
									ReFillFamilyCodeFromCatalog(iturAnalyzes, iturAnalyzes.MakatOriginal);
								}
								if (importType.Contains(ImportDomainEnum.ProductNameContains) == true)
								{
									ReFillProductNameFromCatalog(iturAnalyzes, iturAnalyzes.MakatOriginal);
								}

								if (importType.Contains(ImportDomainEnum.NameAndBalanceOriginalERPAndCountInParentPackAndPriceContains) == true)
								{
									ReFillNameAndBalanceOriginalERPAndCountInParentPackAndPriceFromCatalog(iturAnalyzes, iturAnalyzes.Makat, priceCode);
								}

								if (importType.Contains(ImportDomainEnum.IncludeIPIfContansOnlyOneItur) == true) // список продуктов, которые появляются только в одном Itur 
								{
									iturAnalyzes.Count = 1;
									if (this._inventProductMakatContansOnlyOneIturDictionary.ContainsKey(iturAnalyzes.Makat))
									{
										iturAnalyzes.Count = 2;
									}
								}
								if (importType.Contains(ImportDomainEnum.IturERPCodeContains) == true)
								{
									if (this._iturERPCodeDictionary.ContainsKey(iturAnalyzes.IturCode) == true)
									{
										string iturERPCode = this._iturERPCodeDictionary[iturAnalyzes.IturCode];

										if (string.IsNullOrWhiteSpace(iturERPCode) == false)
										{
											iturAnalyzes.IturCode = iturERPCode;
										}
									}
								}
								if (importType.Contains(ImportDomainEnum.ExportInventProductGroupHeaderByItur) == true)  //если группировка по итурам и заголовок для каждого итура
								{
									//сохраняю значение предыдущего IturCode в следующем, чтобы поймать переход между Itur
									if (iturAnalyzes.IturCode != prevIturCode)
									{
										CountRow = 1;
									}

									if (prevIturCode == "")
									{
										prevIturCode = iturAnalyzes.IturCode;
									}
									else
									{
										iturAnalyzes.Barcode = prevIturCode; //храним prevIturCode

										prevIturCode = iturAnalyzes.IturCode;
									}
								}

								//AddRowSimple
								this._exportInventProductFileWriter.AddRowSimple(sw, iturAnalyzes, CountRow, ERPNum, INVDate, separator);
							}  //foreach iturAnalyzes

						//AddFooterSum
						this._exportInventProductFileWriter.AddFooterSum(sw, CountRow, iturAnalyzesList, ERPNum);
					}
					//============ ImportDomainEnum.ExportSumInventProduct
					// =========== Makat With Mask	  => Makat
					else
					{
						IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null;
						if (inventProductWriter == WriterEnum.ExportInventProductYarpaERPFileWriter
							//		|| inventProductWriter == WriterEnum.ExportInventProductWarehouseXslxFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductGeneralCSVERPFileWriter) //???
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditInPackByMakats(selectParams, fromPathDB);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductAS400LeumitERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditInPackByMakatsWithoutAddQuantityInPackEdit(selectParams, fromPathDB);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductMaccabiPharmSAPERPFileWriter
									|| inventProductWriter == WriterEnum.ExportInventProductNikeIntERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditInPackByMakatsLikeNumber(selectParams, fromPathDB);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductMade4NetERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsAndExpiredDate(selectParams, fromPathDB);
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.IturCode).ThenBy(x => x.Makat).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductNibitERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductTafnitMatrixERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductOrenOriginalsERPFileWriter1
							//	|| inventProductWriter == WriterEnum.ExportInventProductNimrodAvivERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductAS400AprilERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductPriorityKedsShowRoomERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsAndIturCode(selectParams, fromPathDB, refill);//!!
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.IturCode).ThenBy(x => x.Makat).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductMPLFileWriter1)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsAndERPIturCode(selectParams, fromPathDB, refill);//!!
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.IturCode).ThenBy(x => x.Makat).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductNimrodAvivERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductAS400HamashbirERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByBakcodesAndIturCode(selectParams, fromPathDB, refill);
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.IturCode).ThenBy(x => x.Barcode).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductPriorityAldoERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatAndBakcodes(selectParams, fromPathDB, refill);
							iturAnalyzesList = iturAnalyzesList.OrderBy(x => x.Makat).ThenBy(x => x.Barcode).Select(x => x);
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductAS400JaforaERPFileWriter1)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIturAnalyzesSumEnumerable(null, fromPathDB, true, true, null, false, importType).ToList();// список продуктов, которые не попали в инвентаризацию
							iturAnalyzesList = iturAnalyzesList.Where(x => x.FromCatalogType == (int)FromCatalogTypeEnum.ProductMakatWithoutInventProduct).ToList();
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductH_MERPFileWriter)
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakatsPlusCode(selectParams, fromPathDB, true).ToList();
						}
						else if (inventProductWriter == WriterEnum.ExportInventProductXtechMeuhedetXlsxERPFileWriter
							|| inventProductWriter == WriterEnum.ExportInventProductXtechMeuhedetXlsxERPFileWriter2)
						{
							//Dictionary<object, object> param = new Dictionary<object, object>();
							//param[ImportProviderParmEnum.IncludeProductMakatWithoutInventProduct] = "1";
							//включая згачения из каталога, которые не были использованы в инвентаризации
							iturAnalyzesList = iturAnalyzesRepository.GetIturAnalyzesSumEnumerable(null, fromPathDB, true, true, null, false, importType).ToList();// список продуктов, которые не попали в инвентаризацию
						}
						else
						{
							iturAnalyzesList = iturAnalyzesRepository.GetIPSumQuantityEditByMakats(selectParams, fromPathDB, true);
							//if (inventProductWriter == WriterEnum.ExportInventProductAS400JaforaERPFileWriter)
							//{
							//	iturAnalyzesList = iturAnalyzesList.Where(x => x.QuantityEdit != 0).ToList();
							//}
							//else 
						}

						//AddHeaderSum
						this._exportInventProductFileWriter.AddHeaderSum(sw, iturAnalyzesList, ERPNum);

						string prevIturCode = "";
						foreach (IturAnalyzesSimple iturAnalyzes in iturAnalyzesList)
						{
							if (cancellationToken.IsCancellationRequested == true) break;
							if (CountRow % 100 == 0) countAction(CountRow);
							CountRow++;

							if (importType.Contains(ImportDomainEnum.ExistBarcode) == true)
							{  // in Makat == Makat With Mask => Makat
								iturAnalyzes.Barcode = ReFillBarcodeFromCatalog(productMakatBarcodesDictionary, iturAnalyzes.Makat);
							}
							if (importType.Contains(ImportDomainEnum.ChangeMakat2Barcode) == true)
							{
								if (iturAnalyzes.Barcode.Trim() == iturAnalyzes.Makat.Trim())
								{
									iturAnalyzes.Barcode = ChangeMakat2BarcodeFromCatalog(productMakatBarcodesDictionary, iturAnalyzes.Makat);
								}
							}

							if (importType.Contains(ImportDomainEnum.UnitTypeCodeContains) == true)
							{
								ReFillUnitCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
							}
							if (importType.Contains(ImportDomainEnum.TagContains) == true)
							{
								ReFillTagFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
							}
							if (importType.Contains(ImportDomainEnum.SectionCodeContains) == true)
							{
								ReFillSectionCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
							}
							if (importType.Contains(ImportDomainEnum.SubSectionCodeContains) == true)
							{
								ReFillSubSectionCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
							}
							if (importType.Contains(ImportDomainEnum.SupplierCodeContains) == true)
							{
								ReFillSupplierCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
							}
							if (importType.Contains(ImportDomainEnum.FamilyCodeContains) == true)
							{
								ReFillFamilyCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
							}
							if (importType.Contains(ImportDomainEnum.NameAndBalanceOriginalERPAndCountInParentPackAndPriceContains) == true)
							{
								ReFillNameAndBalanceOriginalERPAndCountInParentPackAndPriceFromCatalog(iturAnalyzes, iturAnalyzes.Makat, priceCode);
							}

							if (importType.Contains(ImportDomainEnum.ProductNameContains) == true)
							{
								ReFillProductNameFromCatalog(iturAnalyzes, iturAnalyzes.MakatOriginal);
							}
							if (importType.Contains(ImportDomainEnum.IncludeIPIfContansOnlyOneItur) == true) // список продуктов, которые появляются только в одном Itur 
							{
								iturAnalyzes.Count = 1;
								if (this._inventProductMakatContansOnlyOneIturDictionary.ContainsKey(iturAnalyzes.Makat))
								{
									iturAnalyzes.Count = 2;
								}
							}

							if (importType.Contains(ImportDomainEnum.IturERPCodeContains) == true)
							{
								if (this._iturERPCodeDictionary.ContainsKey(iturAnalyzes.IturCode) == true)
								{
									string iturERPCode = this._iturERPCodeDictionary[iturAnalyzes.IturCode];
									if (string.IsNullOrWhiteSpace(iturERPCode) == false)
									{
										iturAnalyzes.IturCode = iturERPCode;
									}

								}
							}

							if (importType.Contains(ImportDomainEnum.ExportInventProductGroupHeaderByItur) == true)  //если группировка по итурам и заголовок для каждого итура
							{
								//сохраняю значение предыдущего IturCode в следующем, чтобы поймать переход между Itur
								if (iturAnalyzes.IturCode != prevIturCode)
								{
									CountRow = 1;
								}

								if (prevIturCode == "")
								{
									prevIturCode = iturAnalyzes.IturCode;
								}
								else
								{
									iturAnalyzes.Barcode = prevIturCode; //храним prevIturCode

									prevIturCode = iturAnalyzes.IturCode;
								}
							}



							// AddRowSimple
							this._exportInventProductFileWriter.AddRowSimple(sw, iturAnalyzes, CountRow, ERPNum, INVDate, separator);

						}  //foreach  iturAnalyzes

						// AddFooterSum
						this._exportInventProductFileWriter.AddFooterSum(sw, CountRow, iturAnalyzesList, ERPNum);
					}
				}     // ImportDomainEnum.ExportSumInventProduct

				//===================ImportDomainEnum.ExportInventProduct
				//===================WriteRow - All - iturAnalyzes
				else if (importType.Contains(ImportDomainEnum.ExportInventProduct) == true)     //WriteRow
				{
					object argument = null;
					IturAnalyzesCollection iturAnalyzesCollection = new IturAnalyzesCollection();
					if (inventProductWriter == WriterEnum.ExportInventProductNimrodAvivERPFileWriter1)
					{
						iturAnalyzesCollection = iturAnalyzesRepository.GetIPSumQuantityEditByIturCode(selectParams, fromPathDB, false); // , refillIturStatistic в адаптере ExportInventProductNimrodAviv
					}

					else
					{
						iturAnalyzesCollection = iturAnalyzesRepository.GetIACollection(selectParams, fromPathDB, refill);
					}

					if (importType.Contains(ImportDomainEnum.SortInventProductByItur) == true)  //если группировка по итурам и заголовок для каждого итура
					{
						iturAnalyzesCollection = IturAnalyzesCollection.FromEnumerable(iturAnalyzesCollection.OrderBy(x => x.IturCode).Select(x => x));
					}

					if (importType.Contains(ImportDomainEnum.SortInventProductBySubSectionCode) == true)  //если группировка по SubSectionCode 
					{
						iturAnalyzesCollection = IturAnalyzesCollection.FromEnumerable(iturAnalyzesCollection.OrderBy(x => x.SubSessionCode).ThenBy(x => x.SectionCode).Select(x => x));
					}

					if (importType.Contains(ImportDomainEnum.ExportInventProductGroupHeaderByItur) == true)  //если группировка по итурам и заголовок для каждого итура
					{
						if (iturAnalyzesCollection.Count() > 0)
						{
							this._exportInventProductFileWriter.AddHeader(sw, ERPNum, INVDate, parms);
						}
					}


					//===================ImportDomainEnum.ExportInventProduct
					//===================WriteRow - All - iturAnalyzes
					foreach (IturAnalyzes iturAnalyzes in iturAnalyzesCollection) //selectParams
					{
						if (cancellationToken.IsCancellationRequested == true) break;
						if (CountRow % 100 == 0) countAction(CountRow);
						CountRow++;

						if (importType.Contains(ImportDomainEnum.ExistBarcode) == true)
						{
							iturAnalyzes.Barcode = ReFillBarcodeFromCatalog(productMakatBarcodesDictionary, iturAnalyzes.Makat);
						}
						if (importType.Contains(ImportDomainEnum.ChangeMakat2Barcode) == true)
						{
							if (iturAnalyzes.Barcode.Trim() == iturAnalyzes.Makat.Trim())
							{
								iturAnalyzes.Barcode = ChangeMakat2BarcodeFromCatalog(productMakatBarcodesDictionary, iturAnalyzes.Makat);
							}
						}

						if (importType.Contains(ImportDomainEnum.SectionCodeContains) == true)
						{
							ReFillSectionCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
						}
						if (importType.Contains(ImportDomainEnum.SubSectionCodeContains) == true)
						{
							ReFillSubSectionCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
						}
						if (importType.Contains(ImportDomainEnum.SupplierCodeContains) == true)
						{
							ReFillSupplierCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
						}
						if (importType.Contains(ImportDomainEnum.FamilyCodeContains) == true)
						{
							ReFillFamilyCodeFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
						}
						if (importType.Contains(ImportDomainEnum.CountInParentPackAndPricesFromCatalog) == true)
						{
							ReFillCountInParentPackAndPricesFromCatalog(iturAnalyzes, iturAnalyzes.Makat);
						}


						//AddRow
						this._exportInventProductFileWriter.AddRow(sw, iturAnalyzes, CountRow, ERPNum, INVDate, separator, argument);

					}
				}
				else if (importType.Contains(ImportDomainEnum.ExportInventProductOnly) == true)
				{
					object argument = null;
					InventProducts inventProducts = new InventProducts();
					if (inventProductWriter == WriterEnum.ExportInventProductNativExportErpERPFileWriter)
					{
						inventProducts = _inventProductRepository.GetInventProducts(selectParams, fromPathDB);
						IPropertyStrRepository propertyStrRepository = _serviceLocator.GetInstance<IPropertyStrRepository>();
						string domainObjectType = DomainObjectTypeEnum.PropertyExportErpDecorator3.ToString();
						Dictionary<string, PropertyStr> dictionary =
							propertyStrRepository.GetPropertyExportErpDecoratorProperty(domainObjectType, fromPathDB);
						argument = dictionary;
					}

					//===================WriteRow - All - iturAnalyzes
					foreach (InventProduct inventProduct in inventProducts) //selectParams
					{
						if (cancellationToken.IsCancellationRequested == true) break;
						if (CountRow % 100 == 0) countAction(CountRow);
						CountRow++;

						//if (importType.Contains(ImportDomainEnum.ExistBarcode) == true)
						//{
						//	inventProduct.Barcode = ReFillBarcodeFromCatalog(productMakatBarcodesDictionary, inventProduct.Makat);
						//}
						//if (importType.Contains(ImportDomainEnum.ChangeMakat2Barcode) == true)
						//{
						//	if (inventProduct.Barcode.Trim() == inventProduct.Makat.Trim())
						//	{
						//		inventProduct.Barcode = ChangeMakat2BarcodeFromCatalog(productMakatBarcodesDictionary, inventProduct.Makat);
						//	}
						//}

						//if (importType.Contains(ImportDomainEnum.SectionCodeContains) == true)
						//{
						//	ReFillSectionCodeFromCatalog(inventProduct, inventProduct.Makat);
						//}
						//if (importType.Contains(ImportDomainEnum.SubSectionCodeContains) == true)
						//{
						//	ReFillSubSectionCodeFromCatalog(inventProduct, inventProduct.Makat);
						//}
						//if (importType.Contains(ImportDomainEnum.SupplierCodeContains) == true)
						//{
						//	ReFillSupplierCodeFromCatalog(inventProduct, inventProduct.Makat);
						//}
						//if (importType.Contains(ImportDomainEnum.FamilyCodeContains) == true)
						//{
						//	ReFillFamilyCodeFromCatalog(inventProduct, inventProduct.Makat);
						//}
						//if (importType.Contains(ImportDomainEnum.CountInParentPackAndPricesFromCatalog) == true)
						//{
						//	ReFillCountInParentPackAndPricesFromCatalog(inventProduct, inventProduct.Makat);
						//}


						//AddRow
						this._exportInventProductFileWriter.AddRowInventProduct(sw, inventProduct, CountRow, ERPNum, INVDate, separator, argument);
					}
				}
					//AddFooter
				this._exportInventProductFileWriter.AddFooter(sw, CountRow, ERPNum, INVDate, parms);
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

		private void ReFillUnitCodeFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat; 
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.UnitTypeCode = pm.UnitTypeCode != null ? pm.UnitTypeCode : "";
			}
			else
			{
				iturAnalyzes.UnitTypeCode = "";
			}
		}

		private void ReFillUnitCodeFromCatalog(IturAnalyzes iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat; 
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.UnitTypeCode = pm.UnitTypeCode != null ? pm.UnitTypeCode : "";
			}
			else
			{
				iturAnalyzes.UnitTypeCode = "";
			}
		}

		private void ReFillTagFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat; 
			if (this._productTagSimpleDictionary.ContainsKey(makat) == true)
			{
				ProductTagSimple pm = this._productTagSimpleDictionary[makat];
				iturAnalyzes.Tag = pm.Tag != null ? pm.Tag : "";
				iturAnalyzes.ProductName = pm.Name != null ? pm.Name : "";
			}
			else
			{
				iturAnalyzes.Tag = "";
				iturAnalyzes.ProductName = "";
			}
		}

		

		private void ReFillSectionCodeFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.SectionCode = pm.SectionCode != null ? pm.SectionCode : "";
			}
			else
			{
				iturAnalyzes.SectionCode = "";
			}
		}

		private void ReFillSectionCodeFromCatalog(IturAnalyzes iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.SectionCode = pm.SectionCode != null ? pm.SectionCode : "";
			}
			else
			{
				iturAnalyzes.SectionCode = "";
			}
		}

		private void ReFillSubSectionCodeFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.SubSectionCode = pm.SubSectionCode != null ? pm.SubSectionCode : "";
			}
			else
			{
				iturAnalyzes.SubSectionCode = "";
			}
		}

		private void ReFillSubSectionCodeFromCatalog(IturAnalyzes iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.SubSessionCode = pm.SubSectionCode != null ? pm.SubSectionCode : "";		 //SubSectionCode
			}
			else
			{
				iturAnalyzes.SubSessionCode = "";	//SubSectionCode
			}
		}


		private void ReFillSupplierCodeFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.SupplierCode = pm.SupplierCode != null ? pm.SupplierCode : "";
			}
			else
			{
				iturAnalyzes.SupplierCode = "";
			}
		}

		private void ReFillSupplierCodeFromCatalog(IturAnalyzes iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.SupplierCode = pm.SupplierCode != null ? pm.SupplierCode : "";
			}
			else
			{
				iturAnalyzes.SupplierCode = "";
			}
		}

		private void ReFillFamilyCodeFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
        {
            //string makat = iturAnalyzes.Makat;
            if (this._productMakatDictionary.ContainsKey(makat) == true)
            {
                ProductMakat pm = this._productMakatDictionary[makat];
                iturAnalyzes.FamilyCode = pm.FamilyCode != null ? pm.FamilyCode : "";
            }
            else
            {
                iturAnalyzes.FamilyCode = "";
            }
        }

		private void ReFillFamilyCodeFromCatalog(IturAnalyzes iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.FamilyCode = pm.FamilyCode != null ? pm.FamilyCode : "";
			}
			else
			{
				iturAnalyzes.FamilyCode = "";
			}
		}

		private void ReFillProductNameFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.ProductName = pm.Name != null ? pm.Name : "";
			}
			else
			{
				iturAnalyzes.ProductName = "";
			}
		}

		private void ReFillProductNameFromCatalog(IturAnalyzes iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productMakatDictionary.ContainsKey(makat) == true)
			{
				ProductMakat pm = this._productMakatDictionary[makat];
				iturAnalyzes.ProductName = pm.Name != null ? pm.Name : "";
			}
			else
			{
				iturAnalyzes.ProductName = "";
			}
		}

		private void ReFillNameAndBalanceOriginalERPAndCountInParentPackAndPriceFromCatalog(IturAnalyzesSimple iturAnalyzes, string makat, PriceCodeEnum priceCode)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productSimpleDictionary.ContainsKey(makat) == true)
			{
				ProductSimple productSimple = this._productSimpleDictionary[makat];
				iturAnalyzes.ProductName = productSimple.Name != null ? productSimple.Name : "";
				iturAnalyzes.QuantityOriginalERP = productSimple.BalanceQuantityERP;
				iturAnalyzes.PriceBuy = 	productSimple.PriceBuy;
				iturAnalyzes.PriceSale = productSimple.PriceSale;
				iturAnalyzes.Price = iturAnalyzes.PriceBuy;
				if (priceCode == PriceCodeEnum.PriceSale) iturAnalyzes.Price = iturAnalyzes.PriceSale;
				//else if (priceCode == PriceCodeEnum.PriceExtra) iturAnalyzes.PriceExtra = iturAnalyzes.PriceExtra;
				iturAnalyzes.CountInParentPack = productSimple.CountInParentPack;			 //???
			}
			else
			{
				iturAnalyzes.ProductName = "";
				iturAnalyzes.QuantityOriginalERP = 0.0;
				iturAnalyzes.PriceBuy = 0.0;
				iturAnalyzes.PriceSale = 0.0;
				iturAnalyzes.Price = 0.0;
				iturAnalyzes.CountInParentPack = 1;
			}
		}

		private void ReFillCountInParentPackAndPricesFromCatalog(IturAnalyzes iturAnalyzes, string makat)
		{
			//string makat = iturAnalyzes.Makat;
			if (this._productSimpleDictionary.ContainsKey(makat) == true)
			{
				ProductSimple productSimple = this._productSimpleDictionary[makat];
				//iturAnalyzes.ProductName = productSimple.Name != null ? productSimple.Name : "";
				//iturAnalyzes.QuantityOriginalERP = productSimple.BalanceQuantityERP;
				iturAnalyzes.PriceBuy = productSimple.PriceBuy;
				iturAnalyzes.PriceSale = productSimple.PriceSale;
				iturAnalyzes.CountInParentPack = productSimple.CountInParentPack;			 //???
			}
			else
			{
				//iturAnalyzes.ProductName = "";
				//iturAnalyzes.QuantityOriginalERP = 0.0;
				iturAnalyzes.PriceBuy = 0.0;
				iturAnalyzes.PriceSale = 0.0;
				//iturAnalyzes.Price = 0.0;
				iturAnalyzes.CountInParentPack = 1;
			}
		}

		private static string ReFillBarcodeFromCatalog(Dictionary<string, string> productMakatBarcodesDictionary, string makat)
		{
			string barcode = "";
			if (productMakatBarcodesDictionary.ContainsKey(makat) == true)
			{
				barcode = productMakatBarcodesDictionary[makat];
			}
			return barcode;
		}

		// если вместо баркода Makat - его заменить на первый barcode в списке , если нет не одного баркода - оставляем макат
		private static string ChangeMakat2BarcodeFromCatalog(Dictionary<string, string> productMakatBarcodesDictionary, string makat)
		{
			string barcode = makat;
			if (productMakatBarcodesDictionary.ContainsKey(makat) == true)
			{
				string barcodes = productMakatBarcodesDictionary[makat];
				string[] barcodesArray = barcodes.Split(',');
				if (barcodesArray.Length > 0)
					barcode = barcodesArray[0];
			}
			else
			{
				barcode = makat;
			}
			
			return barcode;
		}

	
	
	}
}
