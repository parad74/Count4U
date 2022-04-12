using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Globalization;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	/// <summary>
	/// ?? old
	/// </summary>
	public class InventProductParser : IInventProductSimpleParser
	{
		private readonly IFileParser _fileParser;
		private readonly ILog _log;
		private Dictionary<string, DocumentHeader> _documentHeaderDictionary;
		private Dictionary<string, Session> _sessionDictionary;
		private Dictionary<string, Itur> _iturDictionary;
		//private string _workerDefault;
		//private Dictionary<string, InventProductSimple> _withoutMakatDictionary;
		private List<BitAndRecord> _errorBitList;
		public DateTimeFormatInfo _dtfi;

		public InventProductParser(IFileParser fileParser,
			ILog log)
		{
			if (fileParser == null) throw new ArgumentNullException("fileParser");

			this._fileParser = fileParser;
			this._log = log;
			this._documentHeaderDictionary = new Dictionary<string, DocumentHeader>();
			this._sessionDictionary = new Dictionary<string, Session>();
			this._iturDictionary = new Dictionary<string, Itur>();
			this._errorBitList = new List<BitAndRecord>();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
		}

		public ILog Log
		{
			get { return this._log; }
		}

		public Dictionary<string, DocumentHeader> DocumentHeaderDictionary
		{
			get { return this._documentHeaderDictionary; }
		}

		public Dictionary<string, Session> SessionDictionary
		{
			get { return this._sessionDictionary; }
		}

		public Dictionary<string, Itur> IturDictionary
		{
			get { return this._iturDictionary; }
		}

		public List<BitAndRecord> ErrorBitList
		{
			get { return this._errorBitList; }
		}


		/// <summary>
		/// Получение Dictionary объектов из документа 
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Dictionary<string, string>> GetDictionaryObject(string fromPathFile,
			Encoding encoding, string[] separators,
			int countExcludeFirstString)
		{
			SwitchObjectEnum switchFlag = SwitchObjectEnum.None;
			Dictionary<string, string> objectDB = new Dictionary<string, string>();
			string key = "OBJECTTYPE";
			//string filePath = Properties.Settings.Default.FilePath;
			foreach (String[] record in this._fileParser.GetRecords(fromPathFile, encoding,
				separators, countExcludeFirstString))
			{
				if (record == null)
				{
					Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.FileIsEmpty, fromPathFile));
					yield return null;
				}
				if (record.Length >= 1)
				{
					if (String.IsNullOrWhiteSpace(Convert.ToString(record[0]).Trim()) == false)
					{
						//=================== DOCUMENTSHEADER
						if (Convert.ToString(record[0]).Trim().ToUpper() == "DOCUMENTSHEADER")
						{
							//если был switchFlag == SwitchObjectEnum.DocumentHeader
							if (switchFlag == SwitchObjectEnum.DocumentHeader)
							{
								objectDB.Add(key, SwitchObjectEnum.DocumentHeader.ToString());
								//то сохранить данные в DocumentHeader 
								yield return objectDB;
							}

							//если был switchFlag == SwitchObjectEnum.Item
							else if (switchFlag == SwitchObjectEnum.Item)
							{
								objectDB.Add(key, SwitchObjectEnum.Item.ToString());
								// то сохранить данные в Item 
								yield return objectDB;
							}
							switchFlag = SwitchObjectEnum.DocumentHeader;
							objectDB.Clear();
						}
						//=================== INVENTORS
						else if (Convert.ToString(record[0]).Trim().ToUpper() == "INVENTORS")
						{
							//если был switchFlag == SwitchObjectEnum.DocumentHeader
							if (switchFlag == SwitchObjectEnum.DocumentHeader)
							{
								objectDB.Add(key, SwitchObjectEnum.DocumentHeader.ToString());
								//то сохранить данные в DocumentHeader 
								yield return objectDB;
							}

							//если был switchFlag == SwitchObjectEnum.Item
							else if (switchFlag == SwitchObjectEnum.Item)
							{
								objectDB.Add(key, SwitchObjectEnum.Item.ToString());
								//то сохранить данные в Item  
								yield return objectDB;
							}
							switchFlag = SwitchObjectEnum.InventProduct;
							objectDB.Clear();
						}
						//=================	ITEM

						else if (Convert.ToString(record[0]).ToUpper().Contains("ITEM"))
						{
							//если был switchFlag == SwitchObjectEnum.DocumentHeader
							if (switchFlag == SwitchObjectEnum.DocumentHeader)
							{
								objectDB.Add(key, SwitchObjectEnum.DocumentHeader.ToString());
								//то сохранить данные в DocumentHeader 
								yield return objectDB;
							}

							//если был switchFlag == SwitchObjectEnum.Item
							else if (switchFlag == SwitchObjectEnum.Item)
							{
								objectDB.Add(key, SwitchObjectEnum.Item.ToString());
								yield return objectDB;
								//то сохранить данные в Item  
							}
							switchFlag = SwitchObjectEnum.Item;
							objectDB.Clear();
						}
						//=================	Item
						else
						{
							if (record.Length >= 2)
							{
								//накапливать данные в словаре
								string key1 = Convert.ToString(record[0]).Trim().ToUpper();
								string val = record[1] != null ? Convert.ToString(record[1]).Trim().ToUpper() : String.Empty;
								objectDB.Add(key1, val);
							}
						}
					}
				}
			}	   // end foreach

			//если был switchFlag == SwitchObjectEnum.DocumentHeader
			if (switchFlag == SwitchObjectEnum.DocumentHeader)
			{
				objectDB.Add(key, SwitchObjectEnum.DocumentHeader.ToString());
				//то сохранить данные в DocumentHeader 
				yield return objectDB;
			}

			//если был switchFlag == SwitchObjectEnum.Item
			else if (switchFlag == SwitchObjectEnum.Item)
			{
				objectDB.Add(key, SwitchObjectEnum.Item.ToString());
				//то сохранить данные в Item  
				yield return objectDB;
			}
			objectDB.Clear();
		}

		public IEnumerable<InventProduct> GetInventProducts(string fromPathFile,
			Encoding encoding, string[] separators, int countExcludeFirstString,
			string sessionCodeIn,
			Dictionary<string, ProductMakat> productMakatDictionary,
			//Dictionary<string, ProductMakat> productBarcodeDictionary,
			Dictionary<string, Itur> iturDictionary,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			bool firstStringH = true;
			string newDocumentCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewDocumentCode);
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);
			//string newDocumentCode = Guid.NewGuid().ToString();
			string newIturCode = "";

			this._documentHeaderDictionary.Clear();
			this._sessionDictionary.Clear();
			this._errorBitList.Clear();
			this._iturDictionary.Clear();

			string separator = " ";

			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}

			foreach (Dictionary<string, string> inventProduct in
				this.GetDictionaryObject(fromPathFile, encoding, separators, countExcludeFirstString))
			{
				string objectType = inventProduct["OBJECTTYPE"];
				if ((SwitchObjectEnum)Enum.Parse(typeof(SwitchObjectEnum), objectType, true)
					== SwitchObjectEnum.DocumentHeader)
				{
					//========================================Session============================
					if (importType.Contains(ImportDomainEnum.ImportSession) == true)
					{
						Session newSession = new Session();
						SessionString newSessionString = new SessionString();
						//newSessionString.Code = "";
						try
						{
							newSessionString.SessionCode = inventProduct["SESSIONIID"];
							sessionCodeIn = newSessionString.SessionCode;
						}
						catch { }

						if (this._sessionDictionary.ContainsKey(newSessionString.SessionCode) == false)
						{
							int retBitSession = newSession.ValidateError(newSessionString, this._dtfi);
							if (retBitSession != 0)
							{
								this._errorBitList.Add(new BitAndRecord { Bit = retBitSession, Record = "SESSIONIID = " + newSessionString.SessionCode , ErrorType = MessageTypeEnum.Error});
							}
							else
							{
								retBitSession = newSession.ValidateWarning(newSessionString, this._dtfi);
								if (retBitSession != 0)
								{
									this._errorBitList.Add(new BitAndRecord { Bit = retBitSession, Record = "SESSIONIID = " + newSessionString.SessionCode, ErrorType = MessageTypeEnum.WarningParser });
								}
								this._sessionDictionary.Add(newSessionString.SessionCode, newSession);
							}
						}
						else
						{
							Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.SessionCodeExistInDB, "SESSIONIID = " + newSessionString.SessionCode));
						}
					}

					//========================================DocumentHeader==================
					if (importType.Contains(ImportDomainEnum.ImportDocumentHeader) == true)
					{
						newDocumentCode = inventProduct["IID"];
						DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
						DocumentHeader newDocumentHeader = new DocumentHeader();
						newDocumentHeaderString.DocumentCode = newDocumentCode.Trim();		//in
						newDocumentHeaderString.SessionCode = sessionCodeIn;					//in
						newIturCode = inventProduct["ITURUUD"];
						newDocumentHeaderString.IturCode = inventProduct["ITURUUD"];
						newDocumentHeaderString.Approve = inventProduct["APROVE"];
						newDocumentHeaderString.Name = inventProduct["NAME"];
						newDocumentHeaderString.Code = inventProduct["DOCUMENTCODE"];
	

						//inventProduct["STATUS"];

						if (this._documentHeaderDictionary.ContainsKey(newDocumentHeaderString.DocumentCode) == false)
						{
							int retBitDocumentHeader = newDocumentHeader.Validate(newDocumentHeaderString, this._dtfi);

							if (retBitDocumentHeader != 0)
							{
								this._errorBitList.Add(new BitAndRecord
								{
									Bit = retBitDocumentHeader,
									Record = "DOCUMENT IID = " + newDocumentHeaderString.DocumentCode,
									ErrorType = MessageTypeEnum.WarningParser
								});
							}
							else
							{
								this._documentHeaderDictionary.Add(newDocumentHeader.DocumentCode, newDocumentHeader);
							}
						}
						else
						{
							Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.DocumentCodeExistInDB, "DOCUMENT IID = " + newDocumentHeaderString.DocumentCode));
				  		}
					}
					//========================================Itur+++++++++++++==================
					if (importType.Contains(ImportDomainEnum.ImportItur) == true)
					{
						if (string.IsNullOrWhiteSpace(newIturCode) == false)
						{
							Itur newItur = new Itur();
							IturString newIturString = new IturString();
							newIturString.IturCode = newIturCode;
							newIturString.LocationCode = DomainUnknownCode.UnknownLocation;
							newIturString.StatusIturBit = "0";
							int retBitItur = newItur.Validate(newIturString, this._dtfi);
							//================== ExistItur == true =======================
							if (importType.Contains(ImportDomainEnum.ExistItur) == true)
							{
								if (retBitItur == 0)
								{
									if (iturDictionary.ContainsKey(newItur.IturCode) == false)
									{
										if (this._iturDictionary.ContainsKey(newItur.IturCode) == false)
										{
											this._iturDictionary.Add(newItur.IturCode, newItur);
										}
										//else
										//{
										//    Log.Add(MessageTypeEnum.Error, ParserFileErrorMessage.Warning + String.Format(ParserFileErrorMessage.IturCodeExistInDB, "ITURUUD = " + newIturCode));
										//}
										iturDictionary.Add(newItur.IturCode, newItur);
									}
									//else
									//{
									//    Log.Add(ParserFileErrorMessage.Warning + String.Format(ParserFileErrorMessage.IturCodeExistInDB, "ITURUUD = " + newIturCode));
									//}
								}
								else  //retBit != 0
								{
									this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = "ITURUUD = " + newIturCode, ErrorType = MessageTypeEnum.WarningParser });

									//if (iturDictionary.ContainsKey(newItur.Code) == true)
									//{
									//    this._errorBitList.Add(new BitAndRecord { Bit = (int)ConvertDataErrorCodeEnum.SameCodeExist, Record = "ITURUUD = " + newIturCode });
									//}
								}
							}
							//================== ExistItur == false =======================
							if (importType.Contains(ImportDomainEnum.ExistItur) == false)
							{
								if (retBitItur == 0)
								{
									if (this._iturDictionary.ContainsKey(newItur.IturCode) == false)
									{
										this._iturDictionary.Add(newItur.IturCode, newItur);
									}
									//else
									//{
									//    this._errorBitList.Add(new BitAndRecord { Bit = (int)ConvertDataErrorCodeEnum.SameCodeExist, Record = "ITURUUD = " + newItur.Code });
									//}
								}
								else  //retBit != 0
								{
									this._errorBitList.Add(new BitAndRecord { Bit = retBitItur, Record = "ITURUUD = " + newIturCode, ErrorType = MessageTypeEnum.WarningParser });
								}
							}
						}
					}
				//===========================InventProduct==============================
				}  //end if SwitchObjectEnum.DocumentHeader
				else if ((SwitchObjectEnum)Enum.Parse(typeof(SwitchObjectEnum), objectType, true)
					== SwitchObjectEnum.Item)
				{
					if (importType.Contains(ImportDomainEnum.ImportDocumentHeader) == true)
					{
						string makat = inventProduct["BARCODE"].Trim();
						string barcode = inventProduct["BARCODE"].Trim();
						//поверить, есть ли такой макат в словаре
						//if (productMakatDictionary.ContainsKey(makat) == true)
						//{
						InventProductSimpleString newInventProductString = new InventProductSimpleString();
						InventProduct newInventProduct = new InventProduct();
						newInventProductString.Makat = makat;
						newInventProductString.Barcode = barcode;
						newInventProductString.DocumentCode = inventProduct["DOCUMENTHEADERIID"];
						newInventProductString.ProductName = "NotExistInCatalog";
						newInventProductString.SerialNumber = inventProduct["SERIALNUMBER"];
						newInventProductString.QuantityOriginal = inventProduct["QUANTITYORIGINAL"];
						newInventProductString.InputTypeCode = InputTypeCodeEnum.B.ToString();		//TO DO
						newInventProductString.ImputTypeCodeFromPDA = inventProduct["INPUTTYPE"];
						newInventProductString.CreateDate = inventProduct["CREATEDATE"];
						//inventProduct["PARTIALPACKAGE"];
						//inventProduct["SHELFCODE"];
						//inventProduct["QUANTITYEDIT"]
						//inventProduct["QUANTITYDIFFERENCE"];

						int retBitInventProduct = newInventProduct.Validate(newInventProductString, this._dtfi);

						if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
						{
							if (productMakatDictionary.ContainsKey(barcode) == false)
							{
								Log.Add(MessageTypeEnum.Error, String.Format(ParserFileErrorMessage.MakatAndBarcodeNotExistInDB,
									"BARCODE = " + barcode));
							}
							else
							{
								newInventProduct.ProductName = productMakatDictionary.
									GetProductNameFromMakatDictionary(barcode);
							}
							//if (productMakatDictionary.ContainsKey(makat) == true)
							//{
							//    newInventProduct.ProductName = productMakatDictionary[makat].Name;
							//}
							//else	//makat not Exist in DB
							//{
							//    Log.Add(String.Format(ParserFileErrorMessage.MakadNotExistInDB, "BARCODE = " + makat));

							//    if (importType.Contains(ImportDomainEnum.ImportMakat) == true)
							//    {
							//        //add Makat in newproductMakatDictionary
							//    }
							//    continue;
							//}
						}
						else
						{
							newInventProduct.ProductName = "NotCheckInCatalog";
						}
						yield return newInventProduct;
					}
				}
					//foreach (KeyValuePair<string, string> keyValuePair in inventProduct)
					//{
					//    string key = keyValuePair.Key;
					//    string val = keyValuePair.Value;
					//   // в БД записать
					//}
			}	 //end foreach
		}

		//private string GetProductNameFromMakatDictionary(string barcode, 
		//    Dictionary<string, ProductMakat> productMakatDictionary) 
		//{
		//    if (productMakatDictionary.ContainsKey(barcode) == false)
		//    {
		//        return "";
		//    }

		//    string _barcode = barcode;
		//    while (string.IsNullOrWhiteSpace(productMakatDictionary[_barcode].Name) == true)
		//    {
		//        this._barcode = productMakatDictionary[_barcode].ParentMakat;
		//        if (productMakatDictionary.ContainsKey(this._barcode) == false)
		//        {
		//            return "";
		//        }
		//    }
		//    return productMakatDictionary[_barcode].Name;
		//}

		//private string GetParentMakatFromMakatDictionary(string makat,
		//Dictionary<string, ProductMakat> productMakatDictionary)
		//{
		//    if (productMakatDictionary.ContainsKey(makat) == false)
		//    {
		//        return "";
		//    }

		//    string _makat = makat;
		//    while (string.IsNullOrWhiteSpace(productMakatDictionary[_makat].ParentMakat) == true)
		//    {
		//        this._makat = productMakatDictionary[_makat].ParentMakat;
		//        if (productMakatDictionary.ContainsKey(this._makat) == false)
		//        {
		//            return "";
		//        }
		//    }
		//    return this._makat;
		//}
	}
}


	


	

