using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class InventProductSimpleYarpaParser : InventProductParserBase, IInventProductSimpleParser
	{
		//private readonly IFileParser _fileParser;
		//private readonly ILog _log;
		//private Dictionary<string, DocumentHeader> _documentHeaderDictionary;
		//private Dictionary<string, Itur> _iturDictionary;
		//private IDocumentHeaderRepository _documentHeaderRepository;
		//private List<BitAndRecord> _errorBitList;
		//public DateTimeFormatInfo _dtfi;

		public InventProductSimpleYarpaParser(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
	
		}

	
		/// <summary>
		/// Получение списка Product для ADO запроса
		/// </summary>
		/// <returns></returns>
		public IEnumerable<InventProduct> GetInventProducts(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString, string sessionCodeIn, //Guid workerGUID,
			Dictionary<string, ProductMakat> productMakatDictionary,
			//Dictionary<string, ProductMakat> productBarcodeDictionary,
			Dictionary<string, Itur> iturFromDBDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool firstStringH = false;
			//string newDocumentCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewDocumentCode);
			string newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле
			
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = "";
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			Dictionary<string, int> indexInRecordDictionary = parms.GetIPAdvancedFieldIndexDictionaryFromParm();
			//string newDocumentCode = Guid.NewGuid().ToString();
			string newIturCode = "";
			int IDDocumentHeader = 0;

			//Guid workerGUID = Guid.NewGuid();
			

			this._documentHeaderDictionary.Clear();
			//this._sessionDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();

			string separator = " ";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}
			//H,24/09/2012,000000349,29190005,2,000349
			//0			1			  2				  3		 4		 5 
			//B,7290008546775,5,10,B,24/09/2012,20:23:20
			//0   1                        2 3  4     5              6  

			foreach (String[] record in this._fileParser.GetRecords(fromPathFile, 
				encoding, separators,
				countExcludeFirstString))
			{
				if (record == null)	continue;
				int countRecord = record.Length;
				if (countRecord < 5)
				{
					Log.Add(MessageTypeEnum.Error,
						String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				string objectType = Convert.ToString(record[0]).Trim(" ".ToCharArray());
				Itur newItur1 = new Itur();
				

				if (objectType == "H")
				{
					firstStringH = true;
					//if (firstStringH == false) continue;
					//if (firstStringH == true)
					//{
					//	firstStringH = false;
					//}
					//else
					//{
						newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле
					//}

						bool isNewItur = false;
						newIturCode = record[3].Trim();
						newItur1.IturCode = newIturCode;

						//======================================== Itur ==============================
						if (importType.Contains(ImportDomainEnum.ImportItur) == true)
						{
							Itur newItur = new Itur();
							IturString newIturString = new IturString();

							newIturString.IturCode = newIturCode;
							newIturString.LocationCode = DomainUnknownCode.UnknownLocation;
							newIturString.StatusIturBit = "0";

							int retBitItur = newItur.ValidateError(newIturString, this._dtfi);
							if (retBitItur != 0)  //Error
							{
								this._errorBitList.Add(new BitAndRecord
								{
									Bit = retBitItur,
									Record = record.JoinRecord(separator),
									ErrorType = MessageTypeEnum.Error
								});
								continue;
							}
							else //	Error  retBit == 0 
							{
								retBitItur = newItur.ValidateWarning(newIturString, this._dtfi); //Warning
								if (iturFromDBDictionary.IsDictionaryContainsKey(newItur.IturCode) == false)
								{
									this._iturDictionary[newItur.IturCode] = newItur;
									iturFromDBDictionary[newItur.IturCode] = null;
									//this._iturDictionary.AddToDictionary(newItur.Code, newItur, record.JoinRecord(separator), Log);
									newIturCode = newItur.IturCode;
									isNewItur = true;
								}
								if (retBitItur != 0)
								{
									this._errorBitList.Add(new BitAndRecord
									{
										Bit = retBitItur,
										Record = record.JoinRecord(separator),
										ErrorType = MessageTypeEnum.WarningParser
									});
								}
							}
						}
						//========================================DocumentHeader==================
						//if (importType.Contains(ImportDomainEnum.ImportDocumentHeader) == true)
						//{
						DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
						DocumentHeader newDocumentHeader = new DocumentHeader();
						newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле
						newDocumentHeaderString.DocumentCode = newDocumentCode;		
						newDocumentHeaderString.SessionCode = newSessionCode;				//in
						newDocumentHeaderString.CreateDate = record[1];
						newWorkerID = record[2].Trim();
						newDocumentHeaderString.WorkerGUID = newWorkerID;
						newDocumentHeaderString.IturCode = newIturCode;//record[3];
						newDocumentHeaderString.Name = record[5];//record[4];

						int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
						if (retBitDocumentHeader != 0)  //Error
						{
							this._errorBitList.Add(new BitAndRecord
							{
								Bit = retBitDocumentHeader,
								Record = record.JoinRecord(separator),
								ErrorType = MessageTypeEnum.Error
							});
							continue;
						}
						else //	Error  retBitSession == 0 
						{
							retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
							if (isNewItur == true) newDocumentHeader.Approve = null;//false было //first Document in Itur
							IDDocumentHeader = Convert.ToInt32(this._documentHeaderRepository.Insert(newDocumentHeader, dbPath));

							if (retBitDocumentHeader != 0)
							{
								this._errorBitList.Add(new BitAndRecord {Bit = retBitDocumentHeader, 	Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
							}
						}
						//	}
					//} // if (firstStringH == true) end
				} // "H"
				//===========================InventProduct==============================

				//B,7290104724473,9,B,29/07/2011,08:10:47		old
				//0   1                      2 3       4              5     

				//B,7290000808406,23,,B,24/09/2012,20:23:33
				//Makat 1
				//QuantityOriginal 2
				// QuantityInPackEdit 3
				//B,7290008546775,5,10,B,24/09/2012,20:23:20
				//0   1                       2  3 4     5              6  
				else if (objectType == "B")
				{
					//============================// нет строки документа документа
					if (firstStringH == false)
					{
						firstStringH = true;
						Itur newItur = new Itur();
						newIturCode = "00010001";
						newItur.IturCode = newIturCode;
						newItur.Number = 1;
						newItur.NumberPrefix = "0001";
						newItur.NumberSufix = "0001";
						newItur.LocationCode = DomainUnknownCode.UnknownLocation;
						newItur.StatusIturBit = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
						if (iturFromDBDictionary.IsDictionaryContainsKey(newItur.IturCode) == false)
						{
							this._iturDictionary[newIturCode] = newItur;
							iturFromDBDictionary[newItur.IturCode] = null;
						}

						DocumentHeader newDocumentHeader = new DocumentHeader();
						newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле
						newDocumentHeader.DocumentCode = newDocumentCode;
						newDocumentHeader.SessionCode = newSessionCode;				//in
						newDocumentHeader.CreateDate = DateTime.Now;
						newDocumentHeader.StatusDocHeaderBit = (int)ConvertDataErrorCodeEnum.CodeIsEmpty;
						newWorkerID = "UnknownWorker";
						newDocumentHeader.WorkerGUID = newWorkerID;
						newDocumentHeader.IturCode = newIturCode;
						IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
					}
					//=================//end нет строки документа документа

					if (record.Length < 7)
					{
						Log.Add(MessageTypeEnum.Error,
							String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
						continue;
					}
					string makat = Convert.ToString(record[1]).Trim(" ".ToCharArray());
					string barcode = makat;
					InventProductSimpleString newInventProductString = new InventProductSimpleString();
					InventProduct newInventProduct = new InventProduct();
					newInventProductString.Makat = makat;
					newInventProductString.Barcode = barcode;
					newInventProductString.IturCode = newIturCode;
					newInventProductString.DocumentCode = newDocumentCode;
					newInventProductString.SessionCode = newSessionCode;
					newInventProductString.WorkerID = newWorkerID;
					newInventProductString.ProductName = "NotExistInCatalog";
					newInventProductString.QuantityOriginal = record[2];
					newInventProductString.QuantityInPackEdit = record[3];
					//newInventProductString.InputTypeCode = InputTypeCodeEnum.B.ToString();
					//newInventProductString.ImputTypeCodeFromPDA = record[3];
					newInventProductString.InputTypeCode = record[4];
					newInventProductString.CreateDate = record[5];
					newInventProductString.CreateTime = record[6];

					int retBitInventProduct = newInventProduct.ValidateError(newInventProductString, this._dtfi);
					if (retBitInventProduct != 0)  //Error
					{
						this._errorBitList.Add(new BitAndRecord
						{
							Bit = retBitInventProduct,
							Record = record.JoinRecord(separator),
							ErrorType = MessageTypeEnum.Error
						});
						continue;
					}

					retBitInventProduct = newInventProduct.ValidateWarning(newInventProductString, this._dtfi);
					if (retBitInventProduct != 0)
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitInventProduct, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
					}

					newInventProduct.IturCode = newIturCode;
					newInventProduct.DocumentCode = newDocumentCode;
					newInventProduct.SessionCode = newSessionCode;
					newInventProduct.WorkerID = newWorkerID;
					newInventProduct.SectionNum = 0;
					newInventProduct.DocNum = Convert.ToInt32(IDDocumentHeader);

					if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
					{
						//if (productMakatDictionary.ContainsKey(barcode) == false)
						//{
						//    Log.Add(String.Format(ParserFileErrorMessage.MakatAndBarcodeNotExistInDB, barcode));
						//    //continue;
						//}
						//else
						//{
						newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
						makat = productMakatDictionary.GetParentMakatFromMakatDictionary(barcode, Log);
						if (string.IsNullOrWhiteSpace(makat) == false)
						{
							if (makat == barcode) newInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
							else newInventProduct.TypeMakat = TypeMakatEnum.B.ToString();
							newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
							newInventProduct.Makat = makat;
							newInventProduct.ProductName = productMakatDictionary[makat].Name;
							newInventProduct.SectionCode = productMakatDictionary[makat].SectionCode;	
						}
						else
						{	// TODO: проверить
							newInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
							newInventProduct.Makat = barcode;
							newInventProduct.ProductName = "NotExistInCatalog";
							newInventProduct.StatusInventProductBit =
								newInventProduct.StatusInventProductBit
								+ (int)ConvertDataErrorCodeEnum.InvalidValue;
							newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
							//this._errorBitList.Add(new BitAndRecord
							//{
							//    Bit = (int)ConvertDataErrorCodeEnum.FKCodeIsEmpty,
							//    Record = record.JoinRecord(separator),
							//    ErrorType = MessageType.Warning
							//});
						}
						//}
					}		//ExistMakat
					else   //Not ExistMakat
					{
						newInventProduct.ProductName = "NotCheckInCatalog";
					}
					if (importType.Contains(ImportDomainEnum.ImportParentProductAdvanced) == true)
					{
						newInventProduct.SetAdvancedValue(record, indexInRecordDictionary, dbPath);
					}
					newInventProduct.Code = (newInventProduct.Makat + "^" + newInventProduct.Barcode).CutLength(299); 
					yield return newInventProduct;
					//}
					//else
					//{
					//    //если нет, то складываем в другой список - 
					//    //продукт без маката
					//}
				}
				else // != 'H' !='B' 
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedMarker, record.JoinRecord(separator)));
					continue;
				}

			}
		}

	
	}
}
