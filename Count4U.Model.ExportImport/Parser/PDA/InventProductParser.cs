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
	public class InventProductParser : InventProductParserBase, IInventProductSimpleParser
	{
		public InventProductParser(
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
			Dictionary<string, Itur> iturFromDBDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool firstStringH = false;
			//string newDocumentCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewDocumentCode);
			string newDocumentCode = Guid.NewGuid().ToString(); 

			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = ""; 
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string newIturCode = "00010001";
			int IDDocumentHeader = 0;
			Dictionary<string, int> indexInRecordDictionary =	parms.GetIPAdvancedFieldIndexDictionaryFromParm();

			this._documentHeaderDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();

			string separator = ",";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			//H,29/07/2011,000000004,29170001,1,900006
			//0			1			  2				  3		 4		 5 
			//B,7290104724473,9,B,29/07/2011,08:10:47
		   //0   1                        2 3       4              5     
			foreach (String[] record in this._fileParser.GetRecords(fromPathFile, 
				encoding, separators,
				countExcludeFirstString))
			{
				if (record == null)	continue;
				int countRecord = record.Length;
				if (countRecord < 6)
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
								this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
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
									this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
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
						newWorkerID = record[2];
						newDocumentHeaderString.WorkerGUID = newWorkerID;
						newDocumentHeaderString.IturCode = newIturCode;//record[3];
						newDocumentHeaderString.Name = record[5];//record[4];
						
						int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
						if (retBitDocumentHeader != 0)  //Error
						{
							this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
							continue;
						}
						else //	Error  retBitSession == 0 
						{
							retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
							if (isNewItur == true) newDocumentHeader.Approve = null;//false было  //first Document in Itur
							IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));

							if (retBitDocumentHeader != 0)
							{
								this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
							}
						}
						//	}
					//}// if (firstStringH == true) end
				} // "H"

			
				//===========================InventProduct==============================
				//Makat is a unique string on DB
				//Makat can have multiple Barcodes (On the Barcode table)
				//PDA document of  ITUR can return on Barcode field either Makat or Barcode.
				//B,7290104724473,9,B,29/07/2011,08:10:47
				//0   1                        2 3       4              5     
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


					string makat = Convert.ToString(record[1]).Trim(" ".ToCharArray());
					string barcode = makat + "##" + newIturCode;
					//string barcode = makat;
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
					//newInventProductString.InputTypeCode = InputTypeCodeEnum.B.ToString();
					//newInventProductString.ImputTypeCodeFromPDA = record[3];
					newInventProductString.InputTypeCode = record[3];
					newInventProductString.CreateDate = record[4];
					newInventProductString.CreateTime = record[5];
					

					int retBitInventProduct = newInventProduct.ValidateError(newInventProductString, this._dtfi);
					if (retBitInventProduct != 0)  //Error
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitInventProduct, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
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
							newInventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
							newInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
						}
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
				}
				else // != 'H' !='B' 
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedMarker, record.JoinRecord(separator)));
					continue;
				}

			} //foreach record from file

		
		}

		
	}
}
