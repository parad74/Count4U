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
	public class InventProductDefaultBackupParser : InventProductParserBase, IInventProductSimpleParser
	{
		protected Dictionary<string, string> _iturInFileDictionary; //key IturCode, DocumentCode - для файла
		protected DocumentHeaderString _rowDocumentHeader;

		public InventProductDefaultBackupParser(
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
			string newDocumentCode = Guid.NewGuid().ToString(); 

			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			string newWorkerID = ""; 
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string newIturCode = "00010001";

			Dictionary<string, int> indexInRecordDictionary =	parms.GetIPAdvancedFieldIndexDictionaryFromParm();

			this._documentHeaderDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();
			this._iturInFileDictionary.Clear();

			string separator = "|";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

		
			//B|29270001|A01-01|111|28|M|05/07/2015|22:59:03
			//0   1                 2		 3     4   5     6					7
//Record:
//B																	0
//Field1: Itur Code											1
//Field2: Itur Code ERP									2
//Field3: Code Input from PDA						3
//Field4: Quantity											4
//Field5: Manual\Barcode – How inserted		5
//Field6: Date													6
//Field7: Time													7

		

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

				//string objectType = Convert.ToString(record[0]).Trim(" ".ToCharArray());
	
				//if (objectType == "H")
				//{
				//	string idPDA = record[1].Trim(" ".ToCharArray());
				//	this._rowDocumentHeader.Name = idPDA;
				//	this._rowDocumentHeader.WorkerGUID = idPDA.PadLeft(9, '0');
				//	this._rowDocumentHeader.CreateDate = record[3].Trim(" ".ToCharArray());
				//	this._rowDocumentHeader.CreateTime = record[4].Trim(" ".ToCharArray());

				//} // "H"


				
				//else if (objectType == "B")
				//{
					//if (countRecord < 7)
					//{
					//	Log.Add(MessageTypeEnum.Error,
					//		String.Format(ParserFileErrorMessage.NoMatchNumberSubstrings, record.JoinRecord(separator)));
					//	continue;
					//}

					newIturCode = record[4].Trim();
					int IDDocumentHeader = 0;
					bool isNewItur = false;

					//Line example: 0721740000351,18,30/12/2016,07:53:20,20160007,002
					//Explanation: 
					//Barcode,											0
					//Quantity, 											1
					//DD/MM/YYYY,								    2
					//HH:MM:SS,										3
					//IturCode,											4
					//PDANumber (3 digits)						5
					if (iturFromDBDictionary.IsDictionaryContainsKey(newIturCode) == false)
					{
						//======================================== Itur ==============================
						if (importType.Contains(ImportDomainEnum.ImportItur) == true)
						{
							Itur newItur = new Itur();
							IturString newIturString = new IturString();

							newIturString.IturCode = newIturCode;
							//newIturString.ERPIturCode = record[2].Trim();
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


					newDocumentCode = GetDocumentHeaderCodeByIturCode(ref IDDocumentHeader, iturFromDBDictionary, importType, dbPath, newIturCode, newSessionCode, isNewItur);
					//}//if (firstStringH == false)
					//=================//end нет строки документа документа
					//Barcode,											0
					//Quantity, 											1
					//DD/MM/YYYY,								    2
					//HH:MM:SS,										3
					//IturCode,											4
					//PDANumber (3 digits)						5
					string makat = Convert.ToString(record[0]).Trim(" ".ToCharArray());
					string barcode = makat;
					InventProductSimpleString newInventProductString = new InventProductSimpleString();
					InventProduct newInventProduct = new InventProduct();
					newInventProductString.Makat = makat;
					newInventProductString.Barcode = barcode;
					newInventProductString.IturCode = newIturCode;
					newInventProductString.DocumentCode = newDocumentCode;
					newInventProductString.SessionCode = newSessionCode;
					newInventProductString.WorkerID = record[5];
					newInventProductString.ProductName = "NotExistInCatalog";
					newInventProductString.QuantityOriginal = record[1];
					newInventProductString.InputTypeCode = InputTypeCodeEnum.B.ToString();
					//newInventProductString.ImputTypeCodeFromPDA = record[3];
					//newInventProductString.InputTypeCode = record[5];
					newInventProductString.CreateDate = record[2];
					newInventProductString.CreateTime = record[3];

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
				//}
				//else // != 'H' !='B' 
				//{
				//	Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.NoExpectedMarker, record.JoinRecord(separator)));
				//	continue;
				//}

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
