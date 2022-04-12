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
	public class InventProductWarehouseParser : InventProductParserBase, IInventProductSimpleParser
	{
		protected Dictionary<string, string> _iturInFileDictionary; //key IturCode, DocumentCode - для файла
		protected DocumentHeaderString _rowDocumentHeader;

		public InventProductWarehouseParser(
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base( documentHeaderRepository, serviceLocator, log)
		{
			this._iturInFileDictionary = new Dictionary<string, string>();
			this._rowDocumentHeader = new DocumentHeaderString();
		}

	
		public IEnumerable<InventProduct> GetInventProducts(
			string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString, string sessionCodeIn, //Guid workerGUID,
			Dictionary<string, ProductMakat> productMakatDictionary,
			Dictionary<string, Itur> iturFromDBDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			//bool firstStringH = false;
			//string newDocumentCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewDocumentCode);
			string newDocumentCode = Guid.NewGuid().ToString(); 

			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = ""; 
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string newIturCode = "00010001";
			string orderID = "";
	
			Dictionary<string, int> indexInRecordDictionary =	parms.GetIPAdvancedFieldIndexDictionaryFromParm();

			this._documentHeaderDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();
			this._iturInFileDictionary.Clear();

			Dictionary<string, Itur> erpIturDictionary = new Dictionary<string, Itur>();
	 		IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			if (string.IsNullOrWhiteSpace(dbPath) == false)
			{
				erpIturDictionary = iturRepository.GetERPIturDictionary(dbPath);
			}

			string prefix = "2016";
			string firstIturCode = parms.GetStringValueFromParm(ImportProviderParmEnum.IturCode);
			if (string.IsNullOrWhiteSpace(firstIturCode) == false)
			{
				if (firstIturCode.Length > 4)
				{
					prefix = firstIturCode.Substring(0, 4);
				}
				else
				{
					prefix = firstIturCode.PadLeft(4, '0');
				}
			}

			int number = iturRepository.GetMaxNumber(prefix, dbPath);

			string separator = ",";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			//H,17/03/2016,000000392,29000001,01,000001
			//0,					1,					2,			3,		4,		5
			//	Header:
			//Field1: Const  H								0
			//Field2: Date									1
			//Field3: Worker ID						2
			//Field4: NotInUse						    3
			//Field5: NotInUse						    4
			//Field6: PDA Num							5


			//B,3502034,43,B,17/03/2016,10:58:56,12345,
			//0,			   1 , 2, 3,				   4,		   5 ,	    6,
			//Record:
			//Field1: Const "B"							0
			//Field2: Code Input from PDA	    1
			//Field3: Quantity							2
			//Field4: Manual\Barcode – How inserted  3
			//Field5: Date									4
			//Field6: Time									5
			//Field7: Itur Code ERP					6
			//Field8: Itur Code							7 (optional)
			//	Field9: OrderID							 8(optional)

			foreach (String[] record in this._fileParser.GetRecords(fromPathFile, 
				encoding, separators,
				countExcludeFirstString))
			{
				if (record == null)	continue;
				int countRecord = record.Length;
				if (countRecord < 1)
				{
					Log.Add(MessageTypeEnum.Error, 
						String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					continue;
				}

				string objectType = Convert.ToString(record[0]).Trim(" ".ToCharArray());
				//H,17/03/2016,000000392,29000001,01,000001
				//0,					1,					2,			3,		4,		5
				//	Header:
				//Field1: Const  H								0
				//Field2: Date									1
				//Field3: Worker ID						2
				//Field4: NotInUse						    3
				//Field5: NotInUse						    4
				//Field6: PDA Num							5
				if (objectType == "H")
				{
					if (countRecord < 6)
					{
						Log.Add(MessageTypeEnum.Error,
							String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
						continue;
					}
					string idPDA = record[5].Trim(" ".ToCharArray());
					this._rowDocumentHeader.Name = idPDA.PadLeft(6, '0'); ;
					//this._rowDocumentHeader.WorkerGUID = idPDA.PadLeft(9, '0');
					this._rowDocumentHeader.CreateDate = record[1].Trim(" ".ToCharArray());
					this._rowDocumentHeader.WorkerGUID = record[2].Trim(" ".ToCharArray());

				} // "H"


				//===========================InventProduct==============================
				//B,3502034,43,B,17/03/2016,10:58:56,12345,
				//0,			   1 , 2, 3,				   4,		   5 ,	    6,
				//Record:
				//Field1: Const "B"							0
				//Field2: Code Input from PDA	    1
				//Field3: Quantity							2
				//Field4: Manual\Barcode – How inserted  3
				//Field5: Date									4
				//Field6: Time									5
				//Field7: Itur Code ERP					6
				//Field8: Itur Code							7 (optional)
				//	Field9: OrderID							 8(optional)

				else if (objectType == "B")
				{
					if (countRecord < 7)
					{
						Log.Add(MessageTypeEnum.Error,
							String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
						continue;
					}

					if (countRecord >= 8)
					{
						if (string.IsNullOrWhiteSpace(record[7]) == false)
						{
							newIturCode = record[7].Trim();
						}
					}

					if (countRecord >= 9)
					{
						if (string.IsNullOrWhiteSpace(record[8]) == false)
						{
							orderID = record[8].Trim();
						}
					}

					int IDDocumentHeader = 0;
					bool isNewItur = false;

					//if (iturFromDBDictionary.IsDictionaryContainsKey(newItur.IturCode) == false)
					//{
					//	this._iturDictionary[newIturCode] = newItur;
					//	iturFromDBDictionary[newItur.IturCode] = null;
					//}

					string iturCodeERP = record[6].Trim();
					//string iturCode = iturCodeERP;


					if (erpIturDictionary.ContainsKey(iturCodeERP) == true)
					{
						newIturCode = erpIturDictionary[iturCodeERP].IturCode;
					}
					else   // нет в БД Itur с таким iturCodeERP => создаем новый IturCode, для того чтобы добавить в БД Itur 
					{
						number++;
						newIturCode = prefix.PadLeft(4, '0') +number.ToString().PadLeft(4, '0');
						Itur it = new Itur();
						it.IturCode =  newIturCode;
						it.ERPIturCode =  iturCodeERP;
						erpIturDictionary[iturCodeERP] = it;
						//это еще не все в БД нет
					}

					if (iturFromDBDictionary.IsDictionaryContainsKey(newIturCode) == false)
					{
						//======================================== Itur ==============================
						if (importType.Contains(ImportDomainEnum.ImportItur) == true)
						{
							Itur newItur = new Itur();
							IturString newIturString = new IturString();

							newIturString.IturCode = newIturCode;
							newIturString.ERPIturCode = iturCodeERP;
							newIturString.LocationCode = DomainUnknownCode.UnknownLocation;
							newIturString.StatusIturBit = "0";


							int retBitItur = newItur.ValidateError(newIturString, this._dtfi);
							if (retBitItur != 0)  //Error
							{
								this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.Error });
							}
							else //	Error  retBit == 0 
							{
								retBitItur = newItur.ValidateWarning(newIturString, this._dtfi); //Warning
								this._iturDictionary[newItur.IturCode] = newItur;  // словарь для добавления в БД в репозитории после импорта всего файла
								iturFromDBDictionary[newItur.IturCode] = null;     // словарь для того чтобы не дублировать Itur.IturCode в БД
								newIturCode = newItur.IturCode;
								isNewItur = true;
								if (retBitItur != 0)
								{
									this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = record.JoinRecord(separator), ErrorType = MessageTypeEnum.WarningParser });
								}
							}
						}
					}// Itur

					//словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
					newDocumentCode = GetDocumentHeaderCodeByIturCode(ref IDDocumentHeader, iturFromDBDictionary, importType, dbPath, newIturCode, newSessionCode, isNewItur);
					//}//if (firstStringH == false)
					//=================//end нет строки документа документа

					//Field2: Code Input from PDA	    1
					//Field3: Quantity							2
					//Field4: Manual\Barcode – How inserted  3
					//Field5: Date									4
					//Field6: Time									5
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
					newInventProduct.IPValueStr2 = orderID;
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

		private string GetDocumentHeaderCodeByIturCode(
			ref int IDDocumentHeader,
			Dictionary<string, Itur> iturFromDBDictionary,
			List<ImportDomainEnum> importType,
			string dbPath,
			string newIturCode,
			string newSessionCode,
			bool isNewItur)
		{
			string retDocumentCode = "";
			if (this._iturInFileDictionary.ContainsKey(newIturCode) == true) //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
			{
				retDocumentCode = this._iturInFileDictionary[newIturCode];
				return retDocumentCode;
			}
			//========================================DocumentHeader==================
			else // create new DocumentHeader
			{
				string newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле

				DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
				DocumentHeader newDocumentHeader = new DocumentHeader();
				newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле
				newDocumentHeaderString.DocumentCode = newDocumentCode;
				newDocumentHeaderString.SessionCode = newSessionCode;				//in
				newDocumentHeaderString.CreateDate = this._rowDocumentHeader.CreateDate;
				newDocumentHeaderString.WorkerGUID = "UnknownWorker";
				newDocumentHeaderString.IturCode = newIturCode;
				newDocumentHeaderString.Name = this._rowDocumentHeader.Name;
				newDocumentHeaderString.WorkerGUID = this._rowDocumentHeader.WorkerGUID;

				int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
				if (retBitDocumentHeader != 0)  //Error
				{
					this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.Error });
				}
				else //	Error  retBitSession == 0 
				{
					retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
					if (isNewItur == true) newDocumentHeader.Approve = null;//false было  //first Document in Itur
					IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
					retDocumentCode = newDocumentCode;
					this._iturInFileDictionary[newIturCode] = newDocumentHeader.DocumentCode; //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader

					if (retBitDocumentHeader != 0)
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.WarningParser });
					}
				}
				return retDocumentCode;
			}

		}

		
	}
}
